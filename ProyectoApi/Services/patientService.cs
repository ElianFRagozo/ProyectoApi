using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ProyectoApi.Services
{
    public class PatientService
    {
        private readonly IMongoCollection<Patient> _patients;

        public PatientService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _patients = database.GetCollection<Patient>("Patient");
        }


        public async Task<List<Patient>> GetPatientsAsync()
        {
            return await _patients.Find(patient => true).ToListAsync();
        }

        public async Task CreatePatientAsync(Patient patient)
        {
            await _patients.InsertOneAsync(patient);
        }


        public async Task UpdatePatientAsync(string id, Patient patient)
        {
            var objectId = ObjectId.Parse(id);
            await _patients.ReplaceOneAsync(p => p.Id == objectId.ToString(), patient);
        }

        public async Task<Patient> GetPatientAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _patients.Find(p => p.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        public async Task DeletePatientAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        await _patients.DeleteOneAsync(p => p.Id == objectId.ToString());
    }

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

