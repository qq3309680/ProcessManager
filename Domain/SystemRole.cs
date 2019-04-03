using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public class SystemRole
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色说明
        /// </summary>
        public string RoleExplain { get; set; }



    }
}
