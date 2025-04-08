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

namespace InvernaderoInteligente.Data.Services {
  public class InvernaderoService : IInvernaderoService {
    private readonly IMongoCollection<InvernaderoModel> _invernaderos;
    private readonly MongoClient _Client;
    private readonly ConfiguracionMongo _ConfiguracionMongo;
    private readonly IMongoCollection<UsuarioModel> _usuariosCollection;
    private readonly IMongoCollection<SensorModel> _sensorCollection;

    public InvernaderoService (MongoClient mongoClient, IOptions<ConfiguracionMongo> MongoConfig) {
      _Client = mongoClient;
      _ConfiguracionMongo = MongoConfig.Value ?? throw new ArgumentException (nameof (MongoConfig));

      var MongoDB = _Client.GetDatabase (_ConfiguracionMongo.DataBase);
      _invernaderos = MongoDB.GetCollection<InvernaderoModel> ("Invernadero");
      _usuariosCollection = MongoDB.GetCollection<UsuarioModel> ("Usuario");
      _sensorCollection = MongoDB.GetCollection<SensorModel> ("Sensor");
    }

    #region AgregarInvernadero
    public async Task<InvernaderoModel?> AgregarInvernadero (InvernaderoModel agregarinvernadero) {
      if (agregarinvernadero == null)
        throw new ArgumentNullException (nameof (agregarinvernadero), "El invernadero no puede ser nulo.");

      if (string.IsNullOrWhiteSpace (agregarinvernadero.Nombre))
        throw new ArgumentException ("El campo es obligatorio.");

      if (string.IsNullOrWhiteSpace (agregarinvernadero.NombrePlanta))
        throw new ArgumentException ("El campo es obligatorio.");

      if (string.IsNullOrWhiteSpace (agregarinvernadero.TipoPlanta))
        throw new ArgumentException ("El campo es obligatorio.");


      await _invernaderos.InsertOneAsync (agregarinvernadero);
      return agregarinvernadero;
    }
    #endregion

