using System.IO;
using System.Threading.Tasks;
using BusinessLogic.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BusinessLogic.Services
{
    public class ImageService : IImageService
    {
        private readonly IOptions<AppOptions> _options;
        private readonly string _imagesPath;
        
        private const string ImagePath = "/images/{0}";
        
        public ImageService(IOptions<AppOptions> options)
        {
            _options = options;
            _imagesPath = options.Value.ImagesPath.Path;
        }

        public async Task<string> WriteImageAndGetPathAsync(IFormFile image)
        {
            var imagePath = string.Empty;
                
            if (image != null && image.Length > 0)
            {
                imagePath = string.Format(ImagePath, image.FileName);
                
                using (var fileStream = new FileStream(_options.Value.ImagesPath.Path + imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }

            return imagePath;
        }

        public async Task<string> ResizeImageAndGetPathAsync(string image, int width, int height)
        {
            return await Task.Run(() =>
            {
                using (var img = Image.Load(_imagesPath + image))
                {
                    if (width <= 0)
                    {
                        width = img.Width;
                    }

                    if (height <= 0)
                    {
                        height = img.Height;
                    }
                
                    var newImage = image.Replace(".", $"_{width}_{height}.");
                    
                    if (!File.Exists(_imagesPath + newImage))
                    {
                        img.Mutate(x => x.Resize(width, height));
                        img.Save(_imagesPath + newImage);
                    }

                    return newImage;
                }
            });
        }

        public async Task<byte[]> ReadImageAndGetBytesAsync(string path)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                else
                {
                    var fileStream = File.OpenRead(path);

                    using (var ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            });
        }
    }
}