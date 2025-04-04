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


    public UsuarioService (
        AuthUsuarioService authUsuarioService,MongoClient mongoClient,IOptions<ConfiguracionMongo> mongoConfig, IMemoryCache memorycache) 
    {
      _authUsuarioService = authUsuarioService ?? throw new ArgumentNullException (nameof (authUsuarioService));
      _cliente = mongoClient;
      _configuracionMongo = mongoConfig.Value ?? throw new ArgumentException (nameof (mongoConfig));
      _memorycache = memorycache;

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

    public async Task<UsuarioModel> ActualizarUsuario (UsuarioModel actualizarusuario) {

      var Filtro = Builders<UsuarioModel>.Filter.Eq (a => a.UsuarioId, actualizarusuario.UsuarioId);
      actualizarusuario.Contrasena = BCrypt.Net.BCrypt.HashPassword (actualizarusuario.Contrasena);

      var Resultado = await _usuarios.ReplaceOneAsync (Filtro, actualizarusuario);

      if (Resultado.MatchedCount == 0) {
        throw new Exception ("No existe el usuario");
      }

      return actualizarusuario;
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
      var Filtro = Builders<UsuarioModel>.Filter.Eq (a => a.Email, correo);
      await _usuarios.DeleteOneAsync (Filtro);

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
