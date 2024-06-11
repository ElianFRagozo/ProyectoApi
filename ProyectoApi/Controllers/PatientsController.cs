using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoApi.Models;
using ProyectoApi.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly HttpClient _httpClient; 
        private readonly IConfiguration _configuration;

        private readonly List<string> _validIdentificationTypes = new List<string>
        {
            "Cédula de Ciudadanía",
            "Tarjeta de Identidad",
            "Cédula de Extranjería",
            "Pasaporte"
        };

        public PatientsController(PatientService patientService, HttpClient httpClient, IConfiguration configuration)
        {
            _patientService = patientService;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_validIdentificationTypes.Contains(patientDto.IdentificationType))
            {
                return BadRequest("Tipo de identificación no válido.");
            }

            if (patientDto.Email != patientDto.ConfirmEmail)
            {
                ModelState.AddModelError("ConfirmEmail", "El correo electrónico no coincide.");
                return BadRequest(ModelState);
            }

            if (patientDto.Password != patientDto.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "La contraseña no coincide.");
                return BadRequest(ModelState);
            }

            var patient = new Patient
            {
                IdentificationType = patientDto.IdentificationType,
                IdentificationNumber = patientDto.IdentificationNumber,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth,
                Phone = patientDto.Phone,
                Email = patientDto.Email,
                Password = patientDto.Password,
                ConfirmEmail = patientDto.ConfirmEmail,
                ConfirmPassword = patientDto.ConfirmPassword,
                UserId = patientDto.UserId,
            };

            var createdPatientId = await _patientService.CreatePatientAsync(patient);

            List<string> role = DetermineUserRoles(patient);

            var userModel = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Email = patientDto.Email,
                Password = patientDto.Password,
                Roles = role
            };

            var result = await SendUserToUserApiAsync(userModel);
            if (!result.IsSuccessStatusCode)
            {
                return StatusCode((int)result.StatusCode, "Error al enviar datos a la API de usuarios.");
            }

            return CreatedAtAction(nameof(GetPatientById), new { id = createdPatientId }, new { Patient = patient });
        }

        private List<string> DetermineUserRoles(Patient patient)
        {
            List<string> role = new List<string> { "paciente" };
            return role;
        }
        

        private async Task<HttpResponseMessage> SendUserToUserApiAsync(UserModel userModel)
        {
            var userServiceUrl = _configuration["USER_SERVICE_URL"];
            var response = await _httpClient.PostAsJsonAsync(userServiceUrl, userModel);
            return response;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyDetails()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] PatientDto patientDto)
        {
            var patientToUpdate = await _patientService.GetPatientAsync(id.ToString());
            if (patientToUpdate == null)
            {
                return NotFound();
            }

            if (!_validIdentificationTypes.Contains(patientDto.IdentificationType))
            {
                return BadRequest("Tipo de identificación no válido.");
            }

            patientToUpdate.IdentificationType = patientDto.IdentificationType;
            patientToUpdate.IdentificationNumber = patientDto.IdentificationNumber;
            patientToUpdate.FirstName = patientDto.FirstName;
            patientToUpdate.LastName = patientDto.LastName;
            patientToUpdate.DateOfBirth = patientDto.DateOfBirth;
            patientToUpdate.Phone = patientDto.Phone;
            patientToUpdate.Email = patientDto.Email;
            patientToUpdate.Password = patientDto.Password;
            patientToUpdate.ConfirmEmail = patientDto.ConfirmEmail;
            patientToUpdate.ConfirmPassword = patientDto.ConfirmPassword;

            await _patientService.UpdatePatientAsync(id.ToString(), patientToUpdate);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var patient = await _patientService.GetPatientAsync(id.ToString());
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPatientByUserId(string userId)
        {
            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }
    }
}