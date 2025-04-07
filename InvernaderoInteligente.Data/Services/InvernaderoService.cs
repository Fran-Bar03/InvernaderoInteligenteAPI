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
        private readonly IMongoCollection<UsuarioModel> _usuariosCollection;

        public InvernaderoService(MongoClient mongoClient, IOptions<ConfiguracionMongo> MongoConfig)
        {
            _Client = mongoClient;
            _ConfiguracionMongo = MongoConfig.Value ?? throw new ArgumentException(nameof(MongoConfig));

            var MongoDB = _Client.GetDatabase(_ConfiguracionMongo.DataBase);
            _invernaderos = MongoDB.GetCollection<InvernaderoModel>("Invernadero");
            _usuariosCollection = MongoDB.GetCollection<UsuarioModel>("Usuario");
        }

        #region AgregarInvernadero
        public async Task<InvernaderoModel?> AgregarInvernadero(InvernaderoModel agregarinvernadero)
        {
            if (agregarinvernadero == null)
                throw new ArgumentNullException(nameof(agregarinvernadero), "El invernadero no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(agregarinvernadero.Nombre))
                throw new ArgumentException("El campo es obligatorio.");

            if (string.IsNullOrWhiteSpace(agregarinvernadero.NombrePlanta))
                throw new ArgumentException("El campo es obligatorio.");

            if (string.IsNullOrWhiteSpace(agregarinvernadero.TipoPlanta))
                throw new ArgumentException("El campo es obligatorio.");


            await _invernaderos.InsertOneAsync(agregarinvernadero);
            return agregarinvernadero;
        }
        #endregion

        #region Actualizar Invernadero
        public async Task<InvernaderoModel?> ActualizarInvernadero(InvernaderoModel actualizarinvernadero)
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

        #region ListarInvernaderos
        public async Task<List<InvernaderoModel>> ListarInvernaderos() => await _invernaderos.Find(a => true).ToListAsync();
        #endregion

        #region BuscarInvernadero 
        public async Task<InvernaderoModel> BuscarInvernadero(string Nombre)
        {
            var Filtro = Builders<InvernaderoModel>.Filter.Eq(i => i.Nombre, Nombre);
            return await _invernaderos.Find(Filtro).FirstOrDefaultAsync();
        }
        #endregion

        #region EliminarInvernadero 
        public async Task<InvernaderoModel> EliminarInvernadero(string nombre)
        {
            var filtro = Builders<InvernaderoModel>.Filter.Eq(i => i.Nombre, nombre);
            var invernaderoEliminado = await _invernaderos.FindOneAndDeleteAsync(filtro);
            return invernaderoEliminado;
        }
        #endregion


        public async Task<InvernaderoModel?> AgregarInvernaderoAsync(InvernaderoModel invernadero, List<string> nombresUsuariosEncargados)
        {
            // Validar que la lista de nombres de usuarios no esté vacía
            if (nombresUsuariosEncargados == null || nombresUsuariosEncargados.Count == 0)
            {
                throw new ArgumentException("La lista de nombres de usuarios no puede estar vacía.");
            }

            // Buscar los usuarios encargados por su nombre completo
            var usuariosEncargados = await _usuariosCollection
                .Find(u => nombresUsuariosEncargados.Contains(u.NombreCompleto))
                .ToListAsync();

            // Verificar si se encontraron todos los usuarios
            if (usuariosEncargados.Count < nombresUsuariosEncargados.Count)
            {
                var nombresNoEncontrados = nombresUsuariosEncargados.Except(usuariosEncargados.Select(u => u.NombreCompleto)).ToList();
                throw new Exception($"No se encontraron los siguientes usuarios: {string.Join(", ", nombresNoEncontrados)}");
            }

            // Asignar los nombres de los usuarios al invernadero
            invernadero.Usuarios = usuariosEncargados.Select(u => u.NombreCompleto).ToList();

            // Guardar el invernadero en la base de datos
            await _invernaderos.InsertOneAsync(invernadero);

            // Actualizar los usuarios con el nombre del invernadero asignado
            var updateTasks = new List<Task>();

            foreach (var usuario in usuariosEncargados)
            {
                if (!usuario.Invernaderos.Contains(invernadero.Nombre))
                {
                    usuario.Invernaderos.Add(invernadero.Nombre);
                }

                // Crear la actualización para este usuario
                var updateDefinition = Builders<UsuarioModel>.Update.Set(u => u.Invernaderos, usuario.Invernaderos);

                // Actualizar usuario con el nuevo invernadero asignado
                var updateTask = _usuariosCollection.UpdateOneAsync(
                    u => u.UsuarioId == usuario.UsuarioId,
                    updateDefinition
                );

                updateTasks.Add(updateTask);
            }

            // Esperar a que todas las actualizaciones se completen
            await Task.WhenAll(updateTasks);

            // Devolver el invernadero actualizado con los usuarios asignados
            return invernadero;
        }


        
        #region ObtenerUsuariosConInvernaderos
        public async Task<List<UsuarioModel>> ObtenerUsuariosConInvernaderos()
        {
            // Obtener todos los usuarios que tienen al menos un invernadero asignado
            var usuariosConInvernaderos = await _usuariosCollection
                .Find(u => u.Invernaderos.Any()) // Filtra los usuarios que tienen al menos un invernadero
                .ToListAsync();

            if (usuariosConInvernaderos.Count == 0)
            {
                throw new Exception("No hay usuarios con invernaderos asignados.");
            }

            return usuariosConInvernaderos;
        }
        #endregion


    }
}
