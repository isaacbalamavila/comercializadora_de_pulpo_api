using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class SuppliesInventoryRespository(
        ComercializadoraDePulpoContext context,
        IMapper mapper
    ) : ISuppliesInventoryRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> GetTotalbyDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context
                .SuppliesInventories.Where(sp => sp.PurchaseDate >= start && sp.PurchaseDate < end)
                .CountAsync();
        }

        public async Task<SuppliesResponseDTO> GetSuppliesAsync(SuppliesRequestDTO request)
        {
            IQueryable<SuppliesInventory> query = _context.SuppliesInventories.AsQueryable();

            if (request.RawMaterial.HasValue)
                query = query.Where(sp => sp.RawMaterialId == request.RawMaterial);

            if (request.Availables.HasValue)
                query = (bool)request.Availables
                    ? query.Where(sp => sp.WeightRemainKg > 0)
                    : query.Where(sp => sp.WeightRemainKg == 0);

            if (!String.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(sp => sp.Sku.StartsWith(search));
            }

            int total = await query.CountAsync();

            query = query.Include(sp => sp.RawMaterial).OrderBy(sp => sp.ExpirationDate);

            var supplies = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var response = new SuppliesResponseDTO
            {
                Page = request.Page,
                Total = total,
                TotalPages = ( int )Math.Ceiling(( double )total / request.PageSize),
                Supplies = _mapper.Map<List<SupplyDTO>>(supplies),
            };

            return response;
        }

        public async Task<SuppliesInventory?> GetSupplyByIdAsync(Guid id)
        {
            return await _context
                .SuppliesInventories
                .Include(sp => sp.RawMaterial)
                .Include(sp => sp.Purchase)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<Response<SuppliesInventory>> CreateSupplieAsync(
            SuppliesInventory newSupplier
        )
        {
            try
            {
                await _context.SuppliesInventories.AddAsync(newSupplier);
                await _context.SaveChangesAsync();
                return Response<SuppliesInventory>.Ok(newSupplier, 201);
            }
            catch (Exception ex)
            {
                return Response<SuppliesInventory>.Fail(
                    "Ocurrió un error al intentar registrar el insumo",
                    ex.Message
                );
            }
        }

        public async Task<Response<SuppliesInventory>> UpdateSupplyAsync(SuppliesInventory UpdatedSupply)
        {
            try
            {
                _context.SuppliesInventories.Update(UpdatedSupply);
                await _context.SaveChangesAsync();
                return Response<SuppliesInventory>.Ok(UpdatedSupply, 200);
            }
            catch (Exception ex)
            {
                return Response<SuppliesInventory>.Fail(
                    "Ocurrió un error al intentar actualizar la información del producto",
                    ex.Message
                );
            }
        }
    }
}
