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

            if (request.Search != null)
            {
                var search = request.Search!.Replace(" ", "").ToLower();
                query = query.Where(p =>
                    p.Sku.ToLower().Contains(search)
                    || p.TotalPrice.ToString().Contains(search)
                    || p.PriceKg.ToString().Contains(search)
                    || p.TotalKg.ToString().Contains(search)
                );
            }

            if (request.RawMaterial.HasValue)
                query = query.Where(p => p.RawMaterialId == request.RawMaterial);

            if (request.Date.HasValue)
                query = query.Where(p => p.CreatedAt.Date == request.Date.Value.Date);

            int total = await query.CountAsync();

            query = query
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.CreatedAt);

            var purchases = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            var response = new PurchaseResponseDTO
            {
                Total = total,
                TotalPages = (int)Math.Ceiling((double)total / request.PageSize),
                Page = request.Page,
                Purchases = _mapper.Map<List<PurchaseDTO>>(purchases),
            };
            return response;
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
