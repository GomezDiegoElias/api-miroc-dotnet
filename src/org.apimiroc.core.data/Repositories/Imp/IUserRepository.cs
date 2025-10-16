using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IUserRepository
    {
        public Task<PaginatedResponse<User>> FindAll(UserFilter filters);
        public Task<User?> FindByDni(long dni);
        public Task<User> Save(User user);
        public Task<User?> FindByEmail(string email);
        public Task<User> DeletePermanent(long dni);
        public Task<User> DeleteLogic(long dni);
        public Task<User> Update(User user, long dniOld);
        public Task<User> UpdatePartial(User user, long dniOld);
    }
}
