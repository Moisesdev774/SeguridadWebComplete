using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/********************************/
using SeguridadWeb.EntidadesDeNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

// Libreria necesarias para consumir la Web API
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

//**********************************************

namespace SeguridadWeb.UI.AppWebAspCore.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UsuarioController : Controller
    {
        // Codigo agregar para consumir la Web API
        private readonly HttpClient httpClient;
        public UsuarioController(HttpClient client)
        {
            httpClient = client;           
        }              
        private async Task<Usuario> ObtenerUsuarioPorIdAsync(Usuario pUsuario)
        {           
            Usuario usuario = new Usuario();         
            var response = await httpClient.GetAsync("Usuario/" + pUsuario.Id);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                usuario = JsonSerializer.Deserialize<Usuario>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return usuario;
        }
        private async Task<Rol> ObtenerRolPorIdAsync(Rol pRol)
        {           
            Rol rol = new Rol();         
            var response = await httpClient.GetAsync("Rol/" + pRol.Id);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                rol = JsonSerializer.Deserialize<Rol>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return rol;
        }
        private async Task<List<Rol>> ObtenerRolesAsync()
        {          
            List<Rol> roles = new List<Rol>();

            var response = await httpClient.GetAsync("Rol");            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                roles = JsonSerializer.Deserialize<List<Rol>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return roles;
        }     
        private void RefrescarToken()
        {
           var claimExpired= User.FindFirst(ClaimTypes.Expired);
            if (claimExpired != null)
            {
                var token = claimExpired.Value;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }           
        }                
        //****************************************
        // GET: UsuarioController
        public async Task<IActionResult> Index(Usuario pUsuario = null)
        {
            RefrescarToken();
            if (pUsuario == null)
                pUsuario = new Usuario();
            if (pUsuario.Top_Aux == 0)
                pUsuario.Top_Aux = 10;
            else if (pUsuario.Top_Aux == -1)
                pUsuario.Top_Aux = 0;
            // Codigo agregar para consumir la Web API           
            var usuarios = new List<Usuario>();
            var taskObtenerTodosRoles = ObtenerRolesAsync();
            var taskResponse = httpClient.PostAsJsonAsync("Usuario/Buscar", pUsuario);
            var response = await taskResponse;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Usuario");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                usuarios = JsonSerializer.Deserialize<List<Usuario>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            // ********************************************
            ViewBag.Top = pUsuario.Top_Aux;
            ViewBag.Roles = await taskObtenerTodosRoles;
            return View(usuarios);
        }

        // GET: UsuarioController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Usuario usuario = await ObtenerUsuarioPorIdAsync(new Usuario { Id = id });
            usuario.Rol = await ObtenerRolPorIdAsync(new Rol { Id = usuario.IdRol });
            //*******************************************************           
            return View(usuario);
        }

        // GET: UsuarioController/Create
        public async Task<IActionResult> Create()
        {
            RefrescarToken();
            // Codigo agregar para consumir la Web API
            ViewBag.Roles = await ObtenerRolesAsync();
            //*****************************************
            ViewBag.Error = "";
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario pUsuario)
        {
            RefrescarToken();
            try
            {
                // Codigo agregar para consumir la Web API 
               
                var response = await httpClient.PostAsJsonAsync("Usuario", pUsuario);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pUsuario);
                }
                // ********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Codigo agregar para consumir la Web API
                ViewBag.Roles = await ObtenerRolesAsync();
                //*****************************************
                return View(pUsuario);
            }
        }

        // GET: UsuarioController/Edit/5
        public async Task<IActionResult> Edit(Usuario pUsuario)
        {
            RefrescarToken();
            // Codigo agregar para consumir la Web API
            var taskObtenerTodosRoles = ObtenerRolesAsync();
            var taskObtenerPorId  =  ObtenerUsuarioPorIdAsync(pUsuario);
            // ***********************************************
            var usuario = await taskObtenerPorId;
            ViewBag.Roles = await taskObtenerTodosRoles;
            ViewBag.Error = "";
            return View(usuario);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario pUsuario)
        {
            RefrescarToken();
            try
            {
                // Codigo agregar para consumir la Web API             
                var response = await httpClient.PutAsJsonAsync("Usuario/" + id, pUsuario);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pUsuario);
                }
                // ************************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Codigo agregar para consumir la Web API
                ViewBag.Roles = await ObtenerRolesAsync();
                //*****************************************
                return View(pUsuario);
            }
        }

        // GET: UsuarioController/Delete/5
        public async Task<IActionResult> Delete(Usuario pUsuario)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Usuario usuario = await ObtenerUsuarioPorIdAsync(new Usuario { Id = pUsuario.Id });
            usuario.Rol = await ObtenerRolPorIdAsync(new Rol { Id = usuario.IdRol });
            //*******************************************************    
            ViewBag.Error = "";
            return View(usuario);
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Usuario pUsuario)
        {
            RefrescarToken();
            try
            {
                // Codigo agregar para consumir la Web API               
                var response = await httpClient.DeleteAsync("Usuario/" + id);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pUsuario);
                }
                // **********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Codigo agregar para consumir la Web API               
                Usuario usuario = await ObtenerUsuarioPorIdAsync(new Usuario { Id = pUsuario.Id });
                usuario.Rol = await ObtenerRolPorIdAsync(new Rol { Id = usuario.IdRol });
                // ***************************************               
                return View(usuario);
            }
        }
        // GET: UsuarioController/Create
        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Url = ReturnUrl;
            ViewBag.Error = "";
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Usuario pUsuario, string pReturnUrl = null)
        {
            try
            {
                // Codigo agregar para consumir la Web API                  
                var response = await httpClient.PostAsJsonAsync("Usuario/Login",pUsuario);
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    pUsuario.Top_Aux = 1;                    
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var responseBuscarUsuario = await httpClient.PostAsJsonAsync("Usuario/Buscar", pUsuario);                   
                    if (responseBuscarUsuario.IsSuccessStatusCode)
                    {
                        var responseBody = await responseBuscarUsuario.Content.ReadAsStringAsync();
                       var usuarios = JsonSerializer.Deserialize<List<Usuario>>(responseBody,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (usuarios != null && usuarios.Count > 0)
                        {
                            Usuario usuario = usuarios.FirstOrDefault();
                            usuario.Rol = await ObtenerRolPorIdAsync(new Rol { Id = usuario.IdRol });
                            var claims = new[] { new Claim(
                                ClaimTypes.Name, usuario.Login),
                                new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
                                new Claim(ClaimTypes.Expired,token)
                            };
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));                           
                        }
                    }                    
                }
                else
                    throw new Exception("Credenciales incorrectas");
                // ******************************************************************                
                if (!string.IsNullOrWhiteSpace(pReturnUrl))
                    return Redirect(pReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Url = pReturnUrl;
                ViewBag.Error = ex.Message;
                return View(new Usuario { Login = pUsuario.Login });
            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion(string ReturnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");
        }
        // GET: UsuarioController/Create
        public async Task<IActionResult> CambiarPassword()
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            var usuarios = new List<Usuario>();                    
            var response = await httpClient.PostAsJsonAsync("Usuario/Buscar", new Usuario { Login = User.Identity.Name, Top_Aux = 1 });
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Usuario");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                usuarios = JsonSerializer.Deserialize<List<Usuario>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            // ********************************************
            var usuarioActual = usuarios.FirstOrDefault();
            ViewBag.Error = "";
            return View(usuarioActual);
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(Usuario pUsuario, string pPasswordAnt)
        {
            RefrescarToken();
            try
            {
                // **********************************************************
                // Codigo agregar para consumir la Web API              
                pUsuario.ConfirmPassword_aux = pPasswordAnt;
                var response = await httpClient.PostAsJsonAsync("Usuario/CambiarPassword", pUsuario);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Login", "Usuario");
                }
                else
                {
                    var usuarios = new List<Usuario>();                 
                    var responseBuscar = await httpClient.PostAsJsonAsync("Usuario/Buscar", new Usuario { Login = User.Identity.Name, Top_Aux = 1 });
                    if (responseBuscar.IsSuccessStatusCode)
                    {
                        var responseBody = await responseBuscar.Content.ReadAsStringAsync();
                        usuarios = JsonSerializer.Deserialize<List<Usuario>>(responseBody,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    var usuarioActual = usuarios.FirstOrDefault();
                    return View(usuarioActual);
                }
                //************************************************************************
               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Codigo agregar para consumir la Web API                 
                var usuarios = new List<Usuario>();              
                var response = await httpClient.PostAsJsonAsync("Usuario/Buscar", new Usuario { Login = User.Identity.Name, Top_Aux = 1 }); ;
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    usuarios = JsonSerializer.Deserialize<List<Usuario>>(responseBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                // ********************************************
                var usuarioActual = usuarios.FirstOrDefault();
                return View(usuarioActual);
            }
        }
    }
}
