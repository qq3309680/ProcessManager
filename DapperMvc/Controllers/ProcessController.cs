using BaseEnvironment;
using CommonTool;
using CommonTool.DBHelper;
using CommonTool.Workflow;
using DapperMvc.App_Start;
using DapperMvc.Enum;
using Domain;
using IService;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    [LoginIn]
    public class ProcessController : BasePageController
    {
        #region 私有参数
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ProcessController));

        private IProc_NodeService _procNode;
        private IProc_Type _procType;
        private IOT_WorkflowTemplateDraftService _workflowTemplateDraftService;
        private IOT_WorkflowTemplateService _workflowTemplateService;
        private IOT_BizObjectSchemaDraftService _bizObjectSchemaDraftService;
        private IOT_BizObjectSchemaService _bizObjectSchemaService;
        private IProcessTableModel _processTableModel;
        #endregion

        #region 构造函数
        public ProcessController(IProc_NodeService procNode, IProc_Type procType, IOT_WorkflowTemplateDraftService workflowTemplateDraftService, IOT_BizObjectSchemaDraftService bizObjectSchemaDraftService, IOT_BizObjectSchemaService bizObjectSchemaService, IOT_WorkflowTemplateService workflowTemplateService, IProcessTableModel processTableModel)
        {
            this._procNode = procNode;
            this._procType = procType;
            this._workflowTemplateDraftService = workflowTemplateDraftService;
            this._workflowTemplateService = workflowTemplateService;
            this._bizObjectSchemaDraftService = bizObjectSchemaDraftService;
            this._bizObjectSchemaService = bizObjectSchemaService;
            this._processTableModel = processTableModel;
        }

        #endregion

        #region 文件夹
        /// <summary>
        /// 文件夹
        /// </summary>
        /// <returns></returns>
        public ActionResult File()
        {

            string ObjectId = Request.Params["ObjectId"].ToString();
            List<Proc_Type> list = _procType.SelectData<Proc_Type>("select * from Proc_Type where ObjectId='" + ObjectId + "' and IsDelete!='1'");
            Proc_Type model = new Proc_Type();
            if (list.Count > 0)
            {
                model = list[0];
            }


            return View(model);
        }
        #endregion

        #region 流程模型
        /// <summary>
        /// 流程模型
        /// </summary>
        /// <returns></returns>
        [PageAuth]
        public ActionResult ProceduralModel()
        {
            List<Proc_Type> list = _procType.SelectData<Proc_Type>("select * from Proc_Type where ParentObjectId='0' and IsDelete!='1' order by Sort");
            JArray treeData = new JArray();
            treeData = GetTreeData(list, treeData);
            ViewData["treeData"] = treeData.ToString().Trim();
            return View();
        }


        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private JArray GetTreeData(List<Proc_Type> list, JArray treeData)
        {


            foreach (var item in list)
            {
                #region 获取流程信息
                JObject jo = new JObject();
                jo["id"] = item.ObjectId;
                jo["pid"] = item.ParentObjectId;
                jo["text"] = item.ProcessType;
                jo["isprocesstype"] = true;
                jo["icon"] = "processtreeiconcolor";
                treeData.Add(jo);
                List<OT_WorkflowTemplate> sonList = _workflowTemplateDraftService.SelectData<OT_WorkflowTemplate>("select ObjectID,ParentObjectID,WorkflowCode,ProcessName from OT_WorkflowTemplate where ParentObjectID='" + item.ObjectId + "' and IsDelete!='1' order by Sort ");

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

                    #region 数据模型
                    JObject dataModel = new JObject();
                    dataModel["id"] = sonItem.WorkflowCode + "Data";
                    dataModel["pid"] = sonItem.ObjectID;
                    dataModel["text"] = "数据模型";
                    dataModel["isdatamodel"] = true;
                    dataModel["workflowcode"] = sonItem.WorkflowCode;
                    dataModel["workflowname"] = sonItem.ProcessName;
                    dataModel["icon"] = "processtreeiconcolor glyphicon glyphicon-list";
                    treeData.Add(dataModel);
                    #endregion

                    #region 表单设计
                    JObject formModel = new JObject();
                    formModel["id"] = sonItem.WorkflowCode + "Form";
                    formModel["pid"] = sonItem.ObjectID;
                    formModel["text"] = "表单设计";
                    formModel["isformmodel"] = true;
                    formModel["workflowcode"] = sonItem.WorkflowCode;
                    formModel["workflowname"] = sonItem.ProcessName;
                    formModel["icon"] = "fa fa-fw fa-file-code-o processtreeiconcolor";
                    treeData.Add(formModel);
                    #endregion



                    #region 流程台账
                    JObject tableModel = new JObject();
                    tableModel["id"] = sonItem.WorkflowCode + "Table";
                    tableModel["pid"] = sonItem.ObjectID;
                    tableModel["text"] = "流程台账";
                    tableModel["istablemodel"] = true;
                    tableModel["workflowcode"] = sonItem.WorkflowCode;
                    tableModel["workflowname"] = sonItem.ProcessName;
                    tableModel["icon"] = "processtreeiconcolor  glyphicon glyphicon-list-alt";
                    treeData.Add(tableModel);
                    #endregion


                    #region 流程图
                    ////流程图
                    //JObject picModel = new JObject();
                    //picModel["id"] = sonItem.WorkflowCode + "Picture";
                    //picModel["pid"] = sonItem.ObjectID;
                    //picModel["text"] = "流程图";
                    //picModel["ispicture"] = true;
                    //picModel["workflowcode"] = sonItem.WorkflowCode;
                    //picModel["workflowname"] = sonItem.ProcessName;
                    //picModel["icon"] = "processtreeiconcolor glyphicon glyphicon-stats";
                    //treeData.Add(picModel);
                    #endregion

                }
                #endregion
                #region 获取文件夹信息
                List<Proc_Type> sonFileList = _procType.SelectData<Proc_Type>("select * from Proc_Type where ParentObjectId='" + item.ObjectId + "' and IsDelete!='1'  order by Sort");
                if (sonFileList.Count > 0)
                {

                    treeData = GetTreeData(sonFileList, treeData);

                }
                #endregion
            }
            return treeData;

        }


        /// <summary>
        /// 编辑流程分类
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProc_Type(Proc_Type model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            model.ModifyDate = DateTime.Now;
            if (model.ObjectId == "" || model.ObjectId == null)
            {
                //新增
                model.ObjectId = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(model.ParentObjectId))
                {
                    model.ParentObjectId = "0";
                }



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
        /// 删除文件夹
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult DeleteProc_Type(Proc_Type model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            Dictionary<string, object> param = new Dictionary<string, object>();

            param.Add("ObjectId", model.ObjectId);

            model = _procType.SelectData<Proc_Type>("select  * from Proc_Type where ObjectId='" + model.ObjectId + "'")[0];

            model.IsDelete = true;

            EditCount = _procType.UpdateTable<Proc_Type>(model, param);

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
            //sqlList.Add(string.Format("delete from OT_BizObjectSchemaDraft where SchemaCode='{0}'", WorkflowCode));//删除发布的数据模型
            sqlList.Add(string.Format("update  OT_WorkflowTemplate set IsDelete='1' where WorkflowCode='{0}'", WorkflowCode));//删除流程模板
            //sqlList.Add(string.Format("delete from OT_WorkflowTemplateDraft where WorkflowCode='{0}'", WorkflowCode));//删除流程模板
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


        /// <summary>
        /// 流程基本数据页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkflowPackage()
        {

            string ObjectId = Request.Params["ObjectId"].ToString();
            List<OT_WorkflowTemplate> list = _procType.SelectData<OT_WorkflowTemplate>("select * from OT_WorkflowTemplate where ObjectId='" + ObjectId + "' and IsDelete!='1'");
            OT_WorkflowTemplate model = new OT_WorkflowTemplate();
            if (list.Count > 0)
            {
                model = list[0];
            }

            return View(model);
        }
        #endregion

        #region 数据模型
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
        /// 删除表字段
        /// </summary>
        /// <param name="FieldObjectId"></param>
        /// <param name="WorkFlowCode"></param>
        /// <returns></returns>
        public ActionResult DeleteField(string FieldObjectId, bool IsSonTable, string ParentProperty, string WorkFlowCode)
        {
            AjaxReturnData result = new AjaxReturnData();

            List<OT_BizObjectSchema> list = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + WorkFlowCode + "'");


            if (list.Count > 0)
            {
                OT_BizObjectSchema model = list[0];

                JArray Fields = JArray.Parse(model.Content);

                result = DataModelHelper.DeleteTableField(WorkFlowCode, FieldObjectId, IsSonTable, ParentProperty, Fields, model, _bizObjectSchemaService);

            }

            return AjaxJson(result);
        }


        /// <summary>
        /// 保存数据模型字段
        /// </summary>
        /// <param name="FiledListString"></param>
        /// <returns></returns>
        public ActionResult SaveObjectSchema(string FieldListString, string WorkFlowCode, bool IsShareTable, string ShareTableCode)
        {
            AjaxReturnData result = new AjaxReturnData();
            #region 保存数据模型字段

            List<OT_BizObjectSchema> list = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + WorkFlowCode + "'");

            if (list.Count > 0)
            {
                OT_BizObjectSchema model = list[0];

                JArray Fields = JArray.Parse(model.Content);

                JArray newField = JArray.Parse(FieldListString);

                if (!Convert.ToBoolean(newField[0]["IsSonTable"] + string.Empty))
                {
                    //非子表数据
                    result = DataModelHelper.AddMainTableField(FieldListString, Fields, model, _bizObjectSchemaService);
                }
                else
                {
                    //子表数据
                    result = DataModelHelper.AddSonTableField(FieldListString, Fields, model, _bizObjectSchemaService);
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
                model.IsShareTable = IsShareTable;
                model.ShareTableCode = ShareTableCode;
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
        /// 导入表单数据模型
        /// </summary>
        /// <param name="WorkFlowCode"></param>
        /// <param name="IsShareTable"></param>
        /// <param name="ShareTableCode"></param>
        /// <returns></returns>
        public ActionResult InputDateTable(string WorkFlowCode, bool IsShareTable, string ShareTableCode)
        {
            AjaxReturnData result = new AjaxReturnData();

            List<OT_BizObjectSchema> list = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + WorkFlowCode + "'");

            List<OT_BizObjectSchema> inputList = _bizObjectSchemaService.SelectData<OT_BizObjectSchema>("select * from OT_BizObjectSchema where SchemaCode='" + ShareTableCode + "'");

            if (list.Count > 0)
            {
                list[0].Content = inputList[0].Content;
                list[0].IsShareTable = true;
                list[0].ShareTableCode = ShareTableCode;
                int updateCount = _bizObjectSchemaService.UpdateTable<OT_BizObjectSchema>(list[0]);
                if (updateCount > 0)
                {
                    result.Message = "保存成功.";
                }
                else
                {
                    result.States = false;

                    result.ErrorMessage = "保存数据失败.";
                }
            }
            else
            {
                OT_BizObjectSchema model = new OT_BizObjectSchema();
                model.ObjectID = Guid.NewGuid().ToString();
                model.SchemaCode = WorkFlowCode;
                model.Content = inputList[0].Content;
                model.DisplayName = "数据模型";
                model.CreatedTime = DateTime.Now;
                model.ModifiedTime = DateTime.Now;
                model.IsShareTable = IsShareTable;
                model.ShareTableCode = ShareTableCode;
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

            string hasTableSql = "select count(*) from sysobjects where id = object_id('" + ConfigurationManager.AppSettings["BpmDbName"] + ".dbo.I_" + WorkFlowCode + "')";
            int hasTableCount = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteScalar(hasTableSql);
            if (hasTableCount > 0)
            {
                #region 再次发布
                foreach (JObject item in ObjectSchemaList)
                {
                    if (!(bool)item["IsPublished"])
                    {
                        if (item["LogicType"] + string.Empty != "BizObjectArray")
                        {
                            #region 增加主数据表字段
                            StringBuilder addTableFieldSql = new StringBuilder();
                            addTableFieldSql.Append("alter table  I_" + WorkFlowCode + " add  [" + item["FieldCode"] + "]");
                            switch (item["LogicType"] + string.Empty)
                            {
                                case "ShortString":
                                case "SingleParticipant":
                                    addTableFieldSql.Append(" nvarchar(255)");
                                    break;

                                case "String":
                                    addTableFieldSql.Append(" nvarchar(max)");
                                    break;

                                case "Bool":
                                    addTableFieldSql.Append(" bit");
                                    break;

                                case "Int":
                                    addTableFieldSql.Append(" int");
                                    break;

                                case "Long":
                                    addTableFieldSql.Append(" bigint");
                                    break;

                                case "Double":
                                    addTableFieldSql.Append(" float");
                                    break;

                                case "DateTime":
                                    addTableFieldSql.Append(" datetime");
                                    break;

                                case "Attachment":
                                    addTableFieldSql.Append(" nvarchar(max)");
                                    break;

                                default:
                                    break;
                            }
                            DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(addTableFieldSql.ToString());
                            #endregion
                        }
                        else
                        {
                            #region 增加子表数据表字段

                            string sonTableCode = item["FieldCode"].ToString();

                            string hasSonTableSql = "select count(*) from sysobjects where id = object_id('" + ConfigurationManager.AppSettings["BpmDbName"] + ".dbo.C_" + sonTableCode + "')";
                            int hasSonTableCount = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteScalar(hasSonTableSql);

                            if (hasSonTableCount > 0)
                            {
                                //已发布过子表
                                StringBuilder addSonTableSql = new StringBuilder();

                                addSonTableSql.AppendFormat(@"alter table C_{0} add ", item["FieldCode"]);

                                foreach (JObject sonItem in (JArray)item["children"])
                                {
                                    if (!(bool)sonItem["IsPublished"])
                                    {
                                        //改变发布状态
                                        sonItem["IsPublished"] = true;
                                        addSonTableSql.AppendFormat(@"[{0}]", sonItem["FieldCode"] + string.Empty);
                                        switch (sonItem["LogicType"] + string.Empty)
                                        {
                                            case "ShortString":
                                            case "SingleParticipant":
                                                addSonTableSql.Append(" nvarchar(255)");
                                                break;

                                            case "String":
                                                addSonTableSql.Append(" nvarchar(max)");
                                                break;

                                            case "Bool":
                                                addSonTableSql.Append(" bit");
                                                break;

                                            case "Int":
                                                addSonTableSql.Append(" int");
                                                break;

                                            case "Long":
                                                addSonTableSql.Append(" bigint");
                                                break;

                                            case "Double":
                                                addSonTableSql.Append(" float");
                                                break;

                                            case "DateTime":
                                                addSonTableSql.Append(" datetime");
                                                break;

                                            case "Attachment":
                                                addSonTableSql.Append(" nvarchar(max)");
                                                break;

                                            default:
                                                break;
                                        }

                                    }

                                }
                                DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(addSonTableSql.ToString());
                            }
                            else
                            {
                                //未发布过子表
                                StringBuilder createSonTableSql = new StringBuilder();

                                createSonTableSql.AppendFormat(@"CREATE TABLE C_{0} (ID int IDENTITY(1,1) not null,TaskID int null, ", sonTableCode);

                                foreach (JObject sonItem in (JArray)item["children"])
                                {
                                    //改变发布状态
                                    sonItem["IsPublished"] = true;
                                    createSonTableSql.AppendFormat(@"[{0}]", sonItem["FieldCode"] + string.Empty);
                                    switch (sonItem["LogicType"] + string.Empty)
                                    {
                                        case "ShortString":
                                        case "SingleParticipant":
                                            createSonTableSql.Append(" nvarchar(255),");
                                            break;

                                        case "String":
                                            createSonTableSql.Append(" nvarchar(max),");
                                            break;

                                        case "Bool":
                                            createSonTableSql.Append(" bit,");
                                            break;

                                        case "Int":
                                            createSonTableSql.Append(" int,");
                                            break;

                                        case "Long":
                                            createSonTableSql.Append(" bigint,");
                                            break;

                                        case "Double":
                                            createSonTableSql.Append(" float,");
                                            break;

                                        case "DateTime":
                                            createSonTableSql.Append(" datetime,");
                                            break;

                                        case "Attachment":
                                            createSonTableSql.Append(" nvarchar(max),");
                                            break;

                                        default:
                                            break;
                                    }

                                }
                                createSonTableSql.Append("PRIMARY KEY (ID))");
                                DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(createSonTableSql.ToString());

                            }


                            #endregion

                        }

                        item["IsPublished"] = true;
                    }

                }
                model.Content = ObjectSchemaList.ToString();
                model.ModifiedTime = DateTime.Now;
                int updateCount = _bizObjectSchemaService.UpdateTable<OT_BizObjectSchema>(model);

                if (updateCount > 0)
                {

                    result.Message = "发布成功";

                }
                #endregion
            }
            else
            {
                #region 首次发布
                StringBuilder FieldSql = new StringBuilder();

                List<string> createSonTableSqlDic = new List<string>();


                foreach (JObject item in ObjectSchemaList)
                {

                    if (item["LogicType"] + string.Empty != "BizObjectArray")
                    {
                        #region 主表发布
                        FieldSql.AppendFormat(@"[{0}]", item["FieldCode"] + string.Empty);
                        switch (item["LogicType"] + string.Empty)
                        {
                            case "ShortString":
                            case "SingleParticipant":
                                FieldSql.Append(" nvarchar(255),");
                                break;

                            case "String":
                                FieldSql.Append(" nvarchar(max),");
                                break;

                            case "Bool":
                                FieldSql.Append(" bit,");
                                break;

                            case "Int":
                                FieldSql.Append(" int,");
                                break;

                            case "Long":
                                FieldSql.Append(" bigint,");
                                break;

                            case "Double":
                                FieldSql.Append(" float,");
                                break;

                            case "DateTime":
                                FieldSql.Append(" datetime,");
                                break;

                            case "Attachment":
                                FieldSql.Append(" nvarchar(max),");
                                break;

                            default:
                                break;
                        }
                        #endregion

                    }
                    else
                    {

                        #region 子表发布
                        string sonTableCode = item["FieldCode"].ToString();

                        StringBuilder createSonTableSql = new StringBuilder();

                        createSonTableSql.AppendFormat(@"CREATE TABLE C_{0}  (ID int IDENTITY(1,1) not null,TaskID int null ,", sonTableCode);

                        foreach (JObject sonItem in (JArray)item["children"])
                        {
                            //改变发布状态
                            sonItem["IsPublished"] = true;
                            createSonTableSql.AppendFormat(@"[{0}]", sonItem["FieldCode"] + string.Empty);
                            switch (sonItem["LogicType"] + string.Empty)
                            {
                                case "ShortString":
                                case "SingleParticipant":
                                    createSonTableSql.Append(" nvarchar(255),");
                                    break;

                                case "String":
                                    createSonTableSql.Append(" nvarchar(max),");
                                    break;

                                case "Bool":
                                    createSonTableSql.Append(" bit,");
                                    break;

                                case "Int":
                                    createSonTableSql.Append(" int,");
                                    break;

                                case "Long":
                                    createSonTableSql.Append(" bigint,");
                                    break;

                                case "Double":
                                    createSonTableSql.Append(" float,");
                                    break;

                                case "DateTime":
                                    createSonTableSql.Append(" datetime,");
                                    break;

                                case "Attachment":
                                    createSonTableSql.Append(" nvarchar(max),");
                                    break;

                                default:
                                    break;
                            }

                        }
                        createSonTableSql.Append("PRIMARY KEY (ID))");
                        createSonTableSqlDic.Add(createSonTableSql.ToString());
                        #endregion
                    }

                    //改变发布状态
                    item["IsPublished"] = true;
                }

                string CreateTableSql = string.Format(@"CREATE TABLE I_{1} 
                                            (ID int IDENTITY(1,1) ,
                                            TaskID int ,
                                            SerialNumber varchar(255),
                                            Title nvarchar(255),
                                            Applicant varchar(255),
                                            ApplicantCode varchar(255),
                                            ApplicationDate datetime,
                                            ApplicationCompany varchar(255),
                                            ApplicationCompanyCode varchar(255),
                                            ApplicationDepartment varchar(255),
                                            ApplicationDepartmentCode varchar(255),
                                            {0}
                                            PRIMARY KEY (ID)
                                            )", FieldSql.ToString(), WorkFlowCode);


                #region 发布数据模型字段

                OT_BizObjectSchemaDraft publishModel = new OT_BizObjectSchemaDraft();
                publishModel.ObjectID = Guid.NewGuid().ToString();
                publishModel.SchemaCode = WorkFlowCode;
                publishModel.Content = ObjectSchemaList.ToString();
                publishModel.DisplayName = "数据模型";
                publishModel.CreatedTime = DateTime.Now;
                publishModel.ModifiedTime = DateTime.Now;

                model.Content = ObjectSchemaList.ToString();
                model.ModifiedTime = DateTime.Now;


                //事务处理
                using (IDbConnection dbConnection = new SqlConnection(BaseApplication.DefaultSqlConnectionKey))
                {

                    dbConnection.Open();

                    IDbTransaction transaction = dbConnection.BeginTransaction();

                    int updateCount = 0;
                    try
                    {
                        updateCount += DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(CreateTableSql); //创建数据表

                        foreach (string item in createSonTableSqlDic)
                        {
                            updateCount += DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(item); //创建子表数据表
                        }

                        //int insertCount = _bizObjectSchemaDraftService.Insert<OT_BizObjectSchemaDraft>(publishModel);//发布版本

                        updateCount += _bizObjectSchemaService.UpdateTable<OT_BizObjectSchema>(model);

                        if (updateCount > 0)
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
                #endregion
            }

            return AjaxJson(result);
        }
        #endregion

        #region 流程台账
        /// <summary>
        /// 流程台账页面
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <returns></returns>

        public ActionResult ProcessAccount(string WorkflowCode)
        {
            List<Proc_Node> list = new List<Proc_Node>();

            list.AddRange(_procType.SelectData<Proc_Node>("select * from Proc_Node where ProcessCode='" + WorkflowCode + "' "));

            ViewData["nodeList"] = new JavaScriptSerializer().Serialize(list).ToString().Trim().ToLower();

            JArray nodeData = new JArray();

            int Branch = DapperHelper.CreateInstance().ExecuteScalar("select MAX(Branch) as Branch from Proc_Node where ProcessCode='" + WorkflowCode + "'");


            for (int i = 0; i < Branch; i++)
            {

                JArray branchData = new JArray();

                for (int j = 0; j < 6; j++)
                {


                    JObject columns = new JObject();

                    columns["cols"] = Convert.ToInt32(i + 1) + "_" + Convert.ToInt32(j + 1);

                    JArray rowData = new JArray();

                    List<Proc_Node> branchList = new List<Proc_Node>();

                    branchList = list.Where(c => c.Branch.Equals(i + 1)).Where(c => c.Column.Equals(j + 1)).OrderBy(c => c.Row).ToList();

                    foreach (var item in branchList)
                    {
                        JObject row = new JObject();
                        row["id"] = item.Branch + "_" + item.Column + "_" + item.Row;
                        row["objectid"] = item.ObjectId;
                        rowData.Add(row);
                    }

                    columns["rows"] = rowData;


                    branchData.Add(columns);

                }

                nodeData.Add(branchData);

            }

            ViewData["nodeData"] = nodeData.ToString();

            ViewData["processName"] = Request.Params["WorkFlowName"].ToString();
            if (list.Count > 0)
            {
                ViewData["Version"] = list[0].Version;
            }
            else
            {
                ViewData["Version"] = "1.0";
            }

            List<Proc_Node> procNodeSettingList = new List<Proc_Node>();

            string procNodeSettingListSql = string.Format(@"select ProcessName ,NodeName as Content,LinkName as [Type], ProcessVersion as [Version] from [{0}].[dbo].[BPMProcNodeSetting] where ProcessName='{1}' and [ProcessVersion]='{2}'", ConfigurationManager.AppSettings["BpmSysDbName"], Request.Params["WorkFlowName"].ToString(), ViewData["Version"]);

            procNodeSettingList = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMSYSDB"]).SimpleQuery<Proc_Node>(procNodeSettingListSql);

            List<Proc_Node> publishList = new List<Proc_Node>();

            if (procNodeSettingList.Count == 0)
            {
                list = list.OrderBy(c => c.Branch).OrderBy(c => c.Column).OrderBy(c => c.Row).ToList();

                foreach (var item in list)
                {
                    if (!publishList.Exists(c => c.Content == item.Content && c.Type == item.Type))
                    {
                        if (item.Type != "条件" && item.Type != "分支条件")
                        {
                            publishList.Add(item);
                        }

                    }
                }
            }
            else
            {
                foreach (var item in procNodeSettingList)
                {

                    publishList.Add(item);

                }
            }

            return View(publishList);
        }


        /// <summary>
        /// 流程节点增删改操作
        /// </summary>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public ActionResult EditProcessNode(List<Proc_Node> modelList, string EditType, string Version = "")
        {
            AjaxReturnData result = new AjaxReturnData();

            int EditCount = 0;

            bool publishApproveStepFlag = false;

            foreach (var item in modelList)
            {

                item.ProcessName = HttpUtility.UrlDecode(item.ProcessName);
                item.OldRow = item.Row;

                if (item.State == (int)NodeState.更新)
                {

                    if (EditType == "发布")
                    {
                        item.State = (int)NodeState.正常;

                        if (item.IsDelete)
                        {
                            Dictionary<string, object> deleteparam = new Dictionary<string, object>();
                            deleteparam.Add("ObjectId", item.ObjectId);
                            EditCount += _procNode.DeleteSigerData<Proc_Node>(deleteparam);
                        }
                        else
                        {
                            item.IsNewAdd = false;
                            item.IsRoleChange = false;
                            item.IsSortChange = false;
                            Dictionary<string, object> param = new Dictionary<string, object>();
                            param.Add("ObjectId", item.ObjectId);
                            //修改
                            EditCount += _procNode.UpdateTable<Proc_Node>(item, param);
                        }

                    }

                    if (item.IsNewAdd)
                    {
                        //新增

                        var hasList = _procNode.SelectData<Proc_Node>("select * from Proc_Node where ObjectId='" + item.ObjectId + "'");
                        if (hasList.Count == 0)
                        {
                            EditCount += _procNode.Insert<Proc_Node>(item);
                        }

                    }

                    if (item.IsRoleChange || item.IsSortChange || item.IsDelete)
                    {
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param.Add("ObjectId", item.ObjectId);
                        //修改
                        EditCount += _procNode.UpdateTable<Proc_Node>(item, param);
                    }

                }

                //更新版本
                if (EditType == "发布")
                {
                    if (!string.IsNullOrEmpty(Version))
                    {

                        item.Version = (Convert.ToSingle(Version) + 0.1).ToString("#0.0");
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param.Add("ObjectId", item.ObjectId);
                        //修改
                        EditCount += _procNode.UpdateTable<Proc_Node>(item, param);

                    }
                }

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
        /// 导入流程节点
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="processCode"></param>
        /// <param name="inputProcessCode"></param>
        /// <returns></returns>
        public ActionResult InputProcessNode(string processName, string processCode, string inputProcessCode, string version)
        {
            AjaxReturnData result = new AjaxReturnData();


            List<Proc_Node> list = new List<Proc_Node>();

            list.AddRange(_procType.SelectData<Proc_Node>("select * from Proc_Node where ProcessCode='" + inputProcessCode + "' "));

            foreach (var item in list)
            {
                item.ObjectId = Guid.NewGuid() + String.Empty;
                item.ProcessCode = processCode;
                item.ProcessName = processName;
                item.Version = version;
            }

            if (list.Count > 0)
            {
                int deleteResult = DapperHelper.CreateInstance().ExecuteNoneQuery("delete from [" + ConfigurationManager.AppSettings["LocalDbName"] + "].[dbo].[Proc_Node] where ProcessCode='" + processCode + "'");
            }

            int inputResult = DapperHelper.CreateInstance().InsertBatch<Proc_Node>("Proc_Node", list);

            if (inputResult > 0)
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
        /// 发布审批步骤
        /// </summary>
        /// <returns></returns>
        public ActionResult PublishApproveStep(List<Proc_Node> modelList)
        {

            AjaxReturnData result = new AjaxReturnData();

            StringBuilder valuesSql = new StringBuilder();

            foreach (var item in modelList)
            {
                valuesSql.Append("(");
                valuesSql.AppendFormat("'{0}','{1}', '{2}','{3}'", item.ProcessName, item.Content, item.Type, item.Version);
                valuesSql.Append("),");
            }

            valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);

            StringBuilder insertSql = new StringBuilder();
            insertSql.AppendFormat(@"INSERT INTO [{0}].[dbo].[BPMProcNodeSetting]
           ([ProcessName]--流程名称
           ,[NodeName]--节点名称
           ,[LinkName]--步骤类型
           ,[ProcessVersion])--版本号
     VALUES {1}", ConfigurationManager.AppSettings["BpmSysDbName"], valuesSql.ToString());


            int hasStepCount = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMSYSDB"]).ExecuteScalar("select  count(*) from [" + ConfigurationManager.AppSettings["BpmSysDbName"] + "].[dbo].[BPMProcNodeSetting] where   ProcessName='" + modelList[0].ProcessName + "' and  ProcessVersion='" + modelList[0].Version + "'");
            int resultCount = 0;


            if (hasStepCount > 0)
            {
                string deleteSql = "delete from [" + ConfigurationManager.AppSettings["BpmSysDbName"] + "].[dbo].[BPMProcNodeSetting] where ProcessName='" + modelList[0].ProcessName + "' and  ProcessVersion='" + modelList[0].Version + "'";

                int deleteCount = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMSYSDB"]).ExecuteNoneQuery(deleteSql);
            }

            resultCount = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMSYSDB"]).ExecuteNoneQuery(insertSql.ToString());


            if (resultCount > 0)
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
        /// 发布审批步骤sql
        /// </summary>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public ActionResult CreateApproveStepSql(List<Proc_Node> modelList)
        {

            AjaxReturnData result = new AjaxReturnData();

            StringBuilder valuesSql = new StringBuilder();

            foreach (var item in modelList)
            {
                valuesSql.Append("(");
                valuesSql.AppendFormat("'{0}','{1}', '{2}','{3}'", item.ProcessName, item.Content, item.Type, item.Version);
                valuesSql.Append("),");
            }

            valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);

            StringBuilder insertSql = new StringBuilder();
            insertSql.AppendFormat(@"INSERT INTO [{0}].[dbo].[BPMProcNodeSetting]
           ([ProcessName]--流程名称
           ,[NodeName]--节点名称
           ,[LinkName]--步骤类型
           ,[ProcessVersion])--版本号
     VALUES {1}", ConfigurationManager.AppSettings["BpmSysDbName"], valuesSql.ToString());

            int hasStepCount = DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMSYSDB"]).ExecuteScalar("select  count(*) from [" + ConfigurationManager.AppSettings["BpmSysDbName"] + "].[dbo].[BPMProcNodeSetting] where ProcessName='" + modelList[0].ProcessName + "' and   ProcessVersion='" + modelList[0].Version + "' \r\n");

            string createStepSql = "";

            if (hasStepCount > 0)
            {
                createStepSql += @"delete from [" + ConfigurationManager.AppSettings["BpmSysDbName"] + "].[dbo].[BPMProcNodeSetting] where ProcrssName='" + modelList[0].ProcessName + "' and  ProcessVersion='" + modelList[0].Version + "' ";


            }

            createStepSql += insertSql.ToString();

            result.States = true;
            result.Message = createStepSql;


            return AjaxJson(result);

        }

        #endregion

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
  FROM [Work6-OA].[dbo].[OT_WorkflowTemplate] where ObjectId='" + ObjectId + "' and IsDelete!='1'");
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

        /// <summary>
        /// 编辑流程模板配置
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ObjectID"></param>
        /// <param name="ProcessParentId"></param>
        /// <param name="ProcessName"></param>
        /// <param name="ProcessCode"></param>
        /// <returns></returns>
        public ActionResult EditWorkflowTemplate(int Action, string ObjectID, string ProcessParentId, string ProcessName, string ProcessCode, string ParentPropertyName, string Creator, string ModifiedBy, string CreatedTime, string ModifiedTime, string BizObjectSchemaCode, string BizObjectSchemaField, int Sort = 0)
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
                model.ParentPropertyName = ParentPropertyName;
                model.Creator = CookieHelper.GetCookie("systemUserName", "");
                model.ModifiedBy = CookieHelper.GetCookie("systemUserName", "");
                model.CreatedTime = DateTime.Now;
                model.ModifiedTime = DateTime.Now;
                model.BizObjectSchemaCode = BizObjectSchemaCode;
                model.BizObjectSchemaField = BizObjectSchemaField;
                model.Content = WorkflowHelper.CreateDefaultContent(ProcessCode);
                EditCount = _workflowTemplateService.Insert<OT_WorkflowTemplate>(model);
            }
            else if (Convert.ToInt32(EditType.修改) == Action)
            {
                List<OT_WorkflowTemplate> list = new List<OT_WorkflowTemplate>();

                list = _workflowTemplateService.SelectData<OT_WorkflowTemplate>("select * from OT_WorkflowTemplate where ObjectId='" + ObjectID + "' and IsDelete!='1'");

                if (list.Count > 0)
                {

                    OT_WorkflowTemplate model = new OT_WorkflowTemplate();

                    model = list[0];

                    model.ProcessName = ProcessName;

                    model.ParentObjectID = ProcessParentId;

                    model.Sort = Sort;

                    model.ParentPropertyName = ParentPropertyName;

                    model.ModifiedBy = CookieHelper.GetCookie("systemUserName", "");

                    model.ModifiedTime = DateTime.Now;

                    model.BizObjectSchemaCode = BizObjectSchemaCode;
                    model.BizObjectSchemaField = BizObjectSchemaField;

                    EditCount = _workflowTemplateService.UpdateTable<OT_WorkflowTemplate>(model);

                }

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
        #endregion

        #region 表单设计器

        public ActionResult DesignForm()
        {
            string ProcessCode = Request.Params["WorkFlowCode"].ToString();
            ProcessTableModel model = new ProcessTableModel();
            List<ProcessTableModel> list = _processTableModel.SelectData<ProcessTableModel>("select * from ProcessTableModel where ProcessCode='" + ProcessCode + "'");
            if (list.Count > 0)
            {
                model = list[0];
            }
            ViewData["processName"] = Request.Params["WorkFlowName"].ToString();


            return View(model);
        }


        #region 流程表单操作
        /// <summary>
        /// 保存流程表单数据
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProcessTableModel(ProcessTableModel model)
        {



            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            if (model.ObjectId == "" || model.ObjectId == null)
            {
                //新增
                model.ObjectId = Guid.NewGuid().ToString();

                EditCount = _processTableModel.Insert<ProcessTableModel>(model);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ObjectId", model.ObjectId);
                //修改
                EditCount = _processTableModel.UpdateTable<ProcessTableModel>(model, param);
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
        /// 导入流程表单数据
        /// </summary>
        /// <returns></returns>
        public ActionResult InputForm(string ObjectId, string ProcessCode, string ProcessName, string InputFormCode)
        {
            AjaxReturnData result = new AjaxReturnData();

            int EditCount = 0;

            List<ProcessTableModel> list = new List<ProcessTableModel>();

            list = _processTableModel.SelectData<ProcessTableModel>("select * from ProcessTableModel where ProcessCode='" + InputFormCode + "'");

            list[0].ProcessCode = ProcessCode;
            list[0].ProcessName = ProcessName;

            if (string.IsNullOrEmpty(ObjectId))
            {
                //新增
                list[0].ObjectId = Guid.NewGuid().ToString();
                list[0].ProcessCode = ProcessCode;
                list[0].ProcessName = ProcessName;
                EditCount = _processTableModel.Insert<ProcessTableModel>(list[0]);
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ObjectId", ObjectId);
                list[0].ObjectId = ObjectId;
                //修改
                EditCount = _processTableModel.UpdateTable<ProcessTableModel>(list[0], param);
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

        #endregion

        /// <summary>
        /// 修改流程属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult EditProcessAttr(ProcessTableModel model)
        {
            AjaxReturnData result = new AjaxReturnData();
            int EditCount = 0;
            if (!string.IsNullOrEmpty(model.ObjectId))
            {
                if (!string.IsNullOrEmpty(model.ServiceControl))
                {
                    //修改
                    EditCount = DapperHelper.CreateInstance().ExecuteNoneQuery("update ProcessTableModel set ServiceControl='" + model.ServiceControl + "' where ObjectId='" + model.ObjectId + "'");
                }
                else if (!string.IsNullOrEmpty(model.Explain))
                {
                    //修改
                    EditCount = DapperHelper.CreateInstance().ExecuteNoneQuery("update ProcessTableModel set Explain='" + model.Explain + "' where ObjectId='" + model.ObjectId + "'");
                }
                else if (!string.IsNullOrEmpty(model.AutorityControl))
                {
                    //修改
                    EditCount = DapperHelper.CreateInstance().ExecuteNoneQuery("update ProcessTableModel set AutorityControl='" + model.AutorityControl + "' where ObjectId='" + model.ObjectId + "'");
                }
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

        #endregion

    }
}
