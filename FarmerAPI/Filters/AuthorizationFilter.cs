using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FarmerAPI.Filters
{
    public class AuthorizationFilter: Attribute,IAuthorizationFilter
    {
        private readonly IHttpClientFactory _clientFactory;

        public AuthorizationFilter(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionName = context?.RouteData?.Values["Action"]?.ToString();
            var controllerName = context?.RouteData?.Values["Controller"]?.ToString();
            //----取得JWT轉發是否有權限使用Action----//         
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Auth/CheckAuth/{controllerName}/{actionName}");

            string authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (authHeader != null)
            {
                request.Headers.Add("Authorization", authHeader);
            }

            var client = _clientFactory.CreateClient("systemAuth");

            var httpResponse = client.SendAsync(request).Result;

            // 若回傳成功則代表有權限
            if (!httpResponse.IsSuccessStatusCode)
            {
                FailAuthorize(context);
            }
        }

        private void FailAuthorize(AuthorizationFilterContext context)
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
    }
}
