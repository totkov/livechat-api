using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LiveChat.Services;

namespace LiveChat.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json", "multipart/form-data")]
    [Route("api/profile/[action]")]
    public class ProfileController : BaseController
    {
        private IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            this._profileService = profileService;
        }

        [HttpPost]
        public IActionResult PostProfilePicture(IFormFile file)
        {
            var result = this._profileService.AddProfilePicture(file, this.LoggedUserId);

            if (result != null)
                return this.Ok(result);

            return this.BadRequest();
        }

        [HttpGet]
        public IActionResult GetMyProfile()
        {
            return this.Ok(this._profileService.GetProfile(this.LoggedUserId));
        }

        [HttpGet]
        public IActionResult SearchUser(string phrase, int page, int itemsPerPage)
        {
            var result = this._profileService.SearchUser(phrase, page, itemsPerPage);
            return this.Ok(result);
        }
    }
}
