using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace LiveChat.Services.ImageProcessing
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file);
    }
}
