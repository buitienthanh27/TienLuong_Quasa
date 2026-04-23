using API_Sample.Models.Common;

namespace API_Sample.Models.Response
{
    public class MRes_SystemParameter : BaseModel.History
    {
        public int Id { get; set; }
        public string ParamCode { get; set; }
        public string ParamName { get; set; }
        public decimal ParamValue { get; set; }
        public string DataType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Description { get; set; }
    }
}
