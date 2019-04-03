using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommonTool.Workflow
{
    /// <summary>
    /// Author : 谭振
    /// DateTime : 2017/12/11 15:39:20
    /// Mail : tanz01@haid.com.cn
    /// Description : 流程模板工具类
    /// </summary>
    public class WorkflowHelper
    {
        /// <summary>
        /// 获取节点集合
        /// </summary>
        /// <param name="source">xml数据源</param>
        /// <param name="xPath">节点路劲</param>
        /// <returns></returns>
        public static List<ActivityModel> GetActivities(XmlNode source, string xPath)
        {
            List<ActivityModel> activites = new List<ActivityModel>();
            XmlNodeList ApproveActivityList = XMLHelper.GetNodeList(source, xPath);
            try
            {
                foreach (XmlNode activity in ApproveActivityList)
                {
                    ActivityModel activityModel = new ActivityModel();
                    activityModel.ActivityType = (int)(ActivityType)Enum.Parse(typeof(ActivityType), XMLHelper.GetNodeValue(activity, "ActivityType"), false);
                    //activityModel.ClassName = XMLHelper.GetNodeValue(activity, "ClassName");
                    activityModel.DisplayName = XMLHelper.GetNodeValue(activity, "Text");
                    //activityModel.ToolTipText = XMLHelper.GetNodeValue(activity, "ActivityType");
                    activityModel.ActivityCode = XMLHelper.GetNodeValue(activity, "ActivityCode");
                    activityModel.SortKey = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "SortKey"));
                    activityModel.Description = XMLHelper.GetNodeValue(activity, "Description");
                    activityModel.Height = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "Height"));
                    activityModel.Width = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "Width"));
                    activityModel.X = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "X"));
                    activityModel.Y = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "Y"));
                    //activityModel.IsActivity = true;
                    //activityModel.IsEnd = false;
                    //activityModel.IsApproveActivity = true;
                    //activityModel.FullName = XMLHelper.GetNodeValue(activity, "ActivityCode") + " " + XMLHelper.GetNodeValue(activity, "Text");
                    activityModel.FontSize = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "FontSize"));
                    activityModel.EntryCondition = XMLHelper.GetNodeValue(activity, "EntryCondition");
                    activityModel.FontColor = XMLHelper.GetNodeValue(activity, "Brush");
                    activityModel.ID = Convert.ToInt32(XMLHelper.GetNodeValue(activity, "ID"));
                    activityModel.Participants = XMLHelper.GetNodeValue(activity, "Participants");
                    activites.Add(activityModel);
                }
            }
            catch (Exception e)
            {

                throw;
            }

            return activites;
        }

        /// <summary>
        /// 获得连接规则
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<LineRuleModel> GetRules(XmlNode source)
        {
            List<LineRuleModel> rules = new List<LineRuleModel>();
            XmlNodeList ruleList = XMLHelper.GetNodeList(source, "Rules/Rule");
            foreach (XmlNode rule in ruleList)
            {
                LineRuleModel ruleModel = new LineRuleModel();
                ruleModel.DisplayName = XMLHelper.GetNodeValue(rule, "Text");
                //ruleModel.IsRule = true;
                ruleModel.PreActivityCode = XMLHelper.GetNodeValue(rule, "PreActivityCode");
                ruleModel.PostActivityCode = XMLHelper.GetNodeValue(rule, "PostActivityCode");
                ruleModel.UtilizeElse = Convert.ToBoolean(XMLHelper.GetNodeValue(rule, "UtilizeElse"));
                //ruleModel.FixedPoint = Convert.ToBoolean(XMLHelper.GetNodeValue(rule, "FixedPoint"));
                ruleModel.Formula = XMLHelper.GetNodeValue(rule, "Formula");
                //ruleModel.TextX = XMLHelper.GetNodeValue(rule, "TextX");
                //ruleModel.TextY = XMLHelper.GetNodeValue(rule, "TextX");
                XmlNodeList pointList = XMLHelper.GetNodeList(rule, "Points/Point");
                List<string> points = new List<string>();
                foreach (XmlNode item in pointList)
                {
                    points.Add(item.InnerText);
                }
                ruleModel.Points = points;
                ruleModel.FontSize = Convert.ToInt32(XMLHelper.GetNodeValue(rule, "FontSize"));
                ruleModel.FontColor = XMLHelper.GetNodeValue(rule, "FontColor");
                ruleModel.ID = XMLHelper.GetNodeValue(rule, "ID");
                rules.Add(ruleModel);
            }
            return rules;
        }


        /// <summary>
        /// 获得流程模型模板
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static WorkflowTemplate GetWorkflowTemplate(OT_WorkflowTemplateDraft model)
        {

            XmlNode WorkflowPackage = XMLHelper.GetRootNode(model.Content);
            WorkflowTemplate workflowTemplate = new WorkflowTemplate();
            workflowTemplate.WorkflowCode = XMLHelper.GetNodeValue(WorkflowPackage, "WorkflowCode");
            workflowTemplate.InstanceName = XMLHelper.GetNodeValue(WorkflowPackage, "InstanceName");
            workflowTemplate.Creator = model.Creator;
            workflowTemplate.ModifiedBy = model.ModifiedBy;
            workflowTemplate.CreatedTime = Convert.ToString(model.CreatedTime);
            workflowTemplate.ModifiedTime = Convert.ToString(model.ModifiedTime);
            workflowTemplate.Description = XMLHelper.GetNodeValue(WorkflowPackage, "Description");
            workflowTemplate.Height = 0;
            workflowTemplate.Width = 0;
            workflowTemplate.BizObjectSchemaCode = XMLHelper.GetNodeValue(WorkflowPackage, "BizObjectSchemaCode");
            List<ActivityModel> activites = new List<ActivityModel>();

            #region 获取开始节点
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/StartActivity"));
            #endregion
            #region 获取结束节点
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/EndActivity"));
            #endregion
            #region 获取手工节点
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/FillSheetActivity"));
            #endregion
            #region 获取传阅节点
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/CirculateActivity"));
            #endregion
            #region 获取连接点节点
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/ConnectionActivity"));
            #endregion
            #region 获取业务动作节点
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/BizActionActivity"));
            #endregion
            #region 获取审批节点集合
            activites.AddRange(GetActivities(WorkflowPackage, "Activities/ApproveActivity"));
            #endregion

            workflowTemplate.Activities = activites;

            workflowTemplate.Rules = GetRules(WorkflowPackage);
            return workflowTemplate;
        }

        /// <summary>
        /// 创建默认的模板Xml
        /// </summary>
        /// <returns></returns>
        public static string CreateDefaultContent(string WorkflowCode)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<Workflow>");
            result.Append("<WorkflowCode>" + WorkflowCode + "</WorkflowCode>");
            result.Append(@"  <InstanceName />
              <Creator>18f923a7-5a5e-426d-94ae-a55ad1a4b239</Creator>
              <ModifiedBy>18f923a7-5a5e-426d-94ae-a55ad1a4b239</ModifiedBy>
              <CreatedTime />
              <ModifiedTime />
              <Description />
              <Height>0</Height>
              <Width>0</Width>");
            result.Append("<BizObjectSchemaCode>" + WorkflowCode + "</BizObjectSchemaCode>");

            result.Append(@"<Activities>
                <StartActivity>
                  <ActivityType>Start</ActivityType>
                  <Text>开始节点</Text>
                  <ActivityCode>Activity1</ActivityCode>
                  <SortKey>1</SortKey>
                  <Description />
                  <Height>40</Height>
                  <Width>144</Width>
                  <X>337</X>
                  <Y>98</Y>
                  <FontSize>15</FontSize>
                  <Brush>blue</Brush>
                  <EntryCondition />
                  <ID>1</ID>
                  <Participants />
                </StartActivity>
                <EndActivity>
                  <ActivityType>End</ActivityType>
                  <Text>结束节点</Text>
                  <ActivityCode>Activity4</ActivityCode>
                  <SortKey>1</SortKey>
                  <Description />
                  <Height>40</Height>
                  <Width>144</Width>
                  <X>337</X>
                  <Y>422</Y>
                  <FontSize>15</FontSize>
                  <Brush>black</Brush>
                  <EntryCondition>Any</EntryCondition>
                  <ID>4</ID>
                  <Participants />
                </EndActivity>
                <FillSheetActivity>
                  <ActivityType>FillSheet</ActivityType>
                  <Text>提交申请</Text>
                  <ActivityCode>Activity2</ActivityCode>
                  <SortKey>1</SortKey>
                  <Description />
                  <Height>40</Height>
                  <Width>144</Width>
                  <X>337</X>
                  <Y>206</Y>
                  <FontSize>15</FontSize>
                  <Brush>blue</Brush>
                  <EntryCondition>Any</EntryCondition>
                  <ID>2</ID>
                  <Participants>{Originator}</Participants>
                </FillSheetActivity>
                <ApproveActivity>
                  <ActivityType>Approve</ActivityType>
                  <Text>审批</Text>
                  <ActivityCode>Activity2021</ActivityCode>
                  <SortKey>1</SortKey>
                  <Description />
                  <Height>40</Height>
                  <Width>144</Width>
                  <X>337</X>
                  <Y>314</Y>
                  <FontSize>18</FontSize>
                  <Brush>blue</Brush>
                  <EntryCondition />
                  <ID>2021</ID>
                  <Participants />
                </ApproveActivity>
              </Activities>
              <Rules>
                <Rule>
                  <Text />
                  <PreActivityCode>Activity1</PreActivityCode>
                  <PostActivityCode>Activity2</PostActivityCode>
                  <UtilizeElse>False</UtilizeElse>
                  <Formula />
                  <FontSize>11</FontSize>
                  <FontColor>black</FontColor>
                  <ID>5</ID>
                  <Points>
                    <Point>337.01171875,118.48828125</Point>
                    <Point>337.01171875,152.48774857954544</Point>
                    <Point>337,152.48774857954544</Point>
                    <Point>337,186.4872159090909</Point>
                  </Points>
                </Rule>
                <Rule>
                  <Text />
                  <PreActivityCode>Activity2</PreActivityCode>
                  <PostActivityCode>Activity2021</PostActivityCode>
                  <UtilizeElse>False</UtilizeElse>
                  <Formula />
                  <FontSize>0</FontSize>
                  <FontColor />
                  <ID>20231</ID>
                  <Points>
                    <Point>337,226.4765625</Point>
                    <Point>337,294.484375</Point>
                  </Points>
                </Rule>
                <Rule>
                  <Text />
                  <PreActivityCode>Activity2021</PreActivityCode>
                  <PostActivityCode>Activity4</PostActivityCode>
                  <UtilizeElse>False</UtilizeElse>
                  <Formula />
                  <FontSize>0</FontSize>
                  <FontColor />
                  <ID>20232</ID>
                  <Points>
                    <Point>337,334.484375</Point>
                    <Point>337,402.4921875</Point>
                  </Points>
                </Rule>
              </Rules>
            </Workflow>");
            return result.ToString();
        }
    }
}
