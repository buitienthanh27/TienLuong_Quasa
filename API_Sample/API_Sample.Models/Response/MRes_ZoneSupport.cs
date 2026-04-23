using System;

namespace API_Sample.Models.Response;

public class MRes_ZoneSupport
{
    public int Id { get; set; }
    public int TramId { get; set; }
    public string TramCode { get; set; }
    public string TramName { get; set; }
    public string SupportType { get; set; }
    public decimal SupportAmount { get; set; }
    public decimal? SupportRate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
