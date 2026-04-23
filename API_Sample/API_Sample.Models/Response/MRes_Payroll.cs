using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Payroll : BaseModel.History
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string YearMonth { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal PerformanceCoef { get; set; }
        public decimal WorkingDays { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Bhxh { get; set; }
        public decimal Bhyt { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal NetSalary { get; set; }
        public string PayrollStatus { get; set; }
        public DateTime? CalculatedAt { get; set; }
        public int? CalculatedBy { get; set; }

        public string EmployeeMsnv { get; set; }
        public string EmployeeName { get; set; }
        public string TramCode { get; set; }
        public string TramName { get; set; }
        public string TechnicalGrade { get; set; }
    }

    public class MRes_PayrollDetail
    {
        public int Id { get; set; }
        public int PayrollId { get; set; }
        public int Phase { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class MRes_PayrollResult
    {
        public MRes_Payroll Payroll { get; set; }
        public List<MRes_PayrollDetail> Details { get; set; }
        public List<MRes_CostAllocation> CostAllocations { get; set; }
    }

    public class MRes_CostAllocation
    {
        public int Id { get; set; }
        public int PayrollId { get; set; }
        public int CostCenterId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
    }

    public class MRes_PayrollCalculateSummary
    {
        public string YearMonth { get; set; }
        public int TotalEmployees { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public decimal TotalGrossSalary { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalAllowances { get; set; }
        public List<string> Errors { get; set; }
    }
}
