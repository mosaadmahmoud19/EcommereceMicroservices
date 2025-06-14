using eCommerece.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTO;
using OrderApi.Application.DTO.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrder orderInterface , IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders() 
        {
            var orders = await orderInterface.GetAllAsync();

            if (!orders.Any()) 
            {
                return NotFound("No Order detected in the database");

            }
            var (_, list) = OrderConversion.FromEntity(null, orders);

            return !list!.Any() ? NotFound() :Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id) 
        {
            var order = await orderInterface.FindByIdAsync(id);

            if(order is null) 
            {
                return NotFound(null);
            }

            var (_order, _) = OrderConversion.FromEntity(order, null);

            return Ok(order);

        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId) 
        {
            if(clientId <= 0)
            {
                return BadRequest("Invalid data Provided");

            }
            var orders = await orderInterface.GetOrderAsync(o=>o.ClientId == clientId);

            return !orders.Any() ? NotFound(null) : Ok(orders);

        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDtails(int orderId) 
        {
            if(orderId <= 0) 
            {
              return BadRequest("Invalid data provided");
            }

            var orderDetail = await orderService.GetOrderDetails(orderId);

            return orderDetail.OrderId > 0 ? Ok(orderDetail) : NotFound("No Order Found");


        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest("Incomplete data submitted");
            }

            var getEntity = OrderConversion.ToEntity(orderDTO);

            var response = await orderInterface.CreateAsync(getEntity);

            return response.Flag ? Ok(response) : BadRequest(response);
        }


        [HttpPut]

        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO) 
        {
            // convert from dto to entity

            var order = OrderConversion.ToEntity(orderDTO);

            var response = await orderInterface.UpdateAsync(order);

            return response.Flag ? Ok(response) :BadRequest(response);

        }

        [HttpDelete]

        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            //conver from dto to entity

            var order = OrderConversion.ToEntity(orderDTO);

            var response = await orderInterface.DeleteAsync(order);

            return response.Flag ? Ok(response):BadRequest(response);
        }
    }
}

