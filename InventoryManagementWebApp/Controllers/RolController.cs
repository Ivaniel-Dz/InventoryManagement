using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace InventoryManagementWebApp.Controllers
{
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

    }
}
