using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KingspModel;
using KingspModel.Enum;
using KingspModel.Interface;
using KingspModel.Repository;
using KingspModel.DB;
using System;

namespace admin.Filters
{
	//範例(在Memory裡的"不需要"在Interface產生介面)
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

	#region 縣市DropDownList ViewBag.CitySelect

	/// <summary>
	/// 縣市下拉選單 ViewBag.CitySelect
	/// </summary>
	public class CitySelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Function.NodeList
			   .Where(p => Function.NODE_ID_CITYINFO.CheckStringValue(p.PARENT_ID) && p.ENABLE.IsEnable())
			   .OrderBy(p => p.ORDER).Select(p => new SelectListItem { Text = p.TITLE, Value = p.ID }).ToList();
			filterContext.Controller.ViewBag.CitySelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	#endregion

	/// <summary>
	/// AFMC 館別
	/// </summary>
	public class HallSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//要和 AD 的館別名稱相同，勿改
			List<string> lsHall = new List<string>() { "桃園展演中心", "中壢藝術館", "桃園光影文化館", "其他" };
			List<string> lsOther = Function.SysUserList.Where(p => p.ENABLE.IsEnable()).Select(p => p.CONTENT1).Distinct().OrderBy(p => p).ToList();
			List<SelectListItem> list = lsHall.Union(lsOther).Select(p => new SelectListItem() { Text = p, Value = p }).ToList();
			filterContext.Controller.ViewBag.HallSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	

	/// <summary>
	/// AFMC 公告類別
	/// </summary>
	public class NewsTypeSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Enum.GetValues(typeof(NewsType)).Cast<NewsType>()
				.Select(p => new SelectListItem { Text = p.GetDescription(), Value = p.ToIntValue() }).ToList();
			filterContext.Controller.ViewBag.NewsTypeSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	

