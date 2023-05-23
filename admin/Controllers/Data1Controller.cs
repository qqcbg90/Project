using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenAI_API;
using OpenAI_API.Images;
using OpenAI_API.Models;
using OpenAI_API.Completions;
using System.Threading.Tasks;
using System.Net.Http;


namespace admin.Controllers
{
    [NodeSelect(new string[] { "DATA1_DT", "H", "DATA1_fun3000_DT" })]
    [OrderStatusSelect]
    [Decimal2TypeSelect]
    public class Data1Controller : BaseController
    {
        #region const property

        /// <summary>
        /// 檔案上傳大小限制
        /// </summary>
        public const int FILE_UPLOAD_MAX_SIZE = 15 * 1024 * 1024;
        /// <summary>
        /// 圖片上傳大小限制
        /// </summary>
        public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
        /// <summary>
        /// 資料是讀PLUS的 fun4000、fun5002
        /// </summary>
        public readonly string[] PLUS_DATA = new string[] { "fun4000", "fun5002" };


        #endregion

        #region 共用

        IQueryable<DataModel> GetData(string id, string k, string start, string end, int type)
        {

            IQueryable<DATA1> _data1 = iDB.GetAll<DATA1>()
               .Where(p => p.NODE_ID.Equals("fun1") && (string.IsNullOrEmpty(id) || p.ID.Equals(id))
               && (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k))
                   ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
            IQueryable<DataModel> query = _data1
               .Select(p => new DataModel()
               {
                   ID = p.ID,
                   CREATE_DATE = p.CREATE_DATE,
                   CONTENT1 = p.CONTENT1,
                   CONTENT2 = p.CONTENT2,
                   CONTENT3 = p.CONTENT3,
                   CONTENT4 = p.CONTENT4,
                   CONTENT5 = p.CONTENT5,
                   CONTENT6 = p.CONTENT6,
                   CONTENT7 = p.CONTENT7,
                   CONTENT8 = p.CONTENT8,
                   CONTENT9 = p.CONTENT9,
                   CONTENT10 = p.CONTENT10,
                   CONTENT11 = p.CONTENT11,
                   CONTENT12 = p.CONTENT12,
                   CONTENT13 = p.CONTENT13,
                   CONTENT14 = p.CONTENT14,
                   CONTENT15 = p.CONTENT15,
                   CONTENT16 = p.CONTENT16,
                   CONTENT17 = p.CONTENT17,
                   CONTENT18 = p.CONTENT18,
                   CONTENT19 = p.CONTENT19,
                   CONTENT20 = p.CONTENT20,
                   DATETIME1 = p.DATETIME1,
                   DATETIME2 = p.DATETIME2,
                   DATETIME3 = p.DATETIME3,
                   DECIMAL1 = p.DECIMAL1,
                   DECIMAL2 = p.DECIMAL2,
                   CONTENT = p.PARAGRAPH.FirstOrDefault().CONTENT

               });
            return query;
        }

        IQueryable<PLUS> GetPlus(string id, string k, string k2)
        {
            var query = iDB.GetAll<PLUS>().Where(p => p.MAIN_ID.Equals(id)
             && (string.IsNullOrEmpty(k) || p.CONTENT3.Contains(k))
             && (string.IsNullOrEmpty(k2) || p.CONTENT20.Equals(k2))
            );
            query = query.OrderBy(p => p.ORDER).ThenByDescending(p => p.CREATE_DATE);
            return query;
        }

        #endregion

        #region DATA1 

