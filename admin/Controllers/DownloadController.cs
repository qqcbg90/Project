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
using Ionic.Zip;

namespace admin.Controllers
{
	public class DownloadController : BaseController
	{
		/// <summary>
		/// 檔案上傳大小限制
		/// </summary>
		public const int FILE_UPLOAD_MAX_SIZE = 10 * 1024 * 1024;
		/// <summary>
		/// 下載專區分類
		/// </summary>
		public const string DOWNLOAD_TYPE = "downloadtype";
		/// <summary>
		/// 便民服務>下載專區
		/// </summary>
		public const string fun13_06_02 = "fun13_06_02";

		#region 下載專區
		/// <summary>
		/// 列表
		/// </summary>
		[NodeSelect(DOWNLOAD_TYPE)]
		public ActionResult Index(int? page, int? defaultPage, string k, string k1, string lsID)
		{
			ViewBag.k1 = k1;
			if (k1.IsNullOrEmpty())
			{
				k1 = string.Join(";", (ViewBag.downloadtype as SelectList).Select(p => p.Value).ToArray());
			}
			IQueryable<NODE> list = iDB.GetAll<NODE>()
				.Where(p => (string.IsNullOrEmpty(k) || p.TITLE.Contains(k)) && (string.IsNullOrEmpty(k1) || k1.Contains(p.PARENT_ID)));
			if (!lsID.IsNullOrEmpty()) //改變排序
			{
				List<string> IDs = lsID.Split(';').ToList();
				foreach (NODE n in list)
				{
					n.ORDER = IDs.IndexOf(n.ID) + 1;
				}
				if (iDB.Save())
				{
					UpdateNodeList();
				}
			}
			return View(list.OrderBy(p => p.ORDER));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[NodeSelect(DOWNLOAD_TYPE)]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;
			DownloadModel model = new DownloadModel();
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
					model = new DownloadModel()
					{
						ID = n.ID,
						PARENT_ID = n.PARENT_ID,
						TITLE = n.TITLE,
						ATTAs = n.ATTACHMENT.Where(p => p.CONTENT9.Equals(EnableType.Enable.ToIntValue())).OrderBy(p => p.DESCRIPTION).ThenBy(p => p.CREATE_DATE).ToList()
					};
				}
			}
			return View(model);
		}

		[NodeSelect(DOWNLOAD_TYPE)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, DownloadModel model, int? page, int? defaultPage, string k, string k1)
		{
			SelectList downloadtype = ViewBag.downloadtype as SelectList;

			CheckAuthority(Authority_Right.Update);
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				#region 新增類別
				if (model.PARENT_ID.CheckStringValue("add"))
				{
					string PARENT_ID_OTHER = model.PARENT_ID_OTHER.Trim();
					if (PARENT_ID_OTHER.IsNullOrEmpty())
					{
						SetModelStateError("尚未輸入類別名稱！");
						return View(model);
					}
					if (downloadtype.FirstOrDefault(p => p.Text.CheckStringValue(PARENT_ID_OTHER)) == null)
					{
						NODE addNode = new NODE
						{
							ID = Function.GetGuid(),
							PARENT_ID = DOWNLOAD_TYPE,
							CREATER = User.Identity.Name,
							TITLE = PARENT_ID_OTHER,
							ORDER = downloadtype.Count() + 1
						};
						if (iDB.Add<NODE>(addNode))
						{
							UpdateNodeList();
							model.PARENT_ID = addNode.ID;
						}
					}
				}
				#endregion

				IsAdd = id.IsNullOrEmpty();
				NODE n = iDB.GetByID<NODE>(id);
				if (n == null)
				{
					n = new NODE()
					{
						ID = Function.GetGuid(),
						CREATER = User.Identity.Name
					};
				}
				else
				{
					n.UPDATER = User.Identity.Name;
					n.UPDATE_DATE = DateTime.Now;
				}
				n.PARENT_ID = model.PARENT_ID;
				n.TITLE = model.TITLE;

				//修改描述
				if (n.ATTACHMENT.Any() && model.ATTAs.Any())
				{
					Dictionary<string, string> dict = model.ATTAs.ToDictionary(p => p.ID, p => p.DESCRIPTION);
					if (dict.Any())
					{
						string sDesc = string.Empty;
						foreach (ATTACHMENT atta in n.ATTACHMENT)
						{
							if (dict.ContainsKey(atta.ID))
							{
								sDesc = dict[atta.ID].ToMyString();
								if (!sDesc.IsNullOrEmpty() && !atta.DESCRIPTION.ToMyString().Equals(sDesc))
								{
									atta.DESCRIPTION = sDesc;
									atta.UPDATE_DATE = DateTime.Now;
									atta.UPDATER = User.Identity.Name;
								}
							}
						}
					}
				}

				//附件上傳
				List<HttpPostedFileBase> HPFs = model.HPFs;
				foreach (HttpPostedFileBase hpf in HPFs)
				{
					if (hpf == null || hpf.ContentLength <= 0)
					{
						continue;
					}

					string sExt = Path.GetExtension(hpf.FileName).ToLower();
					if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
					{
						sWarningMsg += hpf.FileName + "：附件大小超過 10 MB！";
					}
					else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
					{
						sWarningMsg += hpf.FileName + "：附件格式不符！";
					}
					if (sWarningMsg.IsNullOrEmpty())
					{
						ATTACHMENT att = new ATTACHMENT(hpf.FileName);
						att.ATT_TYPE = AttachmentType.File.ToIntValue();
						att.SetUpFileName();
						att.CREATER = User.Identity.Name;
						att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: n.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
						att.CONTENT9 = EnableType.Enable.ToIntValue();
						att.DESCRIPTION = att.ORIGINAL_FILE_NAME.Replace(att.EXTENSION, string.Empty);
						n.ATTACHMENT.Add(att);
						SaveAtt(hpf, att.FILE_NAME);
					}
				}

				if (IsAdd)
				{
					IsSuccessful = iDB.Add<NODE>(n);
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
				}
				if (IsSuccessful)
				{
					UpdateNodeList();
					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1" }), "Edit");
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}

		/// <summary>
		/// 類別管理
		/// </summary>
		public ActionResult TypeIndex()
		{
			ViewBag.Close = false;
			return View(iDB.GetAllAsNoTracking<NODE>(MAIN_ID: DOWNLOAD_TYPE).OrderBy(p => p.ORDER).ToList());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TypeIndex(List<NODE> model)
		{
			ViewBag.Close = false;
			if (model != null && model.Any())
			{
				string sTITLE = string.Empty;
				Dictionary<string, string> dict = model.ToDictionary(p => p.ID, p => p.TITLE);
				foreach (NODE n in iDB.GetAll<NODE>(MAIN_ID: DOWNLOAD_TYPE).OrderBy(p => p.ORDER))
				{
					if (dict.ContainsKey(n.ID))
					{
						sTITLE = dict[n.ID].ToString().Trim();
						if (!sTITLE.IsNullOrEmpty() && !n.TITLE.Equals(sTITLE))
						{
							n.TITLE = sTITLE;
							n.UPDATE_DATE = DateTime.Now;
							n.UPDATER = User.Identity.Name;
						}
					}
				}
				if (iDB.Save())
				{
					UpdateNodeList();
				}
				ViewBag.Close = true;
			}
			return View(model);
		}

		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteType(string id, int? page, int? defaultPage, string k, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			if (iDB.GetAllAsNoTracking<NODE>(MAIN_ID: id).Any())
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
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }), "TypeIndex");
		}

		public ActionResult ZipFiles(string id)
		{
			string downloadFileName = string.Empty;
			NODE n = iDB.GetByIDAsNoTracking<NODE>(id);
			if (n != null)
			{
				downloadFileName = n.TITLE + ".zip";
				string UploadPath = Function.GetUploadPath(true);
				string ZipPath = UploadPath + "\\" + "Zip" + "\\";
				if (!Directory.Exists(ZipPath))
				{
					Directory.CreateDirectory(ZipPath);
				}
				string TempPath = UploadPath + "\\" + "Temp" + "\\";
				if (!Directory.Exists(TempPath))
				{
					Directory.CreateDirectory(TempPath);
				}
				string UpFile = string.Empty, TmFile = string.Empty;
				List<string> lsFiles = new List<string>();
				List<ATTACHMENT> lsATTA = n.ATTACHMENT.Where(p => p.CONTENT9.Equals("1")).ToList();
				if (lsATTA != null && lsATTA.Count > 0)
				{
					using (var zip = new ZipFile(System.Text.Encoding.UTF8))
					{
						foreach (ATTACHMENT att in lsATTA)
						{
							UpFile = UploadPath + att.FILE_NAME;
							TmFile = (TempPath + att.DESCRIPTION).Replace(att.EXTENSION, "") + att.EXTENSION;
							if (System.IO.File.Exists(UpFile))
							{
								System.IO.File.Copy(UpFile, TmFile);
							}
							if (System.IO.File.Exists(TmFile))
							{
								zip.AddFile(TmFile, n.TITLE);
								lsFiles.Add(TmFile);
							}
						}
						zip.Save(ZipPath + n.ID + ".zip");
						foreach (string sFile in lsFiles)
						{
							System.IO.File.Delete(sFile);
						}
					}
				}
				return File(ZipPath + n.ID + ".zip", "application/x-zip-compressed", DateTime.Now.ToString("yyyyMMddHHmm") + downloadFileName);
			}
			return Content("");
		}
		#endregion
	}
}