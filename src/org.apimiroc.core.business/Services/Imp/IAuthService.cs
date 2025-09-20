using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IAuthService
    {
        public Task<AuthResponse> Register(RegisterRequest request);
        public Task<AuthResponse> Login(LoginRequest request);
    }
}
