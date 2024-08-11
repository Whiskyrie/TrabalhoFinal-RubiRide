namespace TransportManager.Data;

public class TransportDbContext : DbContext {
  public DbSet<Vehicle> Vehicles { get; set; }
  public DbSet<Driver> Drivers { get; set; }
  public DbSet<Route> Routes { get; set; }

  public TransportDbContext(DbContextOptions<TransportDbContext> options)
      : base(options) {}

  protected override void
  OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    if (!optionsBuilder.IsConfigured) {
      optionsBuilder.UseSqlite("Data Source=transport.db");
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Vehicle>(entity => {
      entity.ToTable("Vehicles");

      entity.HasKey(v => v.Id);

      entity.Property(v => v.Id).HasMaxLength(36).ValueGeneratedNever();

      entity.Property(v => v.Model).IsRequired().HasMaxLength(100);
      entity.Property(v => v.Year).IsRequired();
      entity.Property(v => v.LicensePlate).IsRequired().HasMaxLength(20);
      entity.Property(v => v.Capacity).IsRequired();
      entity.Property(v => v.Type).IsRequired();
      entity.Property(v => v.Status).IsRequired();
    });

    modelBuilder.Entity<Driver>(entity => {
      entity.ToTable("Drivers");

      entity.HasKey(d => d.Id);

      entity.Property(d => d.Id).HasMaxLength(36).ValueGeneratedNever();

      entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
      entity.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(20);
      entity.Property(d => d.LicenseExpirationDate).IsRequired();
      entity.Property(d => d.Status).IsRequired().HasConversion<string>();
      entity.Property(d => d.CreatedAt).IsRequired();
      entity.Property(d => d.UpdatedAt).IsRequired();
    });

    modelBuilder.Entity<Route>(entity => {
      entity.ToTable("Routes");

      entity.HasKey(r => r.Id);

      entity.Property(r => r.Id).HasMaxLength(36).ValueGeneratedNever();

      entity.Property(r => r.StartLocation).IsRequired().HasMaxLength(100);
      entity.Property(r => r.EndLocation).IsRequired().HasMaxLength(100);
      entity.Property(r => r.Distance).IsRequired();
      entity.Property(r => r.EstimatedDuration).IsRequired();

      // Configurando as relações
      entity.HasOne(r => r.Driver)
          .WithMany()
          .HasForeignKey("DriverId")
          .IsRequired()
          .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(r => r.Vehicle)
          .WithMany()
          .HasForeignKey("VehicleId")
          .IsRequired()
          .OnDelete(DeleteBehavior.Restrict);

      entity.Property(r => r.CreatedAt).IsRequired();
      entity.Property(r => r.UpdatedAt).IsRequired();
    });
  }
}