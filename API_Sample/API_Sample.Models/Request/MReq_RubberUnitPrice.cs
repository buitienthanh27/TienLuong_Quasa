using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_RubberUnitPrice : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Trạm là bắt buộc")]
    public int TramId { get; set; }

    [Required(ErrorMessage = "Hạng kỹ thuật là bắt buộc")]
    [StringLength(5)]
    public string Grade { get; set; }

    [Required(ErrorMessage = "Đơn giá là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải >= 0")]
    public decimal UnitPrice { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = "THB";

    public decimal? DifficultAreaBonus { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
    public DateTime EffectiveDate { get; set; }
}

public class MReq_RubberUnitPrice_FullParam : PagingRequestBase
{
    public int? TramId { get; set; }
    public string? Grade { get; set; }
    public DateTime? EffectiveDate { get; set; }
}
