using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace W6EngineService
{
    public partial class ConsoleLog : ServiceBase
    {
       // private static readonly log4net.ILog log = LogManager.GetLogger(typeof(ServiceBase));
        public ConsoleLog()
        {
            InitializeComponent();
        }
        string filePath = @"D:\MyServiceLog.txt";

        //服务执行代码
        protected override void OnStart(string[] args)
        {

            //string path = Path.GetFullPath("/Log4Net.config");
            //var con = log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));
            //Type type = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            //log4net.ILog _logger = log4net.LogManager.GetLogger(type);


            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("{DateTime.Now},服务启动！");
            }
            //log.Info("服务开始.");

            ////开辟新线程处理
            //Thread thread = new Thread(ConsoleLogInfo);
            //while (true)
            //{
            //    thread.Start("现在时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            //    Thread.Sleep(1000);
            //}

        }
        //private void ConsoleLogInfo(Object info)
        //{
        //    log.Info(info);
        //}
        protected override void OnStop()
        {
            //log.Info("服务结束.");
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("{DateTime.Now},服务停止！");
            }
        }
    }
}
