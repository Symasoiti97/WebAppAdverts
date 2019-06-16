using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAdverts.Models
{
    public class CreateViewModel
    {
        public Guid AdvertId { get; set; }
        public string Content { get; set; }
        public IFormFile Image { get; set; }
    }
}
