using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Services
{
    public class SensorService : ISensorService
    {
        private readonly IMongoCollection<SensorModel> _sensores;
        private readonly MongoClient _Client;
        private readonly ConfiguracionMongo _configuracionMongo;

        public SensorService(MongoClient mongoClient, IOptions<ConfiguracionMongo> MongoConfig)
        {

            _Client = mongoClient;
            _configuracionMongo = MongoConfig.Value ?? throw new ArgumentNullException(nameof(MongoConfig));

            var MongoDB = mongoClient.GetDatabase(_configuracionMongo.DataBase);

            _sensores = MongoDB.GetCollection<SensorModel>("Sensor");

        }

        #region AgregarSensor
        public async Task<SensorModel> AgregarSensor(SensorModel sensorModel)
        {
            await _sensores.InsertOneAsync(sensorModel);
            return sensorModel;
        }
        #endregion



        #region BuscarSensor
        public async Task<SensorModel> BuscarSensor(string Tipo)
        {
            var Filtro = Builders<SensorModel>.Filter.Eq(t => t.Tipo, Tipo);
            return await _sensores.Find(Filtro).FirstAsync();
        }
        #endregion



        #region EliminarSensor
        public async Task EliminarSensor(string Tipo)
        {
            var Filtro = Builders<SensorModel>.Filter.Eq(t => t.Tipo, Tipo);
            await _sensores.DeleteOneAsync(Filtro);
        }
        #endregion



        #region MostrarSensores
        public async Task<List<SensorModel>> MostrarSensores() => await _sensores.Find(a => true).ToListAsync();

        #endregion
    }
}
