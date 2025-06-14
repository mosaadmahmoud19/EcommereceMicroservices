using OrderApi.Application.DTO;
using OrderApi.Application.DTO.Conversions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface , HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline
        ) : IOrderService
    {
        // GET Product
        public async Task<ProductDTO> GetProduct(int id)
        {
            // call product api using httpclient

            var getProduct = await httpClient.GetAsync($"/api/products/{id}");
            if (!getProduct.IsSuccessStatusCode) 
            {
                return null!;
            }

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        // GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/products/{userId}");
            if (!getUser.IsSuccessStatusCode) 
            {
                return null!;
            }
            var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return product!;
        }
        public Task<IEnumerable<OrderDTO>> GetOrderByClientId(int clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
           // prepare order 
           var order  = await orderInterface.FindByIdAsync(orderId);
            if(order is null || order!.Id <= 0) 
            {
                return null!;
            }

            // get retry pipeline
            //"If this operation fails (like calling an external service),
            //try again X times before giving up."
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // prepare product

            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // prepare client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDTO(
                
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate


                );
        }

        // GET ORDERS BY CLIENT ID

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // get all client orders

            var orders = await orderInterface.GetOrderAsync(o => o.ClientId == clientId);

            if (!orders.Any()) 
            {
                return null!;
            }

            //conver from entity to DTO

            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
