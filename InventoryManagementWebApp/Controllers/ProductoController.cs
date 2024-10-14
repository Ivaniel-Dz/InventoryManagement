using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using InventoryManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Cargar todas las categorías para pasarlas a la vista
            var categorias = await _appDbContext.Categorias.ToListAsync();

            // Crear un nuevo producto con las categorías cargadas
            var viewModel = new ProductoViewModel
            {
                Categorias = categorias // Pasar las categorías al ViewModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoViewModel viewModel)
        {
            // Verificar si ya existe un producto con el mismo código
            var existingProduct = await _appDbContext.Productos
                .FirstOrDefaultAsync(p => p.CodigoProducto == viewModel.CodigoProducto);

            if (existingProduct != null)
            {
                TempData["Mensaje"] = "Ya existe un Producto Igual.";
                // Si ya existe el producto, recargar las categorías y volver a mostrar el formulario
                viewModel.Categorias = await _appDbContext.Categorias.ToListAsync();
                return View(viewModel);
            }

            // Crear un nuevo producto a partir del ViewModel
            var producto = new Producto
            {
                Nombre = viewModel.Nombre,
                PrecioCompra = viewModel.PrecioCompra,
                PrecioVenta = viewModel.PrecioVenta,
                CantidadStock = viewModel.CantidadStock,
                CodigoProducto = viewModel.CodigoProducto,
                Descripcion = viewModel.Descripcion,
                CategoriaId = viewModel.CategoriaId // Guardar el ID de la categoría seleccionada
            };

            await _appDbContext.Productos.AddAsync(producto);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Producto");
        }

        // Muestra la vista para Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Busca el producto a editar junto con su categori relacionada
            var producto = await _appDbContext.Productos
                .Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) { return NotFound(); }

            // Crea un ViewModel con los datos del producto y lista de categorias
            var model = new ProductoViewModel()
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                CantidadStock = producto.CantidadStock,
                Descripcion = producto.Descripcion,
                CategoriaId = producto.CategoriaId,
                Categorias = await _appDbContext.Categorias.ToListAsync() // carga la lista de categoria
            };

            return View(model);
        }

        // Editamos el producto seleccionado por Id
        [HttpPost]
        public async Task<IActionResult> Edit(ProductoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Recarga la lista de categorias en caso de que haya errores de validación
                model.Categorias = await _appDbContext.Categorias.ToListAsync();
                return View(model);
            }

            // Busca el produco en la base de datos
            var producto = await _appDbContext.Productos.FirstOrDefaultAsync(p => p.Id == model.Id);

            if (producto == null) { return NotFound(); }

            // Actualiza los campos del Producto
            producto.Nombre = model.Nombre;
            producto.PrecioCompra = model.PrecioCompra;
            producto.PrecioVenta = model.PrecioVenta;
            producto.CantidadStock = model.CantidadStock;
            producto.CodigoProducto = model.CodigoProducto;
            producto.Descripcion = model.Descripcion;
            producto.CategoriaId = model.CategoriaId;

            // Guarda los cambios en la BBDD
            _appDbContext.Productos.Update(producto);
            await _appDbContext.SaveChangesAsync();
            //                      View     Controller
            return RedirectToAction("Index", "Producto"); // Redirige a view de productos despues de guardar
        }

        // Eliminar
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Busca el producto por su ID
            Producto producto = await _appDbContext.Productos
                .Include(p => p.Categoria) // Incluye la referencia a Categoría solo si es necesario para mostrar
                .FirstAsync(p => p.Id == id);

            // Verifica si el producto existe
            if (producto == null)
            {
                TempData["Mensaje"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Elimina solo el producto, no la categoría
            _appDbContext.Productos.Remove(producto);
            await _appDbContext.SaveChangesAsync();

            TempData["Mensaje"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
