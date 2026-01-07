using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Sale;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Services
{
    public class SalesService(
        ComercializadoraDePulpoContext context,
        ISalesRepository salesRepository
    ) : ISalesService
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly ISalesRepository _salesRepository = salesRepository;

        public async Task<Response<SalesResponseDTO>> GetSalesAsync(SalesRequestDTO request)
        {
            var sales = await _salesRepository.GetSalesAsync(request);

            return Response<SalesResponseDTO>.Ok(sales);
        }

        public async Task<Response<SaleResponse>> GetSaleDetailsByIdAsync(Guid id)
        {
            var saleSaved = await _salesRepository.GetSaleDetailsByIdAsync(id);

            if (saleSaved == null)
                return Response<SaleResponse>.Fail(
                    $"No se encontró una venta con el ID '{id}'",
                    $"No se encontró ningun registro en las ventas con el ID '{id}'",
                    404
                );

            return Response<SaleResponse>.Ok(saleSaved);
        }

        public async Task<Response<SaleResponse>> SaveSale(SaleRequest request)
        {
            if (request.Products == null || request.Products.Count == 0)
                return Response<SaleResponse>.Fail(
                    "Lista de productos vacía",
                    "La lista de productos se encuentra vacía",
                    400
                );

            var employeeSaved = await _context
                .Users.Where(u => u.Id == request.UserId && !u.IsDeleted)
                .Select(u => new { u.Id, FullName = u.Name + " " + u.LastName })
                .FirstOrDefaultAsync();

            if (employeeSaved == null)
                return Response<SaleResponse>.Fail(
                    "Usuario no encontrado",
                    $"No se encontró un usuario con el ID '{request.UserId}'",
                    404
                );

            var clientSaved = await _context
                .Clients.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Rfc,
                    c.IsDeleted,
                })
                .Where(c => c.Id == request.ClientId)
                .FirstOrDefaultAsync();

            if (clientSaved == null)
                return Response<SaleResponse>.Fail(
                    "Cliente no encontrado",
                    $"No se encontró un cliente con el ID '{request.ClientId}'",
                    404
                );

            if (clientSaved.IsDeleted)
                return Response<SaleResponse>.Fail(
                    "Cliente deshabilitado",
                    $"El cliente con el ID '{request.ClientId}' se encuentra deshabilitado y con las ventas restringidas",
                    404
                );

            var payment_method = await _context
                .PaymentMethods.Select(p => new { p.Id, p.Name })
                .Where(p => p.Id == request.PaymentMethodId)
                .FirstOrDefaultAsync();

            if (payment_method == null)
                return Response<SaleResponse>.Fail(
                    "Método de pago no encontrado",
                    $"No se encontró un método de pago con el ID '{request.PaymentMethodId}'",
                    404
                );

            var productIds = request.Products.Select(p => p.ProductId).ToList();

            var products = await _context
                .Products.Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
                .Select(p => new
                {
                    p.Id,
                    p.Sku,
                    p.Name,
                    p.Price,
                    p.Content,
                    Unit = p.Unit.Abbreviation,
                })
                .ToDictionaryAsync(p => p.Id);

            var missingProducts = productIds.Where(id => !products.ContainsKey(id)).ToList();
            if (missingProducts.Any())
            {
                return Response<SaleResponse>.Fail(
                    "Productos no encontrados",
                    $"No se encontraron los siguientes productos: {string.Join(", ", missingProducts)}",
                    404
                );
            }

            var saleItemsToCreate = new List<SaleItemData>();
            var errors = new List<string>();

            foreach (var item in request.Products)
            {
                var product = products[item.ProductId];

                var availableBatches = await _context
                    .ProductBatches.Where(pb => pb.ProductId == item.ProductId && pb.Remain > 0)
                    .OrderBy(pb => pb.ExpirationDate)
                    .Select(pb => new
                    {
                        pb.Id,
                        pb.Remain,
                        pb.ExpirationDate,
                    })
                    .ToListAsync();

                var totalStock = availableBatches.Sum(b => b.Remain);

                if (totalStock == 0)
                {
                    errors.Add($"El producto '{product.Name}' no tiene stock disponible");
                    continue;
                }

                if (totalStock < item.Quantity)
                {
                    errors.Add(
                        $"Stock insuficiente para '{product.Name}'. Disponible: {totalStock}, Solicitado: {item.Quantity}"
                    );
                    continue;
                }

                var batchDistribution = new List<BatchAllocation>();
                var remainingQuantity = item.Quantity;

                foreach (var batch in availableBatches)
                {
                    if (remainingQuantity <= 0)
                        break;

                    var quantityFromBatch = Math.Min(batch.Remain, remainingQuantity);

                    batchDistribution.Add(
                        new BatchAllocation { BatchId = batch.Id, Quantity = quantityFromBatch }
                    );

                    remainingQuantity -= quantityFromBatch;
                }

                saleItemsToCreate.Add(
                    new SaleItemData
                    {
                        ProductId = item.ProductId,
                        ProductSku = product.Sku,
                        ProductName = product.Name,
                        ProductContent = product.Content,
                        ProductUnit = product.Unit,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = product.Price * item.Quantity,
                        BatchAllocations = batchDistribution,
                    }
                );
            }

            if (errors.Count != 0)
            {
                return Response<SaleResponse>.Fail(
                    "Error en validación de stock",
                    string.Join("; ", errors),
                    400
                );
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sale = new Sale
                {
                    Id = Guid.NewGuid(),
                    ClientId = request.ClientId,
                    EmployeeId = employeeSaved.Id,
                    SaleDate = DateTime.Now,
                    TotalAmount = saleItemsToCreate.Sum(s => s.Subtotal),
                    PaymentMethod = request.PaymentMethodId,
                };

                await _context.Sales.AddAsync(sale);

                var saleItemResponses = new List<SaleItemResponse>();

                foreach (var itemData in saleItemsToCreate)
                {
                    var saleItem = new Models.SaleItem
                    {
                        Id = Guid.NewGuid(),
                        SaleId = sale.Id,
                        ProductId = itemData.ProductId,
                        Quantity = itemData.Quantity,
                        UnitPrice = itemData.UnitPrice,
                        Subtotal = itemData.Subtotal,
                    };

                    await _context.SaleItems.AddAsync(saleItem);

                    foreach (var allocation in itemData.BatchAllocations)
                    {
                        var batch = await _context.ProductBatches.FindAsync(allocation.BatchId);
                        if (batch != null)
                        {
                            batch.Remain -= allocation.Quantity;
                            _context.ProductBatches.Update(batch);
                        }
                    }

                    saleItemResponses.Add(
                        new SaleItemResponse
                        {
                            Sku = itemData.ProductSku,
                            Name = itemData.ProductName,
                            Content = itemData.ProductContent,
                            Unit = itemData.ProductUnit,
                            Quantity = itemData.Quantity,
                            Price = itemData.UnitPrice,
                            Subtotal = itemData.Subtotal,
                        }
                    );
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new SaleResponse
                {
                    Id = sale.Id,
                    Employee = employeeSaved.FullName,
                    Client = clientSaved.Name,
                    Date = sale.SaleDate,
                    PaymentMethod = payment_method.Name,
                    Products = saleItemResponses,
                    Total = sale.TotalAmount,
                };

                return Response<SaleResponse>.Ok(response);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Response<SaleResponse>.Fail(
                    "Error de concurrencia",
                    "El stock cambió durante la transacción. Por favor, intenta nuevamente.",
                    409
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<SaleResponse>.Fail(
                    "Error al procesar la venta",
                    $"Ocurrió un error inesperado: {ex.Message}",
                    500
                );
            }
        }

        // Clases auxiliares internas
        private class SaleItemData
        {
            public Guid ProductId { get; set; }
            public string ProductSku { get; set; } = null!;
            public string ProductName { get; set; } = null!;
            public int ProductContent { get; set; }
            public string ProductUnit { get; set; } = null!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Subtotal { get; set; }
            public List<BatchAllocation> BatchAllocations { get; set; } = null!;
        }

        private class BatchAllocation
        {
            public Guid BatchId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
