using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Models;
using ProyectoApi.Services;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : Controller
    {

        private readonly CitaService _citaService;
        private readonly UserService _userService;
        private readonly PatientService _patientService;

        public CitaController (CitaService citaService, UserService userService)
        {
            _citaService = citaService;
            _userService = userService;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CrearCita(CitaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState + "PRIMER FILTRO");
            }

            var user = await _userService.GetUserAsync(dto.MedicoId);
            if (user == null)
            {
                ModelState.AddModelError("IdMedico", "El médico no ha sido registrado.");
                return BadRequest(ModelState + "SEGUNDO FILTRO");
            }
           /* var paciente = await _patientService.GetPatientAsync(dto.PacienteId);
            if (user == null)
            {
                ModelState.AddModelError("IdMedico", "El paciente no ha sido registrado.");
                return BadRequest(ModelState + "TERCER FILTRO");
            }*/


             try
             {
                 var resultado = await _citaService.GetCitaEstadoAsync(dto.MedicoId, dto.dia, dto.HoraInicio);
                if (resultado != null)
                {
                    return BadRequest("No hay horarios disponibles para la fecha y hora seleccionadas.");
                }
                else
                {
                    var cita = new Cita
                    {
                        MedicoId = dto.MedicoId,
                        PacienteId = dto.PacienteId,
                        HoraInicio = dto.HoraInicio,
                        HoraFin = dto.HoraFin,
                        Estado= "registrado"
                    };

                    await _citaService.CreateCitaAsync(cita);
                }

                 return Ok("Cita creada exitosamente.");
             }
             catch (Exception ex)
             {
                 return StatusCode(500, $"Error interno del servidor: {ex.Message}");
             }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCitas()
        {
            var citas = await _citaService.GetCitasAsync();
            return Ok(citas);
        }

    }
}
