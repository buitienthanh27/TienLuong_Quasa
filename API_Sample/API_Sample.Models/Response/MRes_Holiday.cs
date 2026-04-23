using System;

namespace API_Sample.Models.Response;

public class MRes_Holiday
{
    public int Id { get; set; }
    public int Year { get; set; }
    public DateTime HolidayDate { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public bool IsPaid { get; set; }
    public decimal? BonusRate { get; set; }
    public string? Description { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
