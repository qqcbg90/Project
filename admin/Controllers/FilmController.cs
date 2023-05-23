using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class FilmController : BaseController
	{
		/// <summary>
		/// 圖片上傳大小限制
		/// </summary>
		public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
		/// <summary>
		/// 光影文化館 > 光影電影院＞主題影展管理
		/// </summary>
		public const string fun14_04_01 = "fun14_04_01";
		/// <summary>
		/// 光影文化館＞光影電影院＞單元放映管理
		/// </summary>
		public const string fun14_04_02 = "fun14_04_02";
		/// <summary>
		/// 光影文化館＞光影電影院＞影片管理
		/// </summary>
		public const string fun14_04_03 = "fun14_04_03";
		/// <summary>
		/// 光影文化館＞藝文講堂＞講堂管理
		/// </summary>
		public const string fun14_05 = "fun14_05";
		/// <summary>
		/// 光影文化館＞藝文講堂＞活動前言
		/// </summary>
		public const string fun14_05_02 = "fun14_05_02";
		/// <summary>
		/// 活動前言
		/// </summary>
		public const string PREFACE = "PREFACE";

		#region 主題影展管理&單元放映管理＆活動前言
		/// <summary>
		/// 列表
		/// </summary>
		[FilmExhibitionSelect]
		public ActionResult CategoryIndex(int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;

			bool bfun14_04_01 = NodeID.CheckStringValue(fun14_04_01);
			if (NodeID.CheckStringValue(fun14_04_01) || NodeID.CheckStringValue(fun14_05_02))
			{
				return View(iDB.GetAllAsNoTracking<NODE>(MAIN_ID: (bfun14_04_01 ? Function.FILM_EXHIBITION : PREFACE))
					.Where(p => (string.IsNullOrEmpty(k) || p.TITLE.Contains(k))).OrderByDescending(p => p.CONTENT1).ThenByDescending(p => p.CONTENT2)
					.ToPagedList(page.ToMvcPaging(), _defaultPage));
			}
			else
			{
				string[] arrPARENT_ID = Function.NodeList.Where(p => !string.IsNullOrEmpty(p.PARENT_ID) && p.PARENT_ID.Equals(Function.FILM_EXHIBITION) && p.ENABLE.IsEnable())
					.OrderByDescending(p => p.CONTENT1).Select(p => p.ID).ToArray();
				string PARENT_IDs = string.Join(";", arrPARENT_ID);
				return View(iDB.GetAllAsNoTracking<NODE>()
					.Where(p => arrPARENT_ID.Contains(p.PARENT_ID) &&
					p.CONTENT9.Equals(Function.FILM_SCREENINGS) && (string.IsNullOrEmpty(k) || p.TITLE.Contains(k)) && (string.IsNullOrEmpty(k1) || p.PARENT_ID.Equals(k1)))
					.OrderBy(p => PARENT_IDs.IndexOf(p.PARENT_ID)).ThenBy(p => p.ORDER)
					.ToPagedList(page.ToMvcPaging(), _defaultPage));
			}
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[FilmExhibitionSelect]
		public ActionResult CategoryEdit(string id, int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;
			bool bMainCategory = NodeID.CheckStringValue(fun14_04_01);
			bool bPreface = NodeID.CheckStringValue(fun14_05_02);

			CategoryModel model = new CategoryModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				if (bMainCategory || bPreface)
				{
					DateTime dtStart = DateTime.Now.ToString("yyyy/MM/01").ToDateTime(); //月初
					DateTime dtEnd = dtStart.AddMonths(1).AddDays(-1); //月底
					model.CONTENT1 = dtStart;
					model.CONTENT2 = dtEnd;
					model.CONTENT3 = "免費索票入場，每場次座位有限，索完為止";
				}
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				NODE n = iDB.GetByIDAsNoTracking<NODE>(id);
				if (n != null)
				{
					model = new CategoryModel()
					{
						ID = n.ID,
						PARENT_ID = n.PARENT_ID,
						TITLE = n.TITLE,
						URL = n.URL,
						atta = n.ATTACHMENT.FirstOrDefault()
					};
					if (bMainCategory || bPreface)
					{
						model.CONTENT1 = n.CONTENT1.ToDateTime();
						model.CONTENT2 = n.CONTENT2.ToDateTime();

						if (bMainCategory)
						{
							model.CONTENT3 = n.CONTENT3; //20201013
							model.CONTENT4 = n.CONTENT4; //20201013
							model.CONTENT5 = n.CONTENT5.ToDateTime(); //20210111
							model.CONTENT6 = n.CONTENT6.ToDateTime(); //20210111
						}
					}
				}
			}
			return View(model);
		}

		[FilmExhibitionSelect]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult CategoryEdit(string id, CategoryModel model, int? page, int? defaultPage, string k, string k1)
		{
			bool bMainCategory = NodeID.CheckStringValue(fun14_04_01);
			bool bPreface = NodeID.CheckStringValue(fun14_05_02);
			if (bMainCategory)
			{
				ModelState.Remove("PARENT_ID");
			}
			else
			{
				ModelState.Remove("URL");
				ModelState.Remove("CONTENT1");
				ModelState.Remove("CONTENT2");
				ModelState.Remove("CONTENT5");
				ModelState.Remove("CONTENT6");
			}

			CheckAuthority(Authority_Right.Update);
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				NODE n = iDB.GetByID<NODE>(id);
				if (n == null)
				{
					n = new NODE()
					{
						ID = Function.GetGuid(),
						PARENT_ID = bMainCategory ? Function.FILM_EXHIBITION : (bPreface ? PREFACE : model.PARENT_ID),
						CONTENT9 = !bMainCategory && !bPreface ? "FilmScreenings" : "",
						CREATER = User.Identity.Name
					};
				}
				else
				{
					n.UPDATER = User.Identity.Name;
					n.UPDATE_DATE = DateTime.Now;
				}
				n.TITLE = model.TITLE;
				n.URL = model.URL;
				if (bMainCategory || bPreface)
				{
					n.CONTENT1 = model.CONTENT1.Value.ToString("yyyy/MM/dd");
					n.CONTENT2 = model.CONTENT2.Value.ToString("yyyy/MM/dd");
					if (bMainCategory)
					{
						n.CONTENT3 = model.CONTENT3;
						n.CONTENT4 = model.CONTENT4.ToHttpUrl();

						n.CONTENT5 = model.CONTENT5.Value.ToString("yyyy/MM/dd");
						n.CONTENT6 = model.CONTENT6.Value.ToString("yyyy/MM/dd");
					}
				}

				string sOldPicID = string.Empty;
				HttpPostedFileBase hpf = model.hpf;
				if (hpf != null && hpf.ContentLength > 0)
				{
					ATTACHMENT oldAtt = n.ATTACHMENT.FirstOrDefault();
					sOldPicID = oldAtt == null ? string.Empty : oldAtt.ID;

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
						att.ORDER = 0;
						att.CONTENT9 = EnableType.Enable.ToIntValue();
						n.ATTACHMENT.Add(att);
						SaveAtt(hpf, att.FILE_NAME);
					}
				}

				List<string> lsFilmScreenings = new List<string> { "親子同樂", "懷舊經典", "樂齡有影" };
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<NODE>(n);
					if (!bPreface && IsSuccessful)
					{
						for (int i = 0; i < lsFilmScreenings.Count; i++)
						{
							NODE nSubCategory = new NODE()
							{
								PARENT_ID = n.ID,
								TITLE = lsFilmScreenings[i],
								ORDER = (i + 1),
								CREATER = User.Identity.Name,
								CONTENT9 = "FilmScreenings"
							};
							iDB.Add<NODE>(nSubCategory);
						}
					}
				}
				else
				{
					IsSuccessful = true;

					#region 同步至放映清單
					if (bMainCategory)
					{
						if (!n.CONTENT3.IsNullOrEmpty() || !n.CONTENT4.IsNullOrEmpty())
						{
							string SqlStr = @"UPDATE PLUS
SET CONTENT2 = (CASE WHEN LEN(ISNULL(CONTENT2,'')) = 0 THEN '{1}' ELSE CONTENT2 END)
, CONTENT3 = (CASE WHEN LEN(ISNULL(CONTENT3,'')) = 0 THEN '{2}' ELSE CONTENT3 END)
WHERE PLUS_TYPE = 'PLAY_LIST' AND [ENABLE] = 1
AND CONTENT1 IN (
	SELECT ID FROM NODE WHERE [ENABLE] = 1 AND ID = '{0}' 
	OR (PARENT_ID = '{0}' AND CHARINDEX(TITLE,'親子同樂,懷舊經典,樂齡有影') > 0)
);";
							iDB.ExecuteSqlCommand(string.Format(SqlStr, n.ID, n.CONTENT3, n.CONTENT4));
						}
					}
					#endregion
					iDB.Save();
				}
				if (IsSuccessful)
				{
					if (!sOldPicID.IsNullOrEmpty() && sWarningMsg.IsNullOrEmpty())
					{
						iDB.Delete<ATTACHMENT>(sOldPicID, true);
					}
					UpdateNodeList(isWeb: true);
					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1" }), "CategoryIndex");
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}
		#endregion

		#region 影片管理
		/// <summary>
		/// 列表
		/// </summary>
		[NodeSelect("FilmGenre", "FilmFormat")]
		public ActionResult Index(int? page, int? defaultPage, string k)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAll<DATA1>(MAIN_ID: NodeID)
				.Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k) || p.CONTENT14.Contains(k)))
				.OrderByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[FilmCountrySelect]
		[FilmScreeningSpecificationsSelect]
		[FilmPronunciationSelect]
		[FilmSubtitleSelect]
		[FilmColorSelect]
		[NodeSelect("FilmGenre", "FilmFormat")]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k)
		{
			FilmModel model = new FilmModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);

				//預設值
				model.CONTENT9 = FilmPronunciation.EN.GetDescription();
				model.CONTENT10 = FilmSubtitle.TW.GetDescription();
				model.CONTENT12 = FilmColor.Type1.ToIntValue();
				model.PICs = new List<ATTACHMENT>();
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				DATA1 d1 = iDB.GetByIDAsNoTracking<DATA1>(id);
				if (d1 != null)
				{
					model = new FilmModel()
					{
						ID = d1.ID,
						ORDER = d1.ORDER,
						DECIMAL3 = Convert.ToInt32(d1.DECIMAL3 ?? 1),
						CONTENT1 = d1.CONTENT1,
						CONTENT14 = d1.CONTENT14,
						CONTENT4 = d1.CONTENT4,
						CONTENT5 = d1.CONTENT5,
						CONTENT6 = d1.CONTENT6,
						CONTENT7 = d1.CONTENT7,
						CONTENT8 = d1.CONTENT8,
						CONTENT9 = d1.CONTENT9,
						CONTENT10 = d1.CONTENT10,
						CONTENT11 = d1.CONTENT11,
						CONTENT12 = d1.CONTENT12,
						CONTENT13 = d1.CONTENT13,
						CONTENT21 = d1.CONTENT21,
						DECIMAL1 = Convert.ToInt32(d1.DECIMAL1.Value),
						DECIMAL2 = Convert.ToInt32(d1.DECIMAL2.Value),
						PICs = d1.ATTACHMENT.OrderBy(p => p.ORDER).ToList(),
						TIMEs = d1.PLUS.Where(p => p.PLUS_TYPE.Equals("TIME")).Select(p => new TimeModel() { ID = p.ID, MAIN_ID = p.MAIN_ID, DATETIME1 = p.DATETIME1.Value }).ToList()
					};
				}
			}
			model.TIME = DateTime.Now.ToString("yyyy/MM/dd 10:30").ToDateTime();
			return View(model);
		}

		[FilmCountrySelect]
		[FilmScreeningSpecificationsSelect]
		[FilmPronunciationSelect]
		[FilmSubtitleSelect]
		[FilmColorSelect]
		[NodeSelect("FilmGenre", "FilmFormat")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 15, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult Edit(string id, FilmModel model, int? page, int? defaultPage, string k, string lsDel)
		{
			CheckAuthority(Authority_Right.Update);

			if (model != null && (model.TIMEs == null || model.TIMEs.Count == 0))
			{
				SetModelStateError("放映時間：至少 1 個！");
				return View(model);
			}

			IsSuccessful = true;
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				DATA1 d1 = iDB.GetByID<DATA1>(id);
				if (d1 == null)
				{
					d1 = new DATA1()
					{
						ID = Function.GetGuid(),
						NODE_ID = NodeID,
						CREATER = User.Identity.Name
					};
				}
				else
				{
					d1.UPDATER = User.Identity.Name;
					d1.UPDATE_DATE = DateTime.Now;
				}
				d1.ORDER = model.ORDER ?? 1; //映後（0:是 1:否）
				d1.DECIMAL3 = model.DECIMAL3 ?? 1; //映前（0:是 1:否）
				d1.CONTENT1 = model.CONTENT1;
				d1.CONTENT14 = model.CONTENT14;
				d1.CONTENT4 = model.CONTENT4;
				d1.CONTENT5 = model.CONTENT5;
				d1.CONTENT6 = model.CONTENT6;
				d1.CONTENT7 = model.CONTENT7.CheckStringValue("其他") && !model.CONTENT7_OTHER.IsNullOrEmpty() ? model.CONTENT7_OTHER : model.CONTENT7;
				d1.CONTENT8 = model.CONTENT8.CheckStringValue("其他") && !model.CONTENT8_OTHER.IsNullOrEmpty() ? model.CONTENT8_OTHER : model.CONTENT8;
				d1.CONTENT9 = model.CONTENT9.CheckStringValue("其他") && !model.CONTENT9_OTHER.IsNullOrEmpty() ? model.CONTENT9_OTHER : model.CONTENT9;
				d1.CONTENT10 = model.CONTENT10.CheckStringValue("其他") && !model.CONTENT10_OTHER.IsNullOrEmpty() ? model.CONTENT10_OTHER : model.CONTENT10;
				d1.CONTENT11 = model.CONTENT11;
				d1.CONTENT13 = model.CONTENT13.ToHttpUrl();
				d1.CONTENT12 = model.CONTENT12;
				d1.CONTENT21 = model.CONTENT21;
				d1.DECIMAL1 = model.DECIMAL1;
				d1.DECIMAL2 = model.DECIMAL2;

				if (!lsDel.IsNullOrEmpty())
				{
					if (d1.PLUS != null && d1.PLUS.Any()) //刪除
					{
						foreach (string plusID in d1.PLUS.Where(x => x.PLUS_TYPE.Equals("TIME") && lsDel.Contains(x.ID)).Select(x => x.ID).ToList())
						{
							iDB.Delete<PLUS>(plusID, true);
						}
					}
				}

				if (model.TIMEs.Any())
				{
					string lsPLUS = string.Join(";", d1.PLUS.Select(p => p.ID).ToArray());
					foreach (TimeModel time in model.TIMEs.Where(x => !lsPLUS.Contains(x.ID)))
					{
						d1.PLUS.Add(new PLUS()
						{
							ID = Function.GetGuid(),
							PLUS_TYPE = "TIME",
							CREATE_DATE = DateTime.Now,
							CREATER = User.Identity.Name,
							MAIN_ID = d1.ID,
							ENABLE = EnableType.Enable.ToByteValue(),
							ORDER = 1,
							DATETIME1 = time.DATETIME1,
							DATETIME2 = time.DATETIME1 /*沒有結束時間，預設和開始時間一樣*/
						});
					}
				}

				#region 圖片上傳
				List<HttpPostedFileBase> HPFs = model.HPFs;
				foreach (HttpPostedFileBase hpf in HPFs)
				{
					if (hpf != null && hpf.ContentLength > 0)
					{
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
							att.ORDER = 0;
							att.CONTENT9 = EnableType.Enable.ToIntValue();
							d1.ATTACHMENT.Add(att);
							SaveAtt(hpf, att.FILE_NAME);
						}
					}
				}
				#endregion

				if (IsAdd)
				{
					IsSuccessful = iDB.Add<DATA1>(d1);
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
				}
				if (IsSuccessful)
				{
					//修改 DATA1 的 DATETIME9 & DATETIME10
					UpdatePlusDT9AndDT10("DATA1", "fun14_04_03", d1.ID, 1);

					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { }));
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}
		#endregion

		#region 藝文講堂
		/// <summary>
		/// 列表
		/// </summary>
		[LectureTypeSelect]
		public ActionResult LectureIndex(int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(iDB.GetAll<DATA1>(MAIN_ID: NodeID)
				.Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k)) && (string.IsNullOrEmpty(k1) || p.ORDER.ToString().Equals(k1)))
				.OrderByDescending(p => p.DATETIME1).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		/// <summary>
		/// 新增/編輯
		/// </summary>
		[LectureTypeSelect]
		public ActionResult LectureEdit(string id, int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.k1 = k1;

			LectureModel model = new LectureModel();
			if (id.IsNullOrEmpty())
			{
				CheckAuthority(Authority_Right.Add);
				model.TIMEs = new List<TimeModel>();
				model.PICs = new List<ATTACHMENT>();

				//預設值
				model.CONTENT3 = "桃園光影文化館3F放映廳";
				model.CONTENT8 = "免費索票，自由入座";
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				DATA1 d1 = iDB.GetByIDAsNoTracking<DATA1>(id);
				if (d1 != null)
				{
					model = new LectureModel()
					{
						ID = d1.ID,
						ORDER = d1.ORDER,
						CONTENT1 = d1.CONTENT1,
						CONTENT2 = d1.CONTENT2,
						CONTENT3 = d1.CONTENT3,
						CONTENT4 = d1.CONTENT4,
						CONTENT5 = d1.CONTENT5,
						CONTENT6 = d1.CONTENT6,
						CONTENT7 = d1.CONTENT7,
						CONTENT8 = d1.CONTENT8,
						CONTENT9 = d1.CONTENT9,
						CONTENT14 = d1.CONTENT14,
						CONTENT21 = d1.CONTENT21,
						CONTENT22 = d1.CONTENT22,
						CONTENT23 = d1.CONTENT23,
						TIMEs = d1.PLUS.Select(p => new TimeModel() { ID = p.ID, MAIN_ID = p.MAIN_ID, DATETIME1 = p.DATETIME1.Value, DATETIME2 = p.DATETIME2.Value })
						.OrderBy(p => p.DATETIME1).ToList(),
						PICs = d1.ATTACHMENT.Where(p => p.ATT_TYPE.Equals(AttachmentType.Image.ToIntValue())).ToList()
					};
					if (d1.ORDER == 0)
					{
						model.CONTENT13_0 = d1.CONTENT13;
					}
					else
					{
						model.CONTENT13_2 = d1.CONTENT13;
					}
				}
			}
			return View(model);
		}

		[LectureTypeSelect]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 15, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult LectureEdit(string id, LectureModel model, int? page, int? defaultPage, string k, string k1, string lsDel)
		{
			CheckAuthority(Authority_Right.Update);

			if (model != null && (model.TIMEs == null || model.TIMEs.Count == 0))
			{
				SetModelStateError("時間：至少 1 個！");
				return View(model);
			}

			IsSuccessful = true;
			string sWarningMsg = string.Empty;
			if (ModelState.IsValid)
			{
				IsAdd = id.IsNullOrEmpty();
				DATA1 d1 = iDB.GetByID<DATA1>(id);
				if (d1 == null)
				{
					d1 = new DATA1()
					{
						ID = Function.GetGuid(),
						NODE_ID = NodeID,
						CREATER = User.Identity.Name
					};
				}
				else
				{
					d1.UPDATER = User.Identity.Name;
					d1.UPDATE_DATE = DateTime.Now;
				}
				d1.ORDER = model.ORDER;
				d1.CONTENT1 = model.CONTENT1;
				d1.CONTENT2 = model.CONTENT2;
				d1.CONTENT3 = model.CONTENT3;
				d1.CONTENT4 = model.CONTENT4;
				d1.CONTENT5 = model.CONTENT5;
				d1.CONTENT6 = model.CONTENT6;
				d1.CONTENT7 = model.CONTENT7;
				d1.CONTENT8 = model.CONTENT8;
				d1.CONTENT9 = model.CONTENT9;
				d1.CONTENT21 = model.CONTENT21;
				d1.CONTENT22 = model.CONTENT22;
				d1.CONTENT23 = model.CONTENT23;
				d1.CONTENT13 = (model.ORDER == 0 ? model.CONTENT13_0 : model.CONTENT13_2).ToHttpUrl();
				d1.CONTENT14 = model.CONTENT14.ToHttpUrl();
				if (!lsDel.IsNullOrEmpty())
				{
					if (d1.PLUS != null && d1.PLUS.Any()) //刪除
					{
						foreach (string plusID in d1.PLUS.Where(x => lsDel.Contains(x.ID)).Select(x => x.ID).ToList())
						{
							iDB.Delete<PLUS>(plusID, true);
						}
					}
				}

				if (model.TIMEs.Any())
				{
					string lsPLUS = string.Join(";", d1.PLUS.Select(p => p.ID).ToArray());
					foreach (TimeModel time in model.TIMEs.Where(x => !lsPLUS.Contains(x.ID)))
					{
						d1.PLUS.Add(new PLUS()
						{
							ID = Function.GetGuid(),
							PLUS_TYPE = "TIME",
							CREATE_DATE = DateTime.Now,
							CREATER = User.Identity.Name,
							MAIN_ID = d1.ID,
							ENABLE = EnableType.Enable.ToByteValue(),
							ORDER = 0,
							DATETIME1 = time.DATETIME1,
							DATETIME2 = (time.DATETIME1.ToString("yyyy/MM/dd") + time.DATETIME2.ToString(" HH:mm")).ToDateTime()
						});
					}
				}

				//修改描述
				if (d1.ATTACHMENT.Any() && model.PICs.Any())
				{
					Dictionary<string, string> dict = model.PICs.ToDictionary(p => p.ID, p => p.DESCRIPTION);
					if (dict.Any())
					{
						string sDesc = string.Empty;
						foreach (ATTACHMENT atta in d1.ATTACHMENT)
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
					if (hpf != null && hpf.ContentLength > 0)
					{
						string sExt = Path.GetExtension(hpf.FileName).ToLower();
						if (hpf.ContentLength > IMG_UPLOAD_MAX_SIZE)
						{
							sWarningMsg += hpf.FileName + "：圖片大小超過 2 MB！";
						}
						else if (Function.DEFAULT_FILEUPLOAD_PICTURE_EXT.IndexOf(sExt) == -1)
						{
							sWarningMsg += hpf.FileName + "：圖片格式不符(.jpg,.gif,.png)！";
						}
						if (sWarningMsg.IsNullOrEmpty())
						{
							ATTACHMENT att = new ATTACHMENT(hpf.FileName);
							att.ATT_TYPE = AttachmentType.Image.ToIntValue();
							att.SetUpFileName();
							att.CREATER = User.Identity.Name;
							att.ORDER = 0;
							att.CONTENT9 = EnableType.Enable.ToIntValue();
							att.DESCRIPTION = att.ORIGINAL_FILE_NAME.Replace(att.EXTENSION, string.Empty);
							d1.ATTACHMENT.Add(att);
							SaveAtt(hpf, att.FILE_NAME);
						}
					}
				}
				if (IsAdd)
				{
					IsSuccessful = iDB.Add<DATA1>(d1);
				}
				else
				{
					IsSuccessful = true;
					iDB.Save();
				}
				if (IsSuccessful)
				{
					//修改 DATA1 的 DATETIME9 & DATETIME10
					UpdatePlusDT9AndDT10("DATA1", "fun14_05", d1.ID, 0);

					AlertMsg = (IsAdd ? Function.DEFAULT_ADD_MESSAGE : Function.DEFAULT_UPDATE_MESSAGE) + (sWarningMsg.IsNullOrEmpty() ? "" : "<br />" + sWarningMsg);
					return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1" }), "LectureEdit");
				}
			}
			SetModelStateError(sWarningMsg);
			return View(model);
		}
		#endregion

		#region 影片放映清單

		public void setPlayListContentTitle(string id, bool playAdd = false)
		{
			ViewBag.ID = id;
			ViewBag.Start = string.Empty;
			ViewBag.End = string.Empty;
			NODE m = Function.NodeList.FirstOrDefault(x => x.ID.CheckStringValue(id));
			if (m != null)
			{
				if (m.PARENT_ID.CheckStringValue(Function.FILM_EXHIBITION))
				{
					SetContentTitle("影片放映清單 - " + m.TITLE, 2);
					if (playAdd)
					{
						ViewBag.Start = m.CONTENT1;
						ViewBag.End = m.CONTENT2;
						ViewBag.IDs = Function.NodeList.Where(x => x.ENABLE == 1 && (x.ID.Equals(m.ID) || (!string.IsNullOrEmpty(x.PARENT_ID) && x.PARENT_ID.Equals(m.ID)))).Select(p => p.ID).ToArray();
					}
				}
				else
				{
					NODE mm = Function.GetNode(m.PARENT_ID);
					SetContentTitle("影片放映清單 - " + mm.TITLE + " > " + m.TITLE, 2);
					if (playAdd)
					{
						ViewBag.Start = mm.CONTENT1;
						ViewBag.End = mm.CONTENT2;
						ViewBag.IDs = Function.NodeList.Where(x => x.ENABLE == 1 && (x.ID.Equals(mm.ID) || (!string.IsNullOrEmpty(x.PARENT_ID) && x.PARENT_ID.Equals(mm.ID)))).Select(p => p.ID).ToArray();
					}
				}
			}
		}

		public ActionResult PlayList(string id, int? page, int? defaultPage, string k, string k1)
		{
			ViewBag.ID = id;
			ViewBag.k1 = k1;

			setPlayListContentTitle(id);
			SetIsEdit(IsAuthority(Authority_Right.Update));

			#region 20201013 主題的索票說明和索票連結
			string sDefC2 = string.Empty, sDefC3 = string.Empty;
			NODE nSubCategory = Function.NodeList.FirstOrDefault(p => p.ID == id);
			if (nSubCategory != null)
			{
				NODE nMainCategory = Function.NodeList.FirstOrDefault(p => p.ID == nSubCategory.PARENT_ID);
				if (nMainCategory != null)
				{
					sDefC2 = nMainCategory.CONTENT3;
					sDefC3 = nMainCategory.CONTENT4;
				}
			}
			ViewBag.defC2 = sDefC2;
			ViewBag.defC3 = sDefC3;
			#endregion

			List<FilmPlayModel> list = iDB.GetAllAsNoTracking<PLUS>()
				.Where(p => p.PLUS_TYPE.Equals("PLAY_LIST") && p.CONTENT1.Equals(id))
				.Select(p => new FilmPlayModel()
				{
					ID = p.ID,
					MAIN_ID = p.DATA1.ID,
					FILM_NAME = p.DATA1.CONTENT1,
					STATUS = p.STATUS,
					ORDER = p.ORDER,
					CONTENT1 = p.CONTENT1,
					CONTENT2 = p.CONTENT2,
					CONTENT3 = p.CONTENT3
				}).ToList();
			return View(list);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
		public ActionResult PlayList(string nid, string id, int? page, int? defaultPage, string k, string k1, List<FilmPlayModel> model)
		{
			ViewBag.ID = id;
			ViewBag.k1 = k1;

			setPlayListContentTitle(id);
			CheckAuthority(Authority_Right.Update);
			if (ModelState.IsValid)
			{
				if (model != null && model.Count > 0)
				{
					IQueryable<PLUS> list = iDB.GetAll<PLUS>().Where(p => p.PLUS_TYPE.Equals("PLAY_LIST") && p.CONTENT1.Equals(id));
					PLUS plus;
					foreach (FilmPlayModel m in model)
					{
						plus = list.FirstOrDefault(p => p.MAIN_ID.Equals(m.MAIN_ID) && p.CONTENT1.Equals(id));
						if (plus != null) //編輯
						{
							plus.UPDATER = User.Identity.Name;
							plus.UPDATE_DATE = DateTime.Now;

							plus.STATUS = m.STATUS ?? "0";
							plus.ORDER = m.ORDER ?? 0;
							plus.CONTENT2 = m.CONTENT2;
							plus.CONTENT3 = m.CONTENT3.ToHttpUrl();
							if (plus.ORDER == 1 && plus.STATUS.Equals("0")) //有勾選出席座談就會有映後座談
							{
								plus.STATUS = "1";
							}
						}
						else //新增
						{
							plus = new PLUS()
							{
								ID = Function.GetGuid(),
								CREATER = User.Identity.Name,
								MAIN_ID = m.MAIN_ID,
								PLUS_TYPE = "PLAY_LIST",
								ORDER = m.ORDER ?? 0,
								STATUS = m.STATUS ?? "0",
								CONTENT1 = id,
								CONTENT2 = m.CONTENT2,
								CONTENT3 = m.CONTENT3.ToHttpUrl()
							};
							if (plus.ORDER == 1 && plus.STATUS.Equals("0")) //有勾選出席座談就會有映後座談
							{
								plus.STATUS = "1";
							}
							iDB.Add<PLUS>(plus);
						}
					}
					iDB.Save();
					AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
				}
				return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1" }), "PlayList");
			}
			SetModelStateError();
			return View(model);
		}

		/// <summary>
		/// 刪除 PLUS
		/// </summary>
		[ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult DeleteFilmPlay(string id, string pid, int? page, int? defaultPage, string k, string k1, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);

			//不是真的刪除時，記錄刪除人及刪除時間
			if (!really)
			{
				PLUS plus = iDB.GetByID<PLUS>(pid, false);
				if (plus != null)
				{
					plus.CONTENT30 = string.Format("{0}：{1}", User.Identity.Name, DateTime.Now.ToString("yyyy/MM/dd HH:mm.ss.fff"));
				}
			}
			if (!iDB.Delete<PLUS>(pid, really))
			{
				AlertMsg = Function.DELETE_ERROR_MESSAGE;
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "id", "k1" }), "PlayList");
		}

		public ActionResult PlayAdd(string nid, string id)
		{
			setPlayListContentTitle(id, true);
			NODE n = Function.GetNode(id);
			if (n != null)
			{
				string SqlStr = string.Format(@"SELECT DISTINCT d1.ID, d1.CONTENT1, d1.CONTENT4, d1.CONTENT5, CONVERT(varchar(10), d1.DECIMAL1) as DECIMAL1
FROM DATA1 d1
JOIN PLUS p ON p.MAIN_ID = d1.ID AND d1.NODE_ID = 'fun14_04_03' AND p.PLUS_TYPE = 'TIME'
AND d1.[ENABLE] = '1' AND p.[ENABLE] = '1'
WHERE CONVERT(varchar(10),p.DATETIME1,111) BETWEEN @START AND @END
AND NOT EXISTS (
	SELECT MAIN_ID FROM PLUS WHERE PLUS_TYPE = 'PLAY_LIST' AND [ENABLE] = '1' AND MAIN_ID = d1.ID AND CONTENT1 IN ({0})
)
 ORDER BY d1.CONTENT1", "'" + string.Join("','", ViewBag.IDs) + "'");
				IEnumerable<DataRow> drs;
				using (DBEntities db = new DBEntities())
				{
					drs = Function.getDataTable(db, SqlStr, new SqlParameter("START", (string)ViewBag.Start), new SqlParameter("END", (string)ViewBag.End)).AsEnumerable();
				}
				return View((drs.Select(x => new SelectListItem { Value = x.Field<string>("ID"), Text = x.Field<string>("CONTENT1") })).ToList());
			}
			return View(new List<SelectListItem>());
		}
		#endregion

		#region 票務資訊
		public ActionResult TicketInfo(PARAGRAPH model)
		{
			CheckAuthority(Authority_Right.Update);
			PARAGRAPH par = iDB.GetByID<PARAGRAPH>("TICKET_INFO");
			if (IsPost())
			{
				if (model != null)
				{
					par.CONTENT = model.CONTENT;
					par.UPDATE_DATE = DateTime.Now;
					par.UPDATER = User.Identity.Name;
					iDB.Save();
				}
			}
			return View(par);
		}
		#endregion
	}
}