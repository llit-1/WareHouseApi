using Microsoft.EntityFrameworkCore;
using Portal.Models.MSSQL;
using System.Collections.Generic;
using System.Reflection.Emit;
using WareHouseApi.DbContexts.RKNETDB;

namespace WareHouseApi.DbContexts
{
    public class RKNETDBContext : DbContext
    {
        public RKNETDBContext(DbContextOptions<RKNETDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<WarehouseCategories> WarehouseCategories { get; set; } // Иерархия склада
        public DbSet<Location> Locations { get; set; } // Локация
        public DbSet<WarehouseAction> WarehouseAction { get; set; } // Операции склада
        public DbSet<WarehouseObjects> WarehouseObjects { get; set; } // Объекты склада
        public DbSet<WarehouseTransfer> WarehouseTransfer { get; set; } // События склада


    }
}
