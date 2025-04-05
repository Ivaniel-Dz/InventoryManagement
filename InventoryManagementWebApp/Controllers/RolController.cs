using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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
            List<Role> roles = await _appDbContext.Roles.ToListAsync();
            return View(roles);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Agrega un rol nuevo
        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            //Verifica si el rol ya existe
            var existing = await _appDbContext.Roles.FirstOrDefaultAsync(r => r.Rol == role.Rol);

            if (existing == null) {
                // Si ya existe, mostrar mensaje de error
                TempData["Mensaje"] = "Ya existe el Rol.";
                return RedirectToAction("Create");
            }

            await _appDbContext.Roles.AddAsync(role);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }

        // Redirige segun el Id a la vista de Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Role role = await _appDbContext.Roles.FirstAsync(r => r.Id == id);
            return View(role);
        }


        // Editar Rol
        [HttpPost]
        public async Task<IActionResult> Edit(Role role)
        {
            var existingRole = await _appDbContext.Roles.FindAsync(role.Id);

            if (existingRole == null) {
                TempData["Mensaje"] = "El Rol no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Actualizar Datos
            existingRole.Rol = string.IsNullOrWhiteSpace(role.Rol) ? existingRole.Rol : role.Rol;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Eliminar Role
        public async Task<IActionResult> Delete(int Id)
        {
            Role role = await _appDbContext.Roles.FirstAsync(r => r.Id == Id);
            
            _appDbContext.Roles.Remove(role);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
