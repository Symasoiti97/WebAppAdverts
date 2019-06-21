using DataBase.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BusinessLogic.Services
{
    public class CreateService
    {
        public Advert CreateAdvert(string content, IFormFile image, Guid userId)
        {
            Advert advert = new Advert
            {
                Content = content,
                DateTime = DateTime.Now,
                Rating = 0,
                Number = 0,
                UserId = userId
            };

            if (image != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)image.Length);
                }

                advert.Image = imageData;
            }

            return advert;
        }
    }
}
