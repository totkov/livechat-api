using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace LiveChat.Services.ImageProcessing
{
    public class ImageWriter : IImageWriter
    {
        public async Task<string> UploadImage(IFormFile file)
        {
            if (CheckIfImageFile(file))
            {
                return await WriteFile(file);
            }

            throw new InvalidOperationException();
        }
        
        private bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return WriterHelper.GetImageFormat(fileBytes) != WriterHelper.ImageFormat.unknown;
        }
        
        public async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = Guid.NewGuid().ToString() + extension;
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
                this.CreateIfNotExist(folder);
                var path = Path.Combine(folder, fileName);

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return fileName;
        }

        private void CreateIfNotExist(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
        }
    }
}
