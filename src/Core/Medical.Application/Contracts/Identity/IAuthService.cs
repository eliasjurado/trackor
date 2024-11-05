using Medical.Shared.User;

namespace Medical.Application.Contracts.Identity;

public interface IAuthService
{
    Task<IResponse> Register(UserRegister request);
    Task<IResponse> Login(UserLogin request);
    Task<IResponse> RefreshToken(RefreshTokenRequest request);
}
