using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 数据字典
    /// </summary>
   public class S_DataDictionary
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string TaskID { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 隐藏值、编码
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// 展示值
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
