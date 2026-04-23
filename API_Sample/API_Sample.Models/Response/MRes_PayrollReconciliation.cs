using System;

namespace API_Sample.Models.Response;

public class MRes_PayrollReconciliation
{
    public int Id { get; set; }
    public string YearMonth { get; set; }
    public int? TramId { get; set; }
    public string? TramCode { get; set; }
    public string? TramName { get; set; }
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
    public string ReconciliationStatus { get; set; }
    public int? BalancedBy { get; set; }
    public DateTime? BalancedAt { get; set; }
    public int? LockedBy { get; set; }
    public DateTime? LockedAt { get; set; }
    public string? Notes { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
