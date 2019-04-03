using BaseEnvironment;
using CommonTool;
using DapperMvc.Models;
using Domain;
using IService;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DapperMvc.Controllers
{
    public class BasePageController : BaseController
    {
        //
        // GET: /BasePage/
        private ISys_Menu _sys_MenuService;
        public List<MenuModel> _rootList;
        public BasePageController()
        {
            CacheHelper cache = new CacheHelper();
            _rootList = cache.Get<List<MenuModel>>("webRootMenuList");
            if (_rootList == null || _rootList.Count <= 0)
            {
                this._sys_MenuService = new Sys_MenuService();
                List<MenuModel> RootList = _sys_MenuService.SelectData<MenuModel>("select * from Sys_Menu where ParentID=0 and Visible='True'");
                foreach (MenuModel item in RootList)
                {
                    item.SonMenuList = _sys_MenuService.SelectData<MenuModel>("select * from Sys_Menu where ParentID=" + item.pk_Menu);
                }
                _rootList = RootList;
                CacheHelper.SetCache("webRootMenuList", RootList);
            }
          


        }

    }
}
