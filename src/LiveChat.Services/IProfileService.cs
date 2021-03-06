﻿using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using LiveChat.Infrastructure.DataTransferModels.Profile;

namespace LiveChat.Services
{
    public interface IProfileService
    {
        string AddProfilePicture(IFormFile file, int userId);

        GetProfileResponse GetProfile(int userId);

        IEnumerable<SearchUserResponse> SearchUser(string phrase, int page, int itemsPerPage);
    }
}
