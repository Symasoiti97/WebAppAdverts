using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAdverts.Models
{
    public class FilterViewModel
    {
        public string Name { get; private set; }
        public string Content { get; private set; }

        public FilterViewModel(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
