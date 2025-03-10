using InvernaderoInteligente.Data.Interfaces;
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

        public InvernaderoController(IInvernaderoService invernaderoService)
        {
            _invernaderoService = invernaderoService;
        }

        
        // GET: api/<InvernaderoController>
        [HttpGet("ListarInvernaderos")]
        public async Task<ActionResult<List<InvernaderoModel>>> GetAll()
        {
            var invernaderos = await _invernaderoService.ListarInvernaderos();
            return Ok(invernaderos);
        }

        // GET api/<InvernaderoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<InvernaderoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<InvernaderoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InvernaderoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
