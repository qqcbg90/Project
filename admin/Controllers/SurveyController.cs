using admin.Filters;
using Ionic.Zip;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MessagingToolkit.QRCode.Codec;
using MvcPaging;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class SurveyController : BaseController
	{
		/// <summary>
		/// 館別
		/// </summary>
		public const string HALL_TYPE = "halltype";
		/// <summary>
		/// 展演分類
		/// </summary>
		public const string PERFORMANCE_TYPE = "performancetype";
		/// <summary>
		/// 觀眾問卷調查統計>問卷輸入
		/// </summary>
		public const string fun07_01 = "fun07_01";
		/// <summary>
		/// 觀眾問卷調查統計>問卷輸出
		/// </summary>
		public const string fun07_02 = "fun07_02";
		/// <summary>
		/// 團隊調查統計系統>問卷輸入
		/// </summary>
		public const string fun08_01 = "fun08_01";
		/// <summary>
		/// 團隊調查統計系統>報表產製
		/// </summary>
		public const string fun08_02 = "fun08_02";
		/// <summary>
		/// 鐵玫瑰問卷調查統計>問卷輸入
		/// </summary>
		public const string fun15_01 = "fun15_01";
		/// <summary>
		/// 鐵玫瑰問卷調查統計>報表產製
		/// </summary>
		public const string fun15_02 = "fun15_02";

		#region 觀眾問卷調查統計 - 問卷輸入 > 檔期搜尋
		/// <summary>
		/// 列表
		/// </summary>
		[NodeSelect(HALL_TYPE)]
		public ActionResult ExhibitionIndex(int? pageX, int? defaultPage, string kX, string k1)
		{
			//演出館別
			k1 = k1.IsNullOrEmpty() ? "hall1" : k1; //預設:桃園展演中心
			ViewBag.k1 = k1;

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			pageX = IsPost() ? 0 : pageX;
			if (k1.CheckStringValue("hall3")) //桃園光影文化館
			{
				return View("ExhibitionTacIndex", iDB.GetAllAsNoTracking<DATA1>(MAIN_ID: "fun14_04_03")
				.Where(p => (string.IsNullOrEmpty(kX) || p.CONTENT1.Contains(kX) || p.CONTENT14.Contains(kX)))
				.OrderByDescending(p => p.DATETIME9).ToPagedList(pageX.ToMvcPaging(), _defaultPage));
			}
			else
			{
				return View(iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: "fun13_05_03")
				.Where(p => (string.IsNullOrEmpty(kX) || p.CONTENT1.Contains(kX) || p.CONTENT7.Contains(kX)) &&
				(string.IsNullOrEmpty(k1) || p.CONTENT3.Contains(k1)))
				.OrderByDescending(p => p.DATETIME9).ToPagedList(pageX.ToMvcPaging(), _defaultPage));
			}
		}
		#endregion

		#region 觀眾問卷調查統計 - 問卷輸入 > 問卷QRCode
		/// <summary>
		/// 問卷QRCode
		/// </summary>
		public ActionResult ZipQRCode(string exid, string k1)
		{
			int iQuizNo = -1;
			bool IsRopeTicket = false;
			string downloadFileName = string.Empty;

			if (exid.CheckStringValue("IronRose"))
			{
				string TempPath = Function.GetRealityPath("Temp");
				if (!Directory.Exists(TempPath))
				{
					Directory.CreateDirectory(TempPath);
				}
				string TempIDPath = string.Format("{0}/QRCode_{1}/", TempPath, exid);
				if (!Directory.Exists(TempIDPath))
				{
					Directory.CreateDirectory(TempIDPath);
				}

				List<string> lsFiles = new List<string>();
				string FileName = string.Empty;
				string URL_FORMAT = Function.DEFAULT_ROOT_HTTP_AFMC.TrimEnd('/') + "/Survey/IronRose{0}" + exid;
				Dictionary<string, string> dict = Function.NodeList.Where(p => !p.PARENT_ID.IsNullOrEmpty() && p.PARENT_ID.Equals("halltype")).ToDictionary(p => p.ID, p => p.TITLE);
				if (dict != null)
				{
					foreach (KeyValuePair<string, string> kvp in dict)
					{
						FileName = TempIDPath + kvp.Value + "_鐵玫瑰藝術節QRCode.png";
						CreateQRcode(string.Format(URL_FORMAT, kvp.Key.Replace("hall", "")), FileName);
						lsFiles.Add(FileName);
					}
				}

				downloadFileName = "鐵玫瑰藝術節QRCode.zip";
				using (var zip = new ZipFile(System.Text.Encoding.UTF8))
				{
					foreach (string FILE in lsFiles)
					{
						zip.AddFile(FILE, "");
					}
					zip.Save(TempPath + downloadFileName);
				}
				if (Directory.Exists(TempIDPath))
				{
					Directory.Delete(TempIDPath, true);
				}
				return File(TempPath + downloadFileName, "application/x-zip-compressed", DateTime.Now.ToString("yyyyMMddHHmm") + downloadFileName);
			}
			else
			{
				if (k1.CheckStringValue("hall3")) //桃園光影文化館
				{
					DATA1 d1 = iDB.GetByIDAsNoTracking<DATA1>(exid);
					if (d1 != null)
					{
						downloadFileName = d1.CONTENT1 + "QRCode.zip";
					}
					iQuizNo = 4;
				}
				else
				{
					DATA2 d2 = iDB.GetByIDAsNoTracking<DATA2>(exid);
					if (d2 != null)
					{
						downloadFileName = d2.CONTENT7 + "QRCode.zip";
						IsRopeTicket = d2.CONTENT15.StartsWith(AdmissionType.ropeTicket.ToString());
						if ((new List<string>() { "performance1", "performance2" }).Contains(d2.CONTENT5))
						{
							iQuizNo = d2.CONTENT5.Replace("performance", "").ToInt();
						}
					}
				}

				if (!downloadFileName.IsNullOrEmpty())
				{
					string HallName = Function.GetNodeTitle(k1);
					string TempPath = Function.GetRealityPath("Temp");
					if (!Directory.Exists(TempPath))
					{
						Directory.CreateDirectory(TempPath);
					}
					string TempIDPath = string.Format("{0}/QRCode_{1}/", TempPath, exid);
					if (!Directory.Exists(TempIDPath))
					{
						Directory.CreateDirectory(TempIDPath);
					}

					List<string> lsFiles = new List<string>();
					string FileName = string.Empty;
					string URL_FORMAT = Function.DEFAULT_ROOT_HTTP_AFMC.TrimEnd('/') + "/Survey/{0}" + exid;
					if (iQuizNo == -1 || iQuizNo == 1) //表演藝術
					{
						FileName = TempIDPath + HallName + "_表演藝術QRCode.png";
						CreateQRcode(string.Format(URL_FORMAT, 1), FileName);
						lsFiles.Add(FileName);
					}
					if (iQuizNo == -1 || iQuizNo == 2) //展覽藝術
					{
						FileName = TempIDPath + HallName + "_展覽藝術QRCode.png";
						CreateQRcode(string.Format(URL_FORMAT, 2), FileName);
						lsFiles.Add(FileName);
					}
					//if (iQuizNo == -1 || iQuizNo == 3) //桃園鐵玫瑰藝術節
					//{
					//	FileName = TempIDPath + HallName + "_桃園鐵玫瑰藝術節QRCode.png";
					//	CreateQRcode(string.Format(URL_FORMAT, 3), FileName);
					//	lsFiles.Add(FileName);
					//}
					if (iQuizNo == -1 || iQuizNo == 4) //電影活動
					{
						FileName = TempIDPath + HallName + "_電影活動QRCode.png";
						CreateQRcode(string.Format(URL_FORMAT, 4), FileName);
						lsFiles.Add(FileName);
					}
					if (IsRopeTicket) //索票
					{
						FileName = TempIDPath + HallName + "_索票QRCode.png";
						CreateQRcode(string.Format(URL_FORMAT, 5), FileName);
						lsFiles.Add(FileName);
					}
					using (var zip = new ZipFile(System.Text.Encoding.UTF8))
					{
						foreach (string FILE in lsFiles)
						{
							zip.AddFile(FILE, "");
						}
						zip.Save(TempPath + downloadFileName);
					}
					if (Directory.Exists(TempIDPath))
					{
						Directory.Delete(TempIDPath, true);
					}
					return File(TempPath + downloadFileName, "application/x-zip-compressed", DateTime.Now.ToString("yyyyMMddHHmm") + downloadFileName);
				}
			}
			return Content("");
		}

		/// <summary>
		/// 建立QRCode
		/// </summary>
		public static void CreateQRcode(string url, string uploadPath)
		{
			var encoder = new QRCodeEncoder();
			using (var qrcode = encoder.Encode(url))
			{
				qrcode.Save(uploadPath, ImageFormat.Png);
			}
		}
		#endregion

		#region 觀眾問卷調查統計 - 問卷輸入 > 問卷管理
		[NodeSelect(HALL_TYPE)]
		public ActionResult AudienceIndex(string exid, int? page, int? defaultPage, string k, string k1, string k2, string k3
			, int? pageX, string kX
			, DateTime? start, DateTime? end, string sel = "", int export = 0)
		{
			ViewBag.exid = exid;
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k1.CheckStringValue("hall3") ? "4" : k3;

			ViewBag.ExName = "-";
			ViewBag.IsRopeTicket = false; //索票
			ViewBag.QuizNo = -1; //沒有問卷題庫

			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			//sel = export > 0 && sel.IsNullOrEmpty() ? "#" : sel; //匯出時，至少要選擇一項
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			if (export > 0)
			{
				_defaultPage = int.MaxValue;
			}
			page = IsPost() ? 0 : page;
			IPagedList<DATA8> model;
			if (exid.IsNullOrEmpty() || k1.IsNullOrEmpty())
			{
				return RedirectToAction("ExhibitionIndex", new { nid = "fun07_01" });
			}
			if (k1.CheckStringValue("hall3")) //桃園光影文化館
			{
				DATA1 d1 = iDB.GetByIDAsNoTracking<DATA1>(exid);
				ViewBag.ExName = d1 == null ? "-" : d1.CONTENT1 + (d1.CONTENT14.IsNullOrEmpty() ? "" : " / " + d1.CONTENT14) + " - " + d1.DECIMAL1.ToInt() + "年";
				ViewBag.QuizNo = 4;
				ViewBag.k3 = (ViewBag.QuizNo).ToString();
			}
			else
			{
				DATA2 d2 = iDB.GetByIDAsNoTracking<DATA2>(exid);
				if (d2 != null)
				{
					ViewBag.ExName = d2.CONTENT7;
					ViewBag.IsRopeTicket = d2.CONTENT15.StartsWith(AdmissionType.ropeTicket.ToString());
					if ((new List<string>() { "performance1", "performance2" }).Contains(d2.CONTENT5))
					{
						ViewBag.QuizNo = d2.CONTENT5.Replace("performance", "").ToInt();
						ViewBag.k3 = (ViewBag.QuizNo).ToString();
					}
				}
			}
			int iOrder = k3.IsNullOrEmpty() ? (int)ViewBag.QuizNo : k3.ToInt();
			model = iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: fun07_01)
				.Where(p => p.STATUS == exid && p.ORDER == iOrder).OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage);
			if (export == 2) //匯出E-mail
			{
				string lsEmail = string.Join(Environment.NewLine, iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: fun07_01)
					.Where(p => (string.IsNullOrEmpty(exid) || p.STATUS.Equals(exid)) && !string.IsNullOrEmpty(p.CONTENT6)).Select(p => p.CONTENT6).Distinct().ToArray());
				lsEmail = lsEmail.IsNullOrEmpty() ? "沒有 E-mail 資料" : lsEmail;
				return File(Encoding.UTF8.GetBytes(lsEmail), "text/plain", DateTime.Now.ToString("yyyyMMddHHmm") + "_Email.txt");
			}

			if (export == 1)
			{
				string jsonData = string.Empty;
				if (model != null)
				{
					StringBuilder sb = new StringBuilder();
					PARAGRAPH par;
					foreach (string DATA8_ID in model.OrderBy(p => p.CREATE_DATE).Select(p => p.ID))
					{
						par = iDB.GetAllAsNoTracking<PARAGRAPH>(MAIN_ID: DATA8_ID).FirstOrDefault(p => p.ORDER == 0);
						if (par != null)
						{
							sb.Append((sb.Length == 0 ? string.Empty : ",") + par.CONTENT.ToMyString());
						}
					}
					jsonData = string.Format("[{0}]", sb.ToString());
				}
				return ExportExcel_Fun07_01(jsonData, k1, k3);
			}
			else
			{
				return View(model);
			}
		}
		#endregion

		#region 觀眾問卷調查統計 - 問卷輸出
		[NodeSelect(HALL_TYPE)]
		public ActionResult AudienceExportIndex(string k1, string k3, DateTime? start, DateTime? end)
		{
			ViewBag.k1 = k1.IsNullOrEmpty() ? "hall1" : k1;
			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			string sMAIN_ID = string.Empty;
			if (NodeID.CheckStringValue(fun07_02))
			{
				sMAIN_ID = fun07_01;
				ViewBag.k3 = k1.CheckStringValue("hall3") ? "4" : k3;
			}
			else
			{
				sMAIN_ID = fun15_01;
				ViewBag.k3 = "3";
			}

			if (IsPost())
			{
				string jsonData = string.Empty;
				int iOrder = k3.IsNullOrEmpty() ? 1 : k3.ToInt();
				List<DATA8> model = iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: sMAIN_ID)
					.Where(p => p.DATA_TYPE == k1 && p.ORDER == iOrder && p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd).ToList();
				if (model != null)
				{
					StringBuilder sb = new StringBuilder();
					PARAGRAPH par;
					foreach (string DATA8_ID in model.OrderBy(p => p.CREATE_DATE).Select(p => p.ID))
					{
						par = iDB.GetAllAsNoTracking<PARAGRAPH>(MAIN_ID: DATA8_ID).FirstOrDefault(p => p.ORDER == 0);
						if (par != null)
						{
							sb.Append((sb.Length == 0 ? string.Empty : ",") + par.CONTENT.ToMyString());
						}
					}
					jsonData = string.Format("[{0}]", sb.ToString());
				}
				return ExportExcel_Fun07_01(jsonData, k1, k3);
			}
			return View();
		}
		#endregion

		#region 團隊調查統計系統
		/// <summary>
		/// 只會有1筆資料
		/// </summary>
		public List<DATA8> getFun08_02Data(DateTime? start, DateTime? end)
		{
			string SqlStr = @"SELECT SUM(Q1_1) as Q1_1,SUM(Q1_2) as Q1_2,SUM(Q1_3) as Q1_3,SUM(Q1_4) as Q1_4,SUM(Q1_5) as Q1_5,
SUM(Q1_6) as Q1_6,SUM(Q2_1) as Q2_1,SUM(Q2_2) as Q2_2,SUM(Q2_3) as Q2_3,SUM(Q2_4) as Q2_4,
SUM(Q3_1) as Q3_1,SUM(Q3_2) as Q3_2,SUM(Q3_3) as Q3_3,SUM(Q3_4) as Q3_4,SUM(Q3_5) as Q3_5,
SUM(Q4_1) as Q4_1,SUM(Q4_2) as Q4_2,SUM(Q4_3) as Q4_3,SUM(Q4_4) as Q4_4,SUM(Q4_5) as Q4_5,
SUM(Q4_6) as Q4_6,SUM(Q5_1) as Q5_1,SUM(Q5_2) as Q5_2,SUM(Q5_3) as Q5_3,SUM(Q5_4) as Q5_4,
SUM(Q5_5) as Q5_5,SUM(Q6_1) as Q6_1,SUM(Q6_2) as Q6_2,SUM(Q6_3) as Q6_3,SUM(Q6_4) as Q6_4,
SUM(Q6_5) as Q6_5,SUM(Q6_6) as Q6_6,SUM(Q6_7) as Q6_7,SUM(Q6_8) as Q6_8,SUM(Q7_1) as Q7_1,
SUM(Q7_2) as Q7_2,SUM(Q7_3) as Q7_3,SUM(Q7_4) as Q7_4,SUM(Q7_5) as Q7_5,SUM(Q7_6) as Q7_6,
SUM(Q7_7) as Q7_7,SUM(Q7_8) as Q7_8,SUM(Q7_9) as Q7_9,SUM(Q8_1) as Q8_1,SUM(Q8_2) as Q8_2,
SUM(Q8_3) as Q8_3,SUM(Q8_4) as Q8_4,SUM(Q8_5) as Q8_5,SUM(Q8_6) as Q8_6,SUM(Q8_7) as Q8_7,
SUM(Q9_1) as Q9_1,SUM(Q9_2) as Q9_2,SUM(Q9_3) as Q9_3,SUM(Q9_4) as Q9_4,SUM(Q9_5) as Q9_5,
SUM(Q9_6) as Q9_6,SUM(Q9_7) as Q9_7,SUM(Q10_1) as Q10_1,SUM(Q10_2) as Q10_2,SUM(Q10_3) as Q10_3,
SUM(Q10_4) as Q10_4,SUM(Q10_5) as Q10_5,SUM(Q10_6) as Q10_6,SUM(Q11_1) as Q11_1,SUM(Q11_2) as Q11_2,
SUM(Q11_3) as Q11_3,SUM(Q11_4) as Q11_4,SUM(Q11_5) as Q11_5,SUM(Q11_6) as Q11_6,SUM(Q12_1) as Q12_1,
SUM(Q12_2) as Q12_2,SUM(Q12_3) as Q12_3,SUM(Q12_4) as Q12_4,SUM(Q13_1) as Q13_1,SUM(Q13_2) as Q13_2,
SUM(Q13_3) as Q13_3,SUM(Q13_4) as Q13_4,SUM(Q13_5) as Q13_5,SUM(Q13_6) as Q13_6,SUM(Q14_1) as Q14_1,
SUM(Q14_2) as Q14_2,SUM(Q14_3) as Q14_3,SUM(Q14_4) as Q14_4,COUNT(*) as COUNTs
FROM (
SELECT ID, CONTENT1
, dbo.FindData(CONTENT4,'1') as Q1_1, dbo.FindData(CONTENT4,'2') as Q1_2, dbo.FindData(CONTENT4,'3') as Q1_3, dbo.FindData(CONTENT4,'4') as Q1_4, dbo.FindData(CONTENT4,'5') as Q1_5
, dbo.FindData(CONTENT4,'6') as Q1_6
, dbo.FindData(CONTENT5,'1') as Q2_1, dbo.FindData(CONTENT5,'2') as Q2_2, dbo.FindData(CONTENT5,'3') as Q2_3, dbo.FindData(CONTENT5,'4') as Q2_4
, dbo.FindData(CONTENT6,'1') as Q3_1, dbo.FindData(CONTENT6,'2') as Q3_2, dbo.FindData(CONTENT6,'3') as Q3_3, dbo.FindData(CONTENT6,'4') as Q3_4, dbo.FindData(CONTENT6,'5') as Q3_5
, dbo.FindData(CONTENT7,'1') as Q4_1, dbo.FindData(CONTENT7,'2') as Q4_2, dbo.FindData(CONTENT7,'3') as Q4_3, dbo.FindData(CONTENT7,'4') as Q4_4, dbo.FindData(CONTENT7,'5') as Q4_5
, dbo.FindData(CONTENT7,'6') as Q4_6
, dbo.FindData(CONTENT8,'1') as Q5_1, dbo.FindData(CONTENT8,'2') as Q5_2, dbo.FindData(CONTENT8,'3') as Q5_3, dbo.FindData(CONTENT8,'4') as Q5_4, dbo.FindData(CONTENT8,'5') as Q5_5
, dbo.FindData(CONTENT9,'1') as Q6_1, dbo.FindData(CONTENT9,'2') as Q6_2, dbo.FindData(CONTENT9,'3') as Q6_3, dbo.FindData(CONTENT9,'4') as Q6_4, dbo.FindData(CONTENT9,'5') as Q6_5
, dbo.FindData(CONTENT9,'6') as Q6_6, dbo.FindData(CONTENT9,'7') as Q6_7, dbo.FindData(CONTENT9,'8') as Q6_8
, dbo.FindData(CONTENT10,'1') as Q7_1, dbo.FindData(CONTENT10,'2') as Q7_2, dbo.FindData(CONTENT10,'3') as Q7_3, dbo.FindData(CONTENT10,'4') as Q7_4, dbo.FindData(CONTENT10,'5') as Q7_5
, dbo.FindData(CONTENT10,'6') as Q7_6, dbo.FindData(CONTENT10,'7') as Q7_7, dbo.FindData(CONTENT10,'8') as Q7_8, dbo.FindData(CONTENT10,'9') as Q7_9
, dbo.FindData(CONTENT11,'1') as Q8_1, dbo.FindData(CONTENT11,'2') as Q8_2, dbo.FindData(CONTENT11,'3') as Q8_3, dbo.FindData(CONTENT11,'4') as Q8_4, dbo.FindData(CONTENT11,'5') as Q8_5
, dbo.FindData(CONTENT11,'6') as Q8_6, dbo.FindData(CONTENT11,'7') as Q8_7
, dbo.FindData(CONTENT12,'1') as Q9_1, dbo.FindData(CONTENT12,'2') as Q9_2, dbo.FindData(CONTENT12,'3') as Q9_3, dbo.FindData(CONTENT12,'4') as Q9_4, dbo.FindData(CONTENT12,'5') as Q9_5
, dbo.FindData(CONTENT12,'6') as Q9_6, dbo.FindData(CONTENT12,'7') as Q9_7
, dbo.FindData(CONTENT13,'1') as Q10_1, dbo.FindData(CONTENT13,'2') as Q10_2, dbo.FindData(CONTENT13,'3') as Q10_3, dbo.FindData(CONTENT13,'4') as Q10_4, dbo.FindData(CONTENT13,'5') as Q10_5
, dbo.FindData(CONTENT13,'6') as Q10_6
, dbo.FindData(CONTENT14,'1') as Q11_1, dbo.FindData(CONTENT14,'2') as Q11_2, dbo.FindData(CONTENT14,'3') as Q11_3, dbo.FindData(CONTENT14,'4') as Q11_4, dbo.FindData(CONTENT14,'5') as Q11_5
, dbo.FindData(CONTENT14,'6') as Q11_6
, dbo.FindData(CONTENT15,'1') as Q12_1, dbo.FindData(CONTENT15,'2') as Q12_2, dbo.FindData(CONTENT15,'3') as Q12_3, dbo.FindData(CONTENT15,'4') as Q12_4
, dbo.FindData(CONTENT16,'1') as Q13_1, dbo.FindData(CONTENT16,'2') as Q13_2, dbo.FindData(CONTENT16,'3') as Q13_3, dbo.FindData(CONTENT16,'4') as Q13_4, dbo.FindData(CONTENT16,'5') as Q13_5
, dbo.FindData(CONTENT16,'6') as Q13_6
, dbo.FindData(CONTENT17,'1') as Q14_1, dbo.FindData(CONTENT17,'2') as Q14_2, dbo.FindData(CONTENT17,'3') as Q14_3, dbo.FindData(CONTENT17,'4') as Q14_4
FROM DATA8 WHERE NODE_ID = 'fun08_01' AND [ENABLE] = 1
AND DATETIME3 >= @start AND DATETIME3 < @end
) t";
			List<DATA8> list = new List<DATA8>();
			using (DBEntities db = new DBEntities())
			{
				list = Function.getDataTable(db, SqlStr, new SqlParameter("start", start.Value.ToDateString()), new SqlParameter("end", end.Value.ToDateString())).AsEnumerable()
					.Select(p => new DATA8()
					{
						DECIMAL1 = p.Field<int?>("Q1_1"),
						DECIMAL2 = p.Field<int?>("Q1_2"),
						DECIMAL3 = p.Field<int?>("Q1_3"),
						DECIMAL4 = p.Field<int?>("Q1_4"),
						DECIMAL5 = p.Field<int?>("Q1_5"),
						DECIMAL6 = p.Field<int?>("Q1_6"),
						DECIMAL7 = p.Field<int?>("Q2_1"),
						DECIMAL8 = p.Field<int?>("Q2_2"),
						DECIMAL9 = p.Field<int?>("Q2_3"),
						DECIMAL10 = p.Field<int?>("Q2_4"),
						DECIMAL11 = p.Field<int?>("Q3_1"),
						DECIMAL12 = p.Field<int?>("Q3_2"),
						DECIMAL13 = p.Field<int?>("Q3_3"),
						DECIMAL14 = p.Field<int?>("Q3_4"),
						DECIMAL15 = p.Field<int?>("Q3_5"),
						DECIMAL16 = p.Field<int?>("Q4_1"),
						DECIMAL17 = p.Field<int?>("Q4_2"),
						DECIMAL18 = p.Field<int?>("Q4_3"),
						DECIMAL19 = p.Field<int?>("Q4_4"),
						DECIMAL20 = p.Field<int?>("Q4_5"),
						DECIMAL21 = p.Field<int?>("Q4_6"),
						DECIMAL22 = p.Field<int?>("Q5_1"),
						DECIMAL23 = p.Field<int?>("Q5_2"),
						DECIMAL24 = p.Field<int?>("Q5_3"),
						DECIMAL25 = p.Field<int?>("Q5_4"),
						DECIMAL26 = p.Field<int?>("Q5_5"),
						DECIMAL27 = p.Field<int?>("Q6_1"),
						DECIMAL28 = p.Field<int?>("Q6_2"),
						DECIMAL29 = p.Field<int?>("Q6_3"),
						DECIMAL30 = p.Field<int?>("Q6_4"),
						DECIMAL31 = p.Field<int?>("Q6_5"),
						DECIMAL32 = p.Field<int?>("Q6_6"),
						DECIMAL33 = p.Field<int?>("Q6_7"),
						DECIMAL34 = p.Field<int?>("Q6_8"),
						DECIMAL35 = p.Field<int?>("Q7_1"),
						DECIMAL36 = p.Field<int?>("Q7_2"),
						DECIMAL37 = p.Field<int?>("Q7_3"),
						DECIMAL38 = p.Field<int?>("Q7_4"),
						DECIMAL39 = p.Field<int?>("Q7_5"),
						DECIMAL40 = p.Field<int?>("Q7_6"),
						DECIMAL41 = p.Field<int?>("Q7_7"),
						DECIMAL42 = p.Field<int?>("Q7_8"),
						DECIMAL43 = p.Field<int?>("Q7_9"),
						DECIMAL44 = p.Field<int?>("Q8_1"),
						DECIMAL45 = p.Field<int?>("Q8_2"),
						DECIMAL46 = p.Field<int?>("Q8_3"),
						DECIMAL47 = p.Field<int?>("Q8_4"),
						DECIMAL48 = p.Field<int?>("Q8_5"),
						DECIMAL49 = p.Field<int?>("Q8_6"),
						DECIMAL50 = p.Field<int?>("Q8_7"),
						DECIMAL51 = p.Field<int?>("Q9_1"),
						DECIMAL52 = p.Field<int?>("Q9_2"),
						DECIMAL53 = p.Field<int?>("Q9_3"),
						DECIMAL54 = p.Field<int?>("Q9_4"),
						DECIMAL55 = p.Field<int?>("Q9_5"),
						DECIMAL56 = p.Field<int?>("Q9_6"),
						DECIMAL57 = p.Field<int?>("Q9_7"),
						DECIMAL58 = p.Field<int?>("Q10_1"),
						DECIMAL59 = p.Field<int?>("Q10_2"),
						DECIMAL60 = p.Field<int?>("Q10_3"),
						DECIMAL61 = p.Field<int?>("Q10_4"),
						DECIMAL62 = p.Field<int?>("Q10_5"),
						DECIMAL63 = p.Field<int?>("Q10_6"),
						DECIMAL64 = p.Field<int?>("Q11_1"),
						DECIMAL65 = p.Field<int?>("Q11_2"),
						DECIMAL66 = p.Field<int?>("Q11_3"),
						DECIMAL67 = p.Field<int?>("Q11_4"),
						DECIMAL68 = p.Field<int?>("Q11_5"),
						DECIMAL69 = p.Field<int?>("Q11_6"),
						DECIMAL70 = p.Field<int?>("Q12_1"),
						DECIMAL71 = p.Field<int?>("Q12_2"),
						DECIMAL72 = p.Field<int?>("Q12_3"),
						DECIMAL73 = p.Field<int?>("Q12_4"),
						DECIMAL74 = p.Field<int?>("Q13_1"),
						DECIMAL75 = p.Field<int?>("Q13_2"),
						DECIMAL76 = p.Field<int?>("Q13_3"),
						DECIMAL77 = p.Field<int?>("Q13_4"),
						DECIMAL78 = p.Field<int?>("Q13_5"),
						DECIMAL79 = p.Field<int?>("Q13_6"),
						DECIMAL80 = p.Field<int?>("Q14_1"),
						DECIMAL81 = p.Field<int?>("Q14_2"),
						DECIMAL82 = p.Field<int?>("Q14_3"),
						DECIMAL83 = p.Field<int?>("Q14_4"),
						ORDER = p.Field<int>("COUNTs")
					}).ToList();
			}
			return list;
		}

		public ActionResult TeamIndex(int? page, int? defaultPage, string k, DateTime? start, DateTime? end, string sel, int export = 0)
		{
			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			sel = export == 1 && sel.IsNullOrEmpty() ? "#" : sel; //匯出時，至少要選擇一項
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			if (export == 1)
			{
				_defaultPage = int.MaxValue;
			}
			page = IsPost() ? 0 : page;
			IPagedList<DATA8> model;
			if (NodeID.CheckStringValue(fun08_01))
			{
				model = iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: fun08_01)
					.Where(p => p.DATETIME3 >= tmpStart && p.DATETIME3 < tmpEnd && (string.IsNullOrEmpty(sel) || sel.Contains(p.ID)))
					.OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage);
			}
			else
			{
				model = getFun08_02Data(tmpStart, tmpEnd).ToPagedList(page.ToMvcPaging(), _defaultPage);
			}
			if (export == 1)
			{
				return ExportExcel(model, start, end);
			}
			else
			{
				return View(model);
			}
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		public ActionResult TeamEdit(string id, int? page, int? defaultPage, string k, DateTime? start, DateTime? end)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			TeamSurveyModel model = new TeamSurveyModel();
			if (IsAdd)
			{
				CheckAuthority(Authority_Right.Add);
				model.CREATER = User.Identity.Name;
				model.DATETIME1 = model.DATETIME2 = model.DATETIME3 = DateTime.Now;
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				DATA8 d8 = iDB.GetByIDAsNoTracking<DATA8>(id);
				if (d8 != null)
				{
					model = new TeamSurveyModel()
					{
						ID = d8.ID,
						CONTENT1 = d8.CONTENT1,
						CONTENT2 = d8.CONTENT2,
						CONTENT3 = d8.CONTENT3,
						CONTENT15 = d8.CONTENT15,
						CONTENT17 = d8.CONTENT17,
						CONTENT110 = d8.CONTENT110,
						DATETIME1 = d8.DATETIME1.Value,
						DATETIME2 = d8.DATETIME2.Value,
						DATETIME3 = d8.DATETIME3.Value,
					};
					string[] arr = d8.CONTENT4.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT4 = arr[0].Split(';').ToList();
						model.CONTENT4_OTH = arr[1];
					}
					arr = d8.CONTENT5.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT5 = arr[0];
						model.CONTENT5_OTH = arr[1];
					}
					arr = d8.CONTENT6.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT6 = arr[0].Split(';').ToList();
						model.CONTENT6_OTH = arr[1];
					}
					arr = d8.CONTENT7.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT7 = arr[0].Split(';').ToList();
						model.CONTENT7_OTH = arr[1];
					}
					arr = d8.CONTENT8.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT8 = arr[0].Split(';').ToList();
						model.CONTENT8_OTH = arr[1];
					}
					arr = d8.CONTENT9.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT9 = arr[0].Split(';').ToList();
						model.CONTENT9_OTH = arr[1];
					}
					arr = d8.CONTENT10.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT10 = arr[0].Split(';').ToList();
						model.CONTENT10_OTH = arr[1];
					}
					arr = d8.CONTENT11.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT11 = arr[0].Split(';').ToList();
						model.CONTENT11_OTH = arr[1];
					}
					arr = d8.CONTENT12.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT12 = arr[0].Split(';').ToList();
						model.CONTENT12_OTH = arr[1];
					}
					arr = d8.CONTENT13.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT13 = arr[0].Split(';').ToList();
						model.CONTENT13_OTH = arr[1];
					}
					arr = d8.CONTENT14.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT14 = arr[0].Split(';').ToList();
						model.CONTENT14_OTH = arr[1];
					}
					arr = d8.CONTENT16.Split(Function.DELIMITER);
					if (arr.Length >= 2)
					{
						model.CONTENT16 = arr[0].Split(';').ToList();
						model.CONTENT16_OTH = arr[1];
					}
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 22, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult TeamEdit(string id, TeamSurveyModel model, int? page, int? defaultPage, string k, DateTime? start, DateTime? end)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				DATA8 d8 = iDB.GetByID<DATA8>(id);
				if (d8 == null)
				{
					d8 = new DATA8()
					{
						NODE_ID = fun08_01,
						CREATER = User.Identity.Name,
					};
				}
				else
				{
					d8.UPDATER = User.Identity.Name;
					d8.UPDATE_DATE = DateTime.Now;
				}
				d8.CONTENT1 = model.CONTENT1;
				d8.CONTENT2 = model.CONTENT2;
				d8.CONTENT3 = model.CONTENT3;
				d8.CONTENT4 = (model.CONTENT4 == null ? string.Empty : string.Join(";", model.CONTENT4)) + Function.DELIMITER + model.CONTENT4_OTH;
				d8.CONTENT5 = (model.CONTENT5 == null ? string.Empty : string.Join(";", model.CONTENT5)) + Function.DELIMITER + model.CONTENT5_OTH;
				d8.CONTENT6 = (model.CONTENT6 == null ? string.Empty : string.Join(";", model.CONTENT6)) + Function.DELIMITER + model.CONTENT6_OTH;
				d8.CONTENT7 = (model.CONTENT7 == null ? string.Empty : string.Join(";", model.CONTENT7)) + Function.DELIMITER + model.CONTENT7_OTH;
				d8.CONTENT8 = (model.CONTENT8 == null ? string.Empty : string.Join(";", model.CONTENT8)) + Function.DELIMITER + model.CONTENT8_OTH;
				d8.CONTENT9 = (model.CONTENT9 == null ? string.Empty : string.Join(";", model.CONTENT9)) + Function.DELIMITER + model.CONTENT9_OTH;
				d8.CONTENT10 = (model.CONTENT10 == null ? string.Empty : string.Join(";", model.CONTENT10)) + Function.DELIMITER + model.CONTENT10_OTH;
				d8.CONTENT11 = (model.CONTENT11 == null ? string.Empty : string.Join(";", model.CONTENT11)) + Function.DELIMITER + model.CONTENT11_OTH;
				d8.CONTENT12 = (model.CONTENT12 == null ? string.Empty : string.Join(";", model.CONTENT12)) + Function.DELIMITER + model.CONTENT12_OTH;
				d8.CONTENT13 = (model.CONTENT13 == null ? string.Empty : string.Join(";", model.CONTENT13)) + Function.DELIMITER + model.CONTENT13_OTH;
				d8.CONTENT14 = (model.CONTENT14 == null ? string.Empty : string.Join(";", model.CONTENT14)) + Function.DELIMITER + model.CONTENT14_OTH;
				d8.CONTENT15 = (model.CONTENT15 == null ? string.Empty : string.Join(";", model.CONTENT15));
				d8.CONTENT16 = (model.CONTENT16 == null ? string.Empty : string.Join(";", model.CONTENT16)) + Function.DELIMITER + model.CONTENT16_OTH;
				d8.CONTENT17 = (model.CONTENT17 == null ? string.Empty : string.Join(";", model.CONTENT17));
				d8.CONTENT110 = model.CONTENT110;
				d8.DATETIME1 = model.DATETIME1;
				d8.DATETIME2 = model.DATETIME2;
				d8.DATETIME3 = model.DATETIME3;
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<DATA8>(d8);
				}
				else
				{
					IsSuccessful = iDB.Save();
				}
				if (IsSuccessful)
				{
					AlertMsg = IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE;
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "start", "end" }), "TeamIndex");
				}
			}
			SetModelStateError();
			return View(model);
		}
		#endregion

		#region 鐵玫瑰問卷調查統計 - 問卷輸入
		/// <summary>
		/// 只會有1筆資料
		/// </summary>
		public List<DATA8> getFun15_02Data(string k1, string k2, DateTime? start, DateTime? end)
		{
			List<DATA8> listX = new List<DATA8>();
			List<DATA8> list = iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: fun15_01)
				.Where(p => p.DATETIME1 >= start.Value && p.DATETIME1 < end.Value && (string.IsNullOrEmpty(k2) || p.DATA_TYPE.Equals(k2))).ToList();
			ViewBag.Sum = list == null ? 0 : list.Count;
			if (list != null)
			{
				SelectList Loc = Function.getSurveyLoc(false); //現居地
															   //SelectList LocX = Function.getSurveyLocX(false); //現居地 - 區域
				SelectList Age = Function.getSurveyAge(false); //年齡層

				int idx = 1;

				//性別 CONTENT2
				listX.Add(new DATA8() { ID = "MAIN", CONTENT1 = "※ 性別" });
				foreach (SexType sex in Enum.GetValues(typeof(SexType)))
				{
					listX.Add(new DATA8()
					{
						ORDER = idx,
						CONTENT1 = sex.GetDescription(),
						DECIMAL1 = list.Count(p => p.CONTENT2 == sex.ToIntValue())
					});
					idx++;
				}

				//現居地 CONTENT4
				listX.Add(new DATA8() { ID = "MAIN", CONTENT1 = "※ 現居地" });
				foreach (SelectListItem item in Loc)
				{
					listX.Add(new DATA8()
					{
						ORDER = idx,
						CONTENT1 = item.Text,
						DECIMAL1 = list.Count(p => p.CONTENT4 == item.Value)
					});
					idx++;
				}

				//年齡層 CONTENT7
				listX.Add(new DATA8() { ID = "MAIN", CONTENT1 = "※ 年齡層" });
				foreach (SelectListItem item in Age)
				{
					listX.Add(new DATA8()
					{
						ORDER = idx,
						CONTENT1 = item.Text,
						DECIMAL1 = list.Count(p => p.CONTENT7 == item.Value)
					});
					idx++;
				}

				string Q1 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT21.ToMyString()).ToArray());
				string Q2 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT22.ToMyString()).ToArray());
				string Q3 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT23.ToMyString()).ToArray());
				string Q4 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT24.ToMyString()).ToArray());
				string Q5 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT25.ToMyString()).ToArray());
				string Q6 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT26.ToMyString()).ToArray());
				string Q7 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT27.ToMyString()).ToArray());
				string Q8 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT28.ToMyString()).ToArray());
				string Q9 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT29.ToMyString()).ToArray());
				string Q10 = string.Join(Function.DELIMITER.ToString(), list.Select(p => p.CONTENT30.ToMyString()).ToArray());

				Dictionary<int, string[]> dictQ = new Dictionary<int, string[]>()
				{
					{ 1, Q1.Split(Function.DELIMITER) }, { 2, Q2.Split(Function.DELIMITER) }, { 3, Q3.Split(Function.DELIMITER) },
					{ 4, Q4.Split(Function.DELIMITER) }, { 5, Q5.Split(Function.DELIMITER) }, { 6, Q6.Split(Function.DELIMITER) },
					{ 7, Q7.Split(Function.DELIMITER) }, { 8, Q8.Split(Function.DELIMITER) }, { 9, Q9.Split(Function.DELIMITER) },
					{ 10, Q10.Split(Function.DELIMITER) }
				};

				List<SurveyQues> Ques = Function.getSurveyQAData("3"); //桃園鐵玫瑰藝術節
				if (Ques != null)
				{
					int seq = 1;
					foreach (SurveyQues ques in Ques.Where(p => p.Type != QuesType.textarea))
					{
						listX.Add(new DATA8() { ID = "MAIN", CONTENT1 = seq + ". " + ques.Name });
						foreach (SurveyOpt opt in ques.Opts)
						{
							listX.Add(new DATA8()
							{
								ORDER = idx,
								CONTENT1 = opt.Name,
								DECIMAL1 = dictQ[seq].Count(p => p.Equals(opt.Val))
							});
							idx++;
						}
						idx++;
						seq++;
					}
				}
			}
			return listX;
		}

		[NodeSelect(HALL_TYPE)]
		public ActionResult IronRoseIndex(int? page, int? defaultPage, string k, string k1, string k2, string k3, DateTime? start, DateTime? end, int export = 0)
		{
			k1 = k1.IsNullOrEmpty() ? "1" : k1;
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;

			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			if (export > 0 || NodeID.CheckStringValue(fun15_02))
			{
				_defaultPage = int.MaxValue;
			}
			page = IsPost() ? 0 : page;
			IPagedList<DATA8> model;
			if (NodeID.CheckStringValue(fun15_02))
			{
				model = getFun15_02Data(k1, k2, tmpStart, tmpEnd).ToPagedList(page.ToMvcPaging(), _defaultPage);
			}
			else
			{
				model = iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: fun15_01)
								.Where(p => p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd && (string.IsNullOrEmpty(k2) || p.DATA_TYPE.Equals(k2)))
								.OrderByDescending(p => p.CREATE_DATE)
								.ToPagedList(page.ToMvcPaging(), _defaultPage);
				if (export == 2)
				{
					string lsEmail = string.Join(Environment.NewLine, iDB.GetAllAsNoTracking<DATA8>(MAIN_ID: fun15_01)
						.Where(p => !string.IsNullOrEmpty(p.CONTENT6)).Select(p => p.CONTENT6).Distinct().ToArray());
					lsEmail = lsEmail.IsNullOrEmpty() ? "沒有 E-mail 資料" : lsEmail;
					return File(Encoding.UTF8.GetBytes(lsEmail), "text/plain", DateTime.Now.ToString("yyyyMMddHHmm") + "_Email.txt");
				}
			}

			if (export == 1)
			{
				return ExportExcel(model, start, end, k, k1, k2);
			}
			else
			{
				return View(model);
			}
		}
		#endregion

		[ActionLog(TableNameIndex = 22, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, DateTime? start, DateTime? end, string pid, bool really = false, string actionName = "Index")
		{
			CheckAuthority(Authority_Right.Delete);
			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				DATA8 d8 = iDB.GetByID<DATA8>(id, false);
				if (d8 != null)
				{
					d8.CONTENT109 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			if (!iDB.Delete<DATA8>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "pid", "start", "end" }), actionName);
		}

		public string GetOptionOther(string opt)
		{
			if (!opt.IsNullOrEmpty() && opt.IndexOf(Function.DELIMITER) != -1)
			{
				string[] arr = opt.Split(Function.DELIMITER);
				opt = arr.Length >= 1 ? arr[1] : string.Empty;
			}
			return opt;
		}

		/// <summary>
		/// 匯出 EXCEL
		/// </summary>
		public ActionResult ExportExcel(IPagedList<DATA8> model, DateTime? start, DateTime? end
			, string k = "", string k1 = "", string k2 = "", string k3 = "", string exid = "")
		{
			bool bFUN08_01 = NodeID.CheckStringValue(fun08_01);
			bool bFUN08_02 = NodeID.CheckStringValue(fun08_02);
			bool bFUN15_01 = NodeID.CheckStringValue(fun15_01);
			bool bFUN15_02 = NodeID.CheckStringValue(fun15_02);
			string sFileName = (NodeID.StartsWith("fun08") ? "團體" : "鐵玫瑰藝術節") + "意見調查" + (bFUN08_01 || bFUN15_01 ? "" : "統計") + "表";
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

			//樣式-正常字+置右
			HSSFCellStyle NormalRightBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRightBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRightBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRightBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.WrapText = true;
			NormalRightBorder.SetFont(fontContent2);

			HSSFDataFormat df = workbook.CreateDataFormat() as HSSFDataFormat;
			NormalRightBorder.DataFormat = df.GetFormat("#,##0");

			HSSFCellStyle NormalRightBorder2 = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRightBorder2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRightBorder2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRightBorder2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.WrapText = true;
			NormalRightBorder2.SetFont(fontContent2);

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
				int iCellIndex = bFUN08_01 || bFUN15_01 ? 3 : 2;
				HSSFSheet sheet = workbook.CreateSheet(sFileName) as HSSFSheet;
				HSSFRow row;
				if (bFUN08_01 || bFUN15_01) //問卷輸入
				{
					sheet.CreateRow(0).CreateCell(0).SetCellValue("桃園市政府藝文設施管理中心 " + sFileName);
					sheet.CreateRow(1).CreateCell(0).SetCellValue("匯出日期：" + DateTime.Now.ToDateString());
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, iCellIndex));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, iCellIndex));

					#region 問卷輸入					
					int iRowIndex = 3;
					List<string> lsHeader;

					if (bFUN08_01)
					{
						lsHeader = new List<string>() { "團隊名稱", "活動名稱", "意見調查", "意見與建議" };
						foreach (DATA8 m in model)
						{
							row = sheet.CreateRow(iRowIndex) as HSSFRow;
							row.CreateCell(0).SetCellValue(m.CONTENT1);
							row.CreateCell(1).SetCellValue(m.CONTENT2);
							row.CreateCell(2).SetCellValue(String.Format("表演空間：{1}{0}側台空間：{2}{0}懸吊系統：{3}{0}固定布幕：{4}{0}化妝間：{5}{0}燈光系統：{6}{0}音響系統：{7}{0}前台空間：{8}{0}觀眾席：{9}{0}後台人員：{10}{0}前台人員：{11}{0}整體服務：{12}",
	Environment.NewLine, GetOptionOther(m.CONTENT4), GetOptionOther(m.CONTENT5), GetOptionOther(m.CONTENT6), GetOptionOther(m.CONTENT7), GetOptionOther(m.CONTENT8),
	GetOptionOther(m.CONTENT9), GetOptionOther(m.CONTENT10), GetOptionOther(m.CONTENT11), GetOptionOther(m.CONTENT12), GetOptionOther(m.CONTENT13),
	GetOptionOther(m.CONTENT14), GetOptionOther(m.CONTENT16)));
							row.CreateCell(3).SetCellValue(m.CONTENT110);
							iRowIndex++;
						}
					}
					else
					{
						Dictionary<string, string> dictAge = Function.getSurveyAge(false).ToDictionary(p => p.Value, p => p.Text); //年齡層
						lsHeader = new List<string>() { "性別", "年齡層", "E-mail", "意見與建議" };
						foreach (DATA8 m in model)
						{
							row = sheet.CreateRow(iRowIndex) as HSSFRow;
							row.CreateCell(0).SetCellValue(m.CONTENT2.IsNullOrEmpty() ? "-" : ((SexType)m.CONTENT2.ToInt()).GetDescription());
							row.CreateCell(1).SetCellValue(m.CONTENT7.IsNullOrEmpty() ? "-" : (dictAge.ContainsKey(m.CONTENT7) ? dictAge[m.CONTENT7] : "-"));
							row.CreateCell(2).SetCellValue(m.CONTENT6);
							row.CreateCell(3).SetCellValue(m.CONTENT109);
							iRowIndex++;
						}
					}

					//表頭
					lsW = new List<int>() { 30, 30, 70, 30 };
					HSSFRow rowHeader = sheet.CreateRow(2) as HSSFRow;
					for (int i = 0; i < lsHeader.Count; i++)
					{
						rowHeader.CreateCell(i).SetCellValue(lsHeader[i]);
					}
					//套用格式
					int iLastRowNum = sheet.LastRowNum + 1;
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
							if (i >= 0 && i < 2)
							{
								cell.CellStyle = i == 0 ? NormalCenter : NormalRight;
							}
							else
							{
								cell.CellStyle = i == 2 ? styleHeader : NormalLeftBorder;
							}
						}
					}
					#endregion
				}
				else if (bFUN08_02 || bFUN15_02) //報表產製
				{
					sheet.CreateRow(0).CreateCell(0).SetCellValue("桃園市政府藝文設施管理中心 " + sFileName);
					sheet.CreateRow(1).CreateCell(0).SetCellValue("匯出日期：" + DateTime.Now.ToDateString());
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, iCellIndex));
					sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, iCellIndex));

					#region 報表產製

					lsW = new List<int>() { 50, 10, 10 };
					if (bFUN15_02)
					{
						decimal iVal = 0;
						decimal iSum = (int)ViewBag.Sum;

						int iRowIndex = 2;
						foreach (DATA8 m in model)
						{
							if (m.ID.CheckStringValue("main"))
							{
								sheet.CreateRow(iRowIndex).CreateCell(0).SetCellValue(m.CONTENT1);
								sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 0, iCellIndex));
								iRowIndex++;
							}
							else
							{
								iVal = (m.DECIMAL1 ?? 0);
								row = sheet.CreateRow(iRowIndex) as HSSFRow;
								row.CreateCell(0).SetCellValue(m.CONTENT1);
								row.CreateCell(1).SetCellValue((iSum == 0 ? 0 : Convert.ToInt32((iVal / iSum) * 100)).ToString() + "%");
								row.CreateCell(2).SetCellValue(iVal + " 票");
								iRowIndex++;
							}
						}
					}
					else
					{
						DATA8 m = model.FirstOrDefault();

						List<string> lsQ = new List<string>()
						{
							"", "一、表演區空間哪方面需改進？(可複選)",
							"二、左右區側台空間是否滿意？",
							"三、使用空桿時，哪方面需改進？(可複選)",
							"四、使用布幕時，哪方面需改進？(可複選)",
							"五、使用化妝室、廁所時，哪方面需改進？(可複選)",
							"六、使用燈光時，哪方面需改進？(可複選)",
							"七、使用音響時，哪方面需改進？(可複選)",
							"八、使用前台空間時，哪方面需改進？(可複選)",
							"九、對於觀眾席哪方面建議需要改善？(可複選)",
							"十、對於館方後台管理人員與您配合上需改進的地方？(可複選)",
							"十一、對於館方前台服務人員與您配合上需改進的地方？(可複選)",
							"十二、演出當中，您對於觀眾的回應與狀況？",
							"十三、您對於展演中心整體服務當中哪方面需改進？(可複選)",
							"十四、此次蒞臨，對於展演廳整體印象如何？"
						};
						Dictionary<int, List<string>> dictOpt = new Dictionary<int, List<string>>()
						{
							{ 1, new List<string>() { "1.完善", "2.寬度", "3.深度", "4.高度", "5.與觀眾席距離", "6.其他或說明" } },
							{ 2, new List<string>() { "1.完善", "2.太大", "3.過小", "4.其他或說明" } },
							{ 3, new List<string>() { "1.完善", "2.數量不足", "3.間距", "4.操作不便", "5.其他或說明" } },
							{ 4, new List<string>() { "1.完善", "2.顏色", "3.數量不足", "4.操作不便", "5.距離不適", "6.其他或說明" } },
							{ 5, new List<string>() { "1.完善", "2.數量不足", "3.設備不齊", "4.環境清潔", "5.其他或說明" } },
							{ 6, new List<string>() { "1.完善", "2.燈數不足", "3.種類不夠", "4.迴路不足", "5.燈桿不足", "6.控制台操作不易", "7.控制室位置不好" } },
							{ 7, new List<string>() { "1.完善", "2.主喇叭", "3.監聽喇叭", "4.麥克風迴路", "5.各類型麥克風", "6.控制系統", "7.播放系統", "8.INTER COM", "9.其它或說明" } },
							{ 8, new List<string>() { "1.完善", "2.指標不明", "3.服務台空間", "4.整體清潔", "5.設備不齊", "6.洗手間", "7.其它或說明" } },
							{ 9, new List<string>() { "1.完善", "2.座位", "3.動線", "4.音場表現", "5.視線", "6.空間大小", "7.其它或說明" } },
							{ 10, new List<string>() { "1.良好", "2.服務態度", "3.溝通配合", "4.行為舉止/服裝儀容", "5.其它或說明" } },
							{ 11, new List<string>() { "1.良好", "2.服務態度", "3.溝通配合", "4.行為舉止/服裝儀容", "5.其它或說明" } },
							{ 12, new List<string>() { "1.非常好", "2.良好", "3.尚可", "4.不好" } },
							{ 13, new List<string>() { "1.良好", "2.接待服務", "3.劇場服務", "4.硬體設備", "5.其它或說明" } },
							{ 14, new List<string>() { "1.非常好", "2.良好", "3.尚可", "4.不好" } }
						};
						Dictionary<int, List<decimal>> dictVal = new Dictionary<int, List<decimal>>()
						{
							{ 1, new List<decimal>() { (m.DECIMAL1 ?? 0), (m.DECIMAL2 ?? 0), (m.DECIMAL3 ?? 0), (m.DECIMAL4 ?? 0), (m.DECIMAL5 ?? 0), (m.DECIMAL6 ?? 0) } },
							{ 2, new List<decimal>() { (m.DECIMAL7 ?? 0), (m.DECIMAL8 ?? 0), (m.DECIMAL9 ?? 0), (m.DECIMAL10 ?? 0) }},
							{ 3, new List<decimal>() { (m.DECIMAL11 ?? 0), (m.DECIMAL12 ?? 0), (m.DECIMAL13 ?? 0), (m.DECIMAL14 ?? 0), (m.DECIMAL15 ?? 0) }},
							{ 4, new List<decimal>() { (m.DECIMAL16 ?? 0), (m.DECIMAL17 ?? 0), (m.DECIMAL18 ?? 0), (m.DECIMAL19 ?? 0), (m.DECIMAL20 ?? 0), (m.DECIMAL21 ?? 0) }},
							{ 5, new List<decimal>() { (m.DECIMAL22 ?? 0), (m.DECIMAL23 ?? 0), (m.DECIMAL24 ?? 0), (m.DECIMAL25 ?? 0), (m.DECIMAL26 ?? 0) }},
							{ 6, new List<decimal>() { (m.DECIMAL27 ?? 0), (m.DECIMAL28 ?? 0), (m.DECIMAL29 ?? 0), (m.DECIMAL30 ?? 0), (m.DECIMAL31 ?? 0),
								(m.DECIMAL32 ?? 0), (m.DECIMAL33 ?? 0), (m.DECIMAL34 ?? 0) }},
							{ 7, new List<decimal>() { (m.DECIMAL35 ?? 0), (m.DECIMAL36 ?? 0), (m.DECIMAL37 ?? 0), (m.DECIMAL38 ?? 0), (m.DECIMAL39 ?? 0),
								(m.DECIMAL40 ?? 0), (m.DECIMAL41 ?? 0), (m.DECIMAL42 ?? 0), (m.DECIMAL43 ?? 0) }},
							{ 8, new List<decimal>() { (m.DECIMAL44 ?? 0), (m.DECIMAL45 ?? 0), (m.DECIMAL46 ?? 0), (m.DECIMAL47 ?? 0), (m.DECIMAL48 ?? 0), (m.DECIMAL49 ?? 0), (m.DECIMAL50 ?? 0) }},
							{ 9, new List<decimal>() { (m.DECIMAL51 ?? 0), (m.DECIMAL52 ?? 0), (m.DECIMAL53 ?? 0), (m.DECIMAL54 ?? 0), (m.DECIMAL55 ?? 0), (m.DECIMAL56 ?? 0), (m.DECIMAL57 ?? 0) }},
							{ 10, new List<decimal>() { (m.DECIMAL58 ?? 0), (m.DECIMAL59 ?? 0), (m.DECIMAL60 ?? 0), (m.DECIMAL61 ?? 0), (m.DECIMAL62 ?? 0), (m.DECIMAL63 ?? 0) }},
							{ 11, new List<decimal>() { (m.DECIMAL64 ?? 0), (m.DECIMAL65 ?? 0), (m.DECIMAL66 ?? 0), (m.DECIMAL67 ?? 0), (m.DECIMAL68 ?? 0), (m.DECIMAL69 ?? 0) }},
							{ 12, new List<decimal>() { (m.DECIMAL70 ?? 0), (m.DECIMAL71 ?? 0), (m.DECIMAL72 ?? 0), (m.DECIMAL73 ?? 0) }},
							{ 13, new List<decimal>() { (m.DECIMAL74 ?? 0), (m.DECIMAL75 ?? 0), (m.DECIMAL76 ?? 0), (m.DECIMAL77 ?? 0), (m.DECIMAL78 ?? 0), (m.DECIMAL79 ?? 0) }},
							{ 14, new List<decimal>() { (m.DECIMAL80 ?? 0), (m.DECIMAL81 ?? 0), (m.DECIMAL82 ?? 0), (m.DECIMAL83 ?? 0) } }
						};

						decimal iVal = 0;
						decimal iSum = m.ORDER;

						int iRowIndex = 2;
						for (int i = 1; i < lsQ.Count; i++)
						{
							sheet.CreateRow(iRowIndex).CreateCell(0).SetCellValue(lsQ[i]);
							sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 0, iCellIndex));
							iRowIndex++;

							List<string> lsOpt = dictOpt[i];
							List<decimal> lsVal = dictVal[i];
							for (int j = 0; j < dictOpt[i].Count; j++)
							{
								iVal = lsVal[j];
								row = sheet.CreateRow(iRowIndex) as HSSFRow;
								row.CreateCell(0).SetCellValue(lsOpt[j]);
								row.CreateCell(1).SetCellValue((iSum == 0 ? 0 : Convert.ToInt32((iVal / iSum) * 100)).ToString() + "%");
								row.CreateCell(2).SetCellValue(lsVal[j] + " 票");
								iRowIndex++;
							}
						}
					}

					//套用格式
					int iLastRowNum = sheet.LastRowNum + 1;
					for (int i = 0; i < iLastRowNum; i++)
					{
						HSSFRow rowX = sheet.GetRow(i) as HSSFRow;
						if (rowX == null)
							rowX = sheet.CreateRow(i) as HSSFRow;
						for (int j = 0; j < 3; j++)
						{
							HSSFCell cell = rowX.GetCell(j) as HSSFCell;
							if (cell == null)
								cell = rowX.CreateCell(j) as HSSFCell;
							if (i >= 0 && i < 2)
							{
								cell.CellStyle = i == 0 ? NormalCenter : NormalRight;
							}
							else
							{
								cell.CellStyle = j == 0 ? NormalLeftBorder : NormalRightBorder;
							}
						}
					}
					#endregion
				}

				for (int j = 0; j < lsW.Count; j++)
				{
					sheet.SetColumnWidth(j, lsW[j] * 256);
				}

				#region 查詢條件
				/*HSSFSheet sheetLimit = workbook.CreateSheet("查詢條件") as HSSFSheet;
				HSSFRow rowLimit = sheetLimit.CreateRow(0) as HSSFRow;
				rowLimit.CreateCell(0).SetCellValue("日期區間");
				rowLimit.CreateCell(1).SetCellValue(start.ToDateString() + "~" + end.ToDateString());

				if (bFUN07_01 || bFUN07_02)
				{
					rowLimit = sheetLimit.CreateRow(1) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("活動館別");
					rowLimit.CreateCell(1).SetCellValue(k1.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k1));

					rowLimit = sheetLimit.CreateRow(2) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("展演類型");
					rowLimit.CreateCell(1).SetCellValue(k2.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k2));

					rowLimit = sheetLimit.CreateRow(3) as HSSFRow;
					rowLimit.CreateCell(0).SetCellValue("活動類型");
					rowLimit.CreateCell(1).SetCellValue(k3.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k3));

					if (bFUN07_01)
					{
						rowLimit = sheetLimit.CreateRow(4) as HSSFRow;
						rowLimit.CreateCell(0).SetCellValue("活動名稱");
						rowLimit.CreateCell(1).SetCellValue(k);
					}
				}

				sheetLimit.AutoSizeColumn(0);
				sheetLimit.AutoSizeColumn(1);*/
				#endregion

				workbook.Write(ms);
				workbook = null;
				return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss_") + sFileName + ".xls");
			}
		}


		/// <summary>
		/// 匯出 EXCEL
		/// </summary>
		/// <param name="jsonData">Json格式</param>
		/// <param name="k1">
		/// 演出館別
		/// hall1:桃園展演中心 hall2:中壢藝術館 hall3:桃園光影文化館
		/// hall4:桃園藝文廣場 hall5:A8藝文中心 hall6:桃園流行音樂露天劇場 改名為 桃園陽光劇場
		/// </param>
		/// <param name="k3">
		/// 問卷類型
		/// 1:表演藝術 2:展覽藝術 3:桃園鐵玫瑰藝術節 4:電影活動 5:索票
		/// </param>
		public ActionResult ExportExcel_Fun07_01(string jsonData, string k1, string k3)
		{
			string sFileName = (NodeID.StartsWith("fun07") ? "觀眾" : "鐵玫瑰藝術節") + "意見調查表";
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

			//樣式-正常字+置右
			HSSFCellStyle NormalRightBorder = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRightBorder.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRightBorder.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRightBorder.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder.WrapText = true;
			NormalRightBorder.SetFont(fontContent2);

			HSSFDataFormat df = workbook.CreateDataFormat() as HSSFDataFormat;
			NormalRightBorder.DataFormat = df.GetFormat("#,##0");

			HSSFCellStyle NormalRightBorder2 = workbook.CreateCellStyle() as HSSFCellStyle;
			NormalRightBorder2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
			NormalRightBorder2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
			NormalRightBorder2.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
			NormalRightBorder2.WrapText = true;
			NormalRightBorder2.SetFont(fontContent2);

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
				int iRowIndex = 0;
				int iCellIndex = 0;
				HSSFSheet sheet = workbook.CreateSheet("原始檔") as HSSFSheet;
				HSSFRow row;

				List<string> lsVal = new List<string>();

				#region 原始資料-標題
				List<string> lsHeader = new List<string>()
				{
					"資料序號",
					/*演出基本資訊*/
					"演出地點", "演出日期", "問卷建立日期時間", "演出名稱", "演出團隊", "辦理類型", "演出類型", "演出類型-其他:", "入場方式",
					/*觀眾基本資訊*/
					"姓名", "性別", "電話", "居住地", "居住地-區名", "E-mail", "年齡區間", "資料來源"
				};

				row = sheet.CreateRow(1) as HSSFRow;
				for (int i = 0; i < lsHeader.Count; i++)
				{
					row.CreateCell(i).SetCellValue(lsHeader[i]);
				}

				row = sheet.CreateRow(0) as HSSFRow;
				row.CreateCell(1).SetCellValue("演出基本資訊");
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 1, 9));
				row.CreateCell(10).SetCellValue("觀眾基本資訊");
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 10, 17));

				iCellIndex = 18;
				int iLen = 0;
				List<SurveyQues> lsQA = Function.getSurveyQAData(k3);
				if (lsQA != null && lsQA.Count > 0)
				{
					SurveyQues ques;
					SurveyOpt opt, subOpt;
					bool hasSubOpt = false;
					for (int i = 0; i < lsQA.Count; i++)
					{
						ques = lsQA[i];

						if (ques.Type == QuesType.checkbox)
						{
							hasSubOpt = ques.Opts.Any(p => p.SubOpts != null && p.SubOpts.Count > 0);
							if (hasSubOpt)
							{
								for (int j = 0; j < ques.Opts.Count; j++)
								{
									opt = ques.Opts[j];
									if (opt.SubOpts == null || opt.SubOpts.Count == 0) continue;
									iLen = opt.SubOpts.Count;

									sheet.GetRow(0).CreateCell(iCellIndex).SetCellValue(ques.Name + "【" + opt.Name + "】");
									sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, iCellIndex, iCellIndex + iLen));

									for (int jj = 0; jj < opt.SubOpts.Count; jj++)
									{
										subOpt = opt.SubOpts[jj];
										sheet.GetRow(1).CreateCell(iCellIndex).SetCellValue(subOpt.Name);
										iCellIndex++;

										lsVal.Add(subOpt.Val);

										if (subOpt.hasOth)
										{
											sheet.GetRow(1).CreateCell(iCellIndex).SetCellValue(subOpt.Name + "說明");
											iCellIndex++;
											iLen++;

											lsVal.Add(subOpt.Val + "_DESC");
										}
									}
								}
							}
							else
							{
								iLen = ques.Opts.Count;
								for (int j = 0; j < ques.Opts.Count; j++)
								{
									opt = ques.Opts[j];
									if (j == 0)
									{
										sheet.GetRow(0).CreateCell(iCellIndex).SetCellValue(ques.Name);
										sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, iCellIndex, iCellIndex + iLen));
									}
									sheet.GetRow(1).CreateCell(iCellIndex).SetCellValue(opt.Name);
									iCellIndex++;

									lsVal.Add(opt.Val);

									if (opt.hasOth)
									{
										sheet.GetRow(1).CreateCell(iCellIndex).SetCellValue(opt.Name + "說明");
										iCellIndex++;
										iLen++;

										lsVal.Add(opt.Val + "_DESC");
									}
								}
							}
						}
						else if (ques.Type == QuesType.radio)
						{
							sheet.GetRow(0).CreateCell(iCellIndex).SetCellValue(ques.Name);

							int idx = 1;
							StringBuilder sb = new StringBuilder();
							//foreach (string optName in ques.Opts.OrderByDescending(p => p.Val).Select(p => p.Name))
							foreach (string optName in ques.Opts.Select(p => p.Name))
							{
								sb.AppendFormat("({0}){1}{2}", idx, optName, Environment.NewLine);
								idx++;
							}
							sheet.GetRow(1).CreateCell(iCellIndex).SetCellValue(sb.ToString());

							iCellIndex++;

							//lsVal.Add(string.Join(",", ques.Opts.OrderByDescending(p => p.Val).Select(p => p.Val).ToArray()));
							lsVal.Add(string.Join(",", ques.Opts.Select(p => p.Val).ToArray()));
						}
						else if (ques.Type == QuesType.textarea)
						{
							sheet.GetRow(0).CreateCell(iCellIndex).SetCellValue("其他建議");
							sheet.GetRow(1).CreateCell(iCellIndex).SetCellValue("");
							iCellIndex++;

							lsVal.Add("OTHER_DESC");
						}
					}
				}

				#endregion

				#region 原始資料-內容
				SelectList slAge = Function.getSurveyAge();
				SelectList slLoc = Function.getSurveyLoc();
				SelectList slLocX = Function.getSurveyLocX();

				iRowIndex = 2;
				//iCellIndex = 18;
				//row = sheet.CreateRow(iRowIndex) as HSSFRow;
				//for (int i = 0; i < lsVal.Count; i++)
				//{
				//	row.CreateCell(iCellIndex).SetCellValue(lsVal[i]);
				//	iCellIndex++;
				//}
				//iRowIndex++;

				int iDataIndex = 1;
				List<SurveyModel> lsAns = JsonConvert.DeserializeObject<List<SurveyModel>>(jsonData);
				if (lsAns != null && lsAns.Count > 0)
				{
					foreach (KeyValuePair<string, string> kvp in lsAns.Select(p => new { p.DATA_TYPE, p.STATUS })
						.Distinct().ToDictionary(p => p.STATUS, p => p.DATA_TYPE))
					{
						string sNAME = "-", sTEAM = "-", sTYPE1 = "-", sTYPE2 = "-", sTYPE2_OTH = "-", sENTRY = "-";
						if (kvp.Value.CheckStringValue("hall3")) //桃園光影文化館
						{
							DATA1 d1 = iDB.GetByIDAsNoTracking<DATA1>(kvp.Key);
							if (d1 != null)
							{
								sNAME = d1.CONTENT1;
							}
						}
						else
						{
							DATA2 d2 = iDB.GetByIDAsNoTracking<DATA2>(kvp.Key);
							if (d2 != null)
							{
								sNAME = d2.CONTENT7;
								sTEAM = d2.CONTENT1;

								//辦理類型
								sTYPE1 = d2.CONTENT2.IsNullOrEmpty() ? "-" : ((HostingType)d2.CONTENT2.ToInt()).GetDescription();

								//演出類型
								sTYPE2 = Function.GetNodeTitle(d2.CONTENT6);

								//入場方式
								SelectList slEntry = Function.getSurveyEntry();
								sENTRY = d2.CONTENT15.IsNullOrEmpty() ? "-" : slEntry.FirstOrDefault(p => d2.CONTENT15.StartsWith(p.Value)).Text;
							}
						}

						foreach (SurveyModel ans in lsAns.Where(p => p.STATUS.Equals(kvp.Key)))
						{
							row = sheet.CreateRow(iRowIndex) as HSSFRow;
							row.CreateCell(0).SetCellValue(iDataIndex);

							//演出基本資訊
							row.CreateCell(1).SetCellValue(Function.GetNodeTitle(ans.DATA_TYPE));
							row.CreateCell(2).SetCellValue(ans.DATE_SELECT);
							row.CreateCell(3).SetCellValue(ans.CREATE_DATE.ToDateTimeString());
							row.CreateCell(4).SetCellValue(sNAME);
							row.CreateCell(5).SetCellValue(sTEAM);
							row.CreateCell(6).SetCellValue(sTYPE1);
							row.CreateCell(7).SetCellValue(sTYPE2);
							row.CreateCell(8).SetCellValue(sTYPE2_OTH);
							row.CreateCell(9).SetCellValue(sENTRY);

							//觀眾基本資訊
							row.CreateCell(10).SetCellValue(ans.CONTENT1);
							row.CreateCell(11).SetCellValue(ans.CONTENT2.IsNullOrEmpty() ? "" : ((SexType)ans.CONTENT2.ToInt()).GetDescription());
							row.CreateCell(12).SetCellValue(ans.CONTENT3);
							row.CreateCell(13).SetCellValue(ans.CONTENT4.IsNullOrEmpty() ? "" : slLoc.FirstOrDefault(p => p.Value.CheckStringValue(ans.CONTENT4)).Text);
							row.CreateCell(14).SetCellValue(ans.CONTENT5.IsNullOrEmpty() ? "" : slLocX.FirstOrDefault(p => p.Value.CheckStringValue(ans.CONTENT5)).Text);
							row.CreateCell(15).SetCellValue(ans.CONTENT6);
							row.CreateCell(16).SetCellValue(ans.CONTENT7.IsNullOrEmpty() ? "" : slAge.FirstOrDefault(p => p.Value.CheckStringValue(ans.CONTENT7)).Text);
							row.CreateCell(17).SetCellValue(ans.CREATER.CheckStringValue("admin") ? "紙本" : "電子");

							//QA
							IEnumerable<SurveyOpt> lsCheckBoxAndTextArea = ans.Ques.Where(p => p.Opts != null)
								.SelectMany(p => p.Opts).Where(p => !p.Ans.IsNullOrEmpty() || !p.Other.IsNullOrEmpty());

							IEnumerable<SurveyQues> lsRadio = ans.Ques.Where(p => !p.Ans.IsNullOrEmpty());

							iCellIndex = 18;
							SurveyOpt ansCheckBoxAndTextArea;
							SurveyQues ansRadio;
							string optVal;
							for (int i = 0; i < lsVal.Count; i++)
							{
								optVal = lsVal[i] ?? string.Empty;
								if (optVal.EndsWith("_DESC")) continue;

								ansCheckBoxAndTextArea = lsCheckBoxAndTextArea == null ? null :
									lsCheckBoxAndTextArea.FirstOrDefault(p => p.Val.CheckStringValue(optVal));

								ansRadio = lsRadio == null ? null : lsRadio.FirstOrDefault(p => optVal.IndexOf(p.Ans) != -1);

								if (ansRadio != null)
								{
									List<string> lsOptVal = optVal.Split(',').ToList();
									row.CreateCell(iCellIndex).SetCellValue(lsOptVal.IndexOf(ansRadio.Ans) + 1);
								}
								else if (ansCheckBoxAndTextArea != null)
								{
									row.CreateCell(iCellIndex).SetCellValue(1);
									if (sheet.GetRow(1).GetCell(iCellIndex + 1).StringCellValue.CheckStringValue("其他說明"))
									{
										iCellIndex++;
										row.CreateCell(iCellIndex).SetCellValue(ansCheckBoxAndTextArea.Other);
									}
								}
								else
								{
									row.CreateCell(iCellIndex).SetCellValue("");
									if (sheet.GetRow(1).GetCell(iCellIndex + 1).StringCellValue.CheckStringValue("其他說明"))
									{
										iCellIndex++;
										row.CreateCell(iCellIndex).SetCellValue("");
									}
								}
								iCellIndex++;
							}
							SurveyQues optTextarea = ans.Ques.FirstOrDefault(p => p.Type == QuesType.textarea);
							if (optTextarea != null)
							{
								row.CreateCell(sheet.GetRow(1).LastCellNum - 1).SetCellValue(optTextarea.Ans);
							}
							iRowIndex++;
							iDataIndex++;
						}
					}
				}

				#endregion

				//套用格式
				int iLastRowNum = sheet.LastRowNum + 1;
				for (int i = 0; i < iLastRowNum; i++)
				{
					HSSFRow rowX = sheet.GetRow(i) as HSSFRow;
					if (rowX == null)
						rowX = sheet.CreateRow(i) as HSSFRow;
					for (int j = 0; j < sheet.GetRow(1).LastCellNum; j++)
					{
						HSSFCell cell = rowX.GetCell(j) as HSSFCell;
						if (cell == null)
							cell = rowX.CreateCell(j) as HSSFCell;
						if (i >= 0 && i < 2)
						{
							cell.CellStyle = styleHeader;
						}
						else
						{
							cell.CellStyle = NormalLeftBorder;
						}
					}
				}

				for (int j = 0; j < lsW.Count; j++)
				{
					sheet.SetColumnWidth(j, lsW[j] * 256);
				}

				workbook.Write(ms);
				workbook = null;
				return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss_") + sFileName + ".xls");
			}
		}
	}
}