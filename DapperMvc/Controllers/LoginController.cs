using BaseEnvironment;
using CommonTool;
using CommonTool.DBHelper;
using CommonTool.Workflow;
using Domain;
using IService;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;

namespace DapperMvc.Controllers
{
    public class LoginController : BasePageController
    {
        public LoginController()
        {

        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login(string Account, string Password)
        {
            AjaxReturnData result = new AjaxReturnData();
            //登录
            result.States = false;
            //登录
            List<SystemUser> list = DapperHelper.CreateInstance().SimpleQuery<SystemUser>(@"select * from SystemUser where Account='" + Account + "'");
            if (list.Count > 0)
            {
                if (list[0].Password == Password)
                {
                    result.States = true;
                    CookieHelper.SetCookie("systemUserName", list[0].UserName, DateTime.Now.AddDays(1));
                    CookieHelper.SetCookie("systemUserAccount", list[0].Account, DateTime.Now.AddDays(1));
                    CookieHelper.SetCookie("systemUserObjectId", list[0].ObjectId, DateTime.Now.AddDays(1));
                    result.Data = list[0];
                }
            }

            if (!result.States)
            {
                result.Message = "账号密码不正确.";

            }
            //else
            //{
            //    Response.Redirect(HttpUtility.UrlDecode(Request.Params["ReturnUrl"]));
            //}
            return AjaxJson(result);
        }

        public ActionResult Logout()
        {
            //Session["systemUser"] = null;
            CookieHelper.RemoveCookie("systemUserName");
            CookieHelper.RemoveCookie("systemUserAccount");
            CookieHelper.RemoveCookie("systemUserObjectId");
            Response.Redirect("/Login/Index");
            return View();
        }
    }
}