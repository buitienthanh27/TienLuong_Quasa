using System;

namespace API_Sample.Models.Response;

public class MRes_WorkType
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; }
    public string CalculationType { get; set; }
    public decimal? Multiplier { get; set; }
    public string? AppliesToType { get; set; }
    public string? Description { get; set; }
    public DateTime EffectiveDate { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
