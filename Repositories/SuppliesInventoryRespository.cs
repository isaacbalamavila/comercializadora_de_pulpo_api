using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class SuppliesInventoryRespository(ComercializadoraDePulpoContext context)
        : ISuppliesInventoryRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;

        public async Task<int> GetTotalbyDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context
                .SuppliesInventories.Where(sp => sp.PurchaseDate >= start && sp.PurchaseDate < end)
                .CountAsync();
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
    }
}
