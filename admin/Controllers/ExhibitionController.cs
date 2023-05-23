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
	public class ExhibitionController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
		/// <summary>
		/// 前台網站管理 > 藝文活動＞展演分類
		/// </summary>
		public const string fun13_05_01 = "fun13_05_01";
		/// <summary>
		/// 展演分類
		/// </summary>
		public const string PERFORMANCE_TYPE = "performancetype";
		/// <summary>
		/// 前台網站管理 > 藝文活動＞展演場地
		/// </summary>
		public const string fun13_05_02 = "fun13_05_02";
		/// <summary>
		/// 館別
		/// </summary>
		public const string HALL_TYPE = "halltype";
		/// <summary>
		/// 前台網站管理 > 藝文活動＞檔期管理
		/// </summary>
		public const string fun13_05_03 = "fun13_05_03";
		/// <summary>
		/// 前台網站管理 > 藝文活動＞檔期統計
		/// </summary>
		public const string fun13_05_04 = "fun13_05_04";
		/// <summary>
		/// 前台網站管理 > 便民服務＞索票資訊
		/// </summary>
		public const string fun13_06_01 = "fun13_06_01";
		/// <summary>
		///  前台網站管理 > 便民服務＞身心障礙索票
		/// </summary>
		public const string fun13_06_03 = "fun13_06_03";
		/// <summary>
		/// 檔期管理>時間
		/// </summary>
		public const string PLUS_TYPE_TIME = "TIME";
		/// <summary>
		/// 便民服務>身心障礙索票
		/// </summary>
		public const string PLUS_TYPE_DISABILITY = "DISABILITY";

		#region 展演分類&展演場地
		/// <summary>
		/// 列表
		/// </summary>
		[NodeSelect(HALL_TYPE, PERFORMANCE_TYPE)]
		public ActionResult CategoryIndex(int? page, int? defaultPage, string k)
		{
			if (k.IsNullOrEmpty())
			{
				k = string.Join(";", ((NodeID.CheckStringValue(fun13_05_01) ? ViewBag.performanceType : ViewBag.hallType) as SelectList)
					.Select(p => p.Value).ToArray());
			}
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAllAsNoTracking<NODE>()
				.Where(p => (string.IsNullOrEmpty(k) || p.PARENT_ID.Contains(k) || k.Contains(p.PARENT_ID))).OrderBy(p => p.PARENT_ID).ThenBy(p => p.CREATE_DATE)
				.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[NodeSelect(HALL_TYPE, PERFORMANCE_TYPE)]
		public ActionResult CategoryEdit(string id, int? page, int? defaultPage, string k)
		{
			ExhibitionCategoryModel model = new ExhibitionCategoryModel();
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
					model = new ExhibitionCategoryModel()
					{
						ID = n.ID,
						PARENT_ID = n.PARENT_ID,
						TITLE = n.TITLE,
						CONTENT1 = n.CONTENT1
					};
				}
			}
			return View(model);
		}

		[NodeSelect(HALL_TYPE, PERFORMANCE_TYPE)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult CategoryEdit(string id, ExhibitionCategoryModel model, int? page, int? defaultPage, string k)
		{
			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				NODE n = iDB.GetByID<NODE>(id);
				if (n == null)
				{
					n = new NODE()
					{
						ID = Function.GetGuid(),
						CREATER = User.Identity.Name
					};
				}
				else
				{
					n.UPDATER = User.Identity.Name;
					n.UPDATE_DATE = DateTime.Now;
				}
				n.PARENT_ID = model.PARENT_ID;
				n.TITLE = model.TITLE;
				n.CONTENT1 = model.CONTENT1.ToMyString().IndexOf(";") != -1 ? "0" : model.CONTENT1;

				if (IsAdd)
				{
					IsSuccessful = iDB.Add<NODE>(n);
					AlertMsg = Function.DEFAULT_ADD_MESSAGE;
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
					AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
				}
				if (IsSuccessful)
				{
					UpdateNodeList();
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), "CategoryIndex");
				}
			}
			SetModelStateError();
			return View(model);
		}
		#endregion

		#region 檔期管理
		/// <summary>
		/// 列表
		/// </summary>
		[NodeSelect(HALL_TYPE)]
		public ActionResult Index(int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string c1, string c2, bool bCalendar = false)
		{
			//演出館別
			k1 = k1.IsNullOrEmpty() ? "hall1" : k1; //預設:桃園展演中心

			//演出場地
			List<SelectListItem> hall1 = GetSelectListItem(k1, k2);
			hall1.Insert(0, new SelectListItem() { Value = "all", Text = "全部" });
			ViewBag.hall1 = new SelectList(hall1, "Value", "Text");

			//展演類型
			List<SelectListItem> performanceType = GetSelectListItem(PERFORMANCE_TYPE, k3);
			performanceType.Insert(0, new SelectListItem() { Value = "all", Text = "全部" });
			ViewBag.performanceType = new SelectList(performanceType, "Value", "Text");

			//活動類型
			List<SelectListItem> performance1 = GetSelectListItem(k3, k4);
			performance1.Insert(0, new SelectListItem() { Value = "all", Text = "全部" });
			ViewBag.performance1 = new SelectList(performance1, "Value", "Text");

			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;
			ViewBag.k4 = k4;
			if (IsPost())
			{
				c1 = Request.Form["c1"];
				c2 = Request.Form["c2"];
			}
			else
			{
				c1 = Request.QueryString["c1"];
				c2 = Request.QueryString["c2"];
			}
			ViewBag.c1 = c1;
			ViewBag.c2 = c2;
			int iC1 = c1.ToInt();
			int iC2 = c2.ToInt();

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: NodeID)
				.Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k) || p.CONTENT7.Contains(k)) &&
				(string.IsNullOrEmpty(k1) || p.CONTENT3.Contains(k1)) &&
				(string.IsNullOrEmpty(k2) || k2.Equals("all") || p.CONTENT4.Contains(k2)) &&
				(string.IsNullOrEmpty(k3) || k3.Equals("all") || p.CONTENT5.Contains(k3)) &&
				(string.IsNullOrEmpty(k4) || k4.Equals("all") || p.CONTENT6.Contains(k4)) &&
				(iC1 == 0 || p.DECIMAL2.Value == iC1) &&
				(iC2 == 0 || p.DECIMAL7.Value == iC2))
				.OrderByDescending(p => p.DATETIME9).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[NodeSelect(HALL_TYPE)]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string c1, string c2, bool? bCalendar)
		{
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;
			ViewBag.k4 = k4;
			ViewBag.c1 = c1;
			ViewBag.c2 = c2;

			ExhibitionModel model = new ExhibitionModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				model.CONTENT2 = "1";
				model.CONTENT3 = "hall1";
				model.CONTENT5 = "performance1";
				model.CONTENT15 = AdmissionType.freeTicket.ToString();
				model.TIMEs = new List<TimeModel>();
				model.PICs = new List<ATTACHMENT>();

				//20200921 活動聯絡資料預設顯示AD的聯絡人、電話、郵件
				SYSUSER user = Function.SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(User.Identity.Name));
				if (user != null)
				{
					model.CONTENT8 = user.NAME;
					model.CONTENT9 = user.CONTENT5;//TELEPHONE
					model.CONTENT10 = user.CONTENT4; //MOBILE
					model.CONTENT11 = user.EMAIL;
				}
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				DATA2 d2 = iDB.GetByIDAsNoTracking<DATA2>(id);
				if (d2 != null)
				{
					model = new ExhibitionModel()
					{
						ID = d2.ID,
						CONTENT1 = d2.CONTENT1,
						CONTENT2 = d2.CONTENT2,
						CONTENT3 = d2.CONTENT3,
						CONTENT4 = d2.CONTENT4,
						CONTENT5 = d2.CONTENT5,
						CONTENT6 = d2.CONTENT6,
						CONTENT7 = d2.CONTENT7,
						CONTENT8 = d2.CONTENT8,
						CONTENT9 = d2.CONTENT9,
						CONTENT10 = d2.CONTENT10,
						CONTENT11 = d2.CONTENT11,
						CONTENT12 = d2.CONTENT12,
						CONTENT13 = d2.CONTENT13,
						CONTENT14 = d2.CONTENT14,
						CONTENT16 = d2.CONTENT16,
						DATETIME1 = d2.DATETIME1,
						DECIMAL1 = Convert.ToBoolean(d2.DECIMAL1),
						DECIMAL2 = Convert.ToBoolean(d2.DECIMAL2),
						DECIMAL7 = Convert.ToBoolean(d2.DECIMAL7),
						DECIMAL3 = Convert.ToBoolean(d2.DECIMAL3),
						PICs = d2.ATTACHMENT.OrderBy(p => p.ORDER).ToList(),
					};
					if (!d2.CONTENT15.IsNullOrEmpty())
					{
						string[] arrC15 = d2.CONTENT15.Split(Function.DELIMITER);
						model.CONTENT15 = arrC15[0];
						model.CONTENT15_OTHER = arrC15[1];
					}
					if (d2.DECIMAL4.HasValue)
					{
						model.DECIMAL4 = Convert.ToInt16(d2.DECIMAL4);
					}
					if (d2.DECIMAL5.HasValue)
					{
						model.DECIMAL5 = Convert.ToInt16(d2.DECIMAL5);
					}
					model.MAX_CONTENT = new List<string>();
					for (int i = 0; i < 3; i++)
					{
						PARAGRAPH par = d2.PARAGRAPH.FirstOrDefault(p => p.ORDER == i);
						model.MAX_CONTENT.Add(par == null ? string.Empty : par.CONTENT);
					}
					model.TIMEs = d2.PLUS.Where(p => p.PLUS_TYPE.Equals(PLUS_TYPE_TIME))
							.Select(p => new TimeModel
							{
								ID = p.ID,
								MAIN_ID = p.MAIN_ID,
								ORDER = p.ORDER,
								DATETIME1 = p.DATETIME1.Value,
								DATETIME2 = p.DATETIME2.Value,
								STATUS = p.getDateTime1AndDateTime2(2, week: true) /*僅顯示用*/
							}).OrderBy(p => p.DATETIME1).ToList();
				}
			}
			ViewBag.hallSelect = new SelectList(GetSelectListItem(model.CONTENT3, model.CONTENT4), "Value", "Text"); // 演出場地
			ViewBag.performanceType = new SelectList(GetSelectListItem(PERFORMANCE_TYPE, model.CONTENT5), "Value", "Text"); //展演類型
			ViewBag.performanceSelect = new SelectList(GetSelectListItem(model.CONTENT5, model.CONTENT6), "Value", "Text"); //活動類型
			return View(model);
		}

		[NodeSelect(HALL_TYPE)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 16, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, ExhibitionModel model, int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string c1, string c2, bool? bCalendar, string lsDel, string lsID
            , List<string> croppieFileUpload
            , List<string> croppieFileUpload_orginalFileName)
		{
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;
			ViewBag.k4 = k4;
			ViewBag.c1 = c1;
			ViewBag.c2 = c2;

			CheckAuthority(Authority_Right.Update);
			IsSuccessful = true;
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				DATA2 d2 = iDB.GetByID<DATA2>(id);
				if (d2 == null)
				{
					d2 = new DATA2()
					{
						ID = Function.GetGuid(),
						NODE_ID = NodeID,
						CREATER = User.Identity.Name
					};
				}
				else
				{
					d2.UPDATER = User.Identity.Name;
					d2.UPDATE_DATE = DateTime.Now;
				}
				d2.CONTENT1 = model.CONTENT1;
				d2.CONTENT2 = model.CONTENT2;
				d2.CONTENT3 = model.CONTENT3;
				d2.CONTENT4 = model.CONTENT4;
				d2.CONTENT5 = model.CONTENT5;
				d2.CONTENT6 = model.CONTENT6;
				d2.CONTENT7 = model.CONTENT7;
				d2.CONTENT8 = model.CONTENT8;
				d2.CONTENT9 = model.CONTENT9;
				d2.CONTENT10 = model.CONTENT10;
				d2.CONTENT11 = model.CONTENT11;
				d2.CONTENT12 = model.CONTENT12;
				d2.CONTENT13 = model.CONTENT13;
				d2.CONTENT14 = model.CONTENT14.ToHttpUrl();
				d2.CONTENT15 = model.CONTENT15 + Function.DELIMITER + model.CONTENT15_OTHER.ToMyString(); ;
				d2.CONTENT16 = model.CONTENT16.ToHttpUrl();
				d2.DECIMAL1 = model.DECIMAL1 ? 1 : 0;
				d2.DECIMAL2 = model.DECIMAL2 ? 1 : 0;
				d2.DECIMAL7 = model.DECIMAL7 ? 1 : 0;
				d2.DECIMAL3 = model.DECIMAL3 ? 1 : 0;
				d2.DECIMAL4 = model.DECIMAL4;
				d2.DECIMAL5 = model.DECIMAL5;
				d2.DATETIME1 = model.DATETIME1;

				ViewBag.hallSelect = new SelectList(GetSelectListItem(model.CONTENT3, model.CONTENT4), "Value", "Text"); // 演出場地
				ViewBag.performanceType = new SelectList(GetSelectListItem(PERFORMANCE_TYPE, model.CONTENT5), "Value", "Text"); //展演類型
				ViewBag.performanceSelect = new SelectList(GetSelectListItem(model.CONTENT5, model.CONTENT6), "Value", "Text"); //活動類型

				for (int i = 0; i < model.MAX_CONTENT.Count; i++)
				{
					PARAGRAPH par = d2.PARAGRAPH.FirstOrDefault(p => p.ORDER == i);
					if (par == null)
					{
						par = new PARAGRAPH()
						{
							ID = Function.GetGuid(),
							ORDER = i,
							CREATE_DATE = DateTime.Now,
							CREATER = User.Identity.Name
						};
						d2.PARAGRAPH.Add(par);
					}
					else
					{
						par.UPDATE_DATE = DateTime.Now;
						par.UPDATER = User.Identity.Name;
					}
					par.CONTENT = model.MAX_CONTENT[i].ToMyString();
				}

				#region 時間
				if (!lsDel.IsNullOrEmpty())
				{
					if (d2.PLUS != null && d2.PLUS.Any()) //刪除
					{
						foreach (string plusID in d2.PLUS.Where(x => lsDel.Contains(x.ID)).Select(x => x.ID).ToList())
						{
							iDB.Delete<PLUS>(plusID, true);
						}
					}
				}

				if (model.TIMEs.Any())
				{
					string lsPLUS = string.Join(";", d2.PLUS.Select(p => p.ID).ToArray());
					foreach (TimeModel time in model.TIMEs.Where(x => !lsPLUS.Contains(x.ID)))
					{
						d2.PLUS.Add(new PLUS()
						{
							ID = Function.GetGuid(),
							CREATE_DATE = DateTime.Now,
							CREATER = User.Identity.Name,
							PLUS_TYPE = PLUS_TYPE_TIME,
							MAIN_ID = d2.ID,
							ENABLE = EnableType.Enable.ToByteValue(),
							ORDER = time.ORDER,
							DATETIME1 = time.DATETIME1,
							DATETIME2 = time.DATETIME2
						});
					}
				}
				#endregion

				#region 圖片上傳 fix by danny 2020-12-17
				List<HttpPostedFileBase> HPFs = model.HPFs;
                if (model.HPFs != null && model.HPFs.Count > 0)
                {
                    foreach (HttpPostedFileBase hpf in HPFs)
                    {
                        if (hpf != null && hpf.ContentLength > 0)
                        {
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
                                att.EXTENSION = ".png";
                                att.SetUpFileName();
                                att.CREATER = User.Identity.Name;
                                att.ORDER = 0;
                                att.CONTENT9 = EnableType.Enable.ToIntValue();
                                d2.ATTACHMENT.Add(att);
                                SaveAttachmentFromCroppie(croppieFileUpload[HPFs.IndexOf(hpf)], att.FILE_NAME);
                                //SaveAtt(hpf, att.FILE_NAME);
                            }
                        }
                    }
                }
				#endregion

				if (IsAdd)
				{
					IsSuccessful = iDB.Add<DATA2>(d2);
				}
				else
				{
					IsSuccessful = true;

					#region 排序
					if (!lsID.IsNullOrEmpty()) //改變排序
					{
						List<string> IDs = lsID.Split(';').ToList();
						foreach (ATTACHMENT att in d2.ATTACHMENT)
						{
							att.ORDER = IDs.IndexOf(att.ID) + 1;
						}
					}
					#endregion

					iDB.Save();
				}
				if (IsSuccessful)
				{
					//修改 DATA2 的 DATETIME9 & DATETIME10
					UpdatePlusDT9AndDT10("DATA2", "fun13_05_03", d2.ID, 1);

					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "k4", "c1", "c2" }));
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}

		/// <summary>
		/// 檔期統計
		/// </summary>
		/// <param name="k"></param>
		/// <param name="k1">演出館別</param>
		/// <param name="k2">演出場地</param>
		/// <param name="k3">展演類型</param>
		/// <param name="k4">活動類型</param>
		/// <param name="start">開始日期</param>
		/// <param name="end">結束日期</param>
		/// <returns></returns>
		[NodeSelect(HALL_TYPE)]
		public ActionResult ReportIndex(string k, string k1, string k2, string k3, string k4, string start, string end)
		{
			//演出館別
			k1 = k1.IsNullOrEmpty() ? "hall1" : k1; //預設:桃園展演中心

			//演出場地
			List<SelectListItem> hall1 = GetSelectListItem(k1, k2);
			hall1.Insert(0, new SelectListItem() { Value = "", Text = "全部" });
			ViewBag.hall1 = new SelectList(hall1, "Value", "Text");

			//展演類型
			List<SelectListItem> performanceType = GetSelectListItem(PERFORMANCE_TYPE, k3);
			//performanceType.Insert(0, new SelectListItem() { Value = "", Text = "全部" });
			ViewBag.performanceType = new SelectList(performanceType, "Value", "Text");

			//活動類型
			List<SelectListItem> performance1 = GetSelectListItem(k3, k4);
			performance1.Insert(0, new SelectListItem() { Value = "", Text = "全部" });
			ViewBag.performance1 = new SelectList(performance1, "Value", "Text");

			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;
			ViewBag.k4 = k4;
			ViewBag.start = start;
			ViewBag.end = end;

			DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
			DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);

			if (IsPost())
			{
				string SqlStr = @"SELECT
d2.ID
, d2.CONTENT6 as 類別
, ST as 日期
, d2.CONTENT15 as 入場方式
, d2.CONTENT4 as 地點
, d2.CONTENT7 as 活動名稱, d2.CONTENT1 as 申請單位
, CONVERT(int,d2.DECIMAL4) as 人次
, (SELECT SUM(DATEDIFF(day,CONVERT(varchar(10),pp.DATETIME1,111), CONVERT(varchar(10),pp.[DATETIME2],111)) + 1) 
FROM PLUS pp WHERE pp.MAIN_ID = d2.ID AND pp.PLUS_TYPE = 'TIME' AND pp.[ORDER] = 0) as 裝臺天數
, 1 as 演出天數
FROM DATA2 d2
JOIN PLUS p ON p.MAIN_ID = d2.ID AND p.PLUS_TYPE = 'TIME' AND p.[ORDER] = 1 AND p.[ENABLE] = 1
CROSS APPLY dbo.fnSplitDate2Table(p.ID, p.DATETIME1, p.[DATETIME2])
WHERE d2.NODE_ID = 'fun13_05_03' AND d2.[ENABLE] = 1
AND (@C3 = '' OR d2.CONTENT3 = @C3)
AND (@C4 = '' OR d2.CONTENT4 = @C4)
AND (@C5 = '' OR d2.CONTENT5 = @C5)
AND (@C6 = '' OR d2.CONTENT6 = @C6)
AND (ST >= @Start AND ST < @End)
ORDER BY ST";
				DataTable dt = new DataTable();
				using (DBEntities db = new DBEntities())
				{
					dt = Function.getDataTable(db, SqlStr, new SqlParameter("C3", k1), new SqlParameter("C4", k2), new SqlParameter("C5", k3), new SqlParameter("C6", k4)
						, new SqlParameter("Start", tmpStart.ToDateString()), new SqlParameter("End", tmpEnd.ToDateString()));
				}
				DataTable dtNew = dt.Clone();
				for (int i = 0; i < dtNew.Columns.Count; i++)
				{
					dtNew.Columns[i].ReadOnly = false;
				}
				if (dt != null && dt.Rows.Count > 0)
				{
					int iIndex = 0;
					string DATEs = string.Empty;
					foreach (DataRow dr in dt.Rows)
					{
						dtNew.ImportRow(dr);
						DataRow drNew = dtNew.Rows[iIndex];
						string C15 = dr["入場方式"].ToString().Split(Function.DELIMITER)[0];
						if (C15.IsNullOrEmpty())
						{
							drNew["入場方式"] = "不開放";
						}
						else if (C15.Equals(AdmissionType.freeTicket.ToString()))
						{
							drNew["入場方式"] = AdmissionType.freeTicket.GetDescription();
						}
						else if (C15.Equals(AdmissionType.ropeTicket.ToString()))
						{
							drNew["入場方式"] = AdmissionType.ropeTicket.GetDescription();
						}
						else if (C15.Equals(AdmissionType.sellTicket.ToString()))
						{
							drNew["入場方式"] = AdmissionType.sellTicket.GetDescription();
						}
						else if (C15.Equals(AdmissionType.otherTicket.ToString()))
						{
							drNew["入場方式"] = AdmissionType.otherTicket.GetDescription();
						}
						iIndex++;
					}

					HSSFWorkbook workbook = new HSSFWorkbook();
					#region 查詢條件
					HSSFSheet sheetLimit = workbook.CreateSheet("查詢條件") as HSSFSheet;
					HSSFRow rowLimit = sheetLimit.CreateRow(0) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("演出館別");
					rowLimit.CreateCell(1).SetCellValue(k1.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k1));

					rowLimit = sheetLimit.CreateRow(1) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("演出場地");
					rowLimit.CreateCell(1).SetCellValue(k2.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k2));

					rowLimit = sheetLimit.CreateRow(2) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("展演類型");
					rowLimit.CreateCell(1).SetCellValue(k3.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k3));

					rowLimit = sheetLimit.CreateRow(3) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("活動類型");
					rowLimit.CreateCell(1).SetCellValue(k4.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k4));

					rowLimit = sheetLimit.CreateRow(4) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("日期區間");
					rowLimit.CreateCell(1).SetCellValue(start.ToDateString() + "~" + end.ToDateString());

					sheetLimit.AutoSizeColumn(0);
					sheetLimit.AutoSizeColumn(1);
					#endregion
					#region 樣式－欄位名稱

					//字型
					HSSFFont fontHeader = workbook.CreateFont() as HSSFFont;
					fontHeader.FontHeightInPoints = 12;
					//fontHeader.Boldweight = (short)FontBoldWeight.BOLD; //粗體
					fontHeader.FontName = "標楷體";

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
					fontContent.FontName = "標楷體";

					HSSFFont fontContent2 = workbook.CreateFont() as HSSFFont;
					fontContent2.FontHeightInPoints = 12;
					fontContent2.FontName = "標楷體";

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

					//設定小計格式
					HSSFCellStyle fontStyle_TOTAL = workbook.CreateCellStyle() as HSSFCellStyle;
					fontStyle_TOTAL.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
					fontStyle_TOTAL.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
					fontStyle_TOTAL.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
					fontStyle_TOTAL.FillPattern = NPOI.SS.UserModel.FillPatternType.SOLID_FOREGROUND;
					fontStyle_TOTAL.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
					fontStyle_TOTAL.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
					fontStyle_TOTAL.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
					fontStyle_TOTAL.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
					fontStyle_TOTAL.WrapText = true;
					fontStyle_TOTAL.SetFont(fontContent2);

					#endregion 樣式－欄位內容
					using (MemoryStream ms = new MemoryStream())
					{
						HSSFSheet sheet = workbook.CreateSheet("藝設中心活動列表") as HSSFSheet;

						DateTime _DATE = new DateTime();
						int iRowIndex = 1;
						HSSFRow row = sheet.CreateRow(iRowIndex) as HSSFRow;
						foreach (DataRow dr in dtNew.Rows)
						{
							_DATE = Convert.ToDateTime(dr["日期"]);
							row.CreateCell(0).SetCellValue(iRowIndex);
							row.CreateCell(1).SetCellValue(Function.GetNodeTitle(dr["類別"].ToString()));
							row.CreateCell(2).SetCellValue(_DATE.ToDateString());
							row.CreateCell(3).SetCellValue(_DATE.ToString("HH:mm"));
							row.CreateCell(4).SetCellValue(dr["入場方式"].ToString());
							row.CreateCell(5).SetCellValue(Function.GetNodeTitle(dr["地點"].ToString()));
							row.CreateCell(6).SetCellValue(dr["活動名稱"].ToString());
							row.CreateCell(7).SetCellValue(dr["申請單位"].ToString());
							row.CreateCell(8).SetCellValue(dr["人次"].ToString().ToInt());
							row.CreateCell(9).SetCellValue(dr["裝臺天數"].ToString().ToInt());
							row.CreateCell(10).SetCellValue(dr["演出天數"].ToString().ToInt());
							row.CreateCell(11).SetCellValue("");
							row.CreateCell(12).SetCellValue("");
							row.CreateCell(13).SetCellValue("");
							row.CreateCell(14).SetCellValue("");
							row.CreateCell(15).SetCellValue("");

							iRowIndex++;
							row = sheet.CreateRow(iRowIndex) as HSSFRow;
						}
						for (int n = 0; n < 16; n++)
						{
							if (n == 0)
								row.CreateCell(n).SetCellValue("小計");
							else if (n == 5)
								row.CreateCell(n).SetCellValue(string.Format("預計 {0} 場演出", dtNew.Rows.Count));
							else
								row.CreateCell(n).SetCellValue("");
							row.GetCell(n).CellStyle = fontStyle_TOTAL;
						}
						sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 0, 4));
						sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 5, 6));

						//表頭
						List<string> lsHeader = new List<string>() {
							"項次", "類別", "日期", "時間", "入場方式", "地點", "活動名稱", "申請單位"
							, "人次", "裝臺天數", "演出天數"
							, "承辦窗口", "備註", "場租", "補助款", "上班日" };
						HSSFRow rowHeader = sheet.CreateRow(0) as HSSFRow;
						for (int i = 0; i < lsHeader.Count; i++)
						{
							rowHeader.CreateCell(i).SetCellValue(lsHeader[i]);
						}

						//套用格式
						for (int i = 0; i < sheet.LastRowNum; i++)
						{
							HSSFRow rowX = sheet.GetRow(i) as HSSFRow;
							if (rowX == null)
								rowX = sheet.CreateRow(i) as HSSFRow;
							for (int j = 0; j < lsHeader.Count; j++)
							{
								HSSFCell cell = rowX.GetCell(j) as HSSFCell;
								if (cell == null)
									cell = rowX.CreateCell(j) as HSSFCell;
								cell.CellStyle = i == 0 ? styleHeader : styleContent3;
							}
						}

						for (int j = 0; j < lsHeader.Count; j++)
						{
							sheet.AutoSizeColumn(j);
						}

						workbook.Write(ms);
						workbook = null;
						return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + "藝設中心活動列表.xls");
					}
				}
			}
			return View();
		}

		/// <summary>
		/// 刪除 DATA2
		/// </summary>
		[ActionLog(TableNameIndex = 16, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string k2, bool really = false, string actionName = "Index")
		{
			CheckAuthority(Authority_Right.Delete);

			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				DATA2 d2 = iDB.GetByID<DATA2>(id, false);
				if (d2 != null)
				{
					d2.CONTENT30 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			if (!iDB.Delete<DATA2>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "k4", "start", "end" }), actionName);
		}

		/// <summary>
		/// http://www.afmc.gov.tw:81/Exhibition/fixDateTime?nid=fun13_05_03
		/// </summary>
		public ActionResult fixDateTime()
		{
			StringBuilder sb = new StringBuilder();
			using (DBEntities db = new DBEntities())
			{
				string SqlStr = @"SELECT TOP 100 PERCENT tp.TIME_ID, tp.CREATE_DATE, 'NO_ACCOUNT' as CREATER, tt.ARTICLE_ID, 1 as [ENABLE]
, (CASE tt.TIME_TYPE WHEN 'dress' THEN 0 WHEN 'performance' THEN 1 WHEN 'unload' THEN 2 END) as [ORDER]
, tp.[START_DATE], tp.END_DATE,'TIME' as PLUS_TYPE
, DATEDIFF(HOUR, tp.START_DATE, tp.END_DATE) as diffHour
FROM (
	SELECT t.ARTICLE_ID, t.TIME_TYPE, COUNT(*) as COUNTs
	, MIN(t.[START_DATE]) as minSD, MAX(t.END_DATE) as maxED
	, (DATEDIFF(day, CONVERT(varchar(10),MIN(t.[START_DATE]),111), CONVERT(varchar(10),MAX(t.END_DATE),111)) + 1) as diffDay
	FROM (
		SELECT TOP 100 PERCENT tp.TIME_ID, tp.ARTICLE_ID, tp.TIME_TYPE, tp.[START_DATE], tp.END_DATE
		FROM [AFMC20201026].dbo.NODE_ARTICLE_MAPPING m
		JOIN [AFMC20201026].dbo.ARTICLE a ON m.ARTICLE_ID = a.ARTICLE_ID AND m.NODE_ID = 'function15_2' AND a.LOCKED = 'false'
		JOIN [AFMC20201026].dbo.TIME_PACKAGE tp ON tp.ARTICLE_ID = a.ARTICLE_ID AND tp.TIME_TYPE <> 'admission'
		ORDER BY tp.ARTICLE_ID, tp.TIME_TYPE, tp.[START_DATE]
	) t
	GROUP BY t.ARTICLE_ID, t.TIME_TYPE
) tt
JOIN [AFMC20201026].dbo.TIME_PACKAGE tp ON tp.ARTICLE_ID = tt.ARTICLE_ID AND tp.TIME_TYPE = tt.TIME_TYPE
--WHERE tt.ARTICLE_ID = '6549fc3d820a41cf946f092f65cd3d36'
--WHERE tt.COUNTS <> tt.diffDay AND NOT ((DATEDIFF(day, tp.START_DATE, tp.END_DATE) + 1) > 1 OR DATEDIFF(HOUR, tp.START_DATE, tp.END_DATE) < 0)
ORDER BY tp.ARTICLE_ID, tp.TIME_TYPE, tp.[START_DATE], tp.[END_DATE]";
				DataTable dt = Function.getDataTable(db, SqlStr);
				if (dt != null && dt.Rows.Count > 0)
				{
					DataTable dtNew = dt.Clone();
					for (int x = 0; x < 3; x++)
					{
						string TIME_ID = string.Empty;
						int ORDER = 1, diffHour = 0;
						DateTime CREATE_DATE = DateTime.Now, START_DATE = DateTime.MinValue, END_DATE = DateTime.MaxValue;
						string startDate = string.Empty, endDate = string.Empty, tmpSD = string.Empty, tmpED = string.Empty;

						int idx = 0;
						IEnumerable<DataRow> drs = dt.AsEnumerable();
						List<string> lsID = drs.Select(p => p.Field<string>("ARTICLE_ID")).Distinct().ToList();
						foreach (string sID in lsID)
						{
							diffHour = 0;
							END_DATE = DateTime.MinValue;
							foreach (DataRow dr in drs.Where(p => p.Field<string>("ARTICLE_ID") == sID && p.Field<int>("ORDER") == x))
							{
								if (diffHour == 0)
								{
									if (Convert.ToDateTime(dr["END_DATE"]) == END_DATE.AddDays(1))
									{
										dtNew.Rows[dtNew.Rows.Count - 1]["END_DATE"] = Convert.ToDateTime(dr["END_DATE"]);

										diffHour = Convert.ToInt32(dr["diffHour"]); //相差幾小時
										START_DATE = Convert.ToDateTime(dr["START_DATE"]);
										END_DATE = Convert.ToDateTime(dr["END_DATE"]);

									}
									else
									{
										dtNew.ImportRow(dr);
										idx++;

										diffHour = Convert.ToInt32(dr["diffHour"]); //相差幾小時
										TIME_ID = dr["TIME_ID"].ToString();
										ORDER = Convert.ToInt32(dr["ORDER"]);
										CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
										START_DATE = Convert.ToDateTime(dr["START_DATE"]);
										END_DATE = Convert.ToDateTime(dr["END_DATE"]);
									}
								}
								else
								{
									if (Convert.ToInt32(dr["diffHour"]) == diffHour)
									{
										if (Convert.ToDateTime(dr["END_DATE"]) == END_DATE.AddDays(1))
										{
											dtNew.Rows[dtNew.Rows.Count - 1]["END_DATE"] = Convert.ToDateTime(dr["END_DATE"]);

											diffHour = Convert.ToInt32(dr["diffHour"]); //相差幾小時
											START_DATE = Convert.ToDateTime(dr["START_DATE"]);
											END_DATE = Convert.ToDateTime(dr["END_DATE"]);

										}
										else
										{
											dtNew.ImportRow(dr);
											idx++;

											diffHour = Convert.ToInt32(dr["diffHour"]); //相差幾小時
											TIME_ID = dr["TIME_ID"].ToString();
											ORDER = Convert.ToInt32(dr["ORDER"]);
											CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
											START_DATE = Convert.ToDateTime(dr["START_DATE"]);
											END_DATE = Convert.ToDateTime(dr["END_DATE"]);
										}
									}
									else
									{
										diffHour = Convert.ToInt32(dr["diffHour"]); //相差幾小時
										TIME_ID = dr["TIME_ID"].ToString();
										ORDER = Convert.ToInt32(dr["ORDER"]);
										CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]);
										START_DATE = Convert.ToDateTime(dr["START_DATE"]);
										END_DATE = Convert.ToDateTime(dr["END_DATE"]);

										dtNew.ImportRow(dr);
										idx++;
									}
								}
							}
						}
					}
					//					sb.Append("<table border='1'>");
					//					sb.Append(@"<td>TIME_ID</td><td>CREATE_DATE</td><td>CREATER</td><td>ARTICLE_ID</td>
					//<td>ENABLE</td><td>ORDER</td><td>START_DATE</td><td>END_DATE</td><td>PLUS_TYPE</td>");
					int i = 1;
					foreach (DataRow dr in dtNew.Rows)
					{
						/*sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>"
							, dr["TIME_ID"].ToString(), Convert.ToDateTime(dr["CREATE_DATE"]).ToString("yyyy/MM/dd HH:mm.ss"), "NO_ACCOUNT", dr["ARTICLE_ID"].ToString()
							, EnableType.Enable.ToIntValue(), dr["ORDER"], Convert.ToDateTime(dr["START_DATE"]).ToDateTimeString(), Convert.ToDateTime(dr["END_DATE"]).ToDateTimeString(), "TIME");*/
						sb.AppendFormat(@"INSERT INTO PLUS(ID,CREATE_DATE,CREATER,MAIN_ID,[ENABLE],[ORDER],[DATETIME1],[DATETIME2],PLUS_TYPE)<br/> 
VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');<br/>"
							, dr["TIME_ID"].ToString(), Convert.ToDateTime(dr["CREATE_DATE"]).ToString("yyyy/MM/dd HH:mm.ss"), "NO_ACCOUNT", dr["ARTICLE_ID"].ToString()
							, EnableType.Enable.ToIntValue(), dr["ORDER"], Convert.ToDateTime(dr["START_DATE"]).ToDateTimeString(), Convert.ToDateTime(dr["END_DATE"]).ToDateTimeString(), "TIME");
						i++;
					}
					//sb.Append("</table>");
				}
			}
			sb.Append(@"GO<br/>
UPDATE DATA2
SET DATETIME9 = (SELECT MIN(DATETIME1) FROM PLUS WHERE PLUS_TYPE = 'TIME' AND [ORDER] = 1 AND PLUS.MAIN_ID = DATA2.ID)
, DATETIME10 = (SELECT MAX([DATETIME2]) FROM PLUS WHERE PLUS_TYPE = 'TIME' AND [ORDER] = 1 AND PLUS.MAIN_ID = DATA2.ID)
FROM DATA2 WHERE NODE_ID = 'fun13_05_03';");
			return Content(sb.ToString());
		}

		#endregion

		#region 便民服務>索票資訊&身心障礙索票
		[NodeSelect("hallType")]
		public ActionResult TicketIndex(int? page, int? defaultPage, string k, string k1, string start, string end)
		{
			ViewBag.k1 = k1;
			ViewBag.start = start;
			ViewBag.end = end;

			DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
			DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;

			IPagedList<DATA2> list = null;
			if (NodeID.CheckStringValue(fun13_06_03)) /*身心障礙索票*/
			{
				list = iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: fun13_05_03)
					.Where(p => p.DECIMAL1 == 1 && p.DECIMAL3 == 1 && (string.IsNullOrEmpty(k1) || p.CONTENT3.Contains(k1)) &&
					(string.IsNullOrEmpty(k) || p.CONTENT7.Contains(k)) &&
					p.PLUS.Count(x => x.ORDER == 1 && x.DATETIME1 >= tmpStart && x.DATETIME2 < tmpEnd) > 0)
					.OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage);
			}
			else /*索票資訊*/
			{
				string ropeTicket = AdmissionType.ropeTicket.ToString();
				list = iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: fun13_05_03)
					.Where(p => p.CONTENT15.StartsWith(ropeTicket) && (string.IsNullOrEmpty(k1) || p.CONTENT3.Contains(k1)) &&
					(string.IsNullOrEmpty(k) || p.CONTENT7.Contains(k)) &&
					p.PLUS.Count(x => x.ORDER == 1 && x.DATETIME1 >= tmpStart && x.DATETIME2 < tmpEnd) > 0)
					.OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage);
				foreach (DATA2 d2 in list)
				{
					//僅顯示
					d2.CONTENT = string.Join("<br />", d2.PLUS.Where(x => x.ORDER == 1 && x.DATETIME1 >= tmpStart && x.DATETIME2 < tmpEnd)
						.Select(x => x.DATETIME1.Value.ToString("yyyy/MM/dd HH:mm") + "~" + x.DATETIME2.Value.ToString("yyyy/MM/dd HH:mm")).ToArray());
				}
			}
			return View(list);
		}

		public DATA2 TicketInit(DATA2 d2)
		{
			if (d2 != null)
			{
				d2.CONTENT3 = Function.GetNodeTitle(d2.CONTENT3);
				d2.CONTENT4 = Function.GetNodeTitle(d2.CONTENT4);
				d2.CONTENT5 = Function.GetNodeTitle(d2.CONTENT5);
				d2.CONTENT6 = Function.GetNodeTitle(d2.CONTENT6);
				d2.CONTENT21 = d2.PARAGRAPH.FirstOrDefault(p => p.ORDER == 1).CONTENT;
				d2.CONTENT = string.Join("<br />", d2.PLUS.Where(x => x.ORDER == 1)
					.Select(x => x.DATETIME1.Value.ToString("yyyy/MM/dd HH:mm") + "~" + x.DATETIME2.Value.ToString("yyyy/MM/dd HH:mm")).ToArray());
				if (NodeID.CheckStringValue(fun13_06_03))
				{
					ViewBag.DateList = new SelectList(d2.GetPlusDateList(), "Value", "Text");
				}
			}
			return d2;
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		public ActionResult TicketEdit(string id, int? page, int? defaultPage, string k)
		{
			SetIsEdit(IsAuthority(Authority_Right.Update));
			return View(TicketInit(iDB.GetByIDAsNoTracking<DATA2>(id)));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 16, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult TicketEdit(string id, DATA2 model, int? page, int? defaultPage, string k)
		{
			if (NodeID.CheckStringValue(fun13_06_03))
			{
				model = TicketInit(iDB.GetByIDAsNoTracking<DATA2>(id));
			}
			else
			{
				CheckAuthority(Authority_Right.Update);
				if (ModelState.IsValid)
				{
					DATA2 d2 = iDB.GetByID<DATA2>(id);
					if (d2 != null)
					{
						d2.DECIMAL6 = model.DECIMAL6;
						iDB.Save();
						AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
					}
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), "TicketIndex");
				}
			}
			SetModelStateError();
			return View(model);
		}
		#endregion
	}
}