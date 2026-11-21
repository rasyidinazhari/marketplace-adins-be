using System.Security.Claims;
using marketplace_AdIns_BE.DTOs;
using marketplace_AdIns_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketplace_AdIns_BE.Controllers
{
    [ApiController]
    [Route("api/orderitems")]
    [Authorize]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<OrderItemResponse>>> GetAllOrderItemsByOrderId([FromQuery] long orderId)
        {
            if (orderId <= 0)
                return BadRequest(new { message = "OrderId tidak valid." });

            var items = await _orderItemService.GetAllAsync(orderId);
            return Ok(items);
        }

        /*[Authorize]
        [HttpGet("{id:long}")]
        public async Task<ActionResult<OrderItemResponse>> GetOrderById(long id)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID order item tidak valid." });

            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { message = $"Order item dengan ID {id} tidak ditemukan." });

            return Ok(_orderItemService.ToOrderItemResponse(item));
        }*/
    }
}