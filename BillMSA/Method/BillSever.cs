using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WebBillMSA.Entities;

namespace WebBillMSA.Method
{
    /// <summary>
    /// 账单导入
    /// </summary>
    public class BillSever
    {
        private static BillSever Bill = null;
        private int userid = 0;
        private static object Singleton_Lock = new object();
        private BillSever() { }
        public static BillSever GetInstance()
        {
            if (Bill == null)
            {
                lock (Singleton_Lock)
                {
                    if (Bill == null)
                    {
                        Bill = new BillSever();
                    }
                }
            }
            return Bill;
        }
        public int UserId
        {
            get
            {
                return userid;
            }
            set
            {
                userid = value;
            }
        }
        /// <summary>
        /// 判断是否已有用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Judge(string name)
        {
            int uid = subImportCsv_Dataset.GetInstance().FindUser(name);
            if (uid > 0)
            {
                userid = uid;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 文件导入数据库
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="username"></param>
        public void ImportData(string filePath, string username)
        {
            userid = CreateUserReturnId(username);
            subImportCsv_Dataset.GetInstance().ImportData(Converter(filePath), UserId);
        }
        /// <summary>
        /// 解析csv文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public TextFieldParser Converter(string filePath)
        {
            Microsoft.VisualBasic.FileIO.TextFieldParser TF = new Microsoft.VisualBasic.FileIO.TextFieldParser(filePath, Encoding.GetEncoding("GB2312"));
            TF.Delimiters = new string[] { "," }; //设置分隔符
            return TF;
        }
        /// <summary>
        /// 获取新导入用户的自增长ID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int CreateUserReturnId(string username)
        {
            int result = subImportCsv_Dataset.GetInstance().CreateUser(username);
            return result;
        }
        /// <summary>
        /// 新增账单（库内已有账单不变，有新账单就添加新账单）
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int IncreasedServer(string filePath)
        {
            int result = subImportCsv_Dataset.GetInstance().Increased(Converter(filePath));
            return result;
        }

    }
}