    #region ActualizarInvernadero
    public async Task<InvernaderoModel?> ActualizarInvernadero (
        InvernaderoModel actualizarinvernadero,
        List<string> nombresUsuariosEncargados,
        List<string> tiposSensores) {
      // Buscar invernadero existente
      var invernaderoExistente = await _invernaderos
          .Find (i => i.InvernaderoId == actualizarinvernadero.InvernaderoId)
          .FirstOrDefaultAsync ();

      if (invernaderoExistente == null)
        throw new Exception ("Invernadero no encontrado.");

      // Validación de nulls
      if (nombresUsuariosEncargados == null)
        throw new ArgumentException ("La lista de usuarios encargados no puede ser nula.");

      if (tiposSensores == null)
        throw new ArgumentException ("La lista de tipos de sensores no puede ser nula.");

      // Buscar usuarios encargados
      var usuariosEncargados = await _usuariosCollection
          .Find (u => nombresUsuariosEncargados.Contains (u.NombreCompleto))
          .ToListAsync ();

      if (usuariosEncargados.Count < nombresUsuariosEncargados.Count) {
        var noEncontrados = nombresUsuariosEncargados
            .Except (usuariosEncargados.Select (u => u.NombreCompleto))
            .ToList ();
        throw new Exception ($"Usuarios no encontrados: {string.Join (", ", noEncontrados)}");
      }

      // Agregar nuevos usuarios encargados
      foreach (var usuario in usuariosEncargados) {
        if (!usuario.Invernaderos.Contains (actualizarinvernadero.Nombre)) {
          var updateUsuario = Builders<UsuarioModel>.Update.AddToSet (u => u.Invernaderos, actualizarinvernadero.Nombre);
          await _usuariosCollection.UpdateOneAsync (u => u.UsuarioId == usuario.UsuarioId, updateUsuario);
        }
      }

      // Eliminar usuarios que ya no están o limpiar todos si lista vacía
      if (!nombresUsuariosEncargados.Any ()) {
        await _usuariosCollection.UpdateManyAsync (
            Builders<UsuarioModel>.Filter.AnyEq (u => u.Invernaderos, invernaderoExistente.Nombre),
            Builders<UsuarioModel>.Update.Pull (u => u.Invernaderos, invernaderoExistente.Nombre)
        );
      } else {
        var usuariosParaEliminar = invernaderoExistente.Usuarios
            .Except (nombresUsuariosEncargados)
            .ToList ();

        foreach (var nombreUsuario in usuariosParaEliminar) {
          var updateUsuario = Builders<UsuarioModel>.Update.Pull (u => u.Invernaderos, invernaderoExistente.Nombre);
          await _usuariosCollection.UpdateManyAsync (
              Builders<UsuarioModel>.Filter.Eq (u => u.NombreCompleto, nombreUsuario),
              updateUsuario
          );
        }
      }

      // Buscar sensores por tipo (solo si hay tipos a agregar)
      var sensores = new List<SensorModel> ();
      if (tiposSensores.Any ()) {
        sensores = await _sensorCollection
            .Find (s => tiposSensores.Contains (s.Tipo))
            .ToListAsync ();

        if (sensores.Count < tiposSensores.Count) {
          var noEncontrados = tiposSensores
              .Except (sensores.Select (s => s.Tipo))
              .ToList ();
          throw new Exception ($"Sensores no encontrados: {string.Join (", ", noEncontrados)}");
        }

        // Agregar sensores al invernadero si es necesario
        foreach (var sensor in sensores) {
          if (sensor.Invernadero != actualizarinvernadero.Nombre) {
            var updateSensor = Builders<SensorModel>.Update.Set (s => s.Invernadero, actualizarinvernadero.Nombre);
            await _sensorCollection.UpdateOneAsync (s => s.SensorId == sensor.SensorId, updateSensor);
          }
        }
      }

      // Eliminar todos los sensores si lista vacía
      if (!tiposSensores.Any ()) {
        var updateSensores = Builders<SensorModel>.Update.Set (s => s.Invernadero, null);
        await _sensorCollection.UpdateManyAsync (s => s.Invernadero == invernaderoExistente.Nombre, updateSensores);
      } else {
        // Eliminar sensores que ya no están
        var sensoresParaEliminar = invernaderoExistente.Sensores
            .Except (tiposSensores)
            .ToList ();

        foreach (var tipoSensor in sensoresParaEliminar) {
          var updateSensor = Builders<SensorModel>.Update.Set (s => s.Invernadero, null);
          await _sensorCollection.UpdateManyAsync (s => s.Tipo == tipoSensor && s.Invernadero == invernaderoExistente.Nombre, updateSensor);
        }
      }

      // Actualizar el invernadero
      var updateInvernadero = Builders<InvernaderoModel>.Update
          .Set (i => i.Nombre, actualizarinvernadero.Nombre)
          .Set (i => i.NombrePlanta, actualizarinvernadero.NombrePlanta)
          .Set (i => i.TipoPlanta, actualizarinvernadero.TipoPlanta)
          .Set (i => i.Imagen, actualizarinvernadero.Imagen)
          .Set (i => i.MinTemperatura, actualizarinvernadero.MinTemperatura)
          .Set (i => i.MaxTemperatura, actualizarinvernadero.MaxTemperatura)
          .Set (i => i.MinHumedad, actualizarinvernadero.MinHumedad)
          .Set (i => i.MaxHumedad, actualizarinvernadero.MaxHumedad)
          .Set (i => i.Usuarios, usuariosEncargados.Select (u => u.NombreCompleto).ToList ())
          .Set (i => i.Sensores, tiposSensores);

      var result = await _invernaderos.UpdateOneAsync (
          i => i.InvernaderoId == actualizarinvernadero.InvernaderoId,
          updateInvernadero
      );

      if (result.MatchedCount == 0)
        throw new Exception ("Invernadero no encontrado.");

      return await _invernaderos
          .Find (i => i.InvernaderoId == actualizarinvernadero.InvernaderoId)
          .FirstOrDefaultAsync ();
    }
    #endregion



