using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
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
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Concept> Concepts { get; set; }

        // metodos de paginaciones

        // Configuracion del modelo de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // USER
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

            // ROLE - PERMISSION - ROLEPERMISSION
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

        

            // client 1:n Construction

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Constructions)
                .WithOne(cn => cn.Client)
                .HasForeignKey(cn => cn.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // CONCEPT
            //modelBuilder.Entity<Concept>()
            //    .HasIndex(c => c.Id)
            //    .IsUnique();
            // no hara falta porque es la key primaria
            modelBuilder.Entity<Concept>()
                .HasQueryFilter(q => !q.IsDeleted); // Filtro global para soft delete

            // MOVEMENT
            modelBuilder.Entity<Movement>()
                .HasIndex(m => m.CodMovement)
                .IsUnique();

            modelBuilder.Entity<Movement>()
                .HasQueryFilter(q => !q.IsDeleted); // Filtro global para soft delete

            // Client 1:N Movement
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Movements)
                .WithOne(m => m.Client)
                .HasForeignKey(m => m.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Provider 1:N Movement
            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Movements)
                .WithOne(m => m.Provider)
                .HasForeignKey(m => m.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee 1:N Movement
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Movements)
                .WithOne(m => m.Employee)
                .HasForeignKey(m => m.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Concept 1:N Movement
            modelBuilder.Entity<Concept>()
                .HasMany(c => c.Movements)
                .WithOne(m => m.Concept)
                .HasForeignKey(m => m.ConceptId)
                .OnDelete(DeleteBehavior.Restrict);

            // Construction 1:N Movement
            modelBuilder.Entity<Construction>()
                .HasMany(c => c.Movements)
                .WithOne(m => m.Construction)
                .HasForeignKey(m => m.ConstructionId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
