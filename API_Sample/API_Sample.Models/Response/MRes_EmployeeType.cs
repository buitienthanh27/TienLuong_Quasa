using System;

namespace API_Sample.Models.Response;

public class MRes_EmployeeType
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string SalaryCurrency { get; set; }
    public string PaymentCurrency { get; set; }
    public bool HasInsurance { get; set; }
    public string CalculationMethod { get; set; }
    public string? Description { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
