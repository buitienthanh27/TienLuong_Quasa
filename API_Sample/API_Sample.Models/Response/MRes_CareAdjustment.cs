using System;

namespace API_Sample.Models.Response;

public class MRes_CareAdjustment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeMsnv { get; set; }
    public string EmployeeName { get; set; }
    public string TramCode { get; set; }
    public string YearMonth { get; set; }
    public decimal CareDays { get; set; }
    public decimal CareAmount { get; set; }
    public string AdjustmentType { get; set; }
    public string? Reason { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalStatus { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
