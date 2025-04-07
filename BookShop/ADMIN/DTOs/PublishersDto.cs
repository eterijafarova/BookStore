using static BookShop.ADMIN.DTOs.UpdateBookDto;
namespace BookShop.ADMIN.DTOs;

public class PublisherDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public List<UpdateBookDto> Books { get; set; }  
}
