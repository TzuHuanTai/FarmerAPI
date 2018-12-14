using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmerAPI.Models;
using FarmerAPI.ViewModels;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Menus")]
    public class MenusController : Controller
    {
        private readonly WeatherContext _context;

        public MenusController(WeatherContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<MenuNode> GetMenuTree()
        {
            IEnumerable<Menu> Menu = _context.Menu;

            List<MenuNode> ReturnMenu = new List<MenuNode>();

            //Menu有很多棵tree，每個tree一個root
            foreach (Menu Root in Menu.Where(x => x.RootMenuId == null))
            {
                //找出所有root底下的leafs
                MenuNode RootImenu = new MenuNode() { MenuId = Root.MenuId, MenuText = Root.MenuText };
                List<MenuNode> Tree = TreeMenu(RootImenu, Menu);

                //加入回傳的tree
                ReturnMenu.Add(Tree[0]);
            };

            return ReturnMenu;
        }

        //children會有很多MenuNode因此屬性為List<MenuNode>，所以必須回傳List<MenuNode>才可跑遞迴
        private List<MenuNode> TreeMenu(MenuNode Root, IEnumerable<Menu> AllMenu)
        {
            List<MenuNode> ReturnTreeMenu = new List<MenuNode>();

            MenuNode TreeRoot = new MenuNode()
            {
                MenuId = Root.MenuId,
                MenuText = Root.MenuText                
            };

            if(AllMenu.Any(x => x.RootMenuId == Root.MenuId))
            {
                TreeRoot.Children = new List<MenuNode>();
                //找root及其底下leafs
                foreach (Menu Item in AllMenu.Where(x => x.RootMenuId == Root.MenuId)) // && x.MenuId == Root.MenuId
                {
                    MenuNode ItemImenu = new MenuNode() { MenuId = Item.MenuId, MenuText = Item.MenuText };
                    List<MenuNode> Node = TreeMenu(ItemImenu, AllMenu);
                    TreeRoot.Children.Add(Node[0]);
                };
            }
           

            ReturnTreeMenu.Add(TreeRoot);

            return ReturnTreeMenu;
        }

        // GET: api/Menus
        [HttpGet]
        public IEnumerable<Menu> GetMenu()
        {
            return _context.Menu;
        }

        // GET: api/Menus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menu = await _context.Menu.SingleOrDefaultAsync(m => m.MenuId == id);

            if (menu == null)
            {
                return NotFound();
            }

            return Ok(menu);
        }

        // PUT: api/Menus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenu([FromRoute] int id, [FromBody] Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != menu.MenuId)
            {
                return BadRequest();
            }

            _context.Entry(menu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Menus
        [HttpPost]
        public async Task<IActionResult> PostMenu([FromBody] Menu menu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Menu.Add(menu);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MenuExists(menu.MenuId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMenu", new { StationId = menu.MenuId }, menu);
        }

        // DELETE: api/Menus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menu = await _context.Menu.SingleOrDefaultAsync(m => m.MenuId == id);
            if (menu == null)
            {
                return NotFound();
            }

            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();

            return Ok(menu);
        }

        private bool MenuExists(int id)
        {
            return _context.Menu.Any(e => e.MenuId == id);
        }
    }
}