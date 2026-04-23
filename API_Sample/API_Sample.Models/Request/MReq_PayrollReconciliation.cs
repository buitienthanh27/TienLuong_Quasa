using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_PayrollReconciliation : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tháng/năm là bắt buộc")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    public int? TramId { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetSalary { get; set; }
    public decimal TotalBhxh { get; set; }
    public decimal TotalBhyt { get; set; }
    public decimal TotalBhxhCompany { get; set; }
    public decimal TotalBhytCompany { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalAllowances { get; set; }

    [StringLength(20)]
    public string ReconciliationStatus { get; set; } = "DRAFT";

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class MReq_PayrollReconciliation_FullParam : PagingRequestBase
{
    public string? YearMonth { get; set; }
    public int? TramId { get; set; }
    public string? ReconciliationStatus { get; set; }
}