        public ActionResult Index(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string start, string end, string import, string hid_id, HttpPostedFileBase fileImport)
        {
            if (import == "匯入")
            {
                Import(page, defaultPage, hid_id, start, end, fileImport);
            }
            ViewBag.k = k;
            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            IPagedList<DataModel> list = GetData("", k, start, end, 0).ToPagedList(page.ToMvcPaging(), _defaultPage);
            return View($"{NodeID}_Index", list);

        }

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit(string id, int? page, int? defaultPage, string k, string start, string end)
        {
            DataModel model = new DataModel();
            ViewBag.k = k;
            ViewBag.start = start;
            ViewBag.end = end;

            if (id.IsNullOrEmpty())
            {
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                DATA1 data = iDB.GetByID<DATA1>(id);

                if (data != null)
                {
                    model = new DataModel()
                    {
                        ID = data.ID,
                        DATA_TYPE = data.DATA_TYPE,
                        ORDER = data.ORDER,
                        CONTENT1 = data.CONTENT1,
                        CONTENT2 = data.CONTENT2,
                        CONTENT3 = data.CONTENT3,
                        CONTENT4 = data.CONTENT4,
                        CONTENT5 = data.CONTENT5,
                        CONTENT6 = data.CONTENT6,
                        CONTENT7 = data.CONTENT7,
                        CONTENT8 = data.CONTENT8,
                        CONTENT9 = data.CONTENT9,
                        CONTENT10 = data.CONTENT10,
                        CONTENT11 = data.CONTENT11,
                        CONTENT12 = data.CONTENT12,
                        CONTENT13 = data.CONTENT13,
                        CONTENT14 = data.CONTENT14,
                        CONTENT15 = data.CONTENT15,
                        DECIMAL1 = data.DECIMAL1,
                        DECIMAL2 = data.DECIMAL2,
                        DECIMAL3 = data.DECIMAL3,
                        DATETIME1 = data.DATETIME1,
                        DATETIME2 = data.DATETIME2,
                        DATETIME3 = data.DATETIME3,
                        CONTENT = data.PARAGRAPH.FirstOrDefault().CONTENT
                    };
                }
            }
            return View($"{NodeID}_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 15, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit(string id, DataModel model, int? page, int? defaultPage, string k, string startdate, string enddate
            , string starthour, string endhour, string startminute, string endminute, string startsecond, string endsecond)
        {
            ViewBag.k = k;
            CheckAuthority(Authority_Right.Update);
            IsSuccessful = true;
            string sWarningMsg = string.Empty;
            DateTime datetime1 = (startdate + " " + starthour + ":" + startminute + ":" + startsecond).ToDateTimeString().ToDateTime();
            DateTime datetime2 = (enddate + " " + endhour + ":" + endminute + ":" + endsecond).ToDateTimeString().ToDateTime();
            if (ModelState.IsValid)
            {
                IsAdd = id.IsNullOrEmpty();
                DATA1 data = iDB.GetByID<DATA1>(id);
                List<PLUS> plus = iDB.GetAll<PLUS>().Where(p => p.MAIN_ID.Equals(id)).ToList();
                ViewBag.k = k;
                if (data == null)
                {
                    data = new DATA1()
                    {
                        ID = Function.GetGuid(),
                        NODE_ID = "fun1",
                        CREATER = User.Identity.Name,
                        CREATE_DATE = DateTime.Now,
                    };
                    data.PARAGRAPH.Add(new PARAGRAPH()
                    {
                        ID = Function.GetGuid(),
                        CONTENT = model.CONTENT.ToMyString(),
                        CREATE_DATE = DateTime.Now,
                        MAIN_ID = data.ID,
                        CREATER = User.Identity.Name,
                        ORDER = 1
                    });
                }
                else
                {
                    data.UPDATER = User.Identity.Name;
                    data.UPDATE_DATE = DateTime.Now;

                    PARAGRAPH ph = data.GetParagraphByOrder();
                    if (ph != null)
                    {
                        ph.CONTENT = model.CONTENT.ToMyString();
                        ph.UPDATER = User.Identity.Name;
                        ph.UPDATE_DATE = DateTime.Now;
                    }
                    if (plus != null)
                    {
                        foreach (var pl in plus)
                        {
                            pl.DATETIME1 = datetime1;
                            pl.DATETIME2 = datetime2;
                        }
                    }
                }

                data.CONTENT1 = model.CONTENT1;
                data.CONTENT2 = model.CONTENT2;
                data.CONTENT3 = model.CONTENT3;
                data.CONTENT4 = model.CONTENT4;
                data.CONTENT5 = model.CONTENT5;
                data.CONTENT6 = model.CONTENT6;
                data.CONTENT7 = model.CONTENT2 + "replylist";
                data.CONTENT8 = model.CONTENT8;
                data.CONTENT9 = model.CONTENT9;
                data.CONTENT10 = model.CONTENT10;
                data.CONTENT11 = model.CONTENT11;
                data.CONTENT12 = model.CONTENT12;
                data.CONTENT13 = model.CONTENT13;
                data.CONTENT14 = model.CONTENT14;
                data.CONTENT15 = model.CONTENT15;
                data.CONTENT16 = model.CONTENT16;
                data.CONTENT17 = model.CONTENT17;
                data.CONTENT18 = model.CONTENT18;
                data.CONTENT19 = model.CONTENT19;
                data.CONTENT20 = model.CONTENT20;
                data.DATETIME1 = model.DATETIME1;
                data.DATETIME2 = model.DATETIME2;
                data.DECIMAL1 = model.DECIMAL1;
                data.DECIMAL2 = model.DECIMAL2;
                data.DECIMAL3 = model.DECIMAL3;
                data.DATETIME1 = datetime1;
                data.DATETIME2 = datetime2;

                if (IsAdd)
                {
                    IsSuccessful = iDB.Add<DATA1>(data);
                }
                else
                {
                    iDB.Save();
                }
                if (IsSuccessful)
                {
                    AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
                    return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
                }
            }

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit", model);
        }

        [ActionLog(TableNameIndex = 15, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = true)
        {
            ViewBag.k = k;
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<DATA1>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
        }

        private bool checkIP()
        {
            return "192.168.0.72".Equals(Request.UserHostAddress) || "192.168.0.64".Equals(Request.UserHostAddress) ||
                "118.163.88.193".Equals(Request.UserHostAddress);
        }



        #endregion

        #region 寄信
        public ActionResult SendNotice(string id, string test, string mid, int? page, int? defaultPage, string k, string k2, string k3
           )
        {
            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            int _page = page.ToMvcPaging();
            if (IsPost())
            {
                _page = 0;
            }
            if (checkIP())
            {
                int _count = 0;
                List<string> result = new List<string>();
                List<PLUS> Pluslist = iDB.GetAll<PLUS>(MAIN_ID: id)
                    .Where(p => (string.IsNullOrEmpty(mid) || p.ID.Equals(mid)))
                    .OrderBy(p => p.ORDER).ToList();
                if (k3 != "personal")
                {
                    Pluslist = Pluslist.Where(p => p.DATETIME4 == null && p.DECIMAL10 == null).ToList();
                }
                if (Pluslist != null)
                {
                    foreach (var item in Pluslist)
                    {
                        if (!item.CONTENT3.IsNullOrEmpty() && (id.IsNullOrEmpty() || item.MAIN_ID.Equals(id)))
                        {
                            if (SendMail(item, id, test))
                            {
                                _count++;
                                if (!test.IsNullOrEmpty() || !mid.IsNullOrEmpty())//測試信
                                {
                                    if (_count == 1)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                result.Add(item.CONTENT1);
                            }

                        }
                    }
                }
                if (!mid.IsNullOrEmpty() && result.Count == 0)
                {
                    AlertMsg = "成功寄出！";
                }
                else if (!mid.IsNullOrEmpty())
                {
                    AlertMsg = "失敗！";
                }
                else
                {
                    AlertMsg = $"不成功：{string.Join(",", result)}，成功 {_count} 封";
                }
                if (k3 == "personal")
                {
                    return GoIndex(NodeID, page, defaultPage, k, CreateDict(id: id, k: k, k2: k2), actionName: "Act2_Index");
                }
                return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "start", "end" }));
            }
            return GoIndex();
        }

