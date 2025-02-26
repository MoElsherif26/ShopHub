using AutoMapper;
using ShopHub.Core.DTO;
using ShopHub.Core.Entities.Product;

namespace ShopHub.API.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(x => x.CategoryName,
                op => 
                op.MapFrom(src => src.Category.Name))
                .ReverseMap();

            CreateMap<Photo, PhotoDTO>().ReverseMap();

            CreateMap<AddProductDTO, Product>()
                .ForMember(x => x.Photos,
                op => op.Ignore())
                .ReverseMap();
            
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(x => x.Photos,
                op => op.Ignore())
                .ReverseMap();
        }
    }
}
