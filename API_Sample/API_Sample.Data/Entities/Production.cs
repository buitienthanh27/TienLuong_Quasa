using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Bảng sản lượng mủ cao su - Ghi nhận các loại mủ theo tháng
/// Nguồn: Sheet sản lượng, MỦ DÂY, MỦ SIRUM trong Excel
/// </summary>
[Table("production")]
public partial class Production : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    // === SẢN LƯỢNG MỦ (KG) ===

    /// <summary>Mủ tạp - Mủ thô thu hoạch (kg)</summary>
    [Column("raw_latex_kg", TypeName = "decimal(10,2)")]
    public decimal RawLatexKg { get; set; }

    /// <summary>Mủ dây - Mủ theo dây chảy (kg)</summary>
    [Column("rope_latex_kg", TypeName = "decimal(10,2)")]
    public decimal RopeLatexKg { get; set; }

    /// <summary>Mủ xirum/serum - Mủ đặc biệt (kg)</summary>
    [Column("serum_kg", TypeName = "decimal(10,2)")]
    public decimal SerumKg { get; set; }

    /// <summary>Mủ truy lĩnh tháng trước (kg)</summary>
    [Column("carry_over_kg", TypeName = "decimal(10,2)")]
    public decimal CarryOverKg { get; set; }

    // === QUY KHÔ (DRY RUBBER CONTENT) ===

    /// <summary>Hệ số DRC mủ tạp tháng này (thường 38-50%)</summary>
    [Column("drc_raw", TypeName = "decimal(8,4)")]
    public decimal? DrcRaw { get; set; }

    /// <summary>Hệ số DRC mủ serum</summary>
    [Column("drc_serum", TypeName = "decimal(8,4)")]
    public decimal? DrcSerum { get; set; }

    /// <summary>Mủ quy khô = RawLatexKg × DRC</summary>
    [Column("dry_latex_kg", TypeName = "decimal(10,2)")]
    public decimal DryLatexKg { get; set; }

    /// <summary>Truy lĩnh quy khô</summary>
    [Column("carry_dry_kg", TypeName = "decimal(10,2)")]
    public decimal CarryDryKg { get; set; }

    /// <summary>Tổng kg trả lương = dry_latex + carry_dry</summary>
    [Column("total_pay_kg", TypeName = "decimal(10,2)")]
    public decimal TotalPayKg { get; set; }

    // === XẾP HẠNG KỸ THUẬT ===

    /// <summary>Hạng kỹ thuật: A, B, C, D (ảnh hưởng đến hệ số lương)</summary>
    [Column("tech_grade")]
    [StringLength(5)]
    public string? TechGrade { get; set; }

    /// <summary>Ghi chú</summary>
    [Column("note")]
    [StringLength(500)]
    public string? Note { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
