using BusinessLogic.Options;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IReCaptchaService
    {
        Task<ReCaptchaResponse> Validate(IFormCollection form);
    }
}
