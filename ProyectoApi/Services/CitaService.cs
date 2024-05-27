using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoApi.Models;

namespace ProyectoApi.Services
{
    public class CitaService
    {
        private readonly IMongoCollection<Cita> _cita;
        
        public CitaService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _cita = database.GetCollection<Cita>("Cita");

        }
        public async Task CreateCitaAsync(Cita cita)
        {
            var newId = ObjectId.GenerateNewId();
            cita.CitaId = newId.ToString();

            await _cita.InsertOneAsync(cita);
        }
        public async Task<Cita> GetCitaEstadoAsync(string medicoId,string dia, string horaInicio)
        {
            return await _cita.Find(cita => (cita.HoraInicio==horaInicio && cita.dia == dia && cita.Estado == "registrado" && cita.MedicoId == medicoId.ToString())).FirstOrDefaultAsync();
        }
        public async Task<Cita> GetCitaIdAsync(string _citaId, string estado)
        {
            var objectId = ObjectId.Parse(_citaId);
            return await _cita.Find(cita => ( cita.CitaId == objectId.ToString())).FirstOrDefaultAsync();
        }

        public async Task<List<Cita>> GetCitasAsync()
        {
            return await _cita.Find(horario => true).ToListAsync();
        }


    }
   
}
    