        public bool SendMail(PLUS p1, string id, string test)
        {
            DATA1 d1 = iDB.GetByID<DATA1>(id);

            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/" + d1.CONTENT6 + ".html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sRecipient = p1.CONTENT3;
            if (!test.IsNullOrEmpty())
            {
                sRecipient = "sinyi@kingspread.com.tw";//測試信
            }
            string date = d1.DATETIME2.ToDateTimeString(week: true);
            //string sBody = string.Format(_htmlBody, EMAIL,      獎項,   回覆期限, P_ID  ,活動網址);
            string sBody = string.Format(_htmlBody, p1.CONTENT3, p1.CONTENT1, date, p1.ID, d1.CONTENT2);
            LetterModel model = new LetterModel();
            model.Body = sBody;
            DateTime _deadline = DateTime.Today.AddDays(1);
            //model.Subject = $"【交通部觀光局】自行車友善旅宿優惠方案確認信，煩請於{_deadline.Month} / {_deadline.Day}前確認完畢，謝謝！";
            model.Subject = d1.CONTENT3;
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(sRecipient, sRecipient);//收件者email
            model.SenderName = d1.CONTENT4;
            model.Sender = d1.CONTENT5;
            bool bRet = Function.SendMail(model);
            if (test.IsNullOrEmpty())
            {
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
            }


            return bRet;
        }
        #endregion

