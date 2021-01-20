using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBillMSA.Entities
{
    public class data
    {
        public int UserID { get; set; }
        public string  DealId { get; set; }
        public string MerchantOrderID { get; set; }
        public DateTime DealTime { get; set; }
        public DateTime PaymentTime { get; set; }
        public DateTime RecentlyAlterTime { get; set; }
        public string  DealSite { get; set; }
        public string  Genre { get; set; }
        public string  Counterparty { get; set; }
        public string  GoodsName { get; set; }
        public decimal SumMoney { get; set; }
        public string IncomeAndExpenses { get; set; }
        public string DealStatus { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Refund { get; set; }
        public string  Remark { get; set; }
        public string FundStatus { get; set; }
        
    }
}