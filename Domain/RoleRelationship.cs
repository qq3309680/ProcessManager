using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 角色用户映射表
    /// </summary>
   public class RoleRelationship
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 角色ObjectId 
        /// </summary>
        public string RoleObjectId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 用户ObjectId 
        /// </summary>
        public string UserObjectId { get; set; }


        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

    }
}
