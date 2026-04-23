using System;

namespace API_Sample.Models.Response;

public class MRes_RubberUnitPrice
{
    public int Id { get; set; }
    public int TramId { get; set; }
    public string TramCode { get; set; }
    public string TramName { get; set; }
    public string Grade { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; }
    public decimal? DifficultAreaBonus { get; set; }
    public string? Description { get; set; }
    public DateTime EffectiveDate { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
