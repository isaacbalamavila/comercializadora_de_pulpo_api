using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class ProductSupplyRepository(ComercializadoraDePulpoContext context)
        : IProductSupplyRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;

        public async Task<int> GetTotalbyDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context
                .ProductBatches.Where(pb => pb.CreatedAt >= start && pb.CreatedAt < end)
                .CountAsync();
        }
    }
}
