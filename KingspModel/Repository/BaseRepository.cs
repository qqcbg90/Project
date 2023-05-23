using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KingspModel.DB;

namespace KingspModel.Repository
{
    /// <summary>
    /// BaseRepository
    /// </summary>
    public abstract class BaseRepository : IDisposable
    {
        #region const property
        /// <summary>
        /// Enable 由 1 to 0
        /// </summary>
        protected const string ENABLE_TO_DISABLE = "Enable 由 1 to 0";
        /// <summary>
        /// Enable 由 0 to 1
        /// </summary>
        protected const string DISABLE_TO_ENABLE = "Enable 由 0 to 1";
        /// <summary>
        /// 未命名
        /// </summary>
        protected const string UNKNOW_NAME = "未命名";

        #endregion

        #region protected property

        /// <summary>
        /// 新結構
        /// </summary>
        protected DBEntities db;
        /// <summary>
        /// 是否儲存成功
        /// </summary>
        protected bool IsSave;

        #endregion

        bool isDisposed = false;

        protected BaseRepository()
        {
            db = new DBEntities();
            ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 5 * 60;
        }

        #region Function

        /// <summary>
        /// 儲存修改
        /// </summary>
        /// <returns></returns>
        protected virtual bool Save()
        {
#if DEBUG
            /*
			 * http://sandeep-tada.blogspot.tw/2012/10/validation-failed-for-one-or-more.html
			 * http://contest-start.blogspot.tw/2013/04/visual-studio-web-aspnet.html
			*/
            try
            {
                IsSave = db.SaveChanges() > 0;
            }
            catch (DbEntityValidationException e)
            {
                IsSave = false;
                //可在「輸出>偵錯」查看
                //如果要用LogSystem記錄的話 外部記得初始化跟關閉
                foreach (var v1 in e.EntityValidationErrors)
                {
                    string eEntity = string.Format("※Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", v1.Entry.Entity.GetType(), v1.Entry.State);
                    Debug.WriteLine(eEntity);
                    //LogSystem.WriteLine(eEntity);
                    foreach (var v2 in v1.ValidationErrors)
                    {
                        string validateError = string.Format("※Property: {0}, Error: {1}", v2.PropertyName, v2.ErrorMessage);
                        Debug.WriteLine(validateError);
                        //LogSystem.WriteLine(validateError);
                    }
                }
            }
            return IsSave;
#else
			return db.SaveChanges() > 0;
#endif
        }
        /// <summary>
        /// 刪除實體檔案
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected virtual bool DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                LogSystem.InitLogSystem();
                LogSystem.WriteLine(ex.Message);
                LogSystem.CloseUnderlayingStream();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得Table 資料總筆數
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual Int64 GetTableCount(string tableName)
        {
            return db.Database.SqlQuery<Int64>(@"SELECT ISNULL((select sum (spart.rows) from sys.partitions spart where spart.object_id=object_id({0}) and spart.index_id < 2),0)", tableName).First();
        }

        /// <summary>
        /// 直接執行Build好的 sqlcommand
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        protected virtual int ExecuteSqlCommand(string sqlCommand)
        {
            return db.Database.ExecuteSqlCommand(sqlCommand);
        }

        #endregion

        #region IDisposable 成員

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        ~BaseRepository()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
                isDisposed = true;
            }
        }
    }
}
