using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Portal.Models.MSSQL
{
    public class WarehouseCategories
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Parent { get; set; }
        public byte[]? Img { get; set; }
        public int Actual { get; set; }
    }
}
