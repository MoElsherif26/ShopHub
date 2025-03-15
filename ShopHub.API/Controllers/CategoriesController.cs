using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopHub.API.Helper;
using ShopHub.Core.DTO;
using ShopHub.Core.Entities.Product;
using ShopHub.Core.Interfaces;

namespace ShopHub.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var categories = await work.CategoryRepository.GetAllAsync();
                return Ok(categories);
            }
            catch
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while getting categories"));
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await work.CategoryRepository.GetByIdAsync(id);
                if (category is null)
                {
                    return BadRequest(new ResponseAPI(400, "No Categories with this Id"));
                }
                return Ok(category);
            }
            catch 
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while getting category"));
            }
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> Add(CategoryDTO categoryDTO)
        {
            try
            {
                var category = mapper.Map<Category>(categoryDTO);
                await work.CategoryRepository.AddAsync(category);
                return Ok(new ResponseAPI(200, "Category Added!"));
            }
            catch 
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while adding category"));
            }
        }

        [HttpPut("update-category")]
        public async Task<IActionResult> Update(UpdateCategoryDTO categoryDTO)
        {
            try
            {
                var category = mapper.Map<Category>(categoryDTO);
                await work.CategoryRepository.UpdateAsync(category);
                return Ok(new ResponseAPI(200, "Category Updated!"));
            }
            catch 
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while updating category"));
            }
        }

        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await work.CategoryRepository.DeleteAsync(id);
                return Ok(new ResponseAPI(200, "Category Deleted!"));
            }
            catch 
            {
                return BadRequest(new ResponseAPI(400, "Something went wrong while deleting category"));
            }
        }

    }
}