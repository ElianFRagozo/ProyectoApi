using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using ProyectoApi.Models;
using ProyectoApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Consulta el usuario en base de datos por email
            var user = await _userService.GetUserByEmailAsync(loginModel.Email);
            if (user == null || user.Password != loginModel.Password)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Generar un nuevo ObjectId
            var newId = ObjectId.GenerateNewId();

            // Asignar el nuevo ObjectId al campo Id del UserModel
            userModel.Id = newId.ToString();

            // Crear el usuario en la base de datos
            await _userService.CreateUserAsync(userModel);

            return CreatedAtAction(nameof(GetUserByEmail), new { email = userModel.Email }, userModel);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }
    }
}