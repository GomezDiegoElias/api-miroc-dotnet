using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IUserService
    {
        public Task<PaginatedResponse<User>> FindAllUsers(int pageIndex, int pageSize);
        public Task<User?> FindByDni(long dni);
        public Task<User> SaveCustomUser(UserCreateRequest request);
        public Task<User> DeletePermanent(long dni);
        public Task<User> DeleteLogic(long dni);
        public Task<User> Update(User user);
        public Task<User> UpdatePartial(User user);
    }
}
