using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                                                                                         .Where(c => c.WarehouseObjects.Id == id)
                                                                                         .OrderBy(c => c.DateTime)
                                                                                         .ToList();
            List<ObjectHistoryJson> historyJson = new List<ObjectHistoryJson>();

            foreach (var item in warehouseTransfer)
            {
                ObjectHistoryJson objectHistoryJson = new ObjectHistoryJson();
                objectHistoryJson.Id = item.Id;
                objectHistoryJson.WarehouseObjects = item.WarehouseObjects;
                objectHistoryJson.User = item.User;
                objectHistoryJson.LocationStart = _rKNETDBContext.Location.FirstOrDefault(c => c.Guid == item.LocationStart)?.Name;
                objectHistoryJson.LocationEnd = _rKNETDBContext.Location.FirstOrDefault(c => c.Guid == item.LocationEnd)?.Name;
                objectHistoryJson.DateTime = item.DateTime;
                objectHistoryJson.Comment = item.Comment;
                objectHistoryJson.WarehouseAction = item.WarehouseAction;
                historyJson.Add(objectHistoryJson);
            }
            return Ok(historyJson);
        }

        [HttpGet("GetobjectLocation")]
        // [Authorize]
        public IActionResult GetObjectLocation(string id)
        {
            List<WarehouseTransfer> warehouseTransfer = _rKNETDBContext.WarehouseTransfer.Include(c => c.WarehouseObjects)
                                                                                         .Include(c => c.WarehouseAction)
                                                                                         .Include(c => c.WarehouseObjects.WarehouseCategories)
                                                                                         .Where(c => c.WarehouseObjects.Id == id)
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

        [HttpPut("SetObjectHistory")]
        public IActionResult SetObjectHistory(WarehouseTransfer warehouseTransfer)
        {
            if (warehouseTransfer == null)
            {
                return BadRequest(new { message = "unavailable data" });
            }
            _rKNETDBContext.WarehouseTransfer.Add(warehouseTransfer);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }



        public class ObjectHistoryJson
        {
            public int Id { get; set; }
            public WarehouseObjects WarehouseObjects { get; set; }
            public string User { get; set; }
            public string? LocationStart { get; set; }
            public string? LocationEnd { get; set; }
            public DateTime DateTime { get; set; }
            public string? Comment { get; set; }
            public WarehouseAction WarehouseAction { get; set; }

        }


    }
}
