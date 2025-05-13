namespace BookShop.ADMIN.DTOs
{
    public class PromoCodeResponseDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } 
    }
    public class CreatePromoCodeDTO
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
