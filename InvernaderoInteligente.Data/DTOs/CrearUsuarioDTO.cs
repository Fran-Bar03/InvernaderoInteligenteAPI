using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.DTOs {
  public class CrearUsuarioDTO 
  {
    [Required]
    public string? NombreCompleto { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Contrasena { get; set; }


  }
}
