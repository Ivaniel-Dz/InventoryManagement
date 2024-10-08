namespace InventoryManagementWebApp.Models
{
    public class Producto
    {
        public int Id {  get; set; }
        public string Nombre { get; set; }
        public int CategoriaId { get; set; }
        public decimal PrecioCompra {  get; set; }
        public decimal PrecioVenta {  get; set; }
        public int CantidadStock { get; set; }
        public string CodigoProducto { get; set; }
        public string? Descripcion { get; set; } // null

        // Propiedad de Navegacion
        public Categoria Categoria { get; set; } // Relación con Categoria
        public ICollection<MovimientoInventario> MovimientoInventarios { get; set; } // Relación con MovimientoInventario
    }
}
