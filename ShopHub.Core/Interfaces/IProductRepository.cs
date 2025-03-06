using ShopHub.Core.DTO;
using ShopHub.Core.Entities.Product;
using ShopHub.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<ReturnProductDTO> GetAllAsync(ProductParams productParams);
        Task<bool> AddAsync(AddProductDTO addProductDTO);
        Task<bool> UpdateAsync(UpdateProductDTO updateProductDTO);

        Task DeleteAsync(Product product);
    }
}
