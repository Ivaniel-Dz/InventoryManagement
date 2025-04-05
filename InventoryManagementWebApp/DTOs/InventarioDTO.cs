using InventoryManagementWebApp.Models;

namespace InventoryManagementWebApp.DTOs
{
    public class InventarioDTO
    {
        public int Id { get; set; }
        public string Movimiento { get; set; }
        public DateOnly Fecha { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Descripcion { get; set; } //null

        // Propiedad de Navegacion
        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
