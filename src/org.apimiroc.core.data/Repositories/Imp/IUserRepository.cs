using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IUserRepository
    {
        public Task<User?> FindByDni(long dni);
        public Task<User> Save(User user);
        public Task<User?> FindByEmail(string email);
    }
}
