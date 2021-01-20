using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BillMSA.dto
{
    public class ExpenditureData
    {
        public string name { get; set; }
        public List<decimal> Spending { get; set; }

        public List<string> SpendingName { get; set; }

    }
}