using BaseEnvironment;
using CommonTool;
using CommonTool.DBHelper;
using CommonTool.Workflow;
using DapperMvc.Enum;
using Domain;
using IService;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace DapperMvc.Controllers
{
    /// <summary>
    /// 流程管理
    /// </summary>
    public class ProcessController : BasePageController
    {
        //
        // GET: /Process/
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ProcessController));

        private IProc_Type _procType;
        private IOT_WorkflowTemplateDraftService _workflowTemplateDraftService;
        private IOT_WorkflowTemplateService _workflowTemplateService;
        private IOT_BizObjectSchemaDraftService _bizObjectSchemaDraftService;
        private IOT_BizObjectSchemaService _bizObjectSchemaService;
        public ProcessController(IProc_Type procType, IOT_WorkflowTemplateDraftService workflowTemplateDraftService, IOT_BizObjectSchemaDraftService bizObjectSchemaDraftService, IOT_BizObjectSchemaService bizObjectSchemaService, IOT_WorkflowTemplateService workflowTemplateService)
        {
            this._procType = procType;
            this._workflowTemplateDraftService = workflowTemplateDraftService;
            this._workflowTemplateService = workflowTemplateService;
            this._bizObjectSchemaDraftService = bizObjectSchemaDraftService;
            this._bizObjectSchemaService = bizObjectSchemaService;
        }
        /// <summary>
        /// 流程模型
        /// </summary>
        /// <returns></returns>
        public ActionResult ProceduralModel()
        {
            List<Proc_Type> list = _procType.SelectData<Proc_Type>("select * from Proc_Type");
            JArray treeData = new JArray();
            foreach (var item in list)
            {
                JObject jo = new JObject();
                jo["id"] = item.ObjectId;
                jo["pid"] = 0;
                jo["text"] = item.ProcessType;
                jo["isprocesstype"] = true;
                jo["icon"] = "processtreeiconcolor";
                treeData.Add(jo);
                List<OT_WorkflowTemplate> sonList = _workflowTemplateDraftService.SelectData<OT_WorkflowTemplate>("select ObjectID,ParentObjectID,WorkflowCode,ProcessName from OT_WorkflowTemplate where ParentObjectID='" + item.ObjectId + "'");
                foreach (var sonItem in sonList)
                {
                    JObject sonJo = new JObject();
                    sonJo["id"] = sonItem.ObjectID;
                    sonJo["pid"] = sonItem.ParentObjectID;
                    sonJo["text"] = sonItem.ProcessName;
                    sonJo["isprocess"] = true;
                    sonJo["workflowcode"] = sonItem.WorkflowCode;
                    sonJo["icon"] = "processtreeiconcolor glyphicon glyphicon-briefcase";
                    treeData.Add(sonJo);
                    //数据模型
                    JObject dataModel = new JObject();
                    dataModel["id"] = sonItem.WorkflowCode + "Table";
                    dataModel["pid"] = sonItem.ObjectID;
                    dataModel["text"] = "数据模型";
                    dataModel["isdatamodel"] = true;
                    dataModel["workflowcode"] = sonItem.WorkflowCode;
                    dataModel["icon"] = "processtreeiconcolor glyphicon glyphicon-list";
                    treeData.Add(dataModel);
                    //流程图
                    JObject picModel = new JObject();
                    picModel["id"] = sonItem.WorkflowCode + "Picture";
                    picModel["pid"] = sonItem.ObjectID;
                    picModel["text"] = "流程图";
                    picModel["ispicture"] = true;
                    picModel["workflowcode"] = sonItem.WorkflowCode;
                    picModel["icon"] = "processtreeiconcolor glyphicon glyphicon-stats";
                    treeData.Add(picModel);
                }
            }
            ViewData["treeData"] = treeData.ToString();
            return View();
        }

        /// <summary>
        /// 流程基本数据页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkflowPackage()
        {
            return View();
        }

        /// <summary>
        /// 数据模型页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BizObjectSchema()
        {
            string WorkFlowCode = Request.Params["WorkFlowCode"] + string.Empty;
            ViewData["WorkFlowCode"] = WorkFlowCode;
            List<OT_BizObjectSchema> list = new List<OT_BizObjectSchema>();
            list = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + WorkFlowCode + "'");
            OT_BizObjectSchema model = new OT_BizObjectSchema();
            if (list.Count > 0)
            {
                model = list[0];
            }
            return View(model);
        }
        /// <summary>
        /// 添加数据字段
        /// </summary>
        /// <returns></returns>
        public ActionResult AddObjectSchemaField()
        {
            return View();
        }

        /// <summary>
        /// 保存数据模型字段
        /// </summary>
        /// <param name="FiledListString"></param>
        /// <returns></returns>
        public ActionResult SaveObjectSchema(string FieldListString, string WorkFlowCode)
        {
            AjaxReturnData result = new AjaxReturnData();

            #region 保存数据模型字段

            List<OT_BizObjectSchema> list = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + WorkFlowCode + "'");

            if (list.Count > 0)
            {
                OT_BizObjectSchema model = list[0];

                JArray Fields = JArray.Parse(model.Content);

                JArray newField = JArray.Parse(FieldListString);

                bool hasField = false;

                if (!Convert.ToBoolean(newField[0]["IsSonTable"] + string.Empty))
                {
                    //非子表数据
                    foreach (JObject item in Fields)
                    {
                        if (item["FieldCode"] + string.Empty == newField[0]["FieldCode"] + string.Empty)
                        {
                            hasField = true;
                        }
                    }

                    if (hasField)
                    {

                        result.States = false;

                        result.ErrorMessage = "已存在相同编码";

                    }
                    else
                    {
                        Fields.Add(newField[0]);

                        model.Content = Fields.ToString();

                        int updateCount = _bizObjectSchemaService.UpdateTable<OT_BizObjectSchema>(model);

                        if (updateCount > 0)
                        {
                            result.Message = "更新成功.";
                        }
                        else
                        {
                            result.States = false;

                            result.ErrorMessage = "更新失败.";
                        }
                    }
                }
                else
                {
                    //子表数据
                    foreach (JObject item in Fields)
                    {
                        if (item["ObjectId"] + string.Empty == newField[0]["ParentProperty"] + string.Empty)
                        {
                            foreach (JObject children in JArray.Parse(item["children"] + string.Empty))
                            {
                                if (children["FieldCode"] + string.Empty == newField[0]["FieldCode"] + string.Empty)
                                {
                                    hasField = true;
                                }
                            }
                            if (hasField)
                            {
                                result.States = false;

                                result.ErrorMessage = "已存在相同编码";
                            }
                            else
                            {

                                JArray children = JArray.Parse(item["children"] + string.Empty);

                                children.Add(newField[0]);

                                item["children"] = children;

                                model.Content = Fields.ToString();

                                int updateCount = _bizObjectSchemaService.UpdateTable<OT_BizObjectSchema>(model);

                                if (updateCount > 0)
                                {
                                    result.Message = "更新成功.";
                                }
                                else
                                {
                                    result.States = false;

                                    result.ErrorMessage = "更新失败.";
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                OT_BizObjectSchema model = new OT_BizObjectSchema();
                model.ObjectID = Guid.NewGuid().ToString();
                model.SchemaCode = WorkFlowCode;
                model.Content = FieldListString;
                model.DisplayName = "数据模型";
                model.CreatedTime = DateTime.Now;
                model.ModifiedTime = DateTime.Now;
                int insertCount = _bizObjectSchemaService.Insert<OT_BizObjectSchema>(model);
                if (insertCount > 0)
                {
                    result.Message = "保存成功.";
                }
                else
                {
                    result.States = false;

                    result.ErrorMessage = "保存数据失败.";
                }
            }

            #endregion
            return AjaxJson(result);
        }

        /// <summary>
        /// 发布数据模型
        /// </summary>
        /// <param name="FieldListString"></param>
        /// <param name="WorkFlowCode"></param>
        /// <returns></returns>
        public ActionResult PublishObjectSchema(string FieldListString, string WorkFlowCode)
        {
            AjaxReturnData result = new AjaxReturnData();
            JArray ObjectSchemaList = JArray.Parse(FieldListString);
            //已保存的数据模型
            OT_BizObjectSchema model = new OT_BizObjectSchema();
            model = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + WorkFlowCode + "'")[0];

            string hasTableSql = "select count(*) from sysobjects where id = object_id('Work6-OA.dbo.I_" + WorkFlowCode + "')";
            int hasTableCount = DapperHelper.CreateInstance().ExecuteScalar(hasTableSql);
            if (hasTableCount > 0)
            {
                //已经发布过的数据模型

            }
            else
            {

                StringBuilder FieldSql = new StringBuilder();
                //未发布
                foreach (JObject item in ObjectSchemaList)
                {
                    if (item["LogicType"] + string.Empty != "BizObjectArray")
                    {
                        //非子表
                        FieldSql.Append(item["FieldCode"] + string.Empty);
                        switch (item["LogicType"] + string.Empty)
                        {
                            case "ShortString":
                            case "SingleParticipant":
                                FieldSql.Append(" varchar(255),");
                                break;

                            case "DateTime":
                                FieldSql.Append(" datetime,");
                                break;
                            default:
                                break;
                        }
                    }

                    //改变发布状态
                    item["IsPublished"] = true;
                }

                string CreateTableSql = string.Format(@"CREATE TABLE I_{1} 
                                            (ObjectId char(36) ,
                                            Name nvarchar(255) ,
                                            CreatedBy varchar(255),
                                            CreatedByParentId varchar(255),
                                            ModifiedBy varchar(255),
                                            CreatedTime datetime,
                                            ModifiedTime datetime,
                                            RunningInstanceId varchar(255),
                                            {0}
                                            PRIMARY KEY (ObjectId)
                                            )", FieldSql.ToString(), WorkFlowCode);





                #region 发布数据模型字段


                OT_BizObjectSchemaDraft publishModel = new OT_BizObjectSchemaDraft();

                model.Content = ObjectSchemaList.ToString();
                model.ModifiedTime = DateTime.Now;

                publishModel.ObjectID = Guid.NewGuid().ToString();
                publishModel.SchemaCode = WorkFlowCode;
                publishModel.Content = ObjectSchemaList.ToString();
                publishModel.DisplayName = "数据模型";
                publishModel.CreatedTime = DateTime.Now;
                publishModel.ModifiedTime = DateTime.Now;
                //事务处理
                using (IDbConnection dbConnection = new SqlConnection(BaseApplication.DefaultSqlConnectionKey))
                {
                    dbConnection.Open();
                    IDbTransaction transaction = dbConnection.BeginTransaction();
                    try
                    {
                        DapperHelper.CreateInstance().ExecuteNoneQuery(CreateTableSql); //创建数据表
                        int insertCount = _bizObjectSchemaDraftService.Insert<OT_BizObjectSchemaDraft>(publishModel);
                        int updateCount = _bizObjectSchemaService.UpdateTable<OT_BizObjectSchema>(model);
                        if (insertCount > 0 && updateCount > 0)
                        {
                            result.Message = "发布成功";
                        }

                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        result.ErrorMessage = exception.ToString();
                        transaction.Rollback();

                    }


                }

                #endregion

            }

            return AjaxJson(result);
        }

        /// <summary>
        /// 编辑流程分类
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProc_Type(Proc_Type model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            if (model.ObjectId == "" || model.ObjectId == null)
            {
                //新增
                model.ObjectId = Guid.NewGuid().ToString();
                EditCount = _procType.Insert<Proc_Type>(model);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ObjectId", model.ObjectId);
                //修改
                EditCount = _procType.UpdateTable<Proc_Type>(model, param);
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
        /// 删除流程
        /// </summary>
        /// <param name="WorkFlowCode"></param>
        /// <returns></returns>
        public ActionResult DeleteProcess(string WorkflowCode)
        {
            AjaxReturnData result = new AjaxReturnData();
            List<string> sqlList = new List<string>();
            sqlList.Add(string.Format("delete from OT_BizObjectSchema where SchemaCode='{0}'", WorkflowCode));//删除保存的数据模型
            sqlList.Add(string.Format("delete from OT_BizObjectSchemaDraft where SchemaCode='{0}'", WorkflowCode));//删除发布的数据模型
            sqlList.Add(string.Format("delete from OT_WorkflowTemplate where WorkflowCode='{0}'", WorkflowCode));//删除流程模板
            sqlList.Add(string.Format("delete from OT_WorkflowTemplateDraft where WorkflowCode='{0}'", WorkflowCode));//删除流程模板
            sqlList.Add(string.Format(@"if exists (select 1  from  sysobjects  
                                        where  id = object_id('I_{0}')   and   type = 'U')   
                                        drop table I_{0}", WorkflowCode));//删除数据表
            int successCount = DapperHelper.CreateInstance().TransAction(sqlList);
            if (successCount > 0)
            {
                result.Message = "删除成功.";
            }
            else
            {
                result.States = false;
                result.ErrorMessage = "删除失败.";
            }

            return AjaxJson(result);
        }


        #region 流程图设计器
        /// <summary>
        /// 画图页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProcessDesigner(string ObjectId)
        {
            List<OT_WorkflowTemplateDraft> modelList = _workflowTemplateDraftService.SelectData<OT_WorkflowTemplateDraft>(@"SELECT [ObjectID]
      ,[Content]
      ,[Creator]
      ,[ModifiedBy]
      ,[CreatedTime]
      ,[ModifiedTime]
      ,[WorkflowCode]
      ,[BizObjectSchemaCode]
      ,[ParentObjectID]
      ,[ParentPropertyName]
      ,[ParentIndex],ProcessName
  FROM [Work6-OA].[dbo].[OT_WorkflowTemplate] where ObjectId='" + ObjectId + "'");
            OT_WorkflowTemplateDraft model = modelList[0];
            WorkflowTemplate workflowTemplate = WorkflowHelper.GetWorkflowTemplate(model);
            ViewData["WorkflowTemplate"] = new JavaScriptSerializer().Serialize(workflowTemplate);
            return View();
        }
        /// <summary>
        /// Iframe静态页
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkflowThumbnail()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 编辑流程模板配置
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ObjectID"></param>
        /// <param name="ProcessParentId"></param>
        /// <param name="ProcessName"></param>
        /// <param name="ProcessCode"></param>
        /// <returns></returns>
        public ActionResult EditWorkflowTemplate(int Action, string ObjectID, string ProcessParentId, string ProcessName, string ProcessCode)
        {

            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;

            if (Convert.ToInt32(EditType.增加) == Action)
            {
                //新增
                OT_WorkflowTemplate model = new OT_WorkflowTemplate();
                model.ObjectID = ObjectID;
                model.ParentObjectID = ProcessParentId;
                model.WorkflowCode = ProcessCode;
                model.ProcessName = ProcessName;
                model.Content = WorkflowHelper.CreateDefaultContent(ProcessCode);
                EditCount = _workflowTemplateService.Insert<OT_WorkflowTemplate>(model);
            }
            else if (Convert.ToInt32(EditType.修改) == Action)
            {

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
    }
}
