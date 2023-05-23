using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class BidPicController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
		/// <summary>
		/// 前台網站管理 > 工程標案照片上傳
		/// </summary>
		public const string fun09 = "fun09";

		#region 工程標案照片上傳
		/// <summary>
		/// 列表
		/// </summary>
		public ActionResult Index(int? page, int? defaultPage, string k, string start, string end)
		{
			ViewBag.start = start;
			ViewBag.end = end;

			DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
			DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAll<ARTICLE>(MAIN_ID: NodeID)
				.Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k) || p.CONTENT2.Contains(k)) && (p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd))
				.OrderByDescending(p => p.DATETIME1).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string start, string end)
		{
			ViewBag.start = start;
			ViewBag.end = end;

			BidPicModel model = new BidPicModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				model.DATETIME1 = DateTime.Now;
				model.PICs = new List<ATTACHMENT>();
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
				if (a != null)
				{
					model = new BidPicModel()
					{
						ID = a.ID,
						CONTENT1 = a.CONTENT1,
						CONTENT2 = a.CONTENT2,
						CONTENT3 = a.CONTENT3,
						DATETIME1 = a.DATETIME1.Value,
						PICs = a.ATTACHMENT.ToList()
					};
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, BidPicModel model, int? page, int? defaultPage, string k, string start, string end)
		{
			CheckAuthority(Authority_Right.Update);
			IsSuccessful = true;
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				ARTICLE a = iDB.GetByID<ARTICLE>(id);
				if (a == null)
				{
					a = new ARTICLE()
					{
						ID = Function.GetGuid(),
						NODE_ID = NodeID,
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
				a.DATETIME1 = model.DATETIME1;

				List<HttpPostedFileBase> HPFs = model.HPFs;
				foreach (HttpPostedFileBase hpf in HPFs)
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
						att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: a.ID).Count() + 1;
						att.CONTENT9 = EnableType.Enable.ToIntValue();
						a.ATTACHMENT.Add(att);
						SaveAtt(hpf, att.FILE_NAME);
					}
				}
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<ARTICLE>(a);
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
				}
				if (IsSuccessful)
				{
					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "start", "end" }));
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}

		[ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string start, string end, bool really = false)
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
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "start", "end" }));
		}

		/// <summary>
		/// 刪除附件
		/// </summary>
		[ActionLog(TableNameIndex = 3, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteAttachment(string id, string attid, int? page, int? defaultPage, string k, string start, string end, bool really = true)
		{
			CheckAuthority(Authority_Right.Delete);
			ATTACHMENT att = iDB.GetByID<ATTACHMENT>(attid);
			if (att != null)
			{
				if (really)
				{
					if (!iDB.Delete<ATTACHMENT>(attid))
					{
						AlertMsg = Function.DELETE_ERROR_MESSAGE;
					}
				}
				else
				{
					//不是真的刪除時，記錄刪除人及刪除時間
					att.CONTENT9 = EnableType.Disable.ToIntValue();
					att.CONTENT10 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
					iDB.Save();
				}
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "start", "end" }), "Edit");
		}
		#endregion
	}
}