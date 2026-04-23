namespace API_Sample.Models.Response
{
    public class MRes_Position
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public short Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}
