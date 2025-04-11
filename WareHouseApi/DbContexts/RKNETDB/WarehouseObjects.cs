using Portal.Models.MSSQL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WareHouseApi.DbContexts.RKNETDB
{
    public class WarehouseObjects
    {
        [Key]
        public byte[] Id { get; set; }
        public WarehouseCategories WarehouseCategories { get; set; }
        public int Actual { get; set; }
        public string? Owner { get; set; }
    }
}
