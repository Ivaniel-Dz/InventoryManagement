using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Crear

        // Editar

        // Eliminar
    }
}
