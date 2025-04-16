namespace BookShop.ADMIN.DTOs.PublisherDto
{
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class UpdatePublisherDto: CreatePublisherDto
    {
        public int Id { get; set; }
    }

    public class CreatePublisherDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
    
}
