using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.DTOs {
  public class RecuperarContrasenaDTO 
  {
    [Required(ErrorMessage ="El token es obligatorio")]
    public string Token { get; set; }

    [MinLength (6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Contraseña { get; set; }

    [Compare ("Contraseña", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarContraseña { get; set; }
  }
}
