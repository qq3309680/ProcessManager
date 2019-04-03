using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Author : 谭振
    /// DateTime : 2018/1/3 14:48:24
    /// Mail : tanz01@haid.com.cn
    /// Description : 订单运输信息
    /// </summary>
    public class LogisticsInfo
    {
        public string ObjectId { get; set; }
        public string OrderObjectId { get; set; }
        public string CurrentSite { get; set; }
        public bool UnloadScan { get; set; }
        public bool LoadScan { get; set; }
        public string NextSite { get; set; }
        //public string SendAddress { get; set; }
        //public bool IsTargetAddress { get; set; }
        public string OperationPerson { get; set; }
        public DateTime OperationTime { get; set; }
    }
}
