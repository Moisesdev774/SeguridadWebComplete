using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeguridadWeb.WebAPI.Auth;
using Microsoft.AspNetCore.Authorization;
using SeguridadWeb.EntidadesDeNegocio;
using SeguridadWeb.LogicaDeNegocio;
using System.Text.Json;

namespace SeguridadWeb.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private UsuarioBL usuarioBL = new UsuarioBL();
        private readonly IJwtAuthenticationService authService;

        public UsuarioController(IJwtAuthenticationService pAuthService)
        {
            authService = pAuthService;
        }

        // GET: api/<UsuarioController>
        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get()
        {
            return await usuarioBL.ObtenerTodosAsync();
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{id}")]
        public async Task<Usuario> Get(int id)
        {
            Usuario usuario = new Usuario { Id = id };
            return await usuarioBL.ObtenerPorIdAsync(usuario);
        }

        // POST api/<UsuarioController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                await usuarioBL.CrearAsync(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<UsuarioController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] object pUsuario)
        {
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string strUsuario = JsonSerializer.Serialize(pUsuario);
            Usuario usuario = JsonSerializer.Deserialize<Usuario>(strUsuario, option);

            if (usuario.Id == id)
            {
                try
                {
                    await usuarioBL.ModificarAsync(usuario);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("ID no coincide.");
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Usuario usuario = new Usuario { Id = id };
                await usuarioBL.EliminarAsync(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Buscar")]
        public async Task<List<Usuario>> Buscar([FromBody] object pUsuario)
        {
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string strUsuario = JsonSerializer.Serialize(pUsuario);
            Usuario usuario = JsonSerializer.Deserialize<Usuario>(strUsuario, option);

            var usuarios = await usuarioBL.BuscarIncluirRolesAsync(usuario);
            usuarios.ForEach(s => s.Rol.Usuario = null); // Evitar redundancia de datos
            return usuarios;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] object pUsuario)
        {
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string strUsuario = JsonSerializer.Serialize(pUsuario);
            Usuario usuario = JsonSerializer.Deserialize<Usuario>(strUsuario, option);

            Usuario usuario_auth = await usuarioBL.LoginAsync(usuario);
            if (usuario_auth != null && usuario_auth.Id > 0)
            {
                var token = authService.Authenticate(usuario_auth);
                return Ok(token);
            }
            return Unauthorized();
        }

        [HttpPost("CambiarPassword")]
        public async Task<ActionResult> CambiarPassword([FromBody] object pUsuario)
        {
            try
            {
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                string strUsuario = JsonSerializer.Serialize(pUsuario);
                Usuario usuario = JsonSerializer.Deserialize<Usuario>(strUsuario, option);

                await usuarioBL.CambiarPasswordAsync(usuario, usuario.ConfirmPassword_aux);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