        #region 得獎者回覆狀態
        public ActionResult Act2_Index(string id, int? page, int? defaultPage, string k, string k2)
        {
            ViewBag.k = k;
            ViewBag.k2 = k2;
            ViewBag.d1 = id;
            DATA1 d1 = iDB.GetByID<DATA1>(id);
            ViewBag.ContentTitle2 = "_" + d1.CONTENT1;
            ViewBag.decimal1 = d1.DECIMAL1;
            ViewBag.att1 = d1.CONTENT11;
            ViewBag.att2 = d1.CONTENT12;
            ViewBag.att3 = d1.CONTENT13;
            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            int _page = page.ToMvcPaging();
            if (IsPost())
            {
                _page = 0;
            }
            return View(GetPlus(id, k, k2).ToPagedList(_page, _defaultPage));
        }
        #endregion

        #region 匯入中獎者

        public ActionResult Import(int? page, int? defaultPage,
            string id, string start, string end, HttpPostedFileBase fileImport)
        {
            if (IsPost())
            {
                string _value = string.Empty;
                if (fileImport != null)//新增
                {
                    _value = Importwinner(fileImport, id);

                    if (_value == "")
                    {
                        Msgbox_Toast("匯入完成!!");
                    }
                    else
                    {
                        Msgbox_Toast("匯入失敗！！" + _value);
                    }

                }
                ViewBag.msg = _value;
            }
            return View();
        }

        #region 處理匯入

