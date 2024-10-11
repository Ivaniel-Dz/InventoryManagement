﻿using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.ViewModels;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

/*
 * Controlador para Crear Usario y Acceso a Login
 */

namespace InventoryManagementWebApp.Controllers
{
    public class AccesoController : Controller
    {
        // Contexto de BBDD para interatuar con las tablas
        private readonly AppDBContext _appDbContext;

        // Constructor recibe la instancia del contexto de BBDD
        public AccesoController(AppDBContext appDbContext)
        {
            // Asigna el contexto recibido a la variable local
            _appDbContext = appDbContext;
        }

        // Muestra la Vista de Login
        [HttpGet]
        public IActionResult Login() {
            // Si el usuario ya está autenticado, redirige a la página principal.
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Privacy", "Acesso");

            // Retorna la vista de inicio de sesión.
            return View();
        }

        // Verifica el usuario al tratar de ingresar
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            // Busca en la base de datos un usuario que coincida con el correo y la clave.
            Usuario? userFound = await _appDbContext.Usuarios
                .Where(u =>
                    u.Correo == model.Correo &&
                    u.Password == model.Password
                ).FirstOrDefaultAsync();

            // Condicion para el proceso de no encontro usuario
            if (userFound == null)
            {
                TempData["Mensaje"] = "Contraseña o Correo Incorrecto.";
                return RedirectToAction("Login"); // Retorna la vista con el mensaje de error.
            }

            // Crea una lista de reclamaciones (claims) que representan la identidad del usuario.
            List<Claim> claims = new List<Claim>()
            {
                // Agrega una reclamacines que incluye:
                new Claim(ClaimTypes.Name, userFound.Nombre), //Nombre
                new Claim(ClaimTypes.Email, userFound.Correo), //Correo
                new Claim(ClaimTypes.Role, userFound.Rol) // Rol
            };

            // Crea una identidad basada en las reclamaciones usando autenticación de cookies.
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Define las propiedades de la autenticación.
            AuthenticationProperties properties = new AuthenticationProperties() {
                // Permite que la sesión pueda ser refrescada.
                AllowRefresh = true, 
            };

            // Inicia la sesión del usuario de forma asíncrona.
            await HttpContext.SignInAsync(
                // Especifica el esquema de autenticación basado en cookies.
                CookieAuthenticationDefaults.AuthenticationScheme,
                // Asigna la identidad de reclamaciones al usuario.
                new ClaimsPrincipal(claimsIdentity),
                // Aplica las propiedades de autenticación definidas.
                properties
            );


            // Condicion para redirigir a la pagina principal segun su rol después de iniciar sesión.
            if (userFound.Rol == "Empleado")
            {
                return RedirectToAction("Index", "Producto");
            }
            else if (userFound.Rol == "Admin")
            {
                return RedirectToAction("Index", "Usuario");
            }

            // Si no coincide ningún rol, redirigir a una página por defecto.
            return RedirectToAction("Privacy", "Acceso");
        }

        // Muestra la vista para crear usuario
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Crear Usuario Nuevo
        [HttpPost]
        public async Task<IActionResult> Create(User model)
        {
            if (model.Password != model.ConfirPassword)
            {
                TempData["Mensaje"] = "Las Contraseña no coinciden."; // Muestra un mensaje de error
                return RedirectToAction("Create"); // Retorna la vista con el mensaje de error.
            }

            // Verificación si el correo ya está registrado
            var existingUser = await _appDbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == model.Correo);
            if (existingUser != null)
            {
                TempData["Mensaje"] = "Ya existe un usuario con este correo."; // Muestra un mensaje de error
                return RedirectToAction("Create"); // Retorna la vista con el mensaje de error.
            }

            // Crea un nuevo objeto de usuario basado en el modelo de vista recibido.
            Usuario usuario = new Usuario()
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Password = model.Password,
            };

            //Agrega y Guarda nuevo usauario a la BBDD
            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            // Verifica si el usuario fue creado correctamente.
            if (usuario.Id != 0)
            {
                //                      View, Controller
                return RedirectToAction("Login", "Acceso");
            }

            TempData["Mensaje"] = "No se creo el usuario."; // Muestra un mensaje de error
            return RedirectToAction("Create"); // Retorna la vista con el mensaje de error.
        }

        //Privacy
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // Evita que se almacene en caché la respuesta de la acción Error.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Acción que cierra la sesión del usuario.
        public async Task<IActionResult> Exit()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //                      View    Controlador  
            return RedirectToAction("Login", "Acceso");
        }
    }
}
