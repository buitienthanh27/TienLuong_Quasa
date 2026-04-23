using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("cost_allocation")]
public partial class CostAllocation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("payroll_id")]
    public int PayrollId { get; set; }

    [Column("cost_center_id")]
    public int CostCenterId { get; set; }

    [Column("allocated_amount", TypeName = "decimal(18,4)")]
    public decimal AllocatedAmount { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("PayrollId")]
    public virtual Payroll Payroll { get; set; }

    [ForeignKey("CostCenterId")]
    public virtual CostCenter CostCenter { get; set; }
}
