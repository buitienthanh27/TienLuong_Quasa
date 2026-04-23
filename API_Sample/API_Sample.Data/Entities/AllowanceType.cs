using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("allowance_type")]
public partial class AllowanceType : BaseEntity
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

    [Column("default_rate", TypeName = "decimal(18,4)")]
    public decimal? DefaultRate { get; set; }

    public virtual ICollection<Allowance> Allowances { get; set; } = new List<Allowance>();
}
