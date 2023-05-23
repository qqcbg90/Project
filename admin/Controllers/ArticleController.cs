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
using System.Web.Helpers;
using System.Web.Mvc;

namespace admin.Controllers
{
    [NodeSelect("fun7000_AT")]
    public class ArticleController : BaseController
    {
        #region const property

        /// <summary>
        /// 檔案上傳大小限制
        /// </summary>
        public const int FILE_UPLOAD_MAX_SIZE = 15 * 1024 * 1024;
        /// <summary>
        /// 圖片上傳大小限制
        /// </summary>
        public const int IMG_UPLOAD_MAX_SIZE = 2 * 1024 * 1024;

        #endregion

        #region ARTICLE 活動訊息管理＆常見問題管理
        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult Index(int? page, int? defaultPage, string k, string k1, string start, string end)
        {
            ViewBag.k1 = k1;
            ViewBag.start = start;
            ViewBag.end = end;

            DateTime tmpStart = start.ToFromDate();
            DateTime tmpEnd = end.ToEndDate();

            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            return View($"{NodeID}_Index", iDB.GetAll<ARTICLE>(MAIN_ID: NodeID)
                .Where(p => (string.IsNullOrEmpty(k) || p.CONTENT1.Contains(k)) && (string.IsNullOrEmpty(k1) || p.ARTICLE_TYPE.Equals(k1)) &&
                ((p.DATETIME1 >= tmpStart && p.DATETIME1 < tmpEnd) || (p.DATETIME2 >= tmpStart && p.DATETIME2 < tmpEnd)))
                .OrderByDescending(p => p.DATETIME1).ThenByDescending(p => p.CREATE_DATE).ToPagedList(page.ToMvcPaging(), _defaultPage));
        }

