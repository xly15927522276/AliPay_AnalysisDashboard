using BillMSA.dto;
using BillMSA.Entities;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using WebBillMSA.Entities;

namespace WebBillMSA.Method
{
    /// <summary>
    /// 账单分析
    /// </summary>
    public class BillAnalyseServer
    {
        private static BillAnalyseServer BillAnalyse = null;
        private static object Singleton_Lock = new object();
        private List<data> datas = new List<data>();
        private List<data> temporary = new List<data>();
        private DateTime date_b;
        private DateTime date_e;
        public List<data> Datas
        {
            get
            {
                return datas;
            }
            set
            {
                datas = value;
            }
        }
        public List<data> Temporary
        {
            get
            {
                return temporary;
            }
            set
            {
                temporary = value;
            }
        }

        private BillAnalyseServer() { }

        public static BillAnalyseServer GetInstance()
        {
            if (BillAnalyse == null)
            {
                lock (Singleton_Lock)
                {
                    if (BillAnalyse == null)
                    {
                        BillAnalyse = new BillAnalyseServer();
                    }
                }
            }
            return BillAnalyse;
        }
        public void GetBill()
        {
            List<data> bill = subImportCsv_Dataset.GetInstance().Getbill(BillSever.GetInstance().UserId);
            Datas.AddRange(bill);
            Temporary.AddRange(bill);
        }
        public List<Income> Income()
        {
            //List<Income> res = subImportCsv_Dataset.GetInstance().Income(BillSever.GetInstance().UserId);
            List<Income> res = new List<Income>();
            var c = (from sg in Temporary
                     where sg.UserID == BillSever.GetInstance().UserId && sg.IncomeAndExpenses != ""
                     group sg by sg.IncomeAndExpenses into da
                     select new
                     {
                         IncomeAndExpenses = da.Key,
                         SumMoney = da.Sum(x => x.SumMoney)
                     });
            foreach (var item in c)
            {
                var sumincome = new Income();
                sumincome.IncomeAndExpenses = item.IncomeAndExpenses;
                sumincome.SumMoney = item.SumMoney;
                res.Add(sumincome);
            }
            return res;
        }
        public void UpDate(string date_begin, string date_end)
        {
            if (date_begin == "")
            {
                date_begin = DateTime.MinValue.ToString();
            }
            else
            {
                date_begin += " 00:00:00";
            }
            date_end += " 23:59:59";
            date_b = Convert.ToDateTime(date_begin);
            date_e = Convert.ToDateTime(date_end);
            var c = (from sg in Datas
                     where sg.PaymentTime >= date_b && sg.PaymentTime <= date_e
                     select sg).ToList();
            Temporary.Clear();
            Temporary.AddRange(c);
        }
        public CrossData CrossDataServer()
        {
            CrossData result = new CrossData();
            result.PaymentTimeYM = new List<string>();
            result.Income = new List<decimal>();
            result.Expenditure = new List<decimal>();
            result.LinkRelative = new List<string>();
            //result.MaxSurplus = 0;
            //result.MinSurplus = 0;
            decimal IEMax = 0;
            if ((date_e - date_b).Days <= 31 && date_b != DateTime.MinValue && date_e != DateTime.MinValue)
            {
                var query = from sg in Temporary
                            where sg.IncomeAndExpenses != ""
                            orderby sg.PaymentTime
                            group sg by new
                            {
                                sg.PaymentTime.Month,
                                sg.PaymentTime.Day,
                                sg.IncomeAndExpenses
                            } into da
                            select new
                            {
                                PaymentTimeMonth = da.Key.Month,
                                PaymentTimeDay = da.Key.Day,
                                IncomeAndExpenses = da.Key.IncomeAndExpenses,
                                SumMoney = da.Sum(x => x.SumMoney)
                            };
                int count = 0;
                string type = string.Empty;
                int RecordDay = 0;
                foreach (var item in query)
                {
                    if (count == 0)
                    {
                        RecordDay = item.PaymentTimeDay;
                        result.PaymentTimeYM.Add((item.PaymentTimeMonth.ToString() + "月" + item.PaymentTimeDay.ToString() + "日").ToString());
                        if (item.IncomeAndExpenses == "收入")
                        {
                            result.Income.Add(item.SumMoney);
                            IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            type = "收入";
                        }
                        else
                        {
                            result.Expenditure.Add(item.SumMoney);
                            IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            type = "支出";
                        }
                        count++;
                        continue;
                    }
                    if (count == 1)
                    {
                        if (RecordDay == item.PaymentTimeDay)
                        {
                            if (item.IncomeAndExpenses == "收入")
                            {
                                result.Income.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            }
                            else
                            {
                                result.Expenditure.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            }
                            count = 0;
                        }
                        else
                        {
                            RecordDay = item.PaymentTimeDay;
                            result.PaymentTimeYM.Add((item.PaymentTimeMonth.ToString() + "月" + item.PaymentTimeDay.ToString() + "日").ToString());
                            if (type == "收入")
                            {
                                result.Expenditure.Add(0);
                            }
                            else
                            {
                                result.Income.Add(0);
                            }
                            if (item.IncomeAndExpenses == "收入")
                            {
                                result.Income.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                                type = "收入";
                            }
                            else
                            {
                                result.Expenditure.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                                type = "支出";
                            }
                        }

                    }
                }

            }
            else
            {
                var query = from sg in Temporary
                            where sg.IncomeAndExpenses != ""
                            orderby sg.PaymentTime
                            group sg by new
                            {
                                sg.PaymentTime.Year,
                                sg.PaymentTime.Month,
                                sg.IncomeAndExpenses
                            } into da
                            select new
                            {
                                PaymentTimeYear = da.Key.Year,
                                PaymentTimeMonth = da.Key.Month,
                                IncomeAndExpenses = da.Key.IncomeAndExpenses,
                                SumMoney = da.Sum(x => x.SumMoney)
                            };
                int count = 0;
                string type = string.Empty;
                int RecordMonth = 0;
                foreach (var item in query)
                {
                    if (count == 0)
                    {
                        RecordMonth = item.PaymentTimeMonth;
                        result.PaymentTimeYM.Add((item.PaymentTimeYear.ToString() + "年" + item.PaymentTimeMonth.ToString() + "月").ToString());
                        if (item.IncomeAndExpenses == "收入")
                        {
                            result.Income.Add(item.SumMoney);
                            IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            type = "收入";
                        }
                        else
                        {
                            result.Expenditure.Add(item.SumMoney);
                            IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            type = "支出";
                        }
                        count++;
                        continue;
                    }
                    if (count == 1)
                    {
                        if (RecordMonth == item.PaymentTimeMonth)
                        {
                            if (item.IncomeAndExpenses == "收入")
                            {
                                result.Income.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            }
                            else
                            {
                                result.Expenditure.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                            }
                            count = 0;
                        }
                        else
                        {
                            RecordMonth = item.PaymentTimeMonth;
                            result.PaymentTimeYM.Add((item.PaymentTimeYear.ToString() + "年" + item.PaymentTimeMonth.ToString() + "月").ToString());
                            if (type == "收入")
                            {
                                result.Expenditure.Add(0);
                            }
                            else
                            {
                                result.Income.Add(0);
                            }
                            if (item.IncomeAndExpenses == "收入")
                            {
                                result.Income.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                                type = "收入";
                            }
                            else
                            {
                                result.Expenditure.Add(item.SumMoney);
                                IEMax = IEMax > item.SumMoney ? IEMax : item.SumMoney;
                                type = "支出";
                            }
                        }

                    }
                }
            }
            if (result.Income.Count() != result.Expenditure.Count())
            {
                if (result.Income.Count() > result.Expenditure.Count())
                {
                    result.Expenditure.Add(0);
                }
                else
                {
                    result.Income.Add(0);
                }
            }
            List<decimal> suplus = new List<decimal>();
            decimal assist = 0;
            result.LinkRelative.Add("0");
            for (int i = 0; i < result.Income.Count(); i++)
            {
                suplus.Add(result.Income[i] + result.Expenditure[i]);
                for (int k = suplus.Count() - 1; k > 0;)
                {
                    if (suplus[i - 1] == 0)
                    {
                        result.LinkRelative.Add("100");
                        break;
                    }
                    assist = Math.Ceiling(((suplus[i] - suplus[i - 1]) / suplus[i - 1]) * 100);
                    result.LinkRelative.Add(assist.ToString());
                    break;
                }
                result.MaxLinkRelative = result.MaxLinkRelative > assist ? result.MaxLinkRelative : assist;
                result.MinLinkRelative = result.MinLinkRelative < assist ? result.MinLinkRelative : assist;
            }
            result.MaxLinkRelative = RoundUpInteger(result.MaxLinkRelative);
            result.MinLinkRelative = RoundUpInteger(result.MinLinkRelative);
            //if (result.MaxSurplus != 0)
            //    result.MaxSurplus = Math.Ceiling(((result.MaxSurplus / (decimal)4.5) * 5));
            if (IEMax != 0)
                result.IEMax = RoundUpInteger(IEMax);
            //if (result.MinSurplus != 0)
            //    result.MinSurplus = Math.Ceiling(((result.MinSurplus / (decimal)1.5) * 2));
            return result;
        }

