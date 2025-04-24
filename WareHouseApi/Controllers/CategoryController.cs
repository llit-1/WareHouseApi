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
            List<WarehouseCategories> warehouseCategories = _rKNETDBContext.WarehouseCategories.Where(c => c.Parent == null).ToList();
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

        [HttpGet("GetLazyModel")]
        public IActionResult GetLazyModel()
        {
            LazyModel lazyModel = new LazyModel();
            lazyModel.Locations = _rKNETDBContext.Locations.ToList();
            lazyModel.Holders = _rKNETDBContext.WarehouseHolders.ToList();
            lazyModel.MainCategories = _rKNETDBContext.WarehouseCategories.Where(x => x.Parent == null).ToList();
            foreach (var item in lazyModel.MainCategories)
            {
                item.Img = null;
            }
            return Ok(lazyModel);
        }


        [HttpGet("GetHardModel")]
        public IActionResult GetHardModel(int? cathegory, int? holder, string? location)
        {




            Guid locationGuid = new();
            if (location != null)
            {
                locationGuid = Guid.Parse(location);
            }
            List<WarehouseObjects> warehouseObjects = new List<WarehouseObjects>();
            List<WarehouseCategories> warehouseCategories = new List<WarehouseCategories>();
            List<Cat> catList = new List<Cat>();
            if (cathegory != null)
            {
                WarehouseCategories warehouseCategoriesFirst = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == cathegory);
                warehouseCategories.Add(warehouseCategoriesFirst);
                List<WarehouseCategories> warehouseCategoriesSecond = _rKNETDBContext.WarehouseCategories.Where(c => c.Parent == cathegory).ToList();
                warehouseCategories.AddRange(warehouseCategoriesSecond);
                List<WarehouseCategories> warehouseCategoriesThird = new();
                foreach (var item in warehouseCategoriesSecond)
                {
                    warehouseCategories.AddRange(_rKNETDBContext.WarehouseCategories.Where(c => c.Parent == item.Id));
                }
                var catIds = warehouseCategories.Select(c => c.Id).ToHashSet();
                warehouseObjects.AddRange(_rKNETDBContext.WarehouseObjects.Include(c => c.WarehouseCategories)
                                                                          .Include(c => c.Holder)
                                                                          .Include(c => c.Location)
                                                                          .Where(c => catIds.Contains(c.WarehouseCategoriesId)));
                if (holder != null)
                {
                    warehouseObjects.RemoveAll(c => c.HolderId != holder);
                }
                if (location != null)
                {
                    warehouseObjects.RemoveAll(c => c.LocationGUID != locationGuid);
                }
            }
            else if (holder != null)
            {
                warehouseObjects = _rKNETDBContext.WarehouseObjects.Include(c => c.WarehouseCategories)
                                                                   .Include(c => c.Holder)
                                                                   .Include(c => c.Location)
                                                                   .Where(c => c.HolderId == holder).ToList();
                if (location != null)
                {
                    warehouseObjects.RemoveAll(c => c.LocationGUID != locationGuid);
                }
            }
            else if (location != null)
            {
                warehouseObjects = _rKNETDBContext.WarehouseObjects.Include(c => c.WarehouseCategories)
                                                                   .Include(c => c.Holder)
                                                                   .Include(c => c.Location)
                                                                   .Where(c => c.LocationGUID == locationGuid).ToList();
            }
            else
            {
                warehouseObjects = _rKNETDBContext.WarehouseObjects.Include(c => c.WarehouseCategories)
                                                                  .Include(c => c.Holder)
                                                                  .Include(c => c.Location).ToList();
            }


            foreach (var obj in warehouseObjects)
            {
                Item item = new Item();
                item.code = Global.FromCode(obj.Id);
                item.location = obj.Location;
                item.warehouseHolder = obj.Holder;
                WarehouseCategories first = obj.WarehouseCategories;
                if (first.Parent == null)
                {
                    item.mainCat = first;
                }
                else
                {
                    WarehouseCategories second = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == first.Parent);
                    if (second.Parent == null)
                    {
                        item.mainCat = second;
                        item.cat = first;
                    }
                    else
                    {
                        item.mainCat = _rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == second.Parent);
                        item.cat = second;
                        item.secondCat = first;
                    }
                }
                item.mainCat.Img = null;
                Cat cat = catList.FirstOrDefault(c => c.mainCat.Id == item.mainCat.Id);
                if (cat != null)
                {
                    cat.Items.Add(item);
                    continue;
                }
                cat = new Cat();
                cat.mainCat = item.mainCat;
                cat.mainCat.Img = null;
                cat.Items = new();
                cat.Items.Add(item);
                if (cathegory != null && item.mainCat.Id != cathegory)
                {
                    cat.cat = item.cat;
                    if (_rKNETDBContext.WarehouseCategories.FirstOrDefault(c => c.Id == cathegory).Parent != item.mainCat.Id)
                    {
                        cat.secondCat = item.secondCat;
                    }
                }
                if (holder != null)
                {
                    cat.warehouseHolder = item.warehouseHolder;
                }
                if (location != null)
                {
                    cat.location = item.location;
                }
                catList.Add(cat);
            }

            return Ok(catList);
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
            if (warehouseObjects != null)
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

        private class LazyModel
        {
            public List<WarehouseHolder> Holders { get; set; }
            public List<Location> Locations { get; set; }
            public List<WarehouseCategories> MainCategories { get; set; }
        }

        private class Cat
        {
            public WarehouseCategories mainCat { get; set; }
            public WarehouseCategories? cat { get; set; }
            public WarehouseCategories? secondCat { get; set; }
            public WarehouseHolder? warehouseHolder { get; set; }
            public Location? location { get; set; }
            public List<Item> Items { get; set; }
        }

        private class Item
        {
            public WarehouseCategories mainCat { get; set; }
            public WarehouseCategories? cat { get; set; }
            public WarehouseCategories? secondCat { get; set; }
            public WarehouseHolder? warehouseHolder { get; set; }
            public Location? location { get; set; }
            public string code { get; set; }
        }

        private class JsonParemetrs
        {
            public int holderId { get; set; }
            public int location { get; set; }

        }
    }
}
