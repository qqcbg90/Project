using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using System.Web.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace admin.Controllers
{
	public class VehicleController : BaseController
	{
		/// <summary>
		/// 公務車申請
		/// </summary>
		public const string fun05 = "fun05";
		/// <summary>
		/// 首頁>公務車審核
		/// </summary>
		public const string fun06 = "fun06";
		/// <summary>
		/// 公務車管理>公務車審核
		/// </summary>
		public const string fun12_03 = "fun12_03";
		/// <summary>
		/// 公務車審核
		/// </summary>
		bool IsAudit = false;
		/// <summary>
		/// 初始值
		/// </summary>
		public VehicleController()
		{
			IsSuccessful = true;
		}

		#region 公務車項目管理
		/// <summary>
		/// 公務車項目管理
		/// </summary>
		public ActionResult VehicleIndex(int? page, int? defaultPage, string k)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAllAsNoTracking<NODE>(MAIN_ID: Function.CORPORATE_FLEET_VEHICLE)
				.Where(p => (string.IsNullOrEmpty(k) || p.TITLE.Contains(k))).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 公務車項目管理 - 新增/編輯
		/// </summary>
		public ActionResult VehicleEdit(string id, int? page, int? defaultPage, string k)
		{
			VehicleModel model = new VehicleModel();
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
					model = new VehicleModel()
					{
						ID = n.ID,
						TITLE = n.TITLE,
						URL = n.URL,
						UPDATE_DATE = n.UPDATE_DATE
					};
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult VehicleEdit(string id, VehicleModel model, int? page, int? defaultPage, string k)
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
						PARENT_ID = Function.CORPORATE_FLEET_VEHICLE,
						CREATER = User.Identity.Name
					};
				}
				else
				{
					n.UPDATER = User.Identity.Name;
				}
				n.TITLE = model.TITLE;
				n.URL = model.URL;
				n.UPDATE_DATE = model.UPDATE_DATE;
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<NODE>(n);
					AlertMsg = Function.DEFAULT_ADD_MESSAGE;
				}
				else
				{
					iDB.Save();
					AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
				}
				if (IsSuccessful)
				{
					UpdateNodeList();
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), "VehicleIndex");
				}
			}
			SetModelStateError();
			return View(model);
		}

		/// <summary>
		/// 公務車項目管理 - 刪除
		/// </summary>
		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult VehicleDelete(string id, int? page, int? defaultPage, string k, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);

			if (iDB.GetAllAsNoTracking<ARTICLE>(MAIN_ID: fun05).Any(p => p.ARTICLE_TYPE.Equals(id)))
			{
				AlertMsg = "已有公務車申請紀錄，不可刪除!!";
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
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), "VehicleIndex");
		}
		#endregion

		#region 公務車項目管理 > 保養修配零件記錄
		public void setVehicleRepairTitle(string pid)
		{
			ViewBag.ParentID = pid;
			SetContentTitle("保養修配零件紀錄" + (pid.IsNullOrEmpty() ? "" : " - " + Function.GetNodeTitle(pid)), 2);
		}

		/// <summary>
		/// 公務車項目管理 > 保養修配零件記錄
		/// </summary>
		public ActionResult VehicleRepairIndex(string pid, int? page, int? defaultPage, string k, string start, string end, int print = 0)
		{
			if (pid.IsNullOrEmpty())
			{
				return GoIndex(NodeID, 0, Function.DEFAULT_PAGE_SIZE, k, SetRouteValue(new string[] { }), "VehicleIndex");
			}
			setVehicleRepairTitle(pid);
			ViewBag.start = start;
			ViewBag.end = end;
			DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
			DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View("VehicleRepair" + (print == 0 ? "Index" : "Print")
				, iDB.GetAllAsNoTracking<ARTICLE>(MAIN_ID: pid).Where(p => p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd)
				.ToPagedList(page.ToMvcPaging(), (print == 0 ? _defaultPage : 10000)));
		}

		/// <summary>
		/// 公務車項目管理 > 保養修配零件記錄 - 新增/編輯
		/// </summary>
		public ActionResult VehicleRepairEdit(string pid, string id, int? page, int? defaultPage, string k, string start, string end)
		{
			if (pid.IsNullOrEmpty())
			{
				return GoIndex(NodeID, 0, Function.DEFAULT_PAGE_SIZE, k, SetRouteValue(new string[] { }), "VehicleIndex");
			}
			setVehicleRepairTitle(pid);
			VehicleRepairModel model = new VehicleRepairModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				model.DATETIME1 = DateTime.Now;
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
				if (a != null)
				{
					model = new VehicleRepairModel()
					{
						ID = a.ID,
						NODE_ID = a.NODE_ID,
						CONTENT1 = a.CONTENT1,
						CONTENT2 = a.CONTENT2,
						CONTENT3 = a.CONTENT3,
						DECIMAL1 = a.DECIMAL1.ToInt(),
						DECIMAL2 = a.DECIMAL2.ToInt(),
						DECIMAL3 = a.DECIMAL3.ToInt(),
						DATETIME1 = a.DATETIME1.Value
					};
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult VehicleRepairEdit(string pid, string id, VehicleRepairModel model, int? page, int? defaultPage, string k, string start, string end)
		{
			if (pid.IsNullOrEmpty())
			{
				return GoIndex(NodeID, 0, Function.DEFAULT_PAGE_SIZE, k, SetRouteValue(new string[] { }), "VehicleIndex");
			}
			setVehicleRepairTitle(pid);
			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				ARTICLE a = iDB.GetByID<ARTICLE>(id);
				if (a == null)
				{
					a = new ARTICLE()
					{
						ORDER = 0,
						NODE_ID = pid,
						ARTICLE_TYPE = Function.VEHICLE_REPAIR,
						CREATER = User.Identity.Name
					};
				}
				else
				{
					a.UPDATER = User.Identity.Name;
					a.UPDATE_DATE = DateTime.Now;
				}
				a.CONTENT1 = model.CONTENT1;
				a.CONTENT2 = model.CONTENT2;
				a.CONTENT3 = model.CONTENT3;
				a.DECIMAL1 = model.DECIMAL1;
				a.DECIMAL2 = model.DECIMAL2;
				a.DECIMAL3 = model.DECIMAL3;
				a.DATETIME1 = model.DATETIME1;
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<ARTICLE>(a);
					AlertMsg = Function.DEFAULT_ADD_MESSAGE;
				}
				else
				{
					iDB.Save();
					AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
				}
				if (IsSuccessful)
				{
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "pid", "start", "end" }), "VehicleRepairIndex");
				}
			}
			SetModelStateError();
			return View(model);
		}

		/// <summary>
		/// 公務車項目管理 > 保養修配零件記錄 - 刪除
		/// </summary>
		[ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult VehicleRepairDelete(string pid, string id, int? page, int? defaultPage, string k, string start, string end, bool really = false)
		{
			if (pid.IsNullOrEmpty())
			{
				return GoIndex(NodeID, 0, Function.DEFAULT_PAGE_SIZE, k, SetRouteValue(new string[] { }), "VehicleIndex");
			}
			CheckAuthority(Authority_Right.Delete);

			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				ARTICLE a = iDB.GetByID<ARTICLE>(id, false);
				if (a != null)
				{
					a.CONTENT20 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			if (!iDB.Delete<ARTICLE>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "pid", "start", "end" }), "VehicleRepairIndex");
		}
		#endregion

		#region 公務車申請/公務車審核
		[AuditStatusSelect]
		[NodeSelect(new string[] { "H", Function.CORPORATE_FLEET_VEHICLE })]
		public ActionResult Index(int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
		{
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.start = start;
			ViewBag.end = end;
			DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
			DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);
			int iK1 = k1.ToInt();

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAllAsNoTracking<ARTICLE>(MAIN_ID: fun05)
				.Where(p => (string.IsNullOrEmpty(k) || p.CONTENT12.Contains(k)) &&
				 (string.IsNullOrEmpty(k1) || p.ORDER == iK1) && (string.IsNullOrEmpty(k2) || p.ARTICLE_TYPE.Equals(k2)) &&
				 (p.DATETIME1 >= tmpStart && p.DATETIME2 < tmpEnd)).OrderByDescending(p => p.CREATE_DATE)
				.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 公務車申請/公務車審核 - 新增/編輯
		/// </summary>
		[AuditStatusSelect]
		[NodeSelect(new string[] { "H", Function.CORPORATE_FLEET_VEHICLE })]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, int print = 0)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			VehicleApplyModel model = new VehicleApplyModel();
			if (IsAdd)
			{
				CheckAuthority(Authority_Right.Add);
				model.DATETIME1 = DateTime.Now;
				model.DATETIME2 = model.DATETIME1.AddHours(2);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
				if (a != null)
				{
					model = new VehicleApplyModel()
					{
						ID = a.ID,
						NODE_ID = a.NODE_ID,
						CREATE_DATE = a.CREATE_DATE,
						CREATER = a.CREATER,
						ARTICLE_TYPE = a.ARTICLE_TYPE,
						ORDER = a.ORDER,
						CONTENT1 = a.CONTENT1,
						CONTENT2 = a.CONTENT2,
						CONTENT3 = a.CONTENT3,
						CONTENT4 = a.CONTENT4,
						CONTENT5 = a.CONTENT5,
						CONTENT10 = a.CONTENT10,
						CONTENT11 = a.CONTENT11,
						CONTENT12 = a.CONTENT12,
						CONTENT13 = Function.GetSysUserName(a.CONTENT13),
						CONTENT14 = Function.GetSysUserName(a.CONTENT14),
						CONTENT15 = Function.GetSysUserName(a.CONTENT15),
						CONTENT16 = Function.GetSysUserName(a.CONTENT16),
						DECIMAL1 = a.DECIMAL1.ToInt(),
						DATETIME1 = a.DATETIME1.Value,
						DATETIME2 = a.DATETIME2.Value
					};
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
		[NodeSelect(new string[] { "H", Function.CORPORATE_FLEET_VEHICLE })]
		[ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, VehicleApplyModel model, int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				ARTICLE a = iDB.GetByID<ARTICLE>(id);
				if (a == null)
				{
					a = new ARTICLE()
					{
						ID = getVehicleApplyID(fun05),
						ORDER = AuditStatus.Type0.ToInt() /*待簽核*/,
						NODE_ID = fun05,
						ARTICLE_TYPE = Function.VEHICLE_REPAIR,
						CREATER = User.Identity.Name
					};
					SYSUSER user = Function.SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(a.CREATER));
					if (user != null)
					{
						a.CONTENT11 = user.CONTENT2;
						a.CONTENT12 = user.NAME;
					}
				}
				else
				{
					a.UPDATER = User.Identity.Name;
					a.UPDATE_DATE = DateTime.Now;
					model.CREATE_DATE = a.CREATE_DATE;
				}
				a.ARTICLE_TYPE = model.ARTICLE_TYPE;
				a.CONTENT1 = model.CONTENT1;
				a.CONTENT2 = model.CONTENT2;
				a.CONTENT3 = model.CONTENT3;
				a.CONTENT4 = model.CONTENT4;
				a.CONTENT5 = model.CONTENT5;
				a.DECIMAL1 = model.DECIMAL1;
				a.DATETIME1 = model.DATETIME1;
				a.DATETIME2 = model.DATETIME2;
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<ARTICLE>(a);
					if (IsSuccessful)
					{
						List<string> lsTo = new List<string>();
						DataTable dt = new DataTable();
						using (DBEntities db = new DBEntities())
						{
							string SqlStr = @"SELECT X.[USER_ID], Z.NAME, Z.EMAIL, Z.CONTENT1 AS HALL_NAME, Z.CONTENT2 AS UNIT_NAME
, CONVERT(VARCHAR(100), (SELECT ROLE_GROUP_ID + ';' FROM ROLE_USER_MAPPING Y WHERE Y.[USER_ID] = X.[USER_ID] FOR XML PATH('')))  AS ROLE_GROUPs
FROM ROLE_USER_MAPPING X
JOIN SYSUSER Z ON X.[USER_ID] = Z.[USER_ID]
--WHERE X.ROLE_GROUP_ID LIKE 'VehicleAuditors_'
WHERE (X.ROLE_GROUP_ID = 'VehicleAuditorsA' OR X.ROLE_GROUP_ID = 'VehicleAuditorsD')
GROUP BY X.[USER_ID], Z.NAME, Z.EMAIL, Z.CONTENT1, Z.CONTENT2";
							lsTo = Function.getDataTable(db, SqlStr).AsEnumerable().Select(p => p.Field<string>("EMAIL")).ToList();
						}
						if (lsTo.Any())
						{
							string sBody = "您好，申請人 {0} 提出公務車申請，請前往 <a href=\"{1}\" target=\"_blank\">{2}</a> 進行審核作業。";
							foreach (string To in lsTo)
							{
								Function.SendMail(new LetterModel
								{
									RecipientList = new List<string> { To },
									Subject = string.Format("公務車審核通知 申請單號：{0}", a.ID),
									Body = string.Format(sBody, Function.GetSysUserName(a.CREATER), Function.DEFAULT_ADMIN_HTTP, Function.PAGE_TITLE)
								});
							}
						}
					}
					AlertMsg = Function.DEFAULT_ADD_MESSAGE;
				}
				else
				{
					IsAudit = NodeID.CheckStringValue(fun06) || NodeID.CheckStringValue(fun12_03);
					if (IsAudit)
					{
						string sAuditor = User.Identity.Name;
						List<string> lsRUM = iDB.GetAllAsNoTracking<ROLE_USER_MAPPING>()
							.Where(p => p.ROLE_GROUP_ID.StartsWith("VehicleAuditors") && p.USER_ID.Equals(sAuditor)).Select(p => p.ROLE_GROUP_ID).ToList();
						if (lsRUM.Any())
						{
							if (lsRUM.Any(p => p.CheckStringValue("VehicleAuditorsA")))
							{
								a.CONTENT13 = sAuditor;
							}
							if (lsRUM.Any(p => p.CheckStringValue("VehicleAuditorsB")))
							{
								a.CONTENT14 = sAuditor;
							}
							if (lsRUM.Any(p => p.CheckStringValue("VehicleAuditorsC")))
							{
								a.CONTENT15 = sAuditor;
							}
							if (lsRUM.Any(p => p.CheckStringValue("VehicleAuditorsD")))
							{
								a.CONTENT16 = sAuditor;
							}
							if (a.ORDER != model.ORDER)
							{
								a.ORDER = model.ORDER; //審核狀態

								string sBody = string.Empty;
								int iAuditStatus_Pass = AuditStatus.Type1.ToInt();
								if (a.ORDER == iAuditStatus_Pass)
								{
									a.CONTENT10 = string.Empty; //駁回原因
									sBody = "{0} 您好，您提出的公務車申請，審核「通過」，請前往 <a href =\"{1}\" target=\"_blank\">{2}</a> 列印申請單。";

									if (iDB.GetAllAsNoTracking<ARTICLE>(MAIN_ID: fun05).Any(p => p.ORDER == iAuditStatus_Pass && !p.ID.Equals(a.ID) && p.ARTICLE_TYPE.Equals(a.ARTICLE_TYPE) &&
									((a.DATETIME1 >= p.DATETIME1 && a.DATETIME1 <= p.DATETIME2) || (a.DATETIME2 >= p.DATETIME1 && a.DATETIME2 <= p.DATETIME2))))
									{
										SetModelStateError("此公務車的使用時間和其他申請單時間重疉!!");
										return View(model);
									}
								}
								else if (a.ORDER == AuditStatus.Type2.ToInt())
								{
									a.CONTENT10 = model.CONTENT10; //駁回原因
									sBody = "{0} 您好，您提出的公務車申請，審核「不通過」，原因：" + a.CONTENT10 + "，請前往 <a href =\"{1}\" target=\"_blank\">{2}</a> 重新申請。";
								}
								if (!sBody.IsNullOrEmpty())
								{
									string To = Function.GetSysUserEmail(a.CREATER);
									Function.SendMail(new LetterModel
									{
										RecipientList = new List<string> { To },
										Subject = string.Format("公務車申請審核結果通知 申請單號：{0}", a.ID),
										Body = string.Format(sBody, Function.GetSysUserName(a.CREATER), Function.DEFAULT_ADMIN_HTTP, Function.PAGE_TITLE)
									});
								}
							}
						}
						else
						{
							//無審核權限
							SetModelStateError("非公務車審核者，請至「公務車管理＞公務車審核者管理」設定!!");
							return View(model);
						}
					}
					else
					{
						if (a.ORDER == AuditStatus.Type2.ToInt())
						{
							a.ORDER = AuditStatus.Type0.ToInt(); /*待簽核*/
						}
					}
					iDB.Save();
					AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
				}
				if (IsSuccessful)
				{
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }));
				}
			}
			SetModelStateError();
			return View(model);
		}

		/// <summary>
		/// 建立公務車申請單編號
		/// </summary>
		public string getVehicleApplyID(string nid)
		{
			string sDate = DateTime.Now.ToString("yyyyMMdd");
			string sApplyID = iDB.GetAllAsNoTracking<ARTICLE>(false, nid).Where(p => p.ID.StartsWith(sDate)).Select(p => p.ID).OrderByDescending(p => p).FirstOrDefault();
			if (sApplyID.IsNullOrEmpty())
			{
				return sDate + "01";
			}
			else
			{
				return sDate + (sApplyID.Replace(sDate, "").ToInt() + 1).ToString("00");
			}
		}

		/// <summary>
		/// 公務車管理 - 刪除
		/// </summary>
		[ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);

			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				ARTICLE a = iDB.GetByID<ARTICLE>(id, false);
				if (a != null)
				{
					a.CONTENT20 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			if (!iDB.Delete<ARTICLE>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
		}
		#endregion

		#region 公務車申請 > 行車前檢查紀錄
		public ActionResult addBefore(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
		{
			SetContentTitle("行車前檢查紀錄", 2);
			ViewBag.IsAdd = true;

			BeforeDrivingModel model = new BeforeDrivingModel();
			ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
			if (a != null)
			{
				ARTICLE_PLUG ap = a.ARTICLE_PLUG.FirstOrDefault(p => p.ARTICLE_PLUG_TYPE.Equals(Function.BEFORE_DRIVING));
				if (ap == null)
				{
					CheckAuthority(Authority_Right.Add);
					model.ARTICLE_ID = a.ID;
					model.CONTENT1 = Function.GetNodeTitle(a.ARTICLE_TYPE);
				}
				else
				{
					ViewBag.IsAdd = false;
					SetIsEdit(IsAuthority(Authority_Right.Update));
					model = new BeforeDrivingModel()
					{
						ID = ap.ID,
						ARTICLE_ID = a.ID,
						CREATE_DATE = ap.CREATE_DATE,
						CREATER = ap.CREATER,
						CONTENT1 = Function.GetNodeTitle(ap.CONTENT1)
					};
					string[] arrC2 = ap.CONTENT2.Split(Function.DELIMITER);
					model.CONTENT2_1 = arrC2[0];
					model.CONTENT2_2 = arrC2[1];

					string[] arrC3 = ap.CONTENT3.Split(Function.DELIMITER);
					model.CONTENT3_1 = arrC3[0];
					model.CONTENT3_2 = arrC3[1];

					string[] arrC4 = ap.CONTENT4.Split(Function.DELIMITER);
					model.CONTENT4_1 = arrC4[0];
					model.CONTENT4_2 = arrC4[1];

					string[] arrC5 = ap.CONTENT5.Split(Function.DELIMITER);
					model.CONTENT5_1 = arrC5[0];
					model.CONTENT5_2 = arrC5[1];

					string[] arrC6 = ap.CONTENT6.Split(Function.DELIMITER);
					model.CONTENT6_1 = arrC6[0];
					model.CONTENT6_2 = arrC6[1];

					string[] arrC7 = ap.CONTENT7.Split(Function.DELIMITER);
					model.CONTENT7_1 = arrC7[0];
					model.CONTENT7_2 = arrC7[1];
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 2, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult addBefore(string id, BeforeDrivingModel model, int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
		{
			SetContentTitle("行車前檢查紀錄", 2);
			ViewBag.IsAdd = true;

			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				ARTICLE a = iDB.GetByID<ARTICLE>(id);
				if (a != null)
				{
					ARTICLE_PLUG ap = a.ARTICLE_PLUG.FirstOrDefault(p => p.ARTICLE_PLUG_TYPE.Equals(Function.BEFORE_DRIVING));
					if (ap == null)
					{
						ap = new ARTICLE_PLUG()
						{
							ORDER = 0,
							ARTICLE_ID = a.ID,
							ARTICLE_PLUG_TYPE = Function.BEFORE_DRIVING,
							CREATER = User.Identity.Name
						};
					}
					else
					{
						ViewBag.IsAdd = false;
						ap.UPDATER = User.Identity.Name;
						ap.UPDATE_DATE = DateTime.Now;
					}
					ap.CONTENT1 = a.ARTICLE_TYPE;
					ap.CONTENT2 = model.CONTENT2_1 + Function.DELIMITER + model.CONTENT2_2.ToMyString();
					ap.CONTENT3 = model.CONTENT3_1 + Function.DELIMITER + model.CONTENT3_2.ToMyString();
					ap.CONTENT4 = model.CONTENT4_1 + Function.DELIMITER + model.CONTENT4_2.ToMyString();
					ap.CONTENT5 = model.CONTENT5_1 + Function.DELIMITER + model.CONTENT5_2.ToMyString();
					ap.CONTENT6 = model.CONTENT6_1 + Function.DELIMITER + model.CONTENT6_2.ToMyString();
					ap.CONTENT7 = model.CONTENT7_1 + Function.DELIMITER + model.CONTENT7_2.ToMyString();
					if (ViewBag.IsAdd)
					{
						IsSuccessful = iDB.Add<ARTICLE_PLUG>(ap);
						AlertMsg = Function.DEFAULT_ADD_MESSAGE;
					}
					else
					{
						iDB.Save();
						AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
					}
					if (IsSuccessful)
					{
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
					}
				}
			}
			SetModelStateError();
			return View(model);
		}
		#endregion

		#region 公務車申請 > 行車紀錄
		public ActionResult addAfter(string id, int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
		{
			SetContentTitle("行車紀錄", 2);
			ViewBag.IsAdd = true;

			AfterDrivingModel model = new AfterDrivingModel();
			ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
			if (a != null)
			{
				ARTICLE_PLUG ap = a.ARTICLE_PLUG.FirstOrDefault(p => p.ARTICLE_PLUG_TYPE.Equals(Function.AFTER_DRIVING));
				if (ap == null)
				{
					CheckAuthority(Authority_Right.Add);
					model.ARTICLE_ID = a.ID;
					model.CONTENT1 = Function.GetNodeTitle(a.ARTICLE_TYPE);
					model.CONTENT2 = a.CONTENT2;
					model.CONTENT3 = a.CONTENT3;
					model.DATETIME1 = a.DATETIME1.Value;
					model.DATETIME2 = a.DATETIME2.Value;
				}
				else
				{
					ViewBag.IsAdd = false;
					SetIsEdit(IsAuthority(Authority_Right.Update));
					model = new AfterDrivingModel()
					{
						ID = ap.ID,
						ARTICLE_ID = a.ID,
						CREATE_DATE = ap.CREATE_DATE,
						CREATER = ap.CREATER,
						CONTENT1 = Function.GetNodeTitle(ap.CONTENT1)
					};

					model.CONTENT2 = ap.CONTENT2;
					model.CONTENT3 = ap.CONTENT3;
					model.CONTENT4 = ap.CONTENT4;

					string[] arrC5 = ap.CONTENT5.Split(Function.DELIMITER);
					model.CONTENT5_1 = arrC5[0];
					model.CONTENT5_2 = arrC5[1];

					model.CONTENT6 = ap.CONTENT6;
					model.DECIMAL1 = (ap.DECIMAL1.HasValue ? ap.DECIMAL1.ToInt() : 0);
					model.DECIMAL2 = (ap.DECIMAL2.HasValue ? ap.DECIMAL2.ToInt() : 0);
					model.DECIMAL3 = ap.DECIMAL3;
					model.DECIMAL4 = ap.DECIMAL4;
					model.DECIMAL5 = ap.DECIMAL5;
					model.DATETIME1 = ap.DATETIME1.Value;
					model.DATETIME2 = ap.DATETIME2.Value;
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 2, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult addAfter(string id, AfterDrivingModel model, int? page, int? defaultPage, string k, string k1, string k2, string start, string end)
		{
			SetContentTitle("行車紀錄", 2);
			ViewBag.IsAdd = true;

			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				ARTICLE a = iDB.GetByID<ARTICLE>(id);
				if (a != null)
				{
					ARTICLE_PLUG ap = a.ARTICLE_PLUG.FirstOrDefault(p => p.ARTICLE_PLUG_TYPE.Equals(Function.AFTER_DRIVING));
					if (ap == null)
					{
						ap = new ARTICLE_PLUG()
						{
							ORDER = 0,
							ARTICLE_ID = a.ID,
							ARTICLE_PLUG_TYPE = Function.AFTER_DRIVING,
							CREATER = User.Identity.Name
						};
					}
					else
					{
						ViewBag.IsAdd = false;
						ap.UPDATER = User.Identity.Name;
						ap.UPDATE_DATE = DateTime.Now;
					}
					ap.CONTENT1 = a.ARTICLE_TYPE;
					ap.CONTENT2 = model.CONTENT2.ToMyString();
					ap.CONTENT3 = model.CONTENT3.ToMyString();
					ap.CONTENT4 = model.CONTENT4.ToMyString();
					ap.CONTENT5 = model.CONTENT5_1 + Function.DELIMITER + model.CONTENT5_2.ToMyString();
					ap.CONTENT6 = model.CONTENT6.ToMyString();
					ap.DECIMAL1 = model.DECIMAL1;
					ap.DECIMAL2 = model.DECIMAL2;
					ap.DECIMAL3 = model.DECIMAL3;
					ap.DECIMAL4 = model.DECIMAL4;
					ap.DECIMAL5 = model.DECIMAL5;
					ap.DATETIME1 = model.DATETIME1;
					ap.DATETIME2 = model.DATETIME2;
					if (ViewBag.IsAdd)
					{
						IsSuccessful = iDB.Add<ARTICLE_PLUG>(ap);
						AlertMsg = Function.DEFAULT_ADD_MESSAGE;
					}
					else
					{
						iDB.Save();
						AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
					}
					if (IsSuccessful)
					{
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "start", "end" }));
					}
				}
			}
			SetModelStateError();
			return View(model);
		}
		#endregion

		#region 公務車審核者管理
		public ActionResult Auditors()
		{
			SetIsEdit(IsAuthority(Authority_Right.Update));
			List<ROLE_USER_MAPPING> lsRUM = new List<ROLE_USER_MAPPING>();
			List<ROLE_GROUP> lsRG = iDB.GetAllAsNoTracking<ROLE_GROUP>(false).Where(p => p.ID.StartsWith("VehicleAuditors")).ToList();
			if (lsRG != null)
			{
				lsRUM = lsRG.SelectMany(p => p.ROLE_USER_MAPPING).ToList();
			}
			return View(lsRUM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Auditors(string AUsers, string BUsers, string CUsers, string DUsers)
		{
			CheckAuthority(Authority_Right.Update);

			List<ROLE_GROUP> lsRG = iDB.GetAll<ROLE_GROUP>(false).Where(p => p.ID.StartsWith("VehicleAuditors")).ToList();
			if (lsRG != null)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>()
				{
					{ "D", DUsers }, { "C", CUsers }, { "B", BUsers }, { "A", AUsers }
				};
				foreach (ROLE_GROUP rg in lsRG)
				{
					string Users = dict.ContainsKey(rg.GROUP_TYPE) ? dict[rg.GROUP_TYPE] : string.Empty;
					if (!Users.IsNullOrEmpty())
					{
						iDB.DeleteAllRoleUserMapping(rg.ID);
						foreach (string UserID in Users.Split(','))
						{
							ROLE_USER_MAPPING mapping = new ROLE_USER_MAPPING();
							mapping.ID = Function.GetGuid();
							mapping.USER_ID = UserID;
							mapping.CREATER = User.Identity.Name;
							mapping.CREATE_DATE = DateTime.Now;
							mapping.ROLE_GROUP_ID = "VehicleAuditors" + rg.GROUP_TYPE;
							iDB.Add<ROLE_USER_MAPPING>(mapping);
						}
					}
				}
			}
			AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
			return RedirectToAction("Auditors", new { nid = NodeID });
		}
		#endregion
	}
}