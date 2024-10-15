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
                if (model.TipoMovimiento == "Entrada")
                {
                    producto.CantidadStock += model.Cantidad;

                } else if (model.TipoMovimiento == "Salida") {
                    if (producto.CantidadStock < model.Cantidad)
                    {
                        ModelState.AddModelError("", "La cantidad de salida excede el stock disponible.");
                        model.Productos = await _appDbContext.Productos.ToListAsync();
                        return View(model);
                    }
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
                _appDbContext.MovimientoInventarios.Add(movimientoInventario);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            model.Productos = await _appDbContext.Productos.ToListAsync();
            return View(model);
        }

        // Vista para Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Busca el movimiento del producto a editar 
            var movimiento = await _appDbContext.MovimientoInventarios
                .Include(m => m.Producto).FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null) { return NotFound(); }

            // Crea los productos
            var productos = await _appDbContext.Productos.ToListAsync();

            var model = new Inventario()
            {
                ProductoId = movimiento.ProductoId,
                TipoMovimiento = movimiento.TipoMovimiento,
                Fecha = movimiento.Fecha,
                Cantidad = movimiento.Cantidad,
                Descripcion = movimiento.Descripcion,
                Productos = productos
            };

            return View(model);
        }

        // Editar el movimiento seleccionado por id
        [HttpPost]
        public async Task<IActionResult> Edit(Inventario model)
        {
            if (ModelState.IsValid)
            {
                // Busca el Movimiento del Inventario en la BBDD
                var movimiento = await _appDbContext.MovimientoInventarios.FirstOrDefaultAsync(m => m.Id == model.Id);

                // verifica si el mooviento existe
                if (movimiento == null)
                {
                    TempData["Mensaje"] = "Movimiento no encontrado.";
                    return NotFound();
                }

                // Actualiza solo si el valor es no nulo o no vacío
                movimiento.ProductoId = model.ProductoId != 0 ? model.ProductoId : movimiento.ProductoId;
                movimiento.TipoMovimiento = !string.IsNullOrEmpty(model.TipoMovimiento) ? model.TipoMovimiento : movimiento.TipoMovimiento;
                movimiento.Fecha = model.Fecha != DateOnly.MinValue ? model.Fecha : movimiento.Fecha; //Actualiza la fecha si es distindo al anterior
                movimiento.Cantidad = model.Cantidad != 0 ? model.Cantidad : movimiento.Cantidad;
                movimiento.Descripcion = !string.IsNullOrEmpty(model.Descripcion) ? model.Descripcion : movimiento.Descripcion;

                // Actualizar la cantidad en stock del producto si ha cambiado la cantidad
                var producto = await _appDbContext.Productos.FirstOrDefaultAsync(p => p.Id == model.ProductoId);
                if(producto == null)
                {
                    TempData["Mensaje"] = "Producto no encontrado.";
                    return NotFound();
                }

                if (model.TipoMovimiento == "Entrada")
                {
                    producto.CantidadStock += model.Cantidad - movimiento.Cantidad;
                }else if (model.TipoMovimiento == "Salida")
                {
                    producto.CantidadStock -= model.Cantidad - movimiento.Cantidad;
                }

                // Guarda los cambios en la BBDD
                await _appDbContext.SaveChangesAsync();
                // Mensaje de Connfirmación
                TempData["Mensaje"] = "Inventario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // Recargal la lista de productos en caso de errores
            model.Productos = await _appDbContext.Productos.ToListAsync();
            return View(model);
        }

        // Eliminar el movimiento seleccionado por id
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Busca el movimiento referenciado con producto por su Id
            var movimiento = await _appDbContext.MovimientoInventarios
                .Include(m => m.Producto).FirstAsync(m => m.Id == id);

            // verfica si existe le movimiento del producto
            if (movimiento == null)
            {
                TempData["Mensaje"] = "Movimiento no encontrado";
                return RedirectToAction(nameof(Index));
            }

            // elimina solo el movimeinto, no el producto
            _appDbContext.MovimientoInventarios.Remove(movimiento);
            await _appDbContext.SaveChangesAsync();

            TempData["Mensaje"] = "Movimiento de Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));

        }
    }
}