    #region ListarInvernaderos
    public async Task<List<InvernaderoModel>> ListarInvernaderos () {
      return await _invernaderos.Find (a => true).ToListAsync ();
    }
    #endregion

    #region BuscarInvernadero 
    public async Task<InvernaderoModel> BuscarInvernadero (string Nombre) {
      var Filtro = Builders<InvernaderoModel>.Filter.Eq (i => i.Nombre, Nombre);
      return await _invernaderos.Find (Filtro).FirstOrDefaultAsync ();
    }
    #endregion

    #region EliminarInvernadero 
    public async Task<InvernaderoModel> EliminarInvernadero (string nombre) {
      var filtro = Builders<InvernaderoModel>.Filter.Eq (i => i.Nombre, nombre);
      var invernaderoEliminado = await _invernaderos.FindOneAndDeleteAsync (filtro);
      return invernaderoEliminado;
    }
        #endregion

    #region CrearInvernadero
        public async Task<InvernaderoModel?> AgregarInvernaderoAsync(InvernaderoModel invernadero, List<string> nombresUsuariosEncargados, List<string> tiposSensores)
        {
            try
            {
                // Buscar los usuarios encargados por su nombre completo solo si se proporcionan usuarios
                List<string> nombresFiltrados = new();
                if (nombresUsuariosEncargados != null && nombresUsuariosEncargados.Count > 0)
                {
                    var usuariosEncargados = await _usuariosCollection
                        .Find(u => nombresUsuariosEncargados.Contains(u.NombreCompleto))
                        .ToListAsync();

                    if (usuariosEncargados.Count < nombresUsuariosEncargados.Count)
                    {
                        var nombresNoEncontrados = nombresUsuariosEncargados.Except(usuariosEncargados.Select(u => u.NombreCompleto)).ToList();
                        throw new Exception($"No se encontraron los siguientes usuarios: {string.Join(", ", nombresNoEncontrados)}");
                    }

                    nombresFiltrados = usuariosEncargados.Select(u => u.NombreCompleto).ToList();

                    foreach (var usuario in usuariosEncargados)
                    {
                        if (!usuario.Invernaderos.Contains(invernadero.Nombre))
                        {
                            usuario.Invernaderos.Add(invernadero.Nombre);
                        }

                        var updateDefinition = Builders<UsuarioModel>.Update.AddToSet(u => u.Invernaderos, invernadero.Nombre);
                        await _usuariosCollection.UpdateOneAsync(
                            u => u.UsuarioId == usuario.UsuarioId,
                            updateDefinition
                        );
                    }
                }

                // Buscar sensores por tipo
                List<string> sensoresFiltrados = new();
                if (tiposSensores != null && tiposSensores.Count > 0)
                {
                    var sensoresEncargados = await _sensorCollection
                        .Find(s => tiposSensores.Contains(s.Tipo))
                        .ToListAsync();

                    if (sensoresEncargados.Count < tiposSensores.Count)
                    {
                        var tiposNoEncontrados = tiposSensores.Except(sensoresEncargados.Select(s => s.Tipo)).ToList();
                        throw new Exception($"No se encontraron los siguientes tipos de sensores: {string.Join(", ", tiposNoEncontrados)}");
                    }

                    sensoresFiltrados = sensoresEncargados.Select(s => s.Tipo).ToList();

                    foreach (var sensor in sensoresEncargados)
                    {
                        if (sensor.Invernadero != invernadero.Nombre)
                        {
                            sensor.Invernadero = invernadero.Nombre;
                        }

                        var updateDefinition = Builders<SensorModel>.Update.Set(s => s.Invernadero, sensor.Invernadero);
                        await _sensorCollection.UpdateOneAsync(
                            s => s.SensorId == sensor.SensorId,
                            updateDefinition
                        );
                    }
                }

                // Crear un nuevo objeto limpio solo con los nombres completos y tipos
                var nuevoInvernadero = new InvernaderoModel
                {
                    Nombre = invernadero.Nombre,
                    NombrePlanta = invernadero.NombrePlanta,
                    TipoPlanta = invernadero.TipoPlanta,
                    Imagen = invernadero.Imagen,
                    MinTemperatura = invernadero.MinTemperatura,
                    MaxTemperatura = invernadero.MaxTemperatura,
                    MinHumedad = invernadero.MinHumedad,
                    MaxHumedad = invernadero.MaxHumedad,
                    Usuarios = nombresFiltrados,
                    Sensores = sensoresFiltrados
                };

                // Insertar solo el modelo limpio
                await _invernaderos.InsertOneAsync(nuevoInvernadero);

                return nuevoInvernadero;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar el invernadero: {ex.Message}");
            }
        }






