using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;
using comercializadora_de_pulpo_api.Repositories;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;

namespace comercializadora_de_pulpo_api.Services
{
    public class PurchaseService(
        ComercializadoraDePulpoContext context,
        ISuppliesInventoryRepository suppliesRepository,
        IPurchaseRepository purchaseRepository,
        IRawMaterialsRepository rawMaterialsRepository,
        ISuppliersRepository suppliersRepository,
        IUserRepository userRepository,
        IMapper mapper
    ) : IPurchaseService
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly ISuppliesInventoryRepository _suppliesRepository = suppliesRepository;
        private readonly IPurchaseRepository _purchaseRepository = purchaseRepository;
        private readonly IRawMaterialsRepository _rawMaterialsRepository = rawMaterialsRepository;
        private readonly ISuppliersRepository _suppliersRepository = suppliersRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<PurchaseResponseDTO>> GetPurchasesAsync(
            PurchaseRequestDTO request
        )
        {
            var purchasesPaginated = await _purchaseRepository.GetPurchasesAsync(request);

            return Response<PurchaseResponseDTO>.Ok(purchasesPaginated);
        }

        public async Task<Response<PurchaseDetailsDTO>> GetPurchaseByIdAsync(Guid id)
        {
            var purchaseSaved = await _purchaseRepository.GetPurchaseById(id);

            if (purchaseSaved == null)
                return Response<PurchaseDetailsDTO>.Fail(
                    "Compra no encontrada",
                    $"No se encontró una compra con el ID '{id}'",
                    404
                );

            return Response<PurchaseDetailsDTO>.Ok(_mapper.Map<PurchaseDetailsDTO>(purchaseSaved));
        }

        public async Task<Response<PurchaseDTO>> CreatePurchase(CreatePurchaseDTO request)
        {
            var yucatanTZ = TimeZoneInfo.FindSystemTimeZoneById("America/Merida");
            var nowYucatan = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, yucatanTZ);
            var cutoff = new TimeSpan(15, 0, 0); // 15:00

            if (nowYucatan.TimeOfDay > cutoff)
            {
                return Response<PurchaseDTO>.Fail(
                    "Compra fuera del horario permitido",
                    "Las compras solo pueden registrarse antes de las 15:00 (hora de Yucatán).",
                    400
                );
            }

            var savedUser = await _userRepository.GetUserByIdAsync(request.UserId);

            if (savedUser == null || savedUser.IsDeleted)
                return Response<PurchaseDTO>.Fail(
                    "Usuario inválido",
                    $"El usuario proporcionado no es válido, no existe o esta dado de baja",
                    400
                );

            if (request.TotalKg <= 0)
                return Response<PurchaseDTO>.Fail(
                    "No se pueden registrar compras vacías",
                    $"No se pueden registrar compras sin contenido para almacenar",
                    400
                );

            if (request.TotalPrice <= 0)
                return Response<PurchaseDTO>.Fail(
                    "No se pueden registrar compras sin precio válido",
                    $"Una compra debe tener un precio válido (Mayor a 0)",
                    400
                );

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var supplier = await _suppliersRepository.GetSupplierByIdAsync(request.SupplierId);
                if (supplier == null)
                    return Response<PurchaseDTO>.Fail(
                        "Proveedor no encontrado",
                        $"No se encontró un proveedor con el ID '{request.SupplierId}'",
                        400
                    );

                if (supplier.IsDeleted)
                    return Response<PurchaseDTO>.Fail(
                        "Proveedor no disponible",
                        $"El proveedor se encuentra dado de baja y con las compras restringidas",
                        400
                    );

                var rawMaterial = await _rawMaterialsRepository.GetRawMaterialByIdAsync(
                    request.RawMaterialId
                );
                if (rawMaterial == null)
                    return Response<PurchaseDTO>.Fail(
                        "Materia Prima no encontrada",
                        $"No se encontró una materia prima con el ID '{request.RawMaterialId}'",
                        400
                    );

                var now = DateTime.Now;

                var purchase = new Purchase
                {
                    Id = Guid.NewGuid(),
                    Sku = await GeneratePurchaseSKUAsync(now),
                    UserId = request.UserId,
                    RawMaterialId = request.RawMaterialId,
                    RawMaterial = rawMaterial,
                    SupplierId = request.SupplierId,
                    Supplier = supplier,
                    TotalPrice = request.TotalPrice,
                    TotalKg = request.TotalKg,
                    PriceKg = request.TotalPrice / request.TotalKg,
                    CreatedAt = now,
                };

                var purchaseRequest = await _purchaseRepository.CreatePurchaseAsync(purchase);
                if (!purchaseRequest.IsSuccess)
                    return Response<PurchaseDTO>.Fail(
                        "Ocurrió un error al intentar registrar la compra",
                        purchaseRequest.Error!.ErrorDetails
                    );

                var supplie = new SuppliesInventory
                {
                    Id = Guid.NewGuid(),
                    Sku = await GenerateSupplieSKUAsync(rawMaterial.Abbreviation, now),
                    PurchaseDate = now,
                    ExpirationDate = now.AddMonths(10),
                    PurchaseId = purchase.Id,
                    RawMaterialId = request.RawMaterialId,
                    WeightKg = request.TotalKg,
                    WeightRemainKg = request.TotalKg,
                };

                var supplieRequest = await _suppliesRepository.CreateSupplieAsync(supplie);
                if (!supplieRequest.IsSuccess)
                    return Response<PurchaseDTO>.Fail(
                        "Ocurrió un error al intentar registrar el insumo",
                        supplieRequest.Error!.ErrorDetails
                    );

                await transaction.CommitAsync();

                return Response<PurchaseDTO>.Ok(_mapper.Map<PurchaseDTO>(purchase), 201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<PurchaseDTO>.Fail("Error inesperado", ex.Message);
            }
        }

        //Helper Functions
        private async Task<string> GeneratePurchaseSKUAsync(DateTime date)
        {
            var total = await _purchaseRepository.GetTotalbyDateAsync(date);
            return $"COMP-{date:dd}{date:MM}{date:yy}-{(total + 1):d2}";
        }

        private async Task<string> GenerateSupplieSKUAsync(
            string rawMaterialAbbrevitation,
            DateTime date
        )
        {
            var total = await _suppliesRepository.GetTotalbyDateAsync(date);
            return $"{rawMaterialAbbrevitation}-{date:dd}{date:MM}{date:yy}-{(total + 1):d2}";
        }
    }
}
