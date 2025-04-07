using InvernaderoInteligente.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Interfaces
{
    public interface IInvernaderoService
    {

        public Task<InvernaderoModel?> AgregarInvernadero(InvernaderoModel agregarinvernadero);
        public Task<List<InvernaderoModel?>> ListarInvernaderos();
        public Task<InvernaderoModel> BuscarInvernadero(string Nombre);
        public Task<InvernaderoModel> EliminarInvernadero(string nombre);

    }
}
