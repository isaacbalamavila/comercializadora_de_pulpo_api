using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Proccess;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Services
{
    public class ProcessService(
        ComercializadoraDePulpoContext context,
        IProcessRepository processRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        ISuppliesInventoryRepository suppliesRepository,
        IProductSupplyRepository productSupplyRepository,
        IProductInventoryRepository productInventoryRepository,
        IMapper mapper
    ) : IProcessService
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly IProcessRepository _processRepository = processRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ISuppliesInventoryRepository _suppliesRepository = suppliesRepository;
        private readonly IProductSupplyRepository _productSupplyRepository =
            productSupplyRepository;
        private readonly IProductInventoryRepository _productInventoryRepository =
            productInventoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<ProcessResponseDTO>> GetProcessAsync(ProcessRequestDTO request)
        {
            return await _processRepository.GetProcessesAsync(request);
        }

        public async Task<Response<ProcessDetailsDTO>> GetProccessByIdAsync(Guid processId)
        {
            var processSaved = await _processRepository.GetProcessByIdAsync(processId);

            if (processSaved == null)
                return Response<ProcessDetailsDTO>.Fail(
                    "Proceso no encontrado",
                    $"No se encontró un proceso con el ID '{processId}'",
                    404
                );

            var processDetails = new ProcessDetailsDTO
            {
                Id = processId,
                StartDate = processSaved.StartDate,
                EndDate = processSaved.EndDate,
                Quantity = processSaved.Quantity,
                Status = new StatusProcess
                {
                    Id = processSaved.Status.Id,
                    Label = processSaved.Status.Label,
                },
                Product = new ProductProcess
                {
                    SKU = processSaved.Product.Sku,
                    Name = processSaved.Product.Name,
                    RawMaterial = processSaved.Product.RawMaterial.Name,
                    RawMaterialNeededKg = processSaved.Product.RawMaterialNeededKg,
                    Content = processSaved.Product.Content,
                    Unit = processSaved.Product.Unit.Label!,
                },
                User = new UserProcess
                {
                    Name = processSaved.User.Name,
                    LastName = processSaved.User.LastName,
                },
            };

            return Response<ProcessDetailsDTO>.Ok(processDetails);
        }

        public async Task<Response<ProductionProcessDetailsDTO>> GetProductionProcessDetailsDTO(
            Guid processId
        )
        {
            var processSaved = await _processRepository.GetProductionProcessDetails(processId);

            if (processSaved == null)
                return Response<ProductionProcessDetailsDTO>.Fail(
                    "Proceso no encontrado",
                    $"No se encontró un proceso con el ID '{processId}'",
                    404
                );

            return Response<ProductionProcessDetailsDTO>.Ok(processSaved);
        }

        public async Task<Response<ProcessDTO>> CreateProcessAsync(CreateProcessDTO request)
        {
            if (request.Quantity <= 0)
                return Response<ProcessDTO>.Fail(
                    "Cantidad inválida",
                    "La cantidad mínima para producir es una unidad",
                    400
                );

            var userSaved = await _userRepository.GetUserByIdAsync(request.UserId);

            if (userSaved == null)
                return Response<ProcessDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{request.UserId}'",
                    400
                );

            var productSaved = await _productRepository.GetProductByIdAsync(request.ProductId);

            if (productSaved == null)
                return Response<ProcessDTO>.Fail(
                    "Producto no encontrado",
                    $"No se encontró un producto con el ID '{request.ProductId}'",
                    400
                );

            // Schedule Validation
            var yucatanTZ = TimeZoneInfo.FindSystemTimeZoneById("America/Merida");
            var nowYucatan = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, yucatanTZ);

            var endTime = nowYucatan.AddMinutes(productSaved.TimeNeededMin * request.Quantity);

            var cutoff = nowYucatan.Date.AddHours(17).AddMinutes(45); // 5:45 PM

            if (endTime > cutoff)
            {
                return Response<ProcessDTO>.Fail(
                    "El proceso supera el horario laboral",
                    "El tiempo necesario para producir el lote supera el horario laboral",
                    400
                );
            }

            var supplies = await _suppliesRepository.GetSuppliesForProductionAsync(
                productSaved.RawMaterialId
            );

            if (supplies.Count == 0)
                return Response<ProcessDTO>.Fail(
                    "Insumos insuficientes",
                    "No existen insumos disponibles para esta producción",
                    400
                );

            var totalRawMaterialNeededKg = productSaved.RawMaterialNeededKg * request.Quantity;
            var processId = Guid.NewGuid();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var supply in supplies)
                {
                    if (totalRawMaterialNeededKg <= 0)
                        break;

                    var available = supply.WeightRemainKg;
                    var toConsume = Math.Min(available, totalRawMaterialNeededKg);

                    supply.WeightRemainKg -= toConsume;
                    totalRawMaterialNeededKg -= toConsume;

                    _context.SuppliesInventories.Update(supply);

                    await _context.ProductBatchSupplies.AddAsync(
                        new ProductBatchSupply
                        {
                            Id = Guid.NewGuid(),
                            ProductionProcessId = processId,
                            SuppliesInventoryId = supply.Id,
                            UsedWeightKg = toConsume,
                        }
                    );
                }

                if (totalRawMaterialNeededKg > 0)
                {
                    await transaction.RollbackAsync();
                    return Response<ProcessDTO>.Fail(
                        "Insumos insuficientes",
                        "No hay suficiente materia prima para producir la cantidad solicitada",
                        400
                    );
                }

                var newProcess = new ProductionProcess
                {
                    Id = processId,
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    StartDate = DateTime.Now,
                    StatusId = 1,
                    Product = productSaved,
                    User = userSaved,
                };

                await _context.ProductionProcesses.AddAsync(newProcess);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<ProcessDTO>.Ok(_mapper.Map<ProcessDTO>(newProcess));
            }
            catch (Exception ex)
            {
                return Response<ProcessDTO>.Fail(
                    "Ocurrió un error al intentar crear el proceso",
                    ex.Message,
                    500
                );
            }
        }

        public async Task<Response<ProcessDTO>> StartProcessAsync(Guid processId, string? userId)
        {
            if (userId == null)
                return Response<ProcessDTO>.Fail(
                    "Id del usuario no proporcionado",
                    $"No se encontró el header 'userId' con la información del usuario",
                    404
                );

            var processSaved = await _processRepository.GetProcessByIdAsync(processId);

            if (processSaved == null)
                return Response<ProcessDTO>.Fail(
                    "Proceso no encontrado",
                    $"No se encontró un proceso con el ID : '{processId}'",
                    404
                );

            if (processSaved.StatusId != 1)
                return Response<ProcessDTO>.Fail(
                    "Operación no permitida",
                    "No es posible iniciar el proceso porque ya se encuentra en curso",
                    400
                );

            var userSaved = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));

            if (userSaved == null)
                return Response<ProcessDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID : '{userId!}'",
                    400
                );

            // Update Process Fields
            processSaved.StatusId = 2; // <- Hardcoded;
            processSaved.UserId = Guid.Parse(userId);

            var updateRequest = await _processRepository.UpdateProcessAsync(processSaved);

            return updateRequest.IsSuccess
                ? Response<ProcessDTO>.Ok(_mapper.Map<ProcessDTO>(updateRequest.Data))
                : Response<ProcessDTO>.Fail(
                    "Ocurrió un error al iniciar el proceso",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<ProcessDTO>> EndProcessAsync(Guid processId, string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Response<ProcessDTO>.Fail(
                    "Usuario no proporcionado",
                    "No se encontró el header 'userId' con la información del usuario",
                    400
                );

            if (!Guid.TryParse(userId, out var userGuid))
                return Response<ProcessDTO>.Fail(
                    "Usuario inválido",
                    "El formato del userId no es válido",
                    400
                );

            var processSaved = await _processRepository.GetProcessByIdAsync(processId);
            if (processSaved == null)
                return Response<ProcessDTO>.Fail(
                    "Proceso no encontrado",
                    "No se encontró el proceso solicitado",
                    404
                );

            var userSaved = await _userRepository.GetUserByIdAsync(userGuid);
            if (userSaved == null)
                return Response<ProcessDTO>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{userId}'",
                    404
                );

            if (processSaved.UserId != userGuid)
                return Response<ProcessDTO>.Fail(
                    "Usuario inválido",
                    "El usuario que realiza la petición no es el mismo que inició el proceso",
                    403
                );

            if (processSaved.StatusId != 2)
                return Response<ProcessDTO>.Fail(
                    "Operación no permitida",
                    "El proceso no se encuentra en curso",
                    400
                );

            var productBatchesSupply = await _context
                .ProductBatchSupplies.Where(p => p.ProductionProcessId == processSaved.Id)
                .ToListAsync();

            if (!productBatchesSupply.Any())
                return Response<ProcessDTO>.Fail(
                    "Lotes no encontrados",
                    "No se encontraron insumos relacionados al proceso",
                    404
                );

            var rawMaterialNeeded = await _context
                .Products.Where(p => p.Id == processSaved.ProductId)
                .Select(p => p.RawMaterialNeededKg)
                .FirstOrDefaultAsync();

            if (rawMaterialNeeded <= 0)
                return Response<ProcessDTO>.Fail(
                    "Producto no encontrado",
                    "No se encontró un producto válido relacionado al proceso",
                    404
                );

            await using var transaction = await _context.Database.BeginTransactionAsync();
            int batchSupply = 1;

            try
            {
                foreach (var productSupply in productBatchesSupply)
                {
                    var supply = await _suppliesRepository.GetSupplyByIdAsync(
                        productSupply.SuppliesInventoryId
                    );

                    if (supply == null)
                    {
                        await transaction.RollbackAsync();
                        return Response<ProcessDTO>.Fail(
                            "Insumo no encontrado",
                            "Uno de los insumos usados en el proceso no existe",
                            404
                        );
                    }

                    var productBatchId = Guid.NewGuid();
                    var totalPieces = productSupply.UsedWeightKg / rawMaterialNeeded;
                    var productBatch = new ProductBatch
                    {
                        Id = productBatchId,
                        ProductionProcessId = processSaved.Id,
                        ProductId = processSaved.ProductId,
                        Sku = await GenerateSKU(batchSupply),
                        Quantity = (int)totalPieces,
                        Remain = (int)totalPieces,
                        CreatedAt = DateTime.Now,
                        ExpirationDate = supply.ExpirationDate,
                    };
                    await _context.ProductBatches.AddAsync(productBatch);

                    productSupply.ProductBatchId = productBatchId;

                    _context.ProductBatchSupplies.Update(productSupply);

                    batchSupply++;
                }

                processSaved.StatusId = 3; // Finalizado
                processSaved.UserId = userGuid;
                processSaved.EndDate = DateTime.Now;

                _context.ProductionProcesses.Update(processSaved);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<ProcessDTO>.Ok(_mapper.Map<ProcessDTO>(processSaved));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return Response<ProcessDTO>.Fail("Error al finalizar el proceso", ex.Message, 500);
            }
        }

        private async Task<string> GenerateSKU(int batchSupply)
        {
            var date = DateTime.Now;
            var total = await _productInventoryRepository.GetTotalbyDateAsync(date);

            return $"PROD-{date:ddMMyy}-{total + batchSupply:D4}";
        }
    }
}
