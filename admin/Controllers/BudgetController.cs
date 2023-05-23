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
using System.Web.Mvc;

namespace admin.Controllers
{
	public class BudgetController : BaseController
	{
		/// <summary>
		/// 動支預算=>項目>類別>細目>說明
		/// </summary>
		public const string BUDGET_ITEM = "BUDGET_ITEM";
		/// <summary>
		/// 動支預算查詢
		/// </summary>
		public const string fun02 = "fun02";
		/// <summary>
		/// 動支預算管理>預算科目金額管理
		/// </summary>
		public const string fun10_01 = "fun10_01";
		/// <summary>
		/// 動支預算管理>動支登記
		/// </summary>
		public const string fun10_02 = "fun10_02";

		/// <summary>
		/// 建立動支預算申請單編號
		/// </summary>
		private string getBudgetID(string id)
		{
			string Today = DateTime.Now.ToString("yyyyMMdd");
			if (id.IsNullOrEmpty())
			{
				string maxID = iDB.GetAllAsNoTracking<DATA4>(false, fun10_02).Where(p => !p.ID.Contains("-") && p.ID.StartsWith(Today)).Select(p => p.ID).Max(p => p);
				if (!maxID.IsNullOrEmpty())
				{
					return Convert.ToString(Convert.ToInt64(maxID) + 1);
				}
				return Today + "001";
			}
			else //重新申請
			{
				id = id.IndexOf("-") != -1 ? id.Remove(id.IndexOf("-")) : id;
				string maxID = iDB.GetAllAsNoTracking<DATA4>(false, fun10_02).Where(p => p.ID.Contains("-") && p.ID.StartsWith(Today)).Select(p => p.ID).Max(p => p);
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

		#region 動支預算管理>預算科目金額管理
		[BudgetItemSelect]
		[YearSelect(ORDER_BY_DESC = true, MIN_YEAR = 2011, SHOW_MINGO_TEXT = true, SHOW_MINGO_VALUE = true)]
		public ActionResult CategoryReportIndex(int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4)
		{
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;
			ViewBag.k4 = k4;

			int iYear = k.ToInt();
			if (!IsPost())
			{
				iYear = DateTime.Now.Year - 1911;
				ViewBag.k = iYear;
			}
			string SelectID = k1.IsNullOrEmpty() ? string.Empty : k1;
			if (!k2.IsNullOrEmpty()) SelectID = k2;
			if (!k3.IsNullOrEmpty()) SelectID = k3;
			if (!k4.IsNullOrEmpty()) SelectID = k4;
			DataTable dt = new DataTable();
			List<ReportModel> model = new List<ReportModel>();
			string SqlStr = @";WITH tmp AS
(
	SELECT PARENT_ID, ID, URL, TITLE, CONTENT1, convert(nvarchar(1000),TITLE) as xTITLE, 1 as TreeLevel
	, convert(nvarchar(1000), ID) as IDs
	FROM NODE
	WHERE  ID IN (SELECT ID FROM NODE WHERE PARENT_ID = 'BUDGET_ITEM' AND [ENABLE] = '1') AND [ENABLE] = '1'
	AND [ORDER] = @Y
	UNION ALL
	SELECT n.PARENT_ID, n.ID, n.URL, n.TITLE, n.CONTENT1, convert(nvarchar(1000), (p.xTITLE + ' > ' + n.TITLE)) as xTITLE, TreeLevel + 1 as TreeLevel
	, convert(nvarchar(1000), (p.IDs + ';' + n.ID)) as IDs
	FROM NODE n
	JOIN tmp p ON n.PARENT_ID = p.ID AND n.[ENABLE] = '1'
)
SELECT ID, IDs, TreeLevel
, (CASE TreeLevel WHEN 1 THEN 'G' WHEN 2 THEN 'C' WHEN 3 THEN 'I' WHEN 4 THEN 'E' END) as ROW_TYPE
, (CASE TreeLevel WHEN 2 THEN convert(varchar(10), (TreeLevel - 1)) WHEN 1 THEN t.TITLE ELSE '' END) as GROUP_NAME
, (CASE TreeLevel WHEN 2 THEN t.TITLE ELSE '' END) as CATEGORY_NAME
, (CASE TreeLevel WHEN 3 THEN t.TITLE ELSE '' END) as ITEM_NAME
, (CASE TreeLevel WHEN 4 THEN t.TITLE ELSE '' END) as EXPLAIN_NAME
, CONVERT(int, t.CONTENT1) AS BUDGET
FROM tmp t 
WHERE IDs LIKE '%' + @SelectID + '%'
ORDER BY t.xTITLE";
			using (DBEntities db = new DBEntities())
			{
				dt = Function.getDataTable(db, SqlStr, new SqlParameter("Y", iYear), new SqlParameter("SelectID", SelectID));
			}

			model = dt.AsEnumerable().Select(p => new ReportModel()
			{
				CONTENT1 = p.Field<string>("ID"),
				CONTENT2 = p.Field<string>("ROW_TYPE"),
				CONTENT3 = p.Field<string>("GROUP_NAME"), //組別
				CONTENT4 = p.Field<string>("CATEGORY_NAME"), //類別
				CONTENT5 = p.Field<string>("ITEM_NAME"), //細目
				CONTENT6 = p.Field<string>("EXPLAIN_NAME"), //說明
				CONTENT20 = p.Field<string>("IDs"), //完整ID
				DECIMAL1 = p.Field<int>("BUDGET"), //預算數
				DECIMAL19 = p.Field<int>("TreeLevel"), //層級
			}).ToList();
			return View(model);
		}
		#endregion

		#region 動支預算管理>預算科目金額管理>科目管理
		[YearSelect(ORDER_BY_DESC = true, MIN_YEAR = 2011, SHOW_MINGO_TEXT = true, SHOW_MINGO_VALUE = true)]
		public ActionResult CategoryIndex(int? page, int? defaultPage, string k, string y, string pre, string cur, bool all = true)
		{
			if (y.IsNullOrEmpty())
			{
				y = (DateTime.Now.Year - 1911).ToString();
			}
			ViewBag.y = y;
			ViewBag.pre = pre; 
			ViewBag.cur = cur;
			ViewBag.all = all;
			cur = (cur.IsNullOrEmpty() ? (pre.IsNullOrEmpty() ? BUDGET_ITEM : pre) : cur);
			if (cur.CheckStringValue(BUDGET_ITEM))
			{
				ViewBag.Sup = string.Empty;
			}
			else
			{
				NODE n = Function.GetNode(cur);
				if (n != null)
				{
					ViewBag.pre = n.PARENT_ID;
					ViewBag.Sup = string.Format("上層選單：<span style=\"color:green;margin-right:15px;\">{0}</span>"
						, GetFullParentNodeText(n.ID));
				}
			}

			IQueryable<NODE> model = iDB.GetAllAsNoTracking<NODE>(MAIN_ID: cur).OrderBy(p => p.ORDER);
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			if (all)
			{
				_defaultPage = model.Count() + 1;
			}
			page = IsPost() ? 0 : page;
			return View(model.ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		[YearSelect(ORDER_BY_DESC = true, MIN_YEAR = 2011, SHOW_MINGO_TEXT = true, SHOW_MINGO_VALUE = true)]
		public ActionResult CategoryEdit(string id, int? page, int? defaultPage, string k, string y, string pre, string cur)
		{
			ViewBag.ID = id;
			ViewBag.y = y;
			ViewBag.pre = pre;
			ViewBag.cur = cur;
			ViewBag.hasSub = false;

			cur = (cur.IsNullOrEmpty() ? BUDGET_ITEM : cur);

			BudgetSubjectModel model = new BudgetSubjectModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				model.URL = "1";
				model.PARENT_ID = cur;
				model.PARENT_TEXT = GetFullParentNodeText(cur);

				if (cur.CheckStringValue(BUDGET_ITEM))
				{
					model.ORDER = DateTime.Now.Year - 1911; //預設:今年
				}
				else
				{
					NODE nParent = Function.GetNode(cur);
					if (nParent != null)
					{
						model.ORDER = nParent.ORDER;
					}
				}
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				NODE n = iDB.GetByIDAsNoTracking<NODE>(id);
				if (n != null)
				{
					model = new BudgetSubjectModel()
					{
						ID = n.ID,
						TITLE = n.TITLE,
						PARENT_ID = n.PARENT_ID,
						PARENT_TEXT = GetFullParentNodeText(n.PARENT_ID),
						ORDER = n.ORDER,
						CONTENT1 = n.CONTENT1.ToInt(),
						URL = n.URL,
						CONTENT8 = n.CONTENT8
					};
					ViewBag.hasSub = iDB.GetAllAsNoTracking<NODE>(MAIN_ID: id).Any();
				}
			}
			return View(model);
		}

		[HttpPost]
		[YearSelect(ORDER_BY_DESC = true, MIN_YEAR = 2011, SHOW_MINGO_TEXT = true, SHOW_MINGO_VALUE = true)]
		[ActionLog(TableNameIndex = 0, Description = "新增/編輯 科目管理")]
		public ActionResult CategoryEdit(string id, int? page, int? defaultPage, string k, string y, string pre, string cur, BudgetSubjectModel model)
		{
			CheckAuthority(Authority_Right.Update);

			ViewBag.ID = id;
			ViewBag.y = y;
			ViewBag.pre = pre;
			ViewBag.cur = cur;
			ViewBag.hasSub = false;
			IsAdd = id.IsNullOrEmpty();

			cur = (cur.IsNullOrEmpty() ? BUDGET_ITEM : cur);
			model.PARENT_TEXT = GetFullParentNodeText(cur);

			#region 檢查
			if (iDB.GetAllAsNoTracking<NODE>(MAIN_ID: cur.IsNullOrEmpty() ? BUDGET_ITEM : cur).Any(p => (string.IsNullOrEmpty(id) || !p.ID.Equals(id)) && p.TITLE.Equals(model.TITLE)))
			{
				SetModelStateError("此年度預算已新增過，請勿重複新增！");
				return View(model);
			}
			if (!cur.IsNullOrEmpty() && !cur.CheckStringValue(BUDGET_ITEM))
			{
				NODE nParent = Function.GetNode(cur);
				if (nParent != null)
				{
					int pBudget = nParent.CONTENT1.ToInt(); //上層分類的預算限制
					int uBudget = iDB.GetAllAsNoTracking<NODE>(MAIN_ID: nParent.ID)
						.Where(p => (string.IsNullOrEmpty(id) || !p.ID.Equals(id))).Select(p => p.CONTENT1).ToList().Sum(p => p.ToInt()); //本層分類的預算加總
					if ((model.CONTENT1 + uBudget) > pBudget)
					{
						SetModelStateError("已達可用的年度預算限制！");
						return View(model);
					}
				}
			}
			#endregion

			NODE n = new NODE();
			if (ModelState.IsValid)
			{
				if (IsAdd)
				{
					iDB.Add<NODE>(new NODE()
					{
						CREATER = User.Identity.Name,
						PARENT_ID = model.PARENT_ID,
						TITLE = model.TITLE,
						URL = model.URL.IsNullOrEmpty() ? "0" : model.URL,
						ORDER = model.ORDER,
						CONTENT1 = model.CONTENT1.ToString(),
						CONTENT8 = model.CONTENT8,
						CONTENT9 = BUDGET_ITEM
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
						n.URL = model.URL.IsNullOrEmpty() ? "0" : model.URL;
						n.ORDER = model.ORDER;
						n.CONTENT1 = model.CONTENT1.ToString();
						n.CONTENT8 = model.CONTENT8;
					}
					iDB.Save();
				}
				UpdateNodeList();
				return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "y", "pre", "cur" }), "CategoryIndex");
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteCategory(string id, int? page, int? defaultPage, string k, string y, string pre, string cur, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			if (iDB.GetAllAsNoTracking<NODE>(MAIN_ID: id).Any() || iDB.GetAllAsNoTracking<DATA4>(MAIN_ID: fun10_02).Any(p => p.CONTENT1.Equals(id)))
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
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "y", "pre", "cur" }), "CategoryIndex");
		}
		#endregion

