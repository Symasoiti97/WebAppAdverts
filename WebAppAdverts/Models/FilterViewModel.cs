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
