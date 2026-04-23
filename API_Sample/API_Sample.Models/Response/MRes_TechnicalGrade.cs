using System;

namespace API_Sample.Models.Response;

public class MRes_TechnicalGrade
{
    public int Id { get; set; }
    public string Grade { get; set; }
    public string Name { get; set; }
    public decimal PointCoefficient { get; set; }
    public decimal? MinScore { get; set; }
    public decimal? MaxScore { get; set; }
    public string? Description { get; set; }
    public DateTime EffectiveDate { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
