using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;
using Microsoft.AspNetCore.Authorization;

namespace WareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly RKNETDBContext _rKNETDBContext;
        public TransferController(RKNETDBContext rKNETDBContext)
        {
            _rKNETDBContext = rKNETDBContext;
        }

        [HttpGet("GetObjectHistory")]
        [Authorize]
        public IActionResult GetObjectHistory(string id)
        {
            List<WarehouseTransfer> warehouseTransfer = _rKNETDBContext.WarehouseTransfer.Include(c => c.WarehouseObjects)
                                                                                         .Include(c => c.WarehouseAction)
                                                                                         .Include(c => c.WarehouseObjects.WarehouseCategories)
                                                                                         .Include(c => c.Location)
                                                                                         .Include(c => c.Holder)
                                                                                         .Where(c => c.WarehouseObjects.Id == Global.ToCode(id))
                                                                                         .OrderBy(c => c.DateTime)
                                                                                         .ToList();
            foreach (var item in warehouseTransfer)
            {
                item.WarehouseObjects.WarehouseCategories.Img = null;
                item.WarehouseObjects.Id = null;
            }
            return Ok(warehouseTransfer);
        }

        [HttpGet("GetAllLocations")]
        [Authorize]
        public IActionResult GetAllLocations()
        {
            List<Location> Locations = _rKNETDBContext.Locations.OrderBy(c => c.Name).ToList();
            return Ok(Locations );
        }

        [HttpPost("SetObjectHistory")]
        [Authorize]
        public IActionResult SetObjectHistory([FromBody] List<ObjectHistoryJson> objectHistoryJson)
        {
            List<WarehouseTransfer> warehouseTransfers = new();
            foreach (var item in objectHistoryJson)
            {
                WarehouseObjects warehouseObject = _rKNETDBContext.WarehouseObjects.FirstOrDefault(x => x.Id == Global.ToCode(item.WarehouseObjectsId));
                if (warehouseObject == null)
                {
                    return BadRequest(new { message = "объект отсутствует в БД" });
                }
                WarehouseTransfer warehouseTransfer = new();
                warehouseTransfer.WarehouseObjects = warehouseObject;
                warehouseTransfer.User = item.User;
                warehouseTransfer.NewHolderId = item.NewHolder;
                warehouseTransfer.LocationGUID = item.Location;
                warehouseObject.LocationGUID = item.Location;
                warehouseTransfer.DateTime = item.DateTime;
                warehouseTransfer.Comment = item.Comment;
                warehouseTransfer.WarehouseAction = _rKNETDBContext.WarehouseAction.FirstOrDefault(x => x.Id == item.WarehouseAction);
                _rKNETDBContext.WarehouseTransfer.Add(warehouseTransfer);                
                warehouseObject.HolderId = item.NewHolder;

            }
            if (objectHistoryJson == null)
            {
                return BadRequest(new { message = "unavailable data" });
            }
            _rKNETDBContext.SaveChanges();
            return Ok();
        }

        public class ObjectHistoryJson
        {
            public string WarehouseObjectsId { get; set; }
            public Guid? Location { get; set; }
            public string User { get; set; }
            public int? NewHolder { get; set; }
            public DateTime DateTime { get; set; }
            public string? Comment { get; set; }
            public int WarehouseAction { get; set; }

        }


    }
}
