using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Sale;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class SalesRepository(ComercializadoraDePulpoContext context) : ISalesRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;

        public async Task<SalesResponseDTO> GetSalesAsync(SalesRequestDTO request)
        {
            var query = _context.Sales.AsQueryable();

            if (request.ClientId.HasValue)
                query = query.Where(s => s.ClientId == request.ClientId);

            if (request.EmployeeId.HasValue)
                query = query.Where(s => s.EmployeeId == request.EmployeeId);

            if (request.Date.HasValue)
            {
                var start = request.Date.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(s => s.SaleDate >= start && s.SaleDate < end);
            }

            var salesList = await query
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new SaleDTO
                {
                    Id = s.Id,
                    Client = s.Client != null ? s.Client.Name : "Sin cliente",
                    Employee = $"{s.Employee.Name} {s.Employee.LastName}",
                    PaymentMethod = s.PaymentMethodNavigation.Name,
                    Date = s.SaleDate,
                    Total = s.TotalAmount,
                })
                .ToListAsync();

            int total = salesList.Count;
            var sales = salesList
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new SalesResponseDTO
            {
                Page = request.Page,
                Total = total,
                TotalPages = (int)Math.Ceiling((double)total / request.PageSize),
                Sales = sales,
            };
        }

        public async Task<SaleResponse?> GetSaleDetailsByIdAsync(Guid saleId)
        {
            return await _context
                .Sales.Where(s => s.Id == saleId) // ✅ Filtrar PRIMERO
                .Select(s => new SaleResponse
                {
                    Id = s.Id,
                    Client = s.Client.Name,
                    ClientRfc = s.Client.Rfc,
                    Employee = $"{s.Employee.Name} {s.Employee.LastName}",
                    PaymentMethod = s.PaymentMethodNavigation.Name,
                    Date = s.SaleDate,
                    Total = s.TotalAmount,
                    Products = s
                        .SaleItems.Select(si => new SaleItemResponse
                        {
                            SaleId = s.Id,
                            Sku = si.Product.Sku,
                            Name = si.Product.Name,
                            Content = si.Product.Content,
                            Unit = si.Product.Unit.Abbreviation,
                            Price = si.Product.Price,
                            Quantity = si.Quantity,
                            Subtotal = si.Subtotal,
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }
    }
}