        public ExpenditureData BillClassification()
        {
            ExpenditureData expenditureData = new ExpenditureData();
            expenditureData.Spending = new List<decimal>();
            expenditureData.SpendingName = new List<string>();
            decimal creditloan = 0;
            decimal petexpense = 0;
            decimal tripexpense = 0;
            decimal fooddrink = 0;
            decimal nogroup = 0;
            var expend = from sg in Temporary
                         where sg.IncomeAndExpenses == "支出"
                         select sg;
            foreach (var item in expend)
            {
                expenditureData.name = item.PaymentTime.Year.ToString();
                break;
            }
            foreach (var item in expend)
            {
                if (item.Counterparty.Contains("信用"))
                {
                    creditloan += item.SumMoney;
                    continue;
                }
                if (item.Counterparty.Contains("宠物"))
                {
                    petexpense += item.SumMoney;
                    continue;
                }
                if (item.Counterparty.Contains("公交") || item.Counterparty.Contains("地铁") || item.Counterparty.Contains("公共汽车") || item.Counterparty.Contains("出行") || item.Counterparty.Contains("铁路") || item.GoodsName.Contains("乘车"))
                {
                    //出行
                    tripexpense += item.SumMoney;
                    continue;
                }
                if (item.Counterparty.Contains("饿了么") || item.Counterparty.Contains("美团") || item.GoodsName.Contains("饮") || item.GoodsName.Contains("饭") || item.GoodsName.Contains("米") || item.GoodsName.Contains("菜") || item.GoodsName.Contains("虾") || item.Counterparty.Contains("咖啡") || item.Counterparty.Contains("饮") || item.GoodsName.Contains("面") || item.GoodsName.Contains("酒") || item.GoodsName.Contains("美食") || item.GoodsName.Contains("香锅") || item.GoodsName.Contains("麻辣烫"))
                {
                    //美食饮品
                    fooddrink += item.SumMoney;
                    continue;
                }
                nogroup += item.SumMoney;
            }
            expenditureData.SpendingName.Add("信用借贷");
            expenditureData.SpendingName.Add("宠物花费");
            expenditureData.SpendingName.Add("公交出行");
            expenditureData.SpendingName.Add("美食饮品");
            expenditureData.SpendingName.Add("Others");
            expenditureData.Spending.Add(creditloan);
            expenditureData.Spending.Add(petexpense);
            expenditureData.Spending.Add(tripexpense);
            expenditureData.Spending.Add(fooddrink);
            expenditureData.Spending.Add(nogroup);

            return expenditureData;
        }
        public int RoundUpInteger(decimal data)
        {
            string Figure = Convert.ToString(Math.Ceiling(data));
            string Symbol = string.Empty;
            if (Figure[0].ToString() == "-")
            {
                Symbol = Figure.Substring(0, 1);
                Figure = Figure.Substring(1, Figure.Length - 1);
            }
            int count = Figure.Length;
            string res = (int.Parse(Figure[0].ToString()) + 1).ToString();
            if (res=="10")
            {
                count++;
            }
            res = res.PadRight(count, '0');
            if (Symbol != string.Empty)
            {
                res = res.Insert(0, "-");
            }
            return int.Parse(res);
        }
    }
}