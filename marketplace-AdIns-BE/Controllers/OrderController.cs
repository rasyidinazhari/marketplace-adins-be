using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Models;
using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
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
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            try
            {
                var order = await _orderService.CreateAsync(userId, request);
                return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DBConcurrencyException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            List<OrderResponse> orders;
            if (role == "Buyer")
            {
                orders = await _orderService.GetAllOrderForBuyerAsync(userId);
            }
            else if (role == "Seller")
            {
                orders = await _orderService.GetAllOrderForSellerAsync(userId);
            }
            else
            {
                return Forbid();
            }

            return Ok(orders);
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpGet("{orderId:long}")]
        public async Task<IActionResult> GetOrder(long orderId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            var order = await _orderService.GetByIdAsync(orderId, userId, role == "Seller");
            if (order == null) return NotFound();
            return Ok(order);
        }

        [Authorize(Roles = "Buyer,Seller")]
        [HttpPatch("{orderId:long}")]
        public async Task<IActionResult> UpdateOrderStatus(
            long orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!long.TryParse(userIdClaim, out long userId) || userId <= 0)
            {
                return Unauthorized("Invalid or missing user ID claim.");
            }

            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status)
                || !string.Equals(status.ToString(), request.Status, StringComparison.OrdinalIgnoreCase)
                || !Enum.IsDefined(typeof(OrderStatus), status))
            {
                return BadRequest(new { message = "Status pesanan tidak dikenal." });
            }

            try
            {
                string? role = User.FindFirst(ClaimTypes.Role)?.Value;
                var order = await _orderService.UpdateStatusAsync(orderId, userId, role == "Seller", status);
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (DBConcurrencyException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
