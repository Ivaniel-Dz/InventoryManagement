using SistemaInventario.Models;

namespace SistemaInventario.ViewModels
{
    public class UsuarioVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string? Password { get; set; }

        // Lista de Roles para el dropdown
        public int RolId { get; set; }
        public List<Role> RolesDisponibles { get; set; } = new List<Role>();
    }
}
