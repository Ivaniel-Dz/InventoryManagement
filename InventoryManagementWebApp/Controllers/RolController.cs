using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.DTOs;
using InventoryManagementWebApp.Models;
using InventoryManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public RolController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Role> lista = await _appDbContext.Roles.ToListAsync();
            return View(lista);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Agrega un rol nuevo
        [HttpPost]
        public async Task<IActionResult> Create(RoleDTO model)
        {
            //Verifica si el rol ya existe
            var existing = await _appDbContext.Roles.FirstOrDefaultAsync(r => r.Rol == model.Rol);

            if (existing != null) {
                // Si ya existe, mostrar mensaje de error
                TempData["Warning"] = "Ya existe el Rol.";
                return RedirectToAction("Create");
            }

            var role = new Role
            {
                Rol = model.Rol
            };

            await _appDbContext.Roles.AddAsync(role);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "El Rol creado correctamente.";
            return RedirectToAction("Index", "Rol");
        }

        // Redirige segun el Id a la vista de Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _appDbContext.Roles.FindAsync(id);

            if (role == null)
            {
                TempData["Warning"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Modelo de la vista para llenar los campos de Edición 
            var model = new RoleDTO()
            {
                Rol = role.Rol
            };

            // Retorna la vista con los datos cargados
            return View(model);
        }


        // Editar Rol
        [HttpPost]
        public async Task<IActionResult> Edit(RoleDTO model)
        {
            // Buscar si existe por su ID
            var role = await _appDbContext.Roles.FindAsync(model.Id);

            if (role == null) {
                TempData["Warning"] = "El Rol no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }


            // Verificar si el rol ya esta creado
            if (await _appDbContext.Roles.AnyAsync(r => r.Rol == model.Rol && r.Id != model.Id))
            {
                TempData["Warning"] = "El rol ya esta creado.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }

            // Actualizar Datos
            role.Rol = string.IsNullOrWhiteSpace(model.Rol) ? role.Rol : model.Rol;

            // Guardar cambios en la base de datos
            _appDbContext.Roles.Update(role);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Rol actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Eliminar Role
        public async Task<IActionResult> Delete(int Id)
        {
            // Busca por su ID
            Role role = await _appDbContext.Roles.FirstAsync(r => r.Id == Id);

            // Verifica si existe
            if (role == null)
            {
                TempData["Warning"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _appDbContext.Roles.Remove(role);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Rol eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
