using InventoryManagementWebApp.Models;

namespace InventoryManagementWebApp.ViewModels
{
    public class ProductoVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int CantidadStock { get; set; }
        public string CodigoProducto { get; set; }
        public string? Descripcion { get; set; }

        // Lista de categorías para el dropdown
        public int CategoriaId { get; set; }
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}