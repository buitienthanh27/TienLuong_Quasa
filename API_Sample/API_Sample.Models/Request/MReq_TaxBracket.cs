using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_TaxBracket : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ngưỡng thu nhập từ là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Ngưỡng từ phải >= 0")]
    public decimal FromAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Ngưỡng đến phải >= 0")]
    public decimal? ToAmount { get; set; }

    [Required(ErrorMessage = "Thuế suất là bắt buộc")]
    [Range(0, 1, ErrorMessage = "Thuế suất phải từ 0 đến 1 (VD: 0.05 = 5%)")]
    public decimal TaxRate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Số tiền cộng thêm phải >= 0")]
    public decimal AdditionalAmount { get; set; }

    [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
    public DateTime EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Thứ tự sắp xếp là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải >= 1")]
    public int SortOrder { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}

public class MReq_TaxBracket_FullParam : PagingRequestBase
{
    public DateTime? EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}
