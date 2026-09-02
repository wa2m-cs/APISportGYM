using Microsoft.EntityFrameworkCore;
using APISportGYM.Models;

namespace APISportGYM.Data
{
    public class SportFitDbContext : DbContext
    {
        public SportFitDbContext(DbContextOptions<SportFitDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<VarianteProducto> VariantesProducto { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Nombres de tablas
            modelBuilder.Entity<Rol>()
                .ToTable("Rol", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuario", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Categoria>()
                .ToTable("Categoria", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Producto>()
                .ToTable("Producto", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<VarianteProducto>()
                .ToTable("VarianteProducto", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Pedido>()
                .ToTable("Pedido", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<DetallePedido>()
                .ToTable("DetallePedido", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Pago>()
                .ToTable("Pago", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Entrega>()
                .ToTable("Entrega", tb => tb.UseSqlOutputClause(false));
            modelBuilder.Entity<Auditoria>()
                .ToTable("Auditoria");

            modelBuilder.Entity<Rol>()
                .HasKey(r => r.IdRol);

            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.IdUsuario);

            modelBuilder.Entity<Categoria>()
                .HasKey(c => c.IdCategoria);

            modelBuilder.Entity<Producto>()
                .HasKey(p => p.IdProducto);

            modelBuilder.Entity<VarianteProducto>()
                .HasKey(v => v.IdVariante);

            modelBuilder.Entity<Pedido>()
                .HasKey(p => p.IdPedido);

            modelBuilder.Entity<DetallePedido>()
                .HasKey(d => d.IdDetalle);

            modelBuilder.Entity<Pago>()
                .HasKey(p => p.IdPago);

            modelBuilder.Entity<Entrega>()
                .HasKey(e => e.IdEntrega);

            modelBuilder.Entity<Auditoria>()
                .HasKey(a => a.IdAuditoria);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasComputedColumnSql("[Subtotal] + [CostoEnvio]", stored: true);

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.Subtotal)
                .HasComputedColumnSql("[Cantidad] * [PrecioUnitario]", stored: true);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.CostoEnvio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.IdCategoria);

            modelBuilder.Entity<VarianteProducto>()
                .HasOne(v => v.Producto)
                .WithMany(p => p.Variantes)
                .HasForeignKey(v => v.IdProducto);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdPedido);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Variante)
                .WithMany(v => v.DetallesPedido)
                .HasForeignKey(d => d.IdVariante);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Pedido)
                .WithOne(p => p.Pago)
                .HasForeignKey<Pago>(p => p.IdPedido);

            modelBuilder.Entity<Entrega>()
                .HasOne(e => e.Pedido)
                .WithOne(p => p.Entrega)
                .HasForeignKey<Entrega>(e => e.IdPedido);

            modelBuilder.Entity<Entrega>()
                .HasOne(e => e.Repartidor)
                .WithMany(u => u.Entregas)
                .HasForeignKey(e => e.IdRepartidor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}