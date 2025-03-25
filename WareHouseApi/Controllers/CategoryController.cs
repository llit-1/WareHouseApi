using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.Models.MSSQL;
using System.Collections.Generic;
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
        public IActionResult ChildCategories(int id)
        {
            List<WarehouseCategories> warehouseCategories = _rKNETDBContext.WarehouseCategories.ToList();
            List<CategoriesHierarchy> categoriesHierarchies = new();
            foreach (var category in warehouseCategories)
            {
                if (category.Parent == id)
                {
                    categoriesHierarchies.Add(GetRecursiveChild(category.Id.Value, warehouseCategories));
                }               
            }
            return Ok(categoriesHierarchies);
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

        [HttpPatch("UpdateCategory")]
        public IActionResult UpdateCategory([FromBody] WarehouseCategories warehouseCategory)
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


        [HttpPatch("UpdateCategoryActual")]
        public IActionResult UpdateCategoryActual(int? Id)
        {
            if (Id == null)
            {
                return BadRequest(new { message = "unavailable data" });
            }
            WarehouseCategories SQLWarehouseCategory = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == Id);
            if (SQLWarehouseCategory == null)
            {
                return BadRequest(new { message = "unavailable id" });
            }
            if (SQLWarehouseCategory.Actual == 0)
            {
                SQLWarehouseCategory.Actual = 1;
            }
         else if (SQLWarehouseCategory.Actual == 1)
            {
                SQLWarehouseCategory.Actual = 0;
            }
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




        private CategoriesHierarchy GetRecursiveChild(int id, List<WarehouseCategories> warehouseCategories)
        { 
         WarehouseCategories category = warehouseCategories.FirstOrDefault(c => c.Id == id);
            CategoriesHierarchy categoriesHierarchy = new CategoriesHierarchy();
            categoriesHierarchy.Id = id;
            categoriesHierarchy.Name = category.Name;
            categoriesHierarchy.Categories = new();
            categoriesHierarchy.Actual = category.Actual;
            List<WarehouseCategories> children = warehouseCategories.Where(c => c.Parent == id).ToList();
            foreach (var item in children)
            {
                categoriesHierarchy.Categories.Add(GetRecursiveChild(item.Id.Value, warehouseCategories));
            }
            return categoriesHierarchy;
        }

        private class CategoriesHierarchy
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public List<CategoriesHierarchy> Categories { get; set; } = new();
            public int Actual { get; set; }
        }
    }
}
