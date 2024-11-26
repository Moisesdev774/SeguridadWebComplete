using SeguridadWeb.AccesoADatos;
using SeguridadWeb.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadWeb.LogicaDeNegocio
{
    public class ProductoBL
    {
        #region CRUD
        public async Task<int> CrearAsync(Producto pProducto)
        {
            return await ProductoDAL.CrearAsync(pProducto);
        }
        public async Task<int> ModificarAsync(Producto pProducto)
        {
            return await ProductoDAL.ModificarAsync(pProducto);
        }
        public async Task<int> EliminarAsync(Producto pProducto)
        {
            return await ProductoDAL.EliminarAsync(pProducto);
        }
        public async Task<Producto> ObtenerPorIdAsync(Producto pProducto)
        {
            return await ProductoDAL.ObtenerPorIdAsync(pProducto);
        }
        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await ProductoDAL.ObtenerTodosAsync();
        }
        public async Task<List<Producto>> BuscarAsync(Producto pProducto)
        {
            return await ProductoDAL.BuscarAsync(pProducto);
        }
        #endregion
        public async Task<List<Producto>> BuscarIncluirCategoriasAsync(Producto pProducto)
        {
            return await ProductoDAL.BuscarIncluirCategoriasAsync(pProducto);
        }
    }
}
