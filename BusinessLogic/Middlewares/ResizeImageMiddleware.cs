using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogic.Options;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Middlewares
{
    public class ResizeImageMiddleware
    {
        private const string Path = @"\/(images)\/(.{1,})\.(jpg|png)";
        
        private readonly RequestDelegate _next;
        private readonly IImageService _imageService;
        private readonly string _imagesPath;
        
        public ResizeImageMiddleware(RequestDelegate next, IImageService imageService, IOptions<AppOptions> options)
        {
            _next = next;
            _imageService = imageService;
            _imagesPath = options.Value.ImagesPath.Path;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var currentPath = context.Request.Path.Value;

            int width = 0, height = 0;

            if (int.TryParse(context.Request.Query["width"], out var w))
            {
                width = w;
            }
            if (int.TryParse(context.Request.Query["height"], out var h))
            {
                height = h;
            }
            
            if (Regex.IsMatch(currentPath, Path))
            {
                byte[] image;
                
                if (width <= 0 && height <= 0)
                {
                    image = await _imageService.ReadImageAndGetBytesAsync(_imagesPath + currentPath);
                }
                else
                {
                    var newPath = await _imageService.ResizeImageAndGetPathAsync(currentPath, width, height);
                    image = await _imageService.ReadImageAndGetBytesAsync(_imagesPath + newPath);
                }
                
                if (image != null)
                {
                    await context.Response.Body.WriteAsync(image, 0, image.Length); 
                }
                else
                {
                    await _next.Invoke(context);
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}