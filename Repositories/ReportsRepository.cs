using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Reports;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class ReportsRepository(ComercializadoraDePulpoContext context) : IReportRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;

        public async Task<SaleReportResponseDTO> GetSalesReportAsync(ReportRequestDTO request)
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1);

            var sales = await _context
                .Sales.Include(s => s.Client)
                .Include(s => s.Employee)
                .Include(s => s.PaymentMethodNavigation)
                .Where(s => s.SaleDate >= startDate && s.SaleDate < endDate)
                .Select(s => new SaleReportItem
                {
                    Client = s.Client.Name,
                    Employee = $"{s.Employee.Name} {s.Employee.LastName}",
                    Date = s.SaleDate,
                    PaymentMethod = s.PaymentMethodNavigation.Name,
                    Total = s.TotalAmount,
                })
                .ToListAsync();

            return new SaleReportResponseDTO
            {
                GenerationDate = DateTime.Now,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Sales = sales,
            };
        }

        public async Task<ClientsReportResponseDTO> GetClientReportAsync(ReportRequestDTO request)
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1);

            var clients = await _context
                .Clients.Where(c => c.CreatedAt >= startDate && c.CreatedAt < endDate)
                .Select(c => new ClientReportItem
                {
                    Name = c.Name,
                    Email = c.Email,
                    CreatedAt = c.CreatedAt,
                    Phone = c.Phone,
                    Rfc = c.Rfc,
                })
                .ToListAsync();

            return new ClientsReportResponseDTO
            {
                GenerationDate = DateTime.Now,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Clients = clients,
            };
        }

        public async Task<ProductsReportResponseDTO> GetProductReportAsync(ReportRequestDTO request)
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1);

            var products = await _context
                .ProductBatches.Where(pb => pb.CreatedAt >= startDate && pb.CreatedAt < endDate)
                .GroupBy(pb => new
                {
                    pb.ProductId,
                    pb.Product.Sku,
                    pb.Product.Name,
                    pb.Product.Content,
                    UnitAbbreviation = pb.Product.Unit.Abbreviation,
                })
                .Select(g => new ProductReportItem
                {
                    Sku = g.Key.Sku,
                    Name = g.Key.Name,
                    Content = g.Key.Content,
                    Unit = g.Key.UnitAbbreviation,
                    Quantity = g.Sum(pb => pb.Quantity),
                    Remain = g.Sum(pb => pb.Remain),
                })
                .ToListAsync();

            return new ProductsReportResponseDTO
            {
                GenerationDate = DateTime.Now,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Products = products,
            };
        }

        public async Task<SuppliesReportResponseDTO> GetSuppliesReportAsync(
            ReportRequestDTO request
        )
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1);

            var supplies = await _context
                .SuppliesInventories.Where(p =>
                    p.PurchaseDate >= startDate && p.PurchaseDate < endDate
                )
                .Select(s => new SupplyReportItem
                {
                    Sku = s.Sku,
                    RawMaterial = s.RawMaterial.Name,
                    OriginalWeightKg = s.WeightKg,
                    RemainWeightKg = s.WeightRemainKg,
                    PurchaseDate = s.PurchaseDate,
                    ExpirationDate = s.ExpirationDate,
                })
                .ToListAsync();

            return new SuppliesReportResponseDTO
            {
                GenerationDate = DateTime.Now,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Supplies = supplies,
            };
        }

        public async Task<PurchasesReportResponseDTO> GetPurchasesReportAsync(
            ReportRequestDTO request
        )
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1);

            var purchases = await _context
                .Purchases.Where(p => p.CreatedAt >= startDate && p.CreatedAt < endDate)
                .Select(p => new PurchaseReportItem
                {
                    Sku = p.Sku,
                    Supplier = p.Supplier.Name,
                    RawMaterial = p.RawMaterial.Name,
                    Date = p.CreatedAt,
                    TotalKg = p.TotalKg,
                    PriceKG = p.PriceKg,
                    TotalPrice = p.TotalPrice,
                })
                .ToListAsync();

            return new PurchasesReportResponseDTO
            {
                GenerationDate = DateTime.Now,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Purchases = purchases,
            };
        }
    }
}
