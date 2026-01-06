using System.Linq;
using AutoMapper;
using Azure.Core;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Proccess;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace comercializadora_de_pulpo_api.Repositories
{
    public class ProcessRepository(ComercializadoraDePulpoContext context, IMapper mapper)
        : IProcessRepository
    {
        private readonly ComercializadoraDePulpoContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> GetTotalbyDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context
                .ProductionProcesses.Where(p => p.StartDate >= start && p.StartDate < end)
                .CountAsync();
        }

        public async Task<Response<ProcessResponseDTO>> GetProcessesAsync(ProcessRequestDTO request)
        {
            IQueryable<ProductionProcess> query = _context.ProductionProcesses.AsQueryable();

            if (request.User.HasValue)
                query = query.Where(p => p.UserId == request.User);

            if (request.Product.HasValue)
                query = query.Where(p => p.ProductId == request.Product);

            if (request.Status.HasValue)
                query = query.Where(p => p.StatusId == request.Status);

            if (request.Date.HasValue)
            {
                var start = request.Date.Value.Date;
                var end = start.AddDays(1);
                if (request.IsMovil.HasValue && request.IsMovil.Value &&
                    request.Status.HasValue && request.Status.Value == 3)
                {
                    query = query.Where(p => p.EndDate >= start && p.EndDate < end);
                }
                else
                {
                    query = query.Where(p => p.StartDate >= start && p.StartDate < end);
                }
            }

            int total = await query.CountAsync();

            var processes = await query
                .OrderByDescending(p => p.StartDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProcessDTO
                {
                    Id = p.Id,
                    Product = p.Product.Name,
                    ProductSku = p.Product.Sku,
                    User = $"{p.User.Name} {p.User.LastName}",
                    Quantity = p.Quantity,
                    StatusId = p.StatusId,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                })
                .ToListAsync();

            var response = new ProcessResponseDTO
            {
                Page = request.Page,
                Total = total,
                TotalPages = (int)Math.Ceiling((double)total / request.PageSize),
                Processes = _mapper.Map<List<ProcessDTO>>(processes),
            };

            return Response<ProcessResponseDTO>.Ok(response);
        }

        public async Task<ProductionProcess?> GetProcessByIdAsync(Guid processId)
        {
            return await _context
                .ProductionProcesses.Include(p => p.Status)
                .Include(p => p.User)
                .Include(p => p.Product)
                .ThenInclude(prod => prod.RawMaterial)
                .Include(p => p.Product)
                .ThenInclude(prod => prod.Unit)
                .FirstOrDefaultAsync(p => p.Id == processId);
        }

        public async Task<ProductionProcessDetailsDTO?> GetProductionProcessDetails(Guid processId)
        {
            return await _context
                .ProductionProcesses.Where(p => p.Id == processId)
                .Select(p => new ProductionProcessDetailsDTO
                {
                    Id = p.Id,
                    ProductName = p.Product.Name,
                    Sku = p.Product.Sku,
                    Status = p.StatusId,
                    RawMaterial = p.Product.RawMaterial.Name,
                    RawMaterialNeededKg = p.Product.RawMaterialNeededKg,
                    TimeNeededMin = p.Product.TimeNeededMin,
                    Quantity = p.Quantity,
                    SuppliesUsed = p
                        .ProductBatchSupplies.Select(s => new ProductionSupplyDetailsDTO
                        {
                            Sku = s.SuppliesInventory.Sku,
                            ExpirationDate = s.SuppliesInventory.ExpirationDate,
                            UsedWeightKg = s.UsedWeightKg,
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Response<ProductionProcess>> UpdateProcessAsync(
            ProductionProcess processUpdated
        )
        {
            try
            {
                _context.ProductionProcesses.Update(processUpdated);
                await _context.SaveChangesAsync();
                return Response<ProductionProcess>.Ok(processUpdated);
            }
            catch (Exception ex)
            {
                return Response<ProductionProcess>.Fail(
                    "Ocurrió un error al intentar actualizar el proceso",
                    ex.Message
                );
            }
        }
    }
}
