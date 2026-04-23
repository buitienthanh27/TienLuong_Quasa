using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Performance : BaseModel.History
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string YearMonth { get; set; }
        public string Grade { get; set; }
        public decimal? OutputKg { get; set; }
        public string Note { get; set; }

        public string EmployeeMsnv { get; set; }
        public string EmployeeName { get; set; }
        public string TramName { get; set; }
    }
}
