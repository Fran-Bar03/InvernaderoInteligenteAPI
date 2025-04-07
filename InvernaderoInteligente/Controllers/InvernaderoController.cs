using InvernaderoInteligente.Data.Interfaces;
using InvernaderoInteligente.Data.Services;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvernaderoInteligente.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvernaderoController : ControllerBase
    {


        private readonly IInvernaderoService _invernaderoService;
        private readonly InvernaderoService _iinvernaderoService;

        public InvernaderoController(IInvernaderoService invernaderoService, InvernaderoService invernaderoService1)
        {
            _invernaderoService = invernaderoService;
            _iinvernaderoService = invernaderoService1;
        }

        
        // GET: api/<InvernaderoController>
        [HttpGet("ListarInvernaderos")]
        public async Task<ActionResult<List<InvernaderoModel>>> GetAll()
        {
            var invernaderos = await _invernaderoService.ListarInvernaderos();
            return Ok(invernaderos);
        }

        // GET api/<InvernaderoController>/5
        [HttpGet("BuscarInvernadero/{Nombre}")]
        public async Task<IActionResult> BuscarInvernadero (string Nombre)
        {
           var Invernadero = await _invernaderoService.BuscarInvernadero(Nombre);
            return Ok(Invernadero);
        }

        // POST api/<InvernaderoController>
        [HttpPost("CrearInvernadero")]
        public async Task<IActionResult> CrearInvernadero([FromBody] InvernaderoModel agregarinvernadero)
        {
            if (agregarinvernadero == null)
            {
                return BadRequest();
            }

            var Resultado = await _invernaderoService.AgregarInvernadero(agregarinvernadero);
            return Ok(Resultado);

        }


        // PUT api/<InvernaderoController>/5
        [HttpPut("ActualizarInvernadero")]
        public async Task<IActionResult> ActualizarInvernadero(InvernaderoModel invernaderomodel) 
        {
            await _invernaderoService.ActualizarInvernadero(invernaderomodel);
            return Ok();
        }
        


        // DELETE api/<InvernaderoController>/5
        [HttpDelete("EliminarInvernadero{Nombre}")]
        public async Task<IActionResult> Eliminarinvernadero(string Nombre) 
        {
            await _invernaderoService.EliminarInvernadero(Nombre);
            return Ok(new {Mensaje = "Invernadero eliminado correctamente " });
        }


        // POST: api/Invernadero/AgregarInvernadero
        [HttpPost("AgregarInvernadero")]
        public async Task<IActionResult> AgregarInvernadero([FromBody] InvernaderoModel invernadero)
        {
            try
            {
                // Aquí pasamos la lista de usuarios
                var usuarios = invernadero.Usuarios;
                var result = await _iinvernaderoService.AgregarInvernaderoAsync(invernadero, usuarios);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Si ocurre algún error, retornar un mensaje de error
                return BadRequest($"Error: {ex.Message}");
            }
        }






        // GET: api/Invernadero/Usuarios
        [HttpGet("UsuariosConInvernaderos")]
        public async Task<IActionResult> ObtenerUsuariosConInvernaderos()
        {
            try
            {
                var usuarios = await _iinvernaderoService.ObtenerUsuariosConInvernaderos();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los usuarios: {ex.Message}");
            }
        }
    }
}
