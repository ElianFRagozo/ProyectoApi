using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Models;
using ProyectoApi.Services;


namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HorarioController : ControllerBase
    {
        private readonly HorarioService _horarioService;
        private readonly UserService _userService;

        public HorarioController(HorarioService horarioService, UserService userService)
        {
            _horarioService = horarioService;
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateHorario(HorarioDto horariodto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Aquí deberías await el método GetUserAsync para obtener el resultado correctamente
            var user = await _userService.GetUserAsync(horariodto.IdMedico);
            if (user == null)
            {
                ModelState.AddModelError("IdMedico", "El médico no ha sido registrado.");
                return BadRequest(ModelState);
            }

            var horarioExistente = await _horarioService.GetHorariosAsync(horariodto.IdMedico, horariodto.FechaHora);

            if (horarioExistente != null)
            {
                ModelState.AddModelError("horario", "Error al registrar el horario.");
                return BadRequest(ModelState);
            }

            var horario = new Horario
            {
                IdMedico = horariodto.IdMedico,
                FechaHora = horariodto.FechaHora
            };

            await _horarioService.CreateHorarioAsync(horario);

            // Aquí, en lugar de CreatedAtAction, puedes simplemente devolver el objeto creado
            return Ok(horario);
        }

        [HttpGet("{idHorario}")]
        public async Task<IActionResult> GetHorario(string idHorario)
        {
            var horario = await _horarioService.GetHorarioIDAsync(idHorario);
            if (horario == null)
            {
                return NotFound();
            }

            return Ok(horario);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHorario()
        {
            var horarios = await _horarioService.GetHorariosAsync();
            return Ok(horarios);
        }

    }
}
