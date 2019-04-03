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
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace DapperMvc.Controllers
{
    /// <summary>
    /// 系统后台菜单管理
    /// </summary>
    public class BackGroundController : BasePageController
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(BackGroundController));


        private ISys_Menu _sys_MenuService;

        public BackGroundController(ISys_Menu sys_MenuService)
        {
            this._sys_MenuService = sys_MenuService;
        }


        //
        // GET: /BackGround/
        /// <summary>
        /// 管理系统后台主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {


            return View();
        }

        /// <summary>
        /// 系统菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Menu()
        {
            List<Sys_Menu> model = new List<Sys_Menu>();
            string sql = "select * from Sys_Menu";
            model = _sys_MenuService.SelectData<Sys_Menu>(sql);
            return View(model);
        }

        /// <summary>
        /// 菜单编辑页面
        /// </summary>
        /// <param name="active"></param>
        /// <param name="parentId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MenuEdit(string active, int parentId = 0, string id = "")
        {
            Sys_Menu model = new Sys_Menu();
            if (active == "1")
            {
                //新增
                if (parentId != 0)
                {
                    parentId = Convert.ToInt32(Request.Params["parentId"]);
                }
                model.ParentID = parentId;
            }
            else
            {
                //修改
                if (id != "")
                {
                    id = Request.Params["id"].ToString();
                }
                string sql = "select * from Sys_Menu where pk_Menu='" + id + "'";
                model = _sys_MenuService.SelectData<Sys_Menu>(sql)[0];
            }

            return View(model);
        }

        /// <summary>
        /// 编辑菜单页面保存事件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SubmitMenuEdit(Sys_Menu model)
        {
            AjaxReturnData result = new AjaxReturnData();
            if (model.pk_Menu == 0)
            {
                //新增
                string maxPk_MenuSql = "select max(pk_Menu)+1 as pk_Menu from Sys_Menu";
                int pk_Menu = DapperHelper.CreateInstance().ExecuteScalar(maxPk_MenuSql);
                model.pk_Menu = pk_Menu;
                int insertCount = _sys_MenuService.Insert<Sys_Menu>(model);
                if (insertCount > 0)
                {
                    result.States = true;
                    result.Message = "新增数据成功.";

                }
                else
                {
                    result.States = false;
                    result.Message = "新增数据失败.";

                }
            }
            else
            {
                //修改
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pk_Menu", model.pk_Menu);
                int updateCount = _sys_MenuService.UpdateTable<Sys_Menu>(model, param);
                if (updateCount > 0)
                {
                    result.States = true;
                    result.Message = "修改数据成功.";

                }
                else
                {
                    result.States = false;
                    result.Message = "修改数据失败.";

                }
            }
            CacheHelper.Remove("webRootMenuList");
            return AjaxJson(result);

        }

        /// <summary>
        /// 编辑菜单页面保存事件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult DeleteMenu(string pk_Menu)
        {
            AjaxReturnData result = new AjaxReturnData();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("pk_Menu", pk_Menu);
            int deleteCount = _sys_MenuService.DeleteSigerData<Sys_Menu>(param);
            if (deleteCount > 0)
            {
                result.States = true;
                result.Message = "删除数据成功.";

            }
            else
            {
                result.States = false;
                result.Message = "删除数据失败.";

            }
            CacheHelper.Remove("webRootMenuList");
            return AjaxJson(result);

        }

        /// <summary>
        /// 图标页面
        /// </summary>
        /// <returns></returns>
        public ActionResult IconListPage()
        {
            return View();
        }


        //        /// <summary>
        //        /// 迭代获得所需的结构菜单
        //        /// </summary>
        //        /// <returns></returns>
        //        public List<ManagerPageMenuModel> GetSonMenu(AdminMenu menuModel)
        //        {

        //            List<ManagerPageMenuModel> sonList = new List<ManagerPageMenuModel>();
        //            List<AdminMenu> listTaget = _adminMenuService.GetMenuListByParentId(menuModel.ObjectId);

        //            foreach (var item in listTaget)
        //            {
        //                ManagerPageMenuModel sonModel = new ManagerPageMenuModel();
        //                sonModel.ObjectId = item.ObjectId;
        //                sonModel.ParentObjectId = item.ParentObjectId;
        //                sonModel.DisplayName = item.DisplayName;
        //                sonModel.IsLeaf = item.IsLeaf;
        //                sonModel.IsRoot = item.IsRoot;
        //                sonModel.Href = item.Href;
        //                sonModel.Sort = item.Sort;
        //                sonModel.Level = item.Level;
        //                sonModel.IconImg = item.IconImg;
        //                if (!menuModel.IsLeaf)
        //                {
        //                    sonModel.SonMenuModel = GetSonMenu(item);
        //                }
        //                else
        //                {
        //                    sonModel.SonMenuModel = null;
        //                }
        //                sonList.Add(sonModel);
        //            }
        //            return sonList;
        //        }



    }
}
