using CommonTool.DBHelper;
using Domain;
using IService;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace DapperMvc.WebServices
{
    /// <summary>
    /// DataDictionaryWS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class DataDictionaryWS : System.Web.Services.WebService
    {

        private static readonly ILog _logger = LogManager.GetLogger(typeof(DataDictionaryWS));


        private IProc_NodeService _procNode;
        private IProc_Type _procType;
        private IOT_WorkflowTemplateDraftService _workflowTemplateDraftService;
        private IOT_WorkflowTemplateService _workflowTemplateService;
        private IOT_BizObjectSchemaDraftService _bizObjectSchemaDraftService;
        private IOT_BizObjectSchemaService _bizObjectSchemaService;

        public DataDictionaryWS()
        {

        }

        public DataDictionaryWS(IProc_NodeService procNode, IProc_Type procType, IOT_WorkflowTemplateDraftService workflowTemplateDraftService, IOT_BizObjectSchemaDraftService bizObjectSchemaDraftService, IOT_BizObjectSchemaService bizObjectSchemaService, IOT_WorkflowTemplateService workflowTemplateService)
        {
            this._procNode = procNode;
            this._procType = procType;
            this._workflowTemplateDraftService = workflowTemplateDraftService;
            this._workflowTemplateService = workflowTemplateService;
            this._bizObjectSchemaDraftService = bizObjectSchemaDraftService;
            this._bizObjectSchemaService = bizObjectSchemaService;
        }

        [WebMethod]
        public string HelloWorld(string name)
        {
            return name + ":Hello World";
        }


        /// <summary>
        /// 测试用例
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [WebMethod]
        public Proc_Node GetStringDemo(string code, string value)
        {
            Proc_Node data = new Proc_Node();

            data.Branch = 1;
            data.Column = 1;
            data.Content = value;
            data.Id = code;

            return data;

        }


        /// <summary>
        /// 获取流程名称
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        [WebMethod]
        public List<OT_WorkflowTemplate> GetProcessList(string processName = "")
        {

            //_logger.Info("流程名称：" + processName + ",");

            string sql = "";

            if (string.IsNullOrEmpty(processName.Trim()))
            {
                sql = "select * from OT_WorkflowTemplate where IsDelete!='1'";
            }
            else
            {
                sql = "select * from OT_WorkflowTemplate where ProcessName like '%" + processName + "%' and IsDelete!='1'";
            }

            List<OT_WorkflowTemplate> list = DapperHelper.CreateInstance().SimpleQuery<OT_WorkflowTemplate>(sql);

            //_logger.Info("流程数量：" + list.Count);

            return list;
        }



    }
}
