using InventoryManagementWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using InventoryManagementWebApp.ViewModels;

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

        // Editar

        // Eliminar
    }
}
