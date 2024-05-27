using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoApi.Models;

namespace ProyectoApi.Services
{
    public class HorarioService
    {
        private readonly IMongoCollection<Horario> _horario;

        public HorarioService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _horario = database.GetCollection<Horario>("Horario");


        }

        public async Task<Horario> GetHorarioAsync(string idMedico, string dia, string hora)
        {
            return await _horario.Find(horario => horario.IdMedico == idMedico && horario.dia == dia && horario.horaInicio == hora).FirstOrDefaultAsync();
        }
        public async Task<List<Horario>> GetHorariosAsync()
        {
            return await _horario.Find(horario => true).ToListAsync();
        }


        public async Task CreateHorarioAsync(Horario horario)
        {
            var newId = ObjectId.GenerateNewId();
            horario.Id = newId.ToString();

            await _horario.InsertOneAsync(horario);
        }

        public async Task<Horario> GetHorarioIDAsync(string _horarioId)
        {
            var objectId = ObjectId.Parse(_horarioId);
            return await _horario.Find(horario => (horario.Id == objectId.ToString())).FirstOrDefaultAsync();
        }




    }

    namespace ProyectoApi.Services
    {
        public interface IMongoDatabaseSettings
        {
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
            string NotesCollectionName { get; set; }
        }

        public class MongoDatabaseSettings : IMongoDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
            public string NotesCollectionName { get; set; }
        }
    }
}