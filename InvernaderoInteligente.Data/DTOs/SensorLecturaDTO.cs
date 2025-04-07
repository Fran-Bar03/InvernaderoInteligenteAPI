using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.DTOs {
  public class SensorLecturaDTO 
  {
    public string LecturaId { get; set; }
    public string SensorId { get; set; }
    public decimal Valor { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaLectura { get; set; }
    public string Invernadero { get; set; }
  }
}
