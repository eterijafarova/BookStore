using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Data;
using BookShop.Data.Models;
using BookShop.ADMIN.DTOs.OrderDto;
using BookShop.Data.Contexts;
using BookShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly LibraryContext _context;

        public OrderService(LibraryContext context)
        {
            _context = context;
        }

       public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequest)
{
    var addressExists = await _context.Adresses
        .AnyAsync(a => a.Id == orderRequest.UserAddressId && a.UserId == orderRequest.UserId);
    if (!addressExists)
        throw new KeyNotFoundException("Address not found or does not belong to user.");

    var cardExists = await _context.BankCards
        .AnyAsync(c => c.Id == orderRequest.UserBankCardId && c.UserId == orderRequest.UserId);
    if (!cardExists)
        throw new KeyNotFoundException("Bank card not found or does not belong to user.");


    decimal totalPrice = 0;
    var orderItems = new List<OrderItem>();
    foreach (var item in orderRequest.OrderItems)
    {
        var book = await _context.Books.FindAsync(item.BookId);
        if (book == null) throw new Exception("Book not found");
        if (book.Stock < item.Quantity)
            throw new InvalidOperationException($"Not enough stock for '{book.Title}'");

        totalPrice += book.Price * item.Quantity;
        book.Stock -= item.Quantity;

        orderItems.Add(new OrderItem
        {
            BookId = item.BookId,
            Quantity = item.Quantity,
            Price = book.Price
        });
    }


    var order = new Order
    {
        UserId         = orderRequest.UserId,
        UserAdressId   = orderRequest.UserAddressId,
        UserBankCardId = orderRequest.UserBankCardId,
        TotalPrice     = totalPrice,
        Status         = Order.OrderStatus.Pending,
        OrderDate      = DateTime.UtcNow,
        OrderItems     = orderItems
    };

    _context.Orders.Add(order);
    await _context.SaveChangesAsync();
    
    return new OrderResponseDto(
        order.Id,
        order.UserId,
        order.TotalPrice,
        order.Status.ToString(),
        order.OrderDate,
        order.OrderItems.Select(i => new OrderItemResponseDto
        {
            BookId   = i.BookId,
            Quantity = i.Quantity,
            Price    = i.Price
        }).ToList()
    );
}
       
        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)  
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null; 
            }

            return new OrderResponseDto(
                order.Id,
                order.UserId,
                order.TotalPrice,
                order.Status.ToString(),
                order.OrderDate,
                order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            );
        }
        
        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)  
                .ToListAsync();

            return orders.Select(order => new OrderResponseDto(
                order.Id,
                order.UserId,
                order.TotalPrice,
                order.Status.ToString(),
                order.OrderDate,
                order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            ));
        }
        
        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;  
            }

            _context.Orders.Remove(order);  
            await _context.SaveChangesAsync();  
            return true;
        }
        
        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Order.OrderStatus status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;  
            }

            order.Status = status;  
            await _context.SaveChangesAsync();  
            return true;
        }
    }
}
