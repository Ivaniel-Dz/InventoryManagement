using InventoryManagementWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class ProductoController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public ProductoController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        //Vista de tabla de Producto
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Producto> lista = await _appDbContext.Productos.Include(p => p.Categoria).ToListAsync();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            var existingProduct = await _appDbContext.Productos
               .FirstOrDefaultAsync(p => p.CodigoProducto == producto.CodigoProducto);
            
            if (existingProduct != null) {
                // Si el correo ya existe, mostrar mensaje de error
                TempData["Mensaje"] = "Ya existe un Producto Igual.";
                return RedirectToAction("Create");
            }

            // Si no existe el Producto
            await _appDbContext.Productos.AddAsync(producto);
            await _appDbContext.SaveChangesAsync();
            //                      View    Controller
            return RedirectToAction("Index","Producto");
        }
        // Editar

        // Eliminar
    }
}
