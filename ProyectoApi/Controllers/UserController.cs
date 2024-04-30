using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using ProyectoApi.Models;
using ProyectoApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace Proyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 

    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        private readonly TokenService _tokenService;

        public UsersController(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService; 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Consulta el usuario en base de datos por email
            var user = await _userService.GetUserByEmailAsync(loginModel.Email);
            if (user == null || user.Password != loginModel.Password)
            {
                return Unauthorized();
            }

            // Determina el tipo de usuario (role)
            string role = DetermineUserRole(user);

            // Genera el token JWT con el tipo de usuario
            var token = _tokenService.GenerateToken(user, role);

            return Ok(new { Token = token });
        }

        // Método para determinar el tipo de usuario
        private string DetermineUserRole(UserModel user)
        {
            // Aquí puedes implementar la lógica para determinar el tipo de usuario
            // basado en sus roles u otros criterios. Por ejemplo:
            if (user.Roles.Contains("admin"))
            {
                return "admin";
            }
            else if (user.Roles.Contains("medico"))
            {
                return "medico";
            }
            else
            {
                return "paciente";
            }
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

            string role = DetermineUserRole(userModel);

            // Generar el token JWT
            var token = _tokenService.GenerateToken(userModel, role);

            // Imprimir el token en la consola para visualización
            Console.WriteLine("Token generado: " + token);

            // Retornar una respuesta con el nuevo usuario creado
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
