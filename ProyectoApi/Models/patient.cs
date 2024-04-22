using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProyectoApi.Models
{
    public class Patient
    {
        [BsonId] // Especifica que esta propiedad es el Id en MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Indica que el tipo de representación en la base de datos es ObjectId
        public string Id { get; set; }

        [Required(ErrorMessage = "El tipo de identificación es obligatorio.")]
        public string IdentificationType { get; set; }

        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        public string IdentificationNumber { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [BsonElement("DateOfBirth")] // Especifica el nombre del campo en MongoDB
        public DateTime DateOfBirth { get; set; }

        // Propiedad de solo lectura que concatena el tipo de identificación con el número de identificación
        [BsonIgnore] // Indica que este campo no debe ser mapeado a la base de datos
        public string Identification
        {
            get { return $"{IdentificationType}-{IdentificationNumber}"; }
        }
    }
}
