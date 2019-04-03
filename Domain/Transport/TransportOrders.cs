using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Author : 谭振
    /// DateTime : 2017/12/23 16:06:30
    /// Mail : tanz01@haid.com.cn
    /// Description : 
    /// </summary>
    public class TransportOrders
    {
        public string ObjectId { get; set; }
        public string OrderNum { get; set; }
        public string Addresser { get; set; }
        public string SendContact { get; set; }
        public string SendSource { get; set; }
        public DateTime SendDate { get; set; }
        public int OrderState { get; set; }
        public DateTime SignDate { get; set; }
        public string SignAddress { get; set; }
        public string Signer { get; set; }
        public string SignContact { get; set; }
        public string Remark { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
