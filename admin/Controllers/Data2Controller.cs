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
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace admin.Controllers
{
    /// <summary>
    /// 解說員管理
    /// </summary>
    [NodeSelect(new string[] { "DATA2_fun9003_PT", "fun9001" })]
    public class Data2Controller : BaseController
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
        /// 資料是讀NODE的 fun9001
        /// </summary>
        public readonly string[] NODE_DATA = new string[] { "fun9001" };
        #endregion

        #region 共用
        /// <summary>
        /// 建立遊程名稱下拉 ViewBag.Data1Fun3000Select
        /// </summary>
        void CreateData1Fun3000()
        {
            List<SelectListItem> items =
            iDB.GetAllAsNoTracking<DATA1>(MAIN_ID: "fun3000").OrderBy(p => p.CREATE_DATE)
                .Select(x => new SelectListItem() { Text = x.CONTENT1, Value = x.ID })
                    .ToList();
            ViewBag.Data1Fun3000Select = new SelectList(items, "Value", "Text");
        }


        /// <summary>
        /// 取得DATA2
        /// </summary>
        /// <param name=""></param>
        /// <param name="k"></param>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="k3"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IQueryable<DATA2> GetData2Data(string k, string k1, string k2,
            string k3, string start, string end)
        {
            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();
            return iDB.GetAll<DATA2>(MAIN_ID: NodeID)
                    .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k) || p.CONTENT4.Contains(k))
                    //&& (string.IsNullOrEmpty(k1) || p.PLUS.Any(x => 1 == x.ENABLE && "fun9003".Equals(x.PLUS_TYPE) && x.CONTENT1.Equals(k1)))
                    && (string.IsNullOrEmpty(k1) || p.CONTENT13.Contains(k1))
                    && (string.IsNullOrEmpty(k2) || p.CONTENT6.Equals(k2))
                    //&& ((p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd) || (p.DATETIME2 >= tmpStart && p.DATETIME2 < tmpEnd))
                    ).OrderByDescending(p => p.CREATE_DATE);

        }
        /// <summary>
        /// 取得PLUS 排班資料 fun9001
        /// </summary>
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
            return iDB.GetAllAsNoTracking<PLUS>()
                    .Where(p => p.PLUS_TYPE.Equals("fun9001")
                    && (string.IsNullOrEmpty(k1) || p.CONTENT1.Equals(k1))
                    && ((p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd) || (p.DATETIME2 >= tmpStart && p.DATETIME2 < tmpEnd))
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE);
        }

        #endregion

        #region DATA2 解說員管理
        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult Index(int? page, int? defaultPage, string k, string k1, string k2,
            string k3, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            if (NODE_DATA.Contains(NodeID))
            {
                return View($"{NodeID}_Index", iDB.GetAllAsNoTracking<NODE>(MAIN_ID: NodeID)
                    .OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
            }
            else
            {
                return View($"{NodeID}_Index", GetData2Data(k, k1, k2, k3, start, end).ToPagedList(page.ToMvcPaging(), _defaultPage));
            }
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
                //model.DATETIME1 = DateTime.Today;
                model.Atts = new List<ATTACHMENT>();
                model.Pics = new List<ATTACHMENT>();
                model.plusList = new List<PLUS>();
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                DATA2 data = iDB.GetByIDAsNoTracking<DATA2>(id);
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
                        CONTENT11 = data.CONTENT11,
                        CONTENT12 = data.CONTENT12,
                        CONTENT13 = data.CONTENT13,
                        CONTENT16 = data.CONTENT16,
                        CONTENT17 = data.CONTENT17,
                        CONTENT18 = data.CONTENT18,
                        CONTENT19 = data.CONTENT19,
                        CONTENT20 = data.CONTENT20,
                        Bool_DECIMAL1 = data.DECIMAL1 == 1,
                        Bool_DECIMAL2 = data.DECIMAL2 == 1,
                        DECIMAL3 = data.DECIMAL3.ToInt(),
                        DECIMAL4 = data.DECIMAL4.ToInt(),
                        DECIMAL5 = data.DECIMAL5.ToInt(),
                        DECIMAL6 = data.DECIMAL6.ToInt(),
                        DECIMAL7 = data.DECIMAL7.ToInt(),
                        DATETIME1 = data.DATETIME1,
                        DATETIME2 = data.DATETIME2,
                        Atts = data.GetAttachments(AttachmentType.File.ToIntValue()),
                        Pics = data.GetAttachments(),
                        plusList = data.PLUS.Where(p => p.ENABLE == 1).ToList()
                        //PH0 = data.GetParagraphByOrder(0).CONTENT,
                        //PH1 = data.GetParagraphByOrder().CONTENT
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
        [ActionLog(TableNameIndex = 16, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
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
                DATA2 data = iDB.GetByID<DATA2>(id);
                if (data == null)
                {
                    data = new DATA2()
                    {
                        ID = Function.GetGuid(),
                        NODE_ID = NodeID,
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
                data.DATA_TYPE = model.DATA_TYPE;
                data.STATUS = model.Bool_STATUS ? "1" : "0";
                data.CONTENT1 = model.CONTENT1;
                data.CONTENT2 = model.CONTENT2;
                data.CONTENT3 = model.CONTENT3;
                data.CONTENT4 = model.CONTENT4;
                data.CONTENT5 = model.CONTENT5;
                data.CONTENT6 = model.CONTENT6;
                data.CONTENT7 = model.CONTENT7;
                data.CONTENT8 = model.CONTENT8;
                data.CONTENT9 = model.CONTENT9;
                data.CONTENT10 = model.CONTENT10;
                data.CONTENT11 = model.CONTENT11;
                data.CONTENT12 = model.CONTENT12;
                data.CONTENT13 = model.CONTENT13;
                data.CONTENT16 = model.CONTENT16;
                data.CONTENT17 = model.CONTENT17;
                data.CONTENT18 = model.CONTENT18;
                data.CONTENT19 = model.CONTENT19;
                data.CONTENT20 = model.CONTENT20;
                data.DATETIME1 = model.DATETIME1;
                data.DATETIME2 = model.DATETIME2;
                data.DECIMAL1 = model.Bool_DECIMAL1 ? 1 : 0;
                data.DECIMAL2 = model.Bool_DECIMAL2 ? 1 : 0;
                data.DECIMAL3 = model.DECIMAL3;
                data.DECIMAL4 = model.DECIMAL4;
                data.DECIMAL5 = model.DECIMAL5;
                data.DECIMAL6 = model.DECIMAL6;
                data.DECIMAL7 = model.DECIMAL7;

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

                //#region 處理plus fun9000證照 fun9003
                //if ("fun9000".CheckStringValue(NodeID))
                //{
                //    //先統一刪除舊的
                //    string _delplus = $"delete * from plus where MAIN_ID='{data.ID}' and PLUS_TYPE='fun9003' ";
                //    iDB.ExecuteSqlCommand(_delplus);
                //    //end
                //    if (model.plusList != null && model.plusList.Count > 0)
                //    {
                //        foreach (var _plus in model.plusList)
                //        {
                //            PLUS plus = new PLUS();
                //            plus.ID = Function.GetGuid();
                //            plus.CREATER = User.Identity.Name;
                //            plus.CREATE_DATE = DateTime.Now;
                //            plus.PLUS_TYPE = "fun9003";
                //            plus.ENABLE = 1;
                //            plus.CONTENT1 = _plus.CONTENT1;                            
                //            data.PLUS.Add(plus);
                //        }
                //    }
                //}
                //#endregion

                if (IsAdd)
                {
                    IsSuccessful = iDB.Add<DATA2>(data);
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

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.start = start;
            ViewBag.end = end;

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit", model);
        }

        [ActionLog(TableNameIndex = 16, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = false)
        {
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<DATA2>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
        }

        #region 匯出解說員資料

        public ActionResult ExportIndex(int? page, int? defaultPage,
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

            IQueryable<DATA2> list = GetData2Data(k, k1, k2, k3, start, end);
            string _reportName = $"解說員資料";
            HSSFSheet sheet = workbook.CreateSheet(_reportName) as HSSFSheet;
            using (MemoryStream ms = new MemoryStream())
            {
                int iRowIndex = 1, iCellIndex = 0;
                foreach (DATA2 m in list)
                {
                    iCellIndex = 0;
                    HSSFRow rowM = sheet.CreateRow(iRowIndex) as HSSFRow;

                    #region 基本資料

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CREATE_DATE.ToString("yyyy/MM/dd HH:mm")); //建立日期
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT1); //中文姓名
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT4); //英文姓名
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT3); //身份證字號
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT2); //E-mail
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.GetValueOnLang()); //場域類別
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.GetValueOnLang(5)); //性別
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DATETIME1.ToDefaultString()); //生日
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.GetValueOnLang(6)); //語言別
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT7); //學歷
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT8); //連絡地址
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT9); //連絡電話
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT10); //手機號碼
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT11); //緊急聯絡人姓名
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT12); //緊急聯絡人電話
                    iCellIndex++;
                    
                    #endregion

                    iRowIndex++;
                }
                List<string> lsHeader = new List<string>();
                lsHeader = new List<string>
                    {
                        "建立日期", "中文姓名", "英文姓名","身份證字號","E-mail",
                    "場域類別","性別","生日","語言別","學歷","連絡地址","連絡電話","手機號碼","緊急聯絡人姓名","緊急聯絡人電話"
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

        

        #endregion

        #region 回訓資料

        public ActionResult Index2(int? page, int? defaultPage, string k, string k1)
        {
            DATA2 data = iDB.GetByIDAsNoTracking<DATA2>(k1);
            if (data == null)
                return GoIndex(NodeID, page, defaultPage, k);
            SetContentTitle($"{data.CONTENT1}-回訓資料");
            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            ViewBag.k1 = k1;
            return View($"{NodeID}_Index2", iDB.GetAllAsNoTracking<PLUS>()
                    .Where(p => p.PLUS_TYPE.Equals("fun9002")
                    && p.MAIN_ID.Equals(data.ID)
                    ).OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }

        [ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete2(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = false)
        {
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<PLUS>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }), "Index2");
        }

        #region 回訓資料編輯

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit2(string id, int? page, int? defaultPage, string k, string k1, string k2
            , string k3, string start, string end)
        {
            DataModel model = new DataModel();
            if (id.IsNullOrEmpty())
            {
                CheckAuthority(Authority_Right.Add);
                //model.DATETIME1 = DateTime.Today;
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
                        DECIMAL1= data.DECIMAL1.ToInt(),
                        DECIMAL2 = data.DECIMAL2,
                        DATETIME1 = data.DATETIME1,
                        DATETIME2 = data.DATETIME2,
                        Data2 = data.DATA2
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
            SetContentTitle($"回訓資料");
            return View($"{NodeID}_Edit2", model);
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
                        PLUS_TYPE = "fun9002",
                        CREATER = User.Identity.Name,
                        MAIN_ID = k1
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
                //data.STATUS = model.STATUS;
                data.CONTENT1 = model.CONTENT1;//課程名稱
                //data.CONTENT2 = model.CONTENT2;
                //data.CONTENT3 = model.CONTENT3;
                //data.CONTENT4 = model.CONTENT4;
                //data.CONTENT5 = model.CONTENT5;//備註
                //data.CONTENT6 = model.CONTENT6;
                //data.CONTENT7 = model.CONTENT7;
                data.DECIMAL1 = model.DECIMAL1;//時數
                data.DATETIME1 = model.DATETIME1;//課程日期
                //data.DATETIME3 = model.DATETIME3;

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
                    return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "start", "end" }), "Index2");
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit2", model);
        }


        #endregion

        #endregion

        #region 排班資料設定

        #region 班別資料

        [ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete3(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = false)
        {
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<NODE>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
        }

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit3(string id, int? page, int? defaultPage, string k, string k1, string k2
            , string k3, string start, string end)
        {
            DataModel model = new DataModel();
            if (id.IsNullOrEmpty())
            {
                CheckAuthority(Authority_Right.Add);
                //model.DATETIME1 = DateTime.Today;
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                NODE data = iDB.GetByIDAsNoTracking<NODE>(id);
                if (data != null)
                {
                    model = new DataModel()
                    {
                        ID = data.ID,
                        CONTENT1 = data.TITLE,//Title
                        CONTENT2 = data.CONTENT1,//Content1
                        CONTENT3 = data.CONTENT3,
                        CONTENT4 = data.CONTENT4,
                        CONTENT5 = data.CONTENT5
                    };
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;
            return View($"{NodeID}_Edit3", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit3(string id, DataModel model, int? page, int? defaultPage, string k, string k1
            , string k2, string k3, string start, string end
            )
        {
            CheckAuthority(Authority_Right.Update);
            IsSuccessful = true;
            string sWarningMsg = string.Empty;

            if (ModelState.IsValid)
            {
                IsAdd = id.IsNullOrEmpty();
                NODE data = iDB.GetByID<NODE>(id);
                if (data == null)
                {
                    data = new NODE()
                    {
                        ID = Function.GetGuid(),
                        PARENT_ID = "fun9001",
                        CREATER = User.Identity.Name
                    };
                }
                else
                {
                    data.UPDATER = User.Identity.Name;
                    data.UPDATE_DATE = DateTime.Now;
                }
                data.TITLE = model.CONTENT1;//名稱
                data.CONTENT1 = model.CONTENT2;
                if (IsAdd)
                {
                    IsSuccessful = iDB.Add<NODE>(data);
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
            return View($"{NodeID}_Edit3", model);
        }

        #endregion

        #region 排班設定

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
            return View($"{NodeID}_Index4", GetPlusData(k, k1, k2, k3, start, end).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit4(string id, int? page, int? defaultPage, string k, string k1, string k2
            , string k3, string start, string end)
        {
            DataModel model = new DataModel();
            if (id.IsNullOrEmpty())
            {
                CheckAuthority(Authority_Right.Add);
                //model.DATETIME1 = DateTime.Today;
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
                        MAIN_ID = data.MAIN_ID,
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
                        DECIMAL1 = data.DECIMAL1.ToInt(),
                        DECIMAL2 = data.DECIMAL2.ToInt(),
                        DATETIME1 = data.DATETIME1,
                        DATETIME2 = data.DATETIME2
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
            return View($"{NodeID}_Edit4", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit4(string id, DataModel model, int? page, int? defaultPage, string k, string k1
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
                        PLUS_TYPE = "fun9001",
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
                //data.STATUS = model.STATUS;
                data.MAIN_ID = model.MAIN_ID;
                data.CONTENT1 = model.CONTENT1;//解說場域
                data.CONTENT2 = model.CONTENT2;
                data.CONTENT3 = model.CONTENT3;
                data.CONTENT4 = model.CONTENT4;
                data.CONTENT5 = model.CONTENT5;//備註
                data.CONTENT6 = model.CONTENT6;
                data.CONTENT7 = model.CONTENT7;
                data.CONTENT8 = model.CONTENT8;
                data.DECIMAL1 = model.DECIMAL1;//預約人數
                data.DECIMAL2 = model.DECIMAL2;//預約時間時數
                data.DATETIME1 = model.DATETIME1;//預約時間起
                data.DATETIME2 = model.DATETIME2;//預約時間迄
                //data.DATETIME3 = model.DATETIME3;

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
                    return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "start", "end" }), "Index4");
                }
            }

            ViewBag.k1 = k1;
            ViewBag.k2 = k2;
            ViewBag.k3 = k3;
            ViewBag.start = start;
            ViewBag.end = end;

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit4", model);
        }

        [ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete4(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = false)
        {
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<PLUS>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }), "Index4");
        }

        #endregion

        #region 匯出排班資料統計
        //依解說員匯出
        public ActionResult ExportIndex4_1(int? page, int? defaultPage,
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

            List<PLUS> list = GetPlusData(k, k1, k2, k3, start, end).ToList();
            string _reportName = $"依解說員排班統計";
            NodeID = "fun9000";
            List<DATA2> data2 = GetData2Data(null, null, null, null, null, null).ToList();
            HSSFSheet sheet = workbook.CreateSheet(_reportName) as HSSFSheet;
            using (MemoryStream ms = new MemoryStream())
            {
                int iRowIndex = 1, iCellIndex = 0;
                foreach (DATA2 m in data2)
                {
                    iCellIndex = 0;
                    HSSFRow rowM = sheet.CreateRow(iRowIndex) as HSSFRow;

                    #region 基本資料

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT1); //解說員姓名
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(list.Count(x => x.MAIN_ID.Equals(m.ID))); //排班數量
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(list.Where(x => x.MAIN_ID.Equals(m.ID)).Sum(x => x.DECIMAL1.ToInt())); //服務人次
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(list.Where(x => x.MAIN_ID.Equals(m.ID)).Sum(x => x.DECIMAL2.ToInt())); //服務時數
                    iCellIndex++;

                    #endregion

                    iRowIndex++;
                }
                List<string> lsHeader = new List<string>();
                lsHeader = new List<string>
                    {
                        "解說員姓名", "排班數量", "服務人次","服務時數"
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

        //依日期
        public ActionResult ExportIndex4_2(int? page, int? defaultPage,
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

            List<PLUS> list = GetPlusData(k, k1, k2, k3, start, end).ToList();
            string _reportName = $"依日期排班統計";
            HSSFSheet sheet = workbook.CreateSheet(_reportName) as HSSFSheet;
            using (MemoryStream ms = new MemoryStream())
            {
                int iRowIndex = 1, iCellIndex = 0;
                foreach (PLUS m in list)
                {
                    iCellIndex = 0;
                    HSSFRow rowM = sheet.CreateRow(iRowIndex) as HSSFRow;

                    #region 基本資料

                    rowM.CreateCell(iCellIndex).SetCellValue(m.GetDateRange()); //日期
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(Function.GetNodeTitle(m.CONTENT1)); //場域
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DECIMAL1.ToInt()); //服務人次
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DECIMAL2.ToInt()); //服務時數
                    iCellIndex++;

                    #endregion

                    iRowIndex++;
                }
                List<string> lsHeader = new List<string>();
                lsHeader = new List<string>
                    {
                        "預約日期", "解說場域", "服務人次","服務時數"
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
                    if (j == 0)
                    {
                        sheet.SetColumnWidth(j, 40 * 256);
                    }
                    else
                    {
                        sheet.SetColumnWidth(j, 20 * 256);
                    }
                }
                #endregion

                workbook.Write(ms);
                workbook = null;
                return File(ms.ToArray(), "application/vnd.ms-excel", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{_reportName}.xls");
            }
        }

        #endregion

        #endregion

    }
}