namespace API_Sample.Models.Common
{
    public class PagingRequestBase
    {
        public int Page { get; set; } = 1;
        public int Record { get; set; } = 10;
        public string? SequenceStatus { get; set; }
    }
}
