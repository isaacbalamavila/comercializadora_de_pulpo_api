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
                .Products.Where(p => !p.IsDeleted)
                .Select(p => new ProductInventoryDTO
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    RawMaterial = p.RawMaterial.Name,
                    Price = p.Price,
                    Content = p.Content,
                    Unit = p.Unit.Abbreviation,
                    StockMin = p.StockMin,
                    Quantity = p
                        .ProductBatches.Where(pb => pb.Remain > 0)
                        .Sum(pb => pb.Remain),
                })
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

            if (!String.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(pb => pb.Sku.Contains(search));
            }

            if (request.ProductId.HasValue)
                query = query.Where(pb => pb.ProductId == request.ProductId);

            if (request.Availables.HasValue)
                query = request.Availables.Value
                    ? query.Where(sp => sp.Remain > 0)
                    : query.Where(sp => sp.Remain == 0);

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
                    ExpirationDate = pb.ExpirationDate,
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

        public async Task<ProductBatch?> GetProductBatchByIdAsync(Guid productBatchId)
        {
            return await _context
                .ProductBatches.Where(p => p.Id == productBatchId)
                .Include(p => p.Product)
                .ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync();
        }

        public async Task<Response<ProductBatch>> UpdateProductBatchAsync(
            ProductBatch productBatchUpdated
        )
        {
            try
            {
                _context.ProductBatches.Update(productBatchUpdated);
                await _context.SaveChangesAsync();
                return Response<ProductBatch>.Ok(productBatchUpdated);
            }
            catch (Exception ex)
            {
                return Response<ProductBatch>.Fail(
                    "Ocurrió un error al intentar actualizar la información del lote",
                    ex.Message
                );
            }
        }

        public async Task<ProductBatchDetails?> GetProductBatchDetailsByIdAsync(Guid ProductBatchId)
        {
            var productSaved = await _context
                .ProductBatches.Select(p => new ProductBatchDetails
                {
                    Id = p.Id,
                    quantity = p.Quantity,
                    quantityRemain = p.Remain,
                    Sku = p.Sku,
                    CreationDate = p.CreatedAt,
                    ExpirationDate = p.ExpirationDate,
                    UserName =
                        $"{p.ProductionProcess.User.Name} {p.ProductionProcess.User.LastName}",
                    ProcessId = p.ProductionProcess.Id,
                    Product = new ProductInfo
                    {
                        Sku = p.Product.Sku,
                        Name = p.Product.Name,
                        Content = p.Product.Content,
                        Unit = p.Product.Unit.Abbreviation,
                        Price = p.Product.Price,
                    },
                })
                .Where(p => p.Id == ProductBatchId)
                .FirstOrDefaultAsync();

            return productSaved;
        }
    }
}
