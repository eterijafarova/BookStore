using AutoMapper;
using BookShop.ADMIN.DTOs.OrderDto;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly LibraryContext _context;
        private readonly IPromoCodeService _promoService;
        private readonly IMapper _mapper;

        public OrderService(LibraryContext context, IPromoCodeService promoService, IMapper mapper)
        {
            _context = context;
            _promoService = promoService;
            _mapper = mapper;
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


            decimal originalPrice = 0m;
            var items = new List<OrderItem>();
            foreach (var item in orderRequest.OrderItems)
            {
                var book = await _context.Books.FindAsync(item.BookId)
                           ?? throw new Exception("Book not found");
                if (book.Stock < item.Quantity)
                    throw new InvalidOperationException($"Not enough stock for '{book.Title}'");

                originalPrice += book.Price * item.Quantity;
                book.Stock -= item.Quantity;

                items.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = book.Price
                });
            }


            decimal discountAmount = 0m;
            if (!string.IsNullOrEmpty(orderRequest.PromoCode))
            {
                var promo = await _promoService.GetPromoCodeAsync(orderRequest.PromoCode);
                if (promo == null || !promo.IsActive)
                    throw new InvalidOperationException("Invalid or expired promo code");

                discountAmount = Math.Round(originalPrice * promo.Discount / 100m, 2);
            }

            decimal finalPrice = originalPrice - discountAmount;


            var order = new Order
            {
                UserId = orderRequest.UserId,
                UserAdressId = orderRequest.UserAddressId,
                UserBankCardId = orderRequest.UserBankCardId,

                OriginalPrice = originalPrice,
                PromoCode = orderRequest.PromoCode,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice,

                Status = Order.OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                OrderItems = items
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                UserAddressId = order.UserAdressId,
                UserBankCardId = order.UserBankCardId,
                OriginalPrice = order.OriginalPrice,
                PromoCode = order.PromoCode,
                DiscountAmount = order.DiscountAmount,
                FinalPrice = order.FinalPrice,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems
                    .Select(i => new OrderItemResponseDto
                    {
                        BookId = i.BookId,
                        Quantity = i.Quantity,
                        UnitPrice = i.Price
                    })
                    .ToList()
            };
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            return new OrderResponseDto
            {
                Id = order.Id,
                UserAddressId = order.UserAdressId,
                UserBankCardId = order.UserBankCardId,
                OriginalPrice = order.OriginalPrice,
                PromoCode = order.PromoCode,
                DiscountAmount = order.DiscountAmount,
                FinalPrice = order.FinalPrice,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems
                    .Select(i => new OrderItemResponseDto
                    {
                        BookId = i.BookId,
                        Quantity = i.Quantity,
                        UnitPrice = i.Price
                    })
                    .ToList()
            };
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ToListAsync();

            return orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                UserAddressId = order.UserAdressId,
                UserBankCardId = order.UserBankCardId,
                OriginalPrice = order.OriginalPrice,
                PromoCode = order.PromoCode,
                DiscountAmount = order.DiscountAmount,
                FinalPrice = order.FinalPrice,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems
                    .Select(i => new OrderItemResponseDto
                    {
                        BookId = i.BookId,
                        Quantity = i.Quantity,
                        UnitPrice = i.Price
                    })
                    .ToList()
            });
        }

        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Order.OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
            
            var result = orders.Select(o => new OrderResponseDto
            {
                Id             = o.Id,
                OriginalPrice  = o.OriginalPrice,
                PromoCode      = o.PromoCode,
                DiscountAmount = o.DiscountAmount,
                FinalPrice     = o.FinalPrice,
                OrderDate      = o.OrderDate,
                Status         = o.Status.ToString(),
                UserAddressId  = o.UserAdressId,
                UserBankCardId = o.UserBankCardId,

                OrderItems = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        BookId    = oi.BookId,
                        Quantity  = oi.Quantity,
                        UnitPrice = oi.Price
                    })
                    .ToList()
            });

            return result;
        }
    }
}
