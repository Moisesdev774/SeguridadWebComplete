using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Agregar la siguiente librerias
using SeguridadWeb.EntidadesDeNegocio;
using SeguridadWeb.LogicaDeNegocio;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace SeguridadWeb.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // agregar el siguiente metadato para autorizar JWT la Web API
    public class CategoriaController : ControllerBase
    {
        private CategoriaBL categoriaBL = new CategoriaBL();

        // GET: api/<CategoriaController>
        [HttpGet]
        public async Task<IEnumerable<Categoria>> Get()
        {
            return await categoriaBL.ObtenerTodosAsync();
        }

        // GET api/<CategoriaController>/5
        [HttpGet("{id}")]
        public async Task<Categoria> Get(int id)
        {
            Categoria categoria = new Categoria();
            categoria.Id = id;
            return await categoriaBL.ObtenerPorIdAsync(categoria);
        }

        // POST api/<CategoriaController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Categoria categoria)
        {
            try
            {
                await categoriaBL.CrearAsync(categoria);
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        // PUT api/<CategoriaController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Categoria categoria)
        {

            if (categoria.Id == id)
            {
                await categoriaBL.ModificarAsync(categoria);
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

        // DELETE api/<CategoriaController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Categoria categoria = new Categoria();
                categoria.Id = id;
                await categoriaBL.EliminarAsync(categoria);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Buscar")]
        public async Task<List<Categoria>> Buscar([FromBody] object pCategoria)
        {

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string strCategoria = JsonSerializer.Serialize(pCategoria);
            Categoria categoria = JsonSerializer.Deserialize<Categoria>(strCategoria, option);
            return await categoriaBL.BuscarAsync(categoria);

        }
    }
}
