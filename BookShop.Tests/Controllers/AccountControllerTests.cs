using System;
using System.IO;
using System.Threading.Tasks;
using BookShop.Auth.ControllersAuth;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookShop.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _serviceMock;
        private readonly AccountController _controller;
        private readonly DefaultHttpContext _httpContext;

        public AccountControllerTests()
        {
            _serviceMock = new Mock<IAccountService>();
            _controller = new AccountController(_serviceMock.Object);
            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [Fact]
        public async Task RequestPasswordReset_ReturnsOk_OnSuccess()
        {
            _serviceMock.Setup(s => s.RequestPasswordResetAsync("email@e.com"))
                .Returns(Task.CompletedTask);
            var req = new RequestPasswordResetRequest { Email = "email@e.com" };
            
            var result = await _controller.RequestPasswordReset(req) as OkObjectResult;
            
            Assert.NotNull(result);
            dynamic body = result.Value;
            Assert.Equal("Password reset link sent to your email.", (string)body.message);
        }

        [Fact]
        public async Task RequestPasswordReset_ReturnsBadRequest_OnError()
        {
            _serviceMock.Setup(s => s.RequestPasswordResetAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("User not found."));
            var req = new RequestPasswordResetRequest { Email = "no@e.com" };
            
            var result = await _controller.RequestPasswordReset(req) as BadRequestObjectResult;
            
            Assert.NotNull(result);
            dynamic body = result.Value;
            Assert.Equal("User not found.", (string)body.message);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_OnSuccess()
        {
            _serviceMock.Setup(s => s.ResetPasswordAsync(It.IsAny<ResetPasswordRequest>()))
                .Returns(Task.CompletedTask);
            var req = new ResetPasswordRequest { Token = "t", NewPassword = "p" };
            
            var result = await _controller.ResetPassword(req) as OkObjectResult;
            
            Assert.NotNull(result);
            dynamic body = result.Value;
            Assert.Equal("Password reset successfully.", (string)body.message);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_OnError()
        {
            // Arrange
            _serviceMock.Setup(s => s.ResetPasswordAsync(It.IsAny<ResetPasswordRequest>()))
                .ThrowsAsync(new Exception("Invalid token"));
            var req = new ResetPasswordRequest { Token = "bad", NewPassword = "p" };

            // Act
            var result = await _controller.ResetPassword(req) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            dynamic body = result.Value;
            Assert.Equal("Invalid token", (string)body.message);
        }

        [Fact]
        public async Task Register_ReturnsOk_OnValidRequest()
        {
            _serviceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
                .Returns(Task.CompletedTask);
            _serviceMock.Setup(s => s.ConfirmEmailAsync(It.IsAny<ConfirmRequest>()))
                .Returns(Task.CompletedTask);
            var req = new RegisterRequest("u","e","p","p");
            
            var result = await _controller.Register(req) as OkObjectResult;
            
            Assert.NotNull(result);
            var wrapper = Assert.IsType<Result<string>>(result.Value);
            Assert.True(wrapper.IsSuccess);
            Assert.Equal("u", wrapper.Data);
        }

        [Fact]
        public async Task VerifyEmailAsync_ReturnsOk()
        {
            _serviceMock.Setup(s => s.VerifyEmailAsync("token")).Returns(Task.CompletedTask);
            
            var result = await _controller.VerifyEmailAsync("token") as OkObjectResult;
            
            Assert.NotNull(result);
            var wrapper = Assert.IsType<Result<string>>(result.Value);
            Assert.True(wrapper.IsSuccess);
            Assert.Equal("Email confirmed", wrapper.Data);
        }

        [Fact]
        public async Task ConfirmEmailPage_RedirectsToSuccess_OnValid()
        {
            _serviceMock.Setup(s => s.VerifyEmailAsync("tk")).Returns(Task.CompletedTask);
            
            var result = await _controller.ConfirmEmailPage("tk") as RedirectResult;
            
            Assert.NotNull(result);
            Assert.Equal("/confirm-success.html", result.Url);
        }

        [Fact]
        public async Task ConfirmEmailPage_RedirectsToError_OnException()
        {
            _serviceMock.Setup(s => s.VerifyEmailAsync("tk")).ThrowsAsync(new Exception());
            
            var result = await _controller.ConfirmEmailPage("tk") as RedirectResult;
            
            Assert.NotNull(result);
            Assert.Equal("/confirm-error.html", result.Url);
        }

        [Fact]
        public async Task ResetPasswordPage_ReturnsBadRequest_IfNoToken()
        {
            var result = await _controller.ResetPasswordPage(null) as BadRequestObjectResult;
            
            Assert.NotNull(result);
            dynamic body = result.Value;
            Assert.Equal("Token is required.", (string)body.message);
        }

        [Fact]
        public async Task ResetPasswordPage_ReturnsBadRequest_IfInvalidToken()
        {
            _serviceMock.Setup(s => s.ValidatePasswordResetTokenAsync("tk")).ReturnsAsync(false);
            
            var result = await _controller.ResetPasswordPage("tk") as BadRequestObjectResult;
            
            Assert.NotNull(result);
            dynamic body = result.Value;
            Assert.Equal("Invalid or expired token.", (string)body.message);
        }

        [Fact]
        public async Task ResetPasswordPage_ReturnsFile_IfValidToken()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, "reset-password.html"), "<html/>");

            _serviceMock.Setup(s => s.ValidatePasswordResetTokenAsync("tk")).ReturnsAsync(true);
            
            var result = await _controller.ResetPasswordPage("tk") as PhysicalFileResult;
            
            Assert.NotNull(result);
            Assert.Equal("text/html", result.ContentType);
            Assert.EndsWith("reset-password.html", result.FileName);
        }
    }
}
