using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public class WriteImageService : IWriteImageService
    {
        private readonly IHostingEnvironment _appEnvironment;
        private const string ImagePath = "\\images\\{0}";
        
        public WriteImageService(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public async Task<string> WriteImageAndGetPathAsync(IFormFile image)
        {
            var filePath = string.Empty;
            
            if (image != null && image.Length > 0)
            {
                var fileName = Path.GetFileName(image.FileName);
                filePath = string.Format(ImagePath, image.FileName);

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }

            return filePath;
        }
    }
}