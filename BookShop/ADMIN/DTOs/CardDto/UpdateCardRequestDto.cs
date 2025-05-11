using System.ComponentModel.DataAnnotations;

namespace BookShop.ADMIN.DTOs.CardDto;

public abstract class UpdateCardRequestDto
{
    [Required]
    public Guid Id { get; set; }

    [CreditCard]
    public string CardNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ExpirationDate { get; set; }

    [StringLength(4, MinimumLength = 3)]
    public string CVV { get; set; }

    public string CardHolderName { get; set; }
}