using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Models.MSSQL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectsController : ControllerBase
    {
        private readonly RKNETDBContext _rKNETDBContext;
        public ObjectsController(RKNETDBContext rKNETDBContext)
        {
            _rKNETDBContext = rKNETDBContext;
        }

        [HttpGet("GetObject")]
        // [Authorize]
        public IActionResult GetObject(string id)
        {
            if (id.Length!=24)
            {
                return BadRequest(new {message = "Неверная длина кода"});
            }
            WarehouseObjects? warehouseObject = _rKNETDBContext.WarehouseObjects
                                                              .Include(c => c.WarehouseCategories)
                                                              .Include(c => c.Location)
                                                              .FirstOrDefault(c => c.Id == Global.ToCode(id));
            warehouseObject.Id = null;
            return Ok(warehouseObject);
        }

        [HttpGet("GetNextCode")]
        // [Authorize]
        public IActionResult GetNextCode()
        {
            List<WarehouseObjects> warehouseObjects = _rKNETDBContext.WarehouseObjects.ToList();
            byte[] nextCode = new byte[12];
            if (warehouseObjects.Count == 0)
            {
                return File(nextCode, "application/octet-stream");
            }
            WarehouseObjects max = GetMaxByteArray(warehouseObjects);
            nextCode = max.Id;
            for (int i = nextCode.Length-1; i >=0 ; i--)
            {
                nextCode[i]++;
                if (nextCode[i] != 0)
                {
                    break;
                }
            }
            return File(nextCode, "application/octet-stream");
        }

        [HttpPost("SetObject")]
        // [Authorize]
        public IActionResult SetObject([FromBody] WarehouseObjectsJson warehouseObjectsJson)
        {
            WarehouseCategories? warehouseCategories = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id.Equals(warehouseObjectsJson.WarehouseCategories));
            if (warehouseCategories == null)
            {
                return BadRequest(new { message = "unavailable warehouseCategory" }); 
            }
            WarehouseObjects warehouseObject = new WarehouseObjects();
            warehouseObject.WarehouseCategories = warehouseCategories;
            warehouseObject.Actual = 1;
            warehouseObject.Id = Global.ToCode(warehouseObjectsJson.Id);
            warehouseObject.LocationGUID = warehouseObjectsJson.Location;
            _rKNETDBContext.WarehouseObjects.Add(warehouseObject);
            WarehouseTransfer warehouseTransfer = new WarehouseTransfer();
            warehouseTransfer.WarehouseObjects = warehouseObject;
            warehouseTransfer.User = warehouseObjectsJson.User;
            warehouseTransfer.LocationGUID = warehouseObject.LocationGUID;
            warehouseTransfer.DateTime = DateTime.Now;
            warehouseTransfer.Comment = null;
            warehouseTransfer.WarehouseAction = _rKNETDBContext.WarehouseAction.FirstOrDefault(c => c.Id == 1);
            warehouseTransfer.NewHolderId = null;
            _rKNETDBContext.WarehouseTransfer.Add(warehouseTransfer);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }


        [HttpDelete("WriteOffObject")]
        // [Authorize]
        public IActionResult WriteOffObject(string id, string user)
        {

            WarehouseObjects warehouseObject = _rKNETDBContext.WarehouseObjects.FirstOrDefault(c => c.Id.Equals(Global.ToCode(id)));
            if (warehouseObject == null)
            {
                return BadRequest(new { message = "unavailable id" });
            }
            if (warehouseObject.Actual == 0)
            {
                return BadRequest(new { message = "объект уже списан" });
            }
            warehouseObject.LocationGUID = null;
            warehouseObject.HolderId = null;
            warehouseObject.Actual = 0;
            WarehouseTransfer warehouseTransfer = new();
            warehouseTransfer.WarehouseObjects = warehouseObject;
            warehouseTransfer.User = user;
            warehouseTransfer.DateTime = DateTime.Now;
            warehouseTransfer.WarehouseAction = _rKNETDBContext.WarehouseAction.FirstOrDefault(c => c.Id == 3);
            _rKNETDBContext.WarehouseTransfer.Add(warehouseTransfer);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }


        private WarehouseObjects GetMaxByteArray(List<WarehouseObjects> warehouseObjects)
        {
            WarehouseObjects warehouseObject = warehouseObjects[0];
            List<WarehouseObjects> forRemove = new List<WarehouseObjects>();
            for (int i = 0; i < 12; i++)
            {
                foreach (var item in warehouseObjects)
                {
                    if (item.Id[i] < warehouseObject.Id[i])
                    {
                        forRemove.Add(item);
                    }
                    if (item.Id[i] > warehouseObject.Id[i])
                    {
                        forRemove.Add(warehouseObject);
                        warehouseObject = item;
                    }
                }
                warehouseObjects.RemoveAll(u => forRemove.Contains(u));
                if (warehouseObjects.Count == 1)
                {
                    return warehouseObjects[0];
                }
            }
            return warehouseObjects[0];
        }       

    }
    public class WarehouseObjectsJson
    {
        public string Id { get; set; }
        public int WarehouseCategories { get; set; }
        public int Actual { get; set; }
        public string User { get; set; }
        public Guid Location { get; set; }

    }

   
}
