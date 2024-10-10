using InventoryManagementWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementWebApp.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public ProductoController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [Authorize(Roles = "Empleado")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
