using DataBase.Models;
using System.Collections.Generic;

namespace WebAppAdverts.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Advert> Adverts { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
