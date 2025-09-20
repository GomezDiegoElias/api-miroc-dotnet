using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.core.shared.Sedders
{
    public static class DbSedder
    {

        public static async Task SeedRolesAndPermissions(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var entities = new[] { "User", "Client" };
                var actions = new[] { "CREATE", "READ", "UPDATE", "DELETE" };

                var permissionsDict = new Dictionary<string, Permission>();

                // Crear permisos y almacena en un diccionario para acceso rápido
                // Crea todos los permisos posibles
                foreach (var entity in entities)
                {
                    foreach (var action in actions)
                    {
                        var permName = $"{action}_{entity}".ToUpper(); // Ej: CREATE_CLIENT
                        var perm = new Permission { Name = permName };
                        context.Permissions.Add(perm); // Agrega a la base de datos
                        permissionsDict[permName] = perm;
                    }
                }

                // Crear roles y asignar permisos
                var admin = new Role
                {
                    Name = "ADMIN",
                    RolePermissions = new List<RolePermission>()
                };

                // Permisos para ADMIN (todos los permisos)
                foreach (var perm in permissionsDict.Values)
                {
                    admin.RolePermissions.Add(new RolePermission
                    {
                        Role = admin,          // FK con RoleEntity
                        Permission = perm      // FK con PermissionEntity
                    });
                }

                var presupuestista = new Role
                {
                    Name = "PRESUPUESTISTA",
                    RolePermissions = new List<RolePermission>()
                };

                // Permisos específicos para PRESUPUESTISTA
                var permisosPresupuestista = new[] { "READ_CLIENT", "CREATE_CLIENT", };
                foreach (var permName in permisosPresupuestista)
                {
                    presupuestista.RolePermissions.Add(new RolePermission
                    {
                        Role = presupuestista,
                        Permission = permissionsDict[permName]
                    });
                }

                context.Roles.AddRange(admin, presupuestista);
                await context.SaveChangesAsync();
            }
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
