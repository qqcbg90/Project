using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using KingspModel.Resources;
using MvcPaging;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace admin.Controllers
{
    /// <summary>
    /// 會員管理
    /// </summary>
    [NodeSelect("MemberLevel")]
    public class UserController : BaseController
	{
        #region const property
        #endregion

        #region 共用

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

		#region 會員管理 User
		public ActionResult Index(int? page, int? defaultPage, string k, string c1, string c2
            , string start, string end)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
            return View(GetData(k, c1, c2, start, end).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string c1, string c2)
		{
			//IsAdd = id.IsNullOrEmpty();
            SetIsEdit(IsAuthority(Authority_Right.Update));
            USER model = iDB.GetByIDAsNoTracking<USER>(id);
            if (model == null)
            {
                return GoIndex(NodeID, page, defaultPage, k);
            }
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 14)]
		public ActionResult Edit(string id, USER model, int? page, int? defaultPage, string k, string c1, string c2)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
			CheckAuthority(IsAdd ? Authority_Right.Add : Authority_Right.Update);

			if (!IsAdd)
			{
				//ModelState.Remove("PASSWORD");
			}

			if (ModelState.IsValid)
			{
                USER sys = null;
				if (!IsAdd)
				{
					sys = iDB.GetByID<USER>(id);
				}
				if (sys != null)
				{
					if (!model.PASSWORD.IsNullOrEmpty())
					{
						sys.PASSWORD = model.PASSWORD.ToSHA1();
					}
					//sys.CONTENT1 = model.CONTENT1;
					sys.CONTENT2 = model.CONTENT2;
                    sys.CONTENT3 = model.CONTENT3;
                    sys.CONTENT4 = model.CONTENT4;
                    sys.CONTENT5 = model.CONTENT5;
                    sys.CONTENT6 = model.CONTENT6;
                    sys.CONTENT7 = model.CONTENT7;
                    sys.DATETIME1 = model.DATETIME1;
                    if (IsAdd)
					{
						//sys.USER_ID = model.USER_ID;
						//sys.CREATER = User.Identity.Name;
						//IsSuccessful = iDB.Add<USER>(sys);
						//AlertMsg = Function.DEFAULT_ADD_MESSAGE;
					}
					else
					{
						sys.UPDATER = User.Identity.Name;
						sys.UPDATE_DATE = DateTime.Now;
						iDB.Save();
						IsSuccessful = true;
						AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
					}

					if (IsSuccessful)
					{
                        return GoIndex(NodeID, page, defaultPage, k);
					}
				}
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 14, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string c1, string c2, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			USER sys = iDB.GetByID<USER>(id);
			if (sys != null)
			{
                if (!iDB.Delete<USER>(id, really))
                    AlertMsg = Function.DELETE_ERROR_MESSAGE;
                else
                    AlertMsg = Function.DELETE_MESSAGE;
                //AlertMsg = sys.ENABLE.IsEnable() ? "已啟用!!" : "已停用!!";
            }
			return GoIndex(NodeID, page, defaultPage, k);
		}

        #endregion

        #region 匯出

        public ActionResult Export(string k, string start, string end, string c1, string c2, string c3, string c4, string c5, string s)
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

            IQueryable<USER> list = GetData(k, c1, c2, start, end);

            HSSFSheet sheet = workbook.CreateSheet("會員資料") as HSSFSheet;
            using (MemoryStream ms = new MemoryStream())
            {
                int iRowIndex = 1, iCellIndex = 0;
                List<PLUS> plusListAll = iDB.GetAllAsNoTracking<PLUS>()
                        .Where(p => !"fun3001".Equals(p.PLUS_TYPE)).ToList();//除了點數促銷全取
                foreach (USER m in list)
                {
                    #region 點數訂單計算
                    //取出所有plus
                    List<PLUS> plusList = plusListAll.Where(p => m.USER_ID.Equals(p.CONTENT6)).OrderByDescending(p => p.CREATE_DATE).ToList();
                    List<PLUS> fun4000 = plusList.Where(p => "fun4000".Equals(p.PLUS_TYPE)).OrderByDescending(p => p.CREATE_DATE).ToList();
                    List<PLUS> fun5002 = plusList.Where(p => "fun5002".Equals(p.PLUS_TYPE)).OrderByDescending(p => p.CREATE_DATE).ToList();
                    List<PLUS> fun5003 = plusList.Where(p => "fun5003".Equals(p.PLUS_TYPE)).OrderByDescending(p => p.CREATE_DATE).ToList();
                    //要加總
                    int _fun4000 = fun4000.Where(p => OrderStatus.Type1.ToIntValue().Equals(p.STATUS)).Sum(p => p.DECIMAL5.ToInt());
                    int _fun5002 = fun5002.Where(p => p.DATETIME1 <= DateTime.Today && p.DATETIME2 >= DateTime.Today).Sum(p => p.DECIMAL5.ToInt());
                    int _fun5002_all = fun5002.Sum(p => p.DECIMAL5.ToInt());
                    int _fun5003_2 = fun5003.Where(p => AuditStatus.Type2.ToIntValue().Equals(p.STATUS)).Sum(p => p.DECIMAL5.ToInt());
                    //要扣掉
                    int _fun5003 = fun5003.Where(p => AuditStatus.Type1.ToIntValue().Equals(p.STATUS)).Sum(p => p.DECIMAL5.ToInt());
                    //合計
                    string _c14 = (_fun4000 + _fun5002 + _fun5003_2 - _fun5003).ReplaceNumToThousand();
                    string _c15 = (_fun4000 + _fun5002_all + _fun5003_2).ReplaceNumToThousand();
                    #endregion
                    iCellIndex = 0;
                    HSSFRow rowM = sheet.CreateRow(iRowIndex) as HSSFRow;

                    #region 基本資料

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CREATE_DATE.ToString("yyyy/MM/dd HH:mm:ss")); //建立日期
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT1); //會員帳號E-mail
                    iCellIndex++;

                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT2); //會員姓
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT3); //會員名
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(((SexType)m.CONTENT4.ToInt()).GetDescription()); //性別
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT5); //電話
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.DATETIME1.ToDefaultString()); //生日
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(Function.GetNodeTitle(m.CONTENT6)); //會員等級
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(m.CONTENT7); //來源
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(_c14); //目前點數
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(_c15); //累積點數
                    iCellIndex++;
                    rowM.CreateCell(iCellIndex).SetCellValue(fun4000.Count); //訂單筆數
                    iCellIndex++;
                    #endregion

                    iRowIndex++;
                }
                List<string> lsHeader = new List<string>();
                lsHeader = new List<string>
                    {
                        "日期", "會員帳號E-mail", "會員姓",
                        "會員名","性別","電話","生日","會員等級","來源",
                        "目前點數" ,"累積點數","訂單筆數"
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
                return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + "_會員資料.xls");
            }
        }

        #endregion

    }
}
