using CommonTool;
using DapperMvc.Enum;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DapperMvc.App_Start
{
    public class PageAuthAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            string controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();

            if (CookieHelper.GetCookie("systemUserAccount", "") == "hrAdmin")
            {
                return true;
            }

            if (AuthorityHelper.GetPageAuthority((int)AuthorityType.查看, string.Format("/{0}/{1}", controller, action)))
            {
                return true;
            }

            return false;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            string returnUrl = "/Error/NoAuthority"; ;

            filterContext.HttpContext.Response.Redirect(returnUrl);

            base.HandleUnauthorizedRequest(filterContext);
        }

    }
}