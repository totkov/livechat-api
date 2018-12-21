using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using LiveChat.API.Helpers;
using LiveChat.Services;
using LiveChat.Infrastructure.DataTransferModels;

namespace LiveChat.API.Controllers
{
    [Produces("application/json")]
    [Route("api/authentication/[action]")]
    public class AuthenticationController : BaseController
    {
        private IAuthenticationService _authenticationService;
        private AppSettings _appSettings;

        public AuthenticationController(IAuthenticationService authenticationService, IOptions<AppSettings> appSettings)
        {
            this._authenticationService = authenticationService;
            this._appSettings = appSettings.Value;
        }

        [HttpPost]
        public IActionResult Registration([FromBody] RegistrationRequest model)
        {
            if (this.ModelState.IsValid)
            {
                var result = this._authenticationService.Registration(model);

                if (result.UserId != -1)
                    return this.Ok(result);
                else
                    return this.BadRequest(result);
            }

            return this.BadRequest(new RegistrationResponse { UserId = -1, Message = this.ModelState.Values.ToString() });
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] LogInRequest model)
        {
            if (this.ModelState.IsValid)
            {
                var result = this._authenticationService.LogIn(model, this._appSettings.Secret);

                if (result.UserId != -1)
                    return this.Ok(result);
                else
                    return this.BadRequest(result);
            }

            return this.BadRequest(new LogInResponse { UserId = -1, Message = this.ModelState.Values.ToString() });
        }
    }
}
