using AutoMapper;
using comercializadora_de_pulpo_api.Models;
using comercializadora_de_pulpo_api.Models.DTOs.Client;
using comercializadora_de_pulpo_api.Models.DTOs.Login;
using comercializadora_de_pulpo_api.Models.DTOs.Products;
using comercializadora_de_pulpo_api.Models.DTOs.Purchases;
using comercializadora_de_pulpo_api.Models.DTOs.RawMaterials;
using comercializadora_de_pulpo_api.Models.DTOs.Role;
using comercializadora_de_pulpo_api.Models.DTOs.Supplier;
using comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory;
using comercializadora_de_pulpo_api.Models.DTOs.Units;
using comercializadora_de_pulpo_api.Models.DTOs.User;

namespace comercializadora_de_pulpo_api.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<User, UserDetailsDTO>()
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(
                    dest => dest.RoleDescription,
                    opt => opt.MapFrom(src => src.Role.Description)
                );
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>().ForMember(dest => dest.Id, opt => opt.Ignore());

            // Role
            CreateMap<Role, RoleDTO>();

            //Auth
            CreateMap<User, LoginResponseDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));

            //Suppliers
            CreateMap<Supplier, SupplierDTO>()
                .ForMember(
                    dest => dest.Rfc,
                    opt => opt.MapFrom(src => src.Rfc != null ? src.Rfc.Trim() : null)
                );
            CreateMap<SupplierRequestDTO, Supplier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Clients
            CreateMap<Client, ClientDTO>();
            CreateMap<ClientRequestDTO, Client>();

            //Products
            CreateMap<CreateProductDTO, Product>();
            CreateMap<Product, ProductDTO>()
                .ForMember(
                    dest => dest.RawMaterial,
                    opt => opt.MapFrom(src => src.RawMaterial.Name)
                );
            CreateMap<Product, ProductDetailsDTO>()
                .ForMember(
                    dest => dest.RawMaterial,
                    opt => opt.MapFrom(src => src.RawMaterial.Name)
                )
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.Label));

            //Raw Materials
            CreateMap<RawMaterial, RawMaterialDTO>();

            //Units
            CreateMap<Unit, UnitDTO>();

            //Purchases
            CreateMap<Purchase, PurchaseDTO>()
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(
                    dest => dest.RawMaterial,
                    opt => opt.MapFrom(src => src.RawMaterial.Name)
                );

            CreateMap<Purchase, PurchaseDetailsDTO>()
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(
                    dest => dest.SupplierEmail,
                    opt => opt.MapFrom(src => src.Supplier.Email)
                )
                .ForMember(
                    dest => dest.SupplierPhone,
                    opt => opt.MapFrom(src => src.Supplier.Phone)
                )
                .ForMember(
                    dest => dest.RawMaterial,
                    opt => opt.MapFrom(src => src.RawMaterial.Name)
                )
                .ForMember(
                    dest => dest.RawMaterialDescription,
                    opt => opt.MapFrom(src => src.RawMaterial.Description)
                )
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName));

            // SuppliesInventory
            CreateMap<SuppliesInventory, SupplyDTO>()
                .ForMember(
                    dest => dest.RawMaterial,
                    opt => opt.MapFrom(src => src.RawMaterial.Name)
                );

            CreateMap<SuppliesInventory, SupplyDetailsDTO>()
                .ForMember(
                    dest => dest.PurchaseSku,
                    opt=> opt.MapFrom(src=>src.Purchase.Sku)
                )
                .ForMember(
                    dest => dest.RawMaterial,
                    opt => opt.MapFrom(src => src.RawMaterial.Name)
                );
        }
    }
}
