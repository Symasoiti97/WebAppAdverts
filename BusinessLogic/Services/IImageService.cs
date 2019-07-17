using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public interface IImageService
    {
        Task<string> WriteImageAndGetPathAsync(IFormFile image);
        Task<string> ResizeImageAndGetPathAsync(string image, int width, int height);
        Task<byte[]> ReadImageAndGetBytesAsync(string path);
    }
}