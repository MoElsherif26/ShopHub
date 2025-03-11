using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopHub.API.Helper;
using ShopHub.Core.Entities;
using ShopHub.Core.Interfaces;

namespace ShopHub.API.Controllers
{

    public class BasketsController : BaseController
    {
        public BasketsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("get-basket-item/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await work.CustomerBasket.GetBasketAsync(id);
            if (result is null)
            {
                return Ok(new CustomerBasket());
            }
            return Ok(result);
        }
        [HttpPost("update-basket")]
        public async Task<IActionResult> Add(CustomerBasket basket)
        {
            var updatedBasket = await work.CustomerBasket.UpdateBasketAsync(basket);
            return Ok(updatedBasket);
        }

        [HttpDelete("delete-basket-item/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            bool result = await work.CustomerBasket.DeleteBasketAsync(id);
            return result
                ? Ok(new ResponseAPI(200, "Item deleted"))
                : BadRequest(new ResponseAPI(400, "Delete failed"));
        }
    }
}
