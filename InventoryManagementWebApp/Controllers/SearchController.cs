using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementWebApp.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
