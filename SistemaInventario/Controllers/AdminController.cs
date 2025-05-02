using SistemaInventario.Data;
using SistemaInventario.Models;
using SistemaInventario.Services;
using SistemaInventario.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaInventario.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Inyencion de dependencias
        private readonly AppDBContext _appDbContext;
        private readonly EncryptPass _encryptPass;

        public AdminController(AppDBContext appDbContext, EncryptPass encryptPass)
        {
            _appDbContext = appDbContext;
            _encryptPass = encryptPass;
        }


        //Vista de la lista de usuarios
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Usuario> lista = await _appDbContext.Usuarios.Include(u => u.Role).ToListAsync();
            return View(lista);
        }

        // Muestra la vista para crear usuario
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _appDbContext.Roles.ToListAsync();

            var model = new UsuarioVM
            {
                RolesDisponibles = roles
            };

            return View(model);
        }

        // Crear Usuario Nuevo
        [HttpPost]
        public async Task<IActionResult> Create(UsuarioVM model)
        {
            // Verifica si el correo ya está registrado
            var existingUser = await _appDbContext.Usuarios.
                FirstOrDefaultAsync(u => u.Correo == model.Correo);

            if (existingUser != null)
            {
                // Si el correo ya existe, mostrar mensaje de error
                TempData["Warning"] = "Ya existe un usuario con este correo.";
                // Si ya existe el producto, recargar las categorías y volver a mostrar el formulario
                model.RolesDisponibles = await _appDbContext.Roles.ToListAsync();
                return View(model);
            }

            // Asignar los datos al model para guardar
            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Password = _encryptPass.encryptSHA256(model.Password),
                RolId = model.RolId,
            };

            // Si el correo no existe, proceder con la creación del usuario
            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Usuario Creado exitosamente.";
            return RedirectToAction("Index", "Admin");
        }


        // Redirige segun el Id a la vista de Editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Busca el Usuario
            var usuario = await _appDbContext.Usuarios
                .Include(p => p.Role).FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();

            // Carga los roles
            var roles = await _appDbContext.Roles.ToListAsync();

            // Asignar los datos al viewModel para el form
            var model = new UsuarioVM()
            {
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Password = usuario.Password,
                RolId = usuario.RolId,
                RolesDisponibles = roles
            };

            return View(model);
        }

        // Edita el usuario segun el Id
        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioVM model)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                model.RolesDisponibles = await _appDbContext.Roles.ToListAsync();
                return View(model);
            }

            // Buscar el usuario
            var usuario = await _appDbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (usuario == null)
            {
                TempData["Warning"] = "El usuario no fue encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar correo único
            if (await _appDbContext.Usuarios.AnyAsync(u => u.Correo == model.Correo && u.Id != model.Id))
            {
                TempData["Warning"] = "El correo ya está en uso.";
                model.RolesDisponibles = await _appDbContext.Roles.ToListAsync();
                return View(model);
            }

            // Actualizar campos (excepto contraseña)
            usuario.Nombre = string.IsNullOrWhiteSpace(model.Nombre) ? usuario.Nombre : model.Nombre;
            usuario.Correo = string.IsNullOrWhiteSpace(model.Correo) ? usuario.Correo : model.Correo;
            usuario.RolId = model.RolId != 0 ? model.RolId : usuario.RolId;

            // Actualizar contraseña SOLO si se proporcionó una nueva
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                usuario.Password = _encryptPass.encryptSHA256(model.Password);
            }


            // Guardar cambios en la base de datos
            _appDbContext.Usuarios.Update(usuario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        //Eliminar un Usurio
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Busca por el Id y incluyendo su entidad relacionada
            Usuario usuario = await _appDbContext.Usuarios
                .Include(u => u.Role).FirstAsync(u => u.Id == id);

            // Verifca si existe
            if (usuario == null)
            {
                TempData["Warning"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Elimina solo el Usuario, no el rol relacionda
            _appDbContext.Usuarios.Remove(usuario);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "El Usuario ha sido eliminada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
