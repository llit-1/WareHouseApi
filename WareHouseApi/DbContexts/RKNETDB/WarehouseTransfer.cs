using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WareHouseApi.DbContexts.RKNETDB
{
    public class WarehouseTransfer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id  { get; set; }
        public WarehouseObjects WarehouseObjects { get; set; }
        public string User { get; set; }
        public Guid? LocationStart { get; set; }
        public Guid? LocationEnd { get; set; }
        public DateTime DateTime { get; set; }
        public string? Comment { get; set; }
        public string? NewOwner { get; set; }
        public WarehouseAction WarehouseAction { get; set; }
    }
}
