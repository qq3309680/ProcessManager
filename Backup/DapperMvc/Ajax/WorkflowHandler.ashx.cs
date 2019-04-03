using CommonTool;
using CommonTool.DBHelper;
using CommonTool.Workflow;
using Domain;
using IService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

namespace DapperMvc.Ajax
{
    /// <summary>
    /// WorkflowHandler 的摘要说明
    /// </summary>
    public class WorkflowHandler : IHttpHandler
    {
        HttpContext CurrentContext;
        System.Collections.Specialized.NameValueCollection CurrentParams;

        public void ProcessRequest(HttpContext context)
        {
            CurrentContext = context;
            CurrentParams = CurrentContext.Request.Params;

            //if (userValidator == null)
            //{
            //    WriteResponse("PortalSessionOut");
            //    return;
            //}
            //this._UserID = userValidator.UserID;

            try
            {

                string command = CurrentContext.Request["Command"];
                switch (command)
                {
                    case "SaveWorkflow"://获取业务属性
                        SaveWorkflow();
                        break;
                    case "UpdateClauseName":
                        UpdateClauseName();
                        break;
                    case "ValidateWorkflow":
                        ValidateWorkflow();
                        break;
                    case "PublishWorkflow":
                        PublishWorkflow();
                        break;
                    case "GetWorkflow":
                        GetWorkflow();
                        break;
                    case "ExportWorkflow":
                        ExportWorkflow();
                        break;
                    case "SaveActivityTemplate":
                        SaveActivityTemplate();
                        break;

                    //获取可映射的数据项列表
                    case "GetDataItemsByWorkflowCode":
                        GetDataItemsByWorkflowCode();
                        break;
                    case "GetDataItems":
                        GetDataItems();
                        break;

                    case "GetParticipants":
                        GetParticipants();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteResponse(false, new List<string>() { ex.Message, ex.StackTrace }, null);
            }
        }

        private string _UserID = string.Empty;
        /// <summary>
        /// 获取当前用户的ID
        /// </summary>
        protected string UserID
        {
            get
            {
                return this._UserID;
            }
        }

        #region 序列化器

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private JavaScriptSerializer _JsSerializer = null;
        /// <summary>
        /// 获取JS序列化对象
        /// </summary>
        private JavaScriptSerializer JSSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new JavaScriptSerializer();
                    _JsSerializer.MaxJsonLength = Int32.MaxValue;
                }
                return _JsSerializer;
            }
        }

        /// <summary>
        /// 写响应
        /// </summary>
        void WriteResponse(object responseContext)
        {
            CurrentContext.Response.Write(JSSerializer.Serialize(responseContext));
        }

        /// <summary>
        /// 输出处理结果 
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="Message"></param>
        void WriteResponse(bool Result, string Message)
        {
            WriteResponse(new
            {
                Result = Result,
                Message = Message
            });
        }

        /// <summary>
        /// 输出处理结果 
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="Errors"></param>
        /// <param name="Warnings"></param>
        void WriteResponse(bool Result, List<string> Errors, List<string> Warnings)
        {
            WriteResponse(new
            {
                Result = Result,
                Errors = Errors,
                Warnings = Warnings
            });
        }

        int ParseToInt(object obj)
        {
            decimal value = 0;
            decimal.TryParse(obj + string.Empty, out value);
            return (int)value;
        }

        #endregion

        #region 流程模板设计管理器

        //IWorkflowConfigManager _WorkflowDraftManager;
        //IWorkflowConfigManager WorkflowDraftManager
        //{
        //    get
        //    {
        //        if (this._WorkflowDraftManager == null)
        //            this._WorkflowDraftManager = this.Engine.WorkflowConfigManager;
        //        return this._WorkflowDraftManager;
        //    }
        //}

        #endregion

        #region 流程模板管理器

        //IWorkflowManager _WorkflowManager;
        //IWorkflowManager WorkflowManager
        //{
        //    get
        //    {
        //        if (this._WorkflowManager == null)
        //            this._WorkflowManager = this.Engine.WorkflowManager;
        //        return this._WorkflowManager;
        //    }
        //}

        #endregion

        //OThinker.H3.DataModel.IBizObjectManager _BizObjectManager;
        //OThinker.H3.DataModel.IBizObjectManager BizObjectManager
        //{
        //    get
        //    {
        //        if (this._BizObjectManager == null)
        //            this._BizObjectManager = this.Engine.BizObjectManager;
        //        return this._BizObjectManager;
        //    }
        //}


        #region 从Context中获取流程模板内容

        /// <summary>
        /// 读取模板信息
        /// </summary>
        /// <returns></returns>
        WorkflowTemplate ReadRequestWorkflowTemplate()
        {
            string WorkflowString = HttpUtility.HtmlDecode(CurrentParams["WorkflowTemplate"]);
            //读取活动模板定义信息
            WorkflowTemplate workflowTemplate = (WorkflowTemplate)JSSerializer.Deserialize(WorkflowString, typeof(WorkflowTemplate));
            if (workflowTemplate == null)
                return null;
            return workflowTemplate;
        }

        #endregion

        #region 保存流程模板

        /// <summary>
        /// 保存流程模板
        /// </summary>
        void SaveWorkflow()
        {
            WorkflowTemplate workflowTemplate = ReadRequestWorkflowTemplate();
            XMLHelper xmlHelper = new XMLHelper();
            XmlDocument xmlDocument = xmlHelper.CreateXmlRoot("Workflow");
            Type type = typeof(WorkflowTemplate);
            PropertyInfo[] props = type.GetProperties();
            foreach (var item in props)
            {
                switch (item.Name)
                {
                    case "Activities":
                        xmlHelper.CreatXmlNode("Workflow", "Activities", "");
                        foreach (var activtyItem in workflowTemplate.Activities)
                        {
                            XmlElement activtyEle = xmlDocument.CreateElement(((ActivityType)activtyItem.ActivityType) + string.Empty + "Activity");
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "ActivityType", ((ActivityType)activtyItem.ActivityType) + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Text", activtyItem.DisplayName));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "ActivityCode", activtyItem.ActivityCode));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "SortKey", activtyItem.SortKey + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Description", activtyItem.Description));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Height", activtyItem.Height + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Width", activtyItem.Width + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "X", activtyItem.X + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Y", activtyItem.Y + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "FontSize", activtyItem.FontSize + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Brush", activtyItem.FontColor));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "EntryCondition", activtyItem.EntryCondition));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "ID", activtyItem.ID + string.Empty));
                            activtyEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Participants", activtyItem.Participants + string.Empty));
                            xmlDocument.SelectSingleNode("Workflow").SelectSingleNode("Activities").AppendChild(activtyEle);
                        }

                        break;
                    case "Rules":
                        xmlHelper.CreatXmlNode("Workflow", "Rules", "");
                        foreach (var ruleItem in workflowTemplate.Rules)
                        {
                            XmlElement rule = xmlDocument.CreateElement("Rule");
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Text", ruleItem.DisplayName));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "PreActivityCode", ruleItem.PreActivityCode));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "PostActivityCode", ruleItem.PostActivityCode));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "UtilizeElse", ruleItem.UtilizeElse + string.Empty));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Formula", ruleItem.Formula));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "FontSize", ruleItem.FontSize + string.Empty));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "FontColor", ruleItem.FontColor));
                            rule.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "ID", ruleItem.ID));
                            XmlElement pointEle = xmlDocument.CreateElement("Points");
                            foreach (var pointItem in ruleItem.Points)
                            {
                                pointEle.AppendChild(XMLHelper.CreateXmlNode(xmlDocument, "Point", pointItem));
                            }
                            rule.AppendChild(pointEle);
                            xmlDocument.SelectSingleNode("Workflow").SelectSingleNode("Rules").AppendChild(rule);
                        }
                        break;
                    case "WorkflowCode":
                    case "InstanceName":
                    case "Creator":
                    case "ModifiedBy":
                    case "CreatedTime":
                    case "ModifiedTime":
                    case "Description":
                    case "Height":
                    case "Width":
                    case "BizObjectSchemaCode":
                        xmlHelper.CreatXmlNode("Workflow", item.Name, item.GetValue(workflowTemplate) + string.Empty);
                        break;
                    default:
                        break;
                }

            }

            string content = xmlDocument.InnerXml;
            List<OT_WorkflowTemplate> modelList = DapperHelper.CreateInstance().SimpleQuery<OT_WorkflowTemplate>(@"SELECT [ObjectID]
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
  FROM [Work6-OA].[dbo].[OT_WorkflowTemplate] where WorkflowCode='" + workflowTemplate.WorkflowCode + "'");
            OT_WorkflowTemplate model = modelList[0];
            model.Content = content;
            int updateCount = DapperHelper.CreateInstance().UpdateTable<OT_WorkflowTemplate>("OT_WorkflowTemplate", model);
            if (updateCount>0)
            {
                  WriteResponse(true, null, null);
            }
            else
            {
                WriteResponse(false, null, null);
            }
        }

        #endregion

        void UpdateClauseName()
        {

        }

        #region 验证流程模板
        /// <summary>
        /// 验证流程模板是否合法
        /// </summary>
        /// <param name="DraftWorkflowTemplate"></param>
        /// <returns></returns>
        void ValidateWorkflow()
        {


        }
        #endregion

        #region 发布流程模板

        /// <summary>
        /// 发布流程模板
        /// </summary>
        void PublishWorkflow()
        {

        }

        #endregion

        #region 获取流程模板

        /// <summary>
        /// 获取流程模板
        /// </summary>
        void GetWorkflow()
        {

        }
        #endregion

        #region 导出流程模板

        /// <summary>
        /// 导出流程模板
        /// </summary>
        void ExportWorkflow()
        {

        }

        #endregion

        #region 保存活动模板

        /// <summary>
        /// 保存活动模板
        /// </summary>
        void SaveActivityTemplate()
        {

        }

        #endregion

        /// <summary>
        /// 获取数据项
        /// </summary>
        void GetDataItems()
        {

        }

        /// <summary>
        /// 根据流程编码获取可映射的数据项
        /// </summary>
        void GetDataItemsByWorkflowCode()
        {

        }

        void GetParticipants()
        {

        }

        /// <summary>
        /// 输出数据模型
        /// </summary>
        /// <param name="SchemaCode"></param>
        void WriteSchema(string SchemaCode)
        {

        }
    }
}