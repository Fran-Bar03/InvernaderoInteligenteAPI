using BCrypt.Net;
using InvernaderoInteligente.Data.DTOs;
using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace InvernaderoInteligente.Data.Services {
  public class UsuarioService : IUsuarioService {
    private readonly IMongoCollection<UsuarioModel> _usuarios;
    private ConfiguracionMongo _configuracionMongo;
    private MongoClient _cliente;
    private readonly AuthUsuarioService _authUsuarioService;
    private readonly IMemoryCache _memorycache;
    private readonly InvernaderoService _invernaderoService;


    public UsuarioService (
        AuthUsuarioService authUsuarioService,MongoClient mongoClient,IOptions<ConfiguracionMongo> mongoConfig, IMemoryCache memorycache, InvernaderoService invernaderoService) 
    {
      _authUsuarioService = authUsuarioService ?? throw new ArgumentNullException (nameof (authUsuarioService));
      _cliente = mongoClient;
      _configuracionMongo = mongoConfig.Value ?? throw new ArgumentException (nameof (mongoConfig));
      _memorycache = memorycache;
      _invernaderoService = invernaderoService;


      var mongoDB = _cliente.GetDatabase (_configuracionMongo.DataBase);
      _usuarios = mongoDB.GetCollection<UsuarioModel> ("Usuario");
    }

    #region CrearUsuario
    public async Task<UsuarioModel> CrearUsuario (CrearUsuarioDTO crearusuariodto) {


     if (crearusuariodto == null) 
      {
        throw new ArgumentException (nameof (crearusuariodto));
      }

      var Usuario = new UsuarioModel {
        NombreCompleto = crearusuariodto.NombreCompleto,
        Email = crearusuariodto.Email,
        Contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(crearusuariodto.Contrasena)
      };

      await _usuarios.InsertOneAsync(Usuario);

      return Usuario;


    }


    #endregion



    #region ActualizarUsuario
    public async Task<UsuarioModel> ActualizarUsuario(UsuarioModel actualizarusuario)
{
    var filtro = Builders<UsuarioModel>.Filter.Eq(u => u.UsuarioId, actualizarusuario.UsuarioId);
    var usuarioExistente = await _usuarios.Find(filtro).FirstOrDefaultAsync();

    if (usuarioExistente == null)
        throw new Exception("No existe el usuario");

    // Solo actualiza los campos que se enviaron (los que no sean null o vacíos)
    var updateDef = new List<UpdateDefinition<UsuarioModel>>();

    if (!string.IsNullOrEmpty(actualizarusuario.NombreCompleto))
        updateDef.Add(Builders<UsuarioModel>.Update.Set(u => u.NombreCompleto, actualizarusuario.NombreCompleto));

    if (!string.IsNullOrEmpty(actualizarusuario.Email))
        updateDef.Add(Builders<UsuarioModel>.Update.Set(u => u.Email, actualizarusuario.Email));

    if (!string.IsNullOrEmpty(actualizarusuario.Contrasena))
    {
        string hashed = BCrypt.Net.BCrypt.HashPassword(actualizarusuario.Contrasena);
        updateDef.Add(Builders<UsuarioModel>.Update.Set(u => u.Contrasena, hashed));
    }

    if (actualizarusuario.Rol != 0) // Asumiendo que 0 no es un rol válido
        updateDef.Add(Builders<UsuarioModel>.Update.Set(u => u.Rol, actualizarusuario.Rol));

    if (actualizarusuario.Invernaderos != null && actualizarusuario.Invernaderos.Any())
        updateDef.Add(Builders<UsuarioModel>.Update.Set(u => u.Invernaderos, actualizarusuario.Invernaderos));

    if (updateDef.Count == 0)
        throw new Exception("No se enviaron campos para actualizar");

    var update = Builders<UsuarioModel>.Update.Combine(updateDef);
    await _usuarios.UpdateOneAsync(filtro, update);

    // Devuelve el usuario actualizado
    return await _usuarios.Find(filtro).FirstOrDefaultAsync();
    }
    #endregion



    #region MostrarUsuarios

    public async Task<List<UsuarioModel>> FindAll () => await _usuarios.Find (_ => true).ToListAsync ();

    #endregion



    #region BuscarUsuario

    public async Task<UsuarioModel> BuscarUsuario (string correo) {
      var Filtro = Builders<UsuarioModel>.Filter.Eq (c => c.Email, correo);
      return await _usuarios.Find (Filtro).FirstAsync ();
    }

    #endregion



    #region BorrarUsuario

    public async Task BorrarUsuario (string correo) {
      var filtroUsuario = Builders<UsuarioModel>.Filter.Eq (u => u.Email, correo);
      var usuario = await _usuarios.Find (filtroUsuario).FirstOrDefaultAsync ();

      if (usuario == null)
        throw new Exception ("Usuario no encontrado.");

      // 🔁 Llamamos al método público del servicio de invernaderos
      await _invernaderoService.EliminarUsuarioDeInvernaderos (usuario.NombreCompleto);

      await _usuarios.DeleteOneAsync (filtroUsuario);
    }



    #endregion



    #region CambiarContraseña

    public async Task CambiarContrasena (string correo, string contrasena) {

      var filtro = Builders<UsuarioModel>.Filter.Eq (c => c.Email, correo);
      string passHashed = BCrypt.Net.BCrypt.EnhancedHashPassword (contrasena);
      var actualizar = Builders<UsuarioModel>.Update.Set (p => p.Contrasena, passHashed);

      var resultado = await _usuarios.UpdateOneAsync (filtro, actualizar);

      if (resultado.MatchedCount == 0) {
        throw new Exception ("Usuario no encontrado.");
      }
    }

    #endregion


    #region Login

    public async Task<string> Login (string email, string contrasena) {
      var Filtro = Builders<UsuarioModel>.Filter.Eq (e => e.Email, email);
      var Usuario = await _usuarios.Find (Filtro).FirstOrDefaultAsync ();

      if (Usuario == null) {
        throw new Exception ("Usuario no encontrado");
      }

      if (!BCrypt.Net.BCrypt.EnhancedVerify(contrasena, Usuario.Contrasena)) {
        throw new Exception ("Contrasena incorrecta");
      }

      var Token = _authUsuarioService.GenerarToken (Usuario);
      return Token;


    }


        #endregion



    }
}
