using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.DTOs
{
    public class UsuarioConInvernaderosDTO
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public int Rol { get; set; }
        public List<string> Invernaderos { get; set; }
    }
}
