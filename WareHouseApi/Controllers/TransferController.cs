using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;

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
        // [Authorize]
        public IActionResult GetObjectHistory(string id)
        {
            List<WarehouseTransfer> warehouseTransfer = _rKNETDBContext.WarehouseTransfer.Include(c => c.WarehouseObjects)
                                                                                         .Include(c => c.WarehouseAction)
                                                                                         .Include(c => c.WarehouseObjects.WarehouseCategories)
                                                                                         .Where(c => c.WarehouseObjects.Id == Global.ToCode(id))
                                                                                         .OrderBy(c => c.DateTime)
                                                                                         .ToList();
            return Ok(warehouseTransfer);
        }

        [HttpGet("GetobjectLocation")]
        // [Authorize]
        public IActionResult GetObjectLocation(string id)
        {
            List<WarehouseTransfer> warehouseTransfer = _rKNETDBContext.WarehouseTransfer.Include(c => c.WarehouseObjects)
                                                                                         .Include(c => c.WarehouseAction)
                                                                                         .Include(c => c.WarehouseObjects.WarehouseCategories)
                                                                                         .Where(c => c.WarehouseObjects.Id == Global.ToCode(id))
                                                                                         .OrderByDescending(c => c.Id)
                                                                                         .ToList();
            if (warehouseTransfer.Count == 0)
            {
                return BadRequest(new { message = "unavailable id" });
            }

            if (warehouseTransfer[0].LocationEnd == null)
            {
                return Ok(null);
            }

            return Ok(warehouseTransfer[0].LocationEnd);
        }


        [HttpGet("GetAllLocations")]
        // [Authorize]
        public IActionResult GetAllLocations()
        {
            List<Location> Locations = _rKNETDBContext.Locations.OrderBy(c => c.Name).ToList();
            return Ok(Locations );
        }




        [HttpPost("SetObjectHistory")]
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
                warehouseTransfer.NewOwner = item.NewOwner;
                warehouseTransfer.LocationStart = item.LocationStart;
                warehouseTransfer.LocationEnd = item.LocationEnd;
                warehouseTransfer.DateTime = item.DateTime;
                warehouseTransfer.Comment = item.Comment;
                warehouseTransfer.WarehouseAction = _rKNETDBContext.WarehouseAction.FirstOrDefault(x => x.Id == item.WarehouseAction);
                _rKNETDBContext.WarehouseTransfer.Add(warehouseTransfer);                
                if (item.NewOwner != null)
                {
                    warehouseObject.Owner = item.NewOwner;
                }
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
            public string User { get; set; }
            public string? NewOwner { get; set; }
            public Guid LocationStart { get; set; }
            public Guid LocationEnd { get; set; }
            public DateTime DateTime { get; set; }
            public string? Comment { get; set; }
            public int WarehouseAction { get; set; }

        }


    }
}
