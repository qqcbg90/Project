using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using KingspModel;
using KingspModel.DB;
using KingspModel.Resources;
using KingspModel.Enum;
using MvcPaging;
using web.Filters;
using KingspModel.DataModel;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Ionic.Zip;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Util.Store;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Routing;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Auth;

namespace web.Controllers
{
    public class HomeController : BaseController
    {
        #region const

        /// <summary>
        /// 登入成功顯示的訊息
        /// <para>登入成功!!</para>
        /// </summary>
        public readonly string LOGON_SUCCESS = "登入成功!!";

        /// <summary>
        /// plus type fun2001 投票數
        /// </summary>
        public readonly string PLUS_TYPE1 = "fun2001";

        /// <summary>
        /// plus type fun2002 分享數
        /// </summary>
        public readonly string PLUS_TYPE2 = "fun2002";

        #endregion const

        #region childAction

        /// <summary>
        /// 促銷廣告
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult GetADS()
        {
            NodeID = "funIndex_02";//促銷廣告
            List<ATTACHMENT> indexAtt = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: NodeID).ToList();
            return PartialView("_ADSPartial", GetAttachment(indexAtt, NodeID));
        }

        /// <summary>
        /// 主視覺圖片
        /// </summary>
        /// <returns></returns>
        //[ChildActionOnly]
        //[OutputCache(Duration = DEFAULT_OUTPUTCACHE_DURATION)]
        //public ActionResult GetBanner()
        //{
        //    NodeID = Function.PROJECT_NODE_ID[31];
        //    List<ARTICLE> list = iDB.GetAll<ARTICLE>().Where(p => NodeID.Equals(p.NODE_ID)
        //         && p.DATETIME2 <= DateTime.Today && p.DATETIME3 >= DateTime.Today)
        //        .OrderBy(p => p.ORDER).ThenByDescending(p => p.CREATE_DATE).ToList();
        //    return PartialView("_BannerPartial", list);
        //}

        #endregion childAction

        #region 共用

        public ActionResult Import(int? t)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                return View();
            }
            return GoIndex();
        }

        /// <summary>
        /// use NPOI
        /// </summary>
        /// <param name="excel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excel, int? t)
        {
            List<string> results = new List<string>();
            if (excel != null && ("192.168.0.197".Equals(Request.UserHostAddress) || "118.163.88.193".Equals(Request.UserHostAddress)))
            {
                if (excel.ContentLength > 0)
                {
                    if (!t.HasValue)//不刪除資料
                    {
                        iDB.ExecuteSqlCommand("DELETE from DATA1");
                        //iDB.ExecuteSqlCommand("DELETE from PLUS");
                        //iDB.ExecuteSqlCommand("DELETE from attachment");
                        //iDB.ExecuteSqlCommand("DELETE from PARAGRAPH");
                    }
                    //iData1.ExecuteSqlCommand("DELETE from PARAGRAPH where custom3='dannyImport'");
                    string mark = "dannyImport";
                    HSSFWorkbook book = new HSSFWorkbook(excel.InputStream);

                    for (int j = 0; j < book.NumberOfSheets; j++)
                    {
                        ISheet sheet = book.GetSheetAt(j);

                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            string _id = row.Cells[0].ToMyString();
                            string _city = row.Cells[1].ToMyString();
                            string _c1 = row.Cells[3].ToMyString();
                            string _c2 = row.Cells[5].ToMyString();
                            if (!_c1.IsNullOrEmpty())
                            {
                                NODE city = Function.NodeList.FirstOrDefault(p => p.TITLE.Equals(_city)
                                && Function.NODE_ID_CITYINFO.Equals(p.PARENT_ID));
                                if (city == null)
                                {
                                    results.Add($"編號：{j}-{i}，{_c1} 縣市找不到 {_city}");
                                    continue;
                                }
                                string nid = row.Cells[2].ToMyString();
                                //if ("觀光旅館".Equals(_c2))
                                //{
                                //    nid = "h1";
                                //}
                                //else if ("合法旅館".Equals(_c2))
                                //{
                                //    nid = "h2";
                                //}
                                //else if ("合法民宿".Equals(_c2))
                                //{
                                //    nid = "h3";
                                //}
                                //if (nid.IsNullOrEmpty())
                                //{
                                //    results.Add($"編號：{i}，{_c1} 旅宿類型找不到 {_c2}");
                                //    continue;
                                //}
                                DATA1 d1 = new DATA1();
                                d1.NODE_ID = nid;//旅宿類型
                                d1.CREATER = mark;
                                //d1.DATA_TYPE = city.CONTENT4;//區域 北中南東離島
                                d1.DATA_TYPE = "*";//區域 北中南東離島
                                d1.STATUS = "1";
                                d1.CONTENT1 = _c1;//旅宿名稱
                                d1.CONTENT2 = _c2;//證號
                                d1.CONTENT3 = _city;//縣市
                                d1.CONTENT4 = row.Cells[6].ToMyString();//地址
                                d1.CONTENT5 = row.Cells[7].ToMyString();//電話
                                string _c7 = row.Cells[8].ToMyString();
                                if ("-".Equals(_c7))
                                {
                                    _c7 = string.Empty;
                                }
                                d1.CONTENT6 = _c7;//傳真
                                d1.CONTENT7 = row.Cells[9].ToMyString().ToHttpUrl();//官方網站
                                d1.CONTENT8 = _id;//旅宿網ID
                                d1.CONTENT10 = city.ID;
                                //List<string> imgs = new List<string>();
                                string img1 = row.Cells[4].ToMyString();
                                if (img1.IsNullOrEmpty())
                                {
                                    img1 = "https://www.2022taiwanstay.com.tw/images/default-img.png";
                                }
                                d1.CONTENT11 = img1;//圖片
                                                    //string img2 = row.Cells[11].ToMyString();
                                                    //string img3 = row.Cells[12].ToMyString();
                                                    //if (!img1.IsNullOrEmpty())
                                                    //{
                                                    //    imgs.Add(img1);
                                                    //}
                                                    //if (!img2.IsNullOrEmpty())
                                                    //{
                                                    //    imgs.Add(img2);
                                                    //}
                                                    //if (!img3.IsNullOrEmpty())
                                                    //{
                                                    //    imgs.Add(img3);
                                                    //}
                                                    //d1.CONTENT11 = string.Join(",", imgs);//圖片集合
                                                    //d1.CONTENT12 = row.Cells[2].ToMyString();//圖片2
                                                    //d1.CONTENT13 = row.Cells[2].ToMyString();//圖片3
                                d1.DECIMAL1 = 0;
                                d1.DECIMAL2 = 0;
                                //內容
                                //string _content = row.Cells[13].ToMyString();
                                //d1.CONTENT8 = _content.IsNullOrEmpty() ? "" : "1";
                                //PARAGRAPH p1 = d1.GetParagraphByOrder();
                                //p1.ID = Function.GetGuid();
                                //p1.CREATE_DATE = DateTime.Now;
                                //p1.CREATER = mark;
                                //p1.ORDER = 1;
                                //d1.PARAGRAPH.Add(p1);
                                //if (p1 == null)
                                //{
                                //    p1 = new PARAGRAPH();
                                //    p1.ID = Function.GetGuid();
                                //    p1.CREATE_DATE = DateTime.Now;
                                //    p1.CREATER = mark;
                                //    p1.ORDER = 1;
                                //    d1.PARAGRAPH.Add(p1);
                                //}
                                //if (!_content.IsNullOrEmpty())
                                //{
                                //    _content = _content.Replace("；", "\r\n");
                                //}
                                //p1.CONTENT = _content;

                                //附件
                                //string _pic = row.Cells[3].ToMyString();
                                //ATTACHMENT att = new ATTACHMENT(_pic);
                                //att.ATT_TYPE = AttachmentType.Image.ToIntValue();
                                //att.SetUpFileName();
                                //att.CREATER = mark;
                                //d1.ATTACHMENT.Add(att);
                                //string _path = @"E:\專案\2018\09菲律賓\doc\pic_little\" + _pic;
                                ////Open the stream and read it back.
                                //using (FileStream fs = System.IO.File.OpenRead(_path))
                                //{
                                //    SavePicture(new System.Web.Helpers.WebImage(fs), att);
                                //}
                                iDB.Add<DATA1>(d1);
                            }
                        }
                    }
                    //SetData1List();
                }
            }
            Msgbox_Toast("ok");
            return View(results);
        }

        public ActionResult Import2(int? t)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                return View();
            }
            return GoIndex();
        }

        /// <summary>
        /// use NPOI
        /// </summary>
        /// <param name="excel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import2(HttpPostedFileBase excel, int? t)
        {
            List<string> results = new List<string>();
            if (excel != null && ("192.168.0.197".Equals(Request.UserHostAddress) || "118.163.88.193".Equals(Request.UserHostAddress)))
            {
                if (excel.ContentLength > 0)
                {
                    if (!t.HasValue)//不刪除資料
                    {
                        iDB.ExecuteSqlCommand("DELETE from DATA2 where content16 is not null");
                        //iDB.ExecuteSqlCommand("DELETE from PLUS");
                        //iDB.ExecuteSqlCommand("DELETE from attachment");
                        //iDB.ExecuteSqlCommand("DELETE from PARAGRAPH");
                    }
                    //iData1.ExecuteSqlCommand("DELETE from PARAGRAPH where custom3='dannyImport'");
                    string mark = "dannyImport";
                    HSSFWorkbook book = new HSSFWorkbook(excel.InputStream);

                    for (int j = 0; j < book.NumberOfSheets; j++)
                    {
                        ISheet sheet = book.GetSheetAt(j);

                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            string _id = Function.GetGuid();
                            string _c0 = row.Cells[0].ToMyString();//圖片編號
                            string _carea = row.Cells[1].ToMyString();//區域
                            string _c1 = row.Cells[3].ToMyString();//名稱
                            string _c2 = row.Cells[2].ToMyString();//類別
                            if (!_c1.IsNullOrEmpty())
                            {
                                string dt = string.Empty;
                                foreach (AreaType icon in Enum.GetValues(typeof(AreaType)))
                                {
                                    if (_carea.Equals(icon.GetDescription()))
                                    {
                                        dt = icon.ToIntValue();
                                        break;
                                    }
                                }
                                if (dt.IsNullOrEmpty())
                                {
                                    results.Add($"第 {i + 1} 列：{_c1} 區域找不到 {_carea}");
                                    continue;
                                }
                                string nid = string.Empty;
                                if ("觀光旅館".Equals(_c2))
                                {
                                    nid = "h1";
                                }
                                else if ("一般旅館".Equals(_c2))
                                {
                                    nid = "h2";
                                }
                                else if ("民宿".Equals(_c2))
                                {
                                    nid = "h3";
                                }
                                if (nid.IsNullOrEmpty())
                                {
                                    results.Add($"第 {i + 1} 列：{_c1} 旅宿類型找不到 {_c2}");
                                    continue;
                                }
                                DATA2 d1 = new DATA2();
                                d1.NODE_ID = nid;//旅宿類型
                                d1.CREATER = mark;
                                d1.DATA_TYPE = dt;//區域 北中南東離島
                                d1.STATUS = "1";
                                d1.CONTENT1 = _c1;//旅宿名稱
                                d1.CONTENT2 = row.Cells[4].ToMyString();//電話
                                d1.CONTENT3 = row.Cells[5].ToMyString();//地址
                                d1.CONTENT4 = row.Cells[6].ToMyString();//類別
                                d1.CONTENT5 = row.Cells[7].ToMyString();//文案
                                d1.CONTENT6 = row.Cells[8].ToMyString();//使用期限
                                d1.CONTENT7 = row.Cells[9].ToMyString();//提供張數
                                d1.CONTENT8 = row.Cells[10].ToMyString();//注意事項
                                d1.CONTENT9 = row.Cells[11].ToMyString();//聯絡email
                                d1.CONTENT11 = row.Cells[0].ToMyString();//圖片編號
                                d1.CONTENT12 = _c2;//類型_中文
                                d1.CONTENT13 = _carea;//區域
                                d1.CONTENT14 = row.Cells[12].ToMyString();
                                d1.CONTENT15 = row.Cells[13].ToMyString();
                                d1.DATETIME1 = row.Cells[12].ToMyString().IsNullOrEmpty() ? DateTime.Today : row.Cells[12].ToMyString().ToDateTime();
                                d1.DATETIME2 = row.Cells[13].ToMyString().IsNullOrEmpty() ? "2022/12/31".ToDateTime() : row.Cells[13].ToMyString().ToDateTime();
                                //內容
                                //string _content = row.Cells[13].ToMyString();
                                //PARAGRAPH p1 = d1.GetParagraphByOrder();
                                //p1.ID = Function.GetGuid();
                                //p1.CREATE_DATE = DateTime.Now;
                                //p1.CREATER = mark;
                                //p1.ORDER = 1;
                                //d1.PARAGRAPH.Add(p1);
                                //if (p1 == null)
                                //{
                                //    p1 = new PARAGRAPH();
                                //    p1.ID = Function.GetGuid();
                                //    p1.CREATE_DATE = DateTime.Now;
                                //    p1.CREATER = mark;
                                //    p1.ORDER = 1;
                                //    d1.PARAGRAPH.Add(p1);
                                //}
                                //if (!_content.IsNullOrEmpty())
                                //{
                                //    _content = _content.Replace("；", "\r\n");
                                //}
                                //p1.CONTENT = _content;

                                //附件
                                //string _pic = row.Cells[3].ToMyString();
                                //ATTACHMENT att = new ATTACHMENT(_pic);
                                //att.ATT_TYPE = AttachmentType.Image.ToIntValue();
                                //att.SetUpFileName();
                                //att.CREATER = mark;
                                //d1.ATTACHMENT.Add(att);
                                //string _path = @"E:\專案\2018\09菲律賓\doc\pic_little\" + _pic;
                                ////Open the stream and read it back.
                                //using (FileStream fs = System.IO.File.OpenRead(_path))
                                //{
                                //    SavePicture(new System.Web.Helpers.WebImage(fs), att);
                                //}
                                iDB.Add<DATA2>(d1);
                            }
                        }
                    }
                    SetData2List();
                }
            }
            Msgbox_Toast("ok");
            return View(results);
        }

       

        /// <summary>
        /// ARTICLE內頁建立所需資訊
        /// </summary>
        /// <param name="id"></param>
        private ARTICLE CreateDetailData(string id)
        {
            ARTICLE data = iDB.GetAllAsNoTracking<ARTICLE>().Where(p => p.NODE_ID.Equals(NodeID)
             && p.ID.Equals(id)
           && (p.DATETIME1 <= DateTime.Today)
           && (!p.DATETIME2.HasValue || p.DATETIME2 >= DateTime.Today)
           ).FirstOrDefault();

            return data;
        }

        /// <summary>
        /// DATA1內頁建立所需資訊
        /// </summary>
        /// <param name="id"></param>
        private DATA1 CreateDetailData1(string id)
        {
            DATA1 data = iDB.GetByIDAsNoTracking<DATA1>(id);
            if (data != null && "1".Equals(data.STATUS) &&
           (data.DATETIME1 <= DateTime.Today)
          && (!data.DATETIME2.HasValue || data.DATETIME2 >= DateTime.Today))
            {
                return data;
            }
            return null;
        }

        /// <summary>
        /// 取得 ARTICLE 資料 有判斷 DATETIME1 and DATETIME2
        /// </summary>
        private IQueryable<ARTICLE> GetArticle(int? o = null, DateTime? start = null, DateTime? end = null,
            int? page = null,
            string k = null)
        {
            //CreateViewbag(o, _start, _end, page);
            IQueryable<ARTICLE> query = iDB.GetAllAsNoTracking<ARTICLE>()
                .Where(p => p.NODE_ID.Equals(NodeID)
                && (!IsEN || !string.IsNullOrEmpty(p.CONTENT11))
                && (p.DATETIME1 <= DateTime.Today)
                && (!p.DATETIME2.HasValue || p.DATETIME2 >= DateTime.Today)
                ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
            return query;
        }

        /// <summary>
        /// 取得 ATTACHMENT 資料 by MAIN_ID
        /// </summary>
        private List<ATTACHMENT> GetAttachment(List<ATTACHMENT> model, string m)
        {
            return model.Where(p => m.Equals(p.MAIN_ID)
            && (p.CONTENT9.ToDateTime() <= DateTime.Today)
            && (p.CONTENT10.IsNullOrEmpty() || p.CONTENT10.ToDateTime() >= DateTime.Today))
            .OrderBy(p => p.ORDER).ToList();
        }

        /// <summary>
        /// 取得 DATA1 資料
        /// </summary>
        private IQueryable<DATA1> GetData1(string nid = null, DateTime? start = null, DateTime? end = null,
            string k = null, decimal? d1 = null, string st = "1")
        {
            //DateTime _start = Function.THIS_MONTH_FIRST_DATE;
            DateTime _start = DateTime.Today;
            //DateTime _end = Function.THIS_MONTHR_LAST_DATE;
            DateTime _end = _start.AddMonths(6).AddDays(1);
            //DateTime _end = DateTime.MaxValue;
            if (start.HasValue)
            {
                _start = start.Value;
            }
            if (end.HasValue)
            {
                _end = end.Value.AddDays(1);
            }
            //CreateViewbag(null, _start, _end, page, c3, k, c5);

            IQueryable<DATA1> query = iDB.GetAllAsNoTracking<DATA1>(MAIN_ID: nid)
                .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k))
                && (!IsEN || !string.IsNullOrEmpty(p.CONTENT16))
                && (!d1.HasValue || p.DECIMAL1 == d1)
                && p.STATUS.Equals(st)
                && (p.DATETIME1 <= DateTime.Today)
                && (!p.DATETIME2.HasValue || p.DATETIME2 >= DateTime.Today)
                ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
            return query;
        }

        /// <summary>
        /// 取得 DATA1 資料 from 靜態資料
        /// </summary>
        private List<IGrouping<string, DATA1>> GetData1FromMemory(string nid = null, DateTime? start = null, DateTime? end = null,
            string k = null, string did = null, string dt = "0")
        {
            //DateTime _start = Function.THIS_MONTH_FIRST_DATE;
            DateTime _start = DateTime.Today;
            //DateTime _end = Function.THIS_MONTHR_LAST_DATE;
            DateTime _end = _start.AddMonths(6).AddDays(1);
            //DateTime _end = DateTime.MaxValue;
            if (start.HasValue)
            {
                _start = start.Value;
            }
            if (end.HasValue)
            {
                _end = end.Value.AddDays(1);
            }
            //CreateViewbag(null, _start, _end, page, c3, k, c5);
            ViewBag.k = k;
            List<IGrouping<string, DATA1>> query = Function.Data1List
                .Where(p =>
                (string.IsNullOrEmpty(did) || p.ID.Equals(did))
                &&
                ((string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k))
                && p.DATA_TYPE.Equals(dt))
                //&& (p.DATETIME1 <= DateTime.Today)
                //&& (!p.DATETIME2.HasValue || p.DATETIME2 >= DateTime.Today)
                ).GroupBy(p => p.NODE_ID).OrderBy(p => p.Key).ToList();
            return query;
        }

        /// <summary>
        /// 取得 DATA2 資料
        /// </summary>
        private IQueryable<DATA2> GetData2(string c3 = null, DateTime? start = null, DateTime? end = null,
            int? page = null, string k = null, int? t = 1, string c5 = null)
        {
            //DateTime _start = Function.THIS_MONTH_FIRST_DATE;
            DateTime _start = DateTime.Today;
            //DateTime _end = Function.THIS_MONTHR_LAST_DATE;
            DateTime _end = _start.AddMonths(6).AddDays(1);
            //DateTime _end = DateTime.MaxValue;
            if (start.HasValue)
            {
                _start = start.Value;
            }
            if (end.HasValue)
            {
                _end = end.Value.AddDays(1);
            }
            CreateViewbag(null, _start, _end, page, c3, k, c5);

            IQueryable<DATA2> query = iDB.GetAll<DATA2>()
                .Where(p => (string.IsNullOrEmpty(c3) || p.CONTENT3.Equals(c3))
                && (string.IsNullOrEmpty(k) || p.CONTENT7.Contains(k))
                && (string.IsNullOrEmpty(c5) || p.CONTENT5.Equals(c5))
                && p.DECIMAL1 == 1//公開演出
                && (p.DATETIME9.HasValue && p.DATETIME10.HasValue)//要有正式演出-開始結束時間
                );

            if (t == 1)//要判斷d9, d10
            {
                //query = query.Where(p =>
                //  ((p.DATETIME9 >= _start && p.DATETIME9 <= _end)
                //  || (p.DATETIME10 >= _start && p.DATETIME10 <= _end))
                //);

                //2020-11-24 改成要完全符合搜尋條件
                //query = query.Where(p => (p.DATETIME9 >= _start && p.DATETIME10 <= _end));

                //2020-12-07 日期改成由SQL檢查
                List<string> lsDATA2_ID = new List<string>();
                using (DBEntities db = new DBEntities())
                {
                    string SqlStr = @"SELECT d2.ID
FROM DATA2 d2
JOIN PLUS p ON p.MAIN_ID = d2.ID AND p.PLUS_TYPE = 'TIME' AND p.[ORDER] = 1 AND p.[ENABLE] = 1
CROSS APPLY dbo.fnSplitDate2Table(p.ID, p.DATETIME1, p.[DATETIME2]) t
WHERE d2.NODE_ID = 'fun13_05_03' AND d2.[ENABLE] = 1 AND d2.DECIMAL1 = 1 /*公開演出*/
AND (@C3 = '' OR d2.CONTENT3 LIKE '%' + @C3 + '%')
AND (@K = '' OR d2.CONTENT7 LIKE '%' + @K + '%')
AND (@C5 = '' OR d2.CONTENT5 LIKE '%' + @C5 + '%')
AND CONVERT(varchar(10),t.ST,111) BETWEEN @START AND @END
GROUP BY d2.ID";
                    lsDATA2_ID = db.Database.SqlQuery<string>(SqlStr, new SqlParameter("C3", c3.ToMyString()), new SqlParameter("C5", c5.ToMyString()), new SqlParameter("K", k.ToMyString())
                        , new SqlParameter("START", _start), new SqlParameter("END", _end)).ToList();
                }
                string[] arrDATA2_ID = lsDATA2_ID.ToArray();
                query = query.Where(p => arrDATA2_ID.Contains(p.ID));
            }

            return query;
        }

        /// <summary>
        /// 取得 DATA3 資料 尚未請款的 status=0
        /// </summary>
        private IQueryable<DATA3> GetData3(string nid = null, DateTime? start = null, DateTime? end = null,
            string k = null, decimal? d1 = null, string st = "0")
        {
            //DateTime _start = Function.THIS_MONTH_FIRST_DATE;
            DateTime _start = DateTime.Today;
            //DateTime _end = Function.THIS_MONTHR_LAST_DATE;
            DateTime _end = _start.AddMonths(6).AddDays(1);
            //DateTime _end = DateTime.MaxValue;
            if (start.HasValue)
            {
                _start = start.Value;
            }
            if (end.HasValue)
            {
                _end = end.Value.AddDays(1);
            }
            //CreateViewbag(null, _start, _end, page, c3, k, c5);

            IQueryable<DATA3> query = iDB.GetAllAsNoTracking<DATA3>(MAIN_ID: nid)
                .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k))
                && (!IsEN || !string.IsNullOrEmpty(p.CONTENT2))
                && (!d1.HasValue || p.DECIMAL1 == d1)
                && p.STATUS.Equals(st)
                && (p.DATETIME1 <= DateTime.Today)
                && (!p.DATETIME2.HasValue || p.DATETIME2 >= DateTime.Today)
                ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
            return query;
        }

        #endregion 共用

        #region 首頁

        public ActionResult Index()
        {
            PageTitle = Resource.index;
            return View();
        }

        #endregion 首頁

        #region DATA1 系列 + PLUS 活動

        [HttpPost]
        public ActionResult EndGame(string d1, string d2, string d3)
        {
            List<string> results = new List<string>();
            string _msg = Function.DEFAULT_ERROR;
            string _countStr = "0";
            int _d1 = d1.ToInt();
            if (_d1 < 0 || _d1 > 600)//調整數字
            {
                _d1 = 0;
            }
            string pid = Session[Function.SESSION_GROUP].ToMyString();
            if (pid.IsNullOrEmpty())
            {
            }
            else
            {
                if (!CheckDateTime())
                {
                    _msg = "活動日期已結束!!";
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        _msg = "尚未登入!!";
                    }
                    else
                    {
                        //取得登入者今日的PLUS資料
                        List<PLUS> total = iDB.GetAll<PLUS>().Where(p => p.CREATER.Equals(User.Identity.Name)
                           && p.DATETIME1 == DateTime.Today
                        ).ToList();
                        _countStr = (DEFAULT_VOTE - total.Count).ToMyString();
                        PLUS data = total.Where(p => p.ID.Equals(pid)).FirstOrDefault();
                        if (data != null)
                        {
                            data.DATETIME3 = DateTime.Now;
                            //data.DECIMAL6 = new TimeSpan(data.DATETIME3.Value.Ticks - data.DATETIME2.Value.Ticks).Seconds;//經計算的 有bug
                            TimeSpan _ts = data.DATETIME3.Value.Subtract(data.DATETIME2.Value);
                            data.DECIMAL6 = _ts.TotalSeconds.ToMyString().ToDecimal();//經計算的
                            data.DECIMAL4 = d1.ToDecimal();//回傳的
                            data.DECIMAL1 = _d1;
                            data.DECIMAL2 = d2.ToInt();
                            data.DECIMAL3 = d3.ToInt();
                            iDB.Save();
                            UpdateUserGame();
                            _msg = "本次挑戰結束!!";
                        }
                        else
                        {
                            _msg = "挑戰失敗，資料有誤!!";
                        }
                    }
                }
            }
            results.Add(_msg);
            results.Add(_countStr);
            results.Add(_d1.ToMyString());
            //隨機3家旅宿
            List<DATA1> hotels = Function.Data1List.OrderByRandom().Take(3).ToList();
            ViewBag.hotels = hotels;
            //end
            return PartialView("_GameEndPartial", results);
            //lock (Function.ThisLock)
            //{
            //}
        }

        [NonAction]
        [HttpPost]
        public ActionResult ShareD11(string id)
        {
            lock (Function.ThisLock)
            {
                List<string> results = new List<string>();
                string _msg = Function.DEFAULT_ERROR;
                string _countStr = "0";
                DATA1 data = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id));
                if (!CheckDateTime())
                {
                    _msg = "活動日期已結束!!";
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        _msg = "尚未登入!!";
                    }
                    else
                    {
                        if (data != null)
                        {
                            _countStr = data.DECIMAL2.ToInt().ReplaceNumToThousand();
                            bool isShare = iDB.GetAll<PLUS>().Any(p => p.CREATER.Equals(User.Identity.Name)
                                 && p.PLUS_TYPE.Equals(PLUS_TYPE2)
                                 && p.DATETIME1 == DateTime.Today);
                            if (!isShare)
                            {
                                _msg = $"謝謝分享，今日獲得額外{DEFAULT_SHARE_VOTE}票!!";
                            }
                            else
                            {
                                _msg = "謝謝分享!!";
                            }
                            CreateData1Plus(data, PLUS_TYPE2);
                            _countStr = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id)).DECIMAL2.ToInt().ReplaceNumToThousand();
                        }
                    }
                }
                results.Add(_msg);
                results.Add(_countStr);
                //return Content(_msg);
                return Json(results.ToArray());
            }
        }

        [NonAction]
        [HttpPost]
        public ActionResult ShowD11(string id)
        {
            DATA1 data = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id));
            return PartialView("_DetaiHotelPartial", data);
        }

        [NonAction]
        [HttpPost]
        public ActionResult ShowD21(string id)
        {
            DATA2 data = Function.Data2List.FirstOrDefault(p => p.ID.Equals(id));
            return PartialView("_DetaiHotel2Partial", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartGame()
        {
            List<string> results = new List<string>();
            Random rnd = new Random();
            List<int> rnds = Enumerable.Range(11, 48).OrderBy(x => rnd.Next()).Take(11).OrderByDescending(p => p).ToList();
            rnds.Add(10);
            string _msg = Function.DEFAULT_ERROR;
            string _countStr = "0";
            if (!CheckDateTime())
            {
                _msg = "活動日期已結束!!";
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                {
                    _msg = "尚未登入!!";
                }
                else
                {
                    //取得登入者今日的PLUS資料
                    List<PLUS> total = iDB.GetAll<PLUS>().Where(p => p.CREATER.Equals(User.Identity.Name)
                       && p.DATETIME1 == DateTime.Today
                    ).ToList();
                    _countStr = (DEFAULT_VOTE - total.Count).ToMyString();
                    if (total.Count < DEFAULT_VOTE)
                    {
                        //PLUS start = CreateGamePlus();
                        //Session[Function.SESSION_GROUP] = start.ID;
                        //建立遊戲需要參數
                        _msg = "遊戲即將開始!!";
                    }
                    else
                    {
                        _msg = "今日已無挑戰機會!!";
                    }
                }
            }
            results.Add(_msg);
            results.Add(_countStr);
            results.Add(string.Join(",", rnds));
            //return Content(_msg);
            //return Json(results.ToArray());
            return PartialView("_GameStartPartial2", results);
            //lock (Function.ThisLock)
            //{
            //}
        }

        /// <summary>
        /// 建立PLUS資料
        /// </summary>
        /// <param name="model"></param>
        private PLUS CreateGamePlus(string pt = "fun2001", string c4 = "0")
        {
            PLUS data = new PLUS();
            data.ID = Function.GetGuid();
            data.MAIN_ID = "*";
            data.CREATER = User.Identity.Name;
            data.PLUS_TYPE = pt;
            data.STATUS = "1";//預設1
            data.CREATE_DATE = DateTime.Now;
            data.CONTENT1 = Request.UserHostAddress;
            data.CONTENT2 = Request.UserAgent;
            data.CONTENT3 = Session[Function.SESSION_ROLE].ToMyString();
            data.DECIMAL1 = 0;//總分
            data.DECIMAL2 = DEFAULT_VOTE;//生命
            data.DECIMAL3 = DEFAULT_SHARE_VOTE;//遊戲秒數
            data.DECIMAL4 = 0;//總分
            data.DECIMAL5 = DEFAULT_VOTE;//生命
            data.DECIMAL6 = 0;//初始秒數
                              //data.CONTENT3 = model.DATA_TYPE;
                              //data.CONTENT4 = c4;
                              //data.CONTENT5 = model.NODE_ID;
            data.DATETIME1 = DateTime.Today;
            data.DATETIME2 = data.CREATE_DATE;
            if (iDB.Add<PLUS>(data))
            {
                UpdateUserGame();
                return data;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新資料
        /// </summary>
        private void UpdateUserGame()
        {
            if (User.Identity.IsAuthenticated)
            {
                string _c = User.Identity.Name;
                //執行sql
                string _sql = $"update [USER] set CONTENT5=(select SUM(DECIMAL1) from plus where [ENABLE]=1 and PLUS_TYPE='fun2001' and CREATER='{_c}'), CONTENT6 = (select count(ID) from plus where [ENABLE] = 1 and PLUS_TYPE = 'fun2001' and CREATER = '{_c}') where USER_ID = '{_c}'";
                iDB.ExecuteSqlCommand(_sql);
            }
        }

        #endregion DATA1 系列 + PLUS 活動

        #region 優惠活動
        private bool checkEndDate()
        {
            DateTime checkEnd = new DateTime(2022, 9, 23, 0, 0, 0);
            return DateTime.Now >= checkEnd;
        }

        [NodeSelect("CityInfo")]
        public ActionResult Discount(string id)
        {
            //if (checkEndDate())
            //{
            //	Msgbox_Toast("已超過領獎期限!!");
            //	return GoIndex();
            //}
            if (id.IsNullOrEmpty())
            {
                return GoIndex();
            }
            string b1 = "得獎者回覆專區";
            ViewBag.b1 = b1;
            PageTitle = b1;
            DataModel model = new DataModel();
            model.Atts = new List<ATTACHMENT>();

            DATA2 d2 = iDB.GetByIDAsNoTracking<DATA2>(id);
            if (d2 != null)
            {
                DateTime dtNow = DateTime.Now;
                if (dtNow < d2.DATETIME3.Value || dtNow > d2.DATETIME4.Value)
                {
                    Msgbox_Toast($"領獎期限已逾期!!<br/>({d2.DATETIME4.Value.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }

                //獎項資訊
                model.CONTENT11 = d2.CONTENT11;
                model.CONTENT12 = d2.CONTENT12;
                model.CONTENT13 = d2.CONTENT13;
                model.DECIMAL1 = d2.DECIMAL1;

                //得獎人資料
                model.CONTENT1 = d2.CONTENT1;
                model.CONTENT2 = d2.CONTENT2;
                model.CONTENT3 = d2.CONTENT3;
                model.CONTENT4 = d2.CONTENT4;
                model.CONTENT5 = d2.CONTENT5;
                model.Atts = d2.ATTACHMENT.OrderBy(p => p.ORDER).ToList();
                model.DATETIME1 = d2.DATETIME1;
                model.DATETIME2 = d2.DATETIME2;
                model.DATETIME3 = d2.DATETIME3;
                model.DATETIME4 = d2.DATETIME4;
            }
            else
            {
                return GoIndex();
            }
            return View(model);
        }

        [HttpPost]
        [NodeSelect("CityInfo")]
        public ActionResult Discount(string id, DataModel model, HttpPostedFileBase hpf1, HttpPostedFileBase hpf2)
        {
            //if (checkEndDate())
            //{
            //	Msgbox_Toast("已超過領獎期限!!");
            //	return GoIndex();
            //}
            DATA2 d2 = iDB.GetByID<DATA2>(id);
            if (d2 == null)
            {
                return GoIndex();
            }
            DateTime dtNow = DateTime.Now;
            if (dtNow < d2.DATETIME3.Value || dtNow > d2.DATETIME4.Value)
            {
                Msgbox_Toast($"領獎期限已逾期!!<br/>({d2.DATETIME4.Value.ToString("yyyy/MM/dd HH:mm:ss")})");
                return GoIndex();
            }

            d2.CONTENT1 = model.CONTENT1;
            d2.CONTENT2 = model.CONTENT2;
            d2.CONTENT3 = model.CONTENT3;
            d2.CONTENT4 = model.CONTENT4;
            d2.CONTENT5 = model.CONTENT5;
            if (d2.DATETIME1 == null)
            {
                d2.DATETIME1 = DateTime.Now; //第一次回覆時間
                d2.DECIMAL2 = 0;
            }
            else
            {
                d2.UPDATE_DATE = DateTime.Now; //修改時間
                d2.DECIMAL2 += 1; //修改次數
            }

            #region attachment
            List<HttpPostedFileBase> HPFs = new List<HttpPostedFileBase>() { hpf1, hpf2 };
            string sWarningMsg = string.Empty;
            int i = 0;
            foreach (HttpPostedFileBase hpf in HPFs)
            {
                i++;
                if (hpf == null || hpf.ContentLength <= 0)
                {
                    continue;
                }
                //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                //if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
                //{
                //    sWarningMsg += hpf.FileName + "：附件大小超過 15 MB！";
                //}
                //else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
                //{
                //    sWarningMsg += hpf.FileName + "：附件格式不符！";
                //}
                if (sWarningMsg.IsNullOrEmpty())
                {
                    ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == i);
                    if (org != null)
                    {
                        iDB.Delete<ATTACHMENT>(org.ID, true);
                    }
                    ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                    att.ATT_TYPE = AttachmentType.File.ToIntValue();
                    att.SetUpFileName();
                    att.CREATER = User.Identity.Name;
                    att.ORDER = i;
                    d2.ATTACHMENT.Add(att);
                    SaveAtt(hpf, att.FILE_NAME);
                }
            }
            #endregion

            iDB.Save();
            Msgbox_Toast("謝謝您的回覆!!");
            //return GoIndex();
            return RedirectToAction("Discount", new { id = d2.ID });
        }

        private bool checkIP()
        {
            return "192.168.0.72".Equals(Request.UserHostAddress) || "192.168.0.64".Equals(Request.UserHostAddress) ||
                "118.163.88.193".Equals(Request.UserHostAddress);
        }

        public ActionResult DiscountCheck()
        {
            if (checkIP())
            {
                string b1 = "得獎者回覆確認";
                ViewBag.b1 = b1;
                PageTitle = b1;
                return View(iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: "Winners").OrderBy(p => p.DATA_TYPE).ThenBy(p => p.ORDER));
            }
            return GoIndex();
        }

        [NonAction]
        [HttpPost]
        public ActionResult DiscountCheck(string id, string c)
        {
            if (checkIP())
            {
                DATA2 d2 = iDB.GetByID<DATA2>(id);
                if (c.IsNullOrEmpty())
                {
                    if (d2 != null && "0".Equals(d2.CONTENT16))
                    {
                        d2.DATETIME4 = DateTime.Now;
                        //iDB.Save();
                        return Content("1");
                    }
                }
                else if ("1".Equals(c))
                {
                    if (d2 != null && d2.CONTENT16.IsNullOrEmpty())
                    {
                        d2.CONTENT16 = "1";
                        d2.DATETIME5 = DateTime.Now;
                        //iDB.Save();
                        return Content("1");
                    }
                }
            }
            return Content("0");
        }

        #endregion 優惠活動

        #region DATA1 系列

        [NonAction]
        public ActionResult Hotels(string id, DateTime? start, DateTime? end, string k, int? page, string dt = "0")
        {
            PageTitle = "票選自行車友善旅宿";
            if (!id.IsNullOrEmpty())
            {
                DATA1 model = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id));
                if (model != null)
                {
                    dt = model.DATA_TYPE;
                }
            }
            //NodeID = "fun2000";
            //CreateBread(2);
            if (dt.ToInt() < 0 || dt.ToInt() > 5)
            {
                dt = "0";
            }
            ViewBag.dt = dt;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.dvote = GetVoteTable();
            }
            return View(GetData1FromMemory(k: k, did: id, dt: dt));
        }

        [NonAction]
        [HttpPost]
        public ActionResult ShareD1(string id)
        {
            lock (Function.ThisLock)
            {
                List<string> results = new List<string>();
                string _msg = Function.DEFAULT_ERROR;
                string _countStr = "0";
                DATA1 data = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id));
                if (!CheckDateTime())
                {
                    _msg = "活動日期已結束!!";
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        _msg = "尚未登入!!";
                    }
                    else
                    {
                        if (data != null)
                        {
                            _countStr = data.DECIMAL2.ToInt().ReplaceNumToThousand();
                            bool isShare = iDB.GetAll<PLUS>().Any(p => p.CREATER.Equals(User.Identity.Name)
                                 && p.PLUS_TYPE.Equals(PLUS_TYPE2)
                                 && p.DATETIME1 == DateTime.Today);
                            if (!isShare)
                            {
                                _msg = $"謝謝分享，今日獲得額外{DEFAULT_SHARE_VOTE}票!!";
                            }
                            else
                            {
                                _msg = "謝謝分享!!";
                            }
                            CreateData1Plus(data, PLUS_TYPE2);
                            _countStr = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id)).DECIMAL2.ToInt().ReplaceNumToThousand();
                        }
                    }
                }
                results.Add(_msg);
                results.Add(_countStr);
                //return Content(_msg);
                return Json(results.ToArray());
            }
        }

        [NonAction]
        [HttpPost]
        public ActionResult ShowD1(string id)
        {
            DATA1 data = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id));
            return PartialView("_DetaiHotelPartial", data);
        }

        [NonAction]
        [HttpPost]
        public ActionResult ShowD2(string id)
        {
            DATA2 data = Function.Data2List.FirstOrDefault(p => p.ID.Equals(id));
            return PartialView("_DetaiHotel2Partial", data);
        }

        [NonAction]
        [HttpPost]
        public ActionResult VoteD1(string id)
        {
            lock (Function.ThisLock)
            {
                List<string> results = new List<string>();
                string _msg = Function.DEFAULT_ERROR;
                string _countStr = "0";
                DATA1 data = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id));
                if (!CheckDateTime())
                {
                    _msg = "活動日期已結束!!";
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        _msg = "尚未登入!!";
                    }
                    else
                    {
                        if (data != null)
                        {
                            _countStr = data.DECIMAL1.ToInt().ReplaceNumToThousand();
                            //取得登入者今日的PLUS資料
                            List<PLUS> total = iDB.GetAll<PLUS>().Where(p => p.CREATER.Equals(User.Identity.Name)
                               && p.DATETIME1 == DateTime.Today
                            ).ToList();
                            //一般規則:每區域每日3票(3種分類各1票)
                            List<PLUS> list = total.Where(p =>
                             p.PLUS_TYPE.Equals(PLUS_TYPE1)
                             && p.CONTENT3.Equals(data.DATA_TYPE)//區域
                             && p.CONTENT4.Equals("0")
                            //&& p.CONTENT5.Equals(data.NODE_ID)//類型
                            ).ToList();
                            //額外規則:不限區、不限家每日3票
                            List<PLUS> list2 = total.Where(p =>
                             p.PLUS_TYPE.Equals(PLUS_TYPE1)
                             && p.CONTENT4.Equals("1")//因分享而投的
                            ).ToList();
                            //分享而獲得的額外票數
                            bool isShare = total.Any(p => p.PLUS_TYPE.Equals(PLUS_TYPE2));
                            if (isShare)//先判斷分享
                            {
                                //分享的不可以重覆
                                if ((list2 == null || list2.Where(p => p.MAIN_ID.Equals(data.ID)).FirstOrDefault() == null)
                                    && list2.Count < DEFAULT_SHARE_VOTE)
                                {
                                    CreateData1Plus(data, c4: "1");
                                    _msg = "投票完成!!";
                                }
                                else
                                {
                                    if (list.Count < DEFAULT_VOTE)
                                    {
                                        //不可投同一家或同一類型
                                        if (!list.Any(p => p.MAIN_ID.Equals(data.ID)) && !list.Any(p => data.NODE_ID.Equals(p.CONTENT5)))
                                        {
                                            CreateData1Plus(data);
                                            _msg = "投票完成!!";
                                        }
                                        else
                                        {
                                            _msg = "今日已投過票!!";
                                        }
                                    }
                                    else
                                    {
                                        _msg = $"此區今日已投滿{DEFAULT_VOTE}票!!";
                                    }
                                }
                            }
                            else
                            {
                                if (list.Count < DEFAULT_VOTE)
                                {
                                    //不可投同一家或同一類型
                                    if (!list.Any(p => p.MAIN_ID.Equals(data.ID)) && !list.Any(p => data.NODE_ID.Equals(p.CONTENT5)))
                                    {
                                        CreateData1Plus(data);
                                        _msg = "投票完成!!";
                                    }
                                    else
                                    {
                                        _msg = "今日已投過票!!";
                                    }
                                }
                                else
                                {
                                    _msg = $"此區今日已投滿{DEFAULT_VOTE}票!!";
                                }
                            }
                            _countStr = Function.Data1List.FirstOrDefault(p => p.ID.Equals(id)).DECIMAL1.ToInt().ReplaceNumToThousand();
                        }
                    }
                }
                results.Add(_msg);
                results.Add(_countStr);
                //return Content(_msg);
                return Json(results.ToArray());
            }
        }

        private DataTable GetVoteTable(string user = null, DateTime? date = null)
        {
            if (user.IsNullOrEmpty())
            {
                user = User.Identity.Name;
            }
            if (!date.HasValue)
            {
                date = DateTime.Today;
            }
            DataTable dt = new DataTable();
            //            string SqlStr = string.Format(@"
            //CREATE TABLE #tmp(COL1 int, COL2 varchar(5));

            //INSERT INTO #tmp VALUES(0,'h1'), (1,'h1'), (2,'h1'), (3,'h1'), (4,'h1'), (0,'h2'), (1,'h2'), (2,'h2'), (3,'h2'), (4,'h2'), (0,'h3'), (1,'h3'), (2,'h3'), (3,'h3'), (4,'h3');

            //SELECT t.*, p.MAIN_ID
            //FROM #tmp t
            //left join plus p on t.col1 = p.CONTENT3 and t.col2 = p.CONTENT5
            //and p.PLUS_TYPE = 'fun2001' and p.CONTENT4 = '0' and p.CREATER = '{0}' and p.DATETIME1 = '{1}' "
            //, User.Identity.Name, date.ToDefaultString());
            string SqlStr = $" EXEC SP_x '{user}','{date.ToDefaultString()}' ";
            using (DBEntities db = new DBEntities())
            {
                dt = Function.getDataTable(db, SqlStr);
            }
            return dt;
        }

        #endregion DATA1 系列

        #region 靜態文字系列

        //活動辦法
        public ActionResult About()
        {
            //string b1 = Resource.footer06;
            string b1 = "活動辦法";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        [NonAction]
        public ActionResult ContactUs()
        {
            string b1 = Resource.footer08;
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        [NonAction]
        public ActionResult Cookie()
        {
            string b1 = Resource.footer10;
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        //Facebook刪除資料指示
        public ActionResult FbDelete()
        {
            string b1 = "Facebook資料刪除指示";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        //抽獎結果
        public ActionResult LotteryResult()
        {
            //string b1 = Resource.footer06;
            string b1 = "抽獎結果";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        //隱私政策
        public ActionResult PrivacyPolicy()
        {
            string b1 = Resource.privacypolicy;
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        //服務條款
        public ActionResult ServicePolicy()
        {
            string b1 = Resource.servicepolicy;
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        #endregion 靜態文字系列

        #region 會員相關

        public ActionResult LogOff()
        {
            LogOffCleanEvent();
            Msgbox_Toast($"{Resource.logout}！！");
            return GoIndex();
        }

        //註冊&登入
        [HttpPost]
        [CaptchaVerify("captcha", "驗證碼錯誤")]
        public ActionResult Register(UserModel model, string captcha)
        {
            bool isReg = "1".Equals(model.CONTENT9);
            string _act = isReg ? Resource.register : Resource.login;
            string _msg = $"{_act}失敗!!";

            if (isReg)
            {
                #region 註冊

                if (!ModelState.IsValid)
                {
                    if (ModelState["CONTENT1"].Errors.Count > 0)
                    {
                        _msg = "E-mail(帳號)必填!!";
                    }
                    else if (ModelState["captcha"].Errors.Count > 0)
                    {
                        _msg = "驗證碼錯誤!!";
                    }
                }
                else
                {
                    bool isValidate = model.CONTENT1.IsEmail()
                        && iDB.CheckEmailRepeat<USER>(model.USER_ID, model.CONTENT1);
                    if (isValidate)
                    {
                        USER user = new USER();
                        user.USER_ID = model.USER_ID;
                        user.CREATER = "web";//前台註冊
                        user.MAIN_ID = "fun6000";
                        if (model.PASSWORD.IsNullOrEmpty())
                        {
                            model.PASSWORD = Function.DEFAULT_PASSWORD_SETUP;
                        }
                        user.PASSWORD = model.PASSWORD.ToSHA1();
                        user.CONTENT1 = model.CONTENT1;//帳號 email
                        if (model.CONTENT2.IsNullOrEmpty())
                        {
                            model.CONTENT2 = model.CONTENT1;
                        }
                        user.CONTENT2 = model.CONTENT2;//姓名
                                                       //user.CONTENT3 = model.CONTENT3;//名字
                        user.CONTENT30 = Function.GetGuid();
                        //user.CONTENT4 = SexType.Male.ToIntValue();//性別 男0 女1
                        //user.CONTENT5 = model.CONTENT5;//電話
                        //user.CONTENT6 = "level01";//會員等級
                        //user.CONTENT7 = "自行註冊";//來源
                        user.DATETIME1 = model.DATETIME1;
                        user.DATETIME2 = DateTime.Now;
                        //備註
                        //PARAGRAPH ph = new PARAGRAPH();
                        //ph.ID = Function.GetGuid();
                        //ph.CREATE_DATE = DateTime.Now;
                        //ph.CREATER = user.USER_ID;
                        //ph.ORDER = 1;
                        //user.PARAGRAPH.Add(ph);
                        if (iDB.Add<USER>(user))
                        {
                            Send(user);
                            //SetLogonData(user);
                            Msgbox_Toast("謝謝您的註冊!!<br />請至信箱收取驗證碼!!");
                            _msg = "1";
                        }
                    }
                }

                #endregion 註冊
            }
            else//登入
            {
                #region 登入

                ModelState.Remove("captcha");
                if (!ModelState.IsValid)
                {
                    if (ModelState["CONTENT1"].Errors.Count > 0)
                    {
                        _msg = "E-mail(帳號)必填!!";
                    }
                }
                else
                {
                    string pwd = model.PASSWORD.IsNullOrEmpty() ? "**" : model.PASSWORD.ToSHA1();
                    USER _u = iDB.GetAll<USER>(MAIN_ID: "713642fb9b624405aa6b8efcfa6beae0").
                        Where(p => p.CONTENT1.Equals(model.CONTENT1)
                        && p.PASSWORD.Equals(pwd)
                        && string.IsNullOrEmpty(p.CONTENT30)).FirstOrDefault();
                    if (_u != null)
                    {
                        _u.DATETIME2 = DateTime.Now;
                        iDB.Save();
                        SetLogonData(_u);
                        Msgbox_Toast(LOGON_SUCCESS);
                        _msg = "1";
                    }
                }

                #endregion 登入
            }
            return Content(_msg);
        }

        public ActionResult RegisterVerify(string id)
        {
            bool isReg = !id.IsNullOrEmpty();

            if (isReg)
            {
                #region 驗證

                USER _u = iDB.GetAll<USER>(MAIN_ID: "fun6000").
                        Where(p => p.CONTENT30.Equals(id)
                        && !p.DATETIME4.HasValue).FirstOrDefault();
                if (_u != null)
                {
                    _u.CONTENT30 = string.Empty;
                    _u.DATETIME4 = DateTime.Now;
                    iDB.Save();
                    SetLogonData(_u);
                    Msgbox_Toast("會員驗證成功!!");
                }
                else
                {
                    Msgbox_Toast("會員驗證失敗!!", AlertMsgType.Error);
                }

                #endregion 驗證
            }
            return GoIndex();
        }

        public ActionResult Regtest(string id)
        {
            bool isReg = id.IsNullOrEmpty();
            string _act = isReg ? Resource.register : Resource.login;
            string _msg = $"{_act}失敗!!";

            if (isReg)
            {
                #region 註冊

                USER user = new USER();
                user.USER_ID = Function.GetGuid();
                user.CREATER = "web";//前台註冊
                user.MAIN_ID = "fun6000";
                user.PASSWORD = Function.DEFAULT_PASSWORD_SETUP.ToSHA1();
                user.CONTENT1 = "aaa@test.com";//帳號 email
                user.CONTENT2 = "姓";//姓
                user.CONTENT3 = "名";//名字
                user.DATETIME2 = DateTime.Now;
                //備註
                //PARAGRAPH ph = new PARAGRAPH();
                //ph.ID = Function.GetGuid();
                //ph.CREATE_DATE = DateTime.Now;
                //ph.CREATER = user.USER_ID;
                //ph.ORDER = 1;
                //user.PARAGRAPH.Add(ph);
                if (iDB.Add<USER>(user))
                {
                    //Send(user);
                    SetLogonData(user);
                    Msgbox_Toast("謝謝您的註冊！！");
                    _msg = "1";
                }

                #endregion 註冊
            }
            else//登入
            {
                #region 登入

                USER _u = iDB.GetAll<USER>(MAIN_ID: "fun6000").
                        Where(p => p.USER_ID.Equals(id)
                        ).FirstOrDefault();
                SetLogonData(_u);
                Msgbox_Toast(LOGON_SUCCESS);
                _msg = "1";

                #endregion 登入
            }
            //return Content(_msg);
            return GoIndex();
        }

        /// <summary>
        /// 會員驗證碼通知信 index_02
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public bool Send(USER d1)
        {
            //CreateMemberPlus(d1);
            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/index_02.html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sBody = string.Format(_htmlBody, d1.CONTENT1, $"{Function.DEFAULT_ROOT_HTTP}RegisterVerify/{d1.CONTENT30}");
            LetterModel model = new LetterModel();
            model.Body = sBody;
            model.Subject = $"[{Function.PAGE_TITLE}]會員驗證通知信";
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(d1.CONTENT1, d1.CONTENT2);
            bool bRet = Function.SendMail(model);
            if (!bRet)
            {
                //寄送不成功就留Log
                LogSystem.InitLogSystem();
                LogSystem.WriteLine("[TO] " + d1.CONTENT1 + " [BODY] " + sBody);
                LogSystem.CloseUnderlayingStream();
            }
            return bRet;
        }

        /// <summary>
        /// 優惠方案通知信 index_03
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public bool SendData2N(DATA2 d1)
        {
            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/index_07.html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sRecipient = d1.CONTENT16.IsNullOrEmpty() ? d1.CONTENT11 : d1.CONTENT16.ToMyString().Trim();
            //string sBody = string.Format(_htmlBody, d1.CONTENT1, $"{Function.DEFAULT_ROOT_HTTP}Discount/{d1.ID}");
            string sBody = string.Format(_htmlBody, d1.CONTENT11, d1.CONTENT12, d1.ID);
            LetterModel model = new LetterModel();
            model.Body = sBody;
            //model.Subject = $"[{Function.PAGE_TITLE}]優惠方案確認信";
            DateTime _deadline = DateTime.Today.AddDays(1);
            //model.Subject = $"【交通部觀光局】自行車友善旅宿優惠方案確認信，煩請於{_deadline.Month} / {_deadline.Day}前確認完畢，謝謝！";
            model.Subject = "交通部觀光局合法旅宿「旅宿快篩」抽獎活動 得獎通知信函(請回覆領獎資訊)";
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(sRecipient, sRecipient);
            bool bRet = Function.SendMail(model);
            d1.DATETIME2 = DateTime.Now;
            d1.DECIMAL10 = 1; //成功
            if (!bRet)
            {
                d1.DECIMAL10 = 0; //失敗

                //寄送不成功就留Log
                LogSystem.InitLogSystem();
                LogSystem.WriteLine("<!--TO：" + sRecipient + "-->");
                LogSystem.WriteLine(sBody);
                LogSystem.CloseUnderlayingStream();
            }
            iDB.Save();
            return bRet;
        }

        /// <summary>
        /// 建立PLUS資料
        /// </summary>
        /// <param name="model"></param>
        private void CreateData1Plus(DATA1 model, string pt = "fun2001", string c4 = "0")
        {
            PLUS data = new PLUS();
            data.ID = Function.GetGuid();
            data.MAIN_ID = model.ID;
            data.CREATER = User.Identity.Name;
            data.PLUS_TYPE = pt;
            data.STATUS = "1";//預設1
            data.CREATE_DATE = DateTime.Now;
            data.CONTENT1 = Request.UserHostAddress;
            data.CONTENT2 = Request.UserAgent;
            data.CONTENT3 = model.DATA_TYPE;
            data.CONTENT4 = c4;
            data.CONTENT5 = model.NODE_ID;
            data.DATETIME1 = DateTime.Today;
            if (iDB.Add<PLUS>(data))
            {
                //執行sql
                string _sql = $"update DATA1 set DECIMAL1=(select count(*) from plus where [ENABLE]=1 and PLUS_TYPE='fun2001' and MAIN_ID='{model.ID}'), DECIMAL2 = (select count(*) from plus where [ENABLE] = 1 and PLUS_TYPE = 'fun2002' and MAIN_ID = '{model.ID}') where id = '{model.ID}'";
                iDB.ExecuteSqlCommand(_sql);
                SetData1List();
            }
        }

        /// <summary>
        /// 建立會員送的5點
        /// </summary>
        /// <param name="d1"></param>
        private void CreateMemberPlus(USER fun6000)
        {
            PLUS data = new PLUS();
            data.ID = Function.GetGuid();
            data.MAIN_ID = fun6000.USER_ID;
            data.CREATER = "createMember";
            data.PLUS_TYPE = "fun5002";
            data.STATUS = "1";//預設1
            data.CREATE_DATE = DateTime.Now;
            data.CONTENT1 = "";
            data.CONTENT2 = fun6000.CONTENT1;
            data.CONTENT3 = fun6000.CONTENT2 + fun6000.CONTENT3;
            data.CONTENT5 = "加入會員送5點";
            data.CONTENT6 = fun6000.USER_ID;
            data.DATETIME1 = DateTime.Today;
            data.DATETIME2 = data.DATETIME1.Value.AddYears(1);
            data.DECIMAL5 = 5;
            iDB.Add<PLUS>(data);
        }

        /// <summary>
        /// 設定登入相關資訊
        /// </summary>
        /// <param name="id"></param>
        private void SetLogonData(USER user)
        {
            SetLogOnAuthCookie(user.USER_ID);
            Session[Function.SESSION_ROLE] = user.CONTENT1;//帳號(email)
                                                           //Session[Function.SESSION_NAME] = user.CONTENT2 + user.CONTENT3;//姓+名
            Session[Function.SESSION_NAME] = user.CONTENT2;//姓名
        }

        #region 社群快速登入、註冊

        //Line
        [NonAction]
        public ActionResult LineLoginDirect()
        {
            string response_type = "code";
            string client_id = "1656702982";
            string redirect_uri = HttpUtility.UrlEncode(Function.DEFAULT_ROOT_HTTP + "RegisterByLine");
            //string redirect_uri = HttpUtility.UrlEncode("https://192.168.0.197:168/" + "RegisterByLine");
            string state = "allPlayall";
            string LineLoginUrl = string.Format("https://access.line.me/oauth2/v2.1/authorize?response_type={0}&client_id={1}&redirect_uri={2}&state={3}&scope=profile%20openid%20email",
                response_type,
                client_id,
                redirect_uri,
                state
                );
            return Redirect(LineLoginUrl);
        }

        //驗證FB登入的
        public ActionResult RegisterByFB(string JSON, string id, string type)
        {
            string _msg = "";
            FBUserData fbData = new FBUserData(JObject.Parse(JSON));
            if (!fbData.email.IsEmail() || fbData.id.IsNullOrEmpty())
            {
                _msg = "FB登入失敗!!";
                return Content(_msg);
            }
            //USER user = iDB.GetAll<USER>(MAIN_ID: "fun6000")
            //    .Where(p => p.CONTENT1.Equals(fbData.email)
            //&& p.CONTENT11.Equals(fbData.id)).FirstOrDefault();
            USER user = iDB.GetAll<USER>(MAIN_ID: "fun6000")
                .Where(p => p.CONTENT1.Equals(fbData.email)).FirstOrDefault();
            //檢查帳號是否註冊過
            if (user != null)//有此email
            {
                if (user.CONTENT11.IsNullOrEmpty())//無fb id
                {
                    user.CONTENT11 = fbData.id;
                }
                user.DATETIME2 = DateTime.Now;
                iDB.Save();
            }
            else//無
            {
                user = new USER();
                user.USER_ID = Function.GetGuid();
                user.CREATER = "web_FB";//前台FB註冊
                user.MAIN_ID = "fun6000";
                user.PASSWORD = Function.DEFAULT_PASSWORD_SETUP.ToSHA1();
                user.CONTENT1 = fbData.email;//帳號 email
                user.CONTENT2 = fbData.name;//姓
                user.CONTENT3 = fbData.name;//名字
                                            //user.CONTENT4 = SexType.Male.ToIntValue();//性別 男0 女1
                                            //user.CONTENT5 = "";//電話
                                            //user.CONTENT6 = "level01";//會員等級
                                            //user.CONTENT7 = "自行註冊";//來源
                user.CONTENT11 = fbData.id;
                user.DATETIME2 = DateTime.Now;
                user.DATETIME4 = DateTime.Now;
                if (iDB.Add<USER>(user))
                {
                    //Send(user);
                }
            }
            SetLogonData(user);
            Msgbox_Toast(LOGON_SUCCESS);
            _msg = "1";
            return Content(_msg);
        }

        //驗證line登入的
        [NonAction]
        public ActionResult RegisterByLine(string code, string state)
        {
            if (state == "allPlayall")
            {
                LogSystem.InitLogSystem();

                #region Api變數宣告

                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string result = string.Empty;
                NameValueCollection nvc = new NameValueCollection();

                #endregion Api變數宣告

                try
                {
                    //取回Token
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string ApiUrl_Token = "https://api.line.me/oauth2/v2.1/token";
                    nvc.Add("grant_type", "authorization_code");
                    nvc.Add("code", code);
                    nvc.Add("redirect_uri", Function.DEFAULT_ROOT_HTTP + "RegisterByLine");
                    //nvc.Add("redirect_uri", "https://192.168.0.197:168/" + "RegisterByLine");
                    nvc.Add("client_id", "1656702982");
                    nvc.Add("client_secret", "72d9d0594439009b50789b3266a50f71");
                    string JsonStr = Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Token, "POST", nvc));
                    LogSystem.WriteLine(JsonStr);
                    LineLoginToken ToKenObj = JsonConvert.DeserializeObject<LineLoginToken>(JsonStr);
                    wc.Headers.Clear();

                    //取回User Profile
                    string ApiUrl_Profile = "https://api.line.me/v2/profile";
                    wc.Headers.Add("Authorization", "Bearer " + ToKenObj.access_token);
                    string UserProfile = wc.DownloadString(ApiUrl_Profile);
                    LogSystem.WriteLine(UserProfile);
                    LineProfile ProfileObj = JsonConvert.DeserializeObject<LineProfile>(UserProfile);
                    //取email
                    var jst = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(ToKenObj.id_token);
                    if (jst.Payload.ContainsKey("email") && !string.IsNullOrEmpty(Convert.ToString(jst.Payload["email"])))
                    {//有包含email，使用者有授權email個資存取，並且用戶的email有值
                        ProfileObj.email = jst.Payload["email"].ToString();
                    }

                    //檢查帳號是否註冊過 是否填寫資訊
                    USER user = iDB.GetAll<USER>(MAIN_ID: "fun6000")
                .Where(p => p.CONTENT1.Equals(ProfileObj.email)).FirstOrDefault();
                    if (user != null)//有此email
                    {
                        if (user.CONTENT12.IsNullOrEmpty())
                        {
                            user.CONTENT12 = ProfileObj.userId;
                        }
                        user.DATETIME2 = DateTime.Now;
                        iDB.Save();
                    }
                    else
                    {
                        user = new USER();
                        user.USER_ID = Function.GetGuid();
                        user.CREATER = "web_Line";//前台line註冊
                        user.MAIN_ID = "fun6000";
                        user.PASSWORD = Function.DEFAULT_PASSWORD_SETUP.ToSHA1();
                        user.CONTENT1 = ProfileObj.email;//帳號 email
                        user.CONTENT2 = ProfileObj.displayName;//姓
                        user.CONTENT3 = ProfileObj.displayName;//名字
                        user.CONTENT4 = SexType.Male.ToIntValue();//性別 男0 女1
                        user.CONTENT5 = "";//電話
                        user.CONTENT6 = "level01";//會員等級
                        user.CONTENT7 = "自行註冊";//來源
                        user.CONTENT12 = ProfileObj.userId;
                        user.DATETIME2 = DateTime.Now;
                        if (iDB.Add<USER>(user))
                        {
                            Send(user);
                        }
                    }
                    SetLogonData(user);
                    Msgbox_Toast(LOGON_SUCCESS);
                }
                catch (Exception ex)
                {
                    Msgbox_Toast("Line登入失敗！！", AlertMsgType.Error);
                    string msg = ex.Message;
                    LogSystem.WriteLine(ex.ToString());
                    //throw;
                }
                LogSystem.CloseUnderlayingStream();
            }
            return GoIndex();
        }

        //Google Oauth2

        #region Google用

        public ActionResult GoogleLoginDirect()
        {
            string ClientId = "210957441864-ajeprhj8j1injb3fgbrlakrn7q1n5fd1.apps.googleusercontent.com";
            string ClientSecret = "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";
            string RedirectUri = "https://localhost:72/RegisterByGoogle";
            string[] Scopes = { "profile", "email" };
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                },
                Scopes = Scopes
            });
            var url = flow.CreateAuthorizationCodeRequest(RedirectUri).Build();
            return Redirect(url.ToString());
        }

        //驗證google登入的Oauth2
        public ActionResult RegisterByGoogle(string code)
        {
            //LogSystem.InitLogSystem();
            #region Api變數宣告
            var clientId = "210957441864-ajeprhj8j1injb3fgbrlakrn7q1n5fd1.apps.googleusercontent.com";
            var clientSecret = "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";
            var redirectUri = "https://localhost:72/RegisterByGoogle";
            var scopes = new[] { "email", "profile" };
            #endregion Api變數宣告


            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = scopes
            };

            var flow = new GoogleAuthorizationCodeFlow(initializer);

            var token = flow.ExchangeCodeForTokenAsync("user", code, redirectUri, CancellationToken.None).Result;
            var ProfileObj = GoogleJsonWebSignature.ValidateAsync(token.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            }).Result;

            

            try
            {
                //檢查帳號是否註冊過 是否填寫資訊
                USER user = iDB.GetAll<USER>(MAIN_ID: "fun6000")
            .Where(p => p.CONTENT1.Equals(ProfileObj.Email)).FirstOrDefault();
                if (user != null)//有此email
                {
                    if (user.CONTENT13.IsNullOrEmpty())
                    {
                        user.CONTENT13 = ProfileObj.Subject;
                    }
                    user.DATETIME2 = DateTime.Now;
                    iDB.Save();
                }
                else
                {
                    user = new USER();
                    user.USER_ID = Function.GetGuid();
                    user.CREATER = "web_Google";//前台google註冊
                    user.MAIN_ID = "fun6000";
                    user.PASSWORD = Function.DEFAULT_PASSWORD_SETUP.ToSHA1();
                    user.CONTENT1 = ProfileObj.Email;//帳號 email
                                                     //user.CONTENT2 = ProfileObj.displayName;//姓
                                                     //user.CONTENT3 = ProfileObj.displayName;//名字
                    user.CONTENT2 = ProfileObj.FamilyName + ProfileObj.GivenName;//姓+名字
                    user.CONTENT3 = ProfileObj.FamilyName + ProfileObj.GivenName;//姓+名字
                                                                                   //user.CONTENT3 = ProfileObj.given_name;//名字
                                                                                   //user.CONTENT4 = SexType.Male.ToIntValue();//性別 男0 女1
                                                                                   //user.CONTENT5 = "";//電話
                                                                                   //user.CONTENT6 = "level01";//會員等級
                                                                                   //user.CONTENT7 = "自行註冊";//來源
                    user.CONTENT13 = ProfileObj.Subject;// ID token
                    user.DATETIME2 = DateTime.Now;
                    user.DATETIME4 = DateTime.Now;
                    if (iDB.Add<USER>(user))
                    {
                        //Send(user);
                    }
                }
                SetLogonData(user);
                Msgbox_Toast(LOGON_SUCCESS);
            }
            catch (Exception ex)
            {
                Msgbox_Toast("Google登入失敗！！", AlertMsgType.Error);
                string msg = ex.Message;
            }
            return GoIndex();
        }

        #endregion Google用

        #endregion 社群快速登入、註冊

        #region 忘記密碼

        //忘記密碼
        public ActionResult Forget()
        {
            string b1 = "忘記密碼";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        [HttpPost]
        public ActionResult Forget(ForgetModel model, string captcha)
        {
            if (ModelState.IsValid && model.email.IsEmail())
            {
                USER user = iDB.GetAll<USER>().Where(p =>
                     p.CONTENT1.Equals(model.email)
                    ).FirstOrDefault();
                if (user != null)
                {
                    string _tmppwd = "a" + Function.GetGuid().Substring(0, 7);
                    string _shapwd = _tmppwd.ToSHA1();
                    user.PASSWORD = _shapwd;
                    user.DATETIME3 = DateTime.Now;
                    iDB.Save();
                    //預設內容改由html讀入index_05
                    string _htmlBody = string.Empty;
                    using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/index_05.html")))
                    {
                        _htmlBody = reader.ReadToEnd();
                    }
                    string _body = string.Format(_htmlBody, _tmppwd, Function.DEFAULT_ROOT_HTTP);
                    LetterModel letter = new LetterModel();
                    letter.Body = _body;
                    letter.Subject = $"{Function.PAGE_TITLE} 密碼重新設定信";
                    letter.RecipientNameList = new Dictionary<string, string>();
                    letter.RecipientNameList.Add(user.CONTENT1, user.CONTENT2);
                    bool bRet = Function.SendMail(letter);
                    if (bRet)
                    {
                        Msgbox_Toast("密碼重新設定信已寄出！！");
                    }
                    else
                    {
                        Msgbox_Toast("密碼重新設定信寄送失敗！！", AlertMsgType.Error);
                    }
                    return GoIndex();
                }
            }
            SetModelStateError("帳號不存在！！");
            string b1 = "忘記密碼";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View(model);
        }

        #endregion 忘記密碼

        #region 密碼重設

        [HttpPost]
        public ActionResult ReSetPw(ForgetModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return GoIndex();
            }
            if (!model.email.IsNullOrEmpty())
            {
                USER user = iDB.GetByID<USER>(User.Identity.Name);
                if (user != null)
                {
                    string _tmppwd = model.email.ToSHA1();
                    user.PASSWORD = _tmppwd;
                    user.DATETIME1 = DateTime.Now;
                    iDB.Save();
                    Msgbox_Toast("密碼已修改！！");
                }
            }
            return GoIndex();
        }

        //密碼重設
        public ActionResult ReSetPw()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return GoIndex();
            }
            string b1 = "密碼重設";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        #endregion 密碼重設

        #region 各種小密方

        public ActionResult CleanVoteCount()
        {
            if ("118.163.88.193".Equals(Request.UserHostAddress))
            {
                //執行sql
                string _sql = $"delete from plus";
                iDB.ExecuteSqlCommand(_sql);
                return Content("已清除!!");
            }
            return GoIndex();
        }

        public ActionResult HotelsResult()
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "目前排名";
                PageTitle = b1;
                return View(iDB.GetAllAsNoTracking<USER>()
                    .OrderByDescending(p => p.CONTENT5.Length)
                    .ThenByDescending(p => p.CONTENT5)
                    .Take(20).ToList());
            }
            return GoIndex();
        }

        public ActionResult Partner(string id)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "遊戲記錄";
                PageTitle = b1;
                return View(iDB.GetAllAsNoTracking<PLUS>()
                    .Where(p => p.CREATER.Equals(id))
                    .OrderByDescending(p => p.DATETIME1)
                    .ThenByDescending(p => p.CREATE_DATE).ToList());
            }
            return GoIndex();
        }

        public ActionResult UpdateDataCount()
        {
            //if ("118.163.88.193".Equals(Request.UserHostAddress))
            //{
            //    List<DATA1> list = iDB.GetAll<DATA1>().ToList();
            //    foreach (var item in list)
            //    {
            //        item.DECIMAL1 = item.PLUS.Where(p => 1 == p.ENABLE && PLUS_TYPE1.Equals(p.PLUS_TYPE)).Count();
            //        item.DECIMAL2 = item.PLUS.Where(p => 1 == p.ENABLE && PLUS_TYPE2.Equals(p.PLUS_TYPE)).Count();
            //    }
            //    iDB.Save();
            //    SetData1List();
            //    return Content("已更新!!");
            //}
            return GoIndex();
        }

        public ActionResult UpdateVoteCount()
        {
            if ("118.163.88.193".Equals(Request.UserHostAddress))
            {
                List<USER> list = iDB.GetAll<USER>().Where(p => "1".Equals(p.CONTENT20)).ToList();
                int _total = 0;
                foreach (var item in list)
                {
                    //取得登入者今日的PLUS資料
                    List<PLUS> total = iDB.GetAllAsNoTracking<PLUS>().Where(p => p.CREATER.Equals(item.USER_ID)
                       && p.DATETIME1 == DateTime.Today
                    ).ToList();
                    int _countStr = (DEFAULT_VOTE - total.Count);
                    if (_countStr > 0)
                    {
                        for (int i = 0; i < _countStr; i++)
                        {
                            PLUS data = new PLUS();
                            data.ID = Function.GetGuid();
                            data.MAIN_ID = "*";
                            data.CREATER = item.USER_ID;
                            data.PLUS_TYPE = "fun2001";
                            data.STATUS = "1";//預設1
                            data.CREATE_DATE = DateTime.Now;
                            data.CONTENT1 = Request.UserHostAddress;
                            data.CONTENT2 = Request.UserAgent;
                            data.CONTENT3 = item.CONTENT1;
                            data.DECIMAL1 = 300;//總分
                            data.DECIMAL2 = DEFAULT_VOTE;//生命
                            data.DECIMAL3 = 0;//遊戲秒數
                            data.DECIMAL4 = 300;//總分
                            data.DECIMAL5 = DEFAULT_VOTE;//生命
                            data.DECIMAL6 = 62;//初始秒數
                            data.DATETIME1 = DateTime.Today;
                            data.DATETIME2 = data.CREATE_DATE;
                            data.DATETIME3 = data.CREATE_DATE.AddSeconds(62);
                            iDB.Add<PLUS>(data);
                        }
                        _total += _countStr;
                        string _c = item.USER_ID;
                        //執行sql
                        string _sql = $"update [USER] set CONTENT5=(select SUM(DECIMAL1) from plus where [ENABLE]=1 and PLUS_TYPE='fun2001' and CREATER='{_c}'), CONTENT6 = (select count(ID) from plus where [ENABLE] = 1 and PLUS_TYPE = 'fun2001' and CREATER = '{_c}') where USER_ID = '{_c}'";
                        iDB.ExecuteSqlCommand(_sql);
                    }
                }
                return Content($"已玩完 {list.Count} 會員，共 {_total} 次 !!");
            }
            return GoIndex();
        }

        #endregion 各種小密方

        #endregion 會員相關

        //旅宿快篩小遊戲
        public ActionResult Game()
        {
            //string b1 = Resource.footer06;
            string b1 = "旅宿快篩小遊戲";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        //旅宿快篩小遊戲2
        public ActionResult Game2()
        {
            string b1 = "旅宿快篩小遊戲2";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        #region 安心住

        //安心住
        public ActionResult Stay()
        {
            string b1 = "安心住";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        #endregion 安心住

        #region 旅宿快閃活動

        //旅宿快閃活動
        public ActionResult Popup()
        {
            string b1 = "旅宿快閃活動";
            ViewBag.b1 = b1;
            PageTitle = b1;
            return View();
        }

        #endregion 旅宿快閃活動

        #region 拍照上傳活動 20221121 add

        public ActionResult Pic()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Msgbox_Toast("請先登入", AlertMsgType.Notice);
                return GoIndex();
            }
            string b1 = "拍照上傳";
            ViewBag.b1 = b1;
            PageTitle = b1;

            #region 建立可選擇的下拉選單
            bool isDate = false;
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                isDate = true;
            }
            List<ATTACHMENT> atts = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: User.Identity.Name).ToList();
            List<SelectListItem> dateList = new List<SelectListItem>();
            foreach (var str in Function.DataRangeList)
            {
                DateTime _date = str.ToDateTime();
                if (!atts.Any(p => str.Equals(p.CONTENT1)) && (isDate || _date <= DateTime.Today))
                {
                    dateList.Add(new SelectListItem { Text = $"{str}{Function.WeekDayList[_date.DayOfWeek.ToInt()]}", Value = str });
                }
            }
            ViewBag.dateSelect = new SelectList(dateList, "Value", "Text");
            #endregion

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pic(string c1, string c2, HttpPostedFileBase pics)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Msgbox_Toast("請先登入", AlertMsgType.Notice);
                return GoIndex();
            }
            DateTime _end = new DateTime(2022, 12, 31, 18, 0, 0);
            if (DateTime.Now > _end)
            {
                Msgbox_Toast("活動日期已過", AlertMsgType.Notice);
                return GoIndex();
            }
            string msg = "上傳失敗!!";
            AlertMsgType _altType = AlertMsgType.Notice;
            List<ATTACHMENT> atts = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: User.Identity.Name).ToList();
            DATA3 d3 = Function.Data3List.FirstOrDefault(p => p.CONTENT2.Equals(c2));
            if (Function.DataRangeList.Any(p => p.Equals(c1)) && !atts.Any(p => p.CONTENT1.Equals(c1)) && d3 != null && pics != null && pics.ContentLength > 0)
            {
                Image image = null;
                try
                {
                    image = Image.FromStream(pics.InputStream);
                    //int rotate = 0;
                    //               foreach (var prop in image.PropertyItems)
                    //               {
                    //                   if (prop.Id == 0x112|| prop.Id == 274)
                    //                   {
                    //		if (prop.Value[0] == 6)
                    //                           rotate = 90;
                    //                       else if (prop.Value[0] == 8)
                    //                           rotate = -90;
                    //                       else if (prop.Value[0] == 3)
                    //                           rotate = 180;
                    //		//prop.Value[0] = 1;
                    //		break;
                    //                   }
                    //               }
                    //if (rotate == 90)
                    //{
                    //    image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    //    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    //}
                    //else if (rotate == -90)
                    //{
                    //    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    //}
                    //else if (rotate == 180)
                    //{
                    //    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    //}
                    AttachmentType attType = AttachmentType.Image;
                    ATTACHMENT att = new ATTACHMENT(pics.FileName);
                    att.ATT_TYPE = attType.ToIntValue();
                    if (att.EXTENSION.IsNullOrEmpty())
                    {
                        att.GetRealExtension(image);
                    }
                    att.SetUpFileName();
                    att.MAIN_ID = User.Identity.Name;
                    att.CREATER = User.Identity.Name;
                    att.CONTENT1 = c1;
                    att.CONTENT2 = c2;
                    att.CONTENT3 = d3.CONTENT3;
                    att.CONTENT4 = Session[Function.SESSION_ROLE].ToMyString();
                    att.CONTENT5 = pics.ContentLength.ToMyString();
                    att.CONTENT6 = d3.CONTENT1;

                    //MemoryStream ms = new MemoryStream();
                    //image.Save(ms, image.RawFormat);

                    //SavePicture(new WebImage(pics.InputStream), att, rotate);
                    //SavePicture(new WebImage(ms.ToArray()), att);
                    SaveAtt(pics, att.FILE_NAME);
                    if (iDB.Add<ATTACHMENT>(att))
                    {
                        msg = "上傳成功!!";
                        _altType = AlertMsgType.Success;
                    }

                }
                catch (Exception ex)
                {
                    LogSystem.InitLogSystem();
                    LogSystem.WriteLine(ex.ToString());
                    LogSystem.CloseUnderlayingStream();
                }

            }
            Msgbox_Toast(msg, _altType);
            return RedirectToAction("Pic");
        }

        public ActionResult PicList()
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "已上傳照片";
                ViewBag.b1 = b1;
                PageTitle = b1;
                ViewBag.picTotal = iDB.GetAllAsNoTracking<ATTACHMENT>().Where(p => p.ORDER == 0).Count();
                return View();
            }
            return GoIndex();

        }

        [HttpPost]
        public ActionResult PicList(string kk)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "已上傳照片";
                ViewBag.b1 = b1;
                PageTitle = b1;
                USER user = iDB.GetAllAsNoTracking<USER>().FirstOrDefault(p => p.CONTENT1.Equals(kk));
                if (user != null)
                {
                    ViewBag.kk = kk;
                    return View(iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: user.USER_ID).OrderBy(p => p.CONTENT1).ToList());
                }
                else
                {
                    Msgbox_Toast("無此會員資料", AlertMsgType.Notice);
                    return RedirectToAction("PicList");
                }
            }
            return GoIndex();

        }

        public ActionResult PicList2()
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "已上傳照片";
                ViewBag.b1 = b1;
                PageTitle = b1;
                return View(iDB.GetAllAsNoTracking<ATTACHMENT>().Where(p => p.ORDER == 0).OrderBy(p => p.CREATE_DATE));
            }
            return GoIndex();

        }

        [HttpPost]
        public ActionResult PicList2(string id)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "已上傳照片";
                ViewBag.b1 = b1;
                PageTitle = b1;
                ATTACHMENT att = iDB.GetByID<ATTACHMENT>(id);
                if (att != null)
                {
                    if (att.CONTENT7.IsTrue())
                    {
                        att.CONTENT7 = "false";
                    }
                    else
                    {
                        att.CONTENT7 = "true";
                    }
                    iDB.Save();
                    return Content("1");
                }
            }
            return Content("0");

        }

        public ActionResult Import3(int? t)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                return View();
            }
            return GoIndex();
        }

        /// <summary>
        /// use NPOI
        /// </summary>
        /// <param name="excel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import3(HttpPostedFileBase excel, int? t)
        {
            List<string> results = new List<string>();
            if (excel != null && ("192.168.0.197".Equals(Request.UserHostAddress) || "118.163.88.193".Equals(Request.UserHostAddress)))
            {
                if (excel.ContentLength > 0)
                {
                    if (!t.HasValue)//刪除資料
                    {
                        iDB.ExecuteSqlCommand("DELETE from DATA3");
                        //iDB.ExecuteSqlCommand("DELETE from PLUS");
                        //iDB.ExecuteSqlCommand("DELETE from attachment");
                        //iDB.ExecuteSqlCommand("DELETE from PARAGRAPH");
                    }
                    //iData1.ExecuteSqlCommand("DELETE from PARAGRAPH where custom3='dannyImport'");
                    string mark = "dannyImport";
                    int _newCount = 0;
                    List<DATA3> data3s = iDB.GetAll<DATA3>().ToList();
                    HSSFWorkbook book = new HSSFWorkbook(excel.InputStream);

                    for (int j = 0; j < book.NumberOfSheets; j++)
                    {
                        ISheet sheet = book.GetSheetAt(j);

                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            string _id = row.Cells[0].ToMyString();
                            string _city = row.Cells[7].ToMyString();//縣市
                            string _town = row.Cells[8].ToMyString();//鄉鎮
                            string _c1 = row.Cells[6].ToMyString();//名稱
                            string _c2 = row.Cells[2].ToMyString();//證號
                            bool isAdd = false;
                            if (!_c1.IsNullOrEmpty())
                            {
                                NODE city = Function.NodeList.FirstOrDefault(p => p.TITLE.Equals(_city)
                                && Function.NODE_ID_CITYINFO.Equals(p.PARENT_ID));
                                if (city == null)
                                {
                                    results.Add($"編號：{j}-{i}，{_c1} 縣市找不到 {_city}");
                                    continue;
                                }
                                NODE town = Function.NodeList.FirstOrDefault(p => p.TITLE.Equals(_town));
                                if (town == null)
                                {
                                    results.Add($"編號：{j}-{i}，{_c1} 鄉鎮找不到 {_town}");
                                    continue;
                                }
                                string nid = row.Cells[3].ToMyString();//旅宿類型

                                DATA3 d1 = data3s.FirstOrDefault(p => p.CONTENT2.Equals(_c2));
                                if (d1 == null)
                                {
                                    d1 = new DATA3();
                                    d1.CREATER = mark;
                                    d1.DATA_TYPE = "*";
                                    d1.STATUS = "1";
                                    isAdd = true;
                                }
                                d1.NODE_ID = nid;//旅宿類型
                                d1.CONTENT1 = _c1;//旅宿名稱
                                d1.CONTENT2 = _c2;//證號
                                d1.CONTENT3 = _city;//縣市
                                d1.CONTENT4 = row.Cells[10].ToMyString();//地址
                                d1.CONTENT5 = row.Cells[11].ToMyString();//電話
                                string _c7 = row.Cells[12].ToMyString();//傳真
                                if ("-".Equals(_c7))
                                {
                                    _c7 = string.Empty;
                                }
                                d1.CONTENT6 = _c7;//傳真
                                                  //d1.CONTENT7 = row.Cells[9].ToMyString().ToHttpUrl();//官方網站
                                                  //d1.CONTENT8 = _id;//旅宿網ID
                                d1.CONTENT9 = row.Cells[4].ToMyString();//標章
                                d1.CONTENT10 = city.ID;
                                d1.CONTENT12 = _town;//鄉鎮
                                d1.CONTENT13 = town.ID;
                                d1.CONTENT14 = row.Cells[9].ToMyString();//郵遞區號
                                d1.DECIMAL1 = row.Cells[13].ToMyString().ToInt();//房間數
                                d1.DATETIME1 = row.Cells[1].ToMyString().ToDateTime();//核准登記營業日期
                                                                                      //內容
                                                                                      //string _content = row.Cells[13].ToMyString();
                                                                                      //d1.CONTENT8 = _content.IsNullOrEmpty() ? "" : "1";
                                                                                      //PARAGRAPH p1 = d1.GetParagraphByOrder();
                                                                                      //p1.ID = Function.GetGuid();
                                                                                      //p1.CREATE_DATE = DateTime.Now;
                                                                                      //p1.CREATER = mark;
                                                                                      //p1.ORDER = 1;
                                                                                      //d1.PARAGRAPH.Add(p1);
                                                                                      //if (p1 == null)
                                                                                      //{
                                                                                      //    p1 = new PARAGRAPH();
                                                                                      //    p1.ID = Function.GetGuid();
                                                                                      //    p1.CREATE_DATE = DateTime.Now;
                                                                                      //    p1.CREATER = mark;
                                                                                      //    p1.ORDER = 1;
                                                                                      //    d1.PARAGRAPH.Add(p1);
                                                                                      //}
                                                                                      //if (!_content.IsNullOrEmpty())
                                                                                      //{
                                                                                      //    _content = _content.Replace("；", "\r\n");
                                                                                      //}
                                                                                      //p1.CONTENT = _content;

                                //附件
                                //string _pic = row.Cells[3].ToMyString();
                                //ATTACHMENT att = new ATTACHMENT(_pic);
                                //att.ATT_TYPE = AttachmentType.Image.ToIntValue();
                                //att.SetUpFileName();
                                //att.CREATER = mark;
                                //d1.ATTACHMENT.Add(att);
                                //string _path = @"E:\專案\2018\09菲律賓\doc\pic_little\" + _pic;
                                ////Open the stream and read it back.
                                //using (FileStream fs = System.IO.File.OpenRead(_path))
                                //{
                                //    SavePicture(new System.Web.Helpers.WebImage(fs), att);
                                //}
                                if (isAdd)
                                {
                                    iDB.Add<DATA3>(d1);
                                    _newCount++;
                                }
                            }
                        }
                    }
                    iDB.Save();
                    results.Add($"共新增：{_newCount} 筆旅宿");
                    //SetData1List();
                }
            }
            Msgbox_Toast("ok");
            return View(results);
        }


        public ActionResult Import4(int? t)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                return View();
            }
            return GoIndex();
        }

        /// <summary>
        /// use NPOI
        /// </summary>
        /// <param name="excel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import4(HttpPostedFileBase excel, int? t)
        {
            List<string> results = new List<string>();
            if (excel != null && ("192.168.0.197".Equals(Request.UserHostAddress) || "118.163.88.193".Equals(Request.UserHostAddress)))
            {
                if (excel.ContentLength > 0)
                {
                    if (!t.HasValue)//不刪除資料
                    {
                        //iDB.ExecuteSqlCommand("DELETE from DATA1");
                        //iDB.ExecuteSqlCommand("DELETE from PLUS");
                        //iDB.ExecuteSqlCommand("DELETE from attachment");
                        //iDB.ExecuteSqlCommand("DELETE from PARAGRAPH");
                    }
                    //iData1.ExecuteSqlCommand("DELETE from PARAGRAPH where custom3='dannyImport'");
                    List<DATA1> data1s = iDB.GetAll<DATA1>().Where(p => p.DECIMAL1 == 0).ToList();
                    HSSFWorkbook book = new HSSFWorkbook(excel.InputStream);

                    for (int j = 0; j < book.NumberOfSheets; j++)
                    {
                        ISheet sheet = book.GetSheetAt(j);

                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            string _id = row.Cells[0].ToMyString();
                            string _city = row.Cells[7].ToMyString();//縣市
                            string _town = row.Cells[8].ToMyString();//鄉鎮
                            string _c1 = row.Cells[6].ToMyString();//名稱
                            string _c2 = row.Cells[2].ToMyString();//證號

                            DATA1 d1 = data1s.FirstOrDefault(p => p.CONTENT2.Equals(_c2));
                            if (d1 != null)
                            {
                                results.Add($"編號：{j}-{i}，{_c1} 證號找到 {_c2}");
                            }
                        }
                    }
                }
            }
            Msgbox_Toast("ok");
            return View(results);
        }
        #endregion

        #region 拍照上傳活動 得獎者回覆&名單公佈 20230109

        public ActionResult SendLotteryResult01092(string id)
        {
            int result = 0;
            if ("118.163.88.193".Equals(Request.UserHostAddress))
            {
                var list = iDB.GetAll<USER>().Where(p => id.Equals(p.USER_ID)).ToList();
                foreach (var u in list)
                {
                    if (SendData0109(u))
                    {
                        result++;
                    }
                }
            }
            return Content(result.ToMyString());
        }

        public ActionResult SendLotteryResult0109(string id)
        {
            int result = 0;
            if ("118.163.88.193".Equals(Request.UserHostAddress))
            {
                var list = iDB.GetAll<USER>().Where(p => id.Equals(p.CONTENT21)).ToList();
                foreach (var u in list)
                {
                    if (SendData0109(u))
                    {
                        result++;
                    }
                }
            }
            return Content(result.ToMyString());
        }

        /// <summary>
        /// 拍照上傳得獎通知信 index_08
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public bool SendData0109(USER d1)
        {
            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/index_08.html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sRecipient = d1.CONTENT1;
            //string sBody = string.Format(_htmlBody, d1.CONTENT1, $"{Function.DEFAULT_ROOT_HTTP}Discount/{d1.ID}");
            string _prize = d1.CONTENT22;
            if (_prize.StartsWith("0"))
            {
                _prize = _prize.Replace("0", string.Empty);
            }
            string sBody = string.Format(_htmlBody, d1.CONTENT1, _prize, d1.USER_ID);
            LetterModel model = new LetterModel();
            model.Body = sBody;
            model.Subject = "交通部觀光局合法旅宿「旅宿快篩」抽獎活動 得獎通知信函(請回覆領獎資訊)";
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(sRecipient, sRecipient);
            bool bRet = Function.SendMail(model);
            if (!bRet)
            {
                //寄送不成功就留Log
                LogSystem.InitLogSystem();
                LogSystem.WriteLine("<!--TO：" + sRecipient + "-->");
                LogSystem.WriteLine(sBody);
                LogSystem.CloseUnderlayingStream();
            }
            else
            {
                d1.CONTENT23 = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            iDB.Save();
            return bRet;
        }

        public ActionResult LotteryResult0109()
        {
            string b1 = "抽獎結果";
            ViewBag.b1 = b1;
            PageTitle = b1;
            //return View(iDB.GetAllAsNoTracking<USER>()
            //.Where(p => "20230109".Equals(p.CONTENT21)));
            return View(iDB.GetAllAsNoTracking<USER>()
                .Where(p => !string.IsNullOrEmpty(p.CONTENT21)));
        }

        #region 得獎者回覆專區 0109

        public ActionResult Reply0109(string id)
        {
            //if (checkEndDate())
            //{
            //	Msgbox_Toast("已超過領獎期限!!");
            //	return GoIndex();
            //}
            if (id.IsNullOrEmpty())
            {
                return GoIndex();
            }
            string b1 = "得獎者回覆專區";
            ViewBag.b1 = b1;
            PageTitle = b1;
            DataModel model = new DataModel();
            model.Atts = new List<ATTACHMENT>();

            USER d2 = iDB.GetByIDAsNoTracking<USER>(id);
            if (d2 != null && "20230109".Equals(d2.CONTENT21))
            {
                DateTime deadline = new DateTime(2023, 1, 16, 23, 59, 59);
                if ("c10cc2192eac47418e5b534aca742500".Equals(d2.USER_ID))
                {
                    deadline = new DateTime(2023, 1, 17, 23, 59, 59);
                }
                if (deadline < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({deadline.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }

                //獎項資訊
                model.CONTENT22 = d2.CONTENT22;
                //得獎人資料
                model.CONTENT1 = d2.CONTENT1;
                model.CONTENT2 = d2.CONTENT2;//姓名
                model.Atts = d2.ATTACHMENT.Where(p => p.ORDER == 3 || p.ORDER == 4).OrderBy(p => p.ORDER).ToList();

                ATTACHMENT reply = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == 20230109);

                if (reply != null)
                {
                    model.DATETIME1 = reply.CREATE_DATE;
                    model.CONTENT2 = reply.CONTENT1;//姓名
                    model.CONTENT3 = reply.CONTENT2;//寄送地址
                    model.CONTENT4 = reply.CONTENT3;//電話
                    model.Atts.Add(reply);
                }
            }
            else
            {
                return GoIndex();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply0109(string id, DataModel model
            , HttpPostedFileBase hpf1, HttpPostedFileBase hpf2
            , HttpPostedFileBase hpf3)
        {
            //if (checkEndDate())
            //{
            //	Msgbox_Toast("已超過領獎期限!!");
            //	return GoIndex();
            //}
            USER d2 = iDB.GetByID<USER>(id);
            if (d2 != null && "20230109".Equals(d2.CONTENT21))
            {
                DateTime deadline = new DateTime(2023, 1, 16, 23, 59, 59);
                if ("c10cc2192eac47418e5b534aca742500".Equals(d2.USER_ID))
                {
                    deadline = new DateTime(2023, 1, 17, 23, 59, 59);
                }
                if (deadline < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({deadline.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }
            }
            else
            {
                return GoIndex();
            }

            #region attachment
            List<HttpPostedFileBase> HPFs = new List<HttpPostedFileBase>() { hpf1, hpf2 };
            int i = 2;
            foreach (HttpPostedFileBase hpf in HPFs)
            {
                i++;
                if (hpf == null || hpf.ContentLength <= 0)
                {
                    continue;
                }
                //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                //if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
                //{
                //    sWarningMsg += hpf.FileName + "：附件大小超過 15 MB！";
                //}
                //else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
                //{
                //    sWarningMsg += hpf.FileName + "：附件格式不符！";
                //}
                ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == i);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CREATER = d2.USER_ID;
                att.ORDER = i;
                att.CONTENT5 = hpf.ContentLength.ToMyString();
                att.CONTENT7 = "true";
                d2.ATTACHMENT.Add(att);
                SaveAtt(hpf, att.FILE_NAME);
            }
            //3是憑證
            if (hpf3 != null && hpf3.ContentLength > 0)
            {
                ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == 20230109);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf3.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CONTENT1 = model.CONTENT2;
                att.CONTENT2 = model.CONTENT3;
                att.CONTENT3 = model.CONTENT4;
                att.CONTENT4 = d2.CONTENT1;
                att.CONTENT5 = hpf3.ContentLength.ToMyString();
                //att.CONTENT6 = model.CONTENT6;
                att.CONTENT7 = "true";
                att.CREATER = d2.USER_ID;
                att.ORDER = 20230109;
                d2.ATTACHMENT.Add(att);
                SaveAtt(hpf3, att.FILE_NAME);
            }

            #endregion

            iDB.Save();
            Msgbox_Toast("謝謝您的回覆!!");
            //return GoIndex();
            return RedirectToAction("Reply0109", new { id = d2.USER_ID });
        }

        #endregion

        #region 得獎者回覆專區 0117

        public ActionResult Reply0117(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return GoIndex();
            }
            string b1 = "得獎者回覆專區";
            ViewBag.b1 = b1;
            PageTitle = b1;
            DataModel model = new DataModel();
            model.Atts = new List<ATTACHMENT>();

            USER d2 = iDB.GetByIDAsNoTracking<USER>(id);
            if (d2 != null && "20230117".Equals(d2.CONTENT21))
            {
                DateTime deadline = new DateTime(2023, 2, 6, 23, 59, 59);
                if (deadline < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({deadline.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }

                //獎項資訊
                model.CONTENT22 = d2.CONTENT22;
                //得獎人資料
                model.CONTENT1 = d2.CONTENT1;
                model.CONTENT2 = d2.CONTENT2;//姓名
                model.Atts = d2.ATTACHMENT.Where(p => p.ORDER == 3 || p.ORDER == 4).OrderBy(p => p.ORDER).ToList();

                ATTACHMENT reply = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == 20230117);

                if (reply != null)
                {
                    model.DATETIME1 = reply.CREATE_DATE;
                    model.CONTENT2 = reply.CONTENT1;//姓名
                    model.CONTENT3 = reply.CONTENT2;//寄送地址
                    model.CONTENT4 = reply.CONTENT3;//電話
                    model.Atts.Add(reply);
                }
            }
            else
            {
                return GoIndex();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply0117(string id, DataModel model
            , HttpPostedFileBase hpf1, HttpPostedFileBase hpf2
            , HttpPostedFileBase hpf3)
        {
            USER d2 = iDB.GetByID<USER>(id);
            if (d2 != null && "20230117".Equals(d2.CONTENT21))
            {
                DateTime deadline = new DateTime(2023, 2, 6, 23, 59, 59);
                if (deadline < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({deadline.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }
            }
            else
            {
                return GoIndex();
            }

            #region attachment
            List<HttpPostedFileBase> HPFs = new List<HttpPostedFileBase>() { hpf1, hpf2 };
            int i = 2;
            foreach (HttpPostedFileBase hpf in HPFs)
            {
                i++;
                if (hpf == null || hpf.ContentLength <= 0)
                {
                    continue;
                }
                //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                //if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
                //{
                //    sWarningMsg += hpf.FileName + "：附件大小超過 15 MB！";
                //}
                //else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
                //{
                //    sWarningMsg += hpf.FileName + "：附件格式不符！";
                //}
                ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == i);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CREATER = d2.USER_ID;
                att.ORDER = i;
                att.CONTENT5 = hpf.ContentLength.ToMyString();
                att.CONTENT7 = "true";
                d2.ATTACHMENT.Add(att);
                SaveAtt(hpf, att.FILE_NAME);
            }
            //3是憑證
            if (hpf3 != null && hpf3.ContentLength > 0)
            {
                ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == 20230117);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf3.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CONTENT1 = model.CONTENT2;
                att.CONTENT2 = model.CONTENT3;
                att.CONTENT3 = model.CONTENT4;
                att.CONTENT4 = d2.CONTENT1;
                att.CONTENT5 = hpf3.ContentLength.ToMyString();
                //att.CONTENT6 = model.CONTENT6;
                att.CONTENT7 = "true";
                att.CREATER = d2.USER_ID;
                att.ORDER = 20230117;
                d2.ATTACHMENT.Add(att);
                SaveAtt(hpf3, att.FILE_NAME);
            }

            #endregion

            iDB.Save();
            Msgbox_Toast("謝謝您的回覆!!");
            //return GoIndex();
            return RedirectToAction("Reply0117", new { id = d2.USER_ID });
        }

        #endregion

        #region 得獎者回覆專區 0213

        public ActionResult Reply0213(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return GoIndex();
            }
            string b1 = "得獎者回覆專區";
            ViewBag.b1 = b1;
            PageTitle = b1;
            DataModel model = new DataModel();
            model.Atts = new List<ATTACHMENT>();

            USER d2 = iDB.GetByIDAsNoTracking<USER>(id);
            if (d2 != null && "20230213".Equals(d2.CONTENT21))
            {
                DateTime deadline = new DateTime(2023, 2, 20, 23, 59, 59);
                if (deadline < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({deadline.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }

                //獎項資訊
                model.CONTENT22 = d2.CONTENT22;
                //得獎人資料
                model.CONTENT1 = d2.CONTENT1;
                model.CONTENT2 = d2.CONTENT2;//姓名
                model.Atts = d2.ATTACHMENT.Where(p => p.ORDER == 3 || p.ORDER == 4).OrderBy(p => p.ORDER).ToList();

                ATTACHMENT reply = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == 20230213);

                if (reply != null)
                {
                    model.DATETIME1 = reply.CREATE_DATE;
                    model.CONTENT2 = reply.CONTENT1;//姓名
                    model.CONTENT3 = reply.CONTENT2;//寄送地址
                    model.CONTENT4 = reply.CONTENT3;//電話
                    model.Atts.Add(reply);
                }
            }
            else
            {
                return GoIndex();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply0213(string id, DataModel model
            , HttpPostedFileBase hpf1, HttpPostedFileBase hpf2
            , HttpPostedFileBase hpf3)
        {
            USER d2 = iDB.GetByID<USER>(id);
            if (d2 != null && "20230213".Equals(d2.CONTENT21))
            {
                DateTime deadline = new DateTime(2023, 2, 20, 23, 59, 59);
                if (deadline < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({deadline.ToString("yyyy/MM/dd HH:mm:ss")})");
                    return GoIndex();
                }
            }
            else
            {
                return GoIndex();
            }

            #region attachment
            List<HttpPostedFileBase> HPFs = new List<HttpPostedFileBase>() { hpf1, hpf2 };
            int i = 2;
            foreach (HttpPostedFileBase hpf in HPFs)
            {
                i++;
                if (hpf == null || hpf.ContentLength <= 0)
                {
                    continue;
                }
                //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                //if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
                //{
                //    sWarningMsg += hpf.FileName + "：附件大小超過 15 MB！";
                //}
                //else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
                //{
                //    sWarningMsg += hpf.FileName + "：附件格式不符！";
                //}
                ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == i);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CREATER = d2.USER_ID;
                att.ORDER = i;
                att.CONTENT5 = hpf.ContentLength.ToMyString();
                att.CONTENT7 = "true";
                d2.ATTACHMENT.Add(att);
                SaveAtt(hpf, att.FILE_NAME);
            }
            //3是憑證
            if (hpf3 != null && hpf3.ContentLength > 0)
            {
                ATTACHMENT org = d2.ATTACHMENT.FirstOrDefault(p => p.ORDER == 20230213);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf3.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CONTENT1 = model.CONTENT2;
                att.CONTENT2 = model.CONTENT3;
                att.CONTENT3 = model.CONTENT4;
                att.CONTENT4 = d2.CONTENT1;
                att.CONTENT5 = hpf3.ContentLength.ToMyString();
                //att.CONTENT6 = model.CONTENT6;
                att.CONTENT7 = "true";
                att.CREATER = d2.USER_ID;
                att.ORDER = 20230213;
                d2.ATTACHMENT.Add(att);
                SaveAtt(hpf3, att.FILE_NAME);
            }

            #endregion

            iDB.Save();
            Msgbox_Toast("謝謝您的回覆!!");
            //return GoIndex();
            return RedirectToAction("Reply0213", new { id = d2.USER_ID });
        }

        #endregion



        #endregion

        #region 得獎者回覆區
        [NodeSelect("CityInfo")]
        public ActionResult Reply(string id)
        {

            //if (!User.Identity.IsAuthenticated)
            //{
            //    Msgbox_Toast("請先登入", AlertMsgType.Notice);
            //    return GoIndex();
            //}
            PLUS p1 = iDB.GetByIDAsNoTracking<PLUS>(id);
            if (DateTime.Now > p1.DATA1.DATETIME2)
            {
                Msgbox_Toast($"領獎回覆已逾期!!<br/>({p1.DATETIME2.ToDateTimeString()})", AlertMsgType.Notice);
                return GoIndex();
            }
            string b1 = "得獎者回覆專區";
            ViewBag.b1 = b1;
            PageTitle = b1;
            DataModel model = new DataModel();
            model.Atts = new List<ATTACHMENT>();
            if (p1 != null)
            {

                if (p1.DATETIME2 < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({p1.DATETIME2.ToDateTimeString()})");
                    return GoIndex();
                }

                //獎項資訊
                model.CONTENT1 = p1.CONTENT1;
                model.CONTENT2 = p1.CONTENT2;
                //得獎人資料
                model.CONTENT3 = p1.CONTENT3;//Email
                model.CONTENT4 = p1.CONTENT4;//姓名
                model.Atts = p1.ATTACHMENT.Where(p => p.ORDER == 1 || p.ORDER == 2 || p.ORDER == 3 || p.ORDER == 4 || p.ORDER == 5).OrderBy(p => p.ORDER).ToList();
                model.DECIMAL1 = p1.DATA1.DECIMAL1;
                model.CONTENT5 = p1.CONTENT5;
                ViewBag.c4 = p1.CONTENT5 + "_" + p1.CONTENT7;
                model.CONTENT6 = p1.CONTENT6;
                model.CONTENT7 = p1.CONTENT7;
                model.CONTENT8 = p1.CONTENT8;
                model.CONTENT9 = p1.CONTENT9;
                model.CONTENT10 = p1.CONTENT10;//身分證字號
                model.CONTENT11 = p1.DATA1.CONTENT11;
                model.CONTENT12 = p1.DATA1.CONTENT12;
                model.CONTENT13 = p1.DATA1.CONTENT13;

                string[] content = p1.DATA1.PARAGRAPH.FirstOrDefault().CONTENT.Split(Convert.ToChar(10), Convert.ToChar(13));

                string cnt_ = string.Empty;
                List<string> cc = new List<string>();
                for (int i = 0; i <= content.Count() - 1; i++)
                {
                    if (content[i].ToString() != "")
                    {
                        cc.Add(content[i].ToString());
                    }
                }
                ViewBag.List1 = cc;
                ATTACHMENT reply = p1.ATTACHMENT.FirstOrDefault();

                // if (p1.DATETIME3 != null)
                // {
                //      model.DATETIME1 = reply.CREATE_DATE;
                //      model.CONTENT2 = p1.CONTENT2;//Email
                //      model.CONTENT3 = p1.CONTENT3;//姓名
                //      model.CONTENT7 = reply.CONTENT7;//寄送地址
                //      model.CONTENT8 = reply.CONTENT8;//電話
                //      model.Atts.Add(reply);
                // }
            }
            else
            {
                return GoIndex();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(string id, DataModel model
            , HttpPostedFileBase hpf1, HttpPostedFileBase hpf2
            , HttpPostedFileBase hpf3, HttpPostedFileBase hpf4, HttpPostedFileBase hpf5)
        {
            PLUS p1 = iDB.GetByID<PLUS>(id);
            //USER d2 = iDB.GetAllAsNoTracking<USER>().Where(u => u.CONTENT1.Equals(p1.CONTENT2)).FirstOrDefault();
            if (p1 != null)
            {
                if (p1.DATA1.DATETIME2 < DateTime.Now)
                {
                    Msgbox_Toast($"領獎回覆已逾期!!<br/>({p1.DATA1.DATETIME2.ToDateTimeString()})");
                    return GoIndex();
                }
            }
            else
            {
                return GoIndex();
            }

            if (p1.DATETIME3 != null)
            {
                if (p1.DECIMAL2 == null)
                {
                    p1.DECIMAL2 = 1;
                }
                else
                {
                    p1.DECIMAL2 += 1;
                }
                p1.UPDATER = p1.CONTENT2;
                p1.UPDATE_DATE = DateTime.Now;
            }
            else
            {
                p1.DATETIME3 = DateTime.Now;//第一次回覆日期
            }
            #region 基本資料
            string[] c4 = model.CONTENT7.Split('_');
            p1.CONTENT4 = model.CONTENT4;
            p1.CONTENT5 = c4[0];//郵遞區號
            p1.CONTENT6 = model.CONTENT6;
            p1.CONTENT7 = c4[1];
            p1.CONTENT8 = model.CONTENT8;
            p1.CONTENT9 = model.CONTENT9;
            p1.CONTENT10 = model.CONTENT10;
            p1.CONTENT11 = model.CONTENT11;
            p1.CONTENT12 = model.CONTENT12;
            p1.CONTENT13 = model.CONTENT13;
            p1.CONTENT14 = model.CONTENT14;
            p1.CONTENT15 = model.CONTENT15;
            p1.CONTENT20 = "1";

            #endregion
            #region attachment
            List<HttpPostedFileBase> HPFs = new List<HttpPostedFileBase>() { hpf1, hpf2, hpf3, hpf4, hpf5 };
            int i = 0;
            foreach (HttpPostedFileBase hpf in HPFs)
            {
                i++;
                if (hpf == null || hpf.ContentLength <= 0)
                {
                    continue;
                }
                ATTACHMENT org = p1.ATTACHMENT.FirstOrDefault(p => p.ORDER == i);
                if (org != null)
                {
                    iDB.Delete<ATTACHMENT>(org.ID, true);
                }
                ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                att.ATT_TYPE = AttachmentType.File.ToIntValue();
                att.SetUpFileName();
                att.CREATER = p1.CONTENT2;
                att.ORDER = i;
                att.MAIN_ID = p1.ID;
                p1.ATTACHMENT.Add(att);
                SaveAtt(hpf, att.FILE_NAME);

            }
            #endregion
            iDB.Save();
            Msgbox_Toast("謝謝您的回覆!!");
            return GoIndex();
            //return RedirectToAction("Reply", new { id = p1.ID });
        }
        public ActionResult ReplyList(string result,string url)
        {
            if(result ==null && url == null)
            {
                url = Request.Url.ToString().ToLower();
            }
            DATA1 d1url = iDB.GetAllAsNoTracking<DATA1>().Where(d => d.CONTENT7.Equals(url)).FirstOrDefault();
            if (("192.168.0.72".Equals(Request.UserHostAddress)|| "118.163.88.193".Equals(Request.UserHostAddress)) 
                && d1url!=null)
            {
                string b1 = "得獎者回覆區";
                ViewBag.b1 = d1url.ID;
                ViewBag.rs = 0;
                if (result=="True")
                {
                    Msgbox_Toast("寄出成功!");
                }
                else if(result=="False")
                {
                    Msgbox_Toast("寄出失敗");
                }
                
                PageTitle = b1;
                return View(iDB.GetAllAsNoTracking<PLUS>().Where(p => d1url.ID.Equals(p.MAIN_ID)));
            }
           return GoIndex();
        }
        
        public ActionResult Export(string id)
        {
            string _fn = "中獎者名單範例.xlsx";
            string _filename = $"中獎者名單範例_{DateTime.Now.ToDefaultString2()}.xlsx";
            using (FileStream fs = GetReportFileStream(_fn))
            {
                //載入Excel檔案
                using (ExcelPackage package = new ExcelPackage(fs))
                {
                    // 新增worksheet
                    ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

                    //取得資料
                    List<PLUS> data = iDB.GetAllAsNoTracking<PLUS>().Where(p => id.Equals(p.MAIN_ID) && p.CONTENT20=="1").OrderBy(p=>p.CONTENT9).ThenBy(p=>p.ORDER).ToList();
                    //DataTable dt = GetHR303DataTable(_date);

                    // 將DataTable資料塞到sheet中
                    int count = data.Count;

                    //標題
                    if (count > 0)
                    {
                        int startRow = 2;//開始
                        foreach (var item in data)
                        {
                            List<ATTACHMENT> atts = item.ATTACHMENT.Where(p => p.ORDER <= 2).ToList();
                            ATTACHMENT att1 = null, att2 = null;
                            if (atts != null && atts.Count > 0)
                            {
                                att1 = atts.FirstOrDefault(p => p.ORDER == 1);
                                att2 = atts.FirstOrDefault(p => p.ORDER == 2);
                            }

                            ws.Cells[startRow, 1].Value = item.CONTENT9;//獎項編號
                            ws.Cells[startRow, 2].Value = item.CONTENT2;//Email
                            ws.Cells[startRow, 3].Value = item.CONTENT1;//獎項
                            ws.Cells[startRow, 4].Value = item.CONTENT3;//姓名
                            ws.Cells[startRow, 5].Value = item.CONTENT4+item.CONTENT5.ToNodeTitle()+item.CONTENT6.ToNodeTitle()+item.CONTENT7;//地址
                            ws.Cells[startRow, 6].Value = item.CONTENT8;//電話
                            if (att1 != null)
                            {
                                ws.Cells[startRow, 7].Value = att1.FILE_NAME;
                            }
                            if (att2 != null)
                            {
                                ws.Cells[startRow, 8].Value = att2.FILE_NAME;
                            }
                            startRow++;
                            if (item.DATA1.DECIMAL1 == 1)
                            {
                                //原圖 更改名字規則新增至newname
                                foreach (var att in atts)
                                {
                                    string uploadpath = Server.MapPath(Function.GetUploadPath());
                                    string new_path = uploadpath + @"\" + item.DATA1.CONTENT1 + DateTime.Now.ToDefaultString2();
                                    string[] lo = { "身分證0_正", "身分證1_反", item.DATA1.CONTENT11, item.DATA1.CONTENT12, item.DATA1.CONTENT13 };
                                    string newname = item.CONTENT9 + lo[att.ORDER - 1] + att.EXTENSION;
                                    if (!Directory.Exists(new_path))
                                    {
                                        Directory.CreateDirectory(new_path);
                                    }
                                    System.IO.File.Copy(uploadpath + att.FILE_NAME, new_path + @"\" + newname); //複製原檔為新檔名
                                }
                            }
                        }
                        using (ExcelRange rng = ws.Cells[2, 1, startRow - 1, 8])
                        {
                            rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rng.Style.Font.Name = "微軟正黑體";
                        }
                        
                    }
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream, APPLICATION_VND, _filename);
                }
            }
        }

        public ActionResult PicList3()
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                string b1 = "得獎者回覆區";
                ViewBag.b1 = b1;
                PageTitle = b1;
                return View(iDB.GetAllAsNoTracking<USER>().Where(p => "20230109".Equals(p.CONTENT21)));
            }
            return GoIndex();

        }

        [HttpPost]
        public ActionResult PicList3(string id)
        {
            if ("192.168.0.197".Equals(Request.UserHostAddress)
                || "118.163.88.193".Equals(Request.UserHostAddress))
            {
                ATTACHMENT att = iDB.GetByID<ATTACHMENT>(id);
                if (att != null)
                {
                    if (att.CONTENT7.IsTrue())
                    {
                        att.CONTENT7 = "false";
                    }
                    else
                    {
                        att.CONTENT7 = "true";
                    }
                    iDB.Save();
                    return Content("1");
                }
            }
            return Content("0");

        }


        #endregion

        #region ReplyList 未回覆寄信
        public ActionResult SendNotice(string id, string test, string mid, int? page, int? defaultPage, string k, string k2
            )
        {
            string url = string.Empty;
            if (checkIP())
            {
                string result = string.Empty;
                PLUS p_ = iDB.GetByID<PLUS>(id);
                if(p_ != null)
                {
                    if (!p_.CONTENT2.IsNullOrEmpty()) 
                    {
                      result = SendMail(p_).ToString();
                    }
                    url = p_.DATA1.CONTENT7;
                }
                return RedirectToAction("ReplyList", "Home", new { result = result,url=url});


            }
            return GoIndex();

        }
        /// <summary>
		/// 寄信
		/// </summary>
		/// <param name="d1"></param>
		/// <returns></returns>
		public bool SendMail(PLUS p1)
        {
            DATA1 d1 = iDB.GetByID<DATA1>(p1.MAIN_ID);

            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/" + d1.CONTENT6 + ".html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sRecipient = p1.CONTENT2;
            string date = d1.DATETIME2.ToDateTimeString(week: true);
            string sBody = string.Format(_htmlBody, p1.CONTENT2, p1.CONTENT1, date, p1.ID, d1.CONTENT2);
            LetterModel model = new LetterModel();
            model.Body = sBody;
            DateTime _deadline = DateTime.Today.AddDays(1);
            model.Subject = d1.CONTENT3;
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(sRecipient, sRecipient);
            model.SenderName = d1.CONTENT4;
            model.Sender = d1.CONTENT5;
            bool bRet = Function.SendMail(model);
            p1.DATETIME4 = DateTime.Now;
            p1.DECIMAL10 = 1; //成功
            if (!bRet)
            {
                p1.DECIMAL10 = 0; //失敗

                //寄送不成功就留Log
                LogSystem.InitLogSystem();
                LogSystem.WriteLine("<!--TO：" + sRecipient + "-->");
                LogSystem.WriteLine(sBody);
                LogSystem.CloseUnderlayingStream();
            }
            iDB.Save();

            return bRet;
        }
        #endregion
    }
}