	/// <summary>
	/// AFMC 電影國家
	/// </summary>
	public class FilmCountrySelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<string> lsEnum = new List<string>() { "美國", "日本", "韓國", "印度", "台灣", "中國", "香港" };
			List<string> lsOther = (new DBRepository() as IDB).GetAllAsNoTracking<DATA1>(MAIN_ID: "fun14_04_03").Select(p => p.CONTENT7).Distinct().OrderBy(p => p).ToList();
			List<SelectListItem> list = lsEnum.Union(lsOther).Select(p => new SelectListItem() { Text = p, Value = p }).ToList();
			list.Add(new SelectListItem() { Text = "-- 其他 --", Value = "其他" });
			filterContext.Controller.ViewBag.FilmCountrySelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}


	/// <summary>
	/// AFMC 電影放映規格
	/// </summary>
	public class FilmScreeningSpecificationsSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<string> lsEnum = Enum.GetValues(typeof(FilmScreeningSpecifications)).Cast<FilmScreeningSpecifications>().Select(p => p.GetDescription()).ToList();
			List<string> lsOther = (new DBRepository() as IDB).GetAllAsNoTracking<DATA1>(MAIN_ID: "fun14_04_03").Select(p => p.CONTENT8).Distinct().OrderBy(p => p).ToList();
			List<SelectListItem> list = lsEnum.Union(lsOther).Select(p => new SelectListItem() { Text = p, Value = p }).ToList();
			list.Add(new SelectListItem() { Text = "-- 其他 --", Value = "其他" });
			filterContext.Controller.ViewBag.FilmScreeningSpecificationsSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// AFMC 電影發音
	/// </summary>
	public class FilmPronunciationSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<string> lsEnum = Enum.GetValues(typeof(FilmPronunciation)).Cast<FilmPronunciation>().Select(p => p.GetDescription()).ToList();
			List<string> lsOther = (new DBRepository() as IDB).GetAllAsNoTracking<DATA1>(MAIN_ID: "fun14_04_03").Select(p => p.CONTENT9).Distinct().OrderBy(p => p).ToList();
			List<SelectListItem> list = lsEnum.Union(lsOther).Select(p => new SelectListItem() { Text = p, Value = p }).ToList();
			list.Add(new SelectListItem() { Text = "-- 其他 --", Value = "其他" });
			filterContext.Controller.ViewBag.FilmPronunciationSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// AFMC 電影字幕
	/// </summary>
	public class FilmSubtitleSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<string> lsEnum = Enum.GetValues(typeof(FilmSubtitle)).Cast<FilmSubtitle>().Select(p => p.GetDescription()).ToList();
			List<string> lsOther = (new DBRepository() as IDB).GetAllAsNoTracking<DATA1>(MAIN_ID: "fun14_04_03").Select(p => p.CONTENT10).Distinct().OrderBy(p => p).ToList();
			List<SelectListItem> list = lsEnum.Union(lsOther).Select(p => new SelectListItem() { Text = p, Value = p }).ToList();
			list.Add(new SelectListItem() { Text = "-- 其他 --", Value = "其他" });
			filterContext.Controller.ViewBag.FilmSubtitleSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// AFMC 電影色彩
	/// </summary>
	public class FilmColorSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Enum.GetValues(typeof(FilmColor)).Cast<FilmColor>()
				.Select(p => new SelectListItem { Text = p.GetDescription(), Value = p.ToIntValue() }).ToList();
			filterContext.Controller.ViewBag.FilmColorSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// AFMC 檔期管理 - 入場方式類別
	/// </summary>
	public class AdmissionTypeSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Enum.GetValues(typeof(AdmissionType)).Cast<AdmissionType>()
				.Select(p => new SelectListItem { Text = p.GetDescription(), Value = p.ToString() }).ToList();
			filterContext.Controller.ViewBag.AdmissionTypeSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	#region YearSelect
	public class YearSelect : ActionFilterAttribute
	{
		/// <summary>
		/// true: 由大至小
		/// </summary>
		public bool ORDER_BY_DESC { get; set; }
		/// <summary>
		/// 最小年度：預設是1945
		/// </summary>
		public int MIN_YEAR { get; set; }
		/// <summary>
		/// 最大年度：預設是今年
		/// </summary>
		public int MAX_YEAR { get; set; }
		/// <summary>
		/// true: OPTION TEXT = 民國年
		/// </summary>
		public bool SHOW_MINGO_TEXT { get; set; }
		/// <summary>
		/// true: OPTION VALUE = 民國年
		/// </summary>
		public bool SHOW_MINGO_VALUE { get; set; }
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = new List<SelectListItem>();
			MIN_YEAR = MIN_YEAR == 0 ? 1945 : MIN_YEAR;
			MAX_YEAR = MAX_YEAR == 0 ? DateTime.Now.Year + 1 : MAX_YEAR;
			if (SHOW_MINGO_TEXT)
			{
				for (int y = MIN_YEAR - 1911; y <= MAX_YEAR - 1911; y++)
				{
					list.Add(new SelectListItem() { Value = (SHOW_MINGO_VALUE ? y.ToString("000") : (y + 1911).ToString()), Text = y.ToString("000") });
				}
			}
			else
			{
				for (int y = MIN_YEAR; y <= MAX_YEAR; y++)
				{
					list.Add(new SelectListItem() { Value = (SHOW_MINGO_VALUE ? (y - 1911).ToString("000") : y.ToString()), Text = y.ToString() });
				}
			}
			filterContext.Controller.ViewBag.YearSelect = new SelectList(ORDER_BY_DESC ? list.OrderByDescending(p => p.Value).ToList() : list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}
	#endregion

	#region MonthSelect
	public class MonthSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = new List<SelectListItem>();
			for (int m = 1; m <= 12; m++)
			{
				list.Add(new SelectListItem() { Value = m.ToString(), Text = m.ToString("00") });
			}
			filterContext.Controller.ViewBag.MonthSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

    public class HourSelect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int m = 1; m <= 24; m++)
            {
                list.Add(new SelectListItem() { Value = m.ToString(), Text = m.ToString("00") });
            }
            filterContext.Controller.ViewBag.HourSelect = new SelectList(list, "Value", "Text");
            base.OnActionExecuting(filterContext);
        }
    }
    #endregion
    public class MinutesSelect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int m = 1; m <= 60; m++)
            {
                list.Add(new SelectListItem() { Value = m.ToString(), Text = m.ToString("00") });
            }
            filterContext.Controller.ViewBag.MinutesSelect = new SelectList(list, "Value", "Text");
            base.OnActionExecuting(filterContext);
        }
    }
    

    /// <summary>
    /// AFMC 動支預算 - 組別
    /// </summary>
    public class BudgetItemSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Function.NodeList.Where(p => p.ENABLE.IsEnable() && !string.IsNullOrEmpty(p.PARENT_ID) && p.PARENT_ID.Equals("BUDGET_ITEM"))
				.OrderByDescending(p => p.ORDER).ThenBy(p => p.TITLE)
				.Select(p => new SelectListItem { Text = p.ORDER + "年 " + p.TITLE, Value = p.ID }).ToList();
			filterContext.Controller.ViewBag.BudgetItemSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

	/// <summary>
	/// AFMC 主題影展
	/// </summary>
	public class FilmExhibitionSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Function.NodeList.Where(p => p.ENABLE.IsEnable() && !string.IsNullOrEmpty(p.PARENT_ID) && p.PARENT_ID.Equals(Function.FILM_EXHIBITION))
				.OrderByDescending(p => p.CONTENT1)
				.Select(p => new SelectListItem { Text = p.TITLE + "(" + p.CONTENT1 + "~" + p.CONTENT2 + ")", Value = p.ID }).ToList();
			filterContext.Controller.ViewBag.FilmExhibitionSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

    #region 水水桃花園
    /// <summary>
    /// 呈現類別 PresentationTypeSelect
    /// </summary>
    public class PresentationTypeSelect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<SelectListItem> list = Function.NodeList.Where(p => p.ENABLE.IsEnable() && !string.IsNullOrEmpty(p.PARENT_ID) && p.PARENT_ID.Equals("PRESENTATION_TYPE"))
                .OrderBy(p => p.ORDER)
                .Select(p => new SelectListItem { Text = p.TITLE, Value = p.ID }).ToList();
            filterContext.Controller.ViewBag.PresentationTypeSelect = new SelectList(list, "Value", "Text");
            base.OnActionExecuting(filterContext);
        }
    }

	/// <summary>
	/// 身分別 IdentityTypeSelect
	/// </summary>
	public class IdentityTypeSelect : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			List<SelectListItem> list = Enum.GetValues(typeof(IdentityType)).Cast<IdentityType>()
				.Select(p => new SelectListItem { Text = p.GetDescription(), Value = p.ToIntValue() }).ToList();
			filterContext.Controller.ViewBag.IdentityTypeSelect = new SelectList(list, "Value", "Text");
			base.OnActionExecuting(filterContext);
		}
	}

    /// <summary>
	/// 是否解說
	/// </summary>
	public class Decimal2TypeSelect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<SelectListItem> list = Enum.GetValues(typeof(Decimal2Status)).Cast<Decimal2Status>()
                .Select(p => new SelectListItem { Text = p.GetDescription(), Value = p.ToIntValue() }).ToList();
            filterContext.Controller.ViewBag.Decimal2TypeSelect = new SelectList(list, "Value", "Text");
            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
	/// 訂單狀態
	/// </summary>
	public class OrderStatusSelect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<SelectListItem> list = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>()
                .Select(p => new SelectListItem { Text = p.GetDescription(), Value = p.ToIntValue() }).ToList();
            filterContext.Controller.ViewBag.OrderStatusSelect = new SelectList(list, "Value", "Text");
            base.OnActionExecuting(filterContext);
        }
    }

    #endregion
}

