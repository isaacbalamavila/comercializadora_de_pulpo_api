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

            int total = await query.CountAsync();

            var sales = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SaleDTO
                {
                    Id = s.Id,
                    Client = s.Client.Name,
                    Employee = s.Employee.Name + " " + s.Employee.LastName,
                    Date = s.SaleDate,
                    Total = s.TotalAmount,
                })
                .OrderByDescending(s => s.Date)
                .ToListAsync();

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
                .Sales.Select(s => new SaleResponse
                {
                    Id = s.Id,
                    Client = s.Client.Name,
                    ClientRfc = s.Client.Rfc,
                    Employee = s.Employee.Name + " " + s.Employee.LastName,
                    PaymentMethod = s.PaymentMethodNavigation.Name,
                    SaleDate = s.SaleDate,
                    Total = s.TotalAmount,
                    Products = s
                        .SaleItems.Select(s => new SaleItemResponse
                        {
                            SaleId = saleId,
                            Sku = s.Product.Sku,
                            Name = s.Product.Name,
                            Content = s.Product.Content,
                            Unit = s.Product.Unit.Abbreviation,
                            Price = s.Product.Price,
                            Quantity = s.Quantity,
                            Subtotal = s.Subtotal,
                        })
                        .Where(s => s.SaleId == saleId)
                        .ToList(),
                })
                .Where(s => s.Id == saleId)
                .FirstOrDefaultAsync();
        }
    }
}
