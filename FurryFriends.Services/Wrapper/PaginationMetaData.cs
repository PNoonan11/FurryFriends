namespace FurryFriends.Services.Wrapper
{
    public class PaginationMetaData
    {
        public PaginationMetaData(int totalCount, int currentPage, int itemsPerPage)
        {
            CurrentPage = currentPage;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)itemsPerPage);
        }
        public int CurrentPage { get; set; }


        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}