using System;
using System.Threading.Tasks;
using AutoMapper;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ModelsAuth;
using BookShop.Auth.ServicesAuth.Classes;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BookShop.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly LibraryContext _context;
        private readonly Mock<ITokenService> _tokenMock;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            // In-memory DB
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase("AcctSvcTests_" + Guid.NewGuid())
                .Options;
            _context = new LibraryContext(options);

            // Seed AppUser role
            _context.Roles.Add(new Role { Id = Guid.NewGuid(), RoleName = "AppUser" });
            // Seed a user for reset tests
            _context.Users.Add(new User {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "user@e.com",
                PasswordHash = "old"
            });
            _context.SaveChanges();

            // Mock token service
            _tokenMock = new Mock<ITokenService>();
            _tokenMock.Setup(x => x.CreatePasswordResetTokenAsync("testuser")).ReturnsAsync("reset-token");
            _tokenMock.Setup(x => x.ValidatePasswordResetTokenAsync("reset-token")).ReturnsAsync(true);
            _tokenMock.Setup(x => x.GetNameFromToken("reset-token")).ReturnsAsync("testuser");

            // Minimal mapper
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterRequest, User>()
                   .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()));
            }).CreateMapper();

            // In-memory config for WebAppUrl
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[] {
                    new KeyValuePair<string, string>("AppSettings:WebAppUrl", "https://test")
                }).Build();

            _service = new AccountService(mapper, _context, config, _tokenMock.Object);
        }

        [Fact]
        public async Task RequestPasswordResetAsync_CreatesTokenRecord()
        {
            // Act: expect exception but token record persisted
            Func<Task> act = () => _service.RequestPasswordResetAsync("user@e.com");

            // swallow exception
            await act.Should().ThrowAsync<Exception>();

            // Assert token record
            var tokenEntry = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.UserName == "testuser");
            tokenEntry.Should().NotBeNull();
            tokenEntry.Token.Should().Be("reset-token");
        }

        [Fact]
        public async Task ValidatePasswordResetTokenAsync_ReturnsTrueForValidToken()
        {
            // Arrange: add token record
            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                Token = "T1",
                UserName = "u",
                ExpiryDate = DateTime.UtcNow.AddMinutes(10)
            });
            await _context.SaveChangesAsync();

            // Act & Assert
            var ok = await _service.ValidatePasswordResetTokenAsync("T1");
            ok.Should().BeTrue();
        }

        [Fact]
        public async Task ValidatePasswordResetTokenAsync_ReturnsFalseForExpiredOrMissing()
        {
            // missing
            var miss = await _service.ValidatePasswordResetTokenAsync("none");
            miss.Should().BeFalse();

            // expired
            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                Token = "T2",
                UserName = "u2",
                ExpiryDate = DateTime.UtcNow.AddMinutes(-10)
            });
            await _context.SaveChangesAsync();

            var expired = await _service.ValidatePasswordResetTokenAsync("T2");
            expired.Should().BeFalse();
        }

        [Fact]
        public async Task ResetPasswordAsync_WithValidToken_UpdatesPassword()
        {
            // Arrange
            var req = new ResetPasswordRequest
            {
                Token = "reset-token",
                NewPassword = "newPass123"
            };

            // Act
            await _service.ResetPasswordAsync(req);

            // Assert password changed
            var user = await _context.Users.FirstAsync(u => u.UserName == "testuser");
            user.PasswordHash.Should().NotBe("old");
        }

        [Fact]
        public async Task ResetPasswordAsync_WithInvalidToken_ThrowsException()
        {
            // Arrange: invalid token
            _tokenMock.Setup(x => x.ValidatePasswordResetTokenAsync("bad")).ReturnsAsync(false);
            var req = new ResetPasswordRequest
            {
                Token = "bad",
                NewPassword = "p"
            };

            // Act & Assert
            Func<Task> act = () => _service.ResetPasswordAsync(req);
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Invalid or expired password reset token.");
        }
    }
}
