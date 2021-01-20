using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBillMSA.Method;

namespace BillMSA.Controllers
{
    public class AcquireController : Controller
    {
        // GET: Acquire
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ProcessUploadFiles(HttpPostedFileBase filename, string username)
        {
            //只是提交测试 不用在意
            if (filename != null && username != null)
            {
                if (BillSever.GetInstance().Judge(username))
                {
                    return Content("<script>alert('名称已存在');history.go(-1);</script>");
                }
                var fileName = filename.FileName;
                var filePath = Server.MapPath(string.Format("~/{0}", "File"));
                filename.SaveAs(Path.Combine(filePath, fileName));

                BillSever.GetInstance().ImportData(Path.Combine(filePath, fileName), username);
                BillAnalyseServer.GetInstance().GetBill();
                return RedirectToAction("Ui");
            }
            else
            {
                return Content("<script>alert('请给有效文件或名称');history.go(-1);</script>");
            }
        }
        public ActionResult DL(string textName)
        {
            if (BillSever.GetInstance().Judge(textName))
            {
                BillAnalyseServer.GetInstance().GetBill();
                return RedirectToAction("Ui");
            }
            else
            {
                return Content("<script>alert('没有注册，请注册');history.go(-1);</script>");
            }
        }
        public ActionResult IncreasedData(HttpPostedFileBase filename)
        {
            if (filename!=null)
            {
                var fileName = filename.FileName;
                var filePath = Server.MapPath(string.Format("~/{0}", "File"));
                filename.SaveAs(Path.Combine(filePath, fileName));
                int count = BillSever.GetInstance().IncreasedServer(Path.Combine(filePath, fileName));
                if (count > 0)
                {
                    return Content("<script>alert('新增" + count + "条数据');history.go(-1);</script>");
                }
                else
                {
                    return Content("<script>alert('新增失败,请检查文件');history.go(-1);</script>");
                }
            }
            else
            {
                return Content("<script>alert(请给有效csv文件');history.go(-1);</script>");
            }
        }


        public ActionResult Ui()
        {
            return View();
        }
        public ActionResult IncreasedView()
        {
            return View();
        }
    }
}