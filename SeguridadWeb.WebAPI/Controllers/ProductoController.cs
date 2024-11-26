using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Agregar la siguiente libreria para la seguridad JWT
using SeguridadWeb.WebAPI.Auth;
using Microsoft.AspNetCore.Authorization;
using SeguridadWeb.EntidadesDeNegocio;
using SeguridadWeb.LogicaDeNegocio;
using System.Text.Json;
// ***************************************************

namespace SeguridadWeb.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*[Authorize] */// agregar el siguiente metadato para autorizar JWT la Web API
    public class ProductoController : ControllerBase
    {
        private ProductoBL productoBL = new ProductoBL();
      
        //************************************************
        // GET: api/<UsuarioController>
        [HttpGet]
        public async Task<IEnumerable<Producto>> Get()
        {
            return await productoBL.ObtenerTodosAsync();
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{id}")]
        public async Task<Producto> Get(int id)
        {
            Producto producto = new Producto();
            producto.Id = id;
            return await productoBL.ObtenerPorIdAsync(producto);
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Producto producto)
        {
            try
            {
                // Validar que FotoProducto no sea nulo si es obligatorio
                if (producto.FotoProducto == null || producto.FotoProducto.Length == 0)
                {
                    return BadRequest("La imagen del producto es requerida.");
                }

                await productoBL.CrearAsync(producto);
                return Ok();
            }
            catch (Exception ex)
            {
                // Puedes registrar el error si tienes un sistema de logging
                return BadRequest(new { message = "Ocurrió un error al crear el producto.", details = ex.Message });
            }
        }


        // PUT api/<UsuarioController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] object pProducto)
        {
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string strProducto = JsonSerializer.Serialize(pProducto);
            Producto producto = JsonSerializer.Deserialize<Producto>(strProducto, option);
            if (producto.Id == id)
            {
                await productoBL.ModificarAsync(producto);
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }


        // DELETE api/<UsuarioController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Producto producto = new Producto();
                producto.Id = id;
                await productoBL.EliminarAsync(producto);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Buscar")]
        public async Task<List<Producto>> Buscar([FromBody] object pProducto)
        {

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string strProducto = JsonSerializer.Serialize(pProducto);
            Producto producto = JsonSerializer.Deserialize<Producto>(strProducto, option);
            var productos = await productoBL.BuscarIncluirCategoriasAsync(producto);
            productos.ForEach(s => s.Categoria.Producto = null); // Evitar la redundacia de datos
            return productos;

        }
    }
}
