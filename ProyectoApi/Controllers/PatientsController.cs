using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Models;
using ProyectoApi.Services;


namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly HttpClient _httpClient; 

        
        private readonly List<string> _validIdentificationTypes = new List<string>
        {
            "Cédula de Ciudadanía",
            "Tarjeta de Identidad",
            "Cédula de Extranjería",
            "Pasaporte"
        };

        public PatientsController(PatientService patientService, HttpClient httpClient)
        {
            _patientService = patientService;
            _httpClient = httpClient; 
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
                ConfirmPassword = patientDto.ConfirmPassword
            };

            await _patientService.CreatePatientAsync(patient);

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

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, new { Patient = patient });
        }

        private List<string> DetermineUserRoles(Patient patient)
        {
            
            List<string> role = new List<string>();
            role.Add("paciente");
            return role;
        }

        private async Task<HttpResponseMessage> SendUserToUserApiAsync(UserModel userModel)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:7127/api/Users", userModel);
            return response;
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
    }
}
