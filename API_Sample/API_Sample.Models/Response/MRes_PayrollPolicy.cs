using System;

namespace API_Sample.Models.Response;

public class MRes_PayrollPolicy
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int? EmployeeTypeId { get; set; }
    public string? EmployeeTypeName { get; set; }
    public int? TramId { get; set; }
    public string? TramCode { get; set; }
    public int? PositionId { get; set; }
    public string? PositionName { get; set; }
    public decimal? DivisorValue { get; set; }
    public string? DivisorParamCode { get; set; }
    public bool IncludeAllowance { get; set; }
    public string RoundingRule { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Priority { get; set; }
    public string? Description { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
