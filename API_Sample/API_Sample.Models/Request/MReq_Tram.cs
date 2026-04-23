using API_Sample.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace API_Sample.Models.Request
{
    public class MReq_Tram : BaseModel.History
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class MReq_Tram_FullParam : PagingRequestBase
    {
        public string SequenceStatus { get; set; }
        public string SearchText { get; set; }
    }
}
