using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopHub.API.Helper;
using ShopHub.Core.DTO;
using ShopHub.Core.Interfaces;
using ShopHub.Core.Sharing;

namespace ShopHub.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> Get([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await work.ProductRepository
                    .GetAllAsync(productParams);


                return Ok(new Pagination<ProductDTO>(productParams.PageNumber, 
                    productParams.pageSize, products.TotalCount, 
                    products.products));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while getting products"));
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var productDomain = await work.ProductRepository.GetByIdAsync(id, 
                    x => x.Category, x => x.Photos);

                if (productDomain is null)
                {
                    return BadRequest(new ResponseAPI(400, "No Products with this Id"));
                }
                var productDto = mapper.Map<ProductDTO>(productDomain);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while getting product"));
            }
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> Add(AddProductDTO productDto)
        {
            try
            {
                await work.ProductRepository.AddAsync(productDto);
                return Ok(new ResponseAPI(200, "Product Added!"));

            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseAPI(400, "Something went wrong while adding product"));

            }
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> Update(UpdateProductDTO updateProductDto)
        {
            try
            {
                await work.ProductRepository.UpdateAsync(updateProductDto);
                return Ok(new ResponseAPI(200, "Product Updated!"));

            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseAPI(400, "Something went wrong while updating product"));

            }
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await work.ProductRepository
                    .GetByIdAsync(id, x => x.Photos, x => x.Category);

                await work.ProductRepository.DeleteAsync(product);
                return Ok(new ResponseAPI(200, "Product Deleted!"));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while deleting product"));
            }
        }
    }
}
