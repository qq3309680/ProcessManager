using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DapperMvc.Controllers
{
    /// <summary>
    /// 错误友好页面
    /// </summary>
    public class ErrorController : BasePageController
    {
        public ActionResult NoAuthority()
        {
            return View();
        }
    }
}