using BaseEnvironment;
using DapperMvc.App_Start;
using Domain;
using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DapperMvc.Controllers
{
    [LoginIn]
    public class SystemManagerController : BasePageController
    {

        private ISystemUser _systemUser;
        private ISystemRole _systemRole;
        private IRoleRelationship _roleRelationship;
        private IAuthority _authority;

        public SystemManagerController(ISystemUser systemUser, ISystemRole systemRole, IRoleRelationship roleRelationship, IAuthority authority)
        {
            this._authority = authority;
            this._systemRole = systemRole;
            this._systemUser = systemUser;
            this._roleRelationship = roleRelationship;
        }

        /// <summary>
        /// 人员管理
        /// </summary>
        /// <returns></returns>
        [PageAuth]
        public ActionResult SystemUserView(int pageIndex = 1)
        {
            int pageSize = 15;
            int totalCount = 0;

            List<SystemUser> list = _systemUser.SimplePageQuery<SystemUser>(pageSize, pageIndex, "UserName", out totalCount, "select  * from SystemUser");

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPage = Math.Ceiling(Convert.ToDecimal(totalCount / pageSize)) + totalCount % pageSize != 0 ? 1 : 0;

            return View(list);
        }


        /// <summary>
        /// 人员选择
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectSystemUser()
        {
            AjaxReturnData result = new AjaxReturnData();

            List<SystemUser> list = _systemUser.SelectData<SystemUser>("select  * from SystemUser");

            result.Data = list;

            return AjaxJson(result);
        }


        /// <summary>
        /// 人员管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemUserEdit(SystemUser model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            if (model.ObjectId == "" || model.ObjectId == null)
            {
                //新增
                model.ObjectId = Guid.NewGuid().ToString();

                EditCount = _systemUser.Insert<SystemUser>(model);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ObjectId", model.ObjectId);
                //修改
                EditCount = _systemUser.UpdateTable<SystemUser>(model, param);
            }
            if (EditCount > 0)
            {
                result.States = true;
                result.Message = "成功.";

            }
            else
            {
                result.States = false;
                result.Message = "失败.";

            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 人员管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemUserDelete(string objectId)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            Dictionary<string, object> param = new Dictionary<string, object>();

            param.Add("ObjectId", objectId);

            EditCount = _systemUser.DeleteSigerData<SystemUser>(param);

            if (EditCount > 0)
            {
                result.States = true;
                result.Message = "成功.";

            }
            else
            {
                result.States = false;
                result.Message = "失败.";

            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 角色管理
        /// </summary>
        /// <returns></returns>
        [PageAuth]
        public ActionResult SystemRoleView(int pageIndex = 1)
        {
            int pageSize = 15;
            int totalCount = 0;
            List<SystemRole> list = _systemRole.SimplePageQuery<SystemRole>(pageSize, pageIndex, "RoleName", out totalCount, "select  * from SystemRole");

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPage = Math.Ceiling(Convert.ToDecimal(totalCount / pageSize)) + totalCount % pageSize != 0 ? 1 : 0;

            return View(list);
        }

        /// <summary>
        /// 人员管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemRoleEdit(SystemRole model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            if (model.ObjectId == "" || model.ObjectId == null)
            {
                //新增
                model.ObjectId = Guid.NewGuid().ToString();

                EditCount = _systemRole.Insert<SystemRole>(model);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ObjectId", model.ObjectId);
                //修改
                EditCount = _systemRole.UpdateTable<SystemRole>(model, param);
            }
            if (EditCount > 0)
            {
                result.States = true;
                result.Message = "成功.";

            }
            else
            {
                result.States = false;
                result.Message = "失败.";

            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 角色管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemRoleDelete(string objectId)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            Dictionary<string, object> param = new Dictionary<string, object>();

            param.Add("ObjectId", objectId);

            EditCount = _systemRole.DeleteSigerData<SystemRole>(param);

            if (EditCount > 0)
            {
                result.States = true;
                result.Message = "成功.";

            }
            else
            {
                result.States = false;
                result.Message = "失败.";

            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 获取角色配置的人员
        /// </summary>
        /// <param name="ObjectId">角色ObjectId</param>
        /// <returns></returns>
        public ActionResult GetRoleUsers(string objectId)
        {
            AjaxReturnData result = new AjaxReturnData();


            List<RoleRelationship> list = _roleRelationship.SelectData<RoleRelationship>("select * from RoleRelationship where RoleObjectId='" + objectId + "'");

            result.Data = list;

            return AjaxJson(result);
        }


        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="ObjectId">角色ObjectId</param>
        /// <returns></returns>
        public ActionResult GetRoleLists(string roleName = "", int pageIndex = 1)
        {
            AjaxReturnData result = new AjaxReturnData();
            int pageSize = 15;
            int totalCount = 0;

            List<SystemRole> list = new List<SystemRole>();
            if (string.IsNullOrEmpty(roleName))
            {
                list.AddRange(_systemRole.SimplePageQuery<SystemRole>(pageSize, pageIndex, "RoleName", out totalCount, "select * from SystemRole"));
            }
            else
            {

                list.AddRange(_systemRole.SimplePageQuery<SystemRole>(pageSize, pageIndex, "RoleName", out totalCount, "select * from SystemRole where RoleName like '%" + roleName + "%'"));
            }

            result.Data = new { TotalPage = Math.Ceiling(Convert.ToDecimal(totalCount / pageSize)) + totalCount % pageSize != 0 ? 1 : 0, List = list };

            return AjaxJson(result);
        }


        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult EditAuthority(Authority model)
        {
            AjaxReturnData result = new AjaxReturnData();

            int EditCount = 0;


            if (string.IsNullOrEmpty(model.ObjectId))
            {

                //新增
                model.ObjectId = Guid.NewGuid().ToString();

                EditCount = _authority.Insert<Authority>(model);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ObjectId", model.ObjectId);
                //修改
                EditCount = _authority.UpdateTable<Authority>(model, param);
            }


            if (EditCount < 0)
            {
                result.States = false;
            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AuthorityDelete(Authority model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            Dictionary<string, object> param = new Dictionary<string, object>();

            param.Add("ObjectId", model.ObjectId);

            EditCount = _authority.DeleteSigerData<Authority>(param);

            if (EditCount > 0)
            {
                result.States = true;
                result.Message = "成功.";

            }
            else
            {
                result.States = false;
                result.Message = "失败.";

            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 修改权限角色
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="roleNames"></param>
        /// <param name="roleObjectIds"></param>
        /// <returns></returns>
        public ActionResult EditAuthRole(string objectId, string roleNames, string roleObjectIds)
        {

            AjaxReturnData result = new AjaxReturnData();

            Authority model = new Authority();

            model = _authority.SelectData<Authority>("select * from Authority where ObjectId='" + objectId + "'")[0];

            model.RoleNames = roleNames;

            model.RoleObjectIds = roleObjectIds;

            int count = _authority.UpdateTable<Authority>(model);

            if (count <= 0)
            {
                result.States = false;
            }

            return AjaxJson(result);
        }


        /// <summary>
        /// 添加角色人员
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public ActionResult AddRoleUsers(List<RoleRelationship> array, string roleObjectId)
        {
            AjaxReturnData result = new AjaxReturnData();

            List<RoleRelationship> list = _roleRelationship.SelectData<RoleRelationship>("select * from RoleRelationship where RoleObjectId='" + roleObjectId + "'");

            int deleteCount = 0;
            int insertCount = 0;
            if (array != null)
            {
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (array.Find(c => c.UserObjectId == item.UserObjectId) != null)
                        {
                            array.Remove(array.Find(c => c.UserObjectId == item.UserObjectId));
                        }
                        else
                        {
                            //删除
                            Dictionary<string, Object> param = new Dictionary<string, Object>();
                            param.Add("ObjectId", item.ObjectId);
                            deleteCount = _roleRelationship.DeleteSigerData<RoleRelationship>(param);
                        }
                    }
                }

                foreach (var item in array)
                {
                    item.ObjectId = Guid.NewGuid() + String.Empty;
                }
                insertCount = _roleRelationship.InsertBatch<RoleRelationship>(array);
            }
            else
            {
                {
                    foreach (var item in list)
                    {
                        //删除
                        Dictionary<string, Object> param = new Dictionary<string, Object>();
                        param.Add("ObjectId", item.ObjectId);
                        deleteCount = _roleRelationship.DeleteSigerData<RoleRelationship>(param);
                    }
                }
            }


            if (insertCount > 0 || deleteCount > 0)
            {
                result.States = true;
                result.Message = "成功.";

            }
            else
            {
                result.States = false;
                result.Message = "失败.";

            }

            return AjaxJson(result);
        }


        /// <summary>
        /// 权限配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [PageAuth]
        public ActionResult AuthorityConfig(int pageIndex = 1)
        {

            int pageSize = 15;
            int totalCount = 0;

            List<Authority> list = _systemUser.SimplePageQuery<Authority>(pageSize, pageIndex, "PageName", out totalCount, "select  * from Authority");

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPage = Math.Ceiling(Convert.ToDecimal(totalCount / pageSize)) + Convert.ToInt32(totalCount % pageSize != 0 ? 1 : 0);

            return View(list);

        }

        /// <summary>
        /// 外部验证页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidPage()
        {
            return View();
        }

    }
}