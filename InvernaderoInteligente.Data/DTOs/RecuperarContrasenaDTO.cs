using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.DTOs {
  public class RecuperarContrasenaDTO 
  {
    public string Email { get; set; }

    [MinLength (6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Contrasena { get; set; }
  }
}
