using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API_Sample.Data.Entities;

/// <summary>
/// fruit-to-seed conversion rate
/// </summary>
[Table("Product")]
public partial class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("name_slug")]
    [StringLength(100)]
    public string NameSlug { get; set; }

    [Column("sort")]
    public int? Sort { get; set; }

    /// <summary>
    /// Tỉ lệ chuyển đổi Quả -&gt; Nhân
    /// </summary>
    [Column("ratio_transfer")]
    public double? RatioTransfer { get; set; }

    [Column("remark")]
    [StringLength(150)]
    public string Remark { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }
}
