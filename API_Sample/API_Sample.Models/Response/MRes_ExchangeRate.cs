using System;

namespace API_Sample.Models.Response;

public class MRes_ExchangeRate
{
    public int Id { get; set; }
    public string YearMonth { get; set; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
    public decimal Rate { get; set; }
    public string? Source { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Description { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}
