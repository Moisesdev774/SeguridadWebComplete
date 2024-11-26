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
using SeguridadWeb.LogicaDeNegocio;
using System.IO;

//**********************************************

namespace SeguridadWeb.UI.AppWebAspCore.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ProductoController : Controller
    {
        // Codigo agregar para consumir la Web API
        private readonly HttpClient httpClient;
        public ProductoController(HttpClient client)
        {
            httpClient = client;
        }
        private async Task<Producto> ObtenerProductoPorIdAsync(Producto pProducto)
        {
            Producto producto = new Producto();
            var response = await httpClient.GetAsync("Producto/" + pProducto.Id);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                producto = JsonSerializer.Deserialize<Producto>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return producto;
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
        private async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            List<Categoria> categorias = new List<Categoria>();

            var response = await httpClient.GetAsync("Categoria");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                categorias = JsonSerializer.Deserialize<List<Categoria>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return categorias;
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
        //****************************************
        // GET: UsuarioController
        public async Task<IActionResult> Index(Producto pProducto = null)
        {
            RefrescarToken();
            if (pProducto == null)
                pProducto = new Producto();
            if (pProducto.Top_Aux == 0)
                pProducto.Top_Aux = 10;
            else if (pProducto.Top_Aux == -1)
                pProducto.Top_Aux = 0;
            // Codigo agregar para consumir la Web API           
            var productos = new List<Producto>();
            var taskObtenerTodosCategorias = ObtenerCategoriasAsync();
            var taskResponse = httpClient.PostAsJsonAsync("Producto/Buscar", pProducto);
            var response = await taskResponse;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Login", "Usuario");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                productos = JsonSerializer.Deserialize<List<Producto>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            // ********************************************
            ViewBag.Top = pProducto.Top_Aux;
            ViewBag.Categorias = await taskObtenerTodosCategorias;
            return View(productos);
        }

        // GET: UsuarioController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Producto producto = await ObtenerProductoPorIdAsync(new Producto { Id = id });
            producto.Categoria = await ObtenerCategoriaPorIdAsync(new Categoria { Id = producto.IdCategoria });
            //*******************************************************           
            return View(producto);
        }

        // GET: UsuarioController/Create
        public async Task<IActionResult> Create()
        {
            RefrescarToken();
            try
            {
                // Obtén las categorías desde la API o cualquier otra fuente
                var categorias = await ObtenerCategoriasAsync();
                ViewBag.Categorias = categorias; // Asigna las categorías al ViewBag

                return View(new Producto()); // Devuelve un modelo vacío de Producto
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new Producto());
            }
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto pProducto)
        {
            RefrescarToken();
            try
            {
                if (pProducto.FotoProductoArchivo != null && pProducto.FotoProductoArchivo.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await pProducto.FotoProductoArchivo.CopyToAsync(ms);
                        pProducto.FotoProducto = ms.ToArray();
                    }
                }

                var response = await httpClient.PostAsJsonAsync("Producto", pProducto);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Ocurrió un error al consumir la Web API.";
                    ViewBag.Categorias = await ObtenerCategoriasAsync();
                    return View(pProducto);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Categorias = await ObtenerCategoriasAsync(); // Recargar categorías
                return View(pProducto);
            }
        }



        // GET: UsuarioController/Edit/5
        public async Task<IActionResult> Edit(Producto pProducto)
        {
            RefrescarToken();
            // Codigo agregar para consumir la Web API
            var taskObtenerTodosCategorias = ObtenerCategoriasAsync();
            var taskObtenerPorId = ObtenerProductoPorIdAsync(pProducto);
            // ***********************************************
            var categoria = await taskObtenerPorId;
            ViewBag.Categorias = await taskObtenerTodosCategorias;
            ViewBag.Error = "";
            return View(categoria);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto pProducto)
        {
            RefrescarToken();
            try
            {
                // Codigo agregar para consumir la Web API             
                var response = await httpClient.PutAsJsonAsync("Producto/" + id, pProducto);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pProducto);
                }
                // ************************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Codigo agregar para consumir la Web API
                ViewBag.Categorias = await ObtenerCategoriasAsync();
                //*****************************************
                return View(pProducto);
            }
        }

        // GET: UsuarioController/Delete/5
        public async Task<IActionResult> Delete(Producto pProducto)
        {
            // Codigo agregar para consumir la Web API
            RefrescarToken();
            Producto producto = await ObtenerProductoPorIdAsync(new Producto { Id = pProducto.Id });
            producto.Categoria = await ObtenerCategoriaPorIdAsync(new Categoria { Id = producto.IdCategoria });
            //*******************************************************    
            ViewBag.Error = "";
            return View(producto);
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Producto pProducto)
        {
            RefrescarToken();
            try
            {
                // Codigo agregar para consumir la Web API               
                var response = await httpClient.DeleteAsync("Producto/" + id);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Usuario");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Sucedio un error al consumir la WEP API";
                    return View(pProducto);
                }
                // **********************************************
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Codigo agregar para consumir la Web API               
                Producto producto = await ObtenerProductoPorIdAsync(new Producto { Id = pProducto.Id });
                producto.Categoria = await ObtenerCategoriaPorIdAsync(new Categoria { Id = producto.IdCategoria });
                // ***************************************               
                return View(producto);
            }
        }
    }
}
