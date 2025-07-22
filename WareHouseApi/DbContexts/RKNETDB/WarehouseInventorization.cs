using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WareHouseApi.DbContexts.RKNETDB
{
    [Table("WarehouseInventorization", Schema = "dbo")] // Таблица dbo.WarehouseInventorization
    public class WarehouseInventorization
    {
        // Первичный ключ (IDENTITY)
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // FK -> Locations.GUID
        [Column("Location")] // uniqueidentifier NOT NULL
        public Guid LocationGuid { get; set; }

        // Навигация к Location (класс Location должен существовать в модели)
        [ForeignKey(nameof(LocationGuid))]
        public Location Location { get; set; } = null!;

        // Дата/время инвентаризации
        [Column("Datetime")]
        public DateTime Datetime { get; set; }

        // Сотрудник, выполнявший инвентаризацию
        [Column("Person")]
        [MaxLength(100)]
        public string Person { get; set; } = string.Empty;

        // Статус (int). При желании можно замапить на enum (см. ниже).
        [Column("Status")]
        public int Status { get; set; }

        // Навигация: элементы инвентаризации
        public ICollection<WarehouseInventorizationItem> Items { get; set; } = new List<WarehouseInventorizationItem>();

        public WarehouseInventorization()
        {

        }

        public WarehouseInventorization(string person, Guid location)
        {
            LocationGuid = location;
            Person = person;
            Datetime = DateTime.Now;
            Status = 1;
        }
    }
}
