using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_ExchangeRate : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tháng/năm là bắt buộc")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    [Required(ErrorMessage = "Đơn vị tiền nguồn là bắt buộc")]
    [StringLength(10)]
    public string FromCurrency { get; set; } = "THB";

    [Required(ErrorMessage = "Đơn vị tiền đích là bắt buộc")]
    [StringLength(10)]
    public string ToCurrency { get; set; } = "LAK";

    [Required(ErrorMessage = "Tỷ giá là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Tỷ giá phải > 0")]
    public decimal Rate { get; set; }

    [StringLength(100)]
    public string? Source { get; set; } = "Vietinbank";

    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}

public class MReq_ExchangeRate_FullParam : PagingRequestBase
{
    public string? YearMonth { get; set; }
    public string? FromCurrency { get; set; }
    public string? ToCurrency { get; set; }
}
