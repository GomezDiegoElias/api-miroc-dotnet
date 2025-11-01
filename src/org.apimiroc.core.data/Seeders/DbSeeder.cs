using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.core.data.Sedders
{
    public static class DbSedder
    {

        public static async Task SeedRolesAndPermissions(AppDbContext context)
        {
            // Crear roles si no existen
            var admin = await context.Roles.FirstOrDefaultAsync(r => r.Name == "ADMIN");
            if (admin == null)
            {
                admin = new Role { Name = "ADMIN", RolePermissions = new List<RolePermission>() };
                context.Roles.Add(admin);
            }

            var presupuestista = await context.Roles.FirstOrDefaultAsync(r => r.Name == "PRESUPUESTISTA");
            if (presupuestista == null)
            {
                presupuestista = new Role { Name = "PRESUPUESTISTA", RolePermissions = new List<RolePermission>() };
                context.Roles.Add(presupuestista);
            }

            await context.SaveChangesAsync();

            // Definir entidades y acciones
            var entities = new[] { "User", "Client", "Employee" };
            var actions = new[] { "CREATE", "READ", "UPDATE", "DELETE" };

            foreach (var entity in entities)
            {
                foreach (var action in actions)
                {
                    var permName = $"{action}_{entity}".ToUpper();

                    // Crear permiso si no existe
                    var perm = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                    if (perm == null)
                    {
                        perm = new Permission { Name = permName };
                        context.Permissions.Add(perm);
                        await context.SaveChangesAsync();
                    }

                    // Asignar al rol ADMIN si no lo tiene
                    if (!context.RolePermissions.Any(rp => rp.RoleId == admin.Id && rp.PermissionId == perm.Id))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = admin.Id,
                            PermissionId = perm.Id
                        });
                    }

                    // Asignar algunos al PRESUPUESTISTA
                    var permisosPresupuestista = new[] { "READ_CLIENT", "CREATE_CLIENT" };
                    if (permisosPresupuestista.Contains(permName)
                        && !context.RolePermissions.Any(rp => rp.RoleId == presupuestista.Id && rp.PermissionId == perm.Id))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = presupuestista.Id,
                            PermissionId = perm.Id
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }



        public static async Task SeedUserWhitDiferentRoles(AppDbContext context)
        {

            if (!context.Users.Any())
            {

                var rolePresupuestista = await context.Roles.FirstOrDefaultAsync(x => x.Name == "PRESUPUESTISTA");
                if (rolePresupuestista == null)
                    throw new RoleNotFoundException("PRESUPUESTISTA");

                var roleAdmin = await context.Roles.FirstOrDefaultAsync(x => x.Name == "ADMIN");
                if (roleAdmin == null)
                    throw new RoleNotFoundException("ADMIN");

                // ---- USUARIO 1 ----
                string passwordDiego = "123456";
                string saltDiego = PasswordUtils.GenerateRandomSalt();
                string hashedPasswordDiego = PasswordUtils.HashPasswordWithSalt(passwordDiego, saltDiego);

                var Diego = new User
                {
                    Id = User.GenerateId(),
                    Dni = 46924236,
                    Email = "micorreodiego@gmail.com",
                    FirstName = "Diego",
                    LastName = "Gomez",
                    Role = roleAdmin,
                    RoleId = roleAdmin.Id,
                    Status = Status.ACTIVE,
                    Hash = hashedPasswordDiego,
                    Salt = saltDiego
                };

                // ---- USUARIO 2 ----
                string passwordHugo = "123456";
                string saltHugo = PasswordUtils.GenerateRandomSalt();
                string hashedPasswordHugo = PasswordUtils.HashPasswordWithSalt(passwordHugo, saltHugo);

                var Hugo = new User
                {
                    Id = User.GenerateId(),
                    Dni = 12345678,
                    Email = "micorreohugo@gmail.com",
                    FirstName = "Hugo",
                    LastName = "Brocal",
                    Role = rolePresupuestista,
                    RoleId = rolePresupuestista.Id,
                    Status = Status.ACTIVE,
                    Hash = hashedPasswordHugo,
                    Salt = saltHugo
                };

                // ---- USUARIO 3 ----
                string passwordJoel = "123456";
                string saltJoel = PasswordUtils.GenerateRandomSalt();
                string hashedPasswordJoel = PasswordUtils.HashPasswordWithSalt(passwordJoel, saltJoel);

                var Joel = new User
                {
                    Id = User.GenerateId(),
                    Dni = 87654321,
                    Email = "micorreojoel@gmail.com",
                    FirstName = "Joel",
                    LastName = "Trolson",
                    Role = rolePresupuestista,
                    RoleId = rolePresupuestista.Id,
                    Status = Status.ACTIVE,
                    Hash = hashedPasswordJoel,
                    Salt = saltJoel
                };

                // ---- USUARIO 4 ----
                string passwordMatias = "123456";
                string saltMatias = PasswordUtils.GenerateRandomSalt();
                string hashedPasswordMatias = PasswordUtils.HashPasswordWithSalt(passwordMatias, saltMatias);

                var Matias = new User
                {
                    Id = User.GenerateId(),
                    Dni = 13548654,
                    Email = "micorreomatias@gmail.com",
                    FirstName = "Matias",
                    LastName = "Geymonat",
                    Role = rolePresupuestista,
                    RoleId = rolePresupuestista.Id,
                    Status = Status.ACTIVE,
                    Hash = hashedPasswordMatias,
                    Salt = saltMatias
                };

                context.Users.AddRange(Diego, Hugo, Joel, Matias);
                await context.SaveChangesAsync();

            }

        }

    }
}
