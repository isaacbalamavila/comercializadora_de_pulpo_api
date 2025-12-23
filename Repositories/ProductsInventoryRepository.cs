using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class ProductsInventoryRepository(ComercializadoraDePulpoContext context)
        : IProductInventoryRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;

        public async Task<int> GetTotalbyDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context
                .ProductBatches.Where(sp => sp.CreatedAt >= start && sp.CreatedAt < end)
                .CountAsync();
        }

        public async Task<Response<ProductBatch>> CreateProductBatchAsync(
            ProductBatch newProductBach
        )
        {
            try
            {
                _context.ProductBatches.Add(newProductBach);
                await _context.SaveChangesAsync();
                return Response<ProductBatch>.Ok(newProductBach, 204);
            }
            catch (Exception ex)
            {
                return Response<ProductBatch>.Fail(
                    "Ocurrió un error al crear el lote del producto",
                    ex.Message
                );
            }
        }

        public async Task<List<ProductInventoryDTO>> GetProductInventoryAsync()
        {
            var inventory = await _context
                .ProductBatches.GroupBy(pb => pb.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(pb => pb.Remain), // Remain Pieces
                    BatchCount = g.Count(),
                })
                .Join(
                    _context.Products.Where(p => !p.IsDeleted),
                    batch => batch.ProductId,
                    product => product.Id,
                    (batch, product) =>
                        new ProductInventoryDTO
                        {
                            Id = product.Id,
                            Name = product.Name,
                            RawMaterial = product.RawMaterial.Name,
                            Price = product.Price,
                            Content = product.Content,
                            Unit = product.Unit.Abbreviation,
                            StockMin = product.StockMin,
                            Quantity = batch.TotalQuantity,
                        }
                )
                .Where(p => p.Quantity > 0)
                .OrderByDescending(p => p.Quantity)
                .ToListAsync();

            return inventory;
        }

        public async Task<ProductBatchInventoryResponse> GetProductBatchesAsync(
            ProductBatchesRequestDTO request
        )
        {
            IQueryable<ProductBatch> query = _context.ProductBatches.AsQueryable();

            if (request.Sku != null)
                query = query.Where(pb => pb.Sku.Contains(request.Sku));

            if (request.ProductId.HasValue)
                query = query.Where(pb => pb.ProductId == request.ProductId);

            if (request.Status.HasValue)
            {
                if (request.Status.Value == 1)
                    query = query.Where(pb => pb.Remain > 0);

                if (request.Status.Value == 2)
                    query = query.Where(pb => pb.Remain == 0);
            }

            int total = await query.CountAsync();

            var productBatches = await query
                .OrderBy(pb => pb.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(pb => new ProductBatchInventory
                {
                    Id = pb.Id,
                    Sku = pb.Sku,
                    Name = pb.Product.Name,
                    Content = pb.Product.Content,
                    Unit = pb.Product.Unit.Abbreviation,
                    Quantity = pb.Quantity,
                    QuantityRemain = pb.Remain,
                    Price = pb.Product.Price,
                    CreatedAt = pb.CreatedAt,
                    ExpirationDate = pb.ExpirationDate
                })
                .ToListAsync();

            return new ProductBatchInventoryResponse
            {
                Page = request.Page,
                Total = total,
                TotalPages = (int)Math.Ceiling((double)total / request.PageSize),
                Productbatches = productBatches,
            };
        }
    }
}
