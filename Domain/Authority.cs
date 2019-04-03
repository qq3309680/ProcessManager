using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class Authority
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 角色ObjectId 
        /// 注：多角色，以半角逗号分隔
        /// </summary>
        public string RoleObjectIds { get; set; }

        /// <summary>
        /// 角色名称
        /// 注：多角色，以半角逗号分隔
        /// </summary>
        public string RoleNames { get; set; }

        /// <summary>
        /// 权限类型 
        /// </summary>
        public string AuthorityType { get; set; }

        /// <summary>
        /// 是否有该权限
        /// </summary>
        public bool HasAuth { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// 系统编码
        /// </summary>
        public string SystemCode { get; set; }
        /// <summary>
        /// 页面Url
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string ButtonName { get; set; }

        /// <summary>
        /// 按钮编码
        /// </summary>
        public string ButtonCode { get; set; }
        
    }
}
