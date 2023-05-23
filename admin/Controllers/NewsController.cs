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
    [PresentationTypeSelect]
    public class NewsController : BaseController
    {
        /// <summary>
        /// 檔案上傳大小限制
        /// </summary>
        public const int FILE_UPLOAD_MAX_SIZE = 15 * 1024 * 1024;
        /// <summary>
        /// 圖片上傳大小限制
        /// </summary>
        public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;
        /// <summary>
        /// 前台網站管理 > 最新訊息
        /// </summary>
        public const string fun13_04 = "fun13_04";
        /// <summary>
        /// 光影文化館＞最新公告
        /// </summary>
        public const string fun14_01 = "fun14_01";

        #region 最新訊息＆最新公告
        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult Index(int? page, int? defaultPage, string k, string k1, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.start = start;
            ViewBag.end = end;

            DateTime tmpStart = start.IsNullOrEmpty() ? DateTime.MinValue : start.ToDateTime();
            DateTime tmpEnd = end.IsNullOrEmpty() ? DateTime.MaxValue : end.ToDateTime().AddDays(1);

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            return View(iDB.GetAll<ARTICLE>(MAIN_ID: NodeID)
                .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k)) && (string.IsNullOrEmpty(k1) || p.ARTICLE_TYPE.Equals(k1)) &&
                ((p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd) || (p.DATETIME2 >= tmpStart && p.DATETIME2 < tmpEnd)))
                .OrderByDescending(p => p.DATETIME1).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1, string start, string end, string presType)
        {
            if (id.IsNullOrEmpty() && presType.IsNullOrEmpty())
            {
                AlertMsg = "新增：請先選擇呈現類型！";
                return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "start", "end" }));
            }

            NewsModel model = new NewsModel();
            if (id.IsNullOrEmpty())
            {
                CheckAuthority(Authority_Right.Add);
                //model.ARTICLE_TYPE = (ViewBag.PresentationTypeSelect as SelectList).FirstOrDefault().Value;
                model.ARTICLE_TYPE = presType;
                model.DATETIME1 = DateTime.Now;
                model.DATETIME2 = model.DATETIME1.AddMonths(1);
                model.ATTAs = new List<ATTACHMENT>();
                model.PICs = new List<ATTACHMENT>();
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
                if (a != null)
                {
                    model = new NewsModel()
                    {
                        ID = a.ID,
                        ARTICLE_TYPE = a.ARTICLE_TYPE,
                        ORDER = a.ORDER,
                        CONTENT1 = a.CONTENT1,
                        CONTENT = a.PARAGRAPH.FirstOrDefault().CONTENT,
                        CONTENT11 = a.CONTENT11,
                        DATETIME1 = a.DATETIME1.Value,
                        DATETIME2 = a.DATETIME2.Value,
                        ATTAs = a.ATTACHMENT.Where(p => p.ATT_TYPE.Equals(AttachmentType.File.ToIntValue())).OrderBy(p => p.ORDER).ToList(),
                        PICs = a.ATTACHMENT.Where(p => p.ATT_TYPE.Equals(AttachmentType.Image.ToIntValue())).OrderBy(p => p.ORDER).ToList()
                    };
                }
            }

            ViewBag.k1 = k1;
            ViewBag.start = start;
            ViewBag.end = end;
            ViewBag.presType = presType.IsNullOrEmpty() ? model.ARTICLE_TYPE : presType;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit(string id, NewsModel model, int? page, int? defaultPage, string k, string k1, string start, string end, string presType, bool chk = false)
        {
            CheckAuthority(Authority_Right.Update);
            IsSuccessful = true;
            string sWarningMsg = string.Empty;

            if (model.ARTICLE_TYPE != "PRESENTATION_HTML")
            {
                ModelState.Remove("CONTENT");
            }

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
                    a.PARAGRAPH.Add(new PARAGRAPH()
                    {
                        ID = Function.GetGuid(),
                        CONTENT = model.CONTENT.ToMyString(),
                        CREATE_DATE = DateTime.Now,
                        CREATER = User.Identity.Name
                    });
                }
                else
                {
                    a.UPDATER = User.Identity.Name;
                    a.UPDATE_DATE = DateTime.Now;

                    PARAGRAPH par = a.PARAGRAPH.FirstOrDefault();
                    if (par != null)
                    {
                        par.CONTENT = model.CONTENT.ToMyString();
                        par.UPDATER = User.Identity.Name;
                        par.UPDATE_DATE = DateTime.Now;
                    }
                }
                a.ARTICLE_TYPE = model.ARTICLE_TYPE;
                a.CONTENT1 = model.CONTENT1;
                a.CONTENT11 = model.CONTENT11.ToHttpUrl();
                a.DATETIME1 = model.DATETIME1;
                a.DATETIME2 = (chk ? DateTime.MaxValue : model.DATETIME2);

                List<HttpPostedFileBase> HPF1s = model.HPF1s;
                if (HPF1s != null)
                {
                    foreach (HttpPostedFileBase hpf in HPF1s)
                    {
                        if (hpf == null || hpf.ContentLength <= 0)
                        {
                            continue;
                        }

                        string sExt = Path.GetExtension(hpf.FileName).ToLower();
                        if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
                        {
                            sWarningMsg += hpf.FileName + "：附件大小超過 15 MB！";
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
                            att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: a.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
                            att.CONTENT9 = EnableType.Enable.ToIntValue();
                            a.ATTACHMENT.Add(att);
                            SaveAtt(hpf, att.FILE_NAME);
                        }
                    }
                }

                List<HttpPostedFileBase> HPF2s = model.HPF2s;
                if (HPF2s != null)
                {
                    foreach (HttpPostedFileBase hpf in HPF2s)
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
                            att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: a.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
                            att.CONTENT9 = EnableType.Enable.ToIntValue();
                            a.ATTACHMENT.Add(att);
                            SaveAtt(hpf, att.FILE_NAME);
                        }
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
                    return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "start", "end" }));
                }
            }

            ViewBag.k1 = k1;
            ViewBag.start = start;
            ViewBag.end = end;
            ViewBag.presType = model.ARTICLE_TYPE;

            SetModelStateError(sWarningMsg);
            return View(model);
        }

        [ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string start, string end, bool really = false)
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
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "start", "end" }));
        }

        #endregion
    }
}