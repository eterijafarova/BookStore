using AutoMapper;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ModelsAuth;
using BookShop.Auth.ServicesAuth.Classes;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Data.Contexts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BookShop.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly LibraryContext _context;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase("AuthServiceTestsDb_" + Guid.NewGuid())
                .Options;
            _context = new LibraryContext(options);

            _tokenServiceMock = new Mock<ITokenService>();
            var mapperConfig = new MapperConfiguration(cfg => { });
            _mapper = mapperConfig.CreateMapper();

            _authService = new AuthService(_context, _mapper, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WhenUserExistsAndHasRole_ReturnsLoginResponseAndUpdatesRefreshToken()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "john",
                RefreshToken = "oldToken",
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(1)
            };
            var role = new Role { Id = Guid.NewGuid(), RoleName = "User" };
            var userRole = new UserRole { UserId = user.Id, RoleId = role.Id, Role = role };
            _context.Roles.Add(role);
            _context.Users.Add(user);
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            var loginReq = new LoginRequest("john", "ignored");
            _tokenServiceMock.Setup(x => x.CreateTokenAsync("john")).ReturnsAsync("access-token");
            
            var response = await _authService.LoginAsync(loginReq);
            
            response.Should().NotBeNull();
            response.AccessToken.Should().Be("access-token");
            response.RefreshToken.Should().NotBeNullOrEmpty();
            response.Role.Should().Be("User");
            response.UserId.Should().Be(user.Id);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            updatedUser.RefreshToken.Should().Be(response.RefreshToken);
            updatedUser.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ThrowsException()
        {
            var loginReq = new LoginRequest("nouser", "pwd123!dd");
            
            Func<Task> act = () => _authService.LoginAsync(loginReq);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
        }

        [Fact]
        public async Task LoginAsync_WhenUserHasNoRole_ThrowsException()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "jane",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var loginReq = new LoginRequest("jane", "pwd");
            
            Func<Task> act = () => _authService.LoginAsync(loginReq);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("User role not found");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenValidOldToken_ReturnsNewTokens()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "alice",
                RefreshToken = "oldtoken",
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(1)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var refreshReq = new RefreshTokenRequest("alice", "oldtoken");
            _tokenServiceMock.Setup(x => x.CreateTokenAsync("alice")).ReturnsAsync("new-access");
            
            var response = await _authService.RefreshTokenAsync(refreshReq);
            
            response.accessToken.Should().Be("new-access");
            response.refreshToken.Should().NotBe("oldtoken");

            var updatedUser = await _context.Users.FindAsync(user.Id);
            updatedUser.RefreshToken.Should().Be(response.refreshToken);
            updatedUser.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenUserNotFound_ThrowsException()
        {
            var refreshReq = new RefreshTokenRequest("nouser", "token");
            
            Func<Task> act = () => _authService.RefreshTokenAsync(refreshReq);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenOldTokenInvalid_ThrowsException()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "bob",
                RefreshToken = "othertoken",
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(1)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var refreshReq = new RefreshTokenRequest("bob", "wrongtoken");
            
            Func<Task> act = () => _authService.RefreshTokenAsync(refreshReq);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Invalid refresh token");
        }
    }
}
