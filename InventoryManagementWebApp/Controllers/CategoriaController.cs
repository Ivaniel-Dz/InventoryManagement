using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class CategoriaController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public CategoriaController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Vista para agregar Categoria de Producto
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Agregar Categoria de Producto
        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            var existingCategory = await _appDbContext.Categorias.FirstOrDefaultAsync(c => c.NombreCategoria == categoria.NombreCategoria);

            if (existingCategory != null) {
                // Si ya existe, mostrar mensaje de error
                TempData["Mensaje"] = "Ya existe esta Categoría.";
                return RedirectToAction("Create", "Categoria");
            }

            await _appDbContext.Categorias.AddAsync(categoria);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Producto");
        } 
    }
}
