using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_SalaryScale : BaseModel.History
    {
        public int Id { get; set; }
        public int TramId { get; set; }
        public string Grade { get; set; }
        public decimal Coefficient { get; set; }
        public decimal BaseRate { get; set; }
        public DateTime EffectiveDate { get; set; }

        public string TramCode { get; set; }
        public string TramName { get; set; }
    }
}
