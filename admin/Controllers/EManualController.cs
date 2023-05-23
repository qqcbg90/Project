using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class EManualController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
		/// <summary>
		///  檔案上傳大小限制
		/// </summary>
		public const int FILE_UPLOAD_MAX_SIZE = 20 * 1024 * 1024;
		/// <summary>
		/// 光影文化館 > 電子手冊管理
		/// </summary>
		public const string fun14_09 = "fun14_09";

		#region 電子手冊
		/// <summary>
		/// 設定共用 ViewBag
		/// </summary>
		public void setViewBag(string id = "")
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
		}

		/// <summary>
		/// 列表
		/// </summary>
		public ActionResult Index(int? page, int? defaultPage, string k, string lsID)
		{
			setViewBag();

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			string sEnable = EnableType.Enable.ToIntValue();

			IPagedList<ATTACHMENT> list = iDB.GetAll<ATTACHMENT>(MAIN_ID: NodeID)
				.Where(p => p.CONTENT9.Equals(sEnable) && (string.IsNullOrEmpty(k) || p.DESCRIPTION.Contains(k)))
				.OrderByDescending(p => p.CONTENT6).ThenByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage);

			return View(list);
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		public ActionResult Edit(string id, int? page, int? defaultPage, string k)
		{
			setViewBag(id);

			EManualModel model = new EManualModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);

				model.CONTENT6 = DateTime.Now.ToString("yyyy/MM/01").ToDateTime();
				model.CONTENT7 = model.CONTENT6.Value.AddMonths(1).AddDays(-1);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ATTACHMENT att = iDB.GetByIDAsNoTracking<ATTACHMENT>(id);
				if (att != null)
				{
					model = new EManualModel()
					{
						ID = att.ID,
						DESCRIPTION = att.DESCRIPTION,
						ORDER = att.ORDER,
						CONTENT1 = att.CONTENT1,
						CONTENT6 = att.CONTENT6.ToDateTime(),
						CONTENT7 = att.CONTENT7.ToDateTime(),
						ImgUrl = att.GetPic()
					};
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 3, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, EManualModel model, int? page, int? defaultPage, string k)
		{
			setViewBag(id);

			CheckAuthority(Authority_Right.Update);
			IsSuccessful = true;
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				bool bUploadImg = false, bUploadFile = false;

				//封面圖片
				HttpPostedFileBase hpf = model.hpf;
				if (hpf != null && hpf.ContentLength > 0)
				{
					bUploadImg = true;
					string sExt = Path.GetExtension(hpf.FileName).ToLower();
					if (hpf.ContentLength > IMG_UPLOAD_MAX_SIZE)
					{
						sWarningMsg += "封面圖片大小超過 2 MB！";
					}
					else if (Function.DEFAULT_FILEUPLOAD_PICTURE_EXT.IndexOf(sExt) == -1)
					{
						sWarningMsg += "封面圖片格式不符！";
					}
				}

				//電子手冊
				HttpPostedFileBase hpfFile = model.hpfFile;
				if (hpfFile != null && hpfFile.ContentLength > 0)
				{
					bUploadFile = true;
					string sExt = Path.GetExtension(hpfFile.FileName).ToLower();
					if (hpfFile.ContentLength > FILE_UPLOAD_MAX_SIZE)
					{
						sWarningMsg += "手冊大小超過 20 MB！";
					}
					else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
					{
						sWarningMsg += "手冊格式不符！";
					}
				}

				if (sWarningMsg.IsNullOrEmpty())
				{
					ATTACHMENT att = iDB.GetByID<ATTACHMENT>(id);
					if (att != null)
					{
						att.UPDATER = User.Identity.Name;
						att.UPDATE_DATE = DateTime.Now;

						if (bUploadImg)
						{
							att.DeleteFile(); //刪除舊檔
							att.SetUpValue(false, hpf.FileName);
							att.SetUpFileName();
							SaveAtt(hpf, att.FILE_NAME);
						}
					}
					else
					{
						att = new ATTACHMENT(hpf.FileName);
						att.ATT_TYPE = AttachmentType.Image.ToIntValue();
						att.SetUpFileName();
						att.MAIN_ID = NodeID;
						att.CREATER = User.Identity.Name;
						att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: NodeID).Count() + 1;
						att.CONTENT9 = EnableType.Enable.ToIntValue();
						SaveAtt(hpf, att.FILE_NAME);
					}

					if (bUploadFile)
					{
						if (!att.CONTENT1.IsNullOrEmpty()) //刪除舊檔
						{
							string uploadPath = Server.MapPath(Function.GetUploadPath());
							System.IO.File.Delete(Path.Combine(uploadPath, att.CONTENT1));
						}
						string sExt = System.IO.Path.GetExtension(hpfFile.FileName);
						string sFILE_NAME = DateTime.Now.ToString("yyyyMMddHHMM") + Function.GetGuid().Substring(0, 8) + sExt;
						SaveAtt(hpfFile, sFILE_NAME);
						att.CONTENT1 = sFILE_NAME;
					}

					att.DESCRIPTION = model.DESCRIPTION;
					DateTime tmpStart = DateTime.Now.ToString("yyyy/MM/01").ToDateTime();
					DateTime tmpEnd = tmpStart.AddMonths(1).AddDays(-1);
					att.CONTENT6 = model.CONTENT6.HasValue ? model.CONTENT6.Value.ToDateString() : tmpStart.ToDateString();
					att.CONTENT7 = model.CONTENT7.HasValue ? model.CONTENT7.Value.ToDateString() : tmpEnd.ToDateString();

					if (IsAdd)
					{
						IsSuccessful = iDB.Add<ATTACHMENT>(att);
					}
					else
					{
						IsSuccessful = true;
						iDB.Save();
					}
					if (IsSuccessful)
					{
						AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }));
					}
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}
		#endregion
	}
}