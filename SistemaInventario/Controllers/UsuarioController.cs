using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using SistemaInventario.Data;
using SistemaInventario.Services;
using SistemaInventario.ViewModels;

// Controller para manjer Perfil de Usuario
namespace SistemaInventario.Controllers
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
        

        // Vista para Editar Perfil
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _appDbContext.Usuarios.FindAsync(id);
            if (usuario == null) {
                TempData["Warning"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Edit));
            }

            // Modelo de la vista para llenar los campos de Edición 
            var model = new UsuarioVM()
            {
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Password = usuario.Password,
            };

            // Retorna la vista con los datos cargados
            return View(model);
        }

        // Edita perfil de Usuario
        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioVM model)
        {
            // Buscar si existe por su ID
            var usuario = await _appDbContext.Usuarios.FindAsync(model.Id);
            
            if (usuario == null)
            {
                TempData["Warning"] = "El usuario no fue encontrado.";
                return RedirectToAction(nameof(Edit));
            }

            // Verificar si el correo ya está en uso por otro usuario
            if ( await _appDbContext.Usuarios.AnyAsync(u => u.Correo == model.Correo && u.Id != model.Id))
            {
                TempData["Warning"] = "El correo ya está en uso por otro usuario.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }

            // Actualiza las propiedades si el nuevo valor no es null o vacío
            usuario.Nombre = string.IsNullOrWhiteSpace(model.Nombre) ? usuario.Nombre : model.Nombre;
            usuario.Correo = string.IsNullOrWhiteSpace(model.Correo) ? usuario.Correo : model.Correo;

            // Actualizar contraseña solo si se proporciona una nueva
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                usuario.Password = _encryptPass.encryptSHA256(model.Password);
            }

            // Guardar cambios en la base de datos
            _appDbContext.Usuarios.Update(usuario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(Edit));
        }

        //Eliminar Cuenta de Usuario
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
