using Microsoft.EntityFrameworkCore;

namespace EquipmentRentalAPI.Models
{
    public partial class EquipmentRentalsContext : DbContext
    {
        public EquipmentRentalsContext()
        {
        }

        public EquipmentRentalsContext(DbContextOptions<EquipmentRentalsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Equipment> Equipments { get; set; }
        public virtual DbSet<Models> Models { get; set; }  // Zmiana EquipmentModel na Model
        public virtual DbSet<Rental> Rentals { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Server=localhost;Database=EquipmentRentals;Trusted_Connection=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EquipmentId).HasName("Equipments_pk");

                entity.Property(e => e.EquipmentId).ValueGeneratedOnAdd();
                entity.Property(e => e.SerialNumber).HasMaxLength(50);

                // Zmiana referencji do nowej tabeli "Models"
                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Equipment)  // Odpowiednia zmiana w relacji
                    .HasForeignKey(d => d.ModelId)  // Zmieniono z EquipmentModelId na ModelId
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Equipment_Models");
            });

            modelBuilder.Entity<Models>(entity =>  // Zmiana klasy na Model
            {
                entity.HasKey(e => e.ModelId).HasName("Models_pk"); // Zmiana z EquipmentModels_pk na Models_pk

                entity.Property(e => e.ModelId).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(e => e.RentalId).HasName("Rentals_pk");

                entity.Property(e => e.RentalId).ValueGeneratedOnAdd();
                entity.Property(e => e.RentalDate).HasColumnType("datetime");
                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.EquipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Rentals_Equipment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Rentals_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("Users_pk");

                entity.HasIndex(e => e.Email, "UniqueEmail").IsUnique();

                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
