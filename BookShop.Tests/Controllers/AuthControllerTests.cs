using System;
using System.Threading.Tasks;
using BookShop.Auth.ControllersAuth;
using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.DTOAuth.Responses;
using BookShop.Auth.ServicesAuth.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookShop.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthController _controller;
        private readonly DefaultHttpContext _httpContext;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _controller = new AuthController(_authServiceMock.Object, _tokenServiceMock.Object);

            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [Fact]
        public async Task Login_SetsCookiesAndReturnsOk()
        {
            var userId = Guid.NewGuid();
            var loginResp = new LoginResponse("access123", "refresh123", "User", userId);
            _authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResp);
            var req = new LoginRequest("bob", "pwd");
            
            var resultObj = await _controller.Login(req) as OkObjectResult;
            
            Assert.NotNull(resultObj);
            var wrapper = Assert.IsType<Result<LoginResponse>>(resultObj.Value);
            Assert.True(wrapper.IsSuccess);
            Assert.Equal(loginResp.AccessToken, wrapper.Data.AccessToken);
            Assert.Equal(loginResp.RefreshToken, wrapper.Data.RefreshToken);
            Assert.Equal(loginResp.Role, wrapper.Data.Role);
            Assert.Equal(loginResp.UserId, wrapper.Data.UserId);

            
            var setCookie = _httpContext.Response.Headers["Set-Cookie"].ToString();
            Assert.Contains("accessToken=access123", setCookie);
            Assert.Contains("refreshToken=refresh123", setCookie);
        }

        [Fact]
        public async Task Refresh_SetsCookiesAndReturnsOk()
        {
            _httpContext.Request.Headers["Cookie"] = "accessToken=oldAcc; refreshToken=oldRef";
            _tokenServiceMock
                .Setup(t => t.GetNameFromToken("oldAcc"))
                .ReturnsAsync("alice");

            var newTokens = new RefreshTokenResponse("newAcc", "newRef");
            _authServiceMock
                .Setup(s => s.RefreshTokenAsync(
                    It.Is<RefreshTokenRequest>(r => r.username == "alice" && r.refreshToken == "oldRef")
                ))
                .ReturnsAsync(newTokens);
            
            var resultObj = await _controller.Refresh() as OkObjectResult;


            Assert.NotNull(resultObj);
            var wrapper = Assert.IsType<Result<RefreshTokenResponse>>(resultObj.Value);
            Assert.True(wrapper.IsSuccess);
            Assert.Equal(newTokens.accessToken, wrapper.Data.accessToken);
            Assert.Equal(newTokens.refreshToken, wrapper.Data.refreshToken);

            var setCookie = _httpContext.Response.Headers["Set-Cookie"].ToString();
            Assert.Contains("accessToken=newAcc", setCookie);
            Assert.Contains("refreshToken=newRef", setCookie);
        }

        [Fact]
        public async Task Test_ReturnsOkString()
        {
            var resultObj = await _controller.Test() as OkObjectResult;
            
            Assert.NotNull(resultObj);
            Assert.Equal("Test", resultObj.Value);
        }

        [Fact]
        public void Logout_DeletesCookiesAndReturnsOk()
        {
            var resultObj = _controller.Logout().Result as OkObjectResult;
            
            Assert.NotNull(resultObj);
            Assert.Equal("Logout", resultObj.Value);
            
            var setCookie = _httpContext.Response.Headers["Set-Cookie"].ToString();
            Assert.Contains("accessToken=;", setCookie);
            Assert.Contains("refreshToken=;", setCookie);
        }
    }
}