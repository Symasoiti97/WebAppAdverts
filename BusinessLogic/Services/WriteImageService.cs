using System.IO;
using System.Threading.Tasks;
using BusinessLogic.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Services
{
    public class WriteImageService : IWriteImageService
    {
        private readonly IOptions<AppOptions> _options;
        
        private const string ImagePath = "/{0}";
        
        public WriteImageService(IOptions<AppOptions> options)
        {
            _options = options;
        }

        public async Task<string> WriteImageAndGetPathAsync(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {  
                using (var fileStream = new FileStream(_options.Value.ImagesPath.Path + image.FileName, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }

            return image.FileName;
        }
    }
}