using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class PurchasesRepository(ComercializadoraDePulpoContext context, IMapper mapper)
        : IPurchaseRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> GetTotalbyDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context
                .Purchases.Where(sp => sp.CreatedAt >= start && sp.CreatedAt < end)
                .CountAsync();
        }

        public async Task<PurchaseResponseDTO> GetPurchasesAsync(PurchaseRequestDTO request)
        {
            IQueryable<Purchase> query = _context.Purchases.AsQueryable();

            if (request.Supplier.HasValue)
                query = query.Where(p => p.SupplierId == request.Supplier);

            if (request.RawMaterial.HasValue)
                query = query.Where(p => p.RawMaterialId == request.RawMaterial);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(p => p.Sku.Contains(search));
            }

            if (request.Date.HasValue)
            {
                var start = request.Date.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(p => p.CreatedAt >= start && p.CreatedAt < end);
            }

            int total = await query.CountAsync();

            var purchases = await query
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.CreatedAt)
                .Skip(( request.Page - 1 ) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PurchaseResponseDTO
            {
                Page = request.Page,
                Total = total,
                TotalPages = ( int )Math.Ceiling(( double )total / request.PageSize),
                Purchases = _mapper.Map<List<PurchaseDTO>>(purchases),
            };
        }

        public async Task<Purchase?> GetPurchaseById(Guid id)
        {
            return await _context
                .Purchases.Include(p => p.Supplier)
                .Include(p => p.RawMaterial)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Response<Purchase>> CreatePurchaseAsync(Purchase newPurchase)
        {
            try
            {
                await _context.Purchases.AddAsync(newPurchase);
                await _context.SaveChangesAsync();
                return Response<Purchase>.Ok(newPurchase, 201);
            }
            catch (Exception ex)
            {
                return Response<Purchase>.Fail(
                    "Ocurrió un error al intentar registrar la compra",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }
    }
}
