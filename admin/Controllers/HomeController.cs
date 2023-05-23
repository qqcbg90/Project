using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			return View();
		}

		#region 機關行事曆查詢

		public ActionResult CalendarIndex()
		{
			SetContentTitle("機關行事曆查詢");
			return View();
		}

		public ActionResult GetAllCalendarEvent(DateTime start, DateTime end, string k)
		{
			Dictionary<Guid, EventModel> events = new Dictionary<Guid, EventModel>();
			if (k.IsNullOrEmpty() || k.CheckStringValue("3")) //公務車預約
			{
				Dictionary<Guid, EventModel> eventV = iDB.GetAllAsNoTracking<ARTICLE>(MAIN_ID: "fun05")
					.Where(p => p.ORDER == 1 && p.DATETIME1 >= start && p.DATETIME1 < end).ToList()
					.Select(p => new EventModel()
					{
						id = p.ID,
						title = Function.GetNodeTitle(p.ARTICLE_TYPE),
						start = p.DATETIME1.Value.ToString("s"),
						end = p.DATETIME2.Value.ToString("s"),
						content = "NO_AUTH",
						backgroundColor = "#e6e6e6",
						allDay = false
					}).ToDictionary(p => new Guid(Function.GetGuid()), p => p);
				if (eventV != null && eventV.Count > 0)
				{
					events = events.Concat(eventV).ToDictionary(p => p.Key, p => p.Value);
				}
			}
			if (k.IsNullOrEmpty() || k.CheckStringValue("1") || k.CheckStringValue("2"))
			{
				string SqlStr = @"SELECT
d2.ID as id
, d2.CONTENT7 as title
, d2.CONTENT4 as location
, CONVERT(varchar(20), ST, 126) as start
, CONVERT(varchar(20), ET, 126) as [end]
, (CASE WHEN p.[ORDER] = 1 THEN '#ffd0d7' ELSE '#cee6ff' END) as backgroundColor
, p.[ORDER] as time_type
FROM DATA2 d2
JOIN PLUS p ON p.MAIN_ID = d2.ID AND p.PLUS_TYPE = 'TIME' AND p.[ENABLE] = 1 AND p.[ORDER] <= 2
AND d2.NODE_ID = 'fun13_05_03' AND d2.[ENABLE] = 1
CROSS APPLY dbo.fnSplitDate2Table(p.MAIN_ID, p.DATETIME1, p.[DATETIME2])
WHERE (ST >= @Start AND ST < @End)";
				IEnumerable<DataRow> drs;
				using (DBEntities db = new DBEntities())
				{
					drs = Function.getDataTable(db, SqlStr, new SqlParameter("Start", start.ToDateString()), new SqlParameter("End", end.ToDateString())).AsEnumerable();
				}
				if (drs.Any() && drs.Count() > 0)
				{
					if (k.IsNullOrEmpty() || k.CheckStringValue("1") || k.CheckStringValue("2")) //拆／裝台／佈展／卸展時間
					{
						Dictionary<Guid, EventModel> eventD = drs.Where(p => p.Field<int>("time_type") == 0 || p.Field<int>("time_type") == 2)
						.Select(p => new EventModel()
						{
							id = p.Field<string>("id") + "_" + p.Field<int>("time_type"),
							title = p.Field<string>("title") + "(" + Function.GetNodeTitle(p.Field<string>("location")) + ")",
							start = p.Field<string>("start"),
							end = p.Field<string>("end"),
							content = "NO_AUTH",
							backgroundColor = p.Field<string>("backgroundColor"),
							allDay = false
						}).ToDictionary(p => new Guid(Function.GetGuid()), p => p);
						if (eventD.Any())
						{
							events = events.Concat(eventD).ToDictionary(p => p.Key, p => p.Value);
						}
					}
					if (k.IsNullOrEmpty() || k.CheckStringValue("1") || k.CheckStringValue("2")) //正式展演時間
					{
						Dictionary<Guid, EventModel> eventP = drs.Where(p => p.Field<int>("time_type") == 1)
						.Select(p => new EventModel()
						{
							id = p.Field<string>("id") + "_" + p.Field<int>("time_type"),
							title = p.Field<string>("title") + "(" + Function.GetNodeTitle(p.Field<string>("location")) + ")",
							start = p.Field<string>("start"),
							end = p.Field<string>("end"),
							content = "NO_AUTH",
							backgroundColor = p.Field<string>("backgroundColor"),
							allDay = false
						}).ToDictionary(p => new Guid(Function.GetGuid()), p => p);
						if (eventP.Any())
						{
							events = events.Concat(eventP).ToDictionary(p => p.Key, p => p.Value);
						}
					}
				}
			}
			return Json(events.Values.ToArray());
		}

		#endregion
	}
}