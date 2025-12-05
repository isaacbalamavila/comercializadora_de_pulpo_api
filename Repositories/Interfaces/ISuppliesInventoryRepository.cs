using comercializadora_de_pulpo_api.Models;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface ISuppliesInventoryRepository
    {
        Task<int> GetTotalbyDateAsync(DateTime date);
        Task<Response<SuppliesInventory>> CreateSupplieAsync(SuppliesInventory newSupplier);
    }
}
