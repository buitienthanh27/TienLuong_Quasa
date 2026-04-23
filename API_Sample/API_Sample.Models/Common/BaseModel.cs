namespace API_Sample.Models.Common
{
    public class BaseModel
    {
        public class History
        {
            public short Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public int CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int? UpdatedBy { get; set; }
        }

        public class Image
        {
            public int? Id { get; set; }
            public int? SerialId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string RelativeUrl { get; set; }
            public string SmallUrl { get; set; }
            public string MediumUrl { get; set; }
        }

        public class Country
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string CountryCode { get; set; }
        }

        public class Province
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string ProvinceCode { get; set; }
        }

        public class District
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string DistrictCode { get; set; }
        }

        public class Ward
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string WardCode { get; set; }
        }
    }
}
