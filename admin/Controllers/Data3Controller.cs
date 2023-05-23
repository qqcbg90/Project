using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace admin.Controllers
{
    /// <summary>
    /// 兌換點品項管理
    /// </summary>
    public class Data3Controller : BaseController
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
        /// 資料是讀SYSUSER的 fun5000
        /// </summary>
        public readonly string[] SYSUSER_DATA = new string[] { "fun5000" };

        #endregion

        #region 共用
        /// <summary>
        /// 建立商家名稱下拉 ViewBag.Data3Fun5000Select
        /// </summary>
        void CreateData3Fun5000()
        {
            List<SelectListItem> items =
            Function.SysUserList.Where(p => p.ENABLE == 1 && "1".Equals(p.CONTENT1)).OrderBy(p => p.CREATE_DATE)
                .Select(x => new SelectListItem() { Text = x.NAME, Value = x.USER_ID }).ToList();
            ViewBag.Data3Fun5000Select = new SelectList(items, "Value", "Text");
        }

        /// <summary>
        /// 建立QRCode by ID
        /// </summary>
        string CreateQrCode(string id)
        {
            string _value = $"{SetUploadPath()}/QRCode/{id}.png";
            Function.CreateQRcode($"{Function.DEFAULT_ROOT_HTTP}ExchangeList/{id}"
                        , _value);
            return _value;
        }
        /// <summary>
        /// 取得兌換點紀錄
        /// </summary>
        /// <param name=""></param>
        /// <param name="k"></param>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="k3"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IQueryable<PLUS> GetPlusData(string k, string k1, string k2,
            string k3, string start, string end)
        {
            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();

            string _business = User.Identity.Name;
            //先找商家下&禮品名稱關鍵字
            List<string> data3List = iDB.GetAllAsNoTracking<DATA3>(MAIN_ID: "fun5000")
                .Where(p => _business.Equals(p.DATA_TYPE)
                && (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k)))
                .Select(p => p.ID).ToList();

            return iDB.GetAllAsNoTracking<PLUS>()
                    .Where(p => (string.IsNullOrEmpty(k1) || p.CONTENT2.Contains(k1) || p.CONTENT3.Contains(k1))
                    && "fun5003".Equals(p.PLUS_TYPE)
                    && data3List.Contains(p.MAIN_ID)
                    && ((p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd) || (p.DATETIME2 >= tmpStart && p.DATETIME2 < tmpEnd))
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
        }

        /// <summary>
        /// 取得紅利點數發送紀錄
        /// </summary>
        /// <param name=""></param>
        /// <param name="k"></param>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="k3"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IQueryable<PLUS> GetPlusData2(string k, string k1, string k2,
            string k3, string start, string end)
        {
            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();

            return iDB.GetAllAsNoTracking<PLUS>()
                    .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k) || p.CONTENT2.Contains(k) || p.CONTENT3.Contains(k))
                    && "fun5002".Equals(p.PLUS_TYPE)
                    && (p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd)
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
        }

        /// <summary>
        /// 取得兌換點紀錄 給管理者用
        /// </summary>
        /// <param name=""></param>
        /// <param name="k"></param>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="k3"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IQueryable<PLUS> GetPlusData3(string k, string k1, string k2,
            string k3, string start, string end)
        {
            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();

            //先找商家下&禮品名稱關鍵字
            List<string> data3List = iDB.GetAllAsNoTracking<DATA3>(MAIN_ID: "fun5000")
                .Where(p => (string.IsNullOrEmpty(k2) || k2.Equals(p.DATA_TYPE))
                && (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k)))
                .Select(p => p.ID).ToList();

            return iDB.GetAllAsNoTracking<PLUS>()
                    .Where(p => (string.IsNullOrEmpty(k1) || p.CONTENT2.Contains(k1) || p.CONTENT3.Contains(k1))
                    && "fun5003".Equals(p.PLUS_TYPE)
                    && data3List.Contains(p.MAIN_ID)
                    && (p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd)
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
        }

        /// <summary>
        /// GetData USER by keyword
        /// </summary>
        /// <param name="c1">館別</param>
        /// <param name="c2">單位</param>
        /// <param name="k">關鍵字(帳號、姓名、Email)</param>
        /// <returns></returns>
        IQueryable<USER> GetData(string k, string c1, string c2, string start, string end)
        {
            ViewBag.start = start;
            ViewBag.end = end;

            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();
            return iDB.GetAllAsNoTracking<USER>()
                .Where(p => (string.IsNullOrEmpty(k) || p.USER_ID.Contains(k) || p.CONTENT1.Contains(k)
                || p.CONTENT2.Contains(k) || p.CONTENT3.Contains(k))
                && (p.CREATE_DATE >= tmpStart && p.CREATE_DATE < tmpEnd)
                //&&(string.IsNullOrEmpty(c1) || p.CONTENT6.Equals(c1)) &&
                //(string.IsNullOrEmpty(c2) || p.CONTENT2.Equals(c2))
                ).OrderByDescending(p => p.CREATE_DATE);
        }

        #endregion

        #region DATA3
        /// <summary>
        /// 列表 SYSUSER
        /// </summary>
        public ActionResult Index(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            if (SYSUSER_DATA.Contains(NodeID))
            {
                return View($"{NodeID}_Index", iDB.GetAllAsNoTracking<SYSUSER>()
                    .Where(p => "1".Equals(p.CONTENT1)
                    && (string.IsNullOrEmpty(k) || p.NAME.Contains(k) || p.EMAIL.Contains(k) || p.CONTENT4.Contains(k))
                    ).OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
            }
            else if ("fun5002".Equals(NodeID))//紅利點數管理
            {
                return View($"{NodeID}_Index", GetPlusData2(k, k1, k2, k3, start, end)
                    .ToPagedList(page.ToMvcPaging(), _defaultPage));
            }
            else if ("fun5003".Equals(NodeID))//商家用的會員兌換商品管理
            {
                return View($"{NodeID}_Index", GetData(k, k1, k2, start, end)
                    .ToPagedList(page.ToMvcPaging(), _defaultPage));
            }
            else//商家用的品項管理
            {
                string _business = User.Identity.Name;
                return View($"{NodeID}_Index", iDB.GetAll<DATA3>(MAIN_ID: "fun5000")
                    .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k))
                    && _business.Equals(p.DATA_TYPE)
                    && ((p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd) || (p.DATETIME2 >= tmpStart && p.DATETIME2 < tmpEnd))
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
            }
        }

        /// <summary>
        /// 列表 DATA3 兌換點品項管理
        /// </summary>
        public ActionResult Index2(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            return View($"{NodeID}_Index2", iDB.GetAll<DATA3>(MAIN_ID: NodeID)
                    .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k) || p.CONTENT2.Contains(k))
                    && (p.DATA_TYPE.Equals(k1))
                    && (p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd)
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }

        /// <summary>
        /// QRCode Print
        /// </summary>
        public ActionResult QRcodePrint(int? page, int? defaultPage, string k, string id, string k1, string k2,
            string k3, string start, string end)
        {
            DATA3 data = iDB.GetByIDAsNoTracking<DATA3>(id);
            return View(data);
        }

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
        {
            DataModel model = new DataModel();
            if (id.IsNullOrEmpty())
            {
                CheckAuthority(Authority_Right.Add);
                model.DATETIME1 = DateTime.Today;
                model.Atts = new List<ATTACHMENT>();
                model.Pics = new List<ATTACHMENT>();
                model.plusList = new List<PLUS>();
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                DATA3 data = iDB.GetByIDAsNoTracking<DATA3>(id);
                if (data != null)
                {
                    model = new DataModel()
                    {
                        ID = data.ID,
                        DATA_TYPE = data.DATA_TYPE,
                        Bool_STATUS = "1".Equals(data.STATUS),
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
                        Bool_DECIMAL1 = data.DECIMAL1 == 1,
                        Bool_DECIMAL2 = data.DECIMAL2 == 1,
                        DECIMAL3 = data.DECIMAL3.ToInt(),
                        DECIMAL4 = data.DECIMAL4.ToInt(),
                        DECIMAL5 = data.DECIMAL5.ToInt(),
                        DATETIME1 = data.DATETIME1,
                        DATETIME2 = data.DATETIME2,
                        DATETIME3 = data.DATETIME3,
                        Atts = data.GetAttachments(AttachmentType.File.ToIntValue()),
                        Pics = data.GetAttachments(),
                        plusList = data.PLUS.Where(p => p.ENABLE == 1 && "fun5003".Equals(p.PLUS_TYPE) && "1".Equals(p.STATUS)).ToList(),
                        PH0 = data.GetParagraphByOrder(0).CONTENT,
                        PH1 = data.GetParagraphByOrder().CONTENT
                    };
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.start = start;
            ViewBag.end = end;

            return View($"{NodeID}_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 17, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit(string id, DataModel model, int? page, int? defaultPage, string k, string k1, string k2, string start, string end
            ,List<HttpPostedFileBase> hpfAtt
            , List<HttpPostedFileBase> hpfPic
            )
        {
            CheckAuthority(Authority_Right.Update);
            IsSuccessful = true;
            string sWarningMsg = string.Empty;

            if (ModelState.IsValid)
            {
                IsAdd = id.IsNullOrEmpty();
                DATA3 data = iDB.GetByID<DATA3>(id);
                if (data == null)
                {
                    data = new DATA3()
                    {
                        ID = Function.GetGuid(),
                        NODE_ID = NodeID,
                        CREATER = User.Identity.Name,
                        DATA_TYPE = k1
                    };
                    data.PARAGRAPH.Add(new PARAGRAPH()
                    {
                        ID = Function.GetGuid(),
                        CONTENT = model.PH0.ToMyString(),
                        CREATE_DATE = DateTime.Now,
                        CREATER = User.Identity.Name,
                        ORDER = 0
                    });
                    data.PARAGRAPH.Add(new PARAGRAPH()
                    {
                        ID = Function.GetGuid(),
                        CONTENT = model.PH1.ToMyString(),
                        CREATE_DATE = DateTime.Now,
                        CREATER = User.Identity.Name,
                        ORDER = 1
                    });
                }
                else
                {
                    data.UPDATER = User.Identity.Name;
                    data.UPDATE_DATE = DateTime.Now;

                    PARAGRAPH par = data.GetParagraphByOrder(0);
                    if (par != null)
                    {
                        par.CONTENT = model.PH0.ToMyString();
                        par.UPDATER = User.Identity.Name;
                        par.UPDATE_DATE = DateTime.Now;
                    }
                    PARAGRAPH par1 = data.GetParagraphByOrder();
                    if (par1 != null)
                    {
                        par1.CONTENT = model.PH1.ToMyString();
                        par1.UPDATER = User.Identity.Name;
                        par1.UPDATE_DATE = DateTime.Now;
                    }
                }
                data.STATUS = model.Bool_STATUS ? "1" : "0";
                data.CONTENT1 = model.CONTENT1;
                data.CONTENT2 = model.CONTENT2;
                data.CONTENT3 = model.CONTENT3;
                data.CONTENT4 = model.CONTENT4;
                data.CONTENT5 = model.CONTENT5;
                data.CONTENT6 = model.CONTENT6;
                data.CONTENT7 = model.CONTENT7;
                data.DATETIME1 = model.DATETIME1;
                data.DATETIME2 = model.DATETIME2;
                data.DATETIME3 = model.DATETIME3;
                data.DECIMAL1 = model.Bool_DECIMAL1 ? 1 : 0;
                data.DECIMAL2 = model.Bool_DECIMAL2 ? 1 : 0;
                data.DECIMAL3 = model.DECIMAL3;
                data.DECIMAL4 = model.DECIMAL4;
                data.DECIMAL5 = model.DECIMAL5;

                #region attachment
                if (hpfAtt != null && hpfAtt.Count > 0)
                {
                    foreach (HttpPostedFileBase hpf in hpfAtt)
                    {
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
                            ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                            att.ATT_TYPE = AttachmentType.File.ToIntValue();
                            att.SetUpFileName();
                            att.CREATER = User.Identity.Name;
                            att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: data.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
                            data.ATTACHMENT.Add(att);
                            SaveAtt(hpf, att.FILE_NAME);
                        }
                    }
                }

                if (hpfPic != null && hpfPic.Count > 0)
                {
                    foreach (HttpPostedFileBase hpf in hpfPic)
                    {
                        if (hpf == null || hpf.ContentLength <= 0)
                        {
                            continue;
                        }

                        //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                        //if (hpf.ContentLength > IMG_UPLOAD_MAX_SIZE)
                        //{
                        //    sWarningMsg += hpf.FileName + "：圖片大小超過 2 MB！";
                        //}
                        //else if (Function.DEFAULT_FILEUPLOAD_PICTURE_EXT.IndexOf(sExt) == -1)
                        //{
                        //    sWarningMsg += hpf.FileName + "：圖片格式不符！";
                        //}
                        if (sWarningMsg.IsNullOrEmpty())
                        {
                            ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                            att.ATT_TYPE = AttachmentType.Image.ToIntValue();
                            att.SetUpFileName();
                            att.CREATER = User.Identity.Name;
                            att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: data.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
                            data.ATTACHMENT.Add(att);
                            SavePicture(new WebImage(hpf.InputStream), att);
                        }
                    }
                }

                #endregion

                if (IsAdd)
                {
                    IsSuccessful = iDB.Add<DATA3>(data);
                    CreateQrCode(data.ID);
                }
                else
                {
                    iDB.Save();
                }
                if (IsSuccessful)
                {
                    AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
                    return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }), "Index2");
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.start = start;
            ViewBag.end = end;

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit", model);
        }

        [ActionLog(TableNameIndex = 17, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = false)
        {
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<DATA3>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }), "Index2");
        }

        public ActionResult ResetQRCode(string id)
        {
            string _value = string.Empty;
            _value = CreateQrCode(id);
            return Content(_value);
        }

        #region QRCode 兌換

        public ActionResult Exchange(string id)
        {
            DATA3 data = iDB.GetByIDAsNoTracking<DATA3>(id);
            if (data != null &&
                (DateTime.Today >= data.DATETIME1 && (!data.DATETIME2.HasValue || data.DATETIME2 >= DateTime.Today)))
            {
                return View(data);
            }
            else
            {
                return Content("資料不存在!!");
            }
        }

        #endregion

        #region 訂單管理編輯 no use

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit2(string id, int? page, int? defaultPage, string k, string k1, string k2
            , string k3, string start, string end)
        {
            DataModel model = new DataModel();
            if (id.IsNullOrEmpty())//訂單不行新增；先放著
            {
                CheckAuthority(Authority_Right.Add);
                model.DATETIME1 = DateTime.Today;
                model.Atts = new List<ATTACHMENT>();
                model.Pics = new List<ATTACHMENT>();
                model.plusList = new List<PLUS>();
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                PLUS data = iDB.GetByIDAsNoTracking<PLUS>(id);
                if (data != null)
                {
                    model = new DataModel()
                    {
                        ID = data.ID,
                        DATA_TYPE = data.PLUS_TYPE,
                        STATUS = data.STATUS,
                        //Bool_STATUS = "1".Equals(data.STATUS),
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
                        Bool_DECIMAL1 = data.DECIMAL1 == 1,
                        Bool_DECIMAL2 = data.DECIMAL2 == 1,
                        DECIMAL3 = data.DECIMAL3.ToInt(),
                        DECIMAL4 = data.DECIMAL4.ToInt(),
                        DECIMAL5 = data.DECIMAL5.ToInt(),
                        DECIMAL6 = data.DECIMAL6.ToInt(),
                        DECIMAL7 = data.DECIMAL7.ToInt(),
                        DATETIME1 = data.DATETIME1,
                        DATETIME2 = data.DATETIME2,
                        DATETIME3 = data.DATETIME3,
                        DATETIME4 = data.DATETIME4,
                        DATETIME5 = data.DATETIME5,
                        Data1 = data.DATA1
                        //PH0 = data.GetParagraphByOrder(0).CONTENT,
                        //PH1 = data.GetParagraphByOrder().CONTENT
                    };
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            return View($"{NodeID}_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit2(string id, DataModel model, int? page, int? defaultPage, string k, string k1
            , string k2, string k3, string start, string end
            )
        {
            CheckAuthority(Authority_Right.Update);
            IsSuccessful = true;
            string sWarningMsg = string.Empty;

            if (ModelState.IsValid)
            {
                IsAdd = id.IsNullOrEmpty();
                PLUS data = iDB.GetByID<PLUS>(id);
                if (data == null)
                {
                    data = new PLUS()
                    {
                        ID = Function.GetGuid(),
                        PLUS_TYPE = NodeID,
                        CREATER = User.Identity.Name
                    };
                    //data.PARAGRAPH.Add(new PARAGRAPH()
                    //{
                    //    ID = Function.GetGuid(),
                    //    CONTENT = model.PH0.ToMyString(),
                    //    CREATE_DATE = DateTime.Now,
                    //    CREATER = User.Identity.Name,
                    //    ORDER = 0
                    //});
                    //data.PARAGRAPH.Add(new PARAGRAPH()
                    //{
                    //    ID = Function.GetGuid(),
                    //    CONTENT = model.PH1.ToMyString(),
                    //    CREATE_DATE = DateTime.Now,
                    //    CREATER = User.Identity.Name,
                    //    ORDER = 1
                    //});
                }
                else
                {
                    data.UPDATER = User.Identity.Name;
                    data.UPDATE_DATE = DateTime.Now;

                    //PARAGRAPH par = data.GetParagraphByOrder(0);
                    //if (par != null)
                    //{
                    //    par.CONTENT = model.PH0.ToMyString();
                    //    par.UPDATER = User.Identity.Name;
                    //    par.UPDATE_DATE = DateTime.Now;
                    //}
                    //PARAGRAPH par1 = data.GetParagraphByOrder();
                    //if (par1 != null)
                    //{
                    //    par1.CONTENT = model.PH1.ToMyString();
                    //    par1.UPDATER = User.Identity.Name;
                    //    par1.UPDATE_DATE = DateTime.Now;
                    //}
                }
                data.STATUS = model.STATUS;//訂單狀態
                //data.CONTENT1 = model.CONTENT1;
                //data.CONTENT2 = model.CONTENT2;
                //data.CONTENT3 = model.CONTENT3;
                //data.CONTENT4 = model.CONTENT4;
                data.CONTENT5 = model.CONTENT5;//備註
                //data.CONTENT6 = model.CONTENT6;
                //data.CONTENT7 = model.CONTENT7;
                //data.DATETIME1 = model.DATETIME1;
                data.DATETIME3 = model.DATETIME3;//請款日期

                if (IsAdd)
                {
                    IsSuccessful = iDB.Add<PLUS>(data);
                }
                else
                {
                    iDB.Save();
                }
                if (IsSuccessful)
                {
                    AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
                    return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "start", "end" }));
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit", model);
        }


        #endregion

        #endregion

        #region PLUS 商家兌換紀錄操作

        public ActionResult Index3(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            return View($"{NodeID}_Index3", GetPlusData(k, k1, k2, k3, start, end).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }

        [ActionLog(TableNameIndex = 10, Description = "變更PLUS狀態")]
        public ActionResult ChangeStatus(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string st, string id, string start, string end)
        {
            string _business = User.Identity.Name;
            PLUS data = iDB.GetByID<PLUS>(id);
            if (data != null && _business.Equals(data.DATA3.DATA_TYPE))
            {
                data.STATUS = st;
                data.UPDATER = _business;
                data.UPDATE_DATE = DateTime.Now;
                //根據請款、退回處理點數 待寫
                if (AuditStatus.Type1.ToIntValue().Equals(st))
                {
                    data.DATETIME2 = DateTime.Now;
                }
                else if (AuditStatus.Type2.ToIntValue().Equals(st))
                {
                    data.DATETIME3 = DateTime.Now;
                }
                //end
                iDB.Save();
                AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
            }
            else
            {
                AlertMsg = "更新失敗!!";
            }

            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }), "Index3");
        }

        #endregion

        #region 匯出 會員兌換紀錄、請款檔??

        public ActionResult ExportIndex3(int? page, int? defaultPage,
            string k, string k1, string k2, string k3, string start, string end)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 樣式－欄位名稱

            //字型
            HSSFFont fontHeader = workbook.CreateFont() as HSSFFont;
            fontHeader.FontHeightInPoints = 12;
            fontHeader.Boldweight = (short)FontBoldWeight.BOLD; //粗體
            fontHeader.FontName = "新細明體";

            //樣式
            HSSFCellStyle styleHeader = workbook.CreateCellStyle() as HSSFCellStyle;
            styleHeader.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleHeader.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleHeader.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.WrapText = true;
            styleHeader.SetFont(fontHeader);
            styleHeader.FillForegroundColor = HSSFColor.GREY_25_PERCENT.index;
            styleHeader.FillPattern = FillPatternType.SOLID_FOREGROUND;

            #endregion 樣式－欄位名稱

            #region 樣式－欄位內容

            //字型
            HSSFFont fontContent = workbook.CreateFont() as HSSFFont;
            fontContent.FontHeightInPoints = 12;
            fontContent.Boldweight = (short)FontBoldWeight.BOLD; //粗體
            fontContent.FontName = "新細明體";

            HSSFFont fontContent2 = workbook.CreateFont() as HSSFFont;
            fontContent2.FontHeightInPoints = 12;
            fontContent2.FontName = "新細明體";

            //樣式-粗體字+置中
            HSSFCellStyle styleContent = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleContent.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.WrapText = true;
            styleContent.SetFont(fontContent);

            //樣式-粗體字+置左
            HSSFCellStyle styleContent2 = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
            styleContent2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.WrapText = true;
            styleContent2.SetFont(fontContent);

            //樣式-正常字+置中
            HSSFCellStyle styleContent3 = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent3.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleContent3.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent3.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.WrapText = true;
            styleContent3.SetFont(fontContent2);

            //樣式-正常字+置左
            HSSFCellStyle styleContent4 = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent4.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
            styleContent4.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent4.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.WrapText = true;
            styleContent4.SetFont(fontContent2);

            //超連結
            HSSFFont fontLink = workbook.CreateFont() as HSSFFont;
            fontLink.FontHeightInPoints = 12;
            fontLink.FontName = "新細明體";
            fontLink.Underline = (byte)FontUnderlineType.SINGLE;
            fontLink.Color = HSSFColor.BLUE.index;

            HSSFCellStyle styleLink = workbook.CreateCellStyle() as HSSFCellStyle;
            styleLink.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleLink.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleLink.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.SetFont(fontLink);

            #endregion 樣式－欄位內容

            IQueryable<PLUS> list = GetPlusData(k, k1, k2, k3, start, end);
            string _reportName = $"{Function.GetSysUserName(User.Identity.Name)}_兌換品項紀錄";
            HSSFSheet sheet = workbook.CreateSheet(_reportName) as HSSFSheet;
            using (MemoryStream ms = new MemoryStream())
            {
                int iRowIndex = 1, iCellIndex = 0;
                foreach (PLUS m in list)
                {
                    iCellIndex = 0;
                    HSSFRow rowM = sheet.CreateRow(iRowIndex) as HSSFRow;

                    #region 基本資料

                    rowM.CreateCell(iCellIndex).SetCellValue(m.DATETIME1.Value.ToString("yyyy/MM/dd HH:mm:ss")); //建立日期
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT1); //兌換代碼
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.GetSTATUS()); //狀態
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DATA3.CONTENT1); //禮品名稱
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT2); //會員E-mail
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT3); //會員名稱
                    iCellIndex++;
                    
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DECIMAL4.ToInt()); //金額
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DECIMAL5.ToInt()); //點數
                    iCellIndex++;
                    #endregion

                    iRowIndex++;
                }
                List<string> lsHeader = new List<string>();
                lsHeader = new List<string>
                    {
                        "兌換日期", "兌換代碼", "兌換狀態","禮品名稱",
                    "會員E-mail","會員名稱","金額","點數"
                };

                #region 套用格式
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = sheet.GetRow(i) as HSSFRow;
                    if (row == null)
                        row = sheet.CreateRow(i) as HSSFRow;
                    for (int j = 0; j < lsHeader.Count; j++)
                    {
                        HSSFCell cell = row.GetCell(j) as HSSFCell;
                        if (cell == null)
                            cell = row.CreateCell(j) as HSSFCell;
                        if (i == 0)
                        {
                            cell.SetCellValue(lsHeader[j]);
                            cell.CellStyle = styleHeader;
                        }
                        else
                        {
                            cell.CellStyle = styleContent4;
                        }
                    }
                }
                for (int j = 0; j < lsHeader.Count; j++)
                {
                    //sheet.AutoSizeColumn(j);
                    sheet.SetColumnWidth(j, 20 * 256);
                }
                #endregion

                workbook.Write(ms);
                workbook = null;
                return File(ms.ToArray(), "application/vnd.ms-excel", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{_reportName}.xls");
            }
        }

        public ActionResult CreateMoney(int? page, int? defaultPage, string id,
            string k, string k1, string k2, string k3, string start, string end)
        {

            return Content("給你請款檔!!");

            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 樣式－欄位名稱

            //字型
            HSSFFont fontHeader = workbook.CreateFont() as HSSFFont;
            fontHeader.FontHeightInPoints = 12;
            fontHeader.Boldweight = (short)FontBoldWeight.BOLD; //粗體
            fontHeader.FontName = "新細明體";

            //樣式
            HSSFCellStyle styleHeader = workbook.CreateCellStyle() as HSSFCellStyle;
            styleHeader.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleHeader.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleHeader.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleHeader.WrapText = true;
            styleHeader.SetFont(fontHeader);
            styleHeader.FillForegroundColor = HSSFColor.GREY_25_PERCENT.index;
            styleHeader.FillPattern = FillPatternType.SOLID_FOREGROUND;

            #endregion 樣式－欄位名稱

            #region 樣式－欄位內容

            //字型
            HSSFFont fontContent = workbook.CreateFont() as HSSFFont;
            fontContent.FontHeightInPoints = 12;
            fontContent.Boldweight = (short)FontBoldWeight.BOLD; //粗體
            fontContent.FontName = "新細明體";

            HSSFFont fontContent2 = workbook.CreateFont() as HSSFFont;
            fontContent2.FontHeightInPoints = 12;
            fontContent2.FontName = "新細明體";

            //樣式-粗體字+置中
            HSSFCellStyle styleContent = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleContent.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent.WrapText = true;
            styleContent.SetFont(fontContent);

            //樣式-粗體字+置左
            HSSFCellStyle styleContent2 = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
            styleContent2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent2.WrapText = true;
            styleContent2.SetFont(fontContent);

            //樣式-正常字+置中
            HSSFCellStyle styleContent3 = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent3.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleContent3.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent3.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent3.WrapText = true;
            styleContent3.SetFont(fontContent2);

            //樣式-正常字+置左
            HSSFCellStyle styleContent4 = workbook.CreateCellStyle() as HSSFCellStyle;
            styleContent4.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
            styleContent4.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleContent4.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleContent4.WrapText = true;
            styleContent4.SetFont(fontContent2);

            //超連結
            HSSFFont fontLink = workbook.CreateFont() as HSSFFont;
            fontLink.FontHeightInPoints = 12;
            fontLink.FontName = "新細明體";
            fontLink.Underline = (byte)FontUnderlineType.SINGLE;
            fontLink.Color = HSSFColor.BLUE.index;

            HSSFCellStyle styleLink = workbook.CreateCellStyle() as HSSFCellStyle;
            styleLink.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            styleLink.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            styleLink.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            styleLink.SetFont(fontLink);

            #endregion 樣式－欄位內容

            IQueryable<PLUS> list = GetPlusData(k, k1, k2, k3, start, end);
            string _reportName = $"{Function.GetSysUserName(User.Identity.Name)}_兌換品項紀錄";
            HSSFSheet sheet = workbook.CreateSheet(_reportName) as HSSFSheet;
            using (MemoryStream ms = new MemoryStream())
            {
                int iRowIndex = 1, iCellIndex = 0;
                foreach (PLUS m in list)
                {
                    iCellIndex = 0;
                    HSSFRow rowM = sheet.CreateRow(iRowIndex) as HSSFRow;

                    #region 基本資料

                    rowM.CreateCell(iCellIndex).SetCellValue(m.DATETIME1.Value.ToString("yyyy/MM/dd HH:mm:ss")); //建立日期
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT1); //兌換代碼
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.GetSTATUS()); //狀態
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DATA3.CONTENT1); //禮品名稱
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT2); //會員E-mail
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT3); //會員名稱
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(m.DECIMAL4.ToInt()); //金額
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DECIMAL5.ToInt()); //點數
                    iCellIndex++;
                    #endregion

                    iRowIndex++;
                }
                List<string> lsHeader = new List<string>();
                lsHeader = new List<string>
                    {
                        "兌換日期", "兌換代碼", "兌換狀態","禮品名稱",
                    "會員E-mail","會員名稱","金額","點數"
                };

                #region 套用格式
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = sheet.GetRow(i) as HSSFRow;
                    if (row == null)
                        row = sheet.CreateRow(i) as HSSFRow;
                    for (int j = 0; j < lsHeader.Count; j++)
                    {
                        HSSFCell cell = row.GetCell(j) as HSSFCell;
                        if (cell == null)
                            cell = row.CreateCell(j) as HSSFCell;
                        if (i == 0)
                        {
                            cell.SetCellValue(lsHeader[j]);
                            cell.CellStyle = styleHeader;
                        }
                        else
                        {
                            cell.CellStyle = styleContent4;
                        }
                    }
                }
                for (int j = 0; j < lsHeader.Count; j++)
                {
                    //sheet.AutoSizeColumn(j);
                    sheet.SetColumnWidth(j, 20 * 256);
                }
                #endregion

                workbook.Write(ms);
                workbook = null;
                return File(ms.ToArray(), "application/vnd.ms-excel", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{_reportName}.xls");
            }
        }

        #endregion

        #region PLUS 紅利點數管理

        #region 匯入發送點數

        public ActionResult ImportPoint(int? page, int? defaultPage,
            string k, string start, string end
            , string updateImport, HttpPostedFileBase fileImport)
        {
            if (IsPost())
            {
                string _value = string.Empty;
                if (!updateImport.IsNullOrEmpty() && fileImport != null)//新增
                {
                    _value = ImportP(fileImport);
                    Msgbox_Toast("點數匯入操作完成!!");
                }
                ViewBag.msg = _value;
            }
            return View();
        }

        /// <summary>
        /// 匯入紅利點數
        /// </summary>
        /// <param name="fileImport"></param>
        /// <returns></returns>
        string ImportP(HttpPostedFileBase fileImport)
        {
            List<string> msgs = new List<string>();
            try
            {
                if (fileImport != null && fileImport.ContentLength > 0)
                {
                    string _creater = User.Identity.Name;
                    //載入Excel檔案
                    using (ExcelPackage package = new ExcelPackage(fileImport.InputStream))
                    {
                        // 取得worksheet
                        ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

                        for (int i = ws.Dimension.Start.Row + 1; i <= ws.Dimension.End.Row; i++)
                        {
                            string v1 = ws.Cells[i, 1].Value.ToMyString();//發送日期
                            string v2 = ws.Cells[i, 2].Value.ToMyString();//會員email
                            string v3 = ws.Cells[i, 3].Value.ToMyString();//會員姓名
                            string v4 = ws.Cells[i, 4].Value.ToMyString();//來源
                            string v5 = ws.Cells[i, 5].Value.ToMyString();//點數
                            //string v6 = ws.Cells[i, 6].Value.ToMyString();//點數到期日 //改成發送日加1年
                            string v7 = ws.Cells[i, 6].Value.ToMyString();//備註
                            if (v2.IsNullOrEmpty())
                            {
                                msgs.Add($"第 {i} 筆資料會員E-mail為空值!!");
                                continue;
                            }
                            if (!Regex.IsMatch(v2, Function.EMAIL_REGEX))
                            {
                                msgs.Add($"第 {i} 筆資料E-mail 格式不符!!");
                                continue;
                            }
                            //會員資料對應
                            bool isNewUser = false;
                            USER fun6000 = iDB.GetAllAsNoTracking<USER>(MAIN_ID: "fun6000")
                                .Where(p => p.CONTENT1.Equals(v2)).FirstOrDefault();
                            if (fun6000 == null)
                            {
                                isNewUser = true;
                                msgs.Add($"第 {i} 筆資料自動新增會員 {v2} !!");
                                fun6000 = new USER();
                                fun6000.CREATER = _creater;
                                fun6000.PASSWORD = Function.DEFAULT_PASSWORD_SETUP.ToSHA1();
                                fun6000.MAIN_ID = "fun6000";
                                fun6000.CONTENT1 = v2;
                                fun6000.CONTENT2 = v3;
                                fun6000.CONTENT3 = v3;
                                fun6000.CONTENT4 = SexType.Male.ToIntValue();
                                fun6000.CONTENT5 = "";//電話
                                fun6000.CONTENT6 = "level01";//會員等級
                                fun6000.CONTENT7 = v4;//來源
                                iDB.Add<USER>(fun6000);
                                CreateMemberPlus(fun6000);
                                //寄通知信給新會員
                                //Send(fun6000);
                            }
                            PLUS data = new PLUS();
                            data.ID = Function.GetGuid();
                            data.MAIN_ID = fun6000.USER_ID;
                            data.CREATER = _creater;
                            data.PLUS_TYPE = "fun5002";
                            data.STATUS = "1";//預設1
                            data.CREATE_DATE = DateTime.Now;
                            data.CONTENT1 = "";
                            data.CONTENT2 = v2;
                            data.CONTENT3 = v3;
                            data.CONTENT5 = v7;
                            data.CONTENT6 = fun6000.USER_ID;
                            data.DATETIME1 = v1.ToDateTime();
                            //data.DATETIME2 = v6.ToDateTime();
                            data.DATETIME2 = data.DATETIME1.Value.AddYears(1).AddDays(-1);
                            if ("level99".Equals(fun6000.CONTENT6))//生態守護王
                            {
                                decimal _d5v = v5.ToInt();
                                Decimal.TryParse((v5.ToInt() * 1.2).ToMyDoubleString(false, 0), out _d5v);//1.2倍
                                data.DECIMAL5 = _d5v;
                                data.DECIMAL6 = 1;
                            }
                            else
                            {
                                data.DECIMAL5 = v5.ToInt();
                            }
                            iDB.Add<PLUS>(data);
                            //寄送點數送達通知
                            if (isNewUser)
                            {
                                Send(fun6000, data);
                            }
                            else
                            {
                                Send2(fun6000, data);
                            }
                            ResetMemberLevel(fun6000.USER_ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgs.Add(ex.Message);
            }
            return string.Join("<br />", msgs);
        }

        /// <summary>
        /// 新會員獲得紅利點數 index_01_data3
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public bool Send(USER d1, PLUS p1)
        {
            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/index_01_data3.html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sBody = _htmlBody;
            //string sBody = "<h3 style=\"color: #c74505\">您好,感謝您參與桃園市政府海岸管理工程處舉辦的各項活動，我們誠摯邀請您加入桃園All好遊網站成為會員。</h3>" +
            //    "<p><b>我們在桃園市推出一系統的生態遊程，與地方社區串連推動桃園環境教育及旅遊。</b></p>" +
            //    "<p><b>為了感謝您的加入，桃園All好遊新會員將獲得５點紅利積點。持續購買桃園All好遊在KKday的各項遊程，或是參加海岸管理工程處舉辦的各項活動，</b></p>" +
            //    "<p><b>例如:里海學堂或淨灘活動等，可獲得不同的紅利點數。紅利點數可至桃園All好遊合作商家兌換多項精美禮品。詳情請見:好禮兌換https://www.tyecotour.com.tw/ExchangeList</b></p>" +
            //        "<p><b>您的會員帳號：</b> " + d1.CONTENT1 + "</p>" +
            //        "<p><b>您的會員密碼：</b><b style=\"color: red\"> " + Function.DEFAULT_PASSWORD_SETUP + "</b></p>" +
            //        "<p><b>您本次獲得的紅利點數：" + (p1.DECIMAL5.ToInt() + 5) + "點</b></p>" +
            //    "<p><b>現在趕快來桃園All好遊領取您的點數吧!!</b></p>" +
            //    "<p>" + $"<a href='{Function.DEFAULT_ROOT_HTTP}'>［登入桃園All好遊］</a>" + "</p>" +
            //"<p>桃園All好遊活動小組 敬上</p>";
            //參數集合 0:from 1:fromName 2:subject 3:body
            LetterModel model = new LetterModel();
            model.Body = sBody;
            model.Subject = $"[桃園All好遊]加入會員邀請函";
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(d1.CONTENT1, d1.CONTENT2 + d1.CONTENT3);
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
        /// 既有會員獲得紅利點數 index_03
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public bool Send2(USER d1, PLUS p1)
        {
            //預設內容改由html讀入
            string _htmlBody = string.Empty;
            using (StreamReader reader = new StreamReader(Function.GetPhysicalPath("/Content/email/index_03.html")))
            {
                _htmlBody = reader.ReadToEnd();
            }
            string sBody = string.Format(_htmlBody, p1.DATETIME1.ToDefaultString(), p1.DECIMAL5.ToInt());
            //參數集合 0:from 1:fromName 2:subject 3:body
            LetterModel model = new LetterModel();
            model.Body = sBody;
            model.Subject = $"[桃園All好遊]獲得紅利積點通知";
            model.RecipientNameList = new Dictionary<string, string>();
            model.RecipientNameList.Add(d1.CONTENT1, d1.CONTENT2 + d1.CONTENT3);
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
        #endregion

        #region 兌換紀錄

        public ActionResult Index4(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            CreateData3Fun5000();
            return View($"{NodeID}_Index4", GetPlusData3(k, k1, k2, k3, start, end).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }


        #endregion

        #endregion
    }
}