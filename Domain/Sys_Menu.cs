using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    public class Sys_Menu
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int pk_Menu { get; set; }
        /// <summary>
        /// 父菜单Id
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name_CHS { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name_CHT { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name_EN { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int LevelID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int TypeID { get; set; }
        /// <summary>
        /// 是否课件
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tag_CHS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tag_CHT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tag_EN { get; set; }
    }
}
