using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_DrcRate : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(7)]
        public string YearMonth { get; set; }

        public int? TramId { get; set; }

        /// <summary>DRC mủ tạp (0.38-0.50)</summary>
        public decimal DrcRawLatex { get; set; }

        /// <summary>DRC tham chiếu (dùng cho truy lĩnh)</summary>
        public decimal? DrcReference { get; set; }

        /// <summary>DRC mủ serum</summary>
        public decimal? DrcSerum { get; set; }

        /// <summary>DRC mủ dây</summary>
        public decimal? DrcRope { get; set; }

        [StringLength(200)]
        public string Note { get; set; }
    }

    public class MReq_DrcRate_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public int? TramId { get; set; }
        public string YearMonth { get; set; }
    }
}
