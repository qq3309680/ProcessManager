using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DapperMvc.Models
{
    /// <summary>
    /// 菜单Model
    /// </summary>
    public class MenuModel : Sys_Menu
    {
        public List<MenuModel> SonMenuList { get; set; }
    }
}