using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;
using static WareHouseApi.Controllers.AuthorizationController;
using Portal.Models.MSSQL;

namespace WareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventorizationController : ControllerBase
    {
        private readonly RKNETDBContext _rKNETDBContext;
        public InventorizationController(RKNETDBContext rKNETDBContext)
        {
            _rKNETDBContext = rKNETDBContext;
        }

        [Authorize]
        [HttpGet("GetInventorization")]
        public IActionResult GetInventorization()
        {
            List<WarehouseInventorization> warehouseInventorization = _rKNETDBContext.WarehouseInventorization.Include(x => x.Location).ToList();
            List<InventarizationModel> inventarizationModels = new();
            foreach (var item in warehouseInventorization)
            {
                InventarizationModel inventarizationModel = new();
                inventarizationModel.Id = item.Id;
                inventarizationModel.LocationGuid = item.LocationGuid;
                inventarizationModel.Location = item.Location.Name;
                inventarizationModel.Datetime = item.Datetime;
                inventarizationModel.Person = item.Person;
                inventarizationModel.Status = item.Status;
                inventarizationModels.Add(inventarizationModel);
            }
            List<InventarizationModel> sortedModels = inventarizationModels.OrderByDescending(x => x.Datetime).ToList();
            return Ok(sortedModels);
        }

        [Authorize]
        [HttpGet("GetInventorizationItems")]
        public IActionResult GetInventorizationItems(int Id)
        {
            List<WarehouseInventorizationItem> warehouseInventorizationItems = _rKNETDBContext.WarehouseInventorizationItem.Include(x => x.WarehouseObject)
                                                                                                                           .ThenInclude(o => o.Holder)
                                                                                                                           .Where(x => x.WarehouseInventorizationId == Id).ToList();
                                                                                                                           


            List<InventarizationItemModel> inventarizationItemModels = new();
            foreach (var item in warehouseInventorizationItems)
            {
                InventarizationItemModel inventarizationItemModel = new();
                inventarizationItemModel.Id = item.Id;
                WarehouseCategories category = _rKNETDBContext.WarehouseCategories.FirstOrDefault(x => x.Id == item.WarehouseObject.WarehouseCategoriesId);
                WarehouseCategories midCategory = _rKNETDBContext.WarehouseCategories.FirstOrDefault(x => x.Id == category.Parent);
                WarehouseCategories mainCategory = _rKNETDBContext.WarehouseCategories.FirstOrDefault(x => x.Id == midCategory.Parent);
                inventarizationItemModel.Obj = Global.FromCode(item.ObjectId);
                inventarizationItemModel.ObjectName = category.Name;
                inventarizationItemModel.ObjectCategory = mainCategory.Name;
                if (item.WarehouseObject.Holder != null)
                {
                    inventarizationItemModel.Holder = item.WarehouseObject.Holder.Surname + " " + item.WarehouseObject.Holder.Name;
                }                
                inventarizationItemModel.Detected = item.Detected;
                inventarizationItemModels.Add(inventarizationItemModel);
            }
            return Ok(inventarizationItemModels);
        }



        [Authorize]
        [HttpPost("create")]
        public IActionResult Login([FromBody] CreateModel createModel)
        {
            if (createModel == null || createModel.Location == Guid.Empty || createModel.Person == null)
            {
                return BadRequest(new { message = "Неверные данные" });
            }
            WarehouseInventorization warehouseInventorization = new WarehouseInventorization(createModel.Person, createModel.Location);
            warehouseInventorization.WarehouseCategoriesId = createModel.WarehouseCategoriesId;
            List<WarehouseObjects> warehouseObjects = new();
            if (createModel.WarehouseCategoriesId != null)
            {
                List<int?> companies = _rKNETDBContext.WarehouseCategories.Where(c => c.Parent == createModel.WarehouseCategoriesId).Select(c => c.Id).ToList();
                List<int?> objectNames = _rKNETDBContext.WarehouseCategories.Where(c => c.Parent != null && companies.Contains(c.Parent.Value)).Select(c => c.Id).ToList();
                warehouseObjects = _rKNETDBContext.WarehouseObjects.Where(x => x.LocationGUID == createModel.Location && objectNames.Contains(x.WarehouseCategoriesId)).ToList();
            }
            else
            {
                warehouseObjects = _rKNETDBContext.WarehouseObjects.Where(x => x.LocationGUID == createModel.Location).ToList();
            }            
            foreach (var item in warehouseObjects)
            {
                WarehouseInventorizationItem warehouseInventorizationItem = new WarehouseInventorizationItem();
                warehouseInventorizationItem.ObjectId = item.Id;
                warehouseInventorization.Items.Add(warehouseInventorizationItem);
            }
            _rKNETDBContext.WarehouseInventorization.Add(warehouseInventorization);
            _rKNETDBContext.SaveChanges();
            return Ok(warehouseInventorization);
        }

        [Authorize]
        [HttpGet("Detected")]
        public IActionResult Detected(int Id)
        {
           WarehouseInventorizationItem? warehouseInventorizationItem = _rKNETDBContext.WarehouseInventorizationItem.FirstOrDefault(x => x.Id == Id);
            if (warehouseInventorizationItem == null)
            {
                return BadRequest(new { message = "Объект не найден" });
            }
            warehouseInventorizationItem.Datetime = DateTime.Now;
            warehouseInventorizationItem.Detected = true;
            _rKNETDBContext.SaveChanges(true);
            return Ok();
        }


        [Authorize]
        [HttpGet("InventorizationDone")]
        public IActionResult InventorizationDone(int Id)
        {
            WarehouseInventorization? warehouseInventorization = _rKNETDBContext.WarehouseInventorization.Include(x => x.Items).FirstOrDefault(x => x.Id == Id);
            if (warehouseInventorization == null)
            {
                return BadRequest(new { message = "Объект не найден" });
            }
            foreach (var item in warehouseInventorization.Items)
            {
                if (!item.Detected)
                {
                    warehouseInventorization.Status = 3;
                    _rKNETDBContext.SaveChanges(true);
                    return Ok(3);
                }
            }
            warehouseInventorization.Status = 2;
            _rKNETDBContext.SaveChanges(true);
            return Ok(2);
        }

        [Authorize]
        [HttpGet("StandingCheck")]
        public IActionResult StandingCheck(string mark, string location)
        {
            byte[] markBit = Global.ToCode(mark);
            Guid locationGuid = Guid.Parse(location);
            StandingCheckModel standingCheckModel = new();
            WarehouseObjects? warehouseObjects = _rKNETDBContext.WarehouseObjects.Include(x => x.Holder)
                                                                                 .Include(x => x.WarehouseCategories)
                                                                                 .Include(x => x.Location)
                                                                                 .FirstOrDefault(c => c.Id == markBit);
            if (warehouseObjects == null)
            {
                return BadRequest(new { message = "Объекта нет в БД" });
            }
            WarehouseCategories company = _rKNETDBContext.WarehouseCategories.FirstOrDefault(x => x.Id == warehouseObjects.WarehouseCategories.Parent);
            WarehouseCategories category = _rKNETDBContext.WarehouseCategories.FirstOrDefault(x => x.Id == company.Parent);
            standingCheckModel.Obj = mark;
            standingCheckModel.ObjectCategory = category.Name;
            standingCheckModel.ObjectName = warehouseObjects.WarehouseCategories.Name;
            if (warehouseObjects.Holder != null)
            {
                standingCheckModel.Holder = warehouseObjects.Holder.Surname + " " + warehouseObjects.Holder.Name;
            }         
            standingCheckModel.OnPlace = (warehouseObjects.LocationGUID == locationGuid);
            standingCheckModel.Location = warehouseObjects.Location?.Name;
            return Ok(standingCheckModel);
        }

        public class CreateModel
        {
            public Guid Location { get; set; }
            public string Person { get; set; }
            public int? WarehouseCategoriesId { get; set; }

        }

        public class InventarizationModel
        {
            public int Id { get; set; }
            public Guid LocationGuid { get; set; }
            public string Location { get; set; } = string.Empty;
            public DateTime Datetime { get; set; }
            public string Person { get; set; } = string.Empty;
            public int Status { get; set; }

        }

        public class InventarizationItemModel
        {
            public int Id { get; set; }
            public string Obj { get; set; } = string.Empty ;
            public string ObjectName { get; set; } = string.Empty;
            public string ObjectCategory { get; set; } = string.Empty;
            public string? Holder { get; set; }
            public bool Detected { get; set; }

        }

        public class StandingCheckModel
        {
            public string Obj { get; set; } = string.Empty;
            public string ObjectName { get; set; } = string.Empty;
            public string ObjectCategory { get; set; } = string.Empty;
            public string? Holder { get; set; }
            public string? Location { get; set; }
            public bool OnPlace { get; set; }
        }
    }
}