        #endregion


    #region ObtenerUsuariosConInvernaderos
        public async Task<List<UsuarioModel>> ObtenerUsuariosConInvernaderos () {
      // Obtener todos los usuarios que tienen al menos un invernadero asignado
      var usuariosConInvernaderos = await _usuariosCollection
          .Find (u => u.Invernaderos.Any ()) // Filtra los usuarios que tienen al menos un invernadero
          .ToListAsync ();

      if (usuariosConInvernaderos.Count == 0) {
        throw new Exception ("No hay usuarios con invernaderos asignados.");
      }

      return usuariosConInvernaderos;
    }
        #endregion


        #region EliminarInvernadero
        public async Task<bool> EliminarInvernaderoAsync(string nombreInvernadero)
        {
            // Buscar el invernadero antes de intentar eliminarlo
            var invernaderoExistente = await _invernaderos.Find(i => i.Nombre == nombreInvernadero).FirstOrDefaultAsync();

            // Si no se encuentra el invernadero, retornamos false
            if (invernaderoExistente == null)
            {
                return false; // Invernadero no encontrado
            }

            // Eliminar el invernadero de la colección de invernaderos
            var resultInvernadero = await _invernaderos.DeleteOneAsync(i => i.Nombre == nombreInvernadero);

            // Eliminar la referencia del invernadero de la colección de usuarios
            var resultUsuarios = await _usuariosCollection.UpdateManyAsync(
                u => u.Invernaderos.Contains(nombreInvernadero),
                Builders<UsuarioModel>.Update.Pull(u => u.Invernaderos, nombreInvernadero)
            );

            // Eliminar la referencia del invernadero de la colección de sensores
            var resultSensores = await _sensorCollection.UpdateManyAsync(
                s => s.Invernadero == nombreInvernadero,
                Builders<SensorModel>.Update.Set(s => s.Invernadero, null)
            );

            // Verificamos que la eliminación y actualización de referencias se haya realizado correctamente
            return resultInvernadero.DeletedCount > 0 && resultUsuarios.ModifiedCount > 0 && resultSensores.ModifiedCount > 0;
        }




        #endregion



        #region EliminarUsuarioModeloInvernadero
        public async Task EliminarUsuarioDeInvernaderos (string nombreUsuario) {
      var filtro = Builders<InvernaderoModel>.Filter.ElemMatch (i => i.Usuarios, u => u == nombreUsuario);
      var update = Builders<InvernaderoModel>.Update.Pull (i => i.Usuarios, nombreUsuario);

      await _invernaderos.UpdateManyAsync (filtro, update);
    }
    #endregion

    #region EliminarSensorModeloInvernadero
    public async Task EliminarSensorDeInvernaderos (string tipoSensor) {
      var filtro = Builders<InvernaderoModel>.Filter.AnyEq (i => i.Sensores, tipoSensor);
      var update = Builders<InvernaderoModel>.Update.Pull (i => i.Sensores, tipoSensor);

      await _invernaderos.UpdateManyAsync (filtro, update);
    }
    #endregion


  }
}
