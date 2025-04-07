using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.DTOs;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using InventoryManagementWebApp.Services;

/*
 * Controlador para Crear Usario y Acceso a Login
 */

namespace InventoryManagementWebApp.Controllers
{
    [AllowAnonymous]
    public class AccesoController : Controller
    {
        // Contexto de BBDD para interatuar con las tablas
        private readonly AppDBContext _appDbContext;
        // Inyencion de dependencia
        private readonly EncryptPass _encryptPass;

        // Constructor
        public AccesoController(AppDBContext appDbContext, EncryptPass encryptPass)
        {
            // Asigna el contexto recibido a la variable local
            _appDbContext = appDbContext;
            _encryptPass = encryptPass;
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
        public async Task<IActionResult> Login(LoginDTO model)
        {
            // Busca en la base de datos un usuario que coincida con el correo y la clave.
            Usuario? userFound = await _appDbContext.Usuarios
                .Include(u => u.Role)
                .Where(u =>
                    u.Correo == model.Correo &&
                    u.Password == _encryptPass.encryptSHA256(model.Password)
                ).FirstOrDefaultAsync();

            // Condicion para el proceso de no encontro usuario
            if (userFound == null)
            {
                TempData["Warning"] = "Contraseña o Correo Incorrecto.";
                return RedirectToAction("Login"); // Retorna la vista con el mensaje de error.
            }

            if(userFound.Role == null)
            {
                TempData["Warning"] = "Error de configuración: Usuario sin rol asignado.";
                return RedirectToAction("Login");
            }

            // Crea una lista de reclamaciones (claims) que representan la identidad del usuario.
            List<Claim> claims = new List<Claim>()
            {
                // Agrega una reclamacines que incluye:
                new Claim(ClaimTypes.NameIdentifier, userFound.Id.ToString()),
                new Claim(ClaimTypes.Name, userFound.Nombre),
                new Claim(ClaimTypes.Email, userFound.Correo),
                // Accede al nombre del rol a través de la propiedad de navegación
                new Claim(ClaimTypes.Role, userFound.Role.Rol)
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
            if (userFound.Role.Rol == "Empleado")
            {
                return RedirectToAction("Index", "Producto");
            }
            else if (userFound.Role.Rol == "Admin")
            {
                return RedirectToAction("Index", "Admin");
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
        public async Task<IActionResult> Create(RegistroDTO model)
        {
            if (model.Password != model.ConfirPassword)
            {
                TempData["Warning"] = "Las Contraseña no coinciden."; // Muestra un mensaje de error
                return RedirectToAction("Create"); // Retorna la vista con el mensaje de error.
            }

            // Verificación si el correo ya está registrado
            var existingUser = await _appDbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == model.Correo);
            if (existingUser != null)
            {
                TempData["Warning"] = "Ya existe un usuario con este correo."; // Muestra un mensaje de error
                return RedirectToAction("Create"); // Retorna la vista con el mensaje de error.
            }

            // Crea un nuevo objeto de usuario basado en el modelo de vista recibido.
            Usuario usuario = new Usuario()
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Password = _encryptPass.encryptSHA256(model.Password)
            };

            //Agrega y Guarda nuevo usauario a la BBDD
            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            // Verifica si el usuario fue creado correctamente.
            if (usuario.Id != 0)
            {
                TempData["Success"] = "Cuenta Creada Correctamente.";
                return RedirectToAction("Login", "Acceso"); // View, Controller
            }

            TempData["Warning"] = "No se creo el usuario."; // Muestra un mensaje de error
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
