using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace admin.Controllers
{
    /// <summary>
    /// 首頁管理
    /// </summary>
	public class BannerController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
        /// <summary>
		/// 主視覺 funIndex_01
		/// </summary>
		public const string funIndex_01 = "funIndex_01";
        /// <summary>
		/// 促銷廣告 funIndex_02
		/// </summary>
		public const string funIndex_02 = "funIndex_02";
        /// <summary>
		/// 友站連結 funIndex_03
		/// </summary>
		public const string funIndex_03 = "funIndex_03";
        /// <summary>
		/// 跑馬燈 funIndex_04
		/// </summary>
		public const string funIndex_04 = "funIndex_04";

        #region 主視覺＆促銷廣告&友站連結&跑馬燈
        /// <summary>
        /// 設定共用 ViewBag
        /// </summary>
        public void setViewBag(string id = "")
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
			bool IsMarquee = NodeID.CheckStringValue(funIndex_04);
			ViewBag.IsMarquee = IsMarquee;
			bool LinkNotRequired = NodeID.CheckStringValue(funIndex_01) || NodeID.CheckStringValue(funIndex_02);
			ViewBag.LinkNotRequired = LinkNotRequired;
		}

		/// <summary>
		/// 列表
		/// </summary>
		public ActionResult Index(int? page, int? defaultPage, string k, string k1, string lsID)
		{
			setViewBag();
			ViewBag.k1 = k1;
            IQueryable<ATTACHMENT> list = iDB.GetAll<ATTACHMENT>(MAIN_ID: NodeID)
                .Where(p => (string.IsNullOrEmpty(k) || p.DESCRIPTION.Contains(k)));

			if (!lsID.IsNullOrEmpty()) //改變排序
			{
				List<string> IDs = lsID.Split(';').ToList();
				foreach (ATTACHMENT att in list)
				{
					att.ORDER = IDs.IndexOf(att.ID) + 1;
				}
				iDB.Save();
			}
			return View(list.OrderBy(p => p.ORDER));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1)
		{
			setViewBag(id);
			ViewBag.k1 = k1;

			BannerModel model = new BannerModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
                model.CONTENT9 = DateTime.Today;
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ATTACHMENT att = iDB.GetByIDAsNoTracking<ATTACHMENT>(id);
				if (att != null)
				{
                    model = new BannerModel()
                    {
                        ID = att.ID,
                        DESCRIPTION = att.DESCRIPTION,
                        ORDER = att.ORDER,
                        CONTENT1 = att.CONTENT1,
                        CONTENT2 = att.CONTENT2,
                        CONTENT3 = att.CONTENT3,
                        CONTENT4 = att.CONTENT4,
                        CONTENT6 = att.CONTENT6,
                        CONTENT9 = att.CONTENT9.ToDateTime(),
                        CONTENT10 = att.CONTENT10.ToDateTime(),
                        ImgUrl = att.GetPic(PictureType.Medium)
                    };
				}
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 3, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, BannerModel model, int? page, int? defaultPage, string k, string k1
			, List<string> croppieFileUpload
			, List<string> croppieFileUpload_orginalFileName)
		{
			setViewBag(id);
			ViewBag.k1 = k1;

			if ((bool)ViewBag.LinkNotRequired)
			{
				//主視覺 & 促銷廣告:連結非必填
				ModelState.Remove("CONTENT1");
			}

			CheckAuthority(Authority_Right.Update);
			IsSuccessful = true;
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				bool bUpload = false;

				HttpPostedFileBase hpf = model.hpf;
				if (hpf != null && hpf.ContentLength > 0)
				{
					bUpload = true;
					//string sExt = Path.GetExtension(hpf.FileName).ToLower();
					//if (hpf.ContentLength > IMG_UPLOAD_MAX_SIZE)
					//{
					//	sWarningMsg += "圖片大小超過 2 MB！";
					//}
					//else if (Function.DEFAULT_FILEUPLOAD_PICTURE_EXT.IndexOf(sExt) == -1)
					//{
					//	sWarningMsg += "圖片格式不符！";
					//}
				}

				if (sWarningMsg.IsNullOrEmpty())
				{
					ATTACHMENT att = iDB.GetByID<ATTACHMENT>(id);
					if (att != null)
					{
						if (bUpload)
						{
							att.DeleteFile();
							att.SetUpValue(false, hpf.FileName);
							att.SetUpFileName();
						}
						att.UPDATER = User.Identity.Name;
						att.UPDATE_DATE = DateTime.Now;
					}
					else
					{
						if ((bool)ViewBag.IsMarquee)
						{
							att = new ATTACHMENT();
							att.ATT_TYPE = AttachmentType.Video.ToIntValue();
							att.FILE_NAME = att.ORIGINAL_FILE_NAME = att.EXTENSION = "IS_MARQUEE";
						}
						else
						{
							att = new ATTACHMENT(hpf.FileName);
							att.ATT_TYPE = AttachmentType.Image.ToIntValue();
							att.SetUpFileName();
						}
						att.MAIN_ID = NodeID;
						att.CREATER = User.Identity.Name;
						att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: NodeID).Count() + 1;
					}
					if (bUpload)
					{
                        SavePicture(new WebImage(hpf.InputStream), att);
					}
					att.DESCRIPTION = model.DESCRIPTION;
					att.CONTENT1 = model.CONTENT1.ToHttpUrl();
					att.CONTENT2 = model.CONTENT2;
                    att.CONTENT3 = model.CONTENT3;
                    att.CONTENT4 = model.CONTENT4;
                    att.CONTENT6 = model.CONTENT6;
                    att.CONTENT9 = model.CONTENT9.ToDefaultString();
                    att.CONTENT10 = model.CONTENT10.ToDefaultString();

					if (IsAdd)
					{
						IsSuccessful = iDB.Add<ATTACHMENT>(att);
					}
					else
					{
						iDB.Save();
					}
					if (IsSuccessful)
					{
						AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1" }));
					}
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}
		#endregion
	}
}