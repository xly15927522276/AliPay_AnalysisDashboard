using BillMSA.Entities;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WebBillMSA.Entities;

namespace WebBillMSA.Method
{
    public class subImportCsv_Dataset
    {
        private static string k = System.Configuration.ConfigurationManager.AppSettings["connstring"];
        private static byte[] buffer1 = Convert.FromBase64String(k);
        private static string conn = System.Text.Encoding.UTF8.GetString(buffer1);
        private SqlConnection con = new SqlConnection(conn);
        private static subImportCsv_Dataset subdata = null;
        private static object Singleton_Lock = new object();
        private subImportCsv_Dataset() { }
        public static subImportCsv_Dataset GetInstance()
        {
            if (subdata == null)
            {
                lock (Singleton_Lock)
                {
                    if (subdata == null)
                    {
                        subdata = new subImportCsv_Dataset();
                    }
                }
            }
            return subdata;
        }
        /// <summary>
        /// 添加用户账单
        /// </summary>
        /// <param name="TF"></param>
        /// <param name="id"></param>
        public void ImportData(TextFieldParser TF, int id)
        {
            //string strConn = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=";
            //strConn += filePath;//这个地方只需要目录就可以了                                                      
            //strConn += ";Extensions=asc,csv,tab,txt;";
            //OdbcConnection objConn = new OdbcConnection(strConn);
            //string strSQL = "select * from " + fileName;//文件名，不要带目录
            //OdbcDataAdapter da = new OdbcDataAdapter(strSQL, objConn);
            //DataSet ds = new DataSet();
            //da.Fill(ds,"csv");

            ////开始导入数据库
            //string sql = "insert into data (VarName,TimeString,VarValue,Validity,Time_ms) values (@VarName,@TimeString,@VarValue,@Validity,@Time_ms)";

            //SqlCommand cmd = new SqlCommand(sql, con);
            //SqlParameter p = new SqlParameter("@VarName", SqlDbType.NVarChar);
            //cmd.Parameters.Add(p);
            //p = new SqlParameter("@TimeString", SqlDbType.NVarChar);
            //cmd.Parameters.Add(p);
            //p = new SqlParameter("@VarValue", SqlDbType.NVarChar);
            //cmd.Parameters.Add(p);
            //p = new SqlParameter("@Validity", SqlDbType.NVarChar);
            //cmd.Parameters.Add(p);
            //p = new SqlParameter("@Time_ms", SqlDbType.NVarChar);
            //cmd.Parameters.Add(p);
            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //{
            //    cmd.Parameters["@VarName"].Value = ds.Tables[0].Rows[i]["VarName"].ToString();
            //    cmd.Parameters["@TimeString"].Value = ds.Tables[0].Rows[i]["TimeString"].ToString();
            //    cmd.Parameters["@VarValue"].Value = ds.Tables[0].Rows[i]["VarValue"].ToString();
            //    cmd.Parameters["@Validity"].Value = ds.Tables[0].Rows[i]["Validity"].ToString();
            //    cmd.Parameters["@Time_ms"].Value = ds.Tables[0].Rows[i]["Time_ms"].ToString();
            //    cmd.ExecuteNonQuery();
            //}

            con.Open();
            var cmd = con.CreateCommand();
            var transaction = con.BeginTransaction();
            cmd.Transaction = transaction;
            try
            {
                string[] strLine;
                while (!TF.EndOfData)
                {
                    strLine = TF.ReadFields();
                    if (strLine.Length >= 16 && strLine[0].Length > 20)
                    {
                        cmd.CommandText = "insert into data  values (@UserID,@DealId,@MerchantOrderID,@DealTime,@PaymentTime,@RecentlyAlterTime,@DealSite,@Genre,@Counterparty,@GoodsName,@SumMoney,@IncomeAndExpenses,@DealStatus,@ServiceCharge,@Refund,@Remark,@FundStatus)";
                        cmd.Parameters.Add(new SqlParameter("@UserID", id));
                        cmd.Parameters.Add(new SqlParameter("@DealId", strLine[0]));
                        cmd.Parameters.Add(new SqlParameter("@MerchantOrderID", strLine[1]));
                        cmd.Parameters.Add(new SqlParameter("@DealTime", strLine[2]));
                        cmd.Parameters.Add(new SqlParameter("@PaymentTime", strLine[3]));
                        cmd.Parameters.Add(new SqlParameter("@RecentlyAlterTime", strLine[4]));
                        cmd.Parameters.Add(new SqlParameter("@DealSite", strLine[5]));
                        cmd.Parameters.Add(new SqlParameter("@Genre", strLine[6]));
                        cmd.Parameters.Add(new SqlParameter("@Counterparty", strLine[7]));
                        cmd.Parameters.Add(new SqlParameter("@GoodsName", strLine[8]));
                        cmd.Parameters.Add(new SqlParameter("@SumMoney", strLine[9]));
                        cmd.Parameters.Add(new SqlParameter("@IncomeAndExpenses", strLine[10]));
                        cmd.Parameters.Add(new SqlParameter("@DealStatus", strLine[11]));
                        cmd.Parameters.Add(new SqlParameter("@ServiceCharge", strLine[12]));
                        cmd.Parameters.Add(new SqlParameter("@Refund", strLine[13]));
                        cmd.Parameters.Add(new SqlParameter("@Remark", strLine[14]));
                        cmd.Parameters.Add(new SqlParameter("@FundStatus", strLine[15]));
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                con.Close();
            }

        }
        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int FindUser(string name)
        {
            string sql = "select count(*) from users where UsersName='" + name + "'";
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count <= 0)
            {
                con.Close();
                return 0;
            }
            else
            {
                cmd.CommandText = "select ID from users where UsersName='" + name + "'";
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                return result;
            }
        }
        /// <summary>
        /// 创建用户并返回自增长id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int CreateUser(string name)
        {
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "insert into users values ('" + name + "') select SCOPE_IDENTITY() as Id";
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return result;
        }
        /// <summary>
        /// 获取用户账单
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<data> Getbill(int id)
        {
            List<data> result = new List<data>();
            string sql = "select * from data where UserID='" + id + "'";
            con.Open();
            try
            {
                SqlDataAdapter dap = new SqlDataAdapter(sql, con);
                DataSet ds = new DataSet();
                dap.Fill(ds);
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var res = new data()
                    {
                        UserID = Convert.ToInt32(dt.Rows[i]["UserID"]),
                        DealId = dt.Rows[i]["DealId"].ToString(),
                        MerchantOrderID = dt.Rows[i]["MerchantOrderID"].ToString(),
                        DealTime = (DateTime)dt.Rows[i]["DealTime"],
                        PaymentTime = (DateTime)dt.Rows[i]["PaymentTime"],
                        RecentlyAlterTime = (DateTime)dt.Rows[i]["RecentlyAlterTime"],
                        DealSite = dt.Rows[i]["DealSite"].ToString(),
                        Genre = dt.Rows[i]["Genre"].ToString(),
                        Counterparty = dt.Rows[i]["Counterparty"].ToString(),
                        GoodsName = dt.Rows[i]["Counterparty"].ToString(),
                        SumMoney = (decimal)dt.Rows[i]["SumMoney"],
                        IncomeAndExpenses = dt.Rows[i]["IncomeAndExpenses"].ToString(),
                        DealStatus = dt.Rows[i]["DealStatus"].ToString(),
                        ServiceCharge = (decimal)dt.Rows[i]["ServiceCharge"],
                        Refund = (decimal)dt.Rows[i]["Refund"],
                        Remark = dt.Rows[i]["Remark"].ToString(),
                        FundStatus = dt.Rows[i]["FundStatus"].ToString()
                    };
                    result.Add(res);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        public List<Income> Income(int id)
        {
            List<Income> result = new List<Income>();
            string sql = "select IncomeAndExpenses as 收支,sum(SumMoney)as 金额 from data where UserID='" + id + "'and IncomeAndExpenses !='' group by IncomeAndExpenses";
            con.Open();
            try
            {
                SqlDataAdapter dap = new SqlDataAdapter(sql, con);
                DataSet ds = new DataSet();
                dap.Fill(ds);
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var res = new Income()
                    {
                        IncomeAndExpenses = dt.Rows[i]["收支"].ToString(),
                        SumMoney = (decimal)dt.Rows[i]["金额"]
                    };
                    result.Add(res);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public int Increased(TextFieldParser TF)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("IncreasedData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                string[] strLine;
                int count = 0;
                while (!TF.EndOfData)
                {
                    strLine = TF.ReadFields();
                    if (strLine.Length >= 16 && strLine[0].Length > 20)
                    {
                        SqlParameter[] par = {
                        new SqlParameter("@UserID",BillSever.GetInstance().UserId),
                        new SqlParameter("@DealId",strLine[0]),
                        new SqlParameter("@MerchantOrderID",strLine[1]),
                        new SqlParameter("@DealTime", strLine[2]),
                        new SqlParameter("@PaymentTime", strLine[3]),
                        new SqlParameter("@RecentlyAlterTime", strLine[4]),
                        new SqlParameter("@DealSite", strLine[5]),
                        new SqlParameter("@Genre", strLine[6]),
                        new SqlParameter("@Counterparty", strLine[7]),
                        new SqlParameter("@GoodsName", strLine[8]),
                        new SqlParameter("@SumMoney", strLine[9]),
                        new SqlParameter("@IncomeAndExpenses", strLine[10]),
                        new SqlParameter("@DealStatus", strLine[11]),
                        new SqlParameter("@ServiceCharge", strLine[12]),
                        new SqlParameter("@Refund", strLine[13]),
                        new SqlParameter("@Remark", strLine[14]),
                        new SqlParameter("@FundStatus", strLine[15]),
                        };
                        cmd.Parameters.AddRange(par);
                        count += cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    };
                }
                return count;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }
    }
}