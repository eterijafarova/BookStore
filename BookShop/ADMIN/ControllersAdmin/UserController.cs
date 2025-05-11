using BookShop.ADMIN.DTOs;
using BookShop.ADMIN.DTOs.PublisherDto;
using BookShop.Auth.ServicesAuth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ADMIN.ControllersAdmin
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/{userId}
        // Получение пользователя по Guid
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // GET: api/User?page=1&pageSize=10
        // Получение списка пользователей с пагинацией
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetUsersAsync(page, pageSize);
            return Ok(users);
        }

        // PUT: api/User/{userId}
        // Обновление пользователя
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
            if (updatedUser == null)
                return NotFound();
            return Ok(updatedUser);
        }

        // DELETE: api/User/{userId}
        // Удаление пользователя
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}