        /// <summary>
        /// 新增/編輯
        /// </summary>
        public ActionResult Edit(string id, int? page, int? defaultPage, string k, string k1, string start, string end)
        {
            ArticleModel model = new ArticleModel();
            if (id.IsNullOrEmpty())
            {
                CheckAuthority(Authority_Right.Add);
                //model.ARTICLE_TYPE = (ViewBag.PresentationTypeSelect as SelectList).FirstOrDefault().Value;
                model.DATETIME1 = DateTime.Today;
                model.Atts = new List<ATTACHMENT>();
                model.Pics = new List<ATTACHMENT>();
            }
            else
            {
                SetIsEdit(IsAuthority(Authority_Right.Update));
                ARTICLE a = iDB.GetByIDAsNoTracking<ARTICLE>(id);
                if (a != null)
                {
                    model = new ArticleModel()
                    {
                        ID = a.ID,
                        ARTICLE_TYPE = a.ARTICLE_TYPE,
                        ORDER = a.ORDER,
                        CONTENT1 = a.CONTENT1,
                        CONTENT2 = a.CONTENT2,
                        CONTENT3 = a.CONTENT3,
                        CONTENT11 = a.CONTENT11,
                        CONTENT12 = a.CONTENT12,
                        CONTENT13 = a.CONTENT13,
                        Bool_DECIMAL1 = a.DECIMAL1 == 1,
                        DATETIME1 = a.DATETIME1,
                        DATETIME2 = a.DATETIME2,
                        Atts = a.ATTACHMENT.Where(p => p.ATT_TYPE.Equals(AttachmentType.File.ToIntValue())).OrderBy(p => p.ORDER).ToList(),
                        Pics = a.ATTACHMENT.Where(p => p.ATT_TYPE.Equals(AttachmentType.Image.ToIntValue())).OrderBy(p => p.ORDER).ToList(),
                        PH0 = a.GetParagraphByOrder(0).CONTENT,
                        PH1 = a.GetParagraphByOrder().CONTENT
                    };
                }
            }

            ViewBag.k1 = k1;
            ViewBag.start = start;
            ViewBag.end = end;

            return View($"{NodeID}_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_EDIT)]
        public ActionResult Edit(string id, ArticleModel model, int? page, int? defaultPage, string k, string k1, string start, string end
            ,List<HttpPostedFileBase> hpfAtt
            , List<HttpPostedFileBase> hpfPic
            )
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
                    a.PARAGRAPH.Add(new PARAGRAPH()
                    {
                        ID = Function.GetGuid(),
                        CONTENT = model.PH0.ToMyString(),
                        CREATE_DATE = DateTime.Now,
                        CREATER = User.Identity.Name,
                        ORDER = 0
                    });
                    a.PARAGRAPH.Add(new PARAGRAPH()
                    {
                        ID = Function.GetGuid(),
                        CONTENT = model.PH1.ToMyString(),
                        CREATE_DATE = DateTime.Now,
                        CREATER = User.Identity.Name,
                        ORDER = 1
                    });
                }
                else
                {
                    a.UPDATER = User.Identity.Name;
                    a.UPDATE_DATE = DateTime.Now;

                    PARAGRAPH par = a.GetParagraphByOrder(0);
                    if (par != null)
                    {
                        par.CONTENT = model.PH0.ToMyString();
                        par.UPDATER = User.Identity.Name;
                        par.UPDATE_DATE = DateTime.Now;
                    }
                    PARAGRAPH par1 = a.GetParagraphByOrder();
                    if (par1 != null)
                    {
                        par1.CONTENT = model.PH1.ToMyString();
                        par1.UPDATER = User.Identity.Name;
                        par1.UPDATE_DATE = DateTime.Now;
                    }
                }
                a.ARTICLE_TYPE = model.ARTICLE_TYPE;
                a.CONTENT1 = model.CONTENT1;
                a.CONTENT2 = model.CONTENT2.ToHttpUrl();
                a.CONTENT3 = model.CONTENT3.ToHttpUrl();
                a.CONTENT11 = model.CONTENT11;
                a.CONTENT12 = model.CONTENT12.ToHttpUrl();
                a.CONTENT13 = model.CONTENT13.ToHttpUrl();
                a.DATETIME1 = model.DATETIME1;
                a.DATETIME2 = model.DATETIME2;
                a.DECIMAL1 = model.Bool_DECIMAL1 ? 1 : 0;

                #region attachment
                if (hpfAtt != null && hpfAtt.Count > 0)
                {
                    foreach (HttpPostedFileBase hpf in hpfAtt)
                    {
                        if (hpf == null || hpf.ContentLength <= 0)
                        {
                            continue;
                        }

                        //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                        //if (hpf.ContentLength > FILE_UPLOAD_MAX_SIZE)
                        //{
                        //    sWarningMsg += hpf.FileName + "：附件大小超過 15 MB！";
                        //}
                        //else if (Function.DEFAULT_FILEUPLOAD_EXT.IndexOf(sExt) == -1)
                        //{
                        //    sWarningMsg += hpf.FileName + "：附件格式不符！";
                        //}
                        if (sWarningMsg.IsNullOrEmpty())
                        {
                            ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                            att.ATT_TYPE = AttachmentType.File.ToIntValue();
                            att.SetUpFileName();
                            att.CREATER = User.Identity.Name;
                            att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: a.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
                            a.ATTACHMENT.Add(att);
                            SaveAtt(hpf, att.FILE_NAME);
                        }
                    }
                }

                if (hpfPic != null && hpfPic.Count > 0)
                {
                    foreach (HttpPostedFileBase hpf in hpfPic)
                    {
                        if (hpf == null || hpf.ContentLength <= 0)
                        {
                            continue;
                        }

                        //string sExt = Path.GetExtension(hpf.FileName).ToLower();
                        //if (hpf.ContentLength > IMG_UPLOAD_MAX_SIZE)
                        //{
                        //    sWarningMsg += hpf.FileName + "：圖片大小超過 2 MB！";
                        //}
                        //else if (Function.DEFAULT_FILEUPLOAD_PICTURE_EXT.IndexOf(sExt) == -1)
                        //{
                        //    sWarningMsg += hpf.FileName + "：圖片格式不符！";
                        //}
                        if (sWarningMsg.IsNullOrEmpty())
                        {
                            ATTACHMENT att = new ATTACHMENT(hpf.FileName);
                            att.ATT_TYPE = AttachmentType.Image.ToIntValue();
                            att.SetUpFileName();
                            att.CREATER = User.Identity.Name;
                            att.ORDER = iDB.GetAllAsNoTracking<ATTACHMENT>(MAIN_ID: a.ID).Where(p => p.ATT_TYPE.Equals(att.ATT_TYPE)).Count() + 1;
                            a.ATTACHMENT.Add(att);
                            SavePicture(new WebImage(hpf.InputStream), att);
                        }
                    }
                }

                #endregion


                if (IsAdd)
                {
                    IsSuccessful = iDB.Add<ARTICLE>(a);
                }
                else
                {
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

            SetModelStateError(sWarningMsg);
            return View($"{NodeID}_Edit", model);
        }

        [ActionLog(TableNameIndex = 1, Description = ACTIONLOG_DISPLAY_NAME_DELETE)]
        public ActionResult Delete(string id, int? page, int? defaultPage, string k, string k1, string start, string end, bool really = false)
        {
            CheckAuthority(Authority_Right.Delete);
            AlertMsg = iDB.Delete<ARTICLE>(id, really) ? Function.DELETE_MESSAGE : Function.DELETE_ERROR_MESSAGE;
            return GoIndex(NodeID, page, defaultPage, k, SetRouteValue(new string[] { "k1", "start", "end" }));
        }

        #endregion
    }
}