using SistemaInventario.Data;
using SistemaInventario.DTOs;
using SistemaInventario.Models;
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
        public async Task<IActionResult> Index()
        {
            List<Categoria> lista = await _appDbContext.Categorias.ToListAsync();
            return View(lista);
        }

        // Vista para agregar Categoria de Producto
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Agregar Categoria de Producto
        [HttpPost]
        public async Task<IActionResult> Create(CategoriaDTO model)
        {
            var existingCategory = await _appDbContext.Categorias.FirstOrDefaultAsync(c => c.Nombre == model.Nombre);

            if (existingCategory != null) {
                // Si ya existe, mostrar mensaje de error
                TempData["Warning"] = "Ya existe esta Categoría.";
                return RedirectToAction("Create", "Categoria");
            }

            var categoria = new Categoria
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
            };

            await _appDbContext.Categorias.AddAsync(categoria);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Categoría Creada Correctamente.";
            return RedirectToAction("Index", "Categoria");
        }


        // Redirige segun el Id a la vista de Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _appDbContext.Categorias.FindAsync(id);

            if (categoria == null)
            {
                TempData["Warning"] = "Categoria no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Modelo de la vista para llenar los campos de Edición 
            var model = new CategoriaDTO()
            {
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
            };

            // Retorna la vista con los datos cargados
            return View(model);
        }


        // Editar Categoria
        [HttpPost]
        public async Task<IActionResult> Edit(CategoriaDTO model)
        {
            // Buscar si existe por su ID
            var categoria = await _appDbContext.Categorias.FindAsync(model.Id);

            if (categoria == null)
            {
                TempData["Warning"] = "Categoria no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }


            // Verificar si el rol ya esta creado
            if (await _appDbContext.Categorias.AnyAsync(c => c.Nombre == model.Nombre && c.Id != model.Id))
            {
                TempData["Warning"] = "Categoria ya existe.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }

            // Actualizar Datos
            categoria.Nombre = string.IsNullOrWhiteSpace(model.Nombre) ? categoria.Nombre : model.Nombre;
            categoria.Descripcion = string.IsNullOrWhiteSpace(model.Descripcion) ? categoria.Descripcion : model.Descripcion;

            // Guardar cambios en la base de datos
            _appDbContext.Categorias.Update(categoria);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Categoria Actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // Eliminar Categoria
        public async Task<IActionResult> Delete(int Id)
        {
            // Busca por su ID
            Categoria categoria = await _appDbContext.Categorias.FirstAsync(c => c.Id == Id);

            // Verifica si existe
            if (categoria == null)
            {
                TempData["Warning"] = "Categoria no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _appDbContext.Categorias.Remove(categoria);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Categoria eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
