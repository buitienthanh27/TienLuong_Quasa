using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

[Table("system_parameter")]
public partial class SystemParameter : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("param_code")]
    [StringLength(50)]
    public string ParamCode { get; set; }

    [Required]
    [Column("param_name")]
    [StringLength(200)]
    public string ParamName { get; set; }

    [Column("param_value", TypeName = "decimal(18,4)")]
    public decimal ParamValue { get; set; }

    [Column("data_type")]
    [StringLength(20)]
    public string? DataType { get; set; }

    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }
}
