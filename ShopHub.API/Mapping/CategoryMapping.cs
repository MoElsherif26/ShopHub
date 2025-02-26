using AutoMapper;
using ShopHub.Core.DTO;
using ShopHub.Core.Entities.Product;

namespace ShopHub.API.Mapping
{
    public class CategoryMapping: Profile
    {
        public CategoryMapping()
        {
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<UpdateCategoryDTO, Category>().ReverseMap();
        }
    }
}
