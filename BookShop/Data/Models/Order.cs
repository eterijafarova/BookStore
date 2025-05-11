using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookShop.Auth.ModelsAuth;

namespace BookShop.Data.Models
{
    public class Order
    {
        public Guid Id { get; set; }  

        public Guid UserId { get; set; }  

        [Required]
        public decimal TotalPrice { get; set; }  

        public OrderStatus Status { get; set; } = OrderStatus.Pending;  

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;  
        
        public User User { get; set; } 

        // Связь с адресом пользователя (если есть)
        public Guid UserAdressId { get; set; }  
        public Adress UserAdress { get; set; }
        
        public Guid UserBankCardId { get; set; }
        public BankCard UserBankCard { get; set; }
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public enum OrderStatus
        {
            Pending,    // Ожидает
            Paid,       // Оплачено
            Shipped,    // Отправлено
            Completed,  // Завершено
            Canceled    // Отменено
        }
    }
}