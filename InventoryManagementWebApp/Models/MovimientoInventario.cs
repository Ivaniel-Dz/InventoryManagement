namespace InventoryManagementWebApp.Models
{
    public class MovimientoInventario
    {
        public int Id { get; set; }
        public string Movimiento { get; set; }
        public DateOnly Fecha { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Descripcion { get; set; } //null

        // Propiedad de Navegacion
        public Producto Producto { get; set; } // Relacion con Producto
    }
}
