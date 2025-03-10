using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace InvernaderoInteligente.Data.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IMongoCollection<UsuarioModel> _usuarios;
        private ConfiguracionMongo _configuracionMongo;
        private MongoClient _cliente;
       

        public UsuarioService(MongoClient mongoclient, IOptions<ConfiguracionMongo> MongoConfig) 
        {
            _cliente = mongoclient;
            _configuracionMongo = MongoConfig.Value ?? throw new ArgumentException(nameof(MongoConfig));

            var mongoDB = _cliente.GetDatabase(_configuracionMongo.DataBase);
            _usuarios = mongoDB.GetCollection<UsuarioModel>("Usuario");
        }
        
        #region CrearUsuario
        public async Task<UsuarioModel> CrearUsuario(UsuarioModel crearusuario) 
        {
            
            if (crearusuario == null)
                throw new ArgumentNullException(nameof(crearusuario), "El usuario no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(crearusuario.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(crearusuario.Email))
                throw new ArgumentException("El correo electrónico es obligatorio.");
                
            if (string.IsNullOrEmpty(crearusuario.Contraseña))
                throw new ArgumentException("La contraseña es obligatoria.");


            crearusuario.Contraseña = BCrypt.Net.BCrypt.EnhancedHashPassword(crearusuario.Contraseña);
            await _usuarios.InsertOneAsync(crearusuario);
            return crearusuario;
        }
        #endregion


 
        #region ActualizarUsuario

        public async Task<UsuarioModel> ActualizarUsuario(UsuarioModel actualizarusuario)
        {

            var Filtro = Builders<UsuarioModel>.Filter.Eq(a => a.UsuarioId, actualizarusuario.UsuarioId);
            actualizarusuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(actualizarusuario.Contraseña);

            var Resultado = await _usuarios.ReplaceOneAsync(Filtro, actualizarusuario);

            if (Resultado.MatchedCount == 0)
            {
                throw new Exception("No existe el usuario");
            }
            
            return actualizarusuario;
        }

        #endregion

        

        #region MostrarUsuarios

        public async Task<List<UsuarioModel>> FindAll() => await _usuarios.Find(_ => true).ToListAsync();

        #endregion



        #region BuscarUsuario

        public async Task<UsuarioModel> BuscarUsuario(string correo)
        {
            var Filtro = Builders<UsuarioModel>.Filter.Eq(c => c.Email, correo);
              return await _usuarios.Find(Filtro).FirstAsync();
        }

        #endregion



        #region BorrarUsuario
        
        public async Task BorrarUsuario(string correo)
        {
            var Filtro = Builders<UsuarioModel>.Filter.Eq(a => a.Email, correo);
            await _usuarios.DeleteOneAsync(Filtro);

        }
        
        #endregion

        

        #region CambiarContraseña

        public async Task CambiarContrasena(string correo, string contrasena)
        {
            string PassHashed = BCrypt.Net.BCrypt.EnhancedHashPassword(contrasena);
            var Filtro = Builders<UsuarioModel>.Filter.Eq(c => c.Email, correo);
            var Actualizarr = Builders<UsuarioModel>.Update.Set(p => p.Contraseña, contrasena);

            await _usuarios.UpdateOneAsync(Filtro, Actualizarr);
        }

       


        #endregion


    }
}
