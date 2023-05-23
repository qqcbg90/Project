using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using KingspModel;
using KingspModel.DB;

namespace admin.Filters
{
    /// <summary>
    /// 動作記錄 Log
    /// </summary>
    public sealed class ActionLogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 說明 LOG.CONTENT1
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// DB NAME INDEX
        /// <para>0:NODE</para>
        /// <para>1:ARTICLE</para>
        /// <para>2:ARTICLE_PLUG</para>
        /// <para>3:ATTACHMENT</para>
        /// <para>4:AUTHORITY</para>
        /// <para>5:COUNTER</para>
        /// <para>6:LOG</para>
        /// <para>7:MESSAGE</para>
        /// <para>8:MESSAGE_LOG</para>
        /// <para>9:PARAGRAPH</para>
        /// <para>10:PLUS</para>
        /// <para>11:ROLE_GROUP</para>
        /// <para>12:ROLE_USER_MAPPING</para>
        /// <para>13:SYSUSER</para>
        /// <para>14:USER</para>
        /// <para>15:DATA1</para>
        /// <para>16:DATA2</para>
        /// <para>17:DATA3</para>
        /// <para>18:DATA4</para>
        /// <para>19:DATA5</para>
        /// <para>20:DATA6</para>
        /// <para>21:DATA7</para>
        /// <para>22:DATA8</para>
        /// </summary>
        public int TableNameIndex { get; set; }
        /// <summary>
        /// 是否記錄
        /// </summary>
        bool IsLog = true;

        public ActionLogAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsLog)
            {
                string _description = string.Empty;
                List<string> list = new List<string>() { "Item", "Medium_PicName", "Small_PicName", "Password" };
                using (DBEntities db = new DBEntities())
                {
                    StringBuilder sb = new StringBuilder();
                    string _value = string.Empty;
                    if (filterContext.ActionParameters != null && filterContext.ActionParameters.ContainsKey("model"))
                    {
                        var model = filterContext.ActionParameters["model"];
                        if (null != model)
                        {
                            foreach (var info in model.GetType().GetProperties())
                            {
                                if (list.Contains(info.Name)) continue;//List Model不記錄
                                _value = model.GetType().GetProperty(info.Name).GetValue(model, null).ToMyString();
                                //try
                                //{
                                //    _value = model.GetType().GetProperty(info.Name).GetValue(model, null).ToMyString();
                                //}
                                //catch (Exception ex)
                                //{
                                //    _value = ex.Message;
                                //}
                                if (!_value.Contains("HashSet"))//不記錄這種訊息
                                {
                                    sb.AppendFormat("{0}={1}{2}", info.Name, _value, Environment.NewLine);
                                }
                            }
                        }
                    }
                    string _id = string.Empty;
                    if (filterContext.ActionParameters != null && filterContext.ActionParameters.ContainsKey("id"))
                    {
                        _id = filterContext.ActionParameters["id"].ToMyString();
						if (!_id.IsNullOrEmpty())
						{
							sb.AppendFormat("id={0}{1}", _id, Environment.NewLine);
						}
					}
                    _description = Description;
                    if (!_id.IsNullOrEmpty() && Description.IsNullOrEmpty())
                    {
                        _description = "編輯";
                    }
                    else if (_id.IsNullOrEmpty() && Description.IsNullOrEmpty())
                    {
                        _description = "新增";
                    }
                    LOG log = Function.GetLog(Function.TABLE_NAMES[TableNameIndex], filterContext.HttpContext.User.Identity.Name, _description, _id);
					log.LOG_ID = Function.GetGuid();

					//傳遞LOG_ID
					filterContext.HttpContext.Items["LogID"] = log.LOG_ID;

					log.CONTENT = sb.ToString();
                    //增加ip和agent
                    log.CONTENT4 = filterContext.HttpContext.Request.UserAgent;
                    log.CONTENT5 = filterContext.HttpContext.Request.UserHostAddress;
                    //end
                    db.LOG.Add(log);
                    db.SaveChanges();
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}