﻿namespace BookShop.Data.Models;

public class User
{
    public int Id { get; set; } 

    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public Guid RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; } = DateTime.Now.AddDays(7);

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}