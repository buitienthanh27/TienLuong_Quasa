using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Performance : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(7)]
        public string YearMonth { get; set; }

        [StringLength(5)]
        public string Grade { get; set; }

        public decimal? OutputKg { get; set; }

        [StringLength(500)]
        public string Note { get; set; }
    }

    public class MReq_Performance_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public int? EmployeeId { get; set; }
        public string YearMonth { get; set; }
        public string Grade { get; set; }
    }
}
