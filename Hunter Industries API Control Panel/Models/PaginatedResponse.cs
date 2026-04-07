namespace HunterIndustriesAPIControlPanel.Models
{
    public class PaginatedResponse<T>
    {
        public List<T> Entries { get; set; } = new();
        public int EntryCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPageCount { get; set; }
        public int TotalCount { get; set; }
    }
}
