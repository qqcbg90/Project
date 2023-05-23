using KingspModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace admin.Filters
{
	//範例(在DB裡的"需要"在Interface產生介面)
	public class SampleDB : ActionFilterAttribute//Filter過濾器
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//List<auth_group> agList = db.auth_group.OrderBy(p => p.group_name).ToList();
			//List<SelectListItem> groupNameList = new List<SelectListItem>();
			//groupNameList.AddRange(agList.Select(n => new SelectListItem { Text = n.group_name, Value = n.group_id.ToString() }));
			//groupNameList.Add(new SelectListItem { Text = "選全部", Value = "All" });
			//filterContext.Controller.ViewBag.groupNameList = new SelectList(groupNameList, "Value", "Text");
			//base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// 將NODE轉換為Select
	/// </summary>
	public class NodeSelect : ActionFilterAttribute
	{
		public NodeSelect(params string[] parentID) { ParentIDs = parentID; }

		private string[] ParentIDs { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			foreach (var item in ParentIDs.Where(p => !string.IsNullOrEmpty(p)))
			{
				List<SelectListItem> items = Function.NodeList
					.Where(x => x.ENABLE.IsEnable() && x.PARENT_ID.CheckStringValue(item))
					.Select(x => new SelectListItem() { Text = x.TITLE, Value = x.ID })
					.ToList();
				filterContext.Controller.ViewData[item] = new SelectList(items, "Value", "Text");
			}
			base.OnActionExecuting(filterContext);
		}
	}
}