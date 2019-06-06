using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 保存的数据模型
    /// </summary>
    public class OT_BizObjectSchema
    {
         public OT_BizObjectSchema()
        {
        }
        #region Model
        private string _objectid;
        private string _schemacode;
        private string _content;
        private string _displayname;
        private DateTime? _createdtime;
        private DateTime? _modifiedtime;
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
        public string SchemaCode
        {
            set
            {
                _schemacode = value;
            }
            get
            {
                return _schemacode;
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
        /// 
        /// </summary>
        public string DisplayName
        {
            set
            {
                _displayname = value;
            }
            get
            {
                return _displayname;
            }
        }
        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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

        public bool IsShareTable { get; set; }
        public string ShareTableCode { get; set; }

        public string DbName { get; set; }

        #endregion Model
    }
}
