using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Chính sách tính lương theo nhóm lao động
/// </summary>
[Table("payroll_policy")]
public partial class PayrollPolicy : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; }

    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Loại lao động áp dụng. NULL = áp dụng mặc định
    /// </summary>
    [Column("employee_type_id")]
    public int? EmployeeTypeId { get; set; }

    /// <summary>
    /// Trạm áp dụng. NULL = tất cả trạm
    /// </summary>
    [Column("tram_id")]
    public int? TramId { get; set; }

    /// <summary>
    /// Chức vụ áp dụng. NULL = tất cả chức vụ
    /// </summary>
    [Column("position_id")]
    public int? PositionId { get; set; }

    /// <summary>
    /// Giá trị divisor cố định (VD: 30). XOR với DivisorParamCode.
    /// </summary>
    [Column("divisor_value", TypeName = "decimal(5,2)")]
    public decimal? DivisorValue { get; set; }

    /// <summary>
    /// Tham chiếu SystemParameter (VD: "P7"). XOR với DivisorValue.
    /// </summary>
    [Column("divisor_param_code")]
    [StringLength(50)]
    public string? DivisorParamCode { get; set; }

    /// <summary>
    /// Có tính phụ cấp vào lương không
    /// </summary>
    [Column("include_allowance")]
    public bool IncludeAllowance { get; set; }

    /// <summary>
    /// Quy tắc làm tròn: ROUND_DOWN_1000, ROUND_UP_1000, ROUND_NEAREST, NONE
    /// </summary>
    [Required]
    [Column("rounding_rule")]
    [StringLength(50)]
    public string RoundingRule { get; set; } = "ROUND_DOWN_1000";

    /// <summary>
    /// Ngày hiệu lực
    /// </summary>
    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Ngày kết thúc hiệu lực. NULL = không giới hạn
    /// </summary>
    [Column("end_date", TypeName = "date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Độ ưu tiên khi có nhiều policy match. Cao hơn = ưu tiên hơn.
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 0;

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    // Navigation
    [ForeignKey("EmployeeTypeId")]
    public virtual EmployeeType? EmployeeType { get; set; }

    [ForeignKey("TramId")]
    public virtual Tram? Tram { get; set; }

    [ForeignKey("PositionId")]
    public virtual Position? Position { get; set; }
}
