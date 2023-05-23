using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using admin.Filters;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using MvcPaging;
using System;

namespace admin.Controllers
{
	public class RoleGroupController : BaseController
	{
		#region const property
		/// <summary>
		/// 此專案權限是否要CRUD
		/// </summary>
		const bool IS_CRUD = false;

		#endregion

		#region 建構式

		#endregion

		#region 列表、編輯

		public ActionResult Index(int? page, int? defaultPage, string k)
		{
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			return View(GetData(k, RoleGroupType.SYSUSER).ToPagedList(page.ToMvcPaging(), _defaultPage));
		}

		[HttpPost]
		public ActionResult Index(int? defaultPage, string k)
		{
			CheckAuthority(Authority_Right.Search);
			int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
			return View(GetData(k, RoleGroupType.SYSUSER).ToPagedList(0, _defaultPage));
		}

		public ActionResult Edit(string id, int? page, int? defaultPage, string k)
		{
			IsAdd = id.IsNullOrEmpty();
			RoleGroupModel rg = new RoleGroupModel();
			if (IsAdd)//add
			{
				CheckAuthority(Authority_Right.Add);
			}
			else//edit
			{
				SetIsEdit(IsAuthority(Authority_Right.Update));
				ROLE_GROUP _rg = iDB.GetByID<ROLE_GROUP>(id);
				if (_rg != null)
				{
					rg.GROUP_TYPE = _rg.GROUP_TYPE;
					rg.TITLE = _rg.TITLE;
					rg.MEMO = _rg.MEMO;
					rg.ROLETREELIST = CreateAuthorityString(_rg, IS_CRUD);
				}
			}
			ViewBag.isCRUD = IS_CRUD;
			return View(rg);
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 11)]
		public ActionResult Edit(string id, RoleGroupModel model, int? page, int? defaultPage, string k)
		{
			IsAdd = id.IsNullOrEmpty();
			CheckAuthority(IsAdd ? Authority_Right.Add : Authority_Right.Update);
			if (ModelState.IsValid)
			{
				ROLE_GROUP rg;
				if (IsAdd)//add
				{
					rg = new ROLE_GROUP();
					rg.CREATER = User.Identity.Name;
					rg.GROUP_TYPE = RoleGroupType.SYSUSER.ToString();
				}
				else//edit
				{
					rg = iDB.GetByID<ROLE_GROUP>(id);
				}
				if (rg != null)
				{
					rg.TITLE = model.TITLE;
					rg.MEMO = model.MEMO;
					if (!IsAdd) iDB.DeleteAllAuthority(id);//先清除全部權限

					//將使用者設定的權限轉成 List<AUTHORITY> 集合
					List<AUTHORITY> _authorities = CreateAuthorityList(model.ROLETREELIST, IS_CRUD);
					//↓才真的加到資料庫
					foreach (AUTHORITY a in _authorities)
					{
						rg.AUTHORITY.Add(a);
					}

					if (IsAdd)
					{
						IsSuccessful = iDB.Add<ROLE_GROUP>(rg);
						AlertMsg = Function.DEFAULT_ADD_MESSAGE;
					}
					else
					{
						rg.UPDATER = User.Identity.Name;
						rg.UPDATE_DATE = DateTime.Now;
						iDB.Save();
						IsSuccessful = true;
						AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
					}
					if (IsSuccessful)
					{
						UpdateAllUsersAuthorityRight(false);
						return GoIndex(NodeID, page, defaultPage, k);
					}
				}
			}
			SetModelStateError();
			ViewBag.isCRUD = IS_CRUD;
			return View(model);
		}

		public ActionResult RoleGroupUser(string id, int? page, int? defaultPage, string k)
		{
			SetIsEdit(IsAuthority(Authority_Right.Update));
			ROLE_GROUP rg = iDB.GetByID<ROLE_GROUP>(id);
			if (rg == null) return GoIndex(NodeID, page, defaultPage, k);
			SetContentTitle(rg.TITLE);
			ViewBag.ID = id;
			//return View(rg.ROLE_USER_MAPPING.Where(p => p.SYSUSER.ENABLE.IsEnable()).ToList());
			return View(rg.ROLE_USER_MAPPING.ToList());
		}

		[HttpPost]
		[ActionLog(TableNameIndex = 11, Description = "編輯權限使用者")]
		public ActionResult RoleGroupUser(string id, string users, int? page, int? defaultPage, string k)
		{
			CheckAuthority(Authority_Right.Update);

			ROLE_GROUP rg = iDB.GetByID<ROLE_GROUP>(id);
			if (rg == null)
				return GoIndex(NodeID, page, defaultPage, k);

			List<ROLE_USER_MAPPING> rgUsers = rg.ROLE_USER_MAPPING.ToList();

			if (!string.IsNullOrEmpty(users))
			{
				string[] usersArr = users.ToSplit(',');

				List<ROLE_USER_MAPPING> deleteData = rgUsers.Where(x => !usersArr.Contains(x.USER_ID)).ToList();
				foreach (var item in deleteData)
				{
					iDB.Delete<ROLE_USER_MAPPING>(item.ID);
				}

				foreach (var userID in usersArr)
				{
					SYSUSER sys = Function.GetSysUserByID(userID);
					if (rgUsers.Any(x => x.USER_ID.CheckStringValue(userID)) || sys == null)
						continue;

					ROLE_USER_MAPPING mapping = new ROLE_USER_MAPPING();
					mapping.ID = Function.GetGuid();
					mapping.USER_ID = sys.USER_ID;
					mapping.CREATER = User.Identity.Name;
					mapping.CREATE_DATE = DateTime.Now;
					rg.ROLE_USER_MAPPING.Add(mapping);
				}
				iDB.Save();
				AlertMsg = Function.DEFAULT_UPDATE_MESSAGE;
			}
			else if (rgUsers != null && rgUsers.Count() > 0)
			{
				foreach (var item in rgUsers)
				{
					iDB.Delete<ROLE_USER_MAPPING>(item.ID);
				}
			}
			UpdateAllUsersAuthorityRight(false);
			return GoIndex(NodeID, page, defaultPage, k);
		}


		[ActionLog(TableNameIndex = 11, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult Delete(string id, int? page, int? defaultPage, string k, bool really = false)
		{
			CheckAuthority(Authority_Right.Delete);
			AlertMsg = iDB.Delete<ROLE_GROUP>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
			return GoIndex(NodeID, page, defaultPage, k);
		}

		/// <summary>
		/// GetData ROLE_GROUP by keyword
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		IQueryable<ROLE_GROUP> GetData(string keyword, RoleGroupType groupType)
		{
			string _groupType = groupType.ToString();
			return iDB.GetAllAsNoTracking<ROLE_GROUP>(MAIN_ID: _groupType).Where(p => (string.IsNullOrEmpty(keyword) || p.TITLE.Contains(keyword)));
		}
		#endregion

	}
}
