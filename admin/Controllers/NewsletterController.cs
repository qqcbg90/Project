using admin.Filters;
using System.IO;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;
using MvcPaging;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace admin.Controllers
{
	public class NewsletterController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
		/// <summary>
		/// 群組管理
		/// </summary>
		public const string NEWSLETTER = "NEWSLETTER";
		/// <summary>
		/// 電子報發送
		/// </summary>
		public const string NEWSLETTER_SEND = "NEWSLETTER_SEND";
		/// <summary>
		/// 群組編號－電子報訂閱者
		/// </summary>
		public const string SUBSCRIBER = "SUBSCRIBER";
		/// <summary>
		/// 前台網站管理>電子報管理
		/// </summary>
		public const string fun13_07 = "fun13_07";

		#region 電子報管理
		/// <summary>
		/// 列表
		/// </summary>
		public ActionResult Index(int? page, int? defaultPage, string k, string k1, string lsID)
		{
            //DateTime startDate = DateTime.Now.Date.AddMonths(-1);
            DateTime startDate = DateTime.Now.Date.AddDays(-14);

            IQueryable<ROLE_GROUP> list = iDB.GetAll<ROLE_GROUP>(MAIN_ID: NEWSLETTER_SEND)
                .Where(p => p.CREATE_DATE >= startDate && (string.IsNullOrEmpty(k) || p.TITLE.Contains(k)));

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            return View(list.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		public ActionResult Edit(string id, int? page, int? defaultPage, string k)
		{
			ViewBag.IsAdd = id.IsNullOrEmpty();
			ViewBag.GROUPs = iDB.GetAll<ROLE_GROUP>(MAIN_ID: NEWSLETTER).OrderBy(p => p.CREATE_DATE)
				.Select(p => new SelectListItem() { Value = p.ID, Text = p.TITLE }).ToList();

			NewsletterModel model = new NewsletterModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				model.ORDER = 0;
				model.TITLE = "電子報發送";
				model.CONTENT1 = DateTime.Now.AddDays(1);
				model.CONTENT3 = DateTime.Now.ToString("yyyy/MM/01"); //月初
				model.CONTENT4 = model.CONTENT3.ToDateTime().AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd"); //月底
				model.PICs = new List<ATTACHMENT>();
				model.MEMO = new List<string>() { SUBSCRIBER };
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ROLE_GROUP rg = iDB.GetByIDAsNoTracking<ROLE_GROUP>(id);
				if (rg != null)
				{
					model = new NewsletterModel()
					{
						ID = rg.ID,
						TITLE = rg.TITLE,
						ORDER = rg.ORDER ?? 0,
						CONTENT1 = rg.CONTENT1.ToDateTime(),
						CONTENT2 = rg.CONTENT2,
						CONTENT3 = rg.CONTENT3,
						CONTENT4 = rg.CONTENT4,
						MEMO = rg.MEMO.Split(';').ToList()
					};
					if (rg.ORDER == 0)
					{
						model.PICs = new List<ATTACHMENT>();
					}
					else
					{
						string Enable = EnableType.Enable.ToIntValue();
						model.PICs = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: rg.ID).Where(p => p.CONTENT9.Equals(Enable)).OrderBy(p => p.ORDER).ToList();
					}
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 11, Description = "新增/編輯 電子報管理")]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, NewsletterModel model, string lsDel)
		{
			CheckAuthority(Authority_Right.Update);

			ViewBag.IsAdd = id.IsNullOrEmpty();
			ViewBag.GROUPs = iDB.GetAll<ROLE_GROUP>(MAIN_ID: NEWSLETTER).OrderBy(p => p.CREATE_DATE)
				.Select(p => new SelectListItem() { Value = p.ID, Text = p.TITLE }).ToList();

			if (model.ORDER == 1)
			{
				ModelState.Remove("CONTENT3");
				ModelState.Remove("CONTENT4");
			}
			else
			{
				ModelState.Remove("TITLE");
				model.TITLE = "電子報發送";
			}

			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				ROLE_GROUP rg = iDB.GetByID<ROLE_GROUP>(id);
				if (rg == null)
				{
					rg = new ROLE_GROUP()
					{
						ID = Function.GetGuid(),
						CREATER = User.Identity.Name,
					};
				}
				else
				{
					rg.UPDATER = User.Identity.Name;
					rg.UPDATE_DATE = DateTime.Now;
				}
				rg.GROUP_TYPE = NEWSLETTER_SEND;
				rg.ORDER = model.ORDER;
				rg.TITLE = model.TITLE;
				rg.CONTENT1 = model.CONTENT1.ToDateString();
				rg.MEMO = string.Join(";", model.MEMO);

				//原電子報：需設定資料抓取日期
				if (rg.ORDER == 0)
				{
					rg.TITLE = "電子報發送";
					rg.CONTENT3 = model.CONTENT3;
					rg.CONTENT4 = model.CONTENT4;
				}
				else //自訂電子報
				{
					int PIC_COUNT = 0, HPF_COUNT = 0;
					string Enable = EnableType.Enable.ToIntValue();
					List<ATTACHMENT> PICs = iDB.GetAll<ATTACHMENT>(MAIN_ID: rg.ID).Where(p => p.CONTENT9.Equals(Enable)).OrderBy(p => p.ORDER).ToList();
					if (model.PICs != null && model.PICs.Any())
					{
						PIC_COUNT = model.PICs.Count;

						ATTACHMENT pic;
						HttpPostedFileBase hpf;
						foreach (ATTACHMENT atta in model.PICs)
						{
							hpf = atta.HPF;
							pic = PICs.FirstOrDefault(p => p.ID.Equals(atta.ID));
							if (pic != null) //編輯
							{
								pic.CONTENT1 = atta.CONTENT1.ToHttpUrl(); //圖片連結
								pic.UPDATE_DATE = DateTime.Now;
								pic.UPDATER = User.Identity.Name;
							}
							else //新增
							{
								if (hpf == null || hpf.ContentLength <= 0)
								{
									continue;
								}

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
									att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: atta.MAIN_ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
									att.CONTENT9 = EnableType.Enable.ToIntValue();
									att.CONTENT1 = atta.CONTENT1.ToHttpUrl(); //圖片連結
									att.MAIN_ID = rg.ID;
									SaveAtt(hpf, att.FILE_NAME);
									iDB.Add<ATTACHMENT>(att);
									HPF_COUNT++;
								}
							}
						}
						if (!lsDel.IsNullOrEmpty())  //刪除
						{
							foreach (string attaID in PICs.Where(x => lsDel.Contains(x.ID)).Select(x => x.ID).ToList())
							{
								iDB.Delete<ATTACHMENT>(attaID, true);
							}
						}
					}

					if (PIC_COUNT + HPF_COUNT == 0)
					{
						sWarningMsg += "自訂電子報：至少 1 張圖片！";
					}
					if (!sWarningMsg.IsNullOrEmpty())
					{
						SetModelStateError(sWarningMsg);
						return View(model);
					}
				}

				if (IsAdd)
				{
					IsSuccessful = iDB.Add<ROLE_GROUP>(rg);
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
				}
				if (IsSuccessful)
				{
					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }));
				}
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 11, Description = "刪除 電子報管理")]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);

			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				ROLE_GROUP rg = iDB.GetByID<ROLE_GROUP>(id, false);
				if (rg != null)
				{
					rg.CONTENT5 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			//刪除待發送區
			iDB.ExecuteSqlCommand(string.Format("DELETE FROM PARAGRAPH WHERE MAIN_ID = '{0}';", id));
			iDB.ExecuteSqlCommand(string.Format("DELETE FROM ROLE_USER_MAPPING WHERE ROLE_GROUP_ID = '{0}';", id));

			if (!iDB.Delete<ROLE_GROUP>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }));
		}

		#endregion

		#region 群組管理
		public ActionResult SubscriberIndex(int? page, int? defaultPage, string k)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAllAsNoTracking<ROLE_GROUP>(MAIN_ID: NEWSLETTER).OrderByDescending(p => p.CREATE_DATE).ToList());
		}

		public ActionResult SubscriberGroupEdit(string id, int? page, int? defaultPage, string k, string gid)
		{
			SetContentTitle("群組管理", 2);
			ViewBag.GID = gid;

			ROLE_GROUP model = new ROLE_GROUP();
			if (gid.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				model = iDB.GetByIDAsNoTracking<ROLE_GROUP>(gid);
			}
			return View(model);
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 11, Description = "新增/編輯 電子報管理-群姐")]
		public ActionResult SubscriberGroupEdit(string id, int? page, int? defaultPage, string k, string gid, ROLE_GROUP model)
		{
			SetContentTitle("群組管理", 2);
			ViewBag.GID = gid;
			IsAdd = gid.IsNullOrEmpty();

			ROLE_GROUP rg = new ROLE_GROUP();
			if (ModelState.IsValid)
			{
				if (IsAdd)
				{
					iDB.Add<ROLE_GROUP>(new ROLE_GROUP()
					{
						CREATER = User.Identity.Name,
						GROUP_TYPE = NEWSLETTER,
						TITLE = model.TITLE,
						CONTENT1 = model.CONTENT1
					});
				}
				else
				{
					rg = iDB.GetByID<ROLE_GROUP>(gid);
					if (rg != null)
					{
						rg.UPDATER = User.Identity.Name;
						rg.UPDATE_DATE = DateTime.Now;
						rg.TITLE = model.TITLE;
						rg.CONTENT1 = model.CONTENT1;
					}
					iDB.Save();
				}
				return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id" }), "SubscriberIndex");
			}
			SetModelStateError();
			return View(model);
		}


		[ActionLog(TableNameIndex = 11, Description = "刪除 電子報管理-群組")]
		public ActionResult DeleteGroup(string id, int? page, int? defaultPage, string k, string gid, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			if (iDB.GetAllAsNoTracking<ROLE_USER_MAPPING>(MAIN_ID: gid).Any())
			{
				AlertMsg = "群組下已有資料，不可刪除!!";
			}
			else
			{
				//不是真的刪除時，記錄刪除人及刪除時間
				if (!really)
				{
					ROLE_GROUP rg = iDB.GetByID<ROLE_GROUP>(gid, false);
					if (rg != null)
					{
						rg.CONTENT5 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
					}
				}
				if (!iDB.Delete<ROLE_GROUP>(gid, really))
				{
					AlertMsg = Function.DELETE_ERROR_MESSAGE;
				}
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id" }), "SubscriberIndex");
		}
		#endregion

		#region 訂閱者管理
		public ActionResult SubscriberEdit(string id, int? page, int? defaultPage, string k, string k1, DateTime? start, DateTime? end)
		{
			ViewBag.ID = id;
			ViewBag.k1 = k1;
			ViewBag.start = start.ToDateString();
			ViewBag.end = end.ToDateString();

			ROLE_GROUP rg = iDB.GetByIDAsNoTracking<ROLE_GROUP>(id);
			if (rg != null)
			{
				SetContentTitle(rg.TITLE, 2);
			}

			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (IsPost())
			{
				if (start.HasValue) tmpStart = start.Value;
				if (end.HasValue) tmpEnd = end.Value.AddDays(1);
			}
			List<ROLE_USER_MAPPING> list = iDB.GetAllAsNoTracking<ROLE_USER_MAPPING>(MAIN_ID: id)
				.Where(p => p.CREATE_DATE >= tmpStart && p.CREATE_DATE < tmpEnd && (string.IsNullOrEmpty(k1) || p.USER_ID.Contains(k1)))
				.ToList();
			return View(list);
		}

		[ActionLog(TableNameIndex = 12, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteSubscriber(string id, string sid, int? page, int? defaultPage, string k, string k1, DateTime? start, DateTime? end)
		{
			CheckAuthority(Authority_Right.Delete);
			iDB.Delete<ROLE_USER_MAPPING>(sid, true);
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1", "start", "end" }), "SubscriberEdit");
		}
		#endregion

		#region 訂閱者管理＞新增
		public ActionResult SubscriberAdd(string id, int? page, int? defaultPage, string k, string k1, DateTime? start, DateTime? end)
		{
			ViewBag.ID = id;
			ViewBag.k1 = k1;
			ViewBag.start = start.ToDateString();
			ViewBag.end = end.ToDateString();

			ROLE_GROUP rg = iDB.GetByIDAsNoTracking<ROLE_GROUP>(id);
			if (rg != null)
			{
				SetContentTitle(rg.TITLE, 2);
			}
			return View();
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 12, Description = "新增訂閱者")]
		public ActionResult SubscriberAdd(string id, int? page, int? defaultPage, string k, string k1, DateTime? start, DateTime? end
			, HttpPostedFileBase HPF, string EMAIL)
		{
			ViewBag.ID = id;
			ViewBag.k1 = k1;
			ViewBag.start = start.ToDateString();
			ViewBag.end = end.ToDateString();

			List<string> lsERROR = new List<string>();
			List<string> lsEMAIL = new List<string>();
			ROLE_GROUP rg = iDB.GetByIDAsNoTracking<ROLE_GROUP>(id);
			if (rg != null)
			{
				SetContentTitle(rg.TITLE, 2);
				lsEMAIL = rg.ROLE_USER_MAPPING.Select(p => p.USER_ID).ToList();
			}
			int iCount = 0, iSuccess = 0, iFailed = 0;
			if (!EMAIL.IsNullOrEmpty())
			{
				if (Regex.IsMatch(EMAIL.ToMyString(), Function.EMAIL_REGEX))
				{
					if (lsEMAIL != null && !lsEMAIL.Contains(EMAIL))
					{
						iDB.Add(new ROLE_USER_MAPPING()
						{
							CREATER = "admin_add",
							USER_ID = EMAIL.ToMyString(),
							ROLE_GROUP_ID = id
						});
					}
					iSuccess++;
				}
				else
				{
					lsERROR.Add(EMAIL);
					iFailed++;
				}
				iCount++;
			}
			if (HPF != null && HPF.ContentLength > 0)
			{
				string sExt = Path.GetExtension(HPF.FileName);
				if (sExt.CheckStringValue(".txt"))
				{
					string sLine;
					using (StreamReader sr = new StreamReader(HPF.InputStream, encoding: System.Text.Encoding.UTF8))
					{
						while ((sLine = sr.ReadLine()) != null)
						{
							if (sLine.IsNullOrEmpty()) continue;
							if (Regex.IsMatch(sLine.ToMyString(), Function.EMAIL_REGEX))
							{
								if (lsEMAIL != null && !lsEMAIL.Contains(sLine))
								{
									iDB.Add(new ROLE_USER_MAPPING()
									{
										CREATER = "admin_import",
										USER_ID = sLine.ToMyString(),
										ROLE_GROUP_ID = id
									});
								}
								iSuccess++;
							}
							else
							{
								if (!lsERROR.Contains(sLine))
								{
									lsERROR.Add(sLine);
								}
								iFailed++;
							}
							iCount++;
						}
					}
				}
			}
			if (iFailed > 0)
			{
				if (iFailed == iCount)
				{
					ViewBag.ErrorMsg = "無法匯入，檔案格式不正確！";
				}
				else
				{
					ViewBag.ErrorMsg = string.Format("已匯入！總共 {0} 筆，成功 {1} 筆，失敗 {2} 筆！", iCount, iSuccess, iFailed);
					ViewBag.ErrorList = lsERROR;
				}
				return View();
			}
			else
			{
				AlertMsg = string.Format("已匯入！總共 {0} 筆！", iSuccess);
				return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1", "start", "end" }), "SubscriberEdit");
			}
		}
		#endregion

		#region 預覽電子報
		public ActionResult PreviewNewsletter(string id)
		{
			ROLE_GROUP rg = iDB.GetByIDAsNoTracking<ROLE_GROUP>(id);
			if (rg != null)
			{
				if (rg.ORDER == 0)
				{
					//最新消息
					ViewBag.News = iDB.GetAllAsNoTracking<ARTICLE>(MAIN_ID: "fun13_04")
						.Where(p => p.ORDER == 0 && (!p.DATETIME1.HasValue || p.DATETIME1 <= DateTime.Today) && (!p.DATETIME2.HasValue || p.DATETIME2 >= DateTime.Today))
						.OrderByDescending(p => p.DATETIME1).Take(5).ToList();

					DateTime tmpStart = new DateTime(1753, 1, 1);
					DateTime tmpEnd = new DateTime(9999, 12, 31);
					if (!rg.CONTENT3.IsNullOrEmpty()) tmpStart = rg.CONTENT3.ToDateTime();
					if (!rg.CONTENT4.IsNullOrEmpty()) tmpEnd = rg.CONTENT4.ToDateTime().AddDays(1);

					using (DBEntities db = new DBEntities())
					{
						string SqlStr = @"SELECT TOP 3
d2.ID
, MIN(ST) as 日期
, d2.CONTENT7 as 名稱
, d2.CONTENT3 as 館別
, d2.CONTENT4 as 場地
, d2.CONTENT1 as 展演者
, d2.CONTENT13 as 展演時間說明
FROM DATA2 d2
JOIN PLUS p ON p.MAIN_ID = d2.ID AND p.PLUS_TYPE = 'TIME' AND p.[ORDER] = 1 AND p.[ENABLE] = 1
AND d2.NODE_ID = 'fun13_05_03' AND d2.[ENABLE] = 1
CROSS APPLY dbo.fnSplitDate2Table(p.ID, p.DATETIME1, p.[DATETIME2])
WHERE d2.DECIMAL1 = 1 /*公開演出*/
AND (@C5 = '' OR d2.CONTENT5 = @C5)
AND (ST >= @Start AND ST < @End)
GROUP BY d2.ID, d2.CONTENT7, d2.CONTENT3, d2.CONTENT4, d2.CONTENT1, d2.CONTENT13
ORDER BY 2;";
						for (int i = 1; i <= 2; i++)
						{
							//performance1:表演類 performance2:展覽類
							ViewData["performance" + i] = Function.getDataTable(db, SqlStr
								, new SqlParameter("C5", "performance" + i), new SqlParameter("Start", tmpStart.ToDateString()), new SqlParameter("End", tmpEnd.ToDateString())).AsEnumerable();
						}
					}
				}
				else
				{
					ViewBag.PICs = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: rg.ID).Where(p => p.CONTENT9 == "1").ToList();
				}
			}
			return View(rg);
		}
		#endregion
	}
}