using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillMSA.dto;
using BillMSA.Entities;
using Microsoft.AspNet.SignalR;
using WebBillMSA.Method;

namespace BillMSA
{
    public class ServerHub : Hub
    {
        /// <summary>
        /// 连接测试
        /// </summary>
        public void connect()
        {
            string lianjie = "连接成功";
            Clients.All.connectrunt(lianjie);
        }
        //public void getDate()
        //{
        //    BillAnalyseServer.GetInstance().GetBill();
        //    Clients.All.getdate();
        //}
        public void pie()
        {
            List<Income> res = BillAnalyseServer.GetInstance().Income();
            Clients.All.pieres(res);
        }
        public void optionTime(string date_begin, string date_end)
        {
            BillAnalyseServer.GetInstance().UpDate(date_begin, date_end);
            Clients.All.optiontime();
        }
        public void cross()
        {
            CrossData res = BillAnalyseServer.GetInstance().CrossDataServer();
            Clients.All.crossres(res);
        }
        public void expenditureData() {
            ExpenditureData res = BillAnalyseServer.GetInstance().BillClassification();
            Clients.All.expendituredata(res);
        }
        //public void SendMsg(string message)
        //{
        //    //调用所有客户端的sendMessage方法(sendMessage有2个参数) 
        //    Clients.All.sendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
        //}
    }
}