        /// <summary>
        /// 處理匯入
        /// </summary>
        /// <param name="fileImport"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        private string Importwinner(HttpPostedFileBase fileImport, string id)
        {

            DATA1 d1 = iDB.GetByID<DATA1>(id);

            string msgs = "";
            try
            {
                if (fileImport != null && fileImport.ContentLength > 0 && d1 != null)
                {
                    string _creater = User.Identity.Name;
                    //載入Excel檔案
                    using (ExcelPackage package = new ExcelPackage(fileImport.InputStream))
                    {
                        // 取得worksheet
                        for (int page = 1; page <= package.Workbook.Worksheets.Count; page++)
                        {
                            ExcelWorksheet ws = package.Workbook.Worksheets[page];
                            for (int i = 2; i <= ws.Dimension.End.Row; i++)
                            {
                                if (ws.Cells[i, 1].Value.ToMyString().IsNullOrEmpty())
                                {
                                    break;
                                }
                                PLUS data = new PLUS()
                                {
                                    ID = Function.GetGuid(),
                                    CREATER = User.Identity.Name,
                                    CREATE_DATE = DateTime.Now,
                                    MAIN_ID = d1.ID,
                                    ENABLE = 1,
                                    ORDER = (i - 1),
                                    CONTENT1 = ws.Cells[i, 3].Value.ToMyString(),//獎項內容
                                    CONTENT2 = ws.Cells[i, 1].Value.ToMyString(),//編號
                                    CONTENT3 = ws.Cells[i, 2].Value.ToMyString(),//中獎者email
                                    CONTENT4 = ws.Cells[i, 4].Value.ToMyString(),//中獎者姓名
                                    DATETIME1 = d1.DATETIME1,
                                    DATETIME2 = d1.DATETIME2,
                                    CONTENT20 = "0"
                                };
                                iDB.Add<PLUS>(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgs = ex.Message.ToString();
            }
            return msgs;

        }
        #endregion

        #endregion

        #region 匯出中獎者
        public ActionResult Export(string id)
        {
            string _fn = "中獎者名單範例.xlsx";
            string _filename = $"中獎者名單_{DateTime.Now.ToDefaultString2()}.xlsx";
            using (FileStream fs = GetReportFileStream(_fn))
            {
                //載入Excel檔案
                using (ExcelPackage package = new ExcelPackage(fs))
                {
                    // 新增worksheet
                    ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

                    //取得資料
                    List<PLUS> data = iDB.GetAllAsNoTracking<PLUS>().Where(p => id.Equals(p.MAIN_ID) && p.CONTENT20 == "1").OrderBy(p => p.CONTENT2).ThenBy(p => p.ORDER).ToList();
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

                            ws.Cells[startRow, 1].Value = item.CONTENT2;//獎項編號
                            ws.Cells[startRow, 2].Value = item.CONTENT3;//Email
                            ws.Cells[startRow, 3].Value = item.CONTENT1;//獎項
                            ws.Cells[startRow, 4].Value = item.CONTENT4;//姓名
                            ws.Cells[startRow, 5].Value = item.CONTENT5 + item.CONTENT6.ToNodeTitle() + item.CONTENT7.ToNodeTitle() + item.CONTENT8;//地址
                            ws.Cells[startRow, 6].Value = item.CONTENT9;//電話
                            ws.Cells[startRow, 7].Value = item.CONTENT10;//身分證
                            if (att1 != null)
                            {
                                ws.Cells[startRow, 8].Value = att1.FILE_NAME;
                            }
                            if (att2 != null)
                            {
                                ws.Cells[startRow, 9].Value = att2.FILE_NAME;
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
                                    string newname = item.CONTENT2 + lo[att.ORDER - 1] + att.EXTENSION;
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
        #endregion

        #region ChatGpt

        public ActionResult ChatGPT_Index()
        {
            ViewBag.ContentTitle = "ChatGPT";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChatGPT_Index(string prompt, string action)
        {
            ViewBag.ContentTitle = "ChatGPT";
            ViewBag.prompt = prompt;
            string response = string.Empty;
            if (prompt != null)
            {
                OpenAIAPI _openai = new OpenAIAPI("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");
                if (action == "chat")
                {
                    CompletionRequest completionRequest = new CompletionRequest
                    {
                        Prompt = prompt,
                        Model = Model.CurieText,
                        MaxTokens = 300,
                        Temperature = 0.7,
                    };
                    var completions = await _openai.Completions.CreateCompletionAsync(completionRequest);
                    response = completions.Completions[0].Text.Trim();
                    ViewBag.Response = response;
                }
                else
                {
                    var request = new ImageGenerationRequest(prompt, 1, ImageSize._256, null, ImageResponseFormat.Url);
                    var response_ = await _openai.ImageGenerations.CreateImageAsync(request);
                    var imageUrl = response_.Data[0].Url;
                    ViewBag.imageurl = imageUrl;
                }
               
            }
            return View("ChatGPT_Index");
        }
        #endregion
    }
}