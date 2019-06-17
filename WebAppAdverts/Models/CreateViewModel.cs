using Microsoft.AspNetCore.Http;
using System;

namespace WebAppAdverts.Models
{
    public class CreateViewModel
    {
        public Guid AdvertId { get; set; }
        public string Content { get; set; }
        public IFormFile Image { get; set; }
    }
}
