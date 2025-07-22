using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;
using static WareHouseApi.Controllers.AuthorizationController;

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
            List<WarehouseInventorization> warehouseInventorization = _rKNETDBContext.WarehouseInventorization.Include(x => x.Items)
                                                                                                              .ThenInclude(item => item.WarehouseObject)
                                                                                                              .ThenInclude(WarehouseObject => WarehouseObject.WarehouseCategories).ToList();

            return Ok(warehouseInventorization);
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

            List<WarehouseObjects> warehouseObjects = _rKNETDBContext.WarehouseObjects.Where(x => x.LocationGUID == createModel.Location).ToList();
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



        public class CreateModel
        {
            public Guid Location { get; set; }
            public string Person { get; set; }

        }


    }
}
