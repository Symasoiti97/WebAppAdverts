using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAdverts.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Specify the correct name")]
        public string Login { get; set; }
    }
}
