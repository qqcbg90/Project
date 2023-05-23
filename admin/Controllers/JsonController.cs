using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using admin.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;
using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using KingspModel.Resources;
using Newtonsoft.Json;

namespace admin.Controllers
{
	/// <summary>
	/// 不需權限驗證的皆可放在這
	/// </summary>
	public class JsonController : BaseController
	{

		#region 建構式

		#endregion

		#region 共用

		#region NODE
		/// <summary>
		/// 檢查NODE ID 有無重覆
		/// </summary>
		/// <param name="Node_Id"></param>
		/// <returns></returns>
		public JsonResult NodeExist(string Node_Id)
		{
			bool isValidate = false;
			isValidate = !iDB.IsIDRepeat<NODE>(Node_Id);
			return Json(isValidate, JsonRequestBehavior.AllowGet);
		}

		[HttpDelete]
		[ActionLog(TableNameIndex = 0, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult NodeDelete(string id)
		{
			if (iDB.Delete<NODE>(id))
			{
				UpdateNodeList();
				return Content(Function.DELETE_MESSAGE);
			}
			else
				return JavaScript(Function.ShowAjaxMessage());
		}

		/// <summary>
		/// 取得子NODE
		/// </summary>
		/// <returns>string (HTML)</returns>
		public ActionResult GetChild(string id, string v, string first)
		{
			string strFormat = "<option value={0} {2}>{1}</option>";
			StringBuilder sb = new StringBuilder();
			if (!first.IsNullOrEmpty())
			{
				sb.AppendFormat(string.Format(strFormat, string.Empty, first, string.Empty));
			}
			if (!id.IsNullOrEmpty())
			{
				foreach (NODE child in Function.NodeList.Where(p => id.CheckStringValue(p.PARENT_ID) && p.ENABLE.IsEnable()).OrderBy(p => p.ORDER))
				{
					sb.AppendFormat(string.Format(strFormat, child.ID, child.TITLE, child.ID.CheckStringValue(v) ? "selected" : string.Empty));
				}
			}
			return Content(sb.ToString());
		}
		#endregion

		#region SYSUSER系統帳號管理
		/// <summary>
		/// 檢查USER_ID有無重覆
		/// </summary>
		/// <param name="USER_ID"></param>
		/// <returns></returns>
		[AllowAnonymous]
		public JsonResult SysUserExist(string USER_ID)
		{
			bool isValidate = false;
			isValidate = !iDB.IsIDRepeat<SYSUSER>(USER_ID);
			return Json(isValidate, JsonRequestBehavior.AllowGet);
		}

		[AllowAnonymous]
		public JsonResult SysEmailExist(string USER_ID, string EMAIL)
		{
			bool isValidate = false;
			isValidate = iDB.CheckEmailRepeat<SYSUSER>(USER_ID, EMAIL);
			return Json(isValidate, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ResetPassword()
		{
			SetContentTitle(Resource.ResetPassword.ToString());
			return View();
		}

		[HttpPost]
		public ActionResult ResetPassword(SysUserChangePasswordModel model)
		{
			SetContentTitle(Resource.ResetPassword.ToString());
			if (ModelState.IsValid)
			{
				SYSUSER user = iDB.GetByID<SYSUSER>(User.Identity.Name);
				if (user != null)
				{
					user.PASSWORD = model.Password.ToSHA1();
					iDB.Save();
					Msgbox_Toast("密碼已變更!!<br />下次登入請使用新密碼!!");
				}
			}
			else
			{
				SetModelStateError();
			}
			return View();
		}

		#endregion

		#region USER帳號管理
		/// <summary>
		/// 檢查USER_ID有無重覆
		/// </summary>
		/// <param name="USER_ID"></param>
		/// <returns></returns>
		public JsonResult UserExist(string USER_ID)
		{
			bool isValidate = false;
			isValidate = !iDB.IsIDRepeat<USER>(USER_ID);
			return Json(isValidate, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 檢查帳號有無重複
		/// </summary>
		/// <param name="USER_ID">Guid</param>
		/// <param name="CONTENT1">帳號(Email)</param>
		/// <returns></returns>
		public JsonResult UserEmailExist(string USER_ID, string CONTENT1)
		{
			bool isValidate = false;
			isValidate = iDB.CheckEmailRepeat<USER>(USER_ID, CONTENT1);
			return Json(isValidate, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region 群組管理
		/// <summary>
		/// 產生 zNode 權限列表
		/// </summary>
		/// <param name="isCRUD">Create Read Update Delete</param>
		/// <returns></returns>
		public JsonResult AuthorityList(bool isCRUD = false)
		{
			List<zNode> zNodes = new List<zNode>();
			foreach (NODE n in Function.FunctionNodeList
					.Where(p => p.ENABLE.IsEnable() && !p.ID.Equals(FUNCTION_ROOT_NODE_ID)))
			{
				//母功能關掉，子功能也要一併關掉
				NODE parent = Function.GetNode(n.PARENT_ID);
				if (parent != null && parent.ENABLE.IsDisable())
				{
					continue;
				}
				//end
				zNode z = new zNode();
				z.id = n.ID;
				z.name = n.TITLE;
				z.pId = n.PARENT_ID;
				zNodes.Add(z);
				//動態新增 CRUD功能，無連結的也不會有
				if (isCRUD && !n.URL.IsNullOrEmpty())
				{
					foreach (Authority_Right right in Enum.GetValues(typeof(Authority_Right)))
					{
						zNode _z = new zNode();
						_z.id = string.Format("{0}{1}", n.ID, right.ToIntValue());
						_z.name = right.GetDescription();
						_z.pId = n.ID;
						zNodes.Add(_z);
					}
				}
			}
			return Json(zNodes);
		}
		#endregion

		#region Dialog SysUser Search 群組內的使用者

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userKey"></param>
		/// <param name="userType">公司ID</param>
		/// <returns></returns>
		public ActionResult SysUserSearchDialog(string userKey, string userType)
		{
			List<SYSUSER> data = Function.SysUserList
				.Where(p => (string.IsNullOrEmpty(userKey) || p.USER_ID.Contains(userKey) || p.NAME.Contains(userKey) || p.EMAIL.Contains(userKey)) && p.CONTENT30.Equals("AD"))
				.OrderBy(p => p.CONTENT1).ThenBy(p => p.CONTENT2).ThenBy(p => p.CONTENT3).ThenBy(p => p.USER_ID).ToList();
			return PartialView("Search/_SysUserResultPartial", data);
		}

		#endregion

		#region 群組內的使用者 Ajax Delete

		[HttpDelete]
		[ActionLog(TableNameIndex = 12, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult RoleUserDelete(string id)
		{
			if (iDB.DeleteAllRoleUserMapping("", id))
			{
				return Content(Function.DELETE_MESSAGE);
			}
			else
				return JavaScript(Function.ShowAjaxMessage());
		}

		#endregion

		#region SSO 使用者刪除

		[HttpDelete]
		[ActionLog(TableNameIndex = 2, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult ArticleUserDelete(string id)
		{
			if (iDB.Delete<ARTICLE_PLUG>(id))
			{
				return Content(Function.DELETE_MESSAGE);
			}
			else
				return JavaScript(Function.ShowAjaxMessage());
		}

		#endregion

		#region ATTACHMENT Ajax Delete

		[HttpDelete]
		[ActionLog(TableNameIndex = 3, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult AttachmentDelete(string id)
		{
			if (iDB.Delete<ATTACHMENT>(id))
			{
				return Content(Function.DELETE_MESSAGE);
			}
			else
				return JavaScript(Function.ShowAjaxMessage());
		}

		#endregion

		#region PARAGRAPH Ajax Delete

		[HttpDelete]
		[ActionLog(TableNameIndex = 9, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult ParagraphDelete(string id)
		{
			if (iDB.Delete<PARAGRAPH>(id))
			{
				return Content(Function.DELETE_MESSAGE);
			}
			else
				return JavaScript(Function.ShowAjaxMessage());
		}

		#endregion

		#region PLUS Ajax Delete

		[HttpDelete]
		[ActionLog(TableNameIndex = 10, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
		public ActionResult PlusDelete(string id)
		{
			if (iDB.Delete<PLUS>(id))
			{
				return Content(Function.DELETE_MESSAGE);
			}
			else
				return JavaScript(Function.ShowAjaxMessage());
		}

		#endregion

		#region 縣市切換鄉鎮

		public ActionResult CountrySearch(string cityType)
		{
			return PartialView("Search/_CountryResultPartial", Function.NodeList.Where(p => p.PARENT_ID.CheckStringValue(cityType) && p.ENABLE.IsEnable()).ToList());
		}

		public ActionResult GetCounty(string city)
		{
			string strFormat = "<option value={0}>{1} {2}</option>";
			StringBuilder sb = new StringBuilder();
			if (!city.IsNullOrEmpty())
			{
				foreach (NODE county in Function.NodeList.Where(p => city.Equals(p.PARENT_ID)).OrderBy(p => p.ORDER))
				{
					sb.AppendFormat(string.Format(strFormat, county.ID, county.CONTENT1, county.TITLE));
				}
			}
			return Content(sb.ToString());
		}

		public ActionResult GetTown(string id, string v)
		{
			string strFormat = "<option value={0} {3}>{1} {2}</option>";
			StringBuilder sb = new StringBuilder();
			if (!id.IsNullOrEmpty())
			{
				foreach (NODE county in Function.NodeList.Where(p => id.Equals(p.PARENT_ID) && p.ENABLE.IsEnable()).OrderBy(p => p.ORDER))
				{
					sb.AppendFormat(string.Format(strFormat, county.ID, county.CONTENT1, county.TITLE, county.ID.Equals(v) ? "selected" : string.Empty));
				}
			}
			return Content(sb.ToString());
		}

		#endregion

		#region jQuery FileUpload 上傳

		/// <summary>
		/// 不在分類別 直接上傳至Upload
		/// </summary>
		/// <param name="mid"></param>
		/// <param name="uploadType">檔案類型</param>
		/// <param name="c4"></param>
		/// <param name="c5"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult FileUpload(string mid, AttachmentType? uploadType, string c1 = "", string c4 = "", string c5 = "", string fc1 = "")
		{
			foreach (string file in Request.Files)
			{
				for (int i = 0; i < Request.Files.Count; i++)
				{
					HttpPostedFileBase Filedata = Request.Files[i];
					if (Filedata != null && Filedata.ContentLength > 0)
					{
						System.Drawing.Image image = null;
						try
						{
							image = System.Drawing.Image.FromStream(Filedata.InputStream);
						}
						catch
						{
							//用這種方式判斷是因為flash上傳檔案的ContentType 會被變更為 application/octet-stream
						}
						AttachmentType attType = AttachmentType.Image;

						if (!uploadType.ToString().IsNullOrEmpty())
						{
							attType = (AttachmentType)uploadType;
						}
						else
						{
							attType = image != null ? AttachmentType.Image : AttachmentType.File;
						}

						ATTACHMENT att = new ATTACHMENT(Filedata.FileName);
						att.ATT_TYPE = attType.ToIntValue();
						if (attType == AttachmentType.Image && att.EXTENSION.IsNullOrEmpty())
						{
							att.GetRealExtension(image);
						}
						att.SetUpFileName();
						att.MAIN_ID = mid;
						att.CREATER = User.Identity.Name;
						//att.CREATE_DATE = DateTime.Now; //已經在建構時給值
						att.CONTENT9 = EnableType.Enable.ToIntValue();

						//string _attC1 = Request["attC1"].ToMyString();
						//if (!_attC1.IsNullOrEmpty())
						//{
						//    att.CONTENT1 = _attC1;
						//}
						if (!fc1.IsNullOrEmpty()) // Business 業主管理 >　圖片logo / 圖片top1 
						{
							att.CONTENT1 = fc1;
						}

						//備用
						att.CONTENT4 = c4.IsNullOrEmpty() ? null : c4;
						att.CONTENT5 = c5.IsNullOrEmpty() ? null : c5;

						switch (attType)
						{
							case AttachmentType.File:
								//SaveAtt(Filedata, att.FILE_NAME);
								SaveAtt(Filedata, att.FILE_NAME);
								break;
							case AttachmentType.Image:
								SavePicture(new WebImage(Filedata.InputStream), att);
								break;
						}

						if (iDB.Add<ATTACHMENT>(att))
							return Content(att.ID);
					}
				}
			}
			return Content("檔案上傳失敗!!");

		}

		#endregion

		#endregion

		#region 專案用

		public ActionResult GetHallUnit(string hall, string unit, string firstText = "請選擇")
		{
			string strFormat = "<option value=\"{0}\"{1}>{2}</option>";
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(strFormat, "", "", firstText);
			if (!hall.IsNullOrEmpty())
			{
				if (!hall.IsNullOrEmpty() && (new string[] { "其他", "無" }).Contains(hall))
				{
					sb.AppendFormat(strFormat, hall, (unit.IsNullOrEmpty() ? "" : unit.CheckStringValue(hall) ? " selected" : ""), hall);
				}
				else
				{
					List<string> list = Function.SysUserList.Where(p => p.ENABLE.IsEnable() && p.CONTENT1.CheckStringValue(hall))
					.Select(p => p.CONTENT2).Distinct().OrderBy(p => p).ToList();
					if (list.Any())
					{
						foreach (string str in list)
						{
							sb.AppendFormat(strFormat, str, (unit.IsNullOrEmpty() ? "" : unit.CheckStringValue(str) ? " selected" : ""), str);
						}
					}
				}
			}
			return Content(sb.ToString());
		}

		/// <summary>
		/// 取得使用者資料
		/// </summary>
		/// <param name="title">關鍵字</param>
		/// <param name="type">身份</param>
		/// <returns></returns>
		public JsonResult GetUsersData(string title, string type)
		{
			title = title.IsNullOrEmpty() ? string.Empty : title;
			type = type.IsNullOrEmpty() ? string.Empty : type;
			return Json(JsonConvert.SerializeObject(Function.SysUserList.Where(p => p.ENABLE.IsEnable())
				.Select(x => new { Text = string.Format("[{1}] {0}", x.NAME, Function.GetNodeTitle(x.CONTENT2)), Value = x.USER_ID, Selected = false })));
		}

        /// <summary>
		/// 設定會員兌換好禮
		/// </summary>
		/// <returns></returns>
		public  ActionResult SetUserData(string CONTENT1)
        {
            string _creater = User.Identity.Name;
            if (_creater.IsNullOrEmpty() || CONTENT1.IsNullOrEmpty())
            {
                return Content(Function.DEFAULT_ERROR);
            }
            USER _u = iDB.GetByID<USER>(CONTENT1);
            if (_u == null)
            {
                return Content(Function.DEFAULT_ERROR);
            }
            else
            {
                if (_u.CONTENT8.IsNullOrEmpty() && !_u.DATETIME4.HasValue)
                {
                    _u.CONTENT8 = _creater;
                    _u.DATETIME4 = DateTime.Now;
                    iDB.Save();
                    return Content(_u.DATETIME4.ToDefaultString());
                }
                else
                {
                    return Content(Function.DEFAULT_ERROR);
                }
            }
        }

        /// <summary>
        /// 自動完成
        /// </summary>
        /// <param name="iType">類別</param>
        /// <param name="k">關鍵字</param>
        /// <returns></returns>
        public JsonResult fnAutoComplete(int iType, string k)
		{
			List<string> list = new List<string>();
			switch (iType)
			{
				case 1: //檔期管理＞表演者／展覽者
					list = iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: "fun13_05_03").Where(p => p.CONTENT1.Contains(k))
						.Select(p => p.CONTENT1).Distinct().OrderBy(p => p).ToList();
					break;
				case 2: //檔期管理＞活動名稱
					list = iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: "fun13_05_03").Where(p => p.CONTENT7.Contains(k))
						.Select(p => p.CONTENT7).Distinct().OrderBy(p => p).ToList();
					break;
				case 3:

					break;
			}
			return Json(list);
		}

		/// <summary>
		/// 動支登記 取得預算數和已執行數
		/// </summary>
		/// <param name="c1">說明編號</param>
		/// <returns></returns>
		public JsonResult GetBudgetData(string c1)
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			dict["budget"] = 0;
			dict["exec"] = 0;

			NODE n4 = Function.GetNode(c1);
			if (n4 != null)
			{
				dict["budget"] = n4.CONTENT1.ToInt(); //預算數													  
				dict["exec"] = iDB.GetAllAsNoTracking<DATA4>(MAIN_ID: "fun10_02").Where(p => p.CONTENT1.Equals(c1)).Sum(p => p.DECIMAL2).ToInt(); //已執行數
			}
			return Json(dict);
		}

        /// <summary>
		/// 取得Data1資料
		/// </summary>
		/// <param name="title">關鍵字</param>
		/// <param name="type">分類</param>
		/// <returns></returns>
		public JsonResult GetData1Data(string nid,string title, string type)
        {
            title = title.IsNullOrEmpty() ? string.Empty : title;
            type = type.IsNullOrEmpty() ? string.Empty : type;
            List<DATA1> query = iDB.GetAllAsNoTracking<DATA1>().Where(p => !p.NODE_ID.Equals(nid)
             //&& (string.IsNullOrEmpty(title) || p.CONTENT1.Contains(title))
             //&& (string.IsNullOrEmpty(type) || p.DATA_TYPE.Equals(type))
             ).OrderBy(p => p.DATA_TYPE).ThenByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE).ToList();
            return Json(JsonConvert.SerializeObject(query
                .Select(x => new { Text = $"[{x.GetCustomNodeID()}] {x.CONTENT1}", Value = x.ID, Selected = false })));
        }

        public ActionResult GetData2Data(string CONTENT1, string CONTENT6)
        {
            NODE node = iDB.GetByIDAsNoTracking<NODE>(CONTENT1);
            if (node != null)
            {
                ViewBag.C1Node = node;
            }
            List<DATA2> data2List = iDB.GetAllAsNoTracking<DATA2>(MAIN_ID: "fun9000")
                    .Where(p => string.IsNullOrEmpty(CONTENT6) || CONTENT6.Equals(p.CONTENT6))
                    .OrderByDescending(p => p.CREATE_DATE)
                    .ToList();//先根據 語言別 全部取出來
            return PartialView("Search/_Data2SearchResultPartial", data2List);
        }

		public JsonResult SendtodoText(string id,string text,int order)
		{
            string idd = Function.GetGuid();
            DATA1 d1 = iDB.GetAll<DATA1>().Where(p=>p.NODE_ID.Equals("TodoList") && p.CREATER.Equals(id)).FirstOrDefault();
			if (d1 != null)
			{
				PARAGRAPH todo = new PARAGRAPH()
				{
					ID = idd,
					MAIN_ID = d1.ID,
					CREATER = id,
					CREATE_DATE = DateTime.Now,
					CONTENT = text,
					ORDER = order
                };
                IsSuccessful = iDB.Add<PARAGRAPH>(todo);
            }
			else
                IsSuccessful = false;

            return Json(idd);

        }

        public JsonResult DeletetodoText(string id)
        {
			PARAGRAPH p1 = iDB.GetByID<PARAGRAPH>(id);
			string mainid = p1.MAIN_ID;
            iDB.Delete<PARAGRAPH>(id);

            List<PARAGRAPH> ppp = iDB.GetAll<PARAGRAPH>().Where(p => p.MAIN_ID.Equals(mainid)).OrderBy(p => p.ORDER).ToList();
            int i = 1;
			foreach (var ph in ppp)
			{
				ph.ORDER = i;
				i++;
			}
			
			iDB.Save();
            return Json(id);

        }
		public JsonResult checktodoitem(string id,string data)
		{
			PARAGRAPH p1 = iDB.GetByID<PARAGRAPH>(id);
			if (p1 != null)
			{
				p1.CONTENT1 = data;
				iDB.Save();
			}
            return Json(id);
        }
            
        #endregion
    }
}
