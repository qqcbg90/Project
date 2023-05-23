using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class InventoryController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
		/// <summary>
		/// 物品類別
		/// </summary>
		public const string INVENTORY_ITEM = "INVENTORY_ITEM";
		/// <summary>
		/// 物品領用
		/// </summary>
		public const string INVENTORY_REQUISITION = "INVENTORY_REQUISITION";
		/// <summary>
		/// 物品領用 - 細項
		/// </summary>
		public const string INVENTORY_REQUISITION_ITEM = "INVENTORY_REQUISITION_ITEM";
		/// <summary>
		/// 物品領用申請
		/// </summary>
		public const string fun03 = "fun03";
		/// <summary>
		/// 物品領用審核
		/// </summary>
		public const string fun04 = "fun04";
		/// <summary>
		/// 物品及庫存管理>可領用物品項目管理
		/// </summary>
		public const string fun11_01 = "fun11_01";
		/// <summary>
		/// 物品及庫存管理>物品審核管理
		/// </summary>
		public const string fun11_02 = "fun11_02";
		/// <summary>
		/// 物品及庫存管理>庫存報表
		/// </summary>
		public const string fun11_03 = "fun11_03";
		/// <summary>
		/// 物品及庫存管理>單位物品領用統計表
		/// </summary>
		public const string fun11_04 = "fun11_04";

		/// <summary>
		/// 建立出入庫申請單編號
		/// </summary>
		private string getInventoryID(string id)
		{
			string Today = DateTime.Now.ToString("yyyyMMdd");
			if (id.IsNullOrEmpty())
			{
				string maxID = iDB.GetAllAsNoTracking<DATA5>(false, INVENTORY_REQUISITION).Where(p => !p.ID.Contains("-") && p.ID.StartsWith(Today)).Select(p => p.ID).Max(p => p);
				if (!maxID.IsNullOrEmpty())
				{
					return Convert.ToString(Convert.ToInt64(maxID) + 1);
				}
				return Today + "00001";
			}
			else //重新申請
			{
				id = id.IndexOf("-") != -1 ? id.Remove(id.IndexOf("-")) : id;
				string maxID = iDB.GetAllAsNoTracking<DATA5>(false, INVENTORY_REQUISITION).Where(p => p.ID.Contains("-") && p.ID.StartsWith(Today)).Select(p => p.ID).Max(p => p);
				if (!maxID.IsNullOrEmpty())
				{
					string[] arrID = maxID.Split('-');
					if (arrID.Length == 2)
					{
						return arrID[0] + "-" + (Convert.ToInt32(arrID[1]) + 1);
					}
				}
				return id + "-1";
			}
		}

		#region 物品及庫存管理>可領用物品項目管理>類別管理
		public ActionResult CategoryIndex(int? page, int? defaultPage, string k)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAllAsNoTracking<NODE>(MAIN_ID: INVENTORY_ITEM).OrderBy(p => p.ORDER).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		public ActionResult CategoryEdit(string id, int? page, int? defaultPage, string k)
		{
			SetContentTitle("類別管理", 2);
			ViewBag.ID = id;

			NODE model = new NODE();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				model = iDB.GetByIDAsNoTracking<NODE>(id);
			}
			return View(model);
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 0, Description = "新增/編輯 可領用物品項目管理-類別管理")]
		public ActionResult CategoryEdit(string id, int? page, int? defaultPage, string k, NODE model)
		{
			SetContentTitle("類別管理", 2);
			ViewBag.ID = id;
			IsAdd = id.IsNullOrEmpty();

			NODE n = new NODE();
			if (ModelState.IsValid)
			{
				if (IsAdd)
				{
					iDB.Add<NODE>(new NODE()
					{
						CREATER = User.Identity.Name,
						PARENT_ID = INVENTORY_ITEM,
						TITLE = model.TITLE,
						ORDER = iDB.GetAllAsNoTracking<NODE>(MAIN_ID: INVENTORY_ITEM).Max(p => p.ORDER) + 1
					});
				}
				else
				{
					n = iDB.GetByID<NODE>(id);
					if (n != null)
					{
						n.UPDATER = User.Identity.Name;
						n.UPDATE_DATE = DateTime.Now;
						n.TITLE = model.TITLE;
					}
					iDB.Save();
				}
				UpdateNodeList();
				return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id" }), "CategoryIndex");
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteCategory(string id, int? page, int? defaultPage, string k, bool really = false, string actionName = "Index")
		{
			CheckAuthority(Authority_Right.Delete);
			if (iDB.GetAllAsNoTracking<NODE>(MAIN_ID: id).Any())
			{
				AlertMsg = "類別下已有資料，不可刪除!!";
			}
			else
			{
				//不是真的刪除時，記錄刪除人及刪除時間
				if (!really)
				{
					NODE n = iDB.GetByID<NODE>(id, false);
					if (n != null)
					{
						n.CONTENT10 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
					}
				}
				if (!iDB.Delete<NODE>(id, really))
				{
					AlertMsg = Function.DELETE_ERROR_MESSAGE;
				}
				UpdateNodeList();
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), actionName);
		}
		#endregion

		#region 物品及庫存管理>可領用物品項目管理
		/// <summary>
		/// 列表
		/// </summary>
		[NodeSelect(INVENTORY_ITEM)]
		public ActionResult ItemIndex(int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;
			if (k1.IsNullOrEmpty())
			{
				k1 = string.Join(";", (ViewBag.INVENTORY_ITEM as SelectList).Select(p => p.Value).ToArray());
			}
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			IQueryable<NODE> list = iDB.GetAllAsNoTracking<NODE>()
				.Where(p => (string.IsNullOrEmpty(k) || p.TITLE.Contains(k)) && (string.IsNullOrEmpty(k1) || k1.Contains(p.PARENT_ID))).OrderBy(p => p.TITLE);
			return View(list.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[NodeSelect(INVENTORY_ITEM)]
		public ActionResult ItemEdit(string id, int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;

			InventoryItemModel model = new InventoryItemModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				NODE n = iDB.GetByIDAsNoTracking<NODE>(id);
				if (n != null)
				{
					model = new InventoryItemModel()
					{
						ID = n.ID,
						PARENT_ID = n.PARENT_ID,
						TITLE = n.TITLE,
						ORDER = n.ORDER,
						CONTENT1 = n.CONTENT1,
						CONTENT8 = n.CONTENT8,
						CONTENT9 = n.CONTENT9,
						atta = n.ATTACHMENT.FirstOrDefault()
					};
				}
			}
			return View(model);
		}

		[NodeSelect(INVENTORY_ITEM)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 0, Description = "新增/編輯 可領用物品項目管理")]
		public ActionResult ItemEdit(string id, InventoryItemModel model, int? page, int? defaultPage, string k, string k1)
		{
			CheckAuthority(Authority_Right.Update);
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				NODE n = iDB.GetByID<NODE>(id);
				if (n == null)
				{
					n = new NODE()
					{
						ID = Function.GetGuid(),
						CREATER = User.Identity.Name,
						PARENT_ID = model.PARENT_ID,
						CONTENT9 = "0"
					};
				}
				else
				{
					n.UPDATER = User.Identity.Name;
					n.UPDATE_DATE = DateTime.Now;
				}
				n.TITLE = model.TITLE;
				n.ORDER = model.ORDER ?? 0;
				n.CONTENT1 = model.CONTENT1;
				n.CONTENT8 = model.CONTENT8.IsTrue() ? "1" : "0";

				string sOldPicID = string.Empty;
				HttpPostedFileBase hpf = model.hpf;
				if (hpf != null && hpf.ContentLength > 0)
				{
					ATTACHMENT oldAtt = n.ATTACHMENT.FirstOrDefault();
					sOldPicID = oldAtt == null ? string.Empty : oldAtt.ID;

					string sExt = Path.GetExtension(hpf.FileName).ToLower();
					if (hpf.ContentLength > IMG_UPLOAD_MAX_SIZE)
					{
						sWarningMsg += hpf.FileName + "：圖片大小超過 2 MB！";
					}
					else if (Function.DEFAULT_FILEUPLOAD_PICTURE_EXT.IndexOf(sExt) == -1)
					{
						sWarningMsg += hpf.FileName + "：圖片格式不符！";
					}
					if (sWarningMsg.IsNullOrEmpty())
					{
						ATTACHMENT att = new ATTACHMENT(hpf.FileName);
						att.ATT_TYPE = AttachmentType.Image.ToIntValue();
						att.SetUpFileName();
						att.CREATER = User.Identity.Name;
						att.ORDER = 0;
						att.CONTENT9 = EnableType.Enable.ToIntValue();
						n.ATTACHMENT.Add(att);
						SaveAtt(hpf, att.FILE_NAME);
					}
				}

				if (IsAdd)
				{
					IsSuccessful = iDB.Add<NODE>(n);
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
				}
				if (IsSuccessful)
				{
					if (!sOldPicID.IsNullOrEmpty() && sWarningMsg.IsNullOrEmpty())
					{
						iDB.Delete<ATTACHMENT>(sOldPicID, true);
					}
					UpdateNodeList();
					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1" }), "ItemIndex");
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}
		#endregion

		#region 物品及庫存管理>可領用物品項目理>庫存管理

		public ActionResult InventoryIndex(string id, string nid, int? page, int? defaultPage, string k, DateTime? start, DateTime? end, int export = 0)
		{
			SetContentTitle("庫存管理", 2);
			ViewBag.ID = id;
			SetRouteValueIncludeCommon(new string[] { "start", "end" });

			NODE n = Function.GetNode(id);
			if (n != null)
			{
				ViewBag.Item = Function.GetNodeTitle(n.PARENT_ID) + " > " + n.TITLE;
			}

			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			IQueryable<PLUS> list = iDB.GetAllAsNoTracking<PLUS>()
				.Where(p => p.DATA5.ENABLE == 1 && p.DATA5.ORDER != 2 &&
				p.STATUS.Equals(INVENTORY_REQUISITION_ITEM) && p.PLUS_TYPE.Equals(id) &&
				p.CREATE_DATE >= tmpStart && p.CREATE_DATE < tmpEnd).OrderBy(p => p.CREATE_DATE);
			if (export == 1)
			{
				return InventoryExport(n.TITLE, list.ToList(), start, end, export);
			}

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			ViewBag.DefaultPage = _defaultPage;
			page = IsPost() ? 0 : page;
			return View(list.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		public ActionResult InventoryExport(string title, List<PLUS> model, DateTime? start, DateTime? end, int export)
		{
			string sFileName = title + "_出入庫報表";
			List<string> lsHeader = new List<string>() { "日期 ", "出/入庫 ", "數量 ", "用途", "申請單位 ", "申請人" };
			List<int> lsW = new List<int>() { 12, 10, 10, 70, 12, 12 };
			HSSFWorkbook workbook = new HSSFWorkbook();

			#region 樣式－欄位名稱

			//字型
			HSSFFont fontHeader = workbook.CreateFont() as HSSFFont;
			//fontHeader.FontHeightInPoints = 12;
			//fontHeader.Boldweight = (short)FontBoldWeight.BOLD; //粗體
			//fontHeader.FontName = "標楷體";

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
			//fontContent.FontHeightInPoints = 12;
			fontContent.Boldweight = (short)FontBoldWeight.BOLD; //粗體
																 //fontContent.FontName = "標楷體";

			HSSFFont fontContent2 = workbook.CreateFont() as HSSFFont;
			//fontContent2.FontHeightInPoints = 12;
			//fontContent2.FontName = "標楷體";

			//樣式-粗體字+置中
			HSSFCellStyle CenterBoldBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			CenterBoldBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			CenterBoldBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			CenterBoldBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.WrapText = true;
			CenterBoldBorder.SetFont(fontContent);

			//樣式-粗體字+置左
			HSSFCellStyle LeftBoldBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			LeftBoldBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
			LeftBoldBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			LeftBoldBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.WrapText = true;
			LeftBoldBorder.SetFont(fontContent);

			//樣式-正常字+置中
			HSSFCellStyle NormalCenterBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalCenterBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			NormalCenterBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalCenterBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.WrapText = true;
			NormalCenterBorder.SetFont(fontContent2);

			//樣式-正常字+置左
			HSSFCellStyle NormalLeftBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalLeftBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
			NormalLeftBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalLeftBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.WrapText = true;
			NormalLeftBorder.SetFont(fontContent2);

			//樣式-正常字+置左
			HSSFCellStyle NormalRightBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRightBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRightBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRightBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.WrapText = true;
			NormalRightBorder.SetFont(fontContent2);

			//樣式-正常字+置中
			HSSFCellStyle NormalCenter = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalCenter.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			NormalCenter.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalCenter.WrapText = true;
			NormalCenter.SetFont(fontContent2);

			//樣式-正常字+靠右
			HSSFCellStyle NormalRight = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRight.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRight.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRight.WrapText = true;
			NormalRight.SetFont(fontContent2);

			#endregion 樣式－欄位內容

			using (MemoryStream ms = new MemoryStream())
			{
				HSSFSheet sheet = workbook.CreateSheet(replaceExcelSheetName(sFileName)) as HSSFSheet;
				int iRowIndex = 4;
				HSSFRow row = sheet.CreateRow(iRowIndex) as HSSFRow;
				foreach (PLUS m in model)
				{
					DATA5 d5 = m.DATA5;
					row = sheet.CreateRow(iRowIndex) as HSSFRow;
					row.CreateCell(0).SetCellValue(m.CREATE_DATE.ToDateString());
					row.CreateCell(1).SetCellValue(m.ORDER == 0 ? "入庫" : "出庫");
					row.CreateCell(2).SetCellValue((m.ORDER == 0 ? 1 : -1) * m.DECIMAL1.ToInt());
					row.CreateCell(3).SetCellValue(d5.CONTENT1);
					row.CreateCell(4).SetCellValue(d5.DATA_TYPE);
					row.CreateCell(5).SetCellValue(Function.GetSysUserName(d5.CREATER));
					iRowIndex++;
				}
				sheet.CreateRow(0).CreateCell(0).SetCellValue("桃園市政府藝文設施管理中心 出入庫報表");
				sheet.CreateRow(1).CreateCell(0).SetCellValue("統計項目：" + title);
				sheet.CreateRow(2).CreateCell(0).SetCellValue("統計日期：" + DateTime.Now.ToString("yyyy年MM月dd日"));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 5));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 5));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2, 2, 0, 5));

				//表頭
				HSSFRow rowHeader = sheet.CreateRow(3) as HSSFRow;
				for (int i = 0; i < lsHeader.Count; i++)
				{
					rowHeader.CreateCell(i).SetCellValue(lsHeader[i]);
				}

				//套用格式
				int iLastRowNum = sheet.LastRowNum;
				for (int i = 0; i <= iLastRowNum; i++)
				{
					HSSFRow rowX = sheet.GetRow(i) as HSSFRow;
					if (rowX == null)
						rowX = sheet.CreateRow(i) as HSSFRow;
					for (int j = 0; j < lsHeader.Count; j++)
					{
						HSSFCell cell = rowX.GetCell(j) as HSSFCell;
						if (cell == null)
							cell = rowX.CreateCell(j) as HSSFCell;
						if (i >= 0 && i < 3)
						{
							cell.CellStyle = i == 0 ? NormalCenter : NormalRight;
						}
						else
						{
							cell.CellStyle = i == 3 ? styleHeader : NormalCenterBorder;
						}
					}
				}

				for (int j = 0; j < lsW.Count; j++)
				{
					sheet.SetColumnWidth(j, lsW[j] * 256);
				}

				#region 查詢條件
				HSSFSheet sheetLimit = workbook.CreateSheet("查詢條件") as HSSFSheet;
				HSSFRow rowLimit = sheetLimit.CreateRow(0) as HSSFRow;
				rowLimit.CreateCell(0).SetCellValue("日期區間");
				rowLimit.CreateCell(1).SetCellValue(start.ToDateString() + "~" + end.ToDateString());
				sheetLimit.AutoSizeColumn(0);
				sheetLimit.AutoSizeColumn(1);
				#endregion

				workbook.Write(ms);
				workbook = null;
				return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + sFileName + ".xls");
			}
		}

		public ActionResult InventoryEdit(string id, string nid, int? page, int? defaultPage, string k, DateTime? start, DateTime? end)
		{
			SetContentTitle("庫存管理", 2);
			ViewBag.ID = id;
			SetRouteValueIncludeCommon(new string[] { "start", "end" });

			InventoryRequisitionDetailModel model = new InventoryRequisitionDetailModel();
			NODE n = Function.GetNode(id);
			if (n != null)
			{
				model.PLUS_TYPE = n.ID;
				model.PLUS_TYPE_TITLE = Function.GetNodeTitle(n.PARENT_ID) + " > " + n.TITLE;
			}
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
			}
			return View(model);
		}

		[HttpPost]
		public ActionResult InventoryEdit(string id, string nid, int? page, int? defaultPage, string k, DateTime? start, DateTime? end, InventoryRequisitionDetailModel model)
		{
			CheckAuthority(Authority_Right.Add);
			SetContentTitle("庫存管理", 2);
			ViewBag.ID = id;
			SetRouteValueIncludeCommon(new string[] { "start", "end" });
			NODE n = iDB.GetByID<NODE>(id);
			if (n != null)
			{
				int iNowNum = Convert.ToInt32(n.CONTENT9);
				int iD1 = model.DECIMAL1.Value;
				model.PLUS_TYPE_TITLE = Function.GetNodeTitle(n.PARENT_ID) + " > " + n.TITLE;
				if (model.ORDER == 1 && iNowNum < iD1)
				{
					SetModelStateError("出庫量不得大於目前庫存量！");
					return View(model);
				}
				if (ModelState.IsValid)
				{
					string UserID = User.Identity.Name;
					string InventoryID = getInventoryID(string.Empty);
					DATA5 d5 = new DATA5()
					{
						ID = InventoryID,
						NODE_ID = INVENTORY_REQUISITION,
						CREATE_DATE = DateTime.Now,
						CREATER = UserID,
						DATA_TYPE = Function.GetSysUserDept(UserID),
						STATUS = "1",
						ORDER = 1,
						CONTENT29 = UserID,
						DATETIME1 = DateTime.Now,
						CONTENT1 = model.CONTENT1
					};
					iNowNum = (model.ORDER == 0 ? (iNowNum + iD1) : (iNowNum - iD1));
					d5.PLUS.Add(new PLUS()
					{
						ID = Function.GetGuid(),
						ENABLE = 1,
						CREATE_DATE = d5.CREATE_DATE,
						CREATER = UserID,
						MAIN_ID = InventoryID,
						PLUS_TYPE = n.ID,
						STATUS = INVENTORY_REQUISITION_ITEM,
						ORDER = model.ORDER.Value,
						DECIMAL1 = iD1,
						DECIMAL2 = iNowNum
					});
					if (iDB.Add<DATA5>(d5))
					{
						n.CONTENT9 = iNowNum.ToString();
						iDB.Save();
						UpdateNodeList();
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id" }), "InventoryIndex");
					}
				}
			}
			SetModelStateError();
			return View(model);
		}
		#endregion

		#region 物品領用申請＆物品領用審核＆物品及庫存管理>物品審核管理
		[AuditStatusSelect]
		public ActionResult Index(string id, string nid, int? page, int? defaultPage, string k, DateTime? start, DateTime? end)
		{
			int iAuditStatus = k.ToInt();
			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			bool IsAudit = NodeID.CheckStringValue("fun04") || NodeID.CheckStringValue("fun11_02");
			string UserID = User.Identity.Name;

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			IQueryable<DATA5> list = iDB.GetAllAsNoTracking<DATA5>()
				.Where(p => (IsAudit ? true : p.CREATER.Equals(UserID)) && p.STATUS.Equals("2") /*物品領用*/ &&
				(string.IsNullOrEmpty(k) || p.ORDER == iAuditStatus) && (p.CREATE_DATE >= tmpStart && p.CREATE_DATE < tmpEnd))
				.OrderByDescending(p => p.ID);
			return View(list.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		[NodeSelect(INVENTORY_ITEM)]
		public ActionResult InventoryAdd(string nid, string k, string k1)
		{
			if (IsPost())
			{
				string SqlStr = @"SELECT p.ID as PARENT_ID, p.TITLE as PARENT_TITLE, n.ID, n.TITLE, n.CONTENT1, CONVERT(int,n.CONTENT9) as CONTENT9, a.[FILE_NAME]
FROM NODE n
JOIN NODE p ON n.PARENT_ID = p.ID
AND n.[ENABLE] = 1 AND p.[ENABLE] = 1 AND p.PARENT_ID = 'INVENTORY_ITEM'
LEFT JOIN ATTACHMENT a ON a.MAIN_ID = n.ID
WHERE ISNULL(n.CONTENT8,'0') = '1'
AND (@k1 = '' OR p.ID = @k1) AND (@k = '' OR n.TITLE LIKE '%' + @k + '%')
ORDER BY p.[ORDER], n.TITLE";
				IEnumerable<DataRow> drs;
				using (DBEntities db = new DBEntities())
				{
					drs = Function.getDataTable(db, SqlStr, new SqlParameter("k", k), new SqlParameter("k1", k1)).AsEnumerable();
				}
				return View((drs.Select(x => new InventoryRequisitionDetailModel
				{
					PLUS_TYPE = x.Field<string>("ID"),
					PLUS_TYPE_TITLE = x.Field<string>("PARENT_TITLE") + " > " + x.Field<string>("TITLE"),
					PLUS_TYPE_UNIT = x.Field<string>("CONTENT1"),
					PLUS_TYPE_IMG = x.Field<string>("FILE_NAME"),
					DECIMAL1 = 1,
					DECIMAL2 = x.Field<int>("CONTENT9")
				})).ToList());
			}
			return View(new List<InventoryRequisitionDetailModel>());
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[AuditStatusSelect]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string start, string end, int print = 0)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
			bool IsAudit = NodeID.CheckStringValue(fun04) || NodeID.CheckStringValue(fun11_02);
			ViewBag.IsAudit = IsAudit;

			InventoryRequisitionModel model = new InventoryRequisitionModel();
			if (IsAdd)
			{
				CheckAuthority(Authority_Right.Add);
				model.CREATER = User.Identity.Name;
				model.DATA_TYPE = Function.GetSysUserDept(model.CREATER);
				model.CREATE_DATE = DateTime.Now;
				model.DETAILs = new List<InventoryRequisitionDetailModel>();
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				DATA5 d5 = iDB.GetByIDAsNoTracking<DATA5>(id);
				if (d5 != null)
				{
					model = new InventoryRequisitionModel()
					{
						ID = d5.ID,
						NODE_ID = d5.NODE_ID,
						CREATE_DATE = d5.CREATE_DATE,
						CREATER = d5.CREATER,
						DATA_TYPE = d5.DATA_TYPE,
						ORDER = d5.ORDER,
						CONTENT1 = d5.CONTENT1,
						CONTENT28 = d5.CONTENT28
					};
					model.DETAILs = d5.PLUS.Select(p => new InventoryRequisitionDetailModel()
					{
						ID = p.ID,
						MAIN_ID = p.MAIN_ID,
						PLUS_TYPE = p.PLUS_TYPE,
						DECIMAL1 = Convert.ToInt32(p.DECIMAL1 ?? 0)
					}).ToList();
					foreach (InventoryRequisitionDetailModel m in model.DETAILs)
					{
						NODE n = Function.GetNode(m.PLUS_TYPE);
						if (n != null)
						{
							NODE nn = Function.GetNode(n.PARENT_ID);
							m.PLUS_TYPE_TITLE = nn.TITLE + " > " + n.TITLE;
							m.PLUS_TYPE_UNIT = n.CONTENT1;
							m.DECIMAL2 = n.CONTENT9.ToInt();
							m.CONTENT8 = n.CONTENT8;

							ATTACHMENT att = iDB.GetAll<ATTACHMENT>(MAIN_ID: n.ID).FirstOrDefault();
							if (att != null)
							{
								m.PLUS_TYPE_IMG = att.FILE_NAME.ToMyString();
							}
						}
					}
					if ((!IsAudit && model.ORDER == 0) || model.ORDER == 1 || (IsAudit && model.ORDER == 2))
					{
						SetIsEdit(false);
					}
				}
			}
			if (print == 1)
			{
				return View("Print", model);
			}
			else
			{
				return View(model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuditStatusSelect]
		[ActionLog(TableNameIndex = 19, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, InventoryRequisitionModel model, int? page, int? defaultPage, string k, string start, string end)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
			bool IsAudit = NodeID.CheckStringValue(fun04) || NodeID.CheckStringValue(fun11_02);
			ViewBag.IsAudit = IsAudit;

			CheckAuthority(Authority_Right.Update);
			if (model.DETAILs == null || model.DETAILs.Count == 0)
			{
				SetModelStateError("請領項目：至少新增 1 項！");
				return View(model);
			}
			if (ModelState.IsValid)
			{
				string BackendUrl = string.Format("<a href=\"{0}\" target=\"_blank\">藝設中心管理後台</a>", Function.DEFAULT_ADMIN_HTTP);

				DATA5 d5 = iDB.GetByID<DATA5>(id);
				if (d5 != null) //重新申請 or 審核
				{
					StringBuilder sb = new StringBuilder();
					if (IsAudit) //審核
					{
						IQueryable<PLUS> plusList = iDB.GetAllAsNoTracking<PLUS>().Where(p => p.STATUS.Equals(INVENTORY_REQUISITION_ITEM) && p.DATA5.ORDER != 2 && p.DATA5.ENABLE == 1);
						var plusGroup = from plus in plusList
										where !plus.DATA5.ID.Equals(id)
										group plus by new { plus.PLUS_TYPE } into G
										select new
										{
											PLUS_TYPE = G.Key.PLUS_TYPE,
											R_IN = G.Sum(x => x.ORDER == 0 && x.DATA5.ORDER == 1 ? x.DECIMAL1 : 0), //實際入庫量
											R_OUT = G.Sum(x => x.ORDER == 1 && x.DATA5.ORDER == 1 ? x.DECIMAL1 : 0),  //實際出庫量
											V_IN = G.Sum(x => x.ORDER == 0 && x.DATA5.ORDER != 1 ? x.DECIMAL1 : 0), //預計入庫量
											V_OUT = G.Sum(x => x.ORDER == 1 && x.DATA5.ORDER != 1 ? x.DECIMAL1 : 0),  //預計出庫量
										};
						d5.ORDER = model.ORDER ?? AuditStatus.Type0.ToInt();
						if (model.ORDER > 0)
						{
							int iSafeNum = 0; //安全數量
							string strFormat = "在類別「{0}」下，物品「{1}」，低於安全庫存數量{2}<br />";

							//檢查庫存是否足夠
							foreach (PLUS plusX in d5.PLUS)
							{
								var gR_Num = plusGroup.FirstOrDefault(p => p.PLUS_TYPE.Equals(plusX.PLUS_TYPE));
								int iR_Num = gR_Num == null ? 0 : (Convert.ToInt32(gR_Num.R_IN ?? 0) - Convert.ToInt32(gR_Num.R_OUT ?? 0));  //實際庫存量

								NODE goods = iDB.GetByID<NODE>(plusX.PLUS_TYPE); //物品
								if (goods != null)
								{
									iSafeNum = goods.ORDER; //安全數量
									if (d5.ORDER == 2) //駁回 +
									{
										goods.CONTENT9 = iR_Num.ToString(); //剩餘庫存量
									}
									else //核准 - 
									{
										if (plusX.DECIMAL1 > iR_Num) //申請量大於庫存量,不可核准
										{
											continue;
										}
										goods.CONTENT9 = Convert.ToInt32(iR_Num - (plusX.DECIMAL1 ?? 0)).ToString(); //剩餘庫存量
									}
									plusX.DECIMAL2 = goods.CONTENT9.ToInt(); //剩餘庫存量
									if (plusX.DECIMAL2 <= iSafeNum)
									{
										sb.AppendFormat(strFormat, Function.GetNodeTitle(goods.PARENT_ID), goods.TITLE, iSafeNum);
									}
								}
							}
							if (d5.ORDER == 2)
							{
								d5.CONTENT28 = model.CONTENT28;
							}
							else
							{
								d5.CONTENT28 = null;
							}
							d5.CONTENT29 = User.Identity.Name;
							d5.DATETIME1 = DateTime.Now;
							if (iDB.Save())
							{
								if (sb.Length > 0)
								{
									SendMail2Auditor(new string[] { fun04, fun11_02 }, "物品低於安全數量通知", "您好，<br/>" + sb.ToString() + "請前往" + BackendUrl + "進行管理作業。");
									if (!Function.TIME_GAP.IsNullOrEmpty())
									{
										System.Threading.Thread.Sleep(Convert.ToInt32(Function.TIME_GAP));
									}
								}

								string Email = Function.GetSysUserEmail(d5.CREATER);
								if (!Email.IsNullOrEmpty())
								{
									string sSubject = "物品領用審核通知-請領單號" + id;
									string sBody = "您好，您提出的物品領用申請，已申請通過，請前往" + BackendUrl + "列印申請單。";
									if (d5.ORDER == 2)
									{
										sBody = "您好，您提出的物品領用申請，審核退回，原因為" + d5.CONTENT28 + "，請前往" + BackendUrl + "重新申請。";
									}
									Function.SendMail(new LetterModel { RecipientList = new List<string> { Email }, Subject = sSubject, Body = sBody });
								}
							}
						}
						UpdateNodeList();
						AlertMsg = IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE;
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "start", "end" }));
					}
					else
					{
						IsAdd = true;
					}
				}

				if (IsAdd) //新增 or 重新申請
				{
					string UserID = User.Identity.Name;
					string InventoryID = getInventoryID(id);
					d5 = new DATA5()
					{
						ID = InventoryID,
						NODE_ID = INVENTORY_REQUISITION,
						CREATE_DATE = DateTime.Now,
						CREATER = UserID,
						DATA_TYPE = Function.GetSysUserDept(UserID),
						STATUS = "2",
						ORDER = 0,
						CONTENT1 = model.CONTENT1.ToMyString(),
					};
					int iNowNum = 0;
					foreach (InventoryRequisitionDetailModel plusX in model.DETAILs)
					{
						NODE goods = iDB.GetByID<NODE>(plusX.PLUS_TYPE); //物品
						if (goods != null)
						{
							if (plusX.DECIMAL1 > goods.CONTENT9.ToInt()) //庫存量不足
							{
								continue; //不新增
							}
							iNowNum = Convert.ToInt32(goods.CONTENT9.ToInt() - plusX.DECIMAL1);
							d5.PLUS.Add(new PLUS()
							{
								ID = Function.GetGuid(),
								ENABLE = 1,
								CREATE_DATE = d5.CREATE_DATE,
								CREATER = UserID,
								MAIN_ID = InventoryID,
								PLUS_TYPE = goods.ID,
								STATUS = INVENTORY_REQUISITION_ITEM,
								ORDER = 1,
								DECIMAL1 = plusX.DECIMAL1,
								DECIMAL2 = iNowNum
							});
							goods.CONTENT9 = iNowNum.ToString();
						}
					}
					if (iDB.Add<DATA5>(d5))
					{
						SendMail2Auditor(new string[] { fun04, fun11_02 }, "物品領用申請通知-請領單號" + InventoryID,
							"您好，申請人-" + Function.GetSysUserName(d5.CREATER) + "，提出物品領用申請，請前往" + BackendUrl + "進行審核作業。");

						UpdateNodeList();
						AlertMsg = IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE;
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "start", "end" }));
					}
				}
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 19, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string start, string end, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			if (!really)
			{
				DATA5 d5 = iDB.GetByID<DATA5>(id, false);
				if (d5 != null)
				{
					//不是真的刪除時，記錄刪除人及刪除時間
					d5.CONTENT30 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
					foreach (PLUS plus in d5.PLUS)
					{
						NODE goods = iDB.GetByID<NODE>(plus.PLUS_TYPE);
						if (goods != null)
						{
							goods.CONTENT9 = Convert.ToInt32(goods.CONTENT9.ToInt() + (plus.DECIMAL1 ?? 0)).ToString(); //剩餘庫存量
						}
						plus.ENABLE = EnableType.Disable.ToByteValue();
					}
				}
			}
			if (!iDB.Delete<DATA5>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			UpdateNodeList();
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "start", "end" }));
		}
		#endregion

		#region 庫存報表＆單位物品領用統計表
		[NodeSelect(INVENTORY_ITEM)]
		[YearSelect(ORDER_BY_DESC = true, MIN_YEAR = 2011)]
		[MonthSelect]
		public ActionResult ReportIndex(int? page, int? defaultPage, string k, string k1, string k2, string start, string end, int all = 0, int export = 0)
		{
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.start = start;
			ViewBag.end = end;
			ViewBag.all = all;

			DataTable dt = new DataTable();
			List<ReportModel> model = new List<ReportModel>();
			if (NodeID.CheckStringValue(fun11_03)) //庫存報表
			{
				string SqlStr = @"SELECT t.CATEGORY, t.GOODS, t.GOODS_UNIT
, (t.prev_R_IN - t.prev_R_OUT) as LAST_MONTH_INV
, t.this_R_IN as THIS_MONTH_IN
, t.this_R_OUT as THIS_MONTH_OUT
, ((t.prev_R_IN - t.prev_R_OUT) + t.this_R_IN - t.this_R_OUT) as THIS_MONTH_INV
FROM (
	SELECT category.TITLE as CATEGORY, category.[ORDER] as CategoryOrder, goods.TITLE as GOODS, goods.CONTENT1 as GOODS_UNIT
	--計算實際與預計出入庫量(上個月)
	, SUM(CASE WHEN p.CREATE_DATE < @start AND p.[ORDER] = 0 AND d5.[ORDER] = 1 THEN p.DECIMAL1 ELSE 0 END) as prev_R_IN --實際入庫量
	, SUM(CASE WHEN p.CREATE_DATE < @start AND p.[ORDER] = 1 AND d5.[ORDER] = 1 THEN p.DECIMAL1 ELSE 0 END) as prev_R_OUT --實際出庫量
	--, SUM(CASE WHEN p.CREATE_DATE < @start AND p.[ORDER] = 0 AND d5.[ORDER] <> 1 THEN p.DECIMAL1 ELSE 0 END) as prev_V_IN --預計入庫量
	--, SUM(CASE WHEN p.CREATE_DATE < @start AND p.[ORDER] = 1 AND d5.[ORDER] <> 1 THEN p.DECIMAL1 ELSE 0 END) as prev_V_OUT --預計出庫量
	--計算實際與預計出入庫量(這個月)
	, SUM(CASE WHEN p.CREATE_DATE BETWEEN @start AND @end AND p.[ORDER] = 0 AND d5.[ORDER] = 1 THEN p.DECIMAL1 ELSE 0 END) as this_R_IN --實際入庫量
	, SUM(CASE WHEN p.CREATE_DATE BETWEEN @start AND @end AND p.[ORDER] = 1 AND d5.[ORDER] = 1 THEN p.DECIMAL1 ELSE 0 END) as this_R_OUT --實際出庫量
	--, SUM(CASE WHEN p.CREATE_DATE BETWEEN @start AND @end AND p.[ORDER] = 0 AND d5.[ORDER] <> 1 THEN p.DECIMAL1 ELSE 0 END) as this_V_IN --預計入庫量
	--, SUM(CASE WHEN p.CREATE_DATE BETWEEN @start AND @end AND p.[ORDER] = 1 AND d5.[ORDER] <> 1 THEN p.DECIMAL1 ELSE 0 END) as this_V_OUT --預計出庫量
	FROM NODE goods 
	JOIN NODE category ON goods.PARENT_ID = category.ID AND category.PARENT_ID = 'INVENTORY_ITEM'
	 AND category.[ENABLE] = 1 AND goods.[ENABLE] = 1
	LEFT JOIN PLUS p ON p.PLUS_TYPE = goods.ID AND p.[ENABLE] = 1 AND p.[STATUS] = 'INVENTORY_REQUISITION_ITEM' 
	JOIN DATA5 d5 ON p.MAIN_ID = d5.ID AND d5.[ENABLE] = 1 AND d5.[ORDER] != 2
	WHERE (@PARENT_ID = '' OR category.ID = @PARENT_ID)
	GROUP BY category.TITLE, category.[ORDER], goods.TITLE, goods.CONTENT1
) t
ORDER BY t.CategoryOrder, t.GOODS";
				using (DBEntities db = new DBEntities())
				{
					int iYear = k1.IsNullOrEmpty() ? DateTime.Now.Year : k1.ToInt();
					int iMonth = k2.IsNullOrEmpty() ? DateTime.Now.Month : k2.ToInt();
					ViewBag.k1 = iYear;
					ViewBag.k2 = iMonth;

					DateTime tmpStart = new DateTime(iYear, iMonth, 1);
					DateTime tmpEnd = tmpStart.AddMonths(1).AddDays(-1);
					dt = Function.getDataTable(db, SqlStr, new SqlParameter("PARENT_ID", k.IsNullOrEmpty() ? "" : k),
						new SqlParameter("start", tmpStart.ToDateString()), new SqlParameter("end", tmpEnd.ToDateString()));
				}

				model = dt.AsEnumerable().Select(p => new ReportModel()
				{
					CONTENT1 = p.Field<string>("CATEGORY"),
					CONTENT2 = p.Field<string>("GOODS"),
					CONTENT3 = p.Field<string>("GOODS_UNIT"),
					DECIMAL1 = p.Field<decimal?>("LAST_MONTH_INV"),
					DECIMAL2 = p.Field<decimal?>("THIS_MONTH_IN"),
					DECIMAL3 = p.Field<decimal?>("THIS_MONTH_OUT"),
					DECIMAL4 = p.Field<decimal?>("THIS_MONTH_INV")
				}).ToList();
			}
			else if (NodeID.CheckStringValue(fun11_04)) //單位物品領用統計表
			{
				DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
				DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);

				//ViewBag.DeptSelect = new SelectList(Function.GetADDepartment("展演中心"), "Value", "Text");
				ViewBag.DeptSelect = new SelectList((new List<SelectListItem>() {
					new SelectListItem { Text = "主任室", Value = "主任室" }
					, new SelectListItem { Text = "人事室", Value = "人事室" }
					, new SelectListItem { Text = "推廣組", Value = "推廣組" }
					, new SelectListItem { Text = "會計室", Value = "會計室" }
					, new SelectListItem { Text = "總務組", Value = "總務組" }
					, new SelectListItem { Text = "無部門", Value = "無部門" }
				}), "Value", "Text");

				string SqlStr = @"SELECT category.TITLE as CATEGORY, goods.TITLE as GOODS, d5.DATA_TYPE as UNIT
, CONVERT(int, SUM(p.DECIMAL1)) as DECIMAL1
FROM DATA5 d5 
JOIN PLUS p ON p.MAIN_ID = d5.ID AND d5.[ENABLE] = 1 AND p.[ENABLE] = 1
AND d5.NODE_ID = 'INVENTORY_REQUISITION' AND d5.[STATUS] = '2'
JOIN NODE goods ON p.PLUS_TYPE = goods.ID AND goods.[ENABLE] = 1
JOIN NODE category ON goods.PARENT_ID = category.ID AND category.[ENABLE] = 1 AND category.PARENT_ID = 'INVENTORY_ITEM'
WHERE (@PARENT_ID = '' OR category.ID = @PARENT_ID)
AND (@ID = '' OR goods.ID = @ID)
AND (@UNIT = '' OR d5.DATA_TYPE = @UNIT)
AND (d5.CREATE_DATE >= @start AND d5.CREATE_DATE < @end) 
GROUP BY category.TITLE, goods.TITLE, d5.DATA_TYPE
ORDER BY MAX(d5.CREATE_DATE) DESC";
				using (DBEntities db = new DBEntities())
				{
					dt = Function.getDataTable(db, SqlStr, new SqlParameter("PARENT_ID", k.IsNullOrEmpty() ? "" : k),
						new SqlParameter("ID", k1.IsNullOrEmpty() ? "" : k1),
						new SqlParameter("UNIT", k2.IsNullOrEmpty() ? "" : k2),
						new SqlParameter("start", tmpStart.ToDateString()), new SqlParameter("end", tmpEnd.ToDateString()));
				}

				model = dt.AsEnumerable().Select(p => new ReportModel()
				{
					CONTENT1 = p.Field<string>("CATEGORY"),
					CONTENT2 = p.Field<string>("GOODS"),
					CONTENT3 = p.Field<string>("UNIT"),
					DECIMAL1 = p.Field<int>("DECIMAL1")
				}).ToList();
			}
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			if (all == 1)
			{
				_defaultPage = dt.Rows.Count + 1;
			}
			page = IsPost() ? 0 : page;
			if (export == 1)
			{
				return View("ReportPrint", model.ToPagedList(page.ToMvcPaging(), _defaultPage));
			}
			else if (export == 2 || export == 3 || export == 4)
			{
				return ExportExcel(model, k, k1, k2, start, end, export);
			}
			else
			{
				return View(model.ToPagedList(page.ToMvcPaging(), _defaultPage));
			}
		}

		public ActionResult ExportExcel(List<ReportModel> model, string k, string k1, string k2, string start, string end, int export)
		{
			string sFileName = string.Empty;
			List<string> lsHeader = new List<string>();
			List<int> lsW = new List<int>();
			HSSFWorkbook workbook = new HSSFWorkbook();

			#region 樣式－欄位名稱

			//字型
			HSSFFont fontHeader = workbook.CreateFont() as HSSFFont;
			//fontHeader.FontHeightInPoints = 12;
			//fontHeader.Boldweight = (short)FontBoldWeight.BOLD; //粗體
			//fontHeader.FontName = "標楷體";

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
			//fontContent.FontHeightInPoints = 12;
			fontContent.Boldweight = (short)FontBoldWeight.BOLD; //粗體
																 //fontContent.FontName = "標楷體";

			HSSFFont fontContent2 = workbook.CreateFont() as HSSFFont;
			//fontContent2.FontHeightInPoints = 12;
			//fontContent2.FontName = "標楷體";

			//樣式-粗體字+置中
			HSSFCellStyle CenterBoldBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			CenterBoldBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			CenterBoldBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			CenterBoldBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			CenterBoldBorder.WrapText = true;
			CenterBoldBorder.SetFont(fontContent);

			//樣式-粗體字+置左
			HSSFCellStyle LeftBoldBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			LeftBoldBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
			LeftBoldBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			LeftBoldBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			LeftBoldBorder.WrapText = true;
			LeftBoldBorder.SetFont(fontContent);

			//樣式-正常字+置中
			HSSFCellStyle NormalCenterBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalCenterBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			NormalCenterBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalCenterBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalCenterBorder.WrapText = true;
			NormalCenterBorder.SetFont(fontContent2);

			//樣式-正常字+置左
			HSSFCellStyle NormalLeftBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalLeftBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
			NormalLeftBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalLeftBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalLeftBorder.WrapText = true;
			NormalLeftBorder.SetFont(fontContent2);

			//樣式-正常字+置左
			HSSFCellStyle NormalRightBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRightBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRightBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRightBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.WrapText = true;
			NormalRightBorder.SetFont(fontContent2);

			//樣式-正常字+置中
			HSSFCellStyle NormalCenter = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalCenter.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			NormalCenter.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalCenter.WrapText = true;
			NormalCenter.SetFont(fontContent2);

			//樣式-正常字+靠右
			HSSFCellStyle NormalRight = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRight.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRight.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRight.WrapText = true;
			NormalRight.SetFont(fontContent2);

			#endregion 樣式－欄位內容

			if (export == 2) //庫存報表 - 匯出EXCEL
			{
				sFileName = k1 + "年" + k2 + "月_物品盤存表";
				lsHeader = new List<string>() { "編號", "名稱", "單位", "上月盤存", "本月購進", "本月領用", "本月盤存", "備註" };
				lsW = new List<int>() { 7, 65, 12, 10, 10, 10, 10, 25 };
			}
			else if (export == 3) //庫存報表 - 匯出月報表 EXCEL
			{
				sFileName = k1 + "年" + k2 + "月_物品收發月報表";
				lsHeader = new List<string>() { "品名", "單位", "上月結存", "本月收入", "本月發出", "本月結存", "存量帳值", "備註" };
				lsW = new List<int>() { 65, 12, 10, 10, 10, 10, 25, 25 };
			}
			else if (export == 4)
			{
				sFileName = "單位物品領用統計表";
				lsHeader = new List<string>() { "類別 > 物品", "申請單位", "總申請量" };
				lsW = new List<int>() { 65, 25, 25 };
			}
			using (MemoryStream ms = new MemoryStream())
			{
				HSSFSheet sheet = workbook.CreateSheet(replaceExcelSheetName(sFileName)) as HSSFSheet;
				int iRowIndex = 4;
				HSSFRow row = sheet.CreateRow(iRowIndex) as HSSFRow;
				if (export == 2) //庫存報表 - 匯出EXCEL
				{
					int idx = 1;
					foreach (ReportModel m in model)
					{
						row = sheet.CreateRow(iRowIndex) as HSSFRow;
						row.CreateCell(0).SetCellValue(idx);
						row.CreateCell(1).SetCellValue(m.CONTENT1 + " > " + m.CONTENT2);
						row.CreateCell(2).SetCellValue(m.CONTENT3);
						row.CreateCell(3).SetCellValue(m.DECIMAL1.ToInt());
						row.CreateCell(4).SetCellValue(m.DECIMAL2.ToInt());
						row.CreateCell(5).SetCellValue(m.DECIMAL3.ToInt());
						row.CreateCell(6).SetCellValue(m.DECIMAL4.ToInt());
						row.CreateCell(7).SetCellValue("");
						idx++;
						iRowIndex++;
					}
					sheet.CreateRow(0).CreateCell(0).SetCellValue("桃園市政府藝文設施管理中心 物品盤存表");

					//表尾
					HSSFRow rowFooter = sheet.CreateRow(iRowIndex) as HSSFRow;
					rowFooter.CreateCell(0).SetCellValue("製表人");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 0, 1));

					rowFooter.CreateCell(2).SetCellValue("保管人員");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 2, 3));

					rowFooter.CreateCell(4).SetCellValue("事務主管");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 4, 5));

					rowFooter.CreateCell(6).SetCellValue("事務主管");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 6, 7));
					iRowIndex++;

					rowFooter = sheet.CreateRow(iRowIndex) as HSSFRow;
					rowFooter.CreateCell(0).SetCellValue(Function.GetSysUserName(User.Identity.Name));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 0, 1));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 2, 3));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 4, 5));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 6, 7));
				}
				else if (export == 3) //庫存報表 - 匯出月報表 EXCEL
				{
					foreach (ReportModel m in model)
					{
						row = sheet.CreateRow(iRowIndex) as HSSFRow;
						row.CreateCell(0).SetCellValue(m.CONTENT1 + " > " + m.CONTENT2);
						row.CreateCell(1).SetCellValue(m.CONTENT3);
						row.CreateCell(2).SetCellValue(m.DECIMAL1.ToInt());
						row.CreateCell(3).SetCellValue(m.DECIMAL2.ToInt());
						row.CreateCell(4).SetCellValue(m.DECIMAL3.ToInt());
						row.CreateCell(5).SetCellValue(m.DECIMAL4.ToInt());
						row.CreateCell(6).SetCellValue("");
						row.CreateCell(7).SetCellValue("");
						iRowIndex++;
					}
					sheet.CreateRow(0).CreateCell(0).SetCellValue("桃園市政府藝文設施管理中心 物品收發月報表");

					//表尾
					HSSFRow rowFooter = sheet.CreateRow(iRowIndex) as HSSFRow;
					rowFooter.CreateCell(0).SetCellValue("製表或保管人");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 0, 1));

					rowFooter.CreateCell(2).SetCellValue("複核");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 2, 4));

					rowFooter.CreateCell(5).SetCellValue("事務主管");
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 5, 7));
					iRowIndex++;

					rowFooter = sheet.CreateRow(iRowIndex) as HSSFRow;
					rowFooter.CreateCell(0).SetCellValue(Function.GetSysUserName(User.Identity.Name));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 0, 1));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 2, 4));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex + 3, 5, 7));
				}
				else if (export == 4) //庫存報表 - 匯出EXCEL
				{
					foreach (ReportModel m in model)
					{
						row = sheet.CreateRow(iRowIndex) as HSSFRow;
						row.CreateCell(0).SetCellValue(m.CONTENT1 + " > " + m.CONTENT2);
						row.CreateCell(1).SetCellValue(m.CONTENT3);
						row.CreateCell(2).SetCellValue(m.DECIMAL1.ToInt());
						iRowIndex++;
					}
					sheet.CreateRow(0).CreateCell(0).SetCellValue("桃園市政府藝文設施管理中心 單位物品領用統計表");
				}
				if (export == 2 || export == 3)
				{
					sheet.CreateRow(1).CreateCell(0).SetCellValue("統計時間：" + k1 + "年" + k2 + "月");
				}
				sheet.CreateRow(2).CreateCell(0).SetCellValue("填表時間：" + DateTime.Now.ToString("yyyy年MM月dd日"));

				int iTotalCols = lsHeader.Count;
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, iTotalCols - 1));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, iTotalCols - 1));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2, 2, 0, iTotalCols - 1));

				//表頭
				HSSFRow rowHeader = sheet.CreateRow(3) as HSSFRow;
				for (int i = 0; i < lsHeader.Count; i++)
				{
					rowHeader.CreateCell(i).SetCellValue(lsHeader[i]);
				}

				//套用格式
				int iLastRowNum = sheet.LastRowNum + (export == 4 ? 1 : 4);
				for (int i = 0; i < iLastRowNum; i++)
				{
					HSSFRow rowX = sheet.GetRow(i) as HSSFRow;
					if (rowX == null)
						rowX = sheet.CreateRow(i) as HSSFRow;
					for (int j = 0; j < lsHeader.Count; j++)
					{
						HSSFCell cell = rowX.GetCell(j) as HSSFCell;
						if (cell == null)
							cell = rowX.CreateCell(j) as HSSFCell;
						if (i >= 0 && i < 3)
						{
							cell.CellStyle = i == 0 ? NormalCenter : NormalRight;
						}
						else
						{
							cell.CellStyle = i == 3 ? styleHeader : NormalCenterBorder;
						}
					}
				}

				for (int j = 0; j < lsW.Count; j++)
				{
					sheet.SetColumnWidth(j, lsW[j] * 256);
				}

				#region 查詢條件
				HSSFSheet sheetLimit = workbook.CreateSheet("查詢條件") as HSSFSheet;
				HSSFRow rowLimit = sheetLimit.CreateRow(0) as HSSFRow;
				rowLimit.CreateCell(0).SetCellValue("類別");
				rowLimit.CreateCell(1).SetCellValue(k.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k));
				if (export == 4)
				{
					rowLimit = sheetLimit.CreateRow(1) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("物品");
					rowLimit.CreateCell(1).SetCellValue(k1.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k1));

					rowLimit = sheetLimit.CreateRow(2) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("申請單位");
					rowLimit.CreateCell(1).SetCellValue(k2.IsNullOrEmpty() ? "全部" : k2);

					rowLimit = sheetLimit.CreateRow(3) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("日期");
					rowLimit.CreateCell(1).SetCellValue(start + "~" + end);
				}
				else
				{
					rowLimit = sheetLimit.CreateRow(1) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("年份");
					rowLimit.CreateCell(1).SetCellValue(k1);

					rowLimit = sheetLimit.CreateRow(2) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("月份");
					rowLimit.CreateCell(1).SetCellValue(k2);
				}
				sheetLimit.AutoSizeColumn(0);
				sheetLimit.AutoSizeColumn(1);
				#endregion

				workbook.Write(ms);
				workbook = null;
				return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + sFileName + ".xls");
			}
		}
		#endregion

		string replaceExcelSheetName(string sFileName)
		{
			if (!sFileName.IsNullOrEmpty())
			{
				sFileName = sFileName.Replace("\\", "_");
				sFileName = sFileName.Replace("/", "_");
				sFileName = sFileName.Replace("?", "_");
				sFileName = sFileName.Replace("*", "_");
				sFileName = sFileName.Replace("[", "_");
				sFileName = sFileName.Replace("\\", "]");
				if (sFileName.Length >= 30)
				{
					sFileName = sFileName.Substring(0, 30);
				}
			}
			return sFileName;
		}
	}
}