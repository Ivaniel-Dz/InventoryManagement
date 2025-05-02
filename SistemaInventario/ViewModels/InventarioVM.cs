using SistemaInventario.Models;

namespace SistemaInventario.ViewModels
{
    public class InventarioVM
    {
        public int Id { get; set; }
        public string Movimiento { get; set; }
        public DateOnly Fecha { get; set; }
        public int Cantidad { get; set; }
        public string? Descripcion { get; set; } //null

        // Lista de Productos para el dropdown
        public int ProductoId { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
