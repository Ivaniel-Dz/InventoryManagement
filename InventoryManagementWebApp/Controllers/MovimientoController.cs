using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Empleado")]
    public class MovimientoController : Controller
    {

        private readonly AppDBContext _appDbContext;
        public MovimientoController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }

        //Vista de tabla de Movimiento del Inventario
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<MovimientoInventario> lista = await _appDbContext.MovimientoInventarios.Include(m => m.Producto).ToListAsync();
            return View(lista);
        }

        // Vista para Crear
        [HttpGet]
        public IActionResult Create() 
        {
            ViewBag.Productos = new SelectList(_appDbContext.Productos, "Id", "Nombre");
            return View();
        }

        // Agregar Movimiento
        [HttpPost]
        public async Task<IActionResult> Create(MovimientoInventario model)
        {
            if (ModelState.IsValid) { 
                _appDbContext.MovimientoInventarios.Add(model);

                var producto = await _appDbContext.Productos.FindAsync(model.ProductoId);

                if (model.TipoMovimiento == "Entrada")
                {
                    producto.CantidadStock += model.Cantidad;
                }
                else if(model.TipoMovimiento == "Salida")
                {
                    producto.CantidadStock -= model.Cantidad;
                }

                await _appDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Vuelve a cargar la lista de productos si el modelo no es válido
            ViewBag.Productos = new SelectList(_appDbContext.Productos, "Id", "Nombre", model.ProductoId);
            return View(model);
        }

        // Editar

        // Eliminar
    }
}
