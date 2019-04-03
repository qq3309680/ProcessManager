using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 流程表单模型
    /// </summary>
    public class ProcessTableModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 流程编码
        /// </summary>
        public string ProcessCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 主表数据
        /// </summary>
        public string MainFormData { get; set; }
        /// <summary>
        /// 子表数据
        /// </summary>
        public string ChildFormData { get; set; }

        /// <summary>
        /// 业务管控点
        /// </summary>
        public string ServiceControl { get; set; }

        /// <summary>
        /// 流程说明
        /// </summary>
        public string Explain { get; set; }

        /// <summary>
        /// 权限管控
        /// </summary>
        public string AutorityControl { get; set; }
    }
}
