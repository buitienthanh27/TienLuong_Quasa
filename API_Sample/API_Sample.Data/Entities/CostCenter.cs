using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("cost_center")]
public partial class CostCenter : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(30)]
    public string Code { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("allocation_rate", TypeName = "decimal(5,4)")]
    public decimal? AllocationRate { get; set; }

    [Column("accounting_code")]
    [StringLength(30)]
    public string? AccountingCode { get; set; }

    public virtual ICollection<CostAllocation> CostAllocations { get; set; } = new List<CostAllocation>();
}
