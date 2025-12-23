using comercializadora_de_pulpo_api.Models;

namespace comercializadora_de_pulpo_api.Repositories.Interfaces
{
    public interface IProductSupplyRepository
    {
        Task<int> GetTotalbyDateAsync(DateTime date);
    }
}
