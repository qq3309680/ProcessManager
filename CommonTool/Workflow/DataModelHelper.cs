using BaseEnvironment;
using CommonTool.DBHelper;
using Domain;
using IService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTool.Workflow
{
    /// <summary>
    /// 数据模型帮助类
    /// </summary>
    public class DataModelHelper
    {
        /// <summary>
        /// 增加主表数据字段
        /// </summary>
        /// <param name="FieldJArray">主表字段数组</param>
        /// <param name="Model">主表数据模型</param>
        /// <param name="AddTarget">新增目标字段</param>
        /// <param name="Service">数据保存服务</param>
        /// <returns></returns>
        public static AjaxReturnData AddMainTableField(string AddTarget, JArray FieldJArray, OT_BizObjectSchema Model, IOT_BizObjectSchemaService Service)
        {

            AjaxReturnData result = new AjaxReturnData();

            result.States = true;

            bool hasField = false;

            JArray newField = JArray.Parse(AddTarget);

            foreach (JObject item in FieldJArray)
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
                FieldJArray.Add(newField[0]);

                Model.Content = FieldJArray.ToString();

                int updateCount = Service.UpdateTable<OT_BizObjectSchema>(Model);

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

            return result;

        }

        /// <summary>
        /// 添加子表数据字段
        /// </summary>
        /// <param name="FieldJArray">主表字段数组</param>
        /// <param name="Model">主表数据模型</param>
        /// <param name="AddTarget">新增目标字段</param>
        /// <param name="Service">数据保存服务</param>
        /// <returns></returns>
        public static AjaxReturnData AddSonTableField(string AddTarget, JArray FieldJArray, OT_BizObjectSchema Model, IOT_BizObjectSchemaService Service)
        {

            AjaxReturnData result = new AjaxReturnData();

            result.States = true;

            bool hasField = false;

            JArray newField = JArray.Parse(AddTarget);

            foreach (JObject item in FieldJArray)
            {
                if (item["ObjectId"] + string.Empty == newField[0]["ParentProperty"] + string.Empty)
                {
                    item["IsPublished"] = false;

                    if (!string.IsNullOrEmpty(item["children"] + string.Empty))
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

                            Model.Content = FieldJArray.ToString();

                            int updateCount = Service.UpdateTable<OT_BizObjectSchema>(Model);

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
                        //添加第一个子表数据
                        JArray children = new JArray();

                        children.Add(newField[0]);

                        item["children"] = children;

                        Model.Content = FieldJArray.ToString();

                        int updateCount = Service.UpdateTable<OT_BizObjectSchema>(Model);

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

            return result;

        }


        /// <summary>
        /// 删除表数据字段
        /// </summary>
        /// <param name="ObjectId">被删除字段ObjectId</param>
        /// <param name="FieldJArray">主表字段数组</param>
        /// <param name="Model">主表数据模型</param>
        /// <param name="Service">数据保存服务</param>
        /// <returns></returns>
        public static AjaxReturnData DeleteTableField(string MainTableCode, string ObjectId, bool IsSonTable, string ParentProperty, JArray FieldJArray, OT_BizObjectSchema Model, IOT_BizObjectSchemaService Service)
        {

            AjaxReturnData result = new AjaxReturnData();

            result.States = true;

            if (IsSonTable)
            {
                foreach (var item in FieldJArray)
                {
                    if (item["ObjectId"] + string.Empty == ParentProperty + string.Empty)
                    {
                        foreach (JObject childItem in (JArray)item["children"])
                        {
                            if (childItem["ObjectId"] + string.Empty == ObjectId + string.Empty)
                            {
                                if ((bool)childItem["IsPublished"] == true)
                                {
                                    if (((JArray)item["children"]).Count == 1)
                                    {
                                        #region 删除数据表
                                        string deleteTableFieldSql = "drop table C_" + item["FieldCode"];
                                        DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(deleteTableFieldSql);
                                        #endregion
                                        FieldJArray.Remove(item);
                                    }
                                    else
                                    {
                                        #region 删除数据表字段
                                        string deleteTableFieldSql = "alter table C_" + item["FieldCode"] + " drop column " + childItem["FieldCode"].ToString();
                                        DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(deleteTableFieldSql);
                                        #endregion
                                    }
                                }

                                ((JArray)item["children"]).Remove(childItem);

                                break;
                            }
                        }
                        break;
                    }

                }
            }
            else
            {
                foreach (JObject item in FieldJArray)
                {
                    if (item["ObjectId"] + string.Empty == ObjectId + string.Empty)
                    {
                        if ((bool)item["IsPublished"] == true)
                        {
                            if (item["LogicType"] + string.Empty == "BizObjectArray")
                            {
                                //子表
                                #region 删除数据表
                                string deleteTableFieldSql = "drop table C_" + item["FieldCode"];
                                DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(deleteTableFieldSql);
                                #endregion
                            }
                            else
                            {
                                #region 删除数据表字段
                                string deleteTableFieldSql = "alter table I_" + MainTableCode + " drop column " + item["FieldCode"].ToString();
                                DapperHelper.CreateInstance(ConfigurationManager.AppSettings["SqlConnectionKey_BPMDB"]).ExecuteNoneQuery(deleteTableFieldSql);
                                #endregion
                            }
                        }


                        FieldJArray.Remove(item);

                        break;
                    }
                }
            }

            Model.Content = FieldJArray.ToString();

            int updateCount = Service.UpdateTable<OT_BizObjectSchema>(Model);

            if (updateCount > 0)
            {
                result.Message = "更新成功.";
            }
            else
            {
                result.States = false;

                result.ErrorMessage = "更新失败.";
            }


            return result;
        }




    }
}
