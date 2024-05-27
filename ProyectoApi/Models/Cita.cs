using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProyectoApi.Models
{
    public class Cita
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CitaId { get; set; }
        public string PacienteId { get; set; }
        public string MedicoId { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string dia { get; set; }
        public string Estado { get; set; } // Por ejemplo: "Pendiente", "Confirmada", "Cancelada"

       
    }
}
