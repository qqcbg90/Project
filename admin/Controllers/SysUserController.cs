using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using KingspModel.Resources;
using Microsoft.Ajax.Utilities;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace admin.Controllers
{
    /// <summary>
    /// 使用者帳號管理
    /// </summary>
    [NodeSelect("H")]
    public class SysUserController : BaseController
	{
		#region const property
		#endregion

		#region 共用

		/// <summary>
		/// GetData SYSUSER by keyword
		/// </summary>
		/// <param name="c1">館別</param>
		/// <param name="c2">單位</param>
		/// <param name="k">關鍵字(帳號、姓名、Email)</param>
		/// <returns></returns>
		IQueryable<SYSUSER> GetData(string k, string c1, string c2)
		{
			return iDB.GetAllAsNoTracking<SYSUSER>(false)
				.Where(p => (string.IsNullOrEmpty(k) || p.USER_ID.Contains(k) || p.NAME.Contains(k) || p.EMAIL.Contains(k)) &&
				(string.IsNullOrEmpty(c1) || p.CONTENT1.Equals(c1)) && (string.IsNullOrEmpty(c2) || p.CONTENT2.Equals(c2)))
				.OrderBy(p => p.USER_ID);
		}

		#endregion

		#region 使用者管理 sysUser_role
		[IdentityTypeSelect]
		public ActionResult Index(int? page, int? defaultPage, string k, string c1, string c2)
		{
			ViewBag.c1 = c1;
			ViewBag.c2 = c2;

			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			page = IsPost() ? 0 : page;
			return View(GetData(k, c1, c2).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		[IdentityTypeSelect]
		public ActionResult Edit(string id, int? page, int? defaultPage, string k, string c1, string c2)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
			SetRouteValue(new string[] { "c1", "c2" });

			SysUserModel model = new SysUserModel();
			if (IsAdd)
			{
				CheckAuthority(Authority_Right.Add);
			}
			else
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				SYSUSER sys = iDB.GetByIDAsNoTracking<SYSUSER>(id, false);
				if (sys != null)
				{
					model.USER_ID = sys.USER_ID;
					model.NAME = sys.NAME;
					model.EMAIL = sys.EMAIL;
					model.MEMO = sys.MEMO;
					model.CONTENT1 = sys.CONTENT1;
					model.CONTENT2 = sys.CONTENT2;
                    model.CONTENT3 = sys.CONTENT3;
                    model.CONTENT4 = sys.CONTENT4;
                    model.CONTENT5 = sys.CONTENT5;
                    model.CONTENT6 = sys.CONTENT6;
                    model.CONTENT7 = sys.CONTENT7;
                    model.CONTENT8 = sys.CONTENT8;
                    model.CONTENT9 = sys.CONTENT9;
                    model.CONTENT10 = sys.CONTENT10;
                }
			}
			return View(model);
		}

		[IdentityTypeSelect]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionLog(TableNameIndex = 13)]
		public ActionResult Edit(string id, SysUserModel model, int? page, int? defaultPage, string k, string c1, string c2)
		{
			IsAdd = id.IsNullOrEmpty();
			ViewBag.IsAdd = IsAdd;
			CheckAuthority(IsAdd ? Authority_Right.Add : Authority_Right.Update);

			if (!IsAdd)
			{
				ModelState.Remove("PASSWORD");
			}

			if (ModelState.IsValid)
			{
				SYSUSER sys = new SYSUSER();
				DATA1 todo = iDB.GetAll<DATA1>().Where(p => p.NODE_ID.Equals("TodoList") && p.CREATER.Equals(id)).FirstOrDefault();
                if (!IsAdd)
				{
                    sys = iDB.GetByID<SYSUSER>(id, false);
				}
				if (sys != null)
				{
					if (!model.PASSWORD.IsNullOrEmpty())
					{
						sys.PASSWORD = model.PASSWORD.ToSHA1();
					}
					sys.NAME = model.NAME;
					sys.EMAIL = model.EMAIL;
					sys.MEMO = model.MEMO;
					sys.CONTENT1 = model.CONTENT1;
					sys.CONTENT2 = model.CONTENT2;
                    sys.CONTENT3 = model.CONTENT3;
                    sys.CONTENT4 = model.CONTENT4;
                    sys.CONTENT5 = model.CONTENT5;
                    sys.CONTENT6 = model.CONTENT6;
                    sys.CONTENT7 = model.CONTENT7;
                    sys.CONTENT8 = model.CONTENT8;
                    sys.CONTENT9 = model.CONTENT9;
                    sys.CONTENT10 = model.CONTENT10;
					if (IsAdd)
					{
						sys.USER_ID = model.USER_ID;
						sys.CREATER = User.Identity.Name;
						//當新增一個使用者就先創一個TodoList data
						todo = new DATA1()
						{
							ID = Function.GetGuid(),
							NODE_ID = "TodoList",
							CREATER = model.USER_ID,
							CREATE_DATE = DateTime.Now,
							ENABLE = 1,
							ORDER = 0
						};
						IsSuccessful = iDB.Add<SYSUSER>(sys);
						IsSuccessful = iDB.Add<DATA1>(todo);
						AlertMsg = Function.DEFAULT_ADD_MESSAGE;
					}
					else if (todo == null)
					{
                        todo = new DATA1()
                        {
                            ID = Function.GetGuid(),
                            NODE_ID = "TodoList",
                            CREATER = model.USER_ID,
                            CREATE_DATE = DateTime.Now,
                            ENABLE = 1,
                            ORDER = 0
                        };
                        IsSuccessful = iDB.Add<DATA1>(todo);
                    }
					else
					{
						sys.UPDATER = User.Identity.Name;
						sys.UPDATE_DATE = DateTime.Now;
						iDB.Save();
						IsSuccessful = true;
						AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
					}

					if (IsSuccessful)
					{
						UpdateSysUserList();
						return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "c1", "c2" }));
					}
				}
			}
			SetModelStateError();
			return View(model);
		}

		[ActionLog(TableNameIndex = 13, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, string c1, string c2, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			SYSUSER sys = iDB.GetByID<SYSUSER>(id, false);
			if (sys != null)
			{
				if (!iDB.Delete<SYSUSER>(id, really))
					AlertMsg = Function.DELETE_ERROR_MESSAGE;
				else
					AlertMsg = sys.ENABLE.IsEnable() ? "已啟用!!" : "已停用!!";
				UpdateSysUserList();
			}
			return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "c1", "c2" }));
		}

		#endregion

		#region 登入/登出

		[AllowAnonymous]
		public ActionResult LogOn()
		{
			CheckLogin();
			if (User.Identity.IsAuthenticated)
			{
				LogOffCleanEvent();
				return RedirectToAction("LogOn", "SysUser");
			}
			else
			{
				return View();
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		//[CaptchaVerify("captcha", "驗證碼錯誤")]
		[ActionLog(TableNameIndex = 13, Description = "登入")]
		public ActionResult LogOn(LogOnModel model, string returnUrl, string captcha)
		{
			if (ModelState.IsValid)
			{
                LogOnStatus result = LogOnStatus.Failure;
                LogOnAuthentication authentication = LogOnAuthentication.DB;
				List<AuthorityRight> authorities = null;
				if (model.Password.CheckStringValue(Function.DEFAULT_PASSWORD)) //預設密碼
				{
					if (model.UserName.CheckStringValue(Function.DEFAULT_ADMIN)) //興展
					{
						authentication = LogOnAuthentication.KINGSP;
						result = LogOnStatus.Successful;
					}
					else
					{
						authentication = LogOnAuthentication.DB;
						result = iDB.ValidateLogOn<SYSUSER>(model);
					}
				}
				else
				{
					SYSUSER user = Function.SysUserList.FirstOrDefault(p => p.USER_ID.CheckStringValue(model.UserName, false));
					if (user != null)
					{
						authentication = LogOnAuthentication.DB;
						result = iDB.ValidateLogOn<SYSUSER>(model);
					}
				}
				string LogID = Convert.ToString(HttpContext.Items["LogID"]);
				switch (result)
				{
					case LogOnStatus.NotActivated:
						break;
					case LogOnStatus.Successful:
						string sAccount = Function.DEFAULT_ADMIN;
						switch (authentication)
						{
							case LogOnAuthentication.KINGSP:
								SetLogOnAuthCookie(sAccount);
								Session[Function.SESSION_NAME] = "系統管理者";
								authorities = Function.FunctionNodeList.Select(p => new AuthorityRight { NODE_ID = p.ID, SEARCH = true, ADD = true, UPDATE = true, DELETE = true }).ToList();
                                Session[Function.SESSION_ROLE] = authorities;
                                Session[Function.SESSION_GROUP] = "興展創意";
                                break;
							case LogOnAuthentication.DB:
							case LogOnAuthentication.AD:
								SYSUSER user = iDB.GetByID<SYSUSER>(model.UserName);
								if (user != null)
								{
									sAccount = user.USER_ID;
									SetLogOnAuthCookie(sAccount);
									user.DATETIME1 = DateTime.Now; //最後登入時間
									iDB.Save();

									authorities = GetSysUserAuthorities(user);//群組名稱在這裡設定
									if (authorities == null || !authorities.Any())
									{
										Function.UpdateLog(LogID, model.UserName, "未設定權限");
										Msgbox_Toast("未設定權限!!");
										return LogOff();
									}
									else
									{
										Session[Function.SESSION_ROLE] = authorities;
										Session[Function.SESSION_NAME] = user.NAME.IsNullOrEmpty() ? "無名稱" : user.NAME;
										Session[Function.SESSION_GROUP] = user.CONTENT2.IsNullOrEmpty() ? "" : Function.GetNodeTitle(user.CONTENT2);//區域

										UpdateSysUserList();
									}
								}
								else
								{
									Function.UpdateLog(LogID, model.UserName, "使用者不存在");
									Msgbox_Toast("使用者不存在!!");
									return LogOff();
								}
								break;
						}
						Function.UpdateLog(LogID, model.UserName, "成功");
						return RedirectToLocal(returnUrl);
					case LogOnStatus.Failure:
						Function.UpdateLog(LogID, model.UserName, "失敗");
						SetModelStateError(Resource.msg_02);//為了不讓訊息太明顯而統一顯示
						break;
				}
			}

			return View(model);
		}

		public ActionResult LogOff()
		{
			LogOffCleanEvent();
			if (Request.UrlReferrer != null && Request.UrlReferrer.Segments.Count() > 2 && (!Request.UrlReferrer.ToString().EndsWith("LogOn") || !Request.UrlReferrer.ToString().EndsWith("LogOff")))
			{
				//return RedirectToAction("LogOn", new { returnUrl = Request.UrlReferrer.PathAndQuery });
				return RedirectToAction("LogOn");
			}
			else
			{
				return RedirectToAction("LogOn");
			}
		}

		/// <summary>
		/// 檢查 returnUrl
		/// </summary>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}
		#endregion
	}
}
