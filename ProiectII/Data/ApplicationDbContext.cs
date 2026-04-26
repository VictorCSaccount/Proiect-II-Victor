using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProiectII.Models;
using System.Reflection.Emit;

namespace ProiectII.Data
{
    // Moștenim IdentityDbContext pentru a integra sistemul de utilizatori Identity cu modelele noastre
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabelele principale ale aplicației
        public DbSet<Fox> Foxes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<EnclosurePoint> EnclosurePoints { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<SecurityLog> SecurityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apelul base.OnModelCreating este critic pentru configurarea tabelelor de Identity (Useri, Roluri)
            base.OnModelCreating(builder);
            builder.Entity<Status>().ToTable("Statuses");
            // 1. Configurarea clasei Coordinate ca Owned Type (se va vărsa în coloane în tabelele părinte)
            // Presupunem că ai schimbat Latitude și Longitude în tipul 'decimal' în clasa Coordinate
            builder.Entity<Location>().OwnsOne(l => l.Coordinate, c =>
            {
                c.Property(x => x.Latitude).HasPrecision(18, 10).HasColumnName("Latitude");
                c.Property(x => x.Longitude).HasPrecision(18, 10).HasColumnName("Longitude");
                c.Property(x => x.Altitude).HasColumnName("Altitude");
            });

            builder.Entity<EnclosurePoint>().OwnsOne(p => p.Coordinate, c =>
            {
                c.Property(x => x.Latitude).HasPrecision(18, 10).HasColumnName("Latitude");
                c.Property(x => x.Longitude).HasPrecision(18, 10).HasColumnName("Longitude");
                c.Property(x => x.Altitude).HasColumnName("Altitude");
            });

            // 2. Gestionarea relațiilor multiple către tabelul Locations (pentru a evita erorile de tip Cycle)
            // Spunem bazei de date să nu șteargă în cascadă locațiile folosite de vulpi sau țarcuri
            builder.Entity<Fox>()
                .HasOne(f => f.FirstSeenLocation)
                .WithMany()
                .HasForeignKey(f => f.FirstSeenLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Fox>()
                .HasOne(f => f.LastSeenLocation)
                .WithMany()
                .HasForeignKey(f => f.LastSeenLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enclosure>()
                .HasOne(e => e.CenterLocation)
                .WithMany()
                .HasForeignKey(e => e.CenterLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Constrângeri suplimentare pentru consistența cu ERD-ul
            builder.Entity<Enclosure>()
                .Property(e => e.ColorMaskHex)
                .HasMaxLength(7)
                .IsFixedLength();

            // 4. Indexare pentru performanță (opțional, dar recomandat pentru loguri și comentarii)
            builder.Entity<SecurityLog>().HasIndex(s => s.Timestamp);
            builder.Entity<Comment>().HasIndex(c => c.CreatedAt);
        }
    }
}