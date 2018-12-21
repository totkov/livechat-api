using LiveChat.Infrastructure.DataTransferModels;

namespace LiveChat.Services
{
    public interface IAuthenticationService
    {
        RegistrationResponse Registration(RegistrationRequest requestModel);

        LogInResponse LogIn(LogInRequest requestModel, string jwtSecretKey);
    }
}
