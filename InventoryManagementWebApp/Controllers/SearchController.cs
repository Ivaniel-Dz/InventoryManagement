using InventoryManagementWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    public class SearchController : Controller
    {
        public readonly AppDBContext _appDBContext;

        public SearchController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<IActionResult> BuscarProducto(string? term)
        {
            // Consulta de la BD con Include para evitar N+1
            var query = _appDBContext.Productos.Include(p => p.Categoria).AsQueryable();

            // Aplicar filtro solo si hay término de búsqueda
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(p =>
                        p.Nombre.Contains(term) ||
                        p.CodigoProducto.Contains(term) ||
                        p.Categoria.Nombre.Contains(term) ||
                        p.PrecioCompra.ToString().Contains(term) ||
                        p.PrecioVenta.ToString().Contains(term) ||
                        p.CantidadStock.ToString().Contains(term));
            }

            var productos = await query.ToListAsync();

            // Mensaje de feedback
            if (!string.IsNullOrEmpty(term) && !productos.Any())
            {
                TempData["Warning"] = $"No se encontraron productos para '{term}'.";
            }
            else if (!string.IsNullOrEmpty(term))
            {
                TempData["Success"] = $"Se encontraron {productos.Count} productos.";
            }

            return View(productos);
        }
    }
}
