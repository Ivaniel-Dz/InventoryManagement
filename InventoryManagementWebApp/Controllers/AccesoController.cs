using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.ViewModels;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;

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
                ViewData["Mensaje"] = "Las Contraseña no coinciden"; // Muestra un mensaje de error
                return View(); // Retorna la vista con el mensaje de error.
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
                return RedirectToAction("Login", "Usuario");
            }

            ViewData["Mensaje"] = "No se creo el usuario"; // Muestra un mensaje de error
            return View(); // Retorna la vista con el mensaje de error.
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
    }
}
