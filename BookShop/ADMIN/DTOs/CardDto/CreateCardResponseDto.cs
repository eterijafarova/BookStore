namespace BookShop.ADMIN.DTOs.CardDto;

public class CreateCardResponseDto
{
    public Guid Id { get; set; }
    public string Last4Digits { get; set; }
    public string CardHolderName { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid UserId { get; set; }
}