using GestionComercialFacturacion.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionComercialFacturacion.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
    public DbSet<Oportunidad> Oportunidades => Set<Oportunidad>();
    public DbSet<ActividadComercial> ActividadesComerciales => Set<ActividadComercial>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<LineaFactura> LineasFactura => Set<LineaFactura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarCliente(modelBuilder);
        ConfigurarContacto(modelBuilder);
        ConfigurarOportunidad(modelBuilder);
        ConfigurarActividadComercial(modelBuilder);
        ConfigurarFactura(modelBuilder);
        ConfigurarLineaFactura(modelBuilder);
    }

    private static void ConfigurarCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.IdentificacionFiscal)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.IdentificacionFiscal)
                .IsUnique();

            entity.Property(x => x.CorreoElectronico)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Telefono)
                .HasMaxLength(50);

            entity.Property(x => x.Direccion)
                .HasMaxLength(300);

            entity.Property(x => x.Ciudad)
                .HasMaxLength(100);

            entity.Property(x => x.CodigoPostal)
                .HasMaxLength(20);

            entity.Property(x => x.Pais)
                .HasMaxLength(100);

            entity.Property(x => x.Estado)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(x => x.FechaCreacion)
                .IsRequired();

            entity.HasMany(x => x.Contactos)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Oportunidades)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Facturas)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarContacto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.ToTable("Contactos");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.CorreoElectronico)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Telefono)
                .HasMaxLength(50);

            entity.Property(x => x.Cargo)
                .HasMaxLength(100);

            entity.HasIndex(x => new { x.ClienteId, x.EsPrincipal })
                .IsUnique()
                .HasFilter("[EsPrincipal] = 1");
        });
    }

    private static void ConfigurarOportunidad(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Oportunidad>(entity =>
        {
            entity.ToTable("Oportunidades");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Descripcion)
                .HasMaxLength(1000);

            entity.Property(x => x.MontoEstimado)
                .HasPrecision(18, 2);

            entity.Property(x => x.Estado)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(x => x.FechaCreacion)
                .IsRequired();

            entity.HasMany(x => x.Actividades)
                .WithOne(x => x.Oportunidad)
                .HasForeignKey(x => x.OportunidadId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Factura)
                .WithOne(x => x.Oportunidad)
                .HasForeignKey<Factura>(x => x.OportunidadId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarActividadComercial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActividadComercial>(entity =>
        {
            entity.ToTable("ActividadesComerciales");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Tipo)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(x => x.Descripcion)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(x => x.FechaActividad)
                .IsRequired();

            entity.Property(x => x.FechaCreacion)
                .IsRequired();
        });
    }

    private static void ConfigurarFactura(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Factura>(entity =>
        {
            entity.ToTable("Facturas");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.NumeroFactura)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.NumeroFactura)
                .IsUnique();

            entity.HasIndex(x => x.OportunidadId)
                .IsUnique()
                .HasFilter("[OportunidadId] IS NOT NULL");

            entity.Property(x => x.Subtotal)
                .HasPrecision(18, 2);

            entity.Property(x => x.PorcentajeImpuesto)
                .HasPrecision(5, 2);

            entity.Property(x => x.ValorImpuesto)
                .HasPrecision(18, 2);

            entity.Property(x => x.Total)
                .HasPrecision(18, 2);

            entity.Property(x => x.Estado)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(x => x.FechaCreacion)
                .IsRequired();

            entity.HasMany(x => x.Lineas)
                .WithOne(x => x.Factura)
                .HasForeignKey(x => x.FacturaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigurarLineaFactura(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LineaFactura>(entity =>
        {
            entity.ToTable("LineasFactura");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Descripcion)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.Cantidad)
                .HasPrecision(18, 2);

            entity.Property(x => x.PrecioUnitario)
                .HasPrecision(18, 2);

            entity.Property(x => x.PorcentajeImpuesto)
                .HasPrecision(5, 2);

            entity.Property(x => x.SubtotalLinea)
                .HasPrecision(18, 2);

            entity.Property(x => x.ValorImpuestoLinea)
                .HasPrecision(18, 2);

            entity.Property(x => x.TotalLinea)
                .HasPrecision(18, 2);
        });
    }
}