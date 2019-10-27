using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FarmerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using FarmerAPI.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using Microsoft.EntityFrameworkCore;

/* 20180529 @Richard 統一在Startup.cs AddMvc中全域注入此Filter功能是為了達到要"動態"判斷，可判斷Request是否有權限。
 * 說明：
 * 因有注入AddAuthentication服務判斷jwt，而又在app.UseAuthentication()要求Request進入MVC前都要要判斷jwt，
 * 所以加上標籤[AllowAnonymous]或[Authrize]就會自動判讀可否使用該Action。
 * Action判斷jwt時，沒加標籤等同[AllowAnonymous]。若jwt偽造，[Authrize]會自動阻擋。
 * 這樣就不用每個Action都加標籤了！
 * 因注入在services.AddMvc中，而app.UseMvc()又是最後才執行。
 * 所以來此Filter時，jwt已被app.UseAuthentication()判斷過了，可直接抓jwt判讀結果再依角色做Action權限控管。
 * 因此所有Action上面都不用加標籤，統一在這Filter就判斷好了！
 */

namespace FarmerAPI.Filters
{
    public class AuthorizationFilter: Attribute,IAuthorizationFilter
    {        
        private readonly WeatherContext _context;
        private readonly IHttpContextAccessor _accessor;
      
        public AuthorizationFilter(WeatherContext dbContext, IHttpContextAccessor accessor)
        {            
            _context = dbContext;
            _accessor = accessor;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //----取得參數判斷user是否有權限使用Action----//
            List<int> userRole = new List<int>() { 0 };    //無jwt或偽造時，預設userRole = 0 (Guest)            
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            string accessAction = context.CurrentAction(); //取得Request呼叫的Action名稱
            string userAccount = context.CurrentUserId();  //從jwt抓id，無jwt或偽造時，讀出userAccount = null

            /* ----所有request都紀錄log----
                Request.Body是一個Stream，不能Position = 0
                只能讀取一次
                因此net core提供EnableRewind()倒帶功能
                開啟後讀取完只要把Positoin歸零，就可重複讀寫！  */
                //掉header bug .net core 2.2.0才會修正，小心使用
            //HttpRequest request = context.HttpContext.Request;
            //request.EnableRewind();            
            //LogRequest(request, userAccount, accessAction);    //call db to log
            //request.Body.Seek(0, SeekOrigin.Begin);

            // ----正式驗證request權限---- //
            if (authHeader != null && authHeader.StartsWith("Bearer", true, CultureInfo.CurrentCulture))
            {
                //----Extract credentials----//
                //string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                //Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                //string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                                                            
                IQueryable<int> RoleID = _context.ImemRole.AsNoTracking()
                    .Where(y => y.Account == userAccount)
                    .Select(x => x.RoleId);

                //任一角色有權限即可執行Action
                userRole.AddRange(RoleID);
                if (HasAllowedAction(userRole, accessAction) && HasUserRole(userRole, userAccount))
                {
                    //here should write a log per request!!??
                    //do nothing here!
                }
                else
                {
                    FailAuthorize(context);
                }
            }
            else //Guest & Anonymous without authorized header
            {
                //沒有帶入header，userRole預設是0，對照database Guest角色代號0
                if (HasAllowedAction(userRole, accessAction))
                {
                    //pass vertification and do nothing here
                }
                else
                {
                    FailAuthorize(context);
                }
            }
        }

        public void FailAuthorize(AuthorizationFilterContext context)
        {
            #region 其他回傳方式
            //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            //顯示Json格式"Unauthorized"
            //context.Result = new JsonResult("Unauthorized");

            //顯示400 not found
            //context.Result = new NotFoundResult();



            //預設禁止畫面：描述寫403
            //context.Result = new ForbidResult("403");

            //重新導向至特定View
            //context.Result = new ViewResult() { ViewName = "UnauthorizedAccess" };

            //報錯
            //throw new Exception("The authorization header is either empty or isn't Basic."); 
            #endregion
            //顯示 401 Unauthorized
            context.Result = new UnauthorizedResult();
        }      
              
        //檢查使用者是否有該角色
        public bool HasUserRole(List<int> userRole, string username)
        {
            return _context.ImemRole.Any(x => x.Account == username && userRole.Contains(x.RoleId));
        }

        //檢查使用者是否有權限執行該Action
        public bool HasAllowedAction(List<int> userRole, string action)
        {
            return _context.Actions.Any(x =>
                x.Name == action &&
                x.IactionRole.Any(
                    y => userRole.Contains(y.RoleId) && y.ActionId == x.Id
                )
            );
        }


        public async void LogRequest(HttpRequest Request, string Account, string Action)
        {

            byte[] buffer = new byte[Convert.ToInt32(Request.ContentLength)];
            await Request.Body.ReadAsync(buffer, 0, buffer.Length);
            string bodyAsText = Encoding.UTF8.GetString(buffer);

            //紀錄System Log
            _context.SystemLog.Add(new SystemLog
            {
                LogTime = DateTime.Now,
                Account = Account ?? "Guest",
                Action = Action,
                Route = Request.Path,
                Method = Request.Method,
                Detail = bodyAsText,
                Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
            });

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }
    }
}
