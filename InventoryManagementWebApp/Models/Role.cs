namespace InventoryManagementWebApp.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Rol { get; set; }

        // Propiedad de Navegación 
        public ICollection<Usuario> Usuarios { get; set; } // Relaccion con Usuario
    }
}
