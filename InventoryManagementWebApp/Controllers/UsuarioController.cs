using Microsoft.AspNetCore.Mvc;
using InventoryManagementWebApp.Data;
using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementWebApp.Services;
using Microsoft.AspNetCore.Authentication;


namespace InventoryManagementWebApp.Controllers
{
    [Authorize(Roles = "Admin,Empleado")]
    public class UsuarioController : Controller
    {
        // Inyencion de dependencias 
        private readonly AppDBContext _appDbContext;
        private readonly EncryptPass _encryptPass;

        // Constructor recibe la instancia del contexto de BBDD
        public UsuarioController(AppDBContext appDbContext, EncryptPass encryptPass)
        {
            // Asigna el contexto recibido a la variable local
            _appDbContext = appDbContext;
            _encryptPass = encryptPass;
        }
        

        // Redirige segun el Id a la vista de Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _appDbContext.Usuarios.FindAsync(id);
            if (user == null) {
                TempData["Warning"] = "Usuario no encontrado.";
                return RedirectToAction("Index", "Producto");
            }

            // Modelo de la vista para llenar los campos de Edición 
            var model = new Usuario
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Correo = user.Correo,
                Password = user.Password,
            };

            // Retorna la vista con los datos cargados
            return View(model);
        }

        // Edita el usuario segun el Id
        [HttpPost]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            // Buscar al usuario existente por su ID
            var existingUser = await _appDbContext.Usuarios.FindAsync(usuario.Id);
            
            if (existingUser == null)
            {
                TempData["Warning"] = "El usuario no fue encontrado.";
                return RedirectToAction(nameof(Edit));
            }

            // Verificar si el correo ya está en uso por otro usuario
            if ( await _appDbContext.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != usuario.Id))
            {
                TempData["Warning"] = "El correo ya está en uso por otro usuario.";
                return RedirectToAction(nameof(Edit), new { id = usuario.Id });
            }

            // Actualiza las propiedades si el nuevo valor no es null o vacío
            existingUser.Nombre = string.IsNullOrWhiteSpace(usuario.Nombre) ? existingUser.Nombre : usuario.Nombre;
            existingUser.Correo = string.IsNullOrWhiteSpace(usuario.Correo) ? existingUser.Correo : usuario.Correo;
            existingUser.Password = string.IsNullOrWhiteSpace(usuario.Password) ? existingUser.Password : usuario.Password;

            // Actualizar contraseña solo si se proporciona una nueva
            if (!string.IsNullOrWhiteSpace(usuario.Password))
            {
                existingUser.Password = _encryptPass.encryptSHA256(usuario.Password);
            }

            // Guardar cambios en la base de datos
            _appDbContext.Usuarios.Update(existingUser);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Usuario actualizado exitosamente.";
            return RedirectToAction(nameof(Edit));
        }

        //Eliminar
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out int currentUserId) || id != currentUserId) {
                 return Forbid();
                }

                var usuario = await _appDbContext.Usuarios.FindAsync(id);
                if (usuario == null) {
                    return RedirectToAction("Login", "Acceso");
                }

                _appDbContext.Usuarios.Remove(usuario);
                await _appDbContext.SaveChangesAsync();
                await HttpContext.SignOutAsync();

                TempData["Success"] = "La cuenta ha sido eliminada exitosamente.";
                return RedirectToAction("Login", "Acceso");

            }
            catch (Exception) {
                TempData["Warning"] = "Ocurrió un error al intentar eliminar la cuenta.";
                return RedirectToAction(nameof(Edit));
            }
        }

    }
}
