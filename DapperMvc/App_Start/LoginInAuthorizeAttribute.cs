using CommonTool;
using CommonTool.DBHelper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace DapperMvc.App_Start
{
    public class LoginInAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string account = httpContext.Request.Params["Account"];

            string userName = httpContext.Request.Params["UserName"];

            if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(userName))
            {
                CookieHelper.SetCookie("systemUserName", userName, DateTime.Now.AddDays(1));
                //普通用户身份登录
                CookieHelper.SetCookie("systemUserObjectId", "69b6ed5a-ad88-4193-844b-2746290409e0", DateTime.Now.AddDays(1));
            }

            if (!string.IsNullOrEmpty(CookieHelper.GetCookie("systemUserName", "")))
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            string returnUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;

            filterContext.HttpContext.Response.Redirect(string.Format("/Login/Index?ReturnUrl={0}", HttpUtility.UrlEncode(returnUrl)));

            base.HandleUnauthorizedRequest(filterContext);
        }

    }
}