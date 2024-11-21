using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WareHouseApi.DbContexts.RKNETDB
{
    public class Location
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int? RKCode { get; set; }
        public int? AggregatorsCode { get; set; }
        public int Actual { get; set; }
    }
}
