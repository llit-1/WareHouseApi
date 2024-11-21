using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Models.MSSQL;
using WareHouseApi.DbContexts;
using WareHouseApi.DbContexts.RKNETDB;

namespace WareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly RKNETDBContext _rKNETDBContext;
        public CategoryController(RKNETDBContext rKNETDBContext)
        {
            _rKNETDBContext = rKNETDBContext;
        }

        [HttpGet("maincategories")]
       // [Authorize]
        public IActionResult MainCategories()
        {
            List<WarehouseCategories>  warehouseCategories = _rKNETDBContext.WarehouseCategories.Where(c => c.Parent == null).ToList();
            return Ok(warehouseCategories);
        }

        [HttpGet("allactualcategories")]
        // [Authorize]
        public IActionResult AllActualCategories()
        {
            List<WarehouseCategories> warehouseCategories = _rKNETDBContext.WarehouseCategories.Where(c => c.Actual != 0).ToList();
            return Ok(warehouseCategories);
        }

        [HttpGet("allcategories")]
        // [Authorize]
        public IActionResult AllCategories()
        {
            List<WarehouseCategories> warehouseCategories = _rKNETDBContext.WarehouseCategories.ToList();
            return Ok(warehouseCategories);
        }


        [HttpGet("childCategories")]
        // [Authorize]
        public IActionResult ChildCategories(int i)
        {
            List<WarehouseCategories> warehouseCategories = _rKNETDBContext.WarehouseCategories.ToList();
            List<CategoriesHierarchy> categoriesHierarchies = new();
            foreach (var category in warehouseCategories)
            { 
            
            }




            return Ok(warehouseCategories);



        }



        [HttpPost("SetCategory")]
        public IActionResult SetCategory([FromBody] WarehouseCategories warehouseCategory)
        {
            if (warehouseCategory == null || warehouseCategory.Id != null)
            {
                return BadRequest(new { message = "unavailable data" });
            }
            _rKNETDBContext.WarehouseCategories.Add(warehouseCategory);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }

        [HttpPatch("UpdateCategoryParent")]
        public IActionResult UpdateCategoryParent([FromBody] WarehouseCategories warehouseCategory)
        {
            if (warehouseCategory == null || warehouseCategory.Id == null)
            {
                return BadRequest(new { message = "unavailable data" });
            }
            WarehouseCategories SQLWarehouseCategory = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == warehouseCategory.Id);
            if (SQLWarehouseCategory == null)
            {
                return BadRequest(new { message = "unavailable id" });
            }
            SQLWarehouseCategory.Actual = warehouseCategory.Actual;
            SQLWarehouseCategory.Img = warehouseCategory.Img;
            SQLWarehouseCategory.Name = warehouseCategory.Name;
            SQLWarehouseCategory.Parent = warehouseCategory.Parent;
            _rKNETDBContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("DeleteCategory")]
        public IActionResult DeleteCategory(int id)
        {            
            WarehouseCategories SQLWarehouseCategory = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == id);
            if (SQLWarehouseCategory == null)
            {
                return BadRequest(new { message = "unavailable id" });
            }

            WarehouseCategories warehouseCategoriesChild = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Parent == id);
            if (warehouseCategoriesChild != null)
            {
                return BadRequest(new { message = "Category has Children" });
            }
            WarehouseObjects warehouseObjects = _rKNETDBContext.WarehouseObjects.Include(c => c.WarehouseCategories)
                                                                                .FirstOrDefault(c => c.WarehouseCategories.Id == id);
            if (warehouseObjects!= null)
            {
                return BadRequest(new { message = "Category has Objects" });
            }
            _rKNETDBContext.WarehouseCategories.Remove(SQLWarehouseCategory);
            _rKNETDBContext.SaveChanges();
            return Ok();
        }


        private class CategoriesHierarchy
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public CategoriesHierarchy? Categories { get; set; }
            public int Actual { get; set; }
        }
    }
}
