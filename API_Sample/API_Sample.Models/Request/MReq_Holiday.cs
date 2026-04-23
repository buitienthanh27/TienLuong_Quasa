using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_Holiday : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Năm là bắt buộc")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Ngày lễ là bắt buộc")]
    public DateTime HolidayDate { get; set; }

    [Required(ErrorMessage = "Tên ngày lễ là bắt buộc")]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    [StringLength(10)]
    public string Country { get; set; } = "BOTH";

    public bool IsPaid { get; set; } = true;

    public decimal? BonusRate { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}

public class MReq_Holiday_FullParam : PagingRequestBase
{
    public int? Year { get; set; }
    public string? Country { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsPaid { get; set; }
}
