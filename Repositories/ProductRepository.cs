using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class ProductRepository(ComercializadoraDePulpoContext context, IMapper mapper)
        : IProductRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> GetTotalProductsAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetProductsAsync(bool onlyActives)
        {
            var query = _context.Products.AsQueryable();

            if (onlyActives)
                query = query.Where(p => p.IsDeleted == false);

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.RawMaterial)
                .Include(p => p.Unit)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context
                .Products.Include(p => p.RawMaterial)
                .Include(p => p.Unit)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<bool> VerifyNameAsync(string name)
        {
            return !await _context.Products.AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<Response<Product>> CreateProductAsync(Product newProduct)
        {
            try
            {
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();
                return Response<Product>.Ok(newProduct, 201);
            }
            catch (Exception ex)
            {
                return Response<Product>.Fail(
                    "Ocurrió un error al intentar crear el producto",
                    ex.Message
                );
            }
        }

        public async Task<Response<Product>> UpdateProductAsync(Product UpdatedProduct)
        {
            try
            {
                _context.Products.Update(UpdatedProduct);
                await _context.SaveChangesAsync();
                return Response<Product>.Ok(UpdatedProduct, 200);
            }
            catch (Exception ex)
            {
                return Response<Product>.Fail(
                    "Ocurrió un error al intentar actualizar la información del producto",
                    ex.Message
                );
            }
        }


    }
}
