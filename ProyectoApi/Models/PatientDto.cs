namespace ProyectoApi.Models
{
        public class PatientDto
        {
            public string IdentificationType { get; set; }
            public string IdentificationNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
        }
    }
