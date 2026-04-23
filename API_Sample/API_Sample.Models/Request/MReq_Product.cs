using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Product : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int? Sort { get; set; }

        public double? RatioTransfer { get; set; }

        [StringLength(150)]
        public string Remark { get; set; }
    }

    public class MReq_Product_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public string SearchText { get; set; }
    }
}
