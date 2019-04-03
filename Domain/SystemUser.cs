using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class SystemUser
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string UserName { get; set; }


    }
}
