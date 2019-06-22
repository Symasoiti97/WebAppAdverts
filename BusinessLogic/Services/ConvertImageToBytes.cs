using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BusinessLogic.Services
{
    public class ConvertImageToBytes : IConverterService<byte[], IFormFile>
    {
        public byte[] Convert(IFormFile image)
        {
            byte[] imageData = null;

            if (image != null)
            {
                using (BinaryReader binaryReader = new BinaryReader(image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)image.Length);
                }
            }

            return imageData;
        }
    }
}
