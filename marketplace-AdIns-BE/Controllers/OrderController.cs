using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var result = await _orderService.CreateAsync(request);
            return Created($"api/orders/{result.Id}", result);
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                string? role = User.FindFirst("role")?.Value;

                if (!long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized("Invalid or missing user ID claim.");
                }

                List<Order> orders = null;
                if (role == "Buyer")
                {
                    orders = await _orderService.GetAllOrderForBuyerAsync(userId);
                } else if (role == "Seller")
                {
                    orders = await _orderService.GetAllOrderForSellerAsync(userId);
                }

                return Ok(orders);

            } catch (Exception Ex)
            {
                throw;
            }
        }

        /*[Authorize]
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesOrders()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var orders = await _orderService.GetSalesAsync(userId);
            return Ok(orders);
        }*/

        [Authorize]
        [HttpGet("{orderId:long}")]
        public async Task<IActionResult> GetOrder(long orderId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize]
        [HttpPatch("{orderId:long}")]
        public async Task<IActionResult> UpdateOrderStatus(
            long orderId, [FromBody] OrderRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var order = await _orderService.UpdateAsync(orderId, request);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
