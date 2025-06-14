
using System.ComponentModel.DataAnnotations;


namespace OrderApi.Application.DTO
{
    public record  OrderDTO(
        int id,
        [Required , Range(1,int.MaxValue)] int productId,
        [Required, Range(1, int.MaxValue)] int ClientId,
        [Required, Range(1, int.MaxValue)] int PurchaseQuantity,
        DateTime OrderDate



        );
    
}
