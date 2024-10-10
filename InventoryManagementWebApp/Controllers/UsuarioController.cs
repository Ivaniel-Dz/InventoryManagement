using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementWebApp.ViewModels;

/*
 * Controlador para Mostrar, Editar y Elimar Usuarios (Acceso solo Admin)
 */

namespace InventoryManagementWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        // Contexto de BBDD para interatuar con las tablas
        private readonly AppDBContext _appDbContext;

        // Constructor recibe la instancia del contexto de BBDD
        public UsuarioController(AppDBContext appDbContext)
        {
            // Asigna el contexto recibido a la variable local
            _appDbContext = appDbContext;
        }
        
        //Vista de la lista de usuarios
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
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

        // Editar Usuario

        // Elminiar Usuario

    }
}
