namespace TransportManager.Data;

public class TransportDbContext : DbContext {
  public DbSet<Vehicle> Vehicles { get; set; }

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

      // Configura a chave primária
      entity.HasKey(v => v.Id);

      entity.Property(v => v.Id)
          .HasMaxLength(36)       // 36 caracteres para GUID
          .ValueGeneratedNever(); // Não gera valor automaticamente

      // Configura as propriedades
      entity.Property(v => v.Model).IsRequired().HasMaxLength(100);

      entity.Property(v => v.Year).IsRequired();

      entity.Property(v => v.LicensePlate).IsRequired().HasMaxLength(20);

      entity.Property(v => v.Capacity).IsRequired();

      entity.Property(v => v.Type).IsRequired();

      entity.Property(v => v.Status).IsRequired();
    });
  }
}
