using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_CostCenter : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public decimal? AllocationRate { get; set; }

        [StringLength(30)]
        public string AccountingCode { get; set; }
    }

    public class MReq_CostCenter_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public string SearchText { get; set; }
    }
}
