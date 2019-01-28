using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

using LiveChat.Data;
using LiveChat.Data.Models;
using LiveChat.Infrastructure.DataTransferModels;

namespace LiveChat.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private LiveChatDbContext _context;

        public AuthenticationService(LiveChatDbContext context)
        {
            this._context = context ?? throw new ArgumentException("An instance of DbContext is required to use this repository.", nameof(context));
        }

        public RegistrationResponse Registration(RegistrationRequest requestModel)
        {
            try
            {
                if (this.IsEmailAlreadyExist(requestModel.Email))
                    throw new InvalidOperationException("This email already exist in database.");

                var salt = this.CreateSalt();
                var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: requestModel.Password,
                            salt: salt,
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 10000,
                            numBytesRequested: 256 / 8));

                var user = new User
                {
                    Email = requestModel.Email,
                    Password = hashedPassword,
                    Salt = salt,
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    ProfilePicturePath = "base-avatar.jpg"
                };

                this._context.Users.Add(user);

                this._context.SaveChanges();

                return new RegistrationResponse
                {
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                return new RegistrationResponse
                {
                    UserId = -1,
                    Message = ex.Message
                };
            }
        }

        public LogInResponse LogIn(LogInRequest requestModel, string jwtSecretKey)
        {
            try
            {
                var user = this._context.Users.SingleOrDefault(u => u.Email == requestModel.Email);

                if (user == null)
                    throw new InvalidOperationException("Invalid email.");

                var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: requestModel.Password,
                        salt: user.Salt,
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8));

                if (user.Password != passwordHash)
                    throw new InvalidOperationException("Invalid password.");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return new LogInResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = tokenHandler.WriteToken(token)
                };
            }
            catch (Exception ex)
            {
                return new LogInResponse
                {
                    UserId = -1,
                    Message = ex.Message
                };
            }
        }

        private bool IsEmailAlreadyExist(string email)
        {
            if (this._context.Users.FirstOrDefault(u => u.Email == email) != null)
            {
                return true;
            }

            return false;
        }

        private byte[] CreateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}
