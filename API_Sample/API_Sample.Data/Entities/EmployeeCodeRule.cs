using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Quy tắc mã lao động tự động phát sinh theo năm
/// </summary>
[Table("employee_code_rule")]
public partial class EmployeeCodeRule : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Năm: 2026
    /// </summary>
    [Column("year")]
    public int Year { get; set; }

    /// <summary>
    /// Prefix: "NV-2026-"
    /// </summary>
    [Required]
    [Column("prefix")]
    [StringLength(20)]
    public string Prefix { get; set; }

    /// <summary>
    /// Số chữ số: 4 → 0001
    /// </summary>
    [Column("digit_count")]
    public int DigitCount { get; set; } = 4;

    /// <summary>
    /// Số thứ tự hiện tại
    /// </summary>
    [Column("current_sequence")]
    public int CurrentSequence { get; set; } = 0;

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }
}
