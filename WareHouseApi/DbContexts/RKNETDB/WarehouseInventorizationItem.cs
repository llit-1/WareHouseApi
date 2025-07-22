using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WareHouseApi.DbContexts.RKNETDB
{
    [Table("WarehouseInventorizationItem", Schema = "dbo")]
    [PrimaryKey(nameof(ObjectId), nameof(WarehouseInventorizationId))]
    public class WarehouseInventorizationItem
    {
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // FK -> WarehouseObjects.ID (binary(12))
        [Column("Object", TypeName = "binary(12)")]
        [Required]
        public byte[] ObjectId { get; set; } = Array.Empty<byte>();

        // FK -> WarehouseInventorization.Id (int)
        [Column("WarehouseInventorization")]
        [Required]
        public int WarehouseInventorizationId { get; set; }

        // Дата/время фиксации объекта
        [Column("Datetime")]
        public DateTime? Datetime { get; set; }

        // Был ли объект обнаружен (Detected bit NOT NULL)
        [Column("Detected")]
        public bool Detected { get; set; }

        // Навигации
        [ForeignKey(nameof(WarehouseInventorizationId))]
        [JsonIgnore]
        public WarehouseInventorization WarehouseInventorization { get; set; } = null!;

        [ForeignKey(nameof(ObjectId))]
        public WarehouseObjects WarehouseObject { get; set; } = null!;
    }

}
