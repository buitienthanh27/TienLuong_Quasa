using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Lịch sử thay đổi thông tin nhân viên (bậc kỹ thuật, trạm, chức vụ, lương...)
/// </summary>
[Table("employee_history")]
public partial class EmployeeHistory : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Nhân viên
    /// </summary>
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Loại thay đổi: GRADE_CHANGE, TRAM_CHANGE, POSITION_CHANGE, SALARY_CHANGE, DEPARTMENT_CHANGE, TYPE_CHANGE
    /// </summary>
    [Required]
    [Column("change_type")]
    [StringLength(30)]
    public string ChangeType { get; set; }

    /// <summary>
    /// Giá trị cũ (JSON hoặc plain text)
    /// </summary>
    [Column("old_value")]
    [StringLength(500)]
    public string? OldValue { get; set; }

    /// <summary>
    /// Giá trị mới (JSON hoặc plain text)
    /// </summary>
    [Column("new_value")]
    [StringLength(500)]
    public string? NewValue { get; set; }

    /// <summary>
    /// Ngày thay đổi có hiệu lực
    /// </summary>
    [Column("change_date", TypeName = "date")]
    public DateTime ChangeDate { get; set; }

    /// <summary>
    /// Người thực hiện thay đổi
    /// </summary>
    [Column("changed_by")]
    public int ChangedBy { get; set; }

    /// <summary>
    /// Lý do thay đổi
    /// </summary>
    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Số quyết định/văn bản liên quan
    /// </summary>
    [Column("decision_number")]
    [StringLength(50)]
    public string? DecisionNumber { get; set; }

    /// <summary>
    /// Ngày quyết định
    /// </summary>
    [Column("decision_date", TypeName = "date")]
    public DateTime? DecisionDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
