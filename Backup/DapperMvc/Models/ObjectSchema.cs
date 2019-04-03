using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DapperMvc.Models
{
    /// <summary>
    /// 数据模型字段类
    /// </summary>
    public class ObjectSchema
    {
        public string ObjectId
        {
            get;
            set;
        }
        public string ParentProperty
        {
            get;
            set;
        }
        public string DisplayName
        {
            get;
            set;
        }
        public string FieldCode
        {
            get;
            set;
        }
        public string LogicType
        {
            get;
            set;
        }
        public bool IsPublished
        {
            get;
            set;
        }
        public bool IsSonTable
        {
            get;
            set;
        }

        public bool IsSystemField
        {
            get;
            set;
        }
    }
}