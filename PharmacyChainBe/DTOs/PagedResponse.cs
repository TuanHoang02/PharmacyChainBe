namespace PharmacyChainBe.DTOs
{
    public class PagedResponse<T>
    {
        public T Data { get; set; } = default!;

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages => PageSize == 0
            ? 0
            : (int)Math.Ceiling((double)TotalRecords / PageSize);

        public bool HasPrevious => PageNumber > 1;

        public bool HasNext => PageNumber < TotalPages;
    }
}
