using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IUserService
    {
        public Task<User?> FindByDni(long dni);
        //public Task<User> SaveCustomUser(UserApiRequest request);
    }
}
