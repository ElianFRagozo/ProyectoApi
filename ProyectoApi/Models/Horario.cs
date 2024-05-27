using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ProyectoApi.Models
{
    public class Horario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "La identificacion del medico es obligatorio.")]
        public String IdMedico { get; set; }

        [Required(ErrorMessage = "Debe indicar el horario a asignar.")]
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
        public string dia { get; set; }
    }
}
