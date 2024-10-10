using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

/*
 * Controlador para Mostrar, Editar y Elimar Usuarios (Acceso solo Admin)
 */

namespace InventoryManagementWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public UsuarioController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}
