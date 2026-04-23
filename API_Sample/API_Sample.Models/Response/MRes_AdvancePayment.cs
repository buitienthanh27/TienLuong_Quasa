using System;

namespace API_Sample.Models.Response;

public class MRes_AdvancePayment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeMsnv { get; set; }
    public string EmployeeName { get; set; }
    public string TramCode { get; set; }
    public string YearMonth { get; set; }
    public string PaymentType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Reason { get; set; }
    public bool IsDeducted { get; set; }
    public string? DeductedInMonth { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
