using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Reservas.Entities;
using System;
using System.Collections.Generic;

namespace Reservas.DataAccess;

public partial class FondoDbContext : DbContext
{
    public FondoDbContext()
    {
    }

    public FondoDbContext(DbContextOptions<FondoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Caracteristica> Caracteristicas { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Espacio> Espacios { get; set; }

    public virtual DbSet<Festivo> Festivos { get; set; }

    public virtual DbSet<Municipio> Municipios { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Sede> Sedes { get; set; }

    public virtual DbSet<SedeImagen> SedeImagens { get; set; }

    public virtual DbSet<ServicioBienestar> ServicioBienestars { get; set; }

    public virtual DbSet<ServicioExtra> ServicioExtras { get; set; }

    public virtual DbSet<TarifaConfig> TarifaConfigs { get; set; }

    public virtual DbSet<Temporadum> Temporada { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<ResultadoDisponibilidad> ResultadosDisponibilidad { get; set; }
    public virtual DbSet<ResultadoTarifa> ResultadosTarifa { get; set; }
    public virtual DbSet<ResultadoCalculoTarifa> ResultadosCalculoTarifa { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PCAlejandro\\SQLEXPRESS;Database=FondoXYZ;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Caracteristica>(entity =>
        {
            entity.HasKey(e => e.CaracteristicaId).HasName("PK__Caracter__E5294117F7EDEDA4");

            entity.ToTable("Caracteristica");

            entity.HasIndex(e => e.Nombre, "UQ__Caracter__75E3EFCF4A90A748").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.DepartamentoId).HasName("PK__Departam__66BB0E3E7BA7C604");

            entity.ToTable("Departamento");

            entity.HasIndex(e => e.Nombre, "UQ__Departam__75E3EFCF58731AEE").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Espacio>(entity =>
        {
            entity.HasKey(e => e.EspacioId).HasName("PK__Espacio__EF75FDD9B584128B");

            entity.ToTable("Espacio");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.NumeroAlojamiento)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Descripcion)
                .HasColumnType("nvarchar(max)");

            entity.HasOne(d => d.Sede).WithMany(p => p.Espacios)
                .HasForeignKey(d => d.SedeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Espacio_Sede");

            entity.HasMany(d => d.Caracteristicas).WithMany(p => p.Espacios)
                .UsingEntity<Dictionary<string, object>>(
                    "EspacioCaracteristica",
                    r => r.HasOne<Caracteristica>().WithMany()
                        .HasForeignKey("CaracteristicaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EspacioChar_Char"),
                    l => l.HasOne<Espacio>().WithMany()
                        .HasForeignKey("EspacioId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EspacioChar_Espacio"),
                    j =>
                    {
                        j.HasKey("EspacioId", "CaracteristicaId").HasName("PK__EspacioC__812769C84900A902");
                        j.ToTable("EspacioCaracteristica");
                    });
        });

        modelBuilder.Entity<Festivo>(entity =>
        {
            entity.HasKey(e => e.Fecha).HasName("PK__Festivo__B30C8A5F4DCB88EF");

            entity.ToTable("Festivo");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasKey(e => e.MunicipioId).HasName("PK__Municipi__1EEFE54ED796094A");

            entity.ToTable("Municipio");

            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Departamento).WithMany(p => p.Municipios)
                .HasForeignKey(d => d.DepartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Municipio_Departamento");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.ReservaId).HasName("PK__Reserva__C39937634128E0D8");

            entity.ToTable("Reserva");

            entity.Property(e => e.ComprobantePagoUrl).HasMaxLength(255);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.TipoReserva)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Hospedaje");
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Espacio).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.EspacioId)
                .HasConstraintName("FK_Reserva_Espacio");

            entity.HasOne(d => d.ReservaPadre).WithMany(p => p.InverseReservaPadre)
                .HasForeignKey(d => d.ReservaPadreId)
                .HasConstraintName("FK_Reserva_ReservaPadre");

            entity.HasOne(d => d.Sede).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.SedeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_Sede");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_Usuario");
        });

        modelBuilder.Entity<Sede>(entity =>
        {
            entity.HasKey(e => e.SedeId).HasName("PK__Sede__FD76DFDBA5D4DA26");

            entity.ToTable("Sede");

            entity.Property(e => e.DescripcionCorta).HasMaxLength(500);
            entity.Property(e => e.ImagenPrincipalUrl).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.RutaMapaUrl).HasMaxLength(255);
            entity.Property(e => e.TipoSede)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion).HasMaxLength(100);

            entity.HasMany(d => d.ServicioBienestars).WithMany(p => p.Sedes)
                .UsingEntity<Dictionary<string, object>>(
                    "SedeServicio",
                    r => r.HasOne<ServicioBienestar>().WithMany()
                        .HasForeignKey("ServicioBienestarId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_SedeServicio_Servicio"),
                    l => l.HasOne<Sede>().WithMany()
                        .HasForeignKey("SedeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_SedeServicio_Sede"),
                    j =>
                    {
                        j.HasKey("SedeId", "ServicioBienestarId").HasName("PK__SedeServ__126DE9FAC066EBAE");
                        j.ToTable("SedeServicio");
                    });
        });

        modelBuilder.Entity<SedeImagen>(entity =>
        {
            entity.HasKey(e => e.SedeImagenId).HasName("PK__SedeImag__ADD8F831D12DDBE7");

            entity.ToTable("SedeImagen");

            entity.Property(e => e.ImagenUrl).HasMaxLength(255);

            entity.HasOne(d => d.Sede).WithMany(p => p.SedeImagens)
                .HasForeignKey(d => d.SedeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SedeImagen_Sede");
        });

        modelBuilder.Entity<ServicioBienestar>(entity =>
        {
            entity.HasKey(e => e.ServicioBienestarId).HasName("PK__Servicio__F1B3621F9437CEA6");

            entity.ToTable("ServicioBienestar");

            entity.HasIndex(e => e.Nombre, "UQ__Servicio__75E3EFCF58DB780E").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<ServicioExtra>(entity =>
        {
            entity.HasKey(e => e.ServicioId).HasName("PK__Servicio__D5AEECC22F83C63E");

            entity.ToTable("ServicioExtra");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Sede).WithMany(p => p.ServicioExtras)
                .HasForeignKey(d => d.SedeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServicioExtra_Sede");
        });

        modelBuilder.Entity<TarifaConfig>(entity =>
        {
            entity.HasKey(e => e.TarifaConfigId).HasName("PK__TarifaCo__9814EB8EF12BDCA3");

            entity.ToTable("TarifaConfig");

            entity.Property(e => e.PrecioAcompananteAdicional).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioEspecial).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioOrdinario).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoTemporada)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Baja");

            entity.HasOne(d => d.Espacio).WithMany(p => p.TarifaConfigs)
                .HasForeignKey(d => d.EspacioId)
                .HasConstraintName("FK_TarifaConfig_Espacio");

            entity.HasOne(d => d.Sede).WithMany(p => p.TarifaConfigs)
                .HasForeignKey(d => d.SedeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TarifaConfig_Sede");
        });

        modelBuilder.Entity<Temporadum>(entity =>
        {
            entity.HasKey(e => e.TemporadaId).HasName("PK__Temporad__0B8A4EBC48D013AF");

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuario__2B3DE7B8ADD3D704");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.DireccionEmail, "UQ__Usuario__973C82AC1F9D2C5D").IsUnique();

            entity.HasIndex(e => e.NroDocumento, "UQ__Usuario__CC62C91D85BFFCDB").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.AutorizaCelular).HasDefaultValue(true);
            entity.Property(e => e.AutorizaCorreo).HasDefaultValue(true);
            entity.Property(e => e.Barrio).HasMaxLength(100);
            entity.Property(e => e.Celular)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Clave)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.DireccionEmail).HasMaxLength(100);
            entity.Property(e => e.DireccionResidencia).HasMaxLength(150);
            entity.Property(e => e.NombreCompleto).HasMaxLength(150);
            entity.Property(e => e.NroDocumento)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PreguntaSecreta).HasMaxLength(200);
            entity.Property(e => e.RespuestaSecreta).HasMaxLength(200);
            entity.Property(e => e.TelefonoResidencia)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Municipio).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.MunicipioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Municipio");
        });

        modelBuilder.Entity<ResultadoDisponibilidad>().HasNoKey();
        modelBuilder.Entity<ResultadoTarifa>().HasNoKey();
        modelBuilder.Entity<ResultadoCalculoTarifa>().HasNoKey();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);



    // Procedimiento almacenado 1: Habitaciones disponibles en un rango de fechas
    public async Task<List<ResultadoDisponibilidad>> ObtenerHabitacionesDisponiblesAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin)
    {
        var paramSede = new SqlParameter("@SedeId", sedeId);
        var paramInicio = new SqlParameter("@FechaInicio", fechaInicio);
        var paramFin = new SqlParameter("@FechaFin", fechaFin);

        return await ResultadosDisponibilidad
            .FromSqlRaw("EXEC dbo.sp_ObtenerHabitacionesDisponibles @SedeId, @FechaInicio, @FechaFin",
                        paramSede, paramInicio, paramFin)
            .ToListAsync();
    }

    // Procedimiento almacenado 2: Habitaciones disponibles por rango de fechas Y número de personas
    public async Task<List<ResultadoDisponibilidad>> ObtenerHabitacionesDisponiblesPorCupoAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin, int numPersonas)
    {
        var paramSede = new SqlParameter("@SedeId", sedeId);
        var paramInicio = new SqlParameter("@FechaInicio", fechaInicio);
        var paramFin = new SqlParameter("@FechaFin", fechaFin);
        var paramPersonas = new SqlParameter("@NumPersonas", numPersonas);

        return await ResultadosDisponibilidad
            .FromSqlRaw("EXEC dbo.sp_ObtenerHabitacionesDisponiblesPorCupo @SedeId, @FechaInicio, @FechaFin, @NumPersonas",
                        paramSede, paramInicio, paramFin, paramPersonas)
            .ToListAsync();
    }

    // Procedimiento almacenado 3:
    public async Task<List<ResultadoTarifa>> ObtenerTarifasAsync(int sedeId, string tipoTemporada, int numPersonas, int? espacioId)
    {
        var pSede = new SqlParameter("@SedeId", sedeId);
        var pTemp = new SqlParameter("@TipoTemporada", tipoTemporada);
        var pPers = new SqlParameter("@NumPersonas", numPersonas);
        var pEsp = new SqlParameter("@EspacioId", (object?)espacioId ?? DBNull.Value);

        return await ResultadosTarifa
            .FromSqlRaw("EXEC dbo.sp_ObtenerTarifas @SedeId, @TipoTemporada, @NumPersonas, @EspacioId",
                        pSede, pTemp, pPers, pEsp)
            .ToListAsync();
    }

    // Procedimiento almacenado 4:
    public async Task<decimal> CalcularTarifaReservaAsync(
        int sedeId,
        int? espacioId,
        string tipoTemporada,
        int numHabitaciones,
        int numPersonas,
        int noches,
        string tipoReserva,
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        var pSede = new SqlParameter("@SedeId", sedeId);
        var pEsp = new SqlParameter("@EspacioId", (object?)espacioId ?? DBNull.Value);
        var pTemp = new SqlParameter("@TipoTemporada", tipoTemporada);
        var pHab = new SqlParameter("@NumHabitaciones", numHabitaciones);
        var pPers = new SqlParameter("@NumPersonas", numPersonas);
        var pNoch = new SqlParameter("@Noches", noches);
        var pTipo = new SqlParameter("@TipoReserva", tipoReserva);
        var pIni = new SqlParameter("@FechaInicio", fechaInicio.Date);
        var pFin = new SqlParameter("@FechaFin", fechaFin.Date);

        return await ResultadosCalculoTarifa
            .FromSqlRaw("EXEC dbo.sp_CalcularTarifaReserva @SedeId, @EspacioId, @TipoTemporada, @NumHabitaciones, @NumPersonas, @Noches, @TipoReserva, @FechaInicio, @FechaFin",
                        pSede, pEsp, pTemp, pHab, pPers, pNoch, pTipo, pIni, pFin)
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(r => r.ValorTotal).FirstOrDefault());
    }

}
