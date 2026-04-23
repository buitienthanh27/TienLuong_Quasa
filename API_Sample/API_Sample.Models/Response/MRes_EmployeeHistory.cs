using System;

namespace API_Sample.Models.Response;

public class MRes_EmployeeHistory
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeMsnv { get; set; }
    public string EmployeeName { get; set; }
    public string TramCode { get; set; }
    public string ChangeType { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangeDate { get; set; }
    public int ChangedBy { get; set; }
    public string? Reason { get; set; }
    public string? DecisionNumber { get; set; }
    public DateTime? DecisionDate { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
