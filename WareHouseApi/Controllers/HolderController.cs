using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Models.MSSQL;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;

namespace WareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolderController : ControllerBase
    {
        private readonly RKNETDBContext _rKNETDBContext;
        public HolderController(RKNETDBContext rKNETDBContext)
        {
            _rKNETDBContext = rKNETDBContext;
        }


        [HttpGet("GetHolder")]
        // [Authorize]
        public IActionResult GetHolder(int id)
        {
            WarehouseHolder warehouseHolder = _rKNETDBContext.WarehouseHolders.FirstOrDefault(x => x.Id == id);
            return Ok(warehouseHolder);
        }

        [HttpGet("GetAllHolders")]
        // [Authorize]
        public IActionResult GetAllHolders()
        {
            List<WarehouseHolder> warehouseHolders = _rKNETDBContext.WarehouseHolders.OrderBy(x => x.Surname).ToList();
            return Ok(warehouseHolders);
        }

        [HttpPost("PostHolder")]
        // [Authorize]
        public IActionResult PostHolder([FromBody] WarehouseHolder warehouseHolder)
        {
            if (warehouseHolder == null)
            {
                return BadRequest(new { message = "warehouseHolder is null" });
            }
            _rKNETDBContext.WarehouseHolders.Add(warehouseHolder);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }



    }
}
