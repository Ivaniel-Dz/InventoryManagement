using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using InventoryManagementWebApp.ViewModels;
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
        public async Task<IActionResult> Create(Inventario model)
        {
            if (ModelState.IsValid)
            {
                // Busca el producto seleccionado
                var producto = await _appDbContext.Productos
                    .FirstOrDefaultAsync(p => p.Id == model.ProductoId);

                if (producto == null)
                {
                    ModelState.AddModelError("", "Producto no encontrado.");
                    model.Productos = await _appDbContext.Productos.ToListAsync();
                    return View(model);
                }

                // Actualiza la cantidad en Stock segun el tipo de movimiento
                if(model.TipoMovimiento == "Entrada")
                {
                    producto.CantidadStock += model.Cantidad;
                } else if (model.TipoMovimiento == "Salida") {
                    producto.CantidadStock -= model.Cantidad;
                }

                // Crea el movimiento de Inventario
                var movimientoInventario = new MovimientoInventario
                {
                    ProductoId = model.ProductoId,
                    TipoMovimiento = model.TipoMovimiento,
                    Fecha = model.Fecha,
                    Cantidad = model.Cantidad,
                    Descripcion = model.Descripcion,
                };

                // Guardar en la base de datos
                _appDbContext.MovimientoInventarios.Add( movimientoInventario );
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            model.Productos = await _appDbContext.Productos.ToListAsync();
            return View(model);
        }

        // Editar

        // Eliminar
    }
}
