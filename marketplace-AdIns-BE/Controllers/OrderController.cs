using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using marketplace_AdIns_BE.DTOs;

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

        [Authorize(Roles = "Buyer")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            OrderDetailResponse? result = await _orderService.CreateAsync(userId, request);
            return Created($"api/orders/{result.Id}", result);
        }

        [Authorize(Roles = "Buyer")]
        [HttpGet("purchases")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var orders = await _orderService.GetPurchasesAsync(userId);
            return Ok(orders);
        }

        [Authorize(Roles = "Seller")]
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
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetOrder(long id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var order = await _orderService.GetByIdAsync(userId, id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            long id, [FromBody] UpdateOrderStatusRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId))
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            var order = await _orderService.UpdateStatusAsync(userId, id, request.Status);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
