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
    public class CategoriaController : Controller
    {
        // Codigo agregar para consumir la Web API
        private readonly HttpClient httpClient;
        public CategoriaController(HttpClient client)
        {
            httpClient = client;
        }
        private async Task<Categoria> ObtenerCategoriaPorIdAsync(Categoria pCategoria)
        {
            Categoria categoria = new Categoria();
            var response = await httpClient.GetAsync("Categoria/" + pCategoria.Id);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                categoria = JsonSerializer.Deserialize<Categoria>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return categoria;
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
        // GET: CategoriaController
        public async Task<IActionResult> Index(Categoria pCategoria = null)
        {
            RefrescarToken();
            if (pCategoria == null)
                pCategoria = new Categoria();
            if (pCategoria.Top_Aux == 0)
                pCategoria.Top_Aux = 10;
            else if (pCategoria.Top_Aux == -1)
                pCategoria.Top_Aux = 0;
            // Codigo agregar para consumir la Web API
            var categorias = new List<Categoria>();
            var response = await httpClient.PostAsJsonAsync("Categoria/Buscar", pCategoria);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Usuario");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                categorias = JsonSerializer.Deserialize<List<Categoria>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            //******************************************
            ViewBag.Top = pCategoria.Top_Aux;
            return View(categorias);
        }

        // GET: CategoriaController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Categoria categoria = await ObtenerCategoriaPorIdAsync(new Categoria { Id = id });
            //*******************************************************
            return View(categoria);
        }

        // GET: CategoriaController/Create
        public IActionResult Create()
        {
            ViewBag.Error = "";
            return View();
        }

        // POST: CategoriaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria pCategoria)
        {
            try
            {
                // Codigo agregar para consumir la Web API
                RefrescarToken();
                var response = await httpClient.PostAsJsonAsync("Categoria", pCategoria);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pCategoria);
                }
                // ********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pCategoria);
            }
        }

        // GET: CategoriaController/Edit/5
        public async Task<IActionResult> Edit(Categoria pCategoria)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Categoria categoria = await ObtenerCategoriaPorIdAsync(pCategoria);
            // ***********************************************
            ViewBag.Error = "";
            return View(categoria);
        }

        // POST: CategoriaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria pCategoria)
        {
            try
            {
                // Codigo agregar para consumir la Web API
                RefrescarToken();
                var response = await httpClient.PutAsJsonAsync("Categoria/" + id, pCategoria);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pCategoria);
                }
                // ************************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pCategoria);
            }
        }

        // GET:CategoriaController/Delete/5
        public async Task<IActionResult> Delete(Categoria pCategoria)
        {
            ViewBag.Error = "";
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Categoria categoria = await ObtenerCategoriaPorIdAsync(pCategoria);
            // ************************************************
            return View(categoria);
        }

        // POST: CategoriaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Categoria pCategoria)
        {
            try
            {
                // Codigo agregar para consumir la Web API
                RefrescarToken();
                var response = await httpClient.DeleteAsync("Categoria/" + id);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pCategoria);
                }
                // **********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pCategoria);
            }
        }
    }
}
