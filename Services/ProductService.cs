using Amazon.Util.Internal;
using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Repositories.Interfaces;
using comercializadora_de_pulpo_api.Services.Interfaces;
using comercializadora_de_pulpo_api.Utilities;

namespace comercializadora_de_pulpo_api.Services
{
    public class ProductService(
        IRawMaterialsRepository rawMaterialsRepository,
        IUnitRepository unitRepository,
        IProductRepository productRepository,
        IMapper mapper,
        S3Service s3Service
    ) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUnitRepository _unitRepository = unitRepository;
        private readonly IRawMaterialsRepository _rawMaterialRepository = rawMaterialsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly S3Service _s3Service = s3Service;

        public async Task<Response<List<ProductDTO>>> GetProductsAsync(bool onlyActives)
        {
            var products = await _productRepository.GetProductsAsync(onlyActives);
            return Response<List<ProductDTO>>.Ok(_mapper.Map<List<ProductDTO>>(products));
        }

        public async Task<Response<ProductDetailsDTO>> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            return product == null
                ? Response<ProductDetailsDTO>.Fail(
                    "Producto no encontrado",
                    $"No se encontro un producto con el ID '{productId}'",
                    404
                )
                : Response<ProductDetailsDTO>.Ok(_mapper.Map<ProductDetailsDTO>(product));
        }

        public async Task<Response<ProductDTO>> CreateProductAsync(CreateProductDTO request)
        {
            request.Name = request.Name.Trim().ToLower();
            request.Description = request.Description.ToLower();

            //Data Validations

            if (!await _productRepository.VerifyNameAsync(request.Name))
                return Response<ProductDTO>.Fail(
                    "Ya existe un producto con ese nombre",
                    $"Ya existe un producto con el nombre '{request.Name}'",
                    400
                );

            if (request.RawMaterialNeededKg < 0)
                return Response<ProductDTO>.Fail(
                    "Materia prima necesaria debe ser mayor a 0",
                    $"Para elaborar un producto se requiere minimo un gramo de materia prima",
                    400
                );

            if (request.TimeNeededMin < 1)
                return Response<ProductDTO>.Fail(
                    "El tiempo de producción debe ser mayor a 0 minutos",
                    $"Para elaborar un producto se requiere minimo 1 minuto",
                    400
                );

            if (request.StockMin < 1)
                return Response<ProductDTO>.Fail(
                    "El stock mínimo debe ser mayor a un 1 producto",
                    $"El stock mínimo debe ser mayor a un 1 producto",
                    400
                );

            var rawMaterial = await _rawMaterialRepository.GetRawMaterialByIdAsync(
                request.RawMaterialId
            );

            if (rawMaterial == null)
                return Response<ProductDTO>.Fail(
                    "Materia prima no encontrada",
                    $"No se encontro ninguna materia prima con el ID '{request.RawMaterialId}'",
                    400
                );

            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId);

            if (unit == null)
                return Response<ProductDTO>.Fail(
                    "Unidad no encontrada",
                    $"No se encontro ninguna unidad con el ID '{request.UnitId}'",
                    400
                );

            //Generate SKU
            var SKU = await GenerateSKUAsync(rawMaterial.Abbreviation, unit.Abbreviation);

            // Unique Validations Section

            // Create Product Section
            var newProduct = _mapper.Map<Product>(request);
            newProduct.Id = Guid.NewGuid();
            newProduct.Sku = SKU;
            newProduct.IsDeleted = false;
            newProduct.CreatedAt = DateTime.UtcNow;
            newProduct.RawMaterial = rawMaterial;
            newProduct.Unit = unit;
            newProduct.Img = await _s3Service.UploadImageAsWebpAsync(
                request.Img,
                newProduct.Id.ToString()
            );

            var createRequest = await _productRepository.CreateProductAsync(newProduct);
            return createRequest.IsSuccess
                ? Response<ProductDTO>.Ok(_mapper.Map<ProductDTO>(newProduct), 201)
                : Response<ProductDTO>.Fail(
                    "Ocurrió un error al intentar crear el producto",
                    createRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<ProductDetailsDTO>> UpdateProductAsync(
            Guid productId,
            UpdateProductDTO request
        )
        {
            request.Name = request.Name.Trim().ToLower();

            var productSaved = await _productRepository.GetProductByIdAsync(productId);

            if (productSaved == null)
                return Response<ProductDetailsDTO>.Fail(
                    "Producto no encontrado",
                    $"No se encontró un producto con el ID '{productId}'.",
                    404
                );

            if (productSaved.IsDeleted)
                return Response<ProductDetailsDTO>.Fail(
                    "No se puede modificar un producto eliminado",
                    $"El producto con el ID '{productId}' se encuentra eliminado y con las modificaciones restringidas",
                    400
                );

            if (request.Img != null)
            {
                string imgURL = await _s3Service.UploadImageAsWebpAsync(
                    request.Img!,
                    productSaved.Id.ToString()
                );
                imgURL = imgURL + "?t=" + DateTime.UtcNow.Ticks.ToString();

                // Save New image URL
                productSaved.Img = imgURL;
            }

            if (
                !productSaved.Name.Equals(request.Name, StringComparison.CurrentCultureIgnoreCase)
                && !await _productRepository.VerifyNameAsync(request.Name)
            )
                return Response<ProductDetailsDTO>.Fail(
                    "Nombre ya registrado",
                    $"El nombre '{request.Name}' ya existe en el sistema",
                    400
                );

            if (request.StockMin < 1)
                return Response<ProductDetailsDTO>.Fail(
                    "El stock mínimo debe ser mayor a un 1 producto",
                    $"El stock mínimo debe ser mayor a un 1 producto",
                    400
                );

            // Save updated fields
            productSaved.Name = request.Name;
            productSaved.Description = request.Description;
            productSaved.Price = request.Price;
            productSaved.StockMin = request.StockMin;

            var updateRequest = await _productRepository.UpdateProductAsync(productSaved);
            return updateRequest.IsSuccess
                ? Response<ProductDetailsDTO>.Ok(_mapper.Map<ProductDetailsDTO>(updateRequest.Data))
                : Response<ProductDetailsDTO>.Fail(
                    "Ocurrió un error al intentar actualizar el producto",
                    updateRequest.Error!.ErrorDetails,
                    500
                );
        }

        public async Task<Response<bool>> DeleteProductAsync(Guid productId)
        {
            var productSaved = await _productRepository.GetProductByIdAsync(productId);

            if (productSaved == null)
                return Response<bool>.Fail(
                    "Producto no encontrado",
                    $"No se encontró un producto con el ID : '{productId}'",
                    404
                );

            if (productSaved!.IsDeleted)
                return Response<bool>.Ok(true, 204);

            productSaved.IsDeleted = true;

            var updateRequest = await _productRepository.UpdateProductAsync(productSaved);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true, 204)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar eliminar el producto",
                    updateRequest.Error!.ErrorDetails
                );
        }

        public async Task<Response<bool>> RestoreProductAsync(Guid productId)
        {
            var productSaved = await _productRepository.GetProductByIdAsync(productId);

            if (productSaved == null)
                return Response<bool>.Fail(
                    "Producto no encontrado",
                    $"No se encontró un producto con el ID : '{productId}'",
                    404
                );

            if (!productSaved!.IsDeleted)
                return Response<bool>.Ok(true);

            productSaved.IsDeleted = false;

            var updateRequest = await _productRepository.UpdateProductAsync(productSaved);

            return updateRequest.IsSuccess
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail(
                    "Ocurrió un error al intentar restaurar el producto",
                    updateRequest.Error!.ErrorDetails
                );
        }

        //Helper Functions
        private async Task<string> GenerateSKUAsync(string rawMaterial, string unit)
        {
            var total = await _productRepository.GetTotalProductsAsync();

            return $"{rawMaterial}{DateTime.Now:yy}-{unit}-{(total + 1):D4}";
        }
    }
}
