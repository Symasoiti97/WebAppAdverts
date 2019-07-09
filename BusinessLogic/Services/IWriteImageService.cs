using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public interface IWriteImageService
    {
        Task<string> WriteImageAndGetPathAsync(IFormFile image);
    }
}