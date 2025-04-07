using InvernaderoInteligente.Data;
using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Data.Services;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvernaderoInteligente.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class InvernaderoController : ControllerBase {


    private readonly IInvernaderoService _invernaderoService;
    private readonly InvernaderoService _iinvernaderoService;
    private readonly IMongoCollection<UsuarioModel> _usuariosCollection;

    public InvernaderoController (IInvernaderoService invernaderoService, InvernaderoService invernaderoService1, IMongoClient mongoClient, IOptions<ConfiguracionMongo> config) {
      _invernaderoService = invernaderoService;
      _iinvernaderoService = invernaderoService1;
      var db = mongoClient.GetDatabase (config.Value.DataBase);
      _usuariosCollection = db.GetCollection<UsuarioModel> ("Usuario");
    }



    // GET: api/<InvernaderoController>
    [HttpGet ("ListarInvernaderos")]
    public async Task<IActionResult> ListarInvernaderosConUsuarios () {
      var invernaderos = await _invernaderoService.ListarInvernaderos ();

      // Obtener todos los usuarios que están asociados a estos invernaderos
      var invernaderoNombres = invernaderos.Select (i => i.Nombre).ToList ();
      var usuariosAsignados = await _usuariosCollection
          .Find (u => u.Invernaderos.Any (i => invernaderoNombres.Contains (i)))
          .ToListAsync ();

      var resultado = invernaderos.Select (inv => new {
        Invernadero = inv,
        Usuarios = usuariosAsignados.Where (u => u.Invernaderos.Contains (inv.Nombre)).ToList ()
      }).ToList ();

      return Ok (resultado);
    }




    // GET api/<InvernaderoController>/5
    [HttpGet ("BuscarInvernadero/{Nombre}")]
    public async Task<IActionResult> BuscarInvernadero (string Nombre) {
      var Invernadero = await _invernaderoService.BuscarInvernadero (Nombre);
      return Ok (Invernadero);
    }


    // PUT api/<InvernaderoController>/5
    [HttpPut ("ActualizarInvernadero")]
    public async Task<IActionResult> ActualizarInvernadero (InvernaderoModel invernaderomodel) {
      try {
        var resultado = await _iinvernaderoService.ActualizarInvernadero (invernaderomodel, invernaderomodel.Usuarios, invernaderomodel.Sensores);

        // Devuelves el invernadero actualizado o una confirmación de éxito
        return Ok (resultado); // O un mensaje simple si prefieres: return Ok("Invernadero actualizado correctamente.");
      } catch (Exception ex) {
        // Manejo de excepciones, puedes devolver un 500 si algo sale mal
        return StatusCode (500, $"Error interno: {ex.Message}");
      }
    }




    // POST: api/Invernadero/AgregarInvernadero
    [HttpPost ("AgregarInvernadero")]
    public async Task<IActionResult> AgregarInvernadero ([FromBody] InvernaderoModel invernadero) {
      try {
        // Aquí pasamos la lista de usuarios
        var usuarios = invernadero.Usuarios;

        // Aquí pasamos la lista de tipos de sensores
        var tiposSensores = invernadero.Sensores; // asumiendo que InvernaderoModel tiene una propiedad 'Sensores' que es una lista de tipos de sensores

        // Llamamos al servicio para agregar el invernadero, enlazar usuarios y sensores
        var result = await _iinvernaderoService.AgregarInvernaderoAsync (invernadero, usuarios, tiposSensores);

        return Ok (result);
      } catch (Exception ex) {
        // Si ocurre algún error, retornar un mensaje de error
        return BadRequest ($"Error: {ex.Message}");
      }
    }







    // GET: api/Invernadero/Usuarios
    [HttpGet ("UsuariosConInvernaderos")]
    public async Task<IActionResult> ObtenerUsuariosConInvernaderos () {
      try {
        var usuarios = await _iinvernaderoService.ObtenerUsuariosConInvernaderos ();
        return Ok (usuarios);
      } catch (Exception ex) {
        return StatusCode (500, $"Error al obtener los usuarios: {ex.Message}");
      }
    }



    [HttpDelete ("EliminarInvernadero/{nombreInvernadero}")]
    public async Task<IActionResult> EliminarInvernadero (string nombreInvernadero) {
      try {
        // Llamamos al servicio para eliminar el invernadero
        var result = await _iinvernaderoService.EliminarInvernaderoAsync (nombreInvernadero);

        if (result) {
          return Ok ("Invernadero eliminado correctamente.");
        }

        return NotFound ("El invernadero no fue encontrado.");
      } catch (Exception ex) {
        // Si ocurre un error, retornamos un mensaje de error
        return BadRequest ($"Error: {ex.Message}");
      }
    }
  }
}
