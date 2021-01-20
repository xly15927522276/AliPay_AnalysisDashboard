using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
namespace BillMSA.dto
{
    public class CrossData
    {
        /// <summary>
        /// 支付时间年月
        /// </summary>
        public List<string> PaymentTimeYM { get; set; }
        /// <summary>
        /// 收入
        /// </summary>
        public List<decimal> Income { get; set; }
        /// <summary>
        /// 支出
        /// </summary>
        public List<decimal> Expenditure { get; set; }
        /// <summary>
        /// 最大收支数
        /// </summary>
        public decimal IEMax { get; set; }
        /// <summary>
        /// 最小大环比
        /// </summary>
        public decimal MaxLinkRelative { get; set; }
        /// <summary>
        /// 最小环比
        /// </summary>
        public decimal MinLinkRelative { get; set; }
        /// <summary>
        /// 环比
        /// </summary>
        public List<string> LinkRelative { get; set; }
    }

    internal class IECrossData : IEnumerable<CrossData>
    {
        private readonly List<CrossData> _list = new List<CrossData>();

        public void Add(CrossData item)
        {
            _list.Add(item);
        }

        public IEnumerator<CrossData> GetEnumerator()
        {
            foreach (var item in _list)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}