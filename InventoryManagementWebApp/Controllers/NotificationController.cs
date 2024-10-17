using InventoryManagementWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public NotificationController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int umbralMinimo = 5;

            var productosStockBajo = await _appDbContext.Productos
                .Where(p => p.CantidadStock <= umbralMinimo).ToListAsync();

            return View(productosStockBajo);
        }
    }
}
