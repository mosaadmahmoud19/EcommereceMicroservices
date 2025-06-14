
using OrderApi.Application.DTO;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
       Task<IEnumerable<OrderDTO>> GetOrderByClientId(int clientId);

       Task<OrderDetailsDTO> GetOrderDetails(int orderId);
    }
}
