using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProyectoApi.Models
{
    public class HorarioDto
    {
        public String IdMedico { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
