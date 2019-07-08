namespace WebAppAdverts.Models
{
    public class SortViewModel
    {
        public SortState NameSort { get; private set; }
        public SortState RatingSort { get; private set; }
        public SortState DateSort { get; private set; }
        public SortState NumberSort { get; private set; }
        public SortState CurrentSort { get; set; }

        public SortViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            RatingSort = sortOrder == SortState.RatingAsc ? SortState.RatingDesc : SortState.RatingAsc;
            DateSort = sortOrder == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            NumberSort = sortOrder == SortState.NumberAsc ? SortState.NumberDesc : SortState.NumberAsc;
            CurrentSort = sortOrder;
        }
    }
}
