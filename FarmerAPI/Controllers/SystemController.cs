using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FarmerAPI.Models.Weather;
using FarmerAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using FarmerAPI.Extensions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace FarmerAPI.Controllers
{
    [Route("api/[controller]")]
    public class SystemController : Controller
    {
        private IConfiguration _config;
        private readonly WeatherContext _context;
        private readonly IHttpContextAccessor _accessor;

        public SystemController(IConfiguration config, WeatherContext context, IHttpContextAccessor accessor)
        {
            _config = config;
            _context = context;
            _accessor = accessor;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReflection()
        {
            var DbExistCtrls = _context.Ctrl;
            var DbExistActions = _context.Actions;
 
            IEnumerable<Type> controllers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => typeof(ControllerBase).IsAssignableFrom(t)).Select(t => t);            

            foreach (Type controller in controllers)
            {
                int ControllerID;
                string ControllerName = controller.Name.Replace("Controller","");

                //檢查是否已有Controller登入
                if (IsControllerExists(ControllerName))
                {                    
                    //有則抓出id
                    ControllerID = DbExistCtrls.Where(x => x.Name == ControllerName).Select(x => x.Id).SingleOrDefault();
                }
                else
                {
                    ControllerID = DbExistCtrls.Max(x => x.Id) + 1;
                    Ctrl ctrl = new Ctrl()
                    {
                        Id = ControllerID,
                        Name = ControllerName
                    };

                    _context.Ctrl.Add(ctrl);
                    //先存擋
                    await _context.SaveChangesAsync();
                }                

                List<MethodInfo> actions = controller.GetMethods().Where(t => !t.IsSpecialName && t.DeclaringType.IsSubclassOf(typeof(ControllerBase)) && t.DeclaringType.FullName == controller.FullName && t.IsPublic && !t.IsStatic).ToList();

                foreach (MethodInfo action in actions)
                {
                    Attribute attribute = action.GetCustomAttributes().Where(attr => attr is IActionHttpMethodProvider).FirstOrDefault();
                    
                    string ActionName = action.Name;
                    string HttpMethod = attribute.GetType().Name.Replace("Http", "").Replace("Attribute", "");

                    //int ActionID;

                    //檢查是否已有Action登入在此Controller下
                    if (IsActionsExists(ActionName, ControllerID, HttpMethod))
                    {
                       // do nothing
                    }
                    else
                    {
                        int ActID = DbExistActions.Max(x => x.Id) + 1;
                        Actions act = new Actions()
                        {
                            Id = ActID,
                            Name = ActionName,
                            Method = HttpMethod,
                            ControllerId = ControllerID
                        };
                        _context.Actions.Add(act);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return Ok();
        }

        [HttpGet("[action]")]
        public IEnumerable<vmMenu>  GetAllowedMenu()
        {
            //從Token抓user帳號，無則null
            string Account = _accessor.CurrentUserId();

            //該帳號所有角色可進入的Menu都篩選出來
            List<int> AllowedMenuId = _context.ImenuRole
                .Where(x => x.Role.ImemRole.Any(y => y.Account == Account))
                .Select(x => x.MenuId)
                .ToList();

            //角色至少有Guest
            AllowedMenuId.Add(0);

            //撈出允許的menu資料
            IEnumerable<Menu> AuthMenu = _context.Menu.Where(x =>
                x.ImenuRole.Any(y =>
                    AllowedMenuId.Contains(y.RoleId)
                )
            );

            List<vmMenu> ReturnMenu = new List<vmMenu>();
            
            //Menu有很多棵tree，每個tree一個root
            foreach (Menu Root in AuthMenu.Where(x => x.RootMenuId == null))
            {
                //找出所有root底下的leafs
                List<vmMenu> Tree = TreeMenu(Root, AuthMenu);

                //加入回傳的tree
                ReturnMenu.Add(Tree[0]);
            };

            return ReturnMenu;
        }

        //children會有很多menu因此屬性為List<vmMenu>，所以必須回傳List<vmMenu>才可跑遞迴
        private List<vmMenu> TreeMenu(Menu Root, IEnumerable<Menu> AllMenu)
        {
            List<vmMenu> ReturnTreeMenu = new List<vmMenu>();

            vmMenu TreeRoot = new vmMenu()
            {
                Path = Root.Path,
                MenuText = Root.MenuText,
                Selector = Root.Selector,
                Component = Root.Component
            };

            if (AllMenu.Any(x => x.RootMenuId == Root.MenuId))
            {
                TreeRoot.Children = new List<vmMenu>();
                //找root及其底下leafs
                foreach (Menu item in AllMenu.Where(x => x.RootMenuId == Root.MenuId)) // && x.MenuId == Root.MenuId
                {
                    List<vmMenu> Node = TreeMenu(item, AllMenu);
                    TreeRoot.Children.Add(Node[0]);
                };                
            } 

            ReturnTreeMenu.Add(TreeRoot);

            return ReturnTreeMenu;
        }

        private bool IsActionsExists(string Name, int CtrlID, string HttpMethod)
        {
            return _context.Actions.Any(e => e.Name == Name && e.ControllerId == CtrlID && e.Method.ToLower() == HttpMethod.ToLower());
        }

        private bool IsControllerExists(string Name)
        {
            return _context.Ctrl.Any(e => e.Name == Name);
        }
    }
}