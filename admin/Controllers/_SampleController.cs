using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace admin.Controllers
{
	/// <summary>
	/// 範例Controller
	/// </summary>
	public class _SampleController : BaseController
	{

		#region const property

		#endregion

		#region 建構式

		#endregion

		#region 基本資料 列表 編輯 刪除

		public ActionResult Index(int? page, int? defaultPage, string k)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			//post 與 get 合併action 處理
			page = IsPost() ? 0 : page;
			return View(GetData(k).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		//除非有必要再另外處理post行為
		//[HttpPost]
		//public ActionResult Index(int? defaultPage, string k)
		//{
		//    int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
		//    return View(GetData(k).ToPagedList(0, _defaultPage));
		//}

		public ActionResult Edit(int? page, int? defaultPage, string k, string id)
		{
			IsContinue = true;//測試用的
			IsAdd = id.IsNullOrEmpty();
			if (IsAdd)//新增
			{
				CheckAuthority(Authority_Right.Add);
			}
			else//編輯
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
			}
			DemoModel model = new DemoModel();
			DATA1 d1 = iDB.GetByID<DATA1>(id);
			if (d1 != null)
			{
				model.CONTENT = d1.CONTENT1;
				model.ATT = d1.ID;
				model.DATE = d1.DATETIME1;
				model.DATETIME = d1.DATETIME2;
				model.DECIMAL = d1.DECIMAL1;
				model.INT = d1.DECIMAL1.ToInt();
				model.AttList = d1.ATTACHMENT.OrderBy(p => p.CREATE_DATE).ToList();
			}
			else
			{
				model.ATT = Function.GetGuid();
			}
			SetFileUploadParamter(Function.DEFAULT_FILEUPLOAD_PICTURE_EXT);
			return View(model);
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 15)]
		public ActionResult Edit(int? page, int? defaultPage, string k, string id, DemoModel model)
		{
			IsContinue = true;
			IsAdd = id.IsNullOrEmpty();
			CheckAuthority(IsAdd ? Authority_Right.Add : Authority_Right.Update);
			if (ModelState.IsValid)
			{
				DATA1 d1 = null;
				if (IsAdd)//新增
				{
					d1 = new DATA1();
					d1.ID = model.ATT;//因為有上傳套件，所以ID已經先給了
					d1.NODE_ID = "_sample";//看功能給值
					d1.CREATER = User.Identity.Name;
				}
				else
				{
					d1 = iDB.GetByID<DATA1>(id);
				}
				if (d1 != null)
				{
					d1.UPDATE_DATE = DateTime.Now;
					d1.UPDATER = User.Identity.Name;
				}
				d1.CONTENT1 = model.CONTENT;
				d1.DATETIME1 = model.DATE;
				d1.DATETIME2 = model.DATETIME;
				d1.DECIMAL1 = model.DECIMAL;
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<DATA1>(d1);
				}
				else
				{
					iDB.Save();
					IsSuccessful = true;
				}
				if (IsSuccessful)
				{
					//SendNotice();
					AlertMsg = IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE;
					return GoIndex(NodeID, page, defaultPage, k);
				}

			}
			else
			{
				SaveModelValidMSG(ModelState);
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 15, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			AlertMsg = iDB.Delete<DATA1>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
			return GoIndex(NodeID, page, defaultPage, k);
		}

		[ActionLog(TableNameIndex = 3, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteAtt(string id, int? page, int? defaultPage, string k, string aid)
		{
			AlertMsg = iDB.Delete<ATTACHMENT>(id) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "aid" }), "Edit");
		}

		/// <summary>
		/// GetData
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		IQueryable<DATA1> GetData(string k)
		{
			return iDB.GetAll<DATA1>().Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k)));
		}

		#endregion

		#region 子功能共用Function

		/// <summary>
		/// 儲存變更排序
		/// </summary>
		/// <param name="article1"></param>
		/// <param name="article2"></param>
		void SavePlusSort(string article1, string article2)
		{
			if (article1.IsTrue() && !article2.IsNullOrEmpty())
			{
				int _sort = 1;
				foreach (string aid in article2.ToSplit())
				{
					PLUS plus = iDB.GetByID<PLUS>(aid);
					if (plus != null)
					{
						plus.ORDER = _sort;
						_sort++;
					}
				}
				if (iDB.Save())
				{
					Msgbox_Toast("排序更新成功！！");
				}
			}
		}

		/// <summary>
		/// 寄信
		/// </summary>
		/// <param name="type"></param>
		void SendNotice()
		{
			string _themeParkID = "";
			if (!_themeParkID.IsNullOrEmpty())
			{
				string _body = string.Format("業者：{1}{0}帳號：{2}{0}異動單元：{3}{0}異動日期時間：{4}"
					, "<br /><br />"
					, iDB.GetByID<DATA1>(_themeParkID).CONTENT1
					, User.Identity.Name
					, ""
					, DateTime.Now);
				Function.SendMail(new LetterModel
				{
					RecipientList = Function.GetConfigSetting("notice").ToSplit().ToList(),
					Subject = "通知",
					Body = _body
				});
			}
		}

		#endregion

		#region excel 資料匯入、匯出

		public ActionResult Import()
		{
			return View();
		}
		/// <summary>
		/// use NPOI
		/// </summary>
		/// <param name="excel"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Import(HttpPostedFileBase excel)
		{
			if (excel != null)
			{
				if (excel.ContentLength > 0)
				{
					//iData1.ExecuteSqlCommand("DELETE from DATA1");
					//iData1.ExecuteSqlCommand("DELETE from PARAGRAPH where custom3='dannyImport'");
					string mark = "dannyImport";
					HSSFWorkbook book = new HSSFWorkbook(excel.InputStream);
					ISheet sheet = book.GetSheetAt(0);
					for (int i = 1; i <= 24; i++)
					{
						IRow row = sheet.GetRow(i);
						string _id = row.Cells[0].ToMyString();
						DATA1 d1 = iDB.GetByID<DATA1>(_id);
						if (d1 != null)
						{
							//d1.ID = Function.GetGuid();
							//d1.NODE_ID = "themepark";
							//d1.CREATER = User.Identity.Name;
							d1.UPDATE_DATE = DateTime.Now;
							d1.UPDATER = User.Identity.Name;

							//string _c28 = "0";
							//if ("科學".Equals(sheet.GetRow(8).Cells[i].ToMyString()))
							//    _c28 = "1";
							//if ("生態".Equals(sheet.GetRow(8).Cells[i].ToMyString()))
							//    _c28 = "0";
							//if ("文化".Equals(sheet.GetRow(8).Cells[i].ToMyString()))
							//    _c28 = "2";
							//d1.CONTENT28 = _c28;
							d1.CONTENT29 = row.Cells[1].ToMyString();
							d1.CONTENT30 = row.Cells[2].ToMyString();
							d1.CONTENT1 = row.Cells[3].ToMyString();
							d1.CONTENT2 = row.Cells[4].ToMyString();
							d1.CONTENT3 = row.Cells[5].ToMyString();
							d1.CONTENT4 = row.Cells[6].ToMyString();
							d1.CONTENT5 = row.Cells[7].ToMyString();
							d1.CONTENT6 = row.Cells[8].ToMyString();
							d1.CONTENT7 = row.Cells[9].ToMyString();
							d1.CONTENT8 = row.Cells[10].ToMyString();
							d1.CONTENT9 = row.Cells[15].ToMyString();
							d1.CONTENT10 = row.Cells[16].ToMyString();
							d1.CONTENT11 = row.Cells[17].ToMyString();
							d1.CONTENT12 = row.Cells[18].ToMyString();
							d1.CONTENT13 = row.Cells[19].ToMyString();
							d1.CONTENT14 = row.Cells[20].ToMyString();
							d1.CONTENT15 = row.Cells[21].ToMyString();
							d1.CONTENT16 = row.Cells[22].ToMyString();
							d1.CONTENT17 = row.Cells[11].ToMyString();
							d1.CONTENT18 = row.Cells[12].ToMyString();
							d1.CONTENT19 = row.Cells[13].ToMyString();
							d1.CONTENT20 = row.Cells[14].ToMyString();

							//入園時間
							PARAGRAPH p1 = d1.GetParagraphByOrder();
							if (p1 == null)
							{
								p1 = new PARAGRAPH();
								p1.ID = Function.GetGuid();
								p1.CREATE_DATE = DateTime.Now;
								p1.CREATER = User.Identity.Name;
								p1.ORDER = 1;
								p1.CONTENT3 = mark;
								d1.PARAGRAPH.Add(p1);
							}
							p1.CONTENT = row.Cells[23].ToMyString();

						}

					}
					iDB.Save();
				}
			}
			Msgbox_Toast("ok");
			return View();
		}

		/// <summary>
		/// 匯出 NPOI
		/// </summary>
		/// <returns></returns>
		public ActionResult Export()
		{
			//取得資料
			HSSFWorkbook workbook = new HSSFWorkbook();

			//內容格式設定
			HSSFCellStyle cstyle = workbook.CreateCellStyle() as HSSFCellStyle;
			cstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			cstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			cstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			cstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			cstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			cstyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			cstyle.WrapText = true;

			//置中
			HSSFCellStyle fontStyle_CENTER = workbook.CreateCellStyle() as HSSFCellStyle;
			fontStyle_CENTER.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
			fontStyle_CENTER.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;


			//靠右
			HSSFCellStyle fontStyle_RIGHT = workbook.CreateCellStyle() as HSSFCellStyle;
			fontStyle_RIGHT.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			fontStyle_RIGHT.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;

			string REPORT_NAME = "xxxxxxx";

			MemoryStream ms = new MemoryStream();
			HSSFSheet sheet = workbook.CreateSheet(REPORT_NAME) as HSSFSheet;

			int _columnCount = 7;
			int _number = 1;//流水號
			int _rowCount = 1;//從第2行開始

			HSSFRow rowheader = sheet.CreateRow(_rowCount - 1) as HSSFRow;
			rowheader.CreateCell(0).SetCellValue("流水號");
			rowheader.CreateCell(1).SetCellValue("志工姓名");
			rowheader.CreateCell(2).SetCellValue("班別");
			rowheader.CreateCell(3).SetCellValue("服勤項目");
			rowheader.CreateCell(4).SetCellValue("服務起訖");
			rowheader.CreateCell(5).SetCellValue("受服務人次");
			rowheader.CreateCell(6).SetCellValue("服務時數");


			int j = 0;
			List<ARTICLE> list = new List<ARTICLE>();
			foreach (ARTICLE v in list)
			{
				HSSFRow _row = sheet.CreateRow(_rowCount) as HSSFRow;
				_row.CreateCell(0).SetCellValue(_number);//流水號
				_row.CreateCell(1).SetCellValue(iDB.GetByID<SYSUSER>(v.CONTENT1).NAME);//姓名
				_row.CreateCell(2).SetCellValue("");//班別
				_row.CreateCell(3).SetCellValue(iDB.GetByID<ARTICLE>(v.ID).CONTENT1); //服務項目
				_row.CreateCell(4).SetCellValue(v.DATETIME1.ToDefaultString() + "-" + v.DATETIME2.ToDefaultString());//服務起訖
				_row.CreateCell(5).SetCellValue(v.CONTENT5);//受服務人次
				_row.CreateCell(6).SetCellValue(v.CONTENT6 + "小時");//服務時數
				j++;

				for (int i = 0; i < _columnCount; i++)
				{
					if (_row.GetCell(i) != null)
					{
						_row.GetCell(i).CellStyle = cstyle;
					}
					else if (_row.GetCell(i) == null)
					{
						_row.CreateCell(i).SetCellValue("");
						_row.GetCell(i).CellStyle = cstyle;
					}
					sheet.SetColumnWidth(i, 15 * 256);
				}
				_rowCount++;
				_number++;
			}

			workbook.Write(ms);
			workbook = null;
			return File(ms.ToArray(), Function.APPLICATION_EXCEL, string.Format("{0}-{1}.xls", DateTime.Now.ToString("yyyyMMddhhmm"), REPORT_NAME));
		}
		#endregion

		#region Google Chart

		public ActionResult Chart()
		{
			return View();
		}

		#endregion

	}
}
