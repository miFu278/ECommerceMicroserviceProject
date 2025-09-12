using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No order detected in database");

            var (_, list) = OrderConversion.FromEntity(null, orders);
            return !list!.Any() ? NotFound("No orders available") : Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid order ID provided");

            var order = await orderInterface.FindByIdAsync(orderId);

            if (order is null)
                return NotFound($"Order with ID: {orderId} not found");

            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);

        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return BadRequest("Invalid data provided");

            var orders = await orderService.GetOrdersByClientId(clientId);

            return !orders.Any() ? NotFound($"No orders found for client {clientId}") : Ok(orders);
        }


        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid data provided");
            var orderDetail = await orderService.GetOrderDetails(orderId);
            return orderDetail.OrderId > 0 ? Ok(orderDetail) : NotFound("No order found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder([FromBody] OrderDTO orderDTO)
        {
            // check model state if all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");
            if (orderDTO is null)
                return BadRequest("Order data is required");
            try
            {
                // convert to entity
                var getEntity = OrderConversion.ToEntity(orderDTO);
                var response = await orderInterface.CreateAsync(getEntity);

                return response.Flag
                    ? CreatedAtAction(nameof(GetOrder), new { id = getEntity.Id }, response)
                    : BadRequest(response);
            }
            catch (Exception ex)
            {
                // log original exception
                LogException.LogExceptions(ex);

                // display scary-free message to client
                return new Response(false, "Error occured while placing order");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> UpdateOrder(int id, [FromBody] OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Response(false, "Invalid model state"));

            if (id <= 0)
                return BadRequest(new Response(false, "Invalid order ID provided"));

            if (orderDTO == null)
                return BadRequest(new Response(false, "Order data is required"));

            if (id != orderDTO.Id)
                return BadRequest(new Response(false, "Order ID mismatch"));

            try
            {
                // Check if order exists
                var existingOrder = await orderInterface.FindByIdAsync(id);
                if (existingOrder == null)
                    return NotFound(new Response(false, $"Order with ID {id} not found"));

                var orderEntity = OrderConversion.ToEntity(orderDTO);
                var response = await orderInterface.UpdateAsync(orderEntity);

                return response.Flag ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(false, $"Error updating order: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteOrder(int orderId)
        {
            if (orderId <= 0) return BadRequest(new Response(false, "Invalid order ID provided"));

            try
            {
                var existingOrder = await orderInterface.FindByIdAsync(orderId);
                if (existingOrder == null)
                    return NotFound(new Response(false, $"Order with ID {orderId} not found"));

                var response = await orderInterface.DeleteAsync(existingOrder);

                return response.Flag ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(false, $"Error deleting order: {ex.Message}"));
            }
        }
    }
}