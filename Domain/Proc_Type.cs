using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 流程分类表
    /// </summary>
    public class Proc_Type
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string ProcessType { get; set; }

        /// <summary>
        /// 父亲文件夹id
        /// </summary>
        public string ParentObjectId { get; set; }


        /// <summary>
        /// 级别
        /// </summary>
        public string LevelName { get; set; }


        /// <summary>
        /// 属性
        /// </summary>
        public string Attribute { get; set; }


        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

    }
}
