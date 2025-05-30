﻿using SistemaInventario.Data;
using SistemaInventario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.ViewModels;

namespace SistemaInventario.Controllers
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
            List<Producto> productos = await _appDbContext.Productos.Include(p => p.Categoria).ToListAsync();
            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Cargar todas las categorías para pasarlas a la vista
            var categorias = await _appDbContext.Categorias.ToListAsync();

            // Crear un nuevo producto con las categorías cargadas
            var model = new ProductoVM
            {
                Categorias = categorias // Pasar las categorías al ViewModel
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoVM model)
        {
            // Verificar si ya existe un producto con el mismo código
            var existingProduct = await _appDbContext.Productos
                .FirstOrDefaultAsync(p => p.CodigoProducto == model.CodigoProducto);

            if (existingProduct != null)
            {
                TempData["Warning"] = "Ya existe un Producto Igual.";
                // Si ya existe el producto, recargar las categorías y volver a mostrar el formulario
                model.Categorias = await _appDbContext.Categorias.ToListAsync();
                return View(model);
            }

            // Asignar los datos al model para guardar
            var producto = new Producto
            {
                Nombre = model.Nombre,
                PrecioCompra = model.PrecioCompra,
                PrecioVenta = model.PrecioVenta,
                CantidadStock = model.CantidadStock,
                CodigoProducto = model.CodigoProducto,
                Descripcion = model.Descripcion,
                CategoriaId = model.CategoriaId // Guardar el ID de la categoría seleccionada
            };

            await _appDbContext.Productos.AddAsync(producto);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Producto creada Correctamente!";
            return RedirectToAction("Index", "Producto");
        }

        // Muestra la vista para Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Busca el producto a editar junto con su categori relacionada
            var producto = await _appDbContext.Productos
                .Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            // Cargar categorías
            var categorias = await _appDbContext.Categorias.ToListAsync();

            // Asignar los datos al viewModel para el form
            var model = new ProductoVM()
            {
                Nombre = producto.Nombre,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                CantidadStock = producto.CantidadStock,
                CodigoProducto = producto.CodigoProducto,
                Descripcion = producto.Descripcion,
                CategoriaId = producto.CategoriaId,
                Categorias = categorias
            };

            return View(model);
        }

        // Editamos el producto seleccionado por Id
        [HttpPost]
        public async Task<IActionResult> Edit(ProductoVM model)
        {
            if (!ModelState.IsValid)
            {
                // Recarga la lista de categorias en caso de que haya errores de validación
                model.Categorias = await _appDbContext.Categorias.ToListAsync();
                return View(model);
            }

            // Busca el produco en la base de datos
            var producto = await _appDbContext.Productos.FirstOrDefaultAsync(p => p.Id == model.Id);

            // Verifica si el producto existe
            if (producto == null)
            {
                TempData["Warning"] = "Producto no encontrado.";
                return NotFound();
            }

            // Actualiza solo si el valor es no nulo o no vacío.
            producto.Nombre = !string.IsNullOrEmpty(model.Nombre) ?  model.Nombre : producto.Nombre;
            producto.PrecioCompra = model.PrecioCompra != 0 ? model.PrecioCompra : producto.PrecioCompra;
            producto.PrecioVenta = model.PrecioVenta != 0 ? model.PrecioVenta: producto.PrecioVenta;
            producto.CantidadStock = model.CantidadStock != 0 ? model.CantidadStock : producto.CantidadStock;
            producto.CodigoProducto = !string.IsNullOrEmpty(model.CodigoProducto) ? model.CodigoProducto : producto.CodigoProducto;
            producto.Descripcion = !string.IsNullOrEmpty(model.Descripcion) ? model.Descripcion : producto.Descripcion;
            producto.CategoriaId = model.CategoriaId != 0 ? model.CategoriaId : producto.CategoriaId;

            // Guarda los cambios en la BBDD
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Producto actualizado correctamente.";
            //                      View     Controller
            return RedirectToAction("Index", "Producto"); // Redirige a view de productos despues de guardar
        }

        // Eliminar
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Busca por su ID
            Producto producto = await _appDbContext.Productos
                .Include(p => p.Categoria) // Incluye la referencia a Categoría solo si es necesario para mostrar
                .FirstAsync(p => p.Id == id);

            // Verifica si existe
            if (producto == null)
            {
                TempData["Warning"] = "Producto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Elimina solo el producto, no la categoría
            _appDbContext.Productos.Remove(producto);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
