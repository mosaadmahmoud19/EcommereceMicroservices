using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTO
{
    public record OrderDetailsDTO(

        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int Client,
        [Required] string Name,
        [Required,EmailAddress] string Email,
        [Required, EmailAddress] string Adress,
        [Required] string TelephoneNumber,
        [Required] string ProductName,
        [Required] int PurchaseQuantity,
        [Required,DataType(DataType.Currency)] int UnitPrice,
        [Required,DataType(DataType.Currency)] int TotalPrice,
        [Required] DateTime OrderDate




        );
   
}
