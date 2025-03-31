using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrderTrack.Models.Domain;

public partial class OrdertrackContext : DbContext
{
    public OrdertrackContext()
    {
    }

    public OrdertrackContext(DbContextOptions<OrdertrackContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<Logistica> Logisticas { get; set; }

    public virtual DbSet<Novedade> Novedades { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Tienda> Tiendas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ALVARO;Database=ORDERTRACK;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente);

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(255);
            entity.Property(e => e.NumeroIdentificacion).HasMaxLength(50);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoIdentificacion).HasMaxLength(50);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);

            entity.ToTable("DetallePedido");

            entity.HasIndex(e => e.IdPedidoInterno, "idx_DetallePedidos_Pedido");

            entity.HasIndex(e => e.IdProducto, "idx_DetallePedidos_Producto");

            entity.Property(e => e.Ganancia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PosibleGanancia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioProovedor).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioProovedorCantidad).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdPedidoInternoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdPedidoInterno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePedido_Pedidos");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePedido_Productos");
        });

        modelBuilder.Entity<Logistica>(entity =>
        {
            entity.HasKey(e => e.IdLogistica);

            entity.ToTable("Logistica");

            entity.HasIndex(e => e.IdPedidoInterno, "idx_Logistica_Pedido");

            entity.Property(e => e.Ciudad).HasMaxLength(50);
            entity.Property(e => e.ConceptoUltimoMovimiento).HasMaxLength(255);
            entity.Property(e => e.CostoDevolucionFlete).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Departamento).HasMaxLength(100);
            entity.Property(e => e.NumeroGuia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PosibleCostoDevFlete).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioFlete).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoEnvio).HasMaxLength(100);
            entity.Property(e => e.Transportadora).HasMaxLength(100);
            entity.Property(e => e.UbicacionUltimoMovimiento).HasMaxLength(255);
            entity.Property(e => e.UltimoMovimiento).HasMaxLength(255);

            entity.HasOne(d => d.IdPedidoInternoNavigation).WithMany(p => p.Logisticas)
                .HasForeignKey(d => d.IdPedidoInterno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logistica_Pedidos");
        });

        modelBuilder.Entity<Novedade>(entity =>
        {
            entity.HasKey(e => e.IdNovedad);

            entity.Property(e => e.Novedad).HasMaxLength(255);
            entity.Property(e => e.NovedadSolucionada).HasMaxLength(50);

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Novedades)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Novedades_Pedidos");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedidoInterno);

            entity.HasIndex(e => e.FechaPedido, "idx_Pedidos_Fecha");

            entity.Property(e => e.Comision).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Estado).HasMaxLength(100);
            entity.Property(e => e.IdOrdenTienda).HasMaxLength(100);
            entity.Property(e => e.Notas).HasMaxLength(255);
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.Vendedor).HasMaxLength(100);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Clientes");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdTienda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Tiendas");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.Property(e => e.Nombre).HasMaxLength(255);
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("SKU");
            entity.Property(e => e.VariacionProducto).HasMaxLength(255);
        });

        modelBuilder.Entity<Tienda>(entity =>
        {
            entity.HasKey(e => e.IdTienda);

            entity.Property(e => e.NombreTienda).HasMaxLength(50);
            entity.Property(e => e.TipoTienda)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
