using InvernaderoInteligente.Data.Services;
using InvernaderoInteligente.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvernaderoInteligente.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class LecturaSensorController : ControllerBase {

    private readonly LecturaSensorService _lecturaSensorService;

    public LecturaSensorController (LecturaSensorService lecturaSensorService) {
      _lecturaSensorService = lecturaSensorService;
    }



    // GET: api/LecturaSensor/ObtenerLecturasConInvernadero
    [HttpGet ("ObtenerLecturasConInvernadero")]
    public async Task<IActionResult> ObtenerLecturasConInvernadero () {
      var lecturasConInvernadero = await _lecturaSensorService.ObtenerLecturasConInvernadero ();
      return Ok (lecturasConInvernadero);  // Devuelve la lista de lecturas con invernadero
    }

    // POST: api/LecturaSensor/RegistrarLectura
    [HttpPost ("RegistrarLectura")]
    public async Task<IActionResult> RegistrarLectura ([FromBody] LecturaSensorModel lectura) {
      try {
        // Registrar la nueva lectura del sensor
        await _lecturaSensorService.RegistrarLectura (lectura);

        // Respuesta exitosa
        return Ok (new { mensaje = "Lectura registrada correctamente" });
      } catch (Exception ex) {
        // Si ocurre un error, devolver el mensaje
        return BadRequest (new { mensaje = ex.Message });
      }
    }




  }
}
