namespace DatingApp.API.helpers
{

    // the queryParams that user inputs from Angular will goes here.
    // these are the options that the user can filter from the database.
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;

        public int PageSize {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value;}
        }

        public int userId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public string OrderBy { get; set; }
        public bool Likers { get; set; } = false;
        public bool Likees { get; set; } = false;

    }
}