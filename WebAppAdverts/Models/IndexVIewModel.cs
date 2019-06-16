using DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAdverts.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Advert> Adverts { get; set; }
        public Advert SearchAdvert { get; set; }

        public IndexViewModel()
        {
            SearchAdvert = new Advert()
            {
                User = new User()
            };
        }
    }
}
