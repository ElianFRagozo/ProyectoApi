namespace ProyectoApi.Models
{
    public class Medico:UserModel
    {
        public ICollection<Horario> HorariosDisponibilidad { get; set; }
    }
}
