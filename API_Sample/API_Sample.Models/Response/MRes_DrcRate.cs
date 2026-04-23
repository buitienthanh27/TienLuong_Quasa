using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_DrcRate : BaseModel.History
    {
        public int Id { get; set; }
        public string YearMonth { get; set; }
        public int? TramId { get; set; }
        public decimal DrcRawLatex { get; set; }
        public decimal? DrcReference { get; set; }
        public decimal? DrcSerum { get; set; }
        public decimal? DrcRope { get; set; }
        public string Note { get; set; }

        public string TramCode { get; set; }
        public string TramName { get; set; }
    }
}