		#region 動支預算管理>動支登記
		[BudgetItemSelect]
		[YearSelect(ORDER_BY_DESC = true, MIN_YEAR = 2011)]
		[MonthSelect]
		public ActionResult Index(string id, string nid, int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string y, string m1, string m2)
		{
			DateTime dtNow = DateTime.Now;
			y = y.IsNullOrEmpty() ? dtNow.Year.ToString() : y;
			m1 = m1.IsNullOrEmpty() ? dtNow.Month.ToString() : m1;
			m2 = m2.IsNullOrEmpty() ? "12" : m2;

			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;
			ViewBag.k4 = k4;
			ViewBag.y = y;
			ViewBag.m1 = m1;
			ViewBag.m2 = m2;

			int iY = y.ToInt(), iM1 = m1.ToInt(), iM2 = m2.ToInt();
			DateTime tmpStart = new DateTime(iY, iM1, 1, 0, 0, 0);
			DateTime tmpEnd = (iY.ToString("0000") + "/" + iM2.ToString("00") + "/" + DateTime.DaysInMonth(iY, iM2).ToString("00") + " 23:59:59").ToDateTime();

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			IPagedList<DATA4> list = iDB.GetAllAsNoTracking<DATA4>()
				.Where(p => p.CREATE_DATE >= tmpStart && p.CREATE_DATE <= tmpEnd &&
				(string.IsNullOrEmpty(k) || p.CONTENT2.Contains(k)) &&
				(string.IsNullOrEmpty(k1) || p.CONTENT11.Equals(k1)) && (string.IsNullOrEmpty(k2) || p.CONTENT12.Equals(k2)) &&
				(string.IsNullOrEmpty(k3) || p.CONTENT13.Equals(k3)) && (string.IsNullOrEmpty(k4) || p.CONTENT1.Equals(k4)))
				.OrderBy(p => p.ID).ToPagedList(page.ToMvcPaging(), _defaultPage);

			NODE n1 = new NODE();
			foreach (DATA4 d4 in list)
			{
				n1 = Function.GetNode(d4.CONTENT11);
				d4.CONTENT1 = string.Format("{0}年 {1} > {2} > {3} > {4}", n1.ORDER, n1.TITLE
					, Function.GetNodeTitle(d4.CONTENT12), Function.GetNodeTitle(d4.CONTENT13), Function.GetNodeTitle(d4.CONTENT1));
			}
			return View(list);
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[BudgetItemSelect]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string y, string m1, string m2)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			BudgetRegModel model = new BudgetRegModel();
			if (IsAdd)
			{
				CheckAuthority(Authority_Right.Add);
				model.CREATER = User.Identity.Name;
				model.DATA_TYPE = Function.GetSysUserDept(model.CREATER);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				DATA4 d4 = iDB.GetByIDAsNoTracking<DATA4>(id);
				if (d4 != null)
				{
					model = new BudgetRegModel()
					{
						ID = d4.ID,
						CREATER = d4.CREATER,
						DATA_TYPE = d4.DATA_TYPE,
						ORDER = d4.ORDER,
						CONTENT11 = d4.CONTENT11,
						CONTENT12 = d4.CONTENT12,
						CONTENT13 = d4.CONTENT13,
						CONTENT1 = d4.CONTENT1,
						CONTENT2 = d4.CONTENT2,
						CONTENT3 = d4.CONTENT3,
						CONTENT4 = d4.CONTENT4,
						CONTENT5 = d4.CONTENT5,
						DECIMAL1 = Convert.ToInt32(d4.DECIMAL1 ?? 0), //簽會數
						DECIMAL2 = Convert.ToInt32(d4.DECIMAL2 ?? 0), //執行數
						DATETIME1 = d4.DATETIME1
					};

					//已執行數
					model.DECIMAL5 = iDB.GetAllAsNoTracking<DATA4>(MAIN_ID: fun10_02)
						.Where(p => p.CREATE_DATE < d4.CREATE_DATE && p.CONTENT1.Equals(d4.CONTENT1)).Sum(p => p.DECIMAL2).ToInt();

					NODE n1 = Function.GetNode(d4.CONTENT11);
					if (n1 != null)
					{
						NODE n4 = Function.GetNode(d4.CONTENT1);
						if (n4 != null)
						{
							model.DECIMAL3 = n4.CONTENT1.ToInt(); //預算數
							model.CONTENT1_NAME = string.Format("{0}年 {1} > {2} > {3} > {4}", n4.ORDER, n1.TITLE
								, Function.GetNodeTitle(d4.CONTENT12), Function.GetNodeTitle(d4.CONTENT13), n4.TITLE);
						}
					}
				}
				//剩餘數
				model.DECIMAL4 = (model.DECIMAL3 - model.DECIMAL5 - model.DECIMAL2).ToInt();
			}
			return View(model);
		}

