using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Sample.Data.Entities;

/// <summary>
/// Hệ số DRC (Dry Rubber Content) theo tháng và trạm
/// DRC là tỷ lệ quy đổi từ mủ thực tế sang mủ quy khô để tính lương
/// Nguồn: TRẠM 1!AE5-AE9, THCKT!AN7-AN13
/// </summary>
[Table("drc_rate")]
public partial class DrcRate : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }

    /// <summary>Trạm (null = áp dụng toàn đội)</summary>
    [Column("tram_id")]
    public int? TramId { get; set; }

    /// <summary>DRC mủ tạp (thường 0.38-0.50)</summary>
    [Column("drc_raw_latex", TypeName = "decimal(8,4)")]
    public decimal DrcRawLatex { get; set; }

    /// <summary>DRC tham chiếu (dùng cho truy lĩnh)</summary>
    [Column("drc_reference", TypeName = "decimal(8,4)")]
    public decimal? DrcReference { get; set; }

    /// <summary>DRC mủ serum</summary>
    [Column("drc_serum", TypeName = "decimal(8,4)")]
    public decimal? DrcSerum { get; set; }

    /// <summary>DRC mủ dây</summary>
    [Column("drc_rope", TypeName = "decimal(8,4)")]
    public decimal? DrcRope { get; set; }

    /// <summary>Ghi chú</summary>
    [Column("note")]
    [StringLength(200)]
    public string? Note { get; set; }

    [ForeignKey("TramId")]
    public virtual Tram? Tram { get; set; }
}
