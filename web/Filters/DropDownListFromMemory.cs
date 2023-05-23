using System.Linq;
using System.Web.Mvc;
using KingspModel;
using System.Collections.Generic;
using System;
using KingspModel.Enum;

namespace web.Filters
{
    public class SampleMemory : ActionFilterAttribute//Filter過濾器
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //List<auth_group> agList = db.auth_group.OrderBy(p => p.group_name).ToList();
            //List<SelectListItem> groupNameList = new List<SelectListItem>();
            //groupNameList.AddRange(agList.Select(n => new SelectListItem { Text = n.group_name, Value = n.group_id.ToString() }));
            //groupNameList.Add(new SelectListItem { Text = "選全部", Value = "All" });
            //filterContext.Controller.ViewBag.groupNameList = new SelectList(groupNameList, "Value", "Text");
            //base.OnActionExecuting(filterContext);
        }
    }

    #region Node DropDownList ViewBag.NodeSelect

    /// <summary>
    /// 從 Function.NodeList 根據PARENT_ID 取得下拉選單 ViewBag.NodeSelect
    /// <para>如果有2個則使用 ParentID_Index2 ViewBag.NodeSelect2</para>
    /// </summary>
    public class NodeSelect : ActionFilterAttribute
    {
        public NodeSelect(params string[] parentID) { ParentIDs = parentID; }
        private string[] ParentIDs { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            foreach (var item in ParentIDs.Where(p => !string.IsNullOrEmpty(p)))
            {
                List<SelectListItem> items = Function.NodeList
                    .Where(x => x.ENABLE.IsEnable() && x.PARENT_ID.CheckStringValue(item))
                    .Select(x => new SelectListItem() { Text = x.TITLE, Value = x.ID })
                    .ToList();
                filterContext.Controller.ViewData[item] = new SelectList(items, "Value", "Text");
            }
            base.OnActionExecuting(filterContext);
        }
    }



    ///// <summary>
    ///// 功能 NODE_ID 集合
    ///// <para>0 群組管理 group_role</para>
    ///// <para>1 使用者管理 sysUser_role</para>
    ///// <para>2 客戶管理 customer_admin</para>
    ///// <para>3 最新消息	news_admin</para>
    ///// <para>4 華膳之風	cpcs_admin</para>
    ///// <para>5 人員招募	recruit_admin</para>
    ///// <para>6 樣板維護	purchase_sample</para>
    ///// <para>7 分類維護	purchase_type</para>
    ///// <para>8 採購公告維護	purchase</para>
    ///// <para>9 每月招標項目公告 purchase_month</para>
    ///// <para>10 其它計畫採購項目	purchase_other</para>
    ///// <para>11 職業安全衛生管理準則	purchase_safe</para>
    ///// <para>12 樣板維護root	purchase_sample_root</para>
    ///// <para>13 分類維護root	purchase_type_root</para>
    ///// </summary>
    //public int ParentID_Index { get; set; }
    ///// <summary>
    ///// 功能 NODE_ID 集合
    ///// <para>0 群組管理 group_role</para>
    ///// <para>1 使用者管理 sysUser_role</para>
    ///// <para>2 客戶管理 customer_admin</para>
    ///// <para>3 最新消息	news_admin</para>
    ///// <para>4 華膳之風	cpcs_admin</para>
    ///// <para>5 人員招募	recruit_admin</para>
    ///// <para>6 樣板維護	purchase_sample</para>
    ///// <para>7 分類維護	purchase_type</para>
    ///// <para>8 採購公告維護	purchase</para>
    ///// <para>9 每月招標項目公告 purchase_month</para>
    ///// <para>10 其它計畫採購項目	purchase_other</para>
    ///// <para>11 職業安全衛生管理準則	purchase_safe</para>
    ///// <para>12 樣板維護root	purchase_sample_root</para>
    ///// <para>13 分類維護root	purchase_type_root</para>
    ///// </summary>
    //public int ParentID_Index2 { get; set; }
    //public override void OnActionExecuting(ActionExecutingContext filterContext)
    //{
    //    List<SelectListItem> list = Function.NodeList
    //       .Where(p => "".CheckStringValue(p.PARENT_ID) && p.ENABLE.IsEnable())
    //       .OrderByDescending(p => p.ORDER).Select(p => new SelectListItem { Text = p.TITLE, Value = p.ID }).ToList();
    //    filterContext.Controller.ViewBag.NodeSelect = new SelectList(list, "Value", "Text");
    //    if (ParentID_Index2 > 0)
    //    {
    //        List<SelectListItem> list2 = Function.NodeList
    //       .Where(p => "".CheckStringValue(p.PARENT_ID) && p.ENABLE.IsEnable())
    //       .OrderByDescending(p => p.ORDER).Select(p => new SelectListItem { Text = p.TITLE, Value = p.ID }).ToList();
    //        filterContext.Controller.ViewBag.NodeSelect2 = new SelectList(list2, "Value", "Text");
    //    }
    //    base.OnActionExecuting(filterContext);
    //}

}

    #endregion
