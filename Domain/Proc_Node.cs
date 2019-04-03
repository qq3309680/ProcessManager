using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [Serializable]
    /// <summary>
    /// 流程节点
    /// </summary>
    public class Proc_Node
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 分支
        /// </summary>
        public int Branch { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        public int Row { get; set; }


        /// <summary>
        /// 变更前所在行
        /// </summary>
        public int OldRow { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string ProcessCode { get; set; }


        /// <summary>
        /// 节点状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 是否新增
        /// </summary>
        public bool IsNewAdd { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }


        /// <summary>
        /// 是否角色名称改变
        /// </summary>
        public bool IsRoleChange { get; set; }

        /// <summary>
        /// 是否顺序改变
        /// </summary>
        public bool IsSortChange { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

    }
}
