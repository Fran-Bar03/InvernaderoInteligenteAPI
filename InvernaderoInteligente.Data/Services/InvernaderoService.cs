using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace InvernaderoInteligente.Data.Services
{
    public class InvernaderoService : IInvernaderoService
    {
        private readonly IMongoCollection<InvernaderoModel> _invernaderos;
        private readonly MongoClient _Client;
        private readonly ConfiguracionMongo _ConfiguracionMongo;

    public InvernaderoService(MongoClient mongoClient, IOptions<ConfiguracionMongo> MongoConfig)
        {
            _Client = mongoClient;
            _ConfiguracionMongo = MongoConfig.Value ?? throw new ArgumentException (nameof (MongoConfig));

            var MongoDB = _Client.GetDatabase (_ConfiguracionMongo.DataBase);
            _invernaderos = MongoDB.GetCollection<InvernaderoModel> ("Invernadero");
            
        }

        #region AgregarInvernadero
        public async Task<InvernaderoModel> AgregarInvernadero(InvernaderoModel agregarinvernadero)
        {
            if (agregarinvernadero == null)
                throw new ArgumentNullException(nameof(agregarinvernadero), "El invernadero no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(agregarinvernadero.Nombre))
                throw new ArgumentException("El campo es obligatorio.");

            if (string.IsNullOrWhiteSpace(agregarinvernadero.NombrePlanta))
                throw new ArgumentException("El campo es obligatorio.");

            if (string.IsNullOrWhiteSpace(agregarinvernadero.TipoPlanta))
                throw new ArgumentException("El campo es obligatorio.");

            if (!agregarinvernadero.MinTemperatura.HasValue)
                throw new ArgumentException("Introduzca un valor válido.");

            if (agregarinvernadero.MaxTemperatura.HasValue)
                throw new ArgumentException("Introduzca un valor válido.");

            if (agregarinvernadero.MinHumedad.HasValue)
                throw new ArgumentException("Introduzca un valor válido.");

            if (agregarinvernadero.MaxHumedad.HasValue)
                throw new ArgumentException("Introduzca un valor válido.");


            await _invernaderos.InsertOneAsync(agregarinvernadero);
            return agregarinvernadero;
        }
        #endregion

        #region Actualizar Invernadero
        public async Task<InvernaderoModel> ActualizarInvernadero(InvernaderoModel actualizarinvernadero)
        {
            var filtro = Builders<InvernaderoModel>.Filter.Eq(i => i.InvernaderoId, actualizarinvernadero.InvernaderoId);
            var result = await _invernaderos.ReplaceOneAsync(filtro, actualizarinvernadero);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Invernadero no encontrado");
            }
            return actualizarinvernadero;
        }
        #endregion

        #region Listar Invernaderos
        public async Task<List<InvernaderoModel>> ListarInvernaderos() => await _invernaderos.Find(a => true).ToListAsync();
        #endregion

        #region Buscar Invernadero por Nombre
        public async Task<InvernaderoModel> BuscarPorNombre(string nombre)
        {
            var filtro = Builders<InvernaderoModel>.Filter.Eq(i => i.Nombre, nombre);
            return await _invernaderos.Find(filtro).FirstAsync();
        }
        #endregion

        #region Eliminar Invernadero por Nombre
        public async Task<InvernaderoModel> EliminarPorNombre(string nombre)
        {
            var filtro = Builders<InvernaderoModel>.Filter.Eq(i => i.Nombre, nombre);
            var invernaderoEliminado = await _invernaderos.FindOneAndDeleteAsync(filtro);
            return invernaderoEliminado;
        }
        #endregion

    }
}
