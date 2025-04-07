using InvernaderoInteligente.Data.DTOs;
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
        private readonly InvernaderoService _invernaderoService;

    public SensorService (MongoClient mongoClient, IOptions<ConfiguracionMongo> MongoConfig, InvernaderoService invernaderoService)
        {

            _Client = mongoClient;
            _configuracionMongo = MongoConfig.Value ?? throw new ArgumentNullException(nameof(MongoConfig));

            var MongoDB = mongoClient.GetDatabase(_configuracionMongo.DataBase);

            _sensores = MongoDB.GetCollection<SensorModel> ("Sensor");
            _invernaderoService = invernaderoService;

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
      public async Task EliminarSensor (string Tipo) {
      var filtro = Builders<SensorModel>.Filter.Eq (t => t.Tipo, Tipo);
      var sensor = await _sensores.Find (filtro).FirstOrDefaultAsync ();

      if (sensor == null)
        throw new Exception ("Sensor no encontrado.");

      // También lo quitamos de los invernaderos
      await _invernaderoService.EliminarSensorDeInvernaderos (sensor.Tipo);

      await _sensores.DeleteOneAsync (filtro);
    }
    #endregion



    #region MostrarSensores
    public async Task<List<SensorModel>> MostrarSensores() => await _sensores.Find(a => true).ToListAsync();

    #endregion


  
    #region ActualizarLecturaSensor
    public async Task<SensorModel> ActualizarLecturaSensor (string id, SensorLecturaDTO lectura) {
      var filtro = Builders<SensorModel>.Filter.Eq (s => s.SensorId, id);
      var actualizacion = Builders<SensorModel>.Update
          .Set (s => s.Valor, lectura.Valor)
          .Set (s => s.Estado, lectura.Estado)
          .Set (s => s.FechaLectura, lectura.FechaLectura);

      var resultado = await _sensores.UpdateOneAsync (filtro, actualizacion);

      if (resultado.MatchedCount == 0)
        throw new Exception ("No se encontró el sensor");

      // Devuelve el sensor actualizado si quieres (opcional)
      return await _sensores.Find (filtro).FirstOrDefaultAsync ();

      #endregion
    }

  }
}
