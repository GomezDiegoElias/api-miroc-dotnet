using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.entities.Entities;
using System.Data;

namespace org.apimiroc.core.data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Construction> Constructions { get; set; }

        // metodos de paginaciones

        // Configuracion del modelo de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email) // Indice unico en el campo Email
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Dni) // Indice unico en el campo Dni
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>(); // Almacenar Status como string

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.Status != Status.DELETED); // Filtro global para soft delete

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .IsRequired();

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .IsRequired();

            // CLIENT
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Dni) // Indice unico en el campo Dni
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasQueryFilter(c => !c.IsDeleted); // Filtro global para soft delete

            // EMPLOYEE
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Dni)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted); // Filtro global para soft delete

            // PROVIDER
            modelBuilder.Entity<Provider>()
                .HasIndex(q => q.Cuit)
                .IsUnique();
            
            modelBuilder.Entity<Provider>()
                .HasQueryFilter(q => !q.IsDeleted); // Filtro global para soft delete

            // CONSTRUCTION
            modelBuilder.Entity<Construction>()
                .HasIndex(c => c.Name)
                .IsUnique();
            modelBuilder.Entity<Construction>()
                .HasQueryFilter(c => !c.IsDeleted); // Filtro global para soft delete
        }

    }
}
