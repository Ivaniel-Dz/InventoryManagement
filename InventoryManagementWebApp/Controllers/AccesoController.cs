using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.ViewModels;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                TempData["Mensaje"] = "Las Contraseña no coinciden"; // Muestra un mensaje de error
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

            TempData["Mensaje"] = "No se creo el usuario"; // Muestra un mensaje de error
            return RedirectToAction("Create"); // Retorna la vista con el mensaje de error.
        }

        // Muestra la Vista de Login
        [HttpGet]
        public IActionResult Login() {
            // Si el usuario ya está autenticado, redirige a la página principal.
            if (User.Identity!.IsAuthenticated)
            {
                //                      View, Controller
                return RedirectToAction("Index", "Home");
            }

            // Retorna la vista de inicio de sesión.
            return View();
        }

        // Verifica el usuario al tratar de ingresar
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            // Busca en la base de datos un usuario que coincida con el correo y la clave.
            Usuario? userFound = await _appDbContext.Usuarios
                .Where(user =>
                    user.Correo == model.Correo &&
                    user.Password == model.Password
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
                 // Agrega una reclamación que incluye el nombre completo del usuario.
                new Claim(ClaimTypes.Name, userFound.Nombre)
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

            // Redirige al usuario a la página principal después de iniciar sesión.
            return RedirectToAction("Index", "Home");
        }
    }
}
