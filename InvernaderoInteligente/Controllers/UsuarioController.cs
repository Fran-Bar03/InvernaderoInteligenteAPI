using InvernaderoInteligente.Data.DTOs;
using InvernaderoInteligente.Data.Services;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvernaderoInteligente.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class UsuarioController : ControllerBase {

    private readonly UsuarioService _usuarioService;
    private readonly RecuperarContrasenaService _recuperarcontrasenaservice;


    public UsuarioController (UsuarioService usuarioService, RecuperarContrasenaService recuperarcontrasenaservice) {
      _usuarioService = usuarioService;
      _recuperarcontrasenaservice = recuperarcontrasenaservice;
    }



    // GET: api/<UsuarioController>
    [HttpGet ("MostrarUsuarios")]
    public async Task<IActionResult> FindAll () {
      var usuario = await _usuarioService.FindAll ();
      return Ok (usuario);
    }

    // GET api/<UsuarioController>/5
    [HttpGet ("BuscarUsuario/{correo}")]
    public async Task<IActionResult> BuscarUsuario (string correo) {
      var usuario = await _usuarioService.BuscarUsuario (correo);
      return Ok (usuario);
    }

    // POST api/<UsuarioController>
    [HttpPost ("RegistrarUsuario")]
    public async Task<IActionResult> CrearUsuario ([FromBody] CrearUsuarioDTO crearusuariodto) {
      if (crearusuariodto == null) {
        return BadRequest ();
      }

      var Resultado = await _usuarioService.CrearUsuario (crearusuariodto);
      return Ok (Resultado);
    }

    // PUT api/<UsuarioController>/5
    //[Authorize]
    [HttpPut ("ActualizarUsuario")]
    public async Task<IActionResult> ActualizarUsuario (UsuarioModel usuariomodel) {
      await _usuarioService.ActualizarUsuario (usuariomodel);
      return Ok (new { Mensaje = "Datos actualizados correctamente" });
    }


    [HttpPut ("CambiarContrasena/{correo}")]

    public async Task<IActionResult> CambiarContrasena (string correo, string contrasena) {
      await _usuarioService.CambiarContrasena (correo, contrasena);
      return Ok (new { Mensaje = "Contraseña actualizada correctamente" });
    }


    // DELETE api/<UsuarioController>/5
    //[Authorize]
    [HttpDelete ("BorrarUsuario/{correo}")]
    public async Task<IActionResult> BorrarUsuario (string correo) {
      await _usuarioService.BorrarUsuario (correo);
      return Ok (new { Mensaje = "Usuario eliminado correctamente" });
    }

    [HttpPost ("Login")]
    public async Task<IActionResult> Login ([FromBody] LoginDTO login) {
      if (!ModelState.IsValid) {
        return BadRequest (ModelState);
      }

      try {
        var Token = await _usuarioService.Login (login.Email, login.Contrasena);
        return Ok (Token);
      } catch (Exception ex) {
        return Unauthorized (new { mensaje = ex.Message });
      }
    }

    [HttpPost ("CambiarContrasena-Email")]
    public async Task<IActionResult> RecuperarContrasenaEmail ([FromBody] RecuperarContrasenaDTO dto) {
      try {
        
        // Cambiamos la contraseña del usuario utilizando el correo extraído del token
        await _usuarioService.CambiarContrasena (dto.Email, dto.Contrasena);
        return Ok ("Contraseña cambiada correctamente.");
      } catch (Exception ex) {
        return BadRequest ($"Error: {ex.Message}");
      }
    }
        

    [HttpPost ("ValidarCodigo")]
    public IActionResult ValidarCodigo ([FromBody] ValidarCodigoDTO dto) {
      try {
        string token = _recuperarcontrasenaservice.ValidarCodigo (dto);
        return Ok (token);
      } catch (Exception ex) {
        return BadRequest ($"Error: {ex.Message}");
      }
    }

    [HttpPost ("EnviarCodigo")]
    public async Task<IActionResult> EnviarCodigoRecuperacion ([FromBody] RecuperarContrasenaEmailDTO dto) {
      try {
        var Token = await _recuperarcontrasenaservice.EnviarCodigoRecuperacion (dto);

        return Ok (Token);
      } catch (Exception ex) {
        return BadRequest ($"Error: {ex.Message}");
      }
    }

        


    }
}
