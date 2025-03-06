using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopHub.Core.DTO;
using ShopHub.Core.Entities.Product;
using ShopHub.Core.Interfaces;
using ShopHub.Core.Services;
using ShopHub.Core.Sharing;
using ShopHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;

        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<ReturnProductDTO> GetAllAsync(ProductParams productParams)
        {
            var products = context.Products
                .Include(m => m.Category)
                .Include(m => m.Photos)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(productParams.Search))
            {
                var searchWords = productParams.Search.Split(' ');

                products = products.Where(m => searchWords.All(word =>

                m.Name.ToLower().Contains(word.ToLower()) || m.Description.ToLower().Contains(word.ToLower())

                ));
            }
                

            if (productParams.CategoryId.HasValue)
                products = products.Where(m => m.CategoryId == productParams.CategoryId);




            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                products = productParams.Sort switch
                {
            
                    "PriceAce" => products.OrderBy(m => m.NewPrice),
                    "PriceDce" => products.OrderByDescending(m => m.NewPrice),
                    _ => products.OrderBy(m => m.Name),
                };
            }

            ReturnProductDTO returnProductDTO = new ReturnProductDTO();
            returnProductDTO.TotalCount = products.Count();

            products = products.Skip((productParams.pageSize) * (productParams.PageNumber - 1)).Take(productParams.pageSize);

            returnProductDTO.products = mapper.Map<List<ProductDTO>>(products);
            return returnProductDTO;
        }

        public async Task<bool> AddAsync(AddProductDTO addProductDTO)
        {
            if (addProductDTO == null)
            {
                return false;
            }
            var product = mapper.Map<Product>(addProductDTO);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            var imagePath = await imageManagementService.AddImageAsync(addProductDTO.Photo, addProductDTO.Name);
            var photo = imagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id

            }).ToList();
            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;
            
        }


        public async Task<bool> UpdateAsync(UpdateProductDTO updateProductDTO)
        {
            if (updateProductDTO is null)
            {
                return false;
            }
            var findPrdouct = await context.Products
                .Include(m => m.Category)
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == updateProductDTO.Id);

            if (findPrdouct is null)
            {
                return false;
            }
            mapper.Map(updateProductDTO, findPrdouct);

            var findPhoto = await context.Photos.Where(m => m.ProductId == updateProductDTO.Id).ToListAsync();

            foreach (var item in findPhoto)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);
            }
            context.Photos.RemoveRange(findPhoto);

            var imagePath = await imageManagementService.AddImageAsync(updateProductDTO.Photo, updateProductDTO.Name);
            var photo = imagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = updateProductDTO.Id
            }).ToList();
            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task DeleteAsync(Product product)
        {
            var photo = await context.Photos.Where(m => m.ProductId == product.Id).ToListAsync();
            foreach (var item in photo)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);

            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();

        }
    }
}
