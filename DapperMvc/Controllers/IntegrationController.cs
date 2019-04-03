using BaseEnvironment;
using CommonTool.DBHelper;
using DapperMvc.App_Start;
using Domain;
using IService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DapperMvc.Controllers
{
    /// <summary>
    /// 集成功能
    /// </summary>  
    [LoginIn]
    public class IntegrationController : BasePageController
    {
        private IS_DataDictionaryService _dataDictionaryService;

        public IntegrationController(IS_DataDictionaryService dataDictionaryService)
        {
            this._dataDictionaryService = dataDictionaryService;
        }

        public ActionResult DataDictionaryPage()
        {
            List<S_DataDictionary> list = new List<S_DataDictionary>();

            List<S_DataDictionary> categroyList = new List<S_DataDictionary>();

            categroyList = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_OADB"]).SimpleQuery<S_DataDictionary>("select  distinct(Category) FROM [S_DataDictionary]");

            ViewData["categroyArr"] = categroyList;

            string category = "";

            if (Request.Params["Category"] != null)
            {
                category = Request.Params["Category"] + string.Empty;

            }
            else
            {
                category = categroyList[0].Category;
            }

            list = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_OADB"]).SimpleQuery<S_DataDictionary>("select * from S_DataDictionary where Category='" + category + "'").OrderBy(c => c.Sort).ToList();
        

            return View(list);
        }

        /// <summary>
        /// 编辑数据字典
        /// </summary>
        /// <returns></returns>
        public ActionResult EditDataDictionary(S_DataDictionary model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            if (model.TaskID == "" || model.TaskID == null)
            {
                //新增
                model.TaskID = Guid.NewGuid().ToString();

                EditCount = _dataDictionaryService.Insert<S_DataDictionary>(model, ConfigurationManager.AppSettings["SqlConnectionKey_OADB"]);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("TaskID", model.TaskID);
                //修改
                EditCount = _dataDictionaryService.UpdateTable<S_DataDictionary>(model, param, ConfigurationManager.AppSettings["SqlConnectionKey_OADB"]);
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
        /// 编辑数据字典
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteDictionary(string taskID)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TaskID", taskID);
            EditCount = _dataDictionaryService.DeleteSigerData<S_DataDictionary>(param, ConfigurationManager.AppSettings["SqlConnectionKey_OADB"]);

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

    }
}