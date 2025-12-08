using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;

namespace comercializadora_de_pulpo_api.Services
{
    public class SuppliesInventoryService(
        ISuppliesInventoryRepository suppliesRepository,
        IMapper mapper
    ) : ISuppliesInventoryService
    {
        private readonly ISuppliesInventoryRepository _suppliesRepository = suppliesRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<SuppliesResponseDTO>> GetSuppliesAsync(
            SuppliesRequestDTO request
        )
        {
            return Response<SuppliesResponseDTO>.Ok(
                await _suppliesRepository.GetSuppliesAsync(request)
            );
        }

        public async Task<Response<SupplyDetailsDTO>> GetSupplyByIdAsync(Guid supplyId)
        {
            var savedSupply = await _suppliesRepository.GetSupplyByIdAsync(supplyId);

            if (savedSupply == null)
                return Response<SupplyDetailsDTO>.Fail(
                    "Lote de insumos no encontrado",
                    $"No se encontro un lote de insumos con el ID '{supplyId}'",
                    404
                );

            return Response<SupplyDetailsDTO>.Ok(_mapper.Map<SupplyDetailsDTO>(savedSupply));
        }

        public async Task<Response<SupplyDTO>> UpdateWeightRemainAsync(
            Guid supplyId,
            UpdateWeightRemain request
        )
        {

            if (request.updateWeightRemain < 0)
                return Response<SupplyDTO>.Fail(
                    "Monto inválido",
                    "El monto asignado debe ser mayor 0",
                    400
                );

            var savedSupply = await _suppliesRepository.GetSupplyByIdAsync(supplyId);

            if (savedSupply == null)
                return Response<SupplyDTO>.Fail(
                    "Lote de insumos no encontrado",
                    $"No se encontro un lote de insumos con el ID '{supplyId}'",
                    404
                );

            if (savedSupply.WeightRemainKg == 0)
                return Response<SupplyDTO>.Fail(
                    "El lote se encuentra con las modificaciones restringidas",
                    "No se puede modificar un lote consumido",
                    400
                );

            if (request.updateWeightRemain > savedSupply.WeightRemainKg)
                return Response<SupplyDTO>.Fail(
                    "Monto máximo excedido",
                    "No se puede asignar un monto mayor al disponible",
                    400
                );

            if (savedSupply.WeightRemainKg == request.updateWeightRemain)
                return Response<SupplyDTO>.Ok(
                    _mapper.Map<SupplyDTO>(savedSupply)
                );

            // Update weigth remain
            savedSupply.WeightRemainKg = request.updateWeightRemain;

            var updateRequest = await _suppliesRepository.UpdateSupplyAsync(savedSupply);

            return updateRequest.IsSuccess
                ? Response<SupplyDTO>.Ok(_mapper.Map<SupplyDTO>(updateRequest.Data))
                : Response<SupplyDTO>.Fail(
                    "Ocurrió un error al intentar la cantidad disponible",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> DisposeSupplyAsync(Guid supplyId)
        {
            var savedSupply = await _suppliesRepository.GetSupplyByIdAsync(supplyId);

            if (savedSupply == null)
                return Response<bool>.Fail(
                    "Lote de insumos no encontrado",
                    $"No se encontro un lote de insumos con el ID '{supplyId}'",
                    404
                );

            if (savedSupply.WeightRemainKg == 0)
                return Response<bool>.Ok(true, 204);

            // Set Weight remain to 0
            savedSupply.WeightRemainKg = 0;

            var updateRequest = await _suppliesRepository.UpdateSupplyAsync(savedSupply);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true, 204)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar dar de baja el lote",
                    updateRequest.Error!.ErrorDetails
                );
        }
    }
}
