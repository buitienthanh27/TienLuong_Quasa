using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("payroll_detail")]
public partial class PayrollDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("payroll_id")]
    public int PayrollId { get; set; }

    [Column("phase")]
    public int Phase { get; set; }

    [Required]
    [Column("item_code")]
    [StringLength(50)]
    public string ItemCode { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string? Description { get; set; }

    [Column("amount", TypeName = "decimal(18,4)")]
    public decimal Amount { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("PayrollId")]
    public virtual Payroll Payroll { get; set; }
}
