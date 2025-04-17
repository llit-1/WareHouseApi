using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WareHouseApi.DbContexts.RKNETDB
{
    public class WarehouseTransfer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public WarehouseObjects WarehouseObjects { get; set; }
        public string User { get; set; }
        [Column("Location")]
        public Guid? LocationGUID { get; set; }
        [ForeignKey("LocationGUID")]
        public Location? Location { get; set; }
        public DateTime DateTime { get; set; }
        public string? Comment { get; set; }

        [Column("NewHolder")] // Имя столбца в таблице WarehouseObjects
        public int? NewHolderId { get; set; }

        [ForeignKey("NewHolderId")] // Указывает на свойство-внешний ключ
        public WarehouseHolder? Holder { get; set; }
        public WarehouseAction WarehouseAction { get; set; }
    }
}
