using Microsoft.EntityFrameworkCore;
using SeguridadWeb.EntidadesDeNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadWeb.AccesoADatos
{
    public class ProductoDAL
    {
        #region CRUD
        public static async Task<int> CrearAsync(Producto pProducto)
        {
            int result = 0;
            using (var bdContexto = new BDContexto())
            {        
                    bdContexto.Add(pProducto);
                    result = await bdContexto.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> ModificarAsync(Producto pProducto)
        {
            int result = 0;
            using (var bdContexto = new BDContexto())
            {
               
                var producto = await bdContexto.Producto.FirstOrDefaultAsync(s => s.Id == pProducto.Id);
                producto.IdCategoria = pProducto.IdCategoria;
                producto.Nombre = pProducto.Nombre;
                producto.Precio = pProducto.Precio;
                producto.Cantidad = pProducto.Cantidad;
                producto.Descripcion = pProducto.Descripcion;
                producto.FotoProducto = pProducto.FotoProducto; 
                producto.Estatus = pProducto.Estatus;
                bdContexto.Update(producto);
                result = await bdContexto.SaveChangesAsync();
              
            }
            return result;
        }
        public static async Task<int> EliminarAsync(Producto pProducto)
        {
            int result = 0;
            using (var bdContexto = new BDContexto())
            {
                var producto = await bdContexto.Producto.FirstOrDefaultAsync(s => s.Id == pProducto.Id);
                bdContexto.Producto.Remove(producto);
                result = await bdContexto.SaveChangesAsync();
            }
            return result;
        }
        public static async Task<Producto> ObtenerPorIdAsync(Producto pProducto)
        {
            var producto = new Producto();
            using (var bdContexto = new BDContexto())
            {
                producto = await bdContexto.Producto.FirstOrDefaultAsync(s => s.Id == pProducto.Id);
            }
            return producto;
        }
        public static async Task<List<Producto>> ObtenerTodosAsync()
        {
            var productos = new List<Producto>();
            using (var bdContexto = new BDContexto())
            {
                productos = await bdContexto.Producto.ToListAsync();
            }
            return productos;
        }
        internal static IQueryable<Producto> QuerySelect(IQueryable<Producto> pQuery, Producto pProducto)
        {
            if (pProducto.Id > 0)
                pQuery = pQuery.Where(s => s.Id == pProducto.Id);
            if (pProducto.IdCategoria > 0)
                pQuery = pQuery.Where(s => s.IdCategoria == pProducto.IdCategoria);
            if (!string.IsNullOrWhiteSpace(pProducto.Nombre))
                pQuery = pQuery.Where(s => s.Nombre.Contains(pProducto.Nombre));
            if (pProducto.Precio > 0)
                pQuery = pQuery.Where(s => s.Precio == pProducto.Precio);
            if (pProducto.Cantidad > 0)
                pQuery = pQuery.Where(s => s.Cantidad == pProducto.Cantidad);
            if (!string.IsNullOrWhiteSpace(pProducto.Descripcion))
                pQuery = pQuery.Where(s => s.Descripcion.Contains(pProducto.Descripcion));
            if (pProducto.Estatus > 0)
                pQuery = pQuery.Where(s => s.Estatus == pProducto.Estatus);
            pQuery = pQuery.OrderByDescending(s => s.Id).AsQueryable();
            if (pProducto.Top_Aux > 0)
                pQuery = pQuery.Take(pProducto.Top_Aux).AsQueryable();
            return pQuery;
        }
        public static async Task<List<Producto>> BuscarAsync(Producto pProducto)
        {
            var Productos = new List<Producto>();
            using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Producto.AsQueryable();
                select = QuerySelect(select, pProducto);
                Productos = await select.ToListAsync();
            }
            return Productos;
        }
        #endregion
        public static async Task<List<Producto>> BuscarIncluirCategoriasAsync(Producto pProducto)
        {
            var productos = new List<Producto>();
            using (var bdContexto = new BDContexto())
            {
                var select = bdContexto.Producto.AsQueryable();
                select = QuerySelect(select, pProducto).Include(s => s.Categoria).AsQueryable();
                productos = await select.ToListAsync();
            }
            return productos;
        }
    }
}
