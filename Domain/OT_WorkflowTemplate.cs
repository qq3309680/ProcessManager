using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OT_WorkflowTemplate
    {
        public OT_WorkflowTemplate()
        {
        }
        #region Model
        private string _objectid;
        private string _content;
        private string _creator;
        private string _modifiedby;
        private DateTime? _createdtime;
        private DateTime? _modifiedtime;
        private string _workflowcode;
        private string _bizobjectschemacode;
        private string _parentobjectid;
        private string _parentpropertyname;
        private int? _parentindex;
        /// <summary>
        /// 
        /// </summary>
        public string ObjectID
        {
            set
            {
                _objectid = value;
            }
            get
            {
                return _objectid;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Content
        {
            set
            {
                _content = value;
            }
            get
            {
                return _content;
            }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            set
            {
                _creator = value;
            }
            get
            {
                return _creator;
            }
        }
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy
        {
            set
            {
                _modifiedby = value;
            }
            get
            {
                return _modifiedby;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime
        {
            set
            {
                _createdtime = value;
            }
            get
            {
                return _createdtime;
            }
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedTime
        {
            set
            {
                _modifiedtime = value;
            }
            get
            {
                return _modifiedtime;
            }
        }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string WorkflowCode
        {
            set
            {
                _workflowcode = value;
            }
            get
            {
                return _workflowcode;
            }
        }
        /// <summary>
        /// 标签id
        /// </summary>
        public string BizObjectSchemaCode
        {
            set
            {
                _bizobjectschemacode = value;
            }
            get
            {
                return _bizobjectschemacode;
            }
        }
        /// <summary>
        /// 上级id
        /// </summary>
        public string ParentObjectID
        {
            set
            {
                _parentobjectid = value;
            }
            get
            {
                return _parentobjectid;
            }
        }
        /// <summary>
        /// 上级名称
        /// </summary>
        public string ParentPropertyName
        {
            set
            {
                _parentpropertyname = value;
            }
            get
            {
                return _parentpropertyname;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ParentIndex
        {
            set
            {
                _parentindex = value;
            }
            get
            {
                return _parentindex;
            }
        }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string BizObjectSchemaField
        {
            get;
            set;
        }

       /// <summary>
       /// 流程名称
       /// </summary>
        public string ProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }


        #endregion Model

    }
}
