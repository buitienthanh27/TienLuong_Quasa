using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_PayrollCalculate
    {
        [Required]
        [StringLength(7)]
        public string YearMonth { get; set; }

        public List<int> EmployeeIds { get; set; }

        public int CalculatedBy { get; set; }
    }

    public class MReq_Payroll_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public string YearMonth { get; set; }
        public int? EmployeeId { get; set; }
        public int? TramId { get; set; }
        public string PayrollStatus { get; set; }
    }
}
