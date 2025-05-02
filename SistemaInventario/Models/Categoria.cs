namespace SistemaInventario.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; } //null

        // Propiedad de navegación
        public ICollection<Producto> Productos { get; set; } // Relación con Producto

    }
}
