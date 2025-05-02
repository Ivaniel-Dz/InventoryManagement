using SistemaInventario.Data;
using SistemaInventario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.ViewModels;

namespace SistemaInventario.Controllers
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
        public async Task<IActionResult> Create(InventarioVM model)
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
                if (model.Movimiento == "Entrada")
                {
                    producto.CantidadStock += model.Cantidad;

                } else if (model.Movimiento == "Salida") {
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
                    Movimiento = model.Movimiento,
                    Fecha = model.Fecha,
                    Cantidad = model.Cantidad,
                    Descripcion = model.Descripcion,
                };

                // Guardar en la base de datos
                _appDbContext.MovimientoInventarios.Add(movimientoInventario);
                await _appDbContext.SaveChangesAsync();

                TempData["Success"] = "Movimiento Agregado.";
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
            var movimientoInventario = await _appDbContext.MovimientoInventarios
                .Include(m => m.Producto).FirstOrDefaultAsync(m => m.Id == id);

            if (movimientoInventario == null) { return NotFound(); }

            // Crea los productos
            var productos = await _appDbContext.Productos.ToListAsync();

            var model = new InventarioVM()
            {
                ProductoId = movimientoInventario.ProductoId,
                Movimiento = movimientoInventario.Movimiento,
                Fecha = movimientoInventario.Fecha,
                Cantidad = movimientoInventario.Cantidad,
                Descripcion = movimientoInventario.Descripcion,
                Productos = productos
            };

            return View(model);
        }

        // Editar el movimiento seleccionado por id
        [HttpPost]
        public async Task<IActionResult> Edit(InventarioVM model)
        {
            if (ModelState.IsValid)
            {
                // Busca el Movimiento del Inventario en la BBDD
                var movimientoInventario = await _appDbContext.MovimientoInventarios.FirstOrDefaultAsync(m => m.Id == model.Id);

                // verifica si el mooviento existe
                if (movimientoInventario == null)
                {
                    TempData["Warning"] = "Movimiento no encontrado.";
                    return NotFound();
                }

                // Obtener el producto relacionado con el movimiento
                var producto = await _appDbContext.Productos.FirstOrDefaultAsync(p => p.Id == movimientoInventario.ProductoId);
                if (producto == null)
                {
                    TempData["Warning"] = "Producto no encontrado.";
                    return NotFound();
                }

                // Revertir la cantidad de stock del producto según el movimiento original
                if (movimientoInventario.Movimiento == "Entrada")
                {
                    producto.CantidadStock -= movimientoInventario.Cantidad;
                }
                else if (movimientoInventario.Movimiento == "Salida")
                {
                    producto.CantidadStock += movimientoInventario.Cantidad;
                }

                // Actualiza solo si el valor es no nulo o no vacío
                movimientoInventario.ProductoId = model.ProductoId != 0 ? model.ProductoId : movimientoInventario.ProductoId;
                movimientoInventario.Movimiento = !string.IsNullOrEmpty(model.Movimiento) ? model.Movimiento : movimientoInventario.Movimiento;
                movimientoInventario.Fecha = model.Fecha != DateOnly.MinValue ? model.Fecha : movimientoInventario.Fecha; //Actualiza la fecha si es distindo al anterior
                movimientoInventario.Cantidad = model.Cantidad != 0 ? model.Cantidad : movimientoInventario.Cantidad;
                movimientoInventario.Descripcion = !string.IsNullOrEmpty(model.Descripcion) ? model.Descripcion : movimientoInventario.Descripcion;

                // Volver a obtener el producto en caso de que se haya cambiado el ProductoId
                producto = await _appDbContext.Productos.FirstOrDefaultAsync(p => p.Id == movimientoInventario.ProductoId);
                if (producto == null)
                {
                    TempData["Warning"] = "Producto no encontrado.";
                    return NotFound();
                }

                // Actualizar la cantidad de stock según el nuevo movimiento
                if (movimientoInventario.Movimiento == "Entrada")
                {
                    producto.CantidadStock += movimientoInventario.Cantidad;
                }
                else if (movimientoInventario.Movimiento == "Salida")
                {
                    producto.CantidadStock -= movimientoInventario.Cantidad;
                }

                // Guarda los cambios en la BBDD
                await _appDbContext.SaveChangesAsync();
                // Mensaje de Connfirmación
                TempData["Success"] = "Inventario actualizado correctamente.";
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
            var movimientoInventario = await _appDbContext.MovimientoInventarios
                .Include(m => m.Producto).FirstAsync(m => m.Id == id);

            // verfica si existe le movimiento del producto
            if (movimientoInventario == null)
            {
                TempData["Warning"] = "Movimiento no encontrado";
                return RedirectToAction(nameof(Index));
            }

            // elimina solo el movimeinto, no el producto
            _appDbContext.MovimientoInventarios.Remove(movimientoInventario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Movimiento de Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));

        }
    }
}