		[HttpPost]
		[BudgetItemSelect]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 18, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, BudgetRegModel model, int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string y, string m1, string m2)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				DATA4 d4 = iDB.GetByID<DATA4>(id);
				if (d4 == null)
				{
					d4 = new DATA4()
					{
						ID = getBudgetID(string.Empty),
						NODE_ID = fun10_02,
						CREATER = User.Identity.Name,
						DATA_TYPE = Function.GetSysUserDept(User.Identity.Name),
						CONTENT1 = model.CONTENT1,
						CONTENT11 = model.CONTENT11,
						CONTENT12 = model.CONTENT12,
						CONTENT13 = model.CONTENT13,
					};
					NODE n = Function.GetNode(d4.CONTENT1);
					if (n != null)
					{
						d4.ORDER = n.ORDER; //年度
					}
				}
				else
				{
					d4.UPDATER = User.Identity.Name;
					d4.UPDATE_DATE = DateTime.Now;
				}
				d4.CONTENT2 = model.CONTENT2;
				d4.CONTENT3 = model.CONTENT3;
				d4.CONTENT4 = model.CONTENT4;
				d4.CONTENT5 = model.CONTENT5;
				d4.DECIMAL1 = model.DECIMAL1 ?? 0;
				d4.DECIMAL2 = model.DECIMAL2 ?? 0;
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<DATA4>(d4);
				}
				else
				{
					IsSuccessful = iDB.Save();
				}
				if (IsSuccessful)
				{
					AlertMsg = IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE;
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "k4", "y", "m1", "m2" }));
				}
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 18, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string k2, string k3, string k4, string y, string m1, string m2, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				DATA4 d4 = iDB.GetByID<DATA4>(id, false);
				if (d4 != null)
				{
					d4.CONTENT30 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			if (!iDB.Delete<DATA4>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "k2", "k3", "k4", "y", "m1", "m2" }));
		}
		#endregion

		#region 動支預算查詢
		[BudgetItemSelect]
		public ActionResult ReportIndex(int? page, int? defaultPage, string k1, string k2, string k3, DateTime? start, DateTime? end, int all = 0, int export = 0)
		{
			if (!IsPost() && export == 0)
			{
				int iYear = DateTime.Now.Year;
				start = new DateTime(iYear, 1, 1, 0, 0, 0);
				end = new DateTime(iYear, 12, 31, 23, 59, 59);
			}
			ViewBag.start = start.ToDateString();
			ViewBag.end = end.ToDateString();
			ViewBag.all = all;
			if (k1.IsNullOrEmpty())
			{
				k1 = (ViewBag.BudgetItemSelect as SelectList).FirstOrDefault().Value;
			}
			ViewBag.k1 = k1;
			ViewBag.k2 = k2;
			ViewBag.k3 = k3;

			DateTime tmpStart = new DateTime(1753, 1, 1);
			DateTime tmpEnd = new DateTime(9999, 12, 31);
			if (start.HasValue) tmpStart = start.Value;
			if (end.HasValue) tmpEnd = end.Value.AddDays(1);

			DataTable dt = new DataTable();
			List<ReportModel> model = new List<ReportModel>();
			List<ReportModel> results = new List<ReportModel>();
			string SqlStr = @";WITH tmp AS
(
	SELECT PARENT_ID, ID, URL, TITLE, CONTENT1, convert(nvarchar(1000),TITLE) as xTITLE, 1 as TreeLevel
	, CONVERT(varchar(500), ID + ';') as IDs
	FROM NODE
	WHERE ID = @ROOT_NODE_ID AND [ENABLE] = '1'
	UNION ALL
	SELECT n.PARENT_ID, n.ID, n.URL, n.TITLE, n.CONTENT1, convert(nvarchar(1000), (p.xTITLE + ' > ' + n.TITLE)) as xTITLE, TreeLevel + 1 as TreeLevel
	, CONVERT(varchar(500), IDs + n.ID + ';') as IDs
	FROM NODE n
	JOIN tmp p ON n.PARENT_ID = p.ID AND n.[ENABLE] = '1'
)
SELECT TreeLevel, t.xTITLE, t.ID, t.PARENT_ID
, (CASE TreeLevel WHEN 1 THEN 'G' WHEN 2 THEN 'C' WHEN 3 THEN 'I' WHEN 4 THEN 'E' END) as ROW_TYPE
, (CASE TreeLevel WHEN 2 THEN convert(varchar(10), (TreeLevel - 1)) WHEN 1 THEN t.TITLE ELSE '' END) as GROUP_NAME
, (CASE TreeLevel WHEN 2 THEN t.TITLE ELSE '' END) as CATEGORY_NAME
, (CASE TreeLevel WHEN 3 THEN t.TITLE ELSE '' END) as ITEM_NAME
, (CASE TreeLevel WHEN 4 THEN t.TITLE ELSE '' END) as EXPLAIN_NAME
, CONVERT(int, t.CONTENT1) AS BUDGET
, IDs
FROM tmp t ";
			if (!k3.IsNullOrEmpty())
			{
				SqlStr += "WHERE (ID = @ROOT_NODE_ID) OR (ID = @k2) OR (@k3 ='' OR CHARINDEX(@k3, IDs) > 0)";
			}
			else if (!k2.IsNullOrEmpty())
			{
				SqlStr += "WHERE (ID = @ROOT_NODE_ID) OR (@k2 ='' OR CHARINDEX(@k2, IDs) > 0)";
			}
			SqlStr += " ORDER BY t.xTITLE";
			using (DBEntities db = new DBEntities())
			{
				dt = Function.getDataTable(db, SqlStr
					, new SqlParameter("ROOT_NODE_ID", k1.IsNullOrEmpty() ? "" : k1)
					, new SqlParameter("k2", k2.IsNullOrEmpty() ? "" : k2)
					, new SqlParameter("k3", k3.IsNullOrEmpty() ? "" : k3)
					, new SqlParameter("start", tmpStart.ToDateString()), new SqlParameter("end", tmpEnd.ToDateString()));
			}

			model = dt.AsEnumerable().Select(p => new ReportModel()
			{
				CONTENT1 = p.Field<string>("ID"),
				CONTENT2 = p.Field<string>("ROW_TYPE"),
				CONTENT3 = p.Field<string>("GROUP_NAME"), //組別
				CONTENT4 = p.Field<string>("CATEGORY_NAME"), //類別
				CONTENT5 = p.Field<string>("ITEM_NAME"), //細目
				CONTENT6 = p.Field<string>("EXPLAIN_NAME"), //說明
				CONTENT7 = "", //執行細目
				CONTENT8 = "", //承辦人
				CONTENT9 = "", //廠商
				CONTENT10 = "", //動支字號
				CONTENT11 = p.Field<string>("PARENT_ID"), //
				CONTENT20 = p.Field<string>("xTITLE"), //完整路徑
				DECIMAL1 = p.Field<int>("BUDGET"), //預算數
				DECIMAL2 = 0, //簽會數
				DECIMAL3 = 0, //執行數
				DECIMAL4 = p.Field<int>("BUDGET"), //剩餘數
				DECIMAL5 = 0, //執行率
				DECIMAL19 = p.Field<int>("TreeLevel"), //層級
			}).ToList();

			if (model != null && model.Any())
			{
				int iNum = 0;
				string sNODE_ID = string.Empty;
				int iIndex = 1;
				List<DATA4> list = iDB.GetAllAsNoTracking<DATA4>(MAIN_ID: fun10_02).Where(p => p.CREATE_DATE >= tmpStart && p.CREATE_DATE < tmpEnd).OrderBy(p => p.CREATE_DATE).ToList();
				for (int i = 0; i < model.Count; i++)
				{
					sNODE_ID = string.Empty;
					ReportModel m = model[i];
					switch (m.CONTENT2)
					{
						case "C":
							m.CONTENT3 = iIndex.ToString();
							iIndex++;
							break;
						case "I":
							ReportModel m2 = model.FirstOrDefault(x => x.CONTENT2.CheckStringValue("E") && x.CONTENT20.StartsWith(m.CONTENT20));
							if (m2 != null && m.DECIMAL1 == m2.DECIMAL1)
							{
								m.CONTENT6 = m2.CONTENT6;
								iNum = Convert.ToInt32(m.DECIMAL4 ?? 0); //剩餘數
								sNODE_ID = m2.CONTENT1;
								i++;
							}
							break;
						case "E":
							iNum = Convert.ToInt32(m.DECIMAL4 ?? 0); //剩餘數
							sNODE_ID = m.CONTENT1;
							break;
					}
					results.Add(m);

					if (!sNODE_ID.IsNullOrEmpty())
					{
						foreach (DATA4 d4 in list.Where(p => p.CONTENT1.Equals(sNODE_ID)))
						{
							ReportModel m3 = new ReportModel()
							{
								CONTENT2 = "D",
								CONTENT7 = d4.CONTENT2,
								CONTENT8 = d4.CONTENT4, //承辦人
								CONTENT9 = d4.CONTENT5, //廠商
								CONTENT10 = d4.CONTENT3, //動支字號								
								DECIMAL2 = d4.DECIMAL1 ?? 0, //簽會數
								DECIMAL3 = d4.DECIMAL2 ?? 0, //執行數								
							};
							iNum -= Convert.ToInt32(m3.DECIMAL3 ?? 0);
							m3.DECIMAL4 = iNum; //剩餘數

							m.DECIMAL2 += m3.DECIMAL2; //第三層:簽會數
							m.DECIMAL3 += m3.DECIMAL3; //第三層:執行數

							results.Add(m3);
						}
						m.DECIMAL4 -= Convert.ToInt32(m.DECIMAL3 ?? 0); //剩餘數
						m.DECIMAL5 = m.DECIMAL1 == 0 ? 0 : Math.Round((m.DECIMAL3 ?? 0) / (m.DECIMAL1 ?? 0) * 100, 2); //執行率						
					}
				}
				//I 細目、C 類別、G 組別
				List<ReportModel> lsR = new List<ReportModel>();
				foreach (string sROW_TYPE in (new string[] { "I", "C", "G" }))
				{
					foreach (ReportModel m in results.Where(p => p.CONTENT2.Equals(sROW_TYPE)))
					{
						if (m.DECIMAL2 == 0 && m.DECIMAL3 == 0)
						{
							lsR = results.Where(p => !string.IsNullOrEmpty(p.CONTENT11) && p.CONTENT11.Equals(m.CONTENT1) && !p.CONTENT1.Equals(m.CONTENT1)).ToList();
							m.DECIMAL2 = lsR.Sum(p => p.DECIMAL2); //簽會數
							m.DECIMAL3 = lsR.Sum(p => p.DECIMAL3); //執行數
							m.DECIMAL4 = m.DECIMAL1 - m.DECIMAL3; //剩餘數
							m.DECIMAL5 = m.DECIMAL1 == 0 ? 0 : Math.Round((m.DECIMAL3 ?? 0) / (m.DECIMAL1 ?? 0) * 100, 2); //執行率
						}
					}
				}
			}

			if (export == 1)
			{
				return ExportExcel(results, k1, k2, k3, tmpStart, tmpEnd);
			}
			else
			{
				return View(results);
			}
		}

		public ActionResult ExportExcel(List<ReportModel> model, string k1, string k2, string k3, DateTime? start, DateTime? end)
		{
			NODE n = Function.GetNode(k1);
			string sFileName = (n != null ? n.ORDER + "年度_" : "") + "經費預算執行統計表";
			List<string> lsHeader = new List<string>() { "項目", "類別", "細目", "說明", "執行細目", "預算數", "簽會數", "執行數", "剩餘數", "執行率", "承辦人", "廠商", "動支字號" };
			List<int> lsW = new List<int>() { 5, 5, 30, 45, 75, 12, 12, 12, 12, 12, 12, 25, 20 };
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
				HSSFSheet sheet = workbook.CreateSheet(sFileName) as HSSFSheet;
				int iRowIndex = 2;
				HSSFRow row = sheet.CreateRow(iRowIndex) as HSSFRow;
				foreach (ReportModel m in model)
				{
					row = sheet.CreateRow(iRowIndex) as HSSFRow;
					switch (m.CONTENT2)
					{
						case "G":
							row.CreateCell(0).SetCellValue(m.CONTENT3);
							for (int x = 1; x <= 4; x++)
							{
								row.CreateCell(x).SetCellValue("");
							}
							sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 0, 4));
							iRowIndex++;
							break;
						case "C":
							row.CreateCell(0).SetCellValue(m.CONTENT3);
							row.CreateCell(1).SetCellValue(m.CONTENT4);
							for (int x = 2; x <= 4; x++)
							{
								row.CreateCell(x).SetCellValue("");
							}
							sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(iRowIndex, iRowIndex, 1, 4));
							break;
						default:
							row.CreateCell(0).SetCellValue(m.CONTENT3);
							row.CreateCell(1).SetCellValue(m.CONTENT4);
							row.CreateCell(2).SetCellValue(m.CONTENT5);
							row.CreateCell(3).SetCellValue(m.CONTENT6);
							row.CreateCell(4).SetCellValue(m.CONTENT7);
							break;
					}
					row.CreateCell(5).SetCellValue(Convert.ToInt32(m.DECIMAL1 ?? 0));
					row.CreateCell(6).SetCellValue(Convert.ToInt32(m.DECIMAL2 ?? 0));
					row.CreateCell(7).SetCellValue(Convert.ToInt32(m.DECIMAL3 ?? 0));
					row.CreateCell(8).SetCellValue(Convert.ToInt32(m.DECIMAL4 ?? 0));
					row.CreateCell(9).SetCellValue(Convert.ToDouble(m.DECIMAL5 ?? 0).ToString("#0.##") + "%");
					row.CreateCell(10).SetCellValue(m.CONTENT8);
					row.CreateCell(11).SetCellValue(m.CONTENT9);
					row.CreateCell(12).SetCellValue(m.CONTENT10);

					iRowIndex++;
				}
				sheet.CreateRow(0).CreateCell(0).SetCellValue(sFileName.Replace("_", " "));
				sheet.CreateRow(1).CreateCell(0).SetCellValue("統計日期：" + DateTime.Now.ToString("yyyy年MM月dd日"));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 12));
				sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 12));

				//表頭
				HSSFRow rowHeader = sheet.CreateRow(3) as HSSFRow;
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
							cell.CellStyle = i == 3 ? styleHeader : (j >= 5 && j <= 9 ? (j == 9 ? NormalRightBorder2 : NormalRightBorder) : NormalLeftBorder);
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
				rowLimit.CreateCell(0).SetCellValue("組別");
				rowLimit.CreateCell(1).SetCellValue(k1.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k1));

				rowLimit = sheetLimit.CreateRow(1) as HSSFRow;
				rowLimit.CreateCell(0).SetCellValue("類別");
				rowLimit.CreateCell(1).SetCellValue(k2.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k2));

				rowLimit = sheetLimit.CreateRow(2) as HSSFRow;
				rowLimit.CreateCell(0).SetCellValue("細目");
				rowLimit.CreateCell(1).SetCellValue(k3.IsNullOrEmpty() ? "全部" : Function.GetNodeTitle(k3));

				rowLimit = sheetLimit.CreateRow(3) as HSSFRow;
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
		#endregion
	}
}