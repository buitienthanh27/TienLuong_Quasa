using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Production : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(7)]
        public string YearMonth { get; set; }

        /// <summary>Mủ tạp - Mủ thô thu hoạch (kg)</summary>
        public decimal RawLatexKg { get; set; }

        /// <summary>Mủ dây (kg)</summary>
        public decimal RopeLatexKg { get; set; }

        /// <summary>Mủ xirum/serum (kg)</summary>
        public decimal SerumKg { get; set; }

        /// <summary>Mủ truy lĩnh tháng trước (kg)</summary>
        public decimal CarryOverKg { get; set; }

        /// <summary>Hệ số DRC mủ tạp</summary>
        public decimal? DrcRaw { get; set; }

        /// <summary>Hệ số DRC mủ serum</summary>
        public decimal? DrcSerum { get; set; }

        /// <summary>Mủ quy khô (kg)</summary>
        public decimal DryLatexKg { get; set; }

        /// <summary>Truy lĩnh quy khô (kg)</summary>
        public decimal CarryDryKg { get; set; }

        /// <summary>Tổng kg trả lương</summary>
        public decimal TotalPayKg { get; set; }

        /// <summary>Hạng kỹ thuật A/B/C/D</summary>
        [StringLength(5)]
        public string TechGrade { get; set; }

        [StringLength(500)]
        public string Note { get; set; }
    }

    public class MReq_Production_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public int? EmployeeId { get; set; }
        public int? TramId { get; set; }
        public string YearMonth { get; set; }
        public string TechGrade { get; set; }
    }
}
