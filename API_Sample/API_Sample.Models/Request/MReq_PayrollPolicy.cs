using System;
using System.ComponentModel.DataAnnotations;
using API_Sample.Models.Common;

namespace API_Sample.Models.Request;

public class MReq_PayrollPolicy : BaseModel.History
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã chính sách là bắt buộc")]
    [StringLength(50)]
    public string Code { get; set; }

    [Required(ErrorMessage = "Tên chính sách là bắt buộc")]
    [StringLength(200)]
    public string Name { get; set; }

    public int? EmployeeTypeId { get; set; }

    public int? TramId { get; set; }

    public int? PositionId { get; set; }

    /// <summary>
    /// Giá trị divisor cố định. XOR với DivisorParamCode.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "DivisorValue phải > 0")]
    public decimal? DivisorValue { get; set; }

    /// <summary>
    /// Tham chiếu SystemParameter. XOR với DivisorValue.
    /// </summary>
    [StringLength(50)]
    public string? DivisorParamCode { get; set; }

    public bool IncludeAllowance { get; set; }

    [Required(ErrorMessage = "Quy tắc làm tròn là bắt buộc")]
    [StringLength(50)]
    public string RoundingRule { get; set; } = "ROUND_DOWN_1000";

    [Required(ErrorMessage = "Ngày hiệu lực là bắt buộc")]
    public DateTime EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Priority phải >= 0")]
    public int Priority { get; set; } = 0;

    [StringLength(500)]
    public string? Description { get; set; }
}

public class MReq_PayrollPolicy_FullParam : PagingRequestBase
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? EmployeeTypeId { get; set; }
    public int? TramId { get; set; }
    public int? PositionId { get; set; }
    public DateTime? EffectiveDate { get; set; }
}
