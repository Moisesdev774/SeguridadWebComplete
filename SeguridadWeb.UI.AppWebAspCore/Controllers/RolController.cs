using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/********************************/
using SeguridadWeb.EntidadesDeNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

// Libreria necesarias para consumir la Web API
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Security.Claims; // seguridad por token
using System.Net.Http.Headers; // seguridad por token
//**********************************************

namespace SeguridadWeb.UI.AppWebAspCore.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RolController : Controller
    {
        // Codigo agregar para consumir la Web API
        private readonly HttpClient httpClient;
        public RolController(HttpClient client)
        {
            httpClient = client;
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
        private void RefrescarToken()
        {
            var claimExpired = User.FindFirst(ClaimTypes.Expired);
            if (claimExpired != null)
            {
                var token = claimExpired.Value;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        //***************************************
        // GET: RolController
        public async Task<IActionResult> Index(Rol pRol = null)
        {
            RefrescarToken();
            if (pRol == null)
                pRol = new Rol();
            if (pRol.Top_Aux == 0)
                pRol.Top_Aux = 10;
            else if (pRol.Top_Aux == -1)
                pRol.Top_Aux = 0;
            // Codigo agregar para consumir la Web API
            var roles = new List<Rol>();
            var response = await httpClient.PostAsJsonAsync("Rol/Buscar", pRol);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Usuario");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                roles = JsonSerializer.Deserialize<List<Rol>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            //******************************************
            ViewBag.Top = pRol.Top_Aux;
            return View(roles);
        }

        // GET: RolController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Rol rol = await ObtenerRolPorIdAsync(new Rol { Id = id });
            //*******************************************************
            return View(rol);
        }

        // GET: RolController/Create
        public IActionResult Create()
        {
            ViewBag.Error = "";
            return View();
        }

        // POST: RolController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rol pRol)
        {
            try
            {
                // Codigo agregar para consumir la Web API
                RefrescarToken();
                var response = await httpClient.PostAsJsonAsync("Rol", pRol);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pRol);
                }
                // ********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pRol);
            }
        }

        // GET: RolController/Edit/5
        public async Task<IActionResult> Edit(Rol pRol)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Rol rol = await ObtenerRolPorIdAsync(pRol);
            // ***********************************************
            ViewBag.Error = "";
            return View(rol);
        }

        // POST: RolController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rol pRol)
        {
            try
            {
                // Codigo agregar para consumir la Web API
                RefrescarToken();
                var response = await httpClient.PutAsJsonAsync("Rol/" + id, pRol);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pRol);
                }
                // ************************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pRol);
            }
        }

        // GET: RolController/Delete/5
        public async Task<IActionResult> Delete(Rol pRol)
        {
            ViewBag.Error = "";
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Rol rol = await ObtenerRolPorIdAsync(pRol);
            // ************************************************
            return View(rol);
        }

        // POST: RolController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Rol pRol)
        {
            try
            {
                // Codigo agregar para consumir la Web API
                RefrescarToken();
                var response = await httpClient.DeleteAsync("Rol/" + id);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pRol);
                }
                // **********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pRol);
            }
        }
    }
}
