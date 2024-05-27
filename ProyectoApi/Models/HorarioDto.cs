using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProyectoApi.Models
{
    public class HorarioDto
    {
        public String IdMedico { get; set; }
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
        public string dia { get; set; }
    }
}
