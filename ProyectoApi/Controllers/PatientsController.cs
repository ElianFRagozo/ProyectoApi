﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProyectoApi.Models;
using ProyectoApi.Services;


namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;

        // Lista estática de tipos de identificación válidos
        private readonly List<string> _validIdentificationTypes = new List<string>
        {
            "Cédula de Ciudadanía",
            "Tarjeta de Identidad",
            "Cédula de Extranjería",
            "Pasaporte"
        };

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
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

            var patient = new Patient
            {
                IdentificationType = patientDto.IdentificationType,
                IdentificationNumber = patientDto.IdentificationNumber,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth
            };

            await _patientService.CreatePatientAsync(patient);

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
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