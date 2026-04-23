using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_Production : BaseModel.History
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string YearMonth { get; set; }

        /// <summary>Mủ tạp (kg)</summary>
        public decimal RawLatexKg { get; set; }

        /// <summary>Mủ dây (kg)</summary>
        public decimal RopeLatexKg { get; set; }

        /// <summary>Mủ serum (kg)</summary>
        public decimal SerumKg { get; set; }

        /// <summary>Mủ truy lĩnh (kg)</summary>
        public decimal CarryOverKg { get; set; }

        /// <summary>DRC mủ tạp</summary>
        public decimal? DrcRaw { get; set; }

        /// <summary>DRC mủ serum</summary>
        public decimal? DrcSerum { get; set; }

        /// <summary>Mủ quy khô (kg)</summary>
        public decimal DryLatexKg { get; set; }

        /// <summary>Truy lĩnh quy khô (kg)</summary>
        public decimal CarryDryKg { get; set; }

        /// <summary>Tổng kg trả lương</summary>
        public decimal TotalPayKg { get; set; }

        /// <summary>Hạng kỹ thuật</summary>
        public string TechGrade { get; set; }

        public string Note { get; set; }

        public string EmployeeMsnv { get; set; }
        public string EmployeeName { get; set; }
        public int? TramId { get; set; }
        public string TramCode { get; set; }
        public string TramName { get; set; }
    }
}
