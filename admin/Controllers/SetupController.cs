using KingspModel;
using KingspModel.DataModel;
using KingspModel.DB;
using KingspModel.Enum;
using System.Web.Mvc;

namespace admin.Controllers
{
	public class SetupController : BaseController
    {
		public ActionResult Index(string nid)
		{
			if (nid.IsNullOrEmpty()) return GoIndex();
			SetIsEdit(IsAuthority(Authority_Right.Update));

			SetupModel model = new SetupModel();
			NODE n = iDB.GetByID<NODE>(nid);
			if (n != null)
			{
				model.CONTENT1 = n.CONTENT1.ToInt();
			}			
			return View(model);
		}

		[HttpPost]
		public ActionResult Index(string nid, SetupModel model)
		{
			if (nid.IsNullOrEmpty()) return GoIndex();
			SetIsEdit(IsAuthority(Authority_Right.Update));

			NODE n = iDB.GetByID<NODE>(nid);
			if (n != null)
			{
				n.CONTENT1 = (model.CONTENT1 ?? 1).ToString();
				iDB.Save();
			}
			return View(model);
		}
	}
}
