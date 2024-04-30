using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProyectoApi.Models;
using ProyectoApi.Services;
using System.Diagnostics.Eventing.Reader;


namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly TokenService _tokenService;

        // Lista estática de tipos de identificación válidos
        private readonly List<string> _validIdentificationTypes = new List<string>
        {
            "Cédula de Ciudadanía",
            "Tarjeta de Identidad",
            "Cédula de Extranjería",
            "Pasaporte"
        };

        public PatientsController(PatientService patientService, TokenService tokenService)
        {
            _patientService = patientService;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validamos el tipo de identificación
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

            // Determinar el rol del paciente
            string role = DetermineUserRole(patient);

            // Crear un objeto UserModel basado en la información del paciente
            var userModel = new UserModel
            {
                Email = patient.Email,
                // Añade cualquier otra propiedad necesaria para UserModel
            };

            // Generar el token JWT
            var token = _tokenService.GenerateToken(userModel, role);

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, new { Patient = patient, Token = token });
        }

        private string DetermineUserRole(Patient patient)
        {
            // Determina el rol del paciente aquí, por ejemplo, basado en sus propiedades
            string role = DetermineUserRole(patient);

            return role;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(String id, [FromBody] PatientDto patientDto)
        {
            var patientToUpdate = await _patientService.GetPatientAsync(id.ToString());
            if (patientToUpdate == null)
            {
                return NotFound();
            }

            // Validamos el tipo de identificación
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
        public async Task<IActionResult> GetPatientById(String id)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientAsync(string id)
        {
            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }

    }
}
