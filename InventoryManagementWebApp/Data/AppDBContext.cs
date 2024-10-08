using InventoryManagementWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementWebApp.Data
{
    public class AppDBContext : DbContext
    {
        // Constructor por defecto
        public AppDBContext() {}

        // Constructor que recibe opciones de configuración para el contexto de la base de datos.
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // Definir las tablas del contexto
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<MovimientoInventario> MovimientoInventarios { get; set; }

        // Método para configurar la base de datos (se deja vacío para evitar sobreescritura)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

        // Configuracion del modelo y mapeo de las entidades a las tablas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configuracion de la tabla Usuario
            modelBuilder.Entity<Usuario>(tb =>
            {
                //Define los atribudos de la tabla
                tb.ToTable("Usuario");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Correo).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(50);
                tb.Property(col => col.Rol).HasMaxLength(30).HasDefaultValue("Empleado");
            });

            //Configuracion de la tabla Categoria
            modelBuilder.Entity<Categoria>(tb =>
            {
                tb.ToTable("Categoria");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.NombreCategoria).HasMaxLength(50);
                tb.Property(col => col.Descripcion).HasMaxLength(50);
            });

            //Configuracion de la tabla Producto
            modelBuilder.Entity<Producto>(tb =>
            {
                tb.ToTable("Producto");
                tb.HasKey (col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength (50);
                tb.Property(col => col.CategoriaId).HasColumnType("int");
                tb.Property(col => col.PrecioCompra).HasColumnType("decimal(10,2)");
                tb.Property(col => col.PrecioVenta).HasColumnType("decimal(10,2)");
                tb.Property(col => col.CantidadStock).HasColumnType("int");
                tb.Property(col => col.CodigoProducto).HasMaxLength(50);
                tb.Property(col => col.Descripcion).HasMaxLength(50);

                // Relacion Producto - Categoria
                tb.HasOne<Categoria>(p => p.Categoria)
                            .WithMany(c => c.Productos)
                            .HasForeignKey(p => p.CategoriaId)
                            .OnDelete(DeleteBehavior.Cascade);
            });

            //Configuracion de la tabla Moviemiento de Inventario
            modelBuilder.Entity<MovimientoInventario>(tb =>
            {
                tb.ToTable("MovimientoInventario");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.TipoMovimiento).HasMaxLength(50);
                tb.Property(col => col.Fecha).HasColumnType("date");
                tb.Property(col => col.ProductoId).HasColumnType("int");
                tb.Property(col => col.Cantidad).HasColumnType("int");
                tb.Property(col => col.Descripcion).HasMaxLength (50);

                // Relacion MovimientoInvetario - Producto
                tb.HasOne(m => m.Producto)
                .WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(m => m.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
