using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Models.MSSQL;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;

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
        public IActionResult GetObgect(string id)
        {
            WarehouseObjects warehouseObjects = _rKNETDBContext.WarehouseObjects.FirstOrDefault(c => c.Id == id);
            return Ok(warehouseObjects);
        }


        [HttpPost("SetObject")]
        // [Authorize]
        public IActionResult SetObgect(WarehouseObjects warehouseObjects)
        {
            _rKNETDBContext.WarehouseObjects.Add(warehouseObjects);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }


    }
}
