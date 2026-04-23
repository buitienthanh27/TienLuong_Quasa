using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("audit_log")]
public partial class AuditLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("table_name")]
    [StringLength(50)]
    public string TableName { get; set; }

    [Column("record_id")]
    public int RecordId { get; set; }

    [Required]
    [Column("action")]
    [StringLength(20)]
    public string Action { get; set; }

    [Column("old_value", TypeName = "nvarchar(max)")]
    public string? OldValue { get; set; }

    [Column("new_value", TypeName = "nvarchar(max)")]
    public string? NewValue { get; set; }

    [Column("changed_by")]
    public int ChangedBy { get; set; }

    [Column("changed_at", TypeName = "datetime")]
    public DateTime ChangedAt { get; set; }
}
