using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvernaderoInteligente.Data;
using InvernaderoInteligente.Model;

namespace InvernaderoInteligente.Data.Interfaces
{
    public interface IUsuarioService
    {
        public Task<UsuarioModel> CrearUsuario(UsuarioModel crearusuario);

        public Task<UsuarioModel> ActualizarUsuario(UsuarioModel actualizarusuario);

        public Task<UsuarioModel> BuscarUsuario(string correo);

        public Task BorrarUsuario(string correo);

        public Task CambiarContrasena(string correo, string contrasena);

        public Task Login (string correo, string contrasena);


    }
}
