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
        public DbSet<Usuario> Usuarios { get; set; } // Tabla usuario
        public DbSet<Role> Roles { get; set; } // Tabla Role
        public DbSet<Categoria> Categorias { get; set; } // Tabla Categorias
        public DbSet<Producto> Productos { get; set; } // Tabla Producto
        public DbSet<MovimientoInventario> MovimientoInventarios { get; set; } // Tabla MovimientoInventario

        // Método para configurar la base de datos (se deja vacío para evitar sobreescritura)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

        // Configuracion del modelo y mapeo de las entidades a las tablas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configuracion de la tabla Usuario
            modelBuilder.Entity<Usuario>(tb =>
            {
                //Define los atribudos de la tabla
                tb.ToTable("Usuarios");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Correo).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(100);
                tb.Property(col => col.RolId).HasDefaultValue(2); // Valor por defecto "2"

                // Relacion con la tabla Role
                tb.HasOne(col => col.Role).WithMany(r => r.Usuarios)
                                          .HasForeignKey(col => col.RolId).OnDelete(DeleteBehavior.Restrict);
            });

            // Configuracion de la tabla Role
            modelBuilder.Entity<Role>(tb =>
            {
                tb.ToTable("Roles");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Rol).HasMaxLength(50);
            });

            //Configuracion de la tabla Categoria
            modelBuilder.Entity<Categoria>(tb =>
            {
                tb.ToTable("Categorias");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Descripcion).HasMaxLength(250);
            });

            //Configuracion de la tabla Producto
            modelBuilder.Entity<Producto>(tb =>
            {
                tb.ToTable("Productos");
                tb.HasKey (col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength (50);
                tb.Property(col => col.CategoriaId).HasColumnType("int");
                tb.Property(col => col.PrecioCompra).HasColumnType("decimal(10,2)");
                tb.Property(col => col.PrecioVenta).HasColumnType("decimal(10,2)");
                tb.Property(col => col.CantidadStock).HasColumnType("int");
                tb.Property(col => col.CodigoProducto).HasMaxLength(50);
                tb.Property(col => col.Descripcion).HasMaxLength(250);

                // Relacion Producto - Categoria
                tb.HasOne(col => col.Categoria).WithMany(cat => cat.Productos)
                            .HasForeignKey(col => col.CategoriaId).OnDelete(DeleteBehavior.Cascade);
            });

            //Configuracion de la tabla Moviemiento de Inventario
            modelBuilder.Entity<MovimientoInventario>(tb =>
            {
                tb.ToTable("MovimientoInventario");
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Movimiento).HasMaxLength(50);
                tb.Property(col => col.Fecha).HasColumnType("date");
                tb.Property(col => col.ProductoId).HasColumnType("int");
                tb.Property(col => col.Cantidad).HasColumnType("int");
                tb.Property(col => col.Descripcion).HasMaxLength (250);

                // Relacion MovimientoInvetario - Producto
                tb.HasOne(col => col.Producto).WithMany(p => p.MovimientoInventarios)
                                                .HasForeignKey(col => col.ProductoId).OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
