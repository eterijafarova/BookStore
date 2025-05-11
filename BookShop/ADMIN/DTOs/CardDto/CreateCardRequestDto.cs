using System;
using System.ComponentModel.DataAnnotations;

namespace BookShop.ADMIN.DTOs.CardDto
{
    public class CreateCardRequestDto
    {
        [Required] [CreditCard] public string CardNumber { get; set; }

        [Required] public string CardHolderName { get; set; }

        [Required] [DataType(DataType.Date)] public DateTime ExpirationDate { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; }

        [Required] public Guid UserId { get; set; }
    }
}