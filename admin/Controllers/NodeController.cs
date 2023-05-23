using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using MvcPaging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace admin.Controllers
{
	[Authorize(Users = Function.DEFAULT_ADMIN)]
	public class NodeController : BaseController
	{
		#region NODE 列表、編輯

		public ActionResult Index(int? page, int? defaultPage, string k, string pid)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			ViewBag.DefaultPage = _defaultPage;
			int _page = IsPost() ? 0 : page.ToMvcPaging();
			ViewBag.page = _page;
			ViewBag.Keyword = k;

			IQueryable<NODE> model = iDB.GetAllAsNoTracking<NODE>(false)
				.Where(p => string.IsNullOrEmpty(k) || p.TITLE.Contains(k) || p.ID.Contains(k) || p.PARENT_ID.Contains(k));
			return View(model.ToPagedList(_page, _defaultPage));
		}

		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string pid)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;

			NodeModel model = new NodeModel();
			if (!IsAdd)
			{
				NODE node = iDB.GetByIDAsNoTracking<NODE>(id, false);
				if (node != null)
				{
					model = new NodeModel()
					{
						ID = node.ID,
						CREATE_DATE = node.CREATE_DATE,
						CREATER = node.CREATER,
						UPDATE_DATE = node.UPDATE_DATE,
						UPDATER = node.UPDATER,
						TITLE = node.TITLE,
						URL = node.URL,
						PARENT_ID = node.PARENT_ID,
						ORDER = node.ORDER,
						ENABLE = node.ENABLE,
						CONTENT1 = node.CONTENT1,
						CONTENT2 = node.CONTENT2,
						CONTENT3 = node.CONTENT3,
						CONTENT4 = node.CONTENT4,
						CONTENT5 = node.CONTENT5,
						CONTENT6 = node.CONTENT6,
						CONTENT7 = node.CONTENT7,
						CONTENT8 = node.CONTENT8,
						CONTENT9 = node.CONTENT9,
						CONTENT10 = node.CONTENT10
					};
				}
			}
			return View(model);
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 0, Description = "新增/編輯 NODE")]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string pid, NodeModel model)
		{
			NODE n = new NODE();
			if (ModelState.IsValid)
			{
				n = iDB.GetByID<NODE>(id);
				if (n != null)
				{
					IsAdd = false;
				}
				else
				{
					IsAdd = true;
					n = new NODE();
					n.ID = model.ID;
					n.CREATER = User.Identity.Name;
				}
				ViewBag.IsAdd = IsAdd;

				n.TITLE = model.TITLE;
				n.URL = model.URL;
				n.PARENT_ID = model.PARENT_ID;
				n.ORDER = model.ORDER;
				n.ENABLE = model.ENABLE;
				n.CONTENT1 = model.CONTENT1;
				n.CONTENT2 = model.CONTENT2;
				n.CONTENT3 = model.CONTENT3;
				n.CONTENT4 = model.CONTENT4;
				n.CONTENT5 = model.CONTENT5;
				n.CONTENT6 = model.CONTENT6;
				n.CONTENT7 = model.CONTENT7;
				n.CONTENT8 = model.CONTENT8;
				n.CONTENT9 = model.CONTENT9;
				n.CONTENT10 = model.CONTENT10;
				IsSuccessful = IsAdd ? iDB.Add<NODE>(n) : iDB.Save();
				if (IsSuccessful)
				{
					UpdateNodeList(isWeb: true);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "pid" }));
				}
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 0, Description = "刪除 NODE")]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string pid, bool really = false)
		{
			if (!iDB.Delete<NODE>(id, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			else
			{
				UpdateNodeList(isWeb: true);
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "pid" }));
		}
		#endregion
	}
}
