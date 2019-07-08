using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ConvertImageToBytes : IConverterService<byte[], IFormFile>
    {
        public async Task<byte[]> ConvertAsync(IFormFile image)
        {
            return await Task.Run(function: () =>
            {
                byte[] imageData = null;

                if (image != null)
                {
                    using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)image.Length);
                    }
                }

                return imageData;
            });
        }
    }
}
