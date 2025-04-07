using InvernaderoInteligente.Data.Services;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvernaderoInteligente.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {

        private readonly SensorService _sensorService;

        public SensorController(SensorService sensorService) 
        {
            _sensorService = sensorService;
        }



        // GET: api/<SensorController>
        [HttpGet("MostarSensores")]
        public async Task<IActionResult> MostrarSensores() 
        {
            var Sensor = await _sensorService.MostrarSensores();
            return Ok(Sensor);
        }

        // GET api/<SensorController>/5
        [HttpGet("BuscarSensor/{Tipo}")]
        public async Task <IActionResult> BuscarSensor(string Tipo)
        {
            var Sensor = await _sensorService.BuscarSensor(Tipo);
            return Ok(Sensor);
        }

        // POST api/<SensorController>
        [HttpPost("AgregarSensor")]
        public async Task<IActionResult> AgregarSensor([FromBody] SensorModel sensor)
        {
            if (sensor == null)
            {
                return BadRequest();
            }

            var Resultado = await _sensorService.AgregarSensor(sensor);
            return Ok(Resultado);

        }

    // PUT api/<SensorController>/5
    //[HttpPut("{id}")]
    // public void Put(int id, [FromBody] string value)
    // {
    //}

    // DELETE api/<SensorController>/5
        [HttpDelete ("EliminarSensor/{Tipo}")]
         public async Task<IActionResult> EliminarSensor (string Tipo) {
      try {
        await _sensorService.EliminarSensor (Tipo);
        return Ok (new { Mensaje = "Sensor eliminado correctamente" });
      } catch (Exception ex) {
        return BadRequest (new { Error = ex.Message });
      }
    }
  }
}
