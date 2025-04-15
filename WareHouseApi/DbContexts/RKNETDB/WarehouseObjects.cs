using Portal.Models.MSSQL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WareHouseApi.DbContexts.RKNETDB
{
    public class WarehouseObjects
    {
        [Key]
        [Column("Id")] // Если имя столбца в БД - "Id"
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Если Id автоинкрементный
        public byte[] Id { get; set; }

        // Внешний ключ для WarehouseCategories
        [Column("WarehouseCategoriesId")] // Имя столбца в таблице WarehouseObjects
        public int WarehouseCategoriesId { get; set; }

        [ForeignKey("WarehouseCategoriesId")] // Указывает на свойство-внешний ключ
        public WarehouseCategories WarehouseCategories { get; set; }

        [Column("Actual")]
        public int Actual { get; set; }

        // Внешний ключ для WarehouseHolders (связан через столбец Holder)
        [Column("Holder")] // Имя столбца в таблице WarehouseObjects
        public int? HolderId { get; set; } // Nullable, так как Holder может быть null

        [ForeignKey("HolderId")] // Указывает на свойство-внешний ключ
        public WarehouseHolder? Holder { get; set; } // Ссылка на запись в WarehouseHolders
    }
}
