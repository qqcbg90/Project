using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingspModel.DB;
using KingspModel.Enum;
using KingspModel.DataModel;

namespace KingspModel.Interface
{
    /// <summary>
    /// 資料庫Interface
    /// </summary>
    public interface IDB
    {
        #region 共用的
        /// <summary>
        /// 新增 CREATER 記得要給
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Add<T>(T model);
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="really">是否真的刪除(預設:否)</param>
        /// <returns></returns>
        bool Delete<T>(string id, bool really = false);
        /// <summary>
        /// 取得全部資料(條件可根據實際情形自訂)
        /// </summary>
        /// <param name="isEnable">是否要判斷Enable</param>
        /// <returns></returns>
        IQueryable<T> GetAll<T>(bool isEnable = true, string MAIN_ID = "");
		/// <summary>
		/// 使用時機：1.需完整取出剛寫入DB的資料物件(忽略快取資料) 2.當查詢結果僅供檢視，沒有異動資料的需求時
		/// AsNoTracking：查詢資料未列入追蹤中，查詢速度會有較佳的表現
		/// </summary>
		IQueryable<T> GetAllAsNoTracking<T>(bool isEnable = true, string MAIN_ID = "");
		/// <summary>
		/// 根據id取得實體
		/// </summary>
		/// <param name="id"></param>
		/// <param name="isEnable">是否要判斷Enable</param>
		/// <returns></returns>
		T GetByID<T>(string id, bool isEnable = true);
		/// <summary>
		/// 使用時機：1.需完整取出剛寫入DB的資料物件(忽略快取資料) 2.當查詢結果僅供檢視，沒有異動資料的需求時
		/// AsNoTracking：查詢資料未列入追蹤中，查詢速度會有較佳的表現
		/// </summary>
		T GetByIDAsNoTracking<T>(string id, bool isEnable = true);
		/// <summary>
		/// PK值是否重覆
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool IsIDRepeat<T>(string id);
        /// <summary>
        /// 儲存
        /// </summary>
        /// <returns></returns>
        bool Save();
        /// <summary>
        /// 直接執行Build好的sqlCommand
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sqlCommand);
        /// <summary>
        /// 自訂Sql
        /// </summary>
        /// <param name="dictCols"></param>
        /// <param name="SqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        dynamic ExecuteCustomSql(Dictionary<string, Type> dictCols, string SqlStr, params object[] parameters);
        #endregion

        #region NODE

        
        /// <summary>
        /// 取得 NODE List 包含 id 下層的所有NODE By SQL(遞迴指令)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<NODE> GetAllNodesBySQL(string id);
        #endregion

        #region ROLE_GROUP

        /// <summary>
        /// 刪除群組下的所有權限 或特定NODE_ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nid">特定NODE_ID</param>
        /// <returns></returns>
        bool DeleteAllAuthority(string id, string nid = "");
        /// <summary>
        /// 刪除群組下對應的所有使用者 或 特定的
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mid">特定使用者mappingID</param>
        /// <returns></returns>
        bool DeleteAllRoleUserMapping(string id, string mid = "");
        #endregion

        #region SYSUSER、USER

        ///<summary>
        /// 取得登入狀態 (AUTH_MODE=DB)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        LogOnStatus ValidateLogOn<T>(LogOnModel model);
		///<summary>
		/// 20200811 ting add
		/// 取得登入狀態 (AUTH_MODE=AD)
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		LogOnStatus ValidateLogOn_AD<T>(LogOnModel model);
		/// <summary>
		/// 檢查email重覆(可以跟自己重覆)(泛型)
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		bool CheckEmailRepeat<T>(string userName,string email);
        /// <summary>
        /// 更新AD使用者
        /// </summary>
        void GetADSysuser();

        #endregion

    }
}
