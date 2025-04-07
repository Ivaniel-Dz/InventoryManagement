using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public AdminController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        //Vista de la lista de usuarios
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Usuario> lista = await _appDbContext.Usuarios.ToListAsync();
            return View(lista);
        }

        // Muestra la vista para crear usuario
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Crear Usuario Nuevo
        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            // Verifica si el correo ya está registrado
            var existingUser = await _appDbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == usuario.Correo);

            if (existingUser != null)
            {
                // Si el correo ya existe, mostrar mensaje de error
                TempData["Warning"] = "Ya existe un usuario con este correo.";
                return RedirectToAction("Create");
            }

            // Si el correo no existe, proceder con la creación del usuario
            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Usuario Creado exitosamente.";
            return RedirectToAction("Index", "Admin");
        }


        // Redirige segun el Id a la vista de Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Usuario usuario = await _appDbContext.Usuarios.FirstAsync(u => u.Id == id);
            return View(usuario);
        }

        // Edita el usuario segun el Id
        [HttpPost]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            var existingUser = await _appDbContext.Usuarios.FindAsync(usuario.Id);
            if (existingUser == null)
            {
                TempData["Warning"] = "El usuario no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si el correo ya está en uso por otro usuario
            if (await _appDbContext.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != usuario.Id))
            {
                TempData["Warning"] = "El correo ya está en uso.";
                return RedirectToAction(nameof(Edit), new { id = usuario.Id });
            }

            // Actualiza las propiedades si el nuevo valor no es null o vacío
            existingUser.Nombre = string.IsNullOrWhiteSpace(usuario.Nombre) ? existingUser.Nombre : usuario.Nombre;
            existingUser.Correo = string.IsNullOrWhiteSpace(usuario.Correo) ? existingUser.Correo : usuario.Correo;
            existingUser.Password = string.IsNullOrWhiteSpace(usuario.Password) ? existingUser.Password : usuario.Password;

            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Usuario actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        //Eliminar un Usurio
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Usuario usuario = await _appDbContext.Usuarios.FirstAsync(u => u.Id == id);

            _appDbContext.Usuarios.Remove(usuario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "El Usuario ha sido eliminada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
