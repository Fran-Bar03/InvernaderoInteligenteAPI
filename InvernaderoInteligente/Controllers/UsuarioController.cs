﻿using InvernaderoInteligente.Data.Services;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvernaderoInteligente.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly UsuarioService _usuarioService;
        

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }



        // GET: api/<UsuarioController>
        //[Authorize]
        [HttpGet("MostrarUsuarios")]
        public async Task<IActionResult> MostrarUsuarios()
        {
            var usuario = await _usuarioService.MostrarUsuarios();
            return Ok(usuario);
        }

        // GET api/<UsuarioController>/5
        [HttpGet("Email/{correo}")]
        public async Task<IActionResult> BuscarUsuario(string correo)
        {
            var usuario = await _usuarioService.BuscarUsuario(correo);
            return Ok(usuario);
        }

        // POST api/<UsuarioController>
        [HttpPost("Registro")]
        public async Task<IActionResult> CrearUsuario([FromBody]UsuarioModel usuario)
        {
            if (usuario == null)
            {
                return BadRequest();
            }

            var Resultado = await _usuarioService.CrearUsuario(usuario);
            return Ok(Resultado);
        }

        // PUT api/<UsuarioController>/5
        [Authorize]
        [HttpPut("Actualizar")]
        public async Task<IActionResult> ActualizarUsuario(UsuarioModel usuariomodel)
        {
            await _usuarioService.ActualizarUsuario(usuariomodel);
            return Ok();
        }


        [HttpPut("CambiarContraseña/{correo}")]

        public async Task<IActionResult> CambiarContrasena(string correo,string contrasena)
        {
            await _usuarioService.CambiarContrasena(correo,contrasena);
            return Ok();
        }


        // DELETE api/<UsuarioController>/5
        [Authorize]
        [HttpDelete("{correo}")]
        public async Task<IActionResult> BorrarUsuario(string correo)
        {
            await _usuarioService.BorrarUsuario(correo);
            return Ok();
        }
    }
}
