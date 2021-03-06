﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using LiveChat.Data;
using LiveChat.Services.ImageProcessing;
using LiveChat.Infrastructure.DataTransferModels.Profile;
using System.Collections.Generic;

namespace LiveChat.Services
{
    public class ProfileService : IProfileService
    {
        private LiveChatDbContext _context;
        private IImageWriter _imageWriter;

        public ProfileService(LiveChatDbContext context, IImageWriter imageWriter)
        {
            this._context = context;
            this._imageWriter = imageWriter;
        }

        public string AddProfilePicture(IFormFile file, int userId)
        {
            try
            {
                var result = _imageWriter.UploadImage(file).Result;
                var user = this._context.Users.FirstOrDefault(u => u.Id == userId);
                user.ProfilePicturePath = result;
                var a = this._context.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public GetProfileResponse GetProfile(int userId)
        {
            return this._context.Users
                .Where(u => u.Id == userId)
                .Select(u => new GetProfileResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfilePicturePath = "images/" + u.ProfilePicturePath
                })
                .FirstOrDefault();
        }

        public IEnumerable<SearchUserResponse> SearchUser(string phrase, int page, int itemsPerPage)
        {
            return this._context.Users
                .Where(u => u.Email.Contains(phrase) || u.FirstName.Contains(phrase) || u.LastName.Contains(phrase))
                .Select(u => new SearchUserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProfilePicturePath = "images/" + u.ProfilePicturePath
                })
                .Skip(page * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
        }
    }
}
