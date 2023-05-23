using System;
using System.Collections.Generic;
using System.Linq;
using KingspModel.DB;
using KingspModel.Interface;
using KingspModel.Enum;
using KingspModel;
using System.Reflection.Emit;
using KingspModel.DataModel;
using System.DirectoryServices;
using System.Data;
using System.Text;

namespace KingspModel.Repository
{
	public sealed class DBRepository : BaseRepository, IDB
	{
		#region const property

		#endregion

		#region 共用
		IQueryable<T> IDB.GetAll<T>(bool isEnable, string MAIN_ID)
		{
			IQueryable query = null;
			switch (typeof(T).Name)
			{
				case "NODE":
					query = db.NODE.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.PARENT_ID.Equals(MAIN_ID))).OrderBy(p => p.ORDER);
					break;
				case "ARTICLE":
					query = db.ARTICLE.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ARTICLE_PLUG":
					query = db.ARTICLE_PLUG.Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.ARTICLE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ATTACHMENT":
					query = db.ATTACHMENT.Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "AUTHORITY":
					query = db.AUTHORITY.Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.ROLE_GROUP_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "COUNTER":
					query = db.COUNTER.Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.TYPE.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "LOG":
					query = db.LOG.OrderByDescending(p => p.CREATE_DATE);
					break;
				case "MESSAGE":
					query = db.MESSAGE.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "MESSAGE_LOG":
					query = db.MESSAGE_LOG.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.MESSAGE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "PARAGRAPH":
					query = db.PARAGRAPH.Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "PLUS":
					query = db.PLUS.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ROLE_GROUP":
					query = db.ROLE_GROUP.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.GROUP_TYPE.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ROLE_USER_MAPPING":
					query = db.ROLE_USER_MAPPING.Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.ROLE_GROUP_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "SYSUSER":
					query = db.SYSUSER.Where(p => (!isEnable || p.ENABLE == 1)).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "USER":
					query = db.USER.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA1":
					query = db.DATA1.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA2":
					query = db.DATA2.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA3":
					query = db.DATA3.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA4":
					query = db.DATA4.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA5":
					query = db.DATA5.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA6":
					query = db.DATA6.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA7":
					query = db.DATA7.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA8":
					query = db.DATA8.Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
			}
			return query as IQueryable<T>;
		}

		IQueryable<T> IDB.GetAllAsNoTracking<T>(bool isEnable, string MAIN_ID)
		{
			IQueryable query = null;
			switch (typeof(T).Name)
			{
				case "NODE":
					query = db.NODE.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.PARENT_ID.Equals(MAIN_ID))).OrderBy(p => p.ORDER);
					break;
				case "ARTICLE":
					query = db.ARTICLE.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ARTICLE_PLUG":
					query = db.ARTICLE_PLUG.AsNoTracking().Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.ARTICLE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ATTACHMENT":
					query = db.ATTACHMENT.AsNoTracking().Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "AUTHORITY":
					query = db.AUTHORITY.AsNoTracking().Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.ROLE_GROUP_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "COUNTER":
					query = db.COUNTER.AsNoTracking().Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.TYPE.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "LOG":
					query = db.LOG.AsNoTracking().OrderByDescending(p => p.CREATE_DATE);
					break;
				case "MESSAGE":
					query = db.MESSAGE.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "MESSAGE_LOG":
					query = db.MESSAGE_LOG.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.MESSAGE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "PARAGRAPH":
					query = db.PARAGRAPH.AsNoTracking().Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "PLUS":
					query = db.PLUS.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ROLE_GROUP":
					query = db.ROLE_GROUP.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.GROUP_TYPE.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "ROLE_USER_MAPPING":
					query = db.ROLE_USER_MAPPING.AsNoTracking().Where(p => (string.IsNullOrEmpty(MAIN_ID) || p.ROLE_GROUP_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "SYSUSER":
					query = db.SYSUSER.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1)).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "USER":
					query = db.USER.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.MAIN_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA1":
					query = db.DATA1.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA2":
					query = db.DATA2.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA3":
					query = db.DATA3.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA4":
					query = db.DATA4.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA5":
					query = db.DATA5.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA6":
					query = db.DATA6.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA7":
					query = db.DATA7.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
				case "DATA8":
					query = db.DATA8.AsNoTracking().Where(p => (!isEnable || p.ENABLE == 1) && (string.IsNullOrEmpty(MAIN_ID) || p.NODE_ID.Equals(MAIN_ID))).OrderByDescending(p => p.CREATE_DATE);
					break;
			}
			return query as IQueryable<T>;
		}

		bool IDB.Add<T>(T model)
		{
			if (model == null) return false;
			switch (typeof(T).Name)
			{
				case "NODE":
					NODE node = model as NODE;
					if (node.ID.IsNullOrEmpty()) node.ID = Function.GetGuid();
					if (node.TITLE.IsNullOrEmpty()) node.TITLE = UNKNOW_NAME;
					node.ENABLE = EnableType.Enable.ToByteValue();
					if (node.CREATE_DATE == DateTime.MinValue)
						node.CREATE_DATE = DateTime.Now;
					if (node.ORDER == 0) node.ORDER = GetTableCount(Function.TABLE_NAMES[0]).ToInt() + 1;
					db.NODE.Add(node);
					if (!Save())
					{
						db.NODE.Remove(node);
					}
					break;
				case "ARTICLE":
					ARTICLE article = model as ARTICLE;
					if (article.NODE_ID.IsNullOrEmpty()) return false;
					if (article.ID.IsNullOrEmpty()) article.ID = Function.GetGuid();
					if (article.CREATE_DATE == DateTime.MinValue)
						article.CREATE_DATE = DateTime.Now;
					article.ENABLE = EnableType.Enable.ToByteValue();
					//if (article.ORDER == 0) article.ORDER = GetTableCount(Function.TABLE_NAMES[1]).ToInt() + 1; //20200612 comment by ting
					db.ARTICLE.Add(article);
					if (!Save())
					{
						db.ARTICLE.Remove(article);
					}
					break;
				case "ARTICLE_PLUG":
					ARTICLE_PLUG article_plug = model as ARTICLE_PLUG;
					if (article_plug == null || article_plug.ARTICLE_ID.IsNullOrEmpty()) return false;
					if (article_plug.ID.IsNullOrEmpty()) article_plug.ID = Function.GetGuid();
					if (article_plug.CREATE_DATE == DateTime.MinValue)
						article_plug.CREATE_DATE = DateTime.Now;
					db.ARTICLE_PLUG.Add(article_plug);
					if (!Save())
					{
						db.ARTICLE_PLUG.Remove(article_plug);
					}
					break;
				case "ATTACHMENT":
					ATTACHMENT attachment = model as ATTACHMENT;
					if (attachment == null || attachment.MAIN_ID.IsNullOrEmpty()) return false;
					if (attachment.ID.IsNullOrEmpty()) attachment.ID = Function.GetGuid();
					if (attachment.CREATE_DATE == DateTime.MinValue)
						attachment.CREATE_DATE = DateTime.Now;
					db.ATTACHMENT.Add(attachment);
					if (!Save())
					{
						db.ATTACHMENT.Remove(attachment);
					}
					break;
				case "AUTHORITY":
					AUTHORITY authority = model as AUTHORITY;
					if (authority == null || authority.NODE_ID.IsNullOrEmpty() || authority.ROLE_GROUP_ID.IsNullOrEmpty()) return false;
					if (authority.ID.IsNullOrEmpty()) authority.ID = Function.GetGuid();
					authority.CREATE_DATE = DateTime.Now;
					db.AUTHORITY.Add(authority);
					if (!Save())
					{
						db.AUTHORITY.Remove(authority);
					}
					break;
				case "COUNTER":
					COUNTER counter = model as COUNTER;
					if (counter == null || counter.TYPE.IsNullOrEmpty()) return false;
					counter.CREATE_DATE = DateTime.Now;
					db.COUNTER.Add(counter);
					if (!Save())
					{
						db.COUNTER.Remove(counter);
					}
					break;
				case "LOG":
					LOG log = model as LOG;
					if (log == null) return false;
					if (log.LOG_ID.IsNullOrEmpty()) log.LOG_ID = Function.GetGuid();
					log.CREATE_DATE = DateTime.Now;
					if (log.TABLE_ID.IsNullOrEmpty()) log.TABLE_ID = UNKNOW_NAME;
					if (log.TABLE_NAME.IsNullOrEmpty()) log.TABLE_NAME = UNKNOW_NAME;
					if (log.USER_ID.IsNullOrEmpty()) log.USER_ID = UNKNOW_NAME;
					db.LOG.Add(log);
					if (!Save())
					{
						db.LOG.Remove(log);
					}
					break;
				case "MESSAGE":
					MESSAGE message = model as MESSAGE;
					if (message == null || message.MSG_TYPE.IsNullOrEmpty()) return false;
					if (message.ID.IsNullOrEmpty()) message.ID = Function.GetGuid();
					message.ENABLE = EnableType.Enable.ToByteValue();
					if (message.CREATE_DATE == DateTime.MinValue)
						message.CREATE_DATE = DateTime.Now;
					db.MESSAGE.Add(message);
					if (!Save())
					{
						db.MESSAGE.Remove(message);
					}
					break;
				case "MESSAGE_LOG":
					MESSAGE_LOG message_log = model as MESSAGE_LOG;
					if (message_log == null || message_log.MESSAGE_ID.IsNullOrEmpty() || message_log.USER_ID.IsNullOrEmpty()) return false;
					if (message_log.ID.IsNullOrEmpty()) message_log.ID = Function.GetGuid();
					message_log.ENABLE = EnableType.Enable.ToByteValue();
					if (message_log.CREATE_DATE == DateTime.MinValue)
						message_log.CREATE_DATE = DateTime.Now;
					db.MESSAGE_LOG.Add(message_log);
					if (!Save())
					{
						db.MESSAGE_LOG.Remove(message_log);
					}
					break;
				case "PARAGRAPH":
					PARAGRAPH paragraph = model as PARAGRAPH;
					if (paragraph == null || paragraph.MAIN_ID.IsNullOrEmpty()) return false;
					if (paragraph.ID.IsNullOrEmpty()) paragraph.ID = Function.GetGuid();
					if (paragraph.CREATE_DATE == DateTime.MinValue)
						paragraph.CREATE_DATE = DateTime.Now;
					db.PARAGRAPH.Add(paragraph);
					if (!Save())
					{
						db.PARAGRAPH.Remove(paragraph);
					}
					break;
				case "PLUS":
					PLUS plus = model as PLUS;
					if (plus == null || plus.MAIN_ID.IsNullOrEmpty()) return false;
					if (plus.CREATE_DATE == DateTime.MinValue)
						plus.CREATE_DATE = DateTime.Now;
					plus.ENABLE = EnableType.Enable.ToByteValue();
					db.PLUS.Add(plus);
					if (!Save())
					{
						db.PLUS.Remove(plus);
					}
					break;
				case "ROLE_GROUP":
					ROLE_GROUP role_group = model as ROLE_GROUP;
					if (role_group == null) return false;
					if (role_group.ID.IsNullOrEmpty()) role_group.ID = Function.GetGuid();
					if (role_group.TITLE.IsNullOrEmpty()) role_group.TITLE = UNKNOW_NAME;
					role_group.CREATE_DATE = DateTime.Now;
					role_group.ENABLE = EnableType.Enable.ToByteValue();
					db.ROLE_GROUP.Add(role_group);
					if (!Save())
					{
						db.ROLE_GROUP.Remove(role_group);
					}
					break;
				case "ROLE_USER_MAPPING":
					ROLE_USER_MAPPING role_user_mapping = model as ROLE_USER_MAPPING;
					if (role_user_mapping == null) return false;
					if (role_user_mapping.ID.IsNullOrEmpty()) role_user_mapping.ID = Function.GetGuid();
					role_user_mapping.CREATE_DATE = DateTime.Now;
					db.ROLE_USER_MAPPING.Add(role_user_mapping);
					if (!Save())
					{
						db.ROLE_USER_MAPPING.Remove(role_user_mapping);
					}
					break;
				case "SYSUSER":
					SYSUSER sysuser = model as SYSUSER;
					if (sysuser == null) return false;
					if (sysuser.USER_ID.IsNullOrEmpty()) sysuser.USER_ID = Function.GetGuid();
					sysuser.CREATE_DATE = DateTime.Now;
					sysuser.ENABLE = EnableType.Enable.ToByteValue();
					db.SYSUSER.Add(sysuser);
					if (!Save())
					{
						db.SYSUSER.Remove(sysuser);
					}
					break;
				case "USER":
					USER user = model as USER;
					if (user == null) return false;
					if (user.USER_ID.IsNullOrEmpty()) user.USER_ID = Function.GetGuid();
					if (user.MAIN_ID.IsNullOrEmpty()) user.MAIN_ID = UNKNOW_NAME;
					user.CREATE_DATE = DateTime.Now;
					user.ENABLE = EnableType.Enable.ToByteValue();
					db.USER.Add(user);
					if (!Save())
					{
						db.USER.Remove(user);
					}
					break;
				case "DATA1":
					DATA1 data1 = model as DATA1;
					if (data1 == null) return false;
					if (data1.ID.IsNullOrEmpty()) data1.ID = Function.GetGuid();
					if (data1.NODE_ID.IsNullOrEmpty()) data1.NODE_ID = UNKNOW_NAME;
					data1.CREATE_DATE = DateTime.Now;
					data1.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA1.Add(data1);
					if (!Save())
					{
						db.DATA1.Remove(data1);
					}
					break;
				case "DATA2":
					DATA2 data2 = model as DATA2;
					if (data2 == null) return false;
					if (data2.ID.IsNullOrEmpty()) data2.ID = Function.GetGuid();
					if (data2.NODE_ID.IsNullOrEmpty()) data2.NODE_ID = UNKNOW_NAME;
					data2.CREATE_DATE = DateTime.Now;
					data2.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA2.Add(data2);
					if (!Save())
					{
						db.DATA2.Remove(data2);
					}
					break;
				case "DATA3":
					DATA3 data3 = model as DATA3;
					if (data3 == null) return false;
					if (data3.ID.IsNullOrEmpty()) data3.ID = Function.GetGuid();
					if (data3.NODE_ID.IsNullOrEmpty()) data3.NODE_ID = UNKNOW_NAME;
					data3.CREATE_DATE = DateTime.Now;
					data3.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA3.Add(data3);
					if (!Save())
					{
						db.DATA3.Remove(data3);
					}
					break;
				case "DATA4":
					DATA4 data4 = model as DATA4;
					if (data4 == null) return false;
					if (data4.ID.IsNullOrEmpty()) data4.ID = Function.GetGuid();
					if (data4.NODE_ID.IsNullOrEmpty()) data4.NODE_ID = UNKNOW_NAME;
					data4.CREATE_DATE = DateTime.Now;
					data4.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA4.Add(data4);
					if (!Save())
					{
						db.DATA4.Remove(data4);
					}
					break;
				case "DATA5":
					DATA5 data5 = model as DATA5;
					if (data5 == null) return false;
					if (data5.ID.IsNullOrEmpty()) data5.ID = Function.GetGuid();
					if (data5.NODE_ID.IsNullOrEmpty()) data5.NODE_ID = UNKNOW_NAME;
					data5.CREATE_DATE = DateTime.Now;
					data5.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA5.Add(data5);
					if (!Save())
					{
						db.DATA5.Remove(data5);
					}
					break;
				case "DATA6":
					DATA6 data6 = model as DATA6;
					if (data6 == null) return false;
					if (data6.ID.IsNullOrEmpty()) data6.ID = Function.GetGuid();
					if (data6.NODE_ID.IsNullOrEmpty()) data6.NODE_ID = UNKNOW_NAME;
					data6.CREATE_DATE = DateTime.Now;
					data6.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA6.Add(data6);
					if (!Save())
					{
						db.DATA6.Remove(data6);
					}
					break;
				case "DATA7":
					DATA7 data7 = model as DATA7;
					if (data7 == null) return false;
					if (data7.ID.IsNullOrEmpty()) data7.ID = Function.GetGuid();
					if (data7.NODE_ID.IsNullOrEmpty()) data7.NODE_ID = UNKNOW_NAME;
					data7.CREATE_DATE = DateTime.Now;
					data7.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA7.Add(data7);
					if (!Save())
					{
						db.DATA7.Remove(data7);
					}
					break;
				case "DATA8":
					DATA8 data8 = model as DATA8;
					if (data8 == null) return false;
					if (data8.ID.IsNullOrEmpty()) data8.ID = Function.GetGuid();
					if (data8.NODE_ID.IsNullOrEmpty()) data8.NODE_ID = UNKNOW_NAME;
					data8.CREATE_DATE = DateTime.Now;
					data8.ENABLE = EnableType.Enable.ToByteValue();
					db.DATA8.Add(data8);
					if (!Save())
					{
						db.DATA8.Remove(data8);
					}
					break;
			}
			return IsSave;
		}

		T IDB.GetByID<T>(string id, bool isEnable)
		{
			IQueryable query = null;
			switch (typeof(T).Name)
			{
				case "NODE":
					query = db.NODE.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ARTICLE":
					query = db.ARTICLE.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ARTICLE_PLUG":
					query = db.ARTICLE_PLUG.Where(p => p.ID.Equals(id));
					break;
				case "ATTACHMENT":
					query = db.ATTACHMENT.Where(p => p.ID.Equals(id));
					break;
				case "AUTHORITY":
					query = db.AUTHORITY.Where(p => p.ID.Equals(id));
					break;
				case "COUNTER":
					query = db.COUNTER.Where(p => p.SNO.Equals(id));
					break;
				case "LOG":
					query = db.LOG.Where(p => p.LOG_ID.Equals(id));
					break;
				case "MESSAGE":
					query = db.MESSAGE.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "MESSAGE_LOG":
					query = db.MESSAGE_LOG.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "PARAGRAPH":
					query = db.PARAGRAPH.Where(p => p.ID.Equals(id));
					break;
				case "PLUS":
					query = db.PLUS.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ROLE_GROUP":
					query = db.ROLE_GROUP.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ROLE_USER_MAPPING":
					query = db.ROLE_USER_MAPPING.Where(p => p.ID.Equals(id));
					break;
				case "SYSUSER":
					query = db.SYSUSER.Where(p => p.USER_ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "USER":
					query = db.USER.Where(p => p.USER_ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA1":
					query = db.DATA1.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA2":
					query = db.DATA2.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA3":
					query = db.DATA3.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA4":
					query = db.DATA4.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA5":
					query = db.DATA5.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA6":
					query = db.DATA6.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA7":
					query = db.DATA7.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA8":
					query = db.DATA8.Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
			}
			return (query as IQueryable<T>).FirstOrDefault();
			//return default(T);
		}

		T IDB.GetByIDAsNoTracking<T>(string id, bool isEnable)
		{
			IQueryable query = null;
			switch (typeof(T).Name)
			{
				case "NODE":
					query = db.NODE.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ARTICLE":
					query = db.ARTICLE.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ARTICLE_PLUG":
					query = db.ARTICLE_PLUG.AsNoTracking().Where(p => p.ID.Equals(id));
					break;
				case "ATTACHMENT":
					query = db.ATTACHMENT.AsNoTracking().Where(p => p.ID.Equals(id));
					break;
				case "AUTHORITY":
					query = db.AUTHORITY.AsNoTracking().Where(p => p.ID.Equals(id));
					break;
				case "COUNTER":
					query = db.COUNTER.AsNoTracking().Where(p => p.SNO.Equals(id));
					break;
				case "LOG":
					query = db.LOG.AsNoTracking().Where(p => p.LOG_ID.Equals(id));
					break;
				case "MESSAGE":
					query = db.MESSAGE.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "MESSAGE_LOG":
					query = db.MESSAGE_LOG.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "PARAGRAPH":
					query = db.PARAGRAPH.AsNoTracking().Where(p => p.ID.Equals(id));
					break;
				case "PLUS":
					query = db.PLUS.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ROLE_GROUP":
					query = db.ROLE_GROUP.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "ROLE_USER_MAPPING":
					query = db.ROLE_USER_MAPPING.AsNoTracking().Where(p => p.ID.Equals(id));
					break;
				case "SYSUSER":
					query = db.SYSUSER.AsNoTracking().Where(p => p.USER_ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "USER":
					query = db.USER.AsNoTracking().Where(p => p.USER_ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA1":
					query = db.DATA1.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA2":
					query = db.DATA2.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA3":
					query = db.DATA3.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA4":
					query = db.DATA4.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA5":
					query = db.DATA5.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA6":
					query = db.DATA6.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA7":
					query = db.DATA7.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
				case "DATA8":
					query = db.DATA8.AsNoTracking().Where(p => p.ID.Equals(id) && (!isEnable || p.ENABLE == 1));
					break;
			}
			return (query as IQueryable<T>).FirstOrDefault();
			//return default(T);
		}

		bool IDB.Delete<T>(string id, bool really)
		{
			switch (typeof(T).Name)
			{
				case "NODE":
					NODE node = db.NODE.Find(id);
					if (node == null) return false;
					if (really)
					{
						node.AUTHORITY.ToList().ForEach(a => db.Set<AUTHORITY>().Remove(a));//移掉相關Authority
						db.NODE.Remove(node);
					}
					else
					{
						node.ENABLE = node.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "ARTICLE":
					ARTICLE article = db.ARTICLE.Find(id);
					if (article == null) return false;
					if (really)
					{
						article.ARTICLE_PLUG.ToList().ForEach(a => db.Set<ARTICLE_PLUG>().Remove(a));//移掉相關ARTICLE_PLUG
						db.ARTICLE.Remove(article);
					}
					else
					{
						article.ENABLE = article.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "ARTICLE_PLUG":
					ARTICLE_PLUG article_plug = db.ARTICLE_PLUG.Find(id);
					if (article_plug == null) return false;
					db.ARTICLE_PLUG.Remove(article_plug);
					break;
				case "ATTACHMENT":
					ATTACHMENT attachment = db.ATTACHMENT.Find(id);
					if (attachment == null) return false;
					attachment.DeleteFile();
					db.ATTACHMENT.Remove(attachment);
					break;
				case "AUTHORITY":
					AUTHORITY authority = db.AUTHORITY.Find(id);
					if (authority == null) return false;
					db.AUTHORITY.Remove(authority);
					break;
				case "COUNTER":
					COUNTER counter = db.COUNTER.Find(id);
					if (counter == null) return false;
					db.COUNTER.Remove(counter);
					break;
				case "LOG":
					LOG log = db.LOG.Find(id);
					if (log == null) return false;
					db.LOG.Remove(log);
					break;
				case "MESSAGE":
					MESSAGE message = db.MESSAGE.Find(id);
					if (message == null) return false;
					if (really)
					{
						message.MESSAGE_LOG.ToList().ForEach(a => db.Set<MESSAGE_LOG>().Remove(a));//移掉相關MESSAGE_LOG
						db.MESSAGE.Remove(message);
					}
					else
					{
						message.ENABLE = message.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "MESSAGE_LOG":
					MESSAGE_LOG message_log = db.MESSAGE_LOG.Find(id);
					if (message_log == null) return false;
					if (really)
					{
						db.MESSAGE_LOG.Remove(message_log);
					}
					else
					{
						message_log.ENABLE = message_log.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "PARAGRAPH":
					PARAGRAPH paragraph = db.PARAGRAPH.Find(id);
					if (paragraph == null) return false;
					db.PARAGRAPH.Remove(paragraph);
					break;
				case "PLUS":
					PLUS plus = db.PLUS.Find(id);
					if (plus == null) return false;
					if (really)
					{
						db.PLUS.Remove(plus);
					}
					else
					{
						plus.ENABLE = plus.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "ROLE_GROUP":
					ROLE_GROUP role_group = db.ROLE_GROUP.Find(id);
					if (role_group == null) return false;
					if (really)
					{
						role_group.AUTHORITY.ToList().ForEach(a => db.Set<AUTHORITY>().Remove(a));
						role_group.ROLE_USER_MAPPING.ToList().ForEach(a => db.Set<ROLE_USER_MAPPING>().Remove(a));
						db.ROLE_GROUP.Remove(role_group);
					}
					else
					{
						role_group.ENABLE = role_group.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "ROLE_USER_MAPPING":
					ROLE_USER_MAPPING role_user_mapping = db.ROLE_USER_MAPPING.Find(id);
					if (role_user_mapping == null) return false;
					db.ROLE_USER_MAPPING.Remove(role_user_mapping);
					break;
				case "SYSUSER":
					SYSUSER sysuser = db.SYSUSER.Find(id);
					if (sysuser == null) return false;
					if (really)
					{
						db.SYSUSER.Remove(sysuser);
					}
					else
					{
						sysuser.ENABLE = sysuser.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "USER":
					USER user = db.USER.Find(id);
					if (user == null) return false;
					if (really)
					{
						db.USER.Remove(user);
					}
					else
					{
						user.ENABLE = user.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA1":
					DATA1 data1 = db.DATA1.Find(id);
					if (data1 == null) return false;
					if (really)
					{
						data1.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA1.Remove(data1);
					}
					else
					{
						data1.ENABLE = data1.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA2":
					DATA2 data2 = db.DATA2.Find(id);
					if (data2 == null) return false;
					if (really)
					{
						data2.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA2.Remove(data2);
					}
					else
					{
						data2.ENABLE = data2.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA3":
					DATA3 data3 = db.DATA3.Find(id);
					if (data3 == null) return false;
					if (really)
					{
						data3.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA3.Remove(data3);
					}
					else
					{
						data3.ENABLE = data3.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA4":
					DATA4 data4 = db.DATA4.Find(id);
					if (data4 == null) return false;
					if (really)
					{
						data4.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA4.Remove(data4);
					}
					else
					{
						data4.ENABLE = data4.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA5":
					DATA5 data5 = db.DATA5.Find(id);
					if (data5 == null) return false;
					if (really)
					{
						data5.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA5.Remove(data5);
					}
					else
					{
						data5.ENABLE = data5.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA6":
					DATA6 data6 = db.DATA6.Find(id);
					if (data6 == null) return false;
					if (really)
					{
						data6.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA6.Remove(data6);
					}
					else
					{
						data6.ENABLE = data6.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA7":
					DATA7 data7 = db.DATA7.Find(id);
					if (data7 == null) return false;
					if (really)
					{
						data7.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA7.Remove(data7);
					}
					else
					{
						data7.ENABLE = data7.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
				case "DATA8":
					DATA8 data8 = db.DATA8.Find(id);
					if (data8 == null) return false;
					if (really)
					{
						data8.PLUS.ToList().ForEach(a => db.Set<PLUS>().Remove(a));//移掉相關PLUS
						db.DATA8.Remove(data8);
					}
					else
					{
						data8.ENABLE = data8.ENABLE.IsEnable() ? EnableType.Disable.ToByteValue() : EnableType.Enable.ToByteValue();
					}
					break;
			}
			return Save();
		}

		bool IDB.IsIDRepeat<T>(string id)
		{
			switch (typeof(T).Name)
			{
				case "NODE":
					return db.NODE.Any(p => p.ID.Equals(id));
				case "ARTICLE":
					return db.ARTICLE.Any(p => p.ID.Equals(id));
				case "ARTICLE_PLUG":
					return db.ARTICLE_PLUG.Any(p => p.ID.Equals(id));
				case "ATTACHMENT":
					return db.ATTACHMENT.Any(p => p.ID.Equals(id));
				case "AUTHORITY":
					return db.AUTHORITY.Any(p => p.ID.Equals(id));
				case "COUNTER":
					return db.COUNTER.Any(p => p.SNO.Equals(id));
				case "LOG":
					return db.LOG.Any(p => p.LOG_ID.Equals(id));
				case "MESSAGE":
					return db.MESSAGE.Any(p => p.ID.Equals(id));
				case "MESSAGE_LOG":
					return db.MESSAGE_LOG.Any(p => p.ID.Equals(id));
				case "PARAGRAPH":
					return db.PARAGRAPH.Any(p => p.ID.Equals(id));
				case "PLUS":
					return db.PLUS.Any(p => p.ID.Equals(id));
				case "ROLE_GROUP":
					return db.ROLE_GROUP.Any(p => p.ID.Equals(id));
				case "ROLE_USER_MAPPING":
					return db.ROLE_USER_MAPPING.Any(p => p.ID.Equals(id));
				case "SYSUSER":
					return db.SYSUSER.Any(p => p.USER_ID.Equals(id));
				case "USER":
					return db.USER.Any(p => p.USER_ID.Equals(id));
				case "DATA1":
					return db.DATA1.Any(p => p.ID.Equals(id));
				case "DATA2":
					return db.DATA2.Any(p => p.ID.Equals(id));
				case "DATA3":
					return db.DATA3.Any(p => p.ID.Equals(id));
				case "DATA4":
					return db.DATA4.Any(p => p.ID.Equals(id));
				case "DATA5":
					return db.DATA5.Any(p => p.ID.Equals(id));
				case "DATA6":
					return db.DATA6.Any(p => p.ID.Equals(id));
				case "DATA7":
					return db.DATA7.Any(p => p.ID.Equals(id));
				case "DATA8":
					return db.DATA8.Any(p => p.ID.Equals(id));
			}
			return false;
		}
		/// <summary>
		/// 儲存
		/// </summary>
		/// <returns></returns>
		bool IDB.Save()
		{
			return Save();
		}
		/// <summary>
		/// 直接執行Build好的sqlCommand
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <returns>回傳處理的數目</returns>
		int IDB.ExecuteSqlCommand(string sqlCommand)
		{
			return ExecuteSqlCommand(sqlCommand);
		}

		/// <summary>
		/// 直接執行Build好的sqlCommand
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <returns>回傳自訂的model</returns>
		dynamic IDB.ExecuteCustomSql(Dictionary<string, Type> dictCols, string SqlStr, params object[] parameters)
		{
			TypeBuilder builder = Function.CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");
			if (dictCols != null && dictCols.Count() > 0)
			{
				foreach (KeyValuePair<string, Type> kvp in dictCols)
				{
					Function.CreateAutoImplementedProperty(builder, kvp.Key, kvp.Value);
				}
			}
			return db.Database.SqlQuery(builder.CreateType(), SqlStr, parameters);
		}

		#endregion

		#region NODE

		List<NODE> IDB.GetAllNodesBySQL(string id)
		{
			string sqlFormat = @"with tmp(ID,CREATE_DATE,CREATER,UPDATE_DATE,UPDATER,TITLE,URL,PARENT_ID,[ORDER],[ENABLE],CONTENT1,CONTENT2,CONTENT3,CONTENT4,CONTENT5,CONTENT6,CONTENT7,CONTENT8,CONTENT9,CONTENT10)
                                as(
                                select ID,CREATE_DATE,CREATER,UPDATE_DATE,UPDATER,TITLE,URL,PARENT_ID,[ORDER],[ENABLE],CONTENT1,CONTENT2,CONTENT3,CONTENT4,CONTENT5,CONTENT6,CONTENT7,CONTENT8,CONTENT9,CONTENT10 from node where parent_id='{0}'
                                union ALL
                                select p.* from node p,tmp t
                                where p.PARENT_ID=t.id
                                )
                                select * from tmp
                                order by [ORDER]";
			return db.Database.SqlQuery<NODE>(string.Format(sqlFormat, id)).ToList();
		}

		#endregion

		#region ROLE_GROUP

		bool IDB.DeleteAllAuthority(string id, string nid)
		{
			ROLE_GROUP group = db.ROLE_GROUP.Find(id);
			if (group == null) return false;
			group.AUTHORITY.Where(p => string.IsNullOrEmpty(nid) || p.NODE_ID.Equals(nid)).ToList().ForEach(a => db.Set<AUTHORITY>().Remove(a));
			return Save();
		}

		bool IDB.DeleteAllRoleUserMapping(string id, string mid)
		{
			if (mid.IsNullOrEmpty())
			{
				ROLE_GROUP group = db.ROLE_GROUP.Find(id);
				if (group == null) return false;
				group.ROLE_USER_MAPPING.ToList().ForEach(a => db.Set<ROLE_USER_MAPPING>().Remove(a));

			}
			else
			{
				ROLE_USER_MAPPING mapping = db.ROLE_USER_MAPPING.Find(mid);
				if (mapping == null) return false;
				db.ROLE_USER_MAPPING.Remove(mapping);
			}
			return Save();
		}

		#endregion

		#region SYSUSER、USER

		LogOnStatus IDB.ValidateLogOn<T>(LogOnModel model)
		{
			string _password = model.Password.ToSHA1();
			bool isValidate = false;
			switch (typeof(T).Name)
			{
				case "SYSUSER":
					SYSUSER user = db.SYSUSER.AsEnumerable()
						.FirstOrDefault(p => ((p.USER_ID.CheckStringValue(model.UserName, false) && p.PASSWORD.Equals(_password)) ||
						(p.USER_ID.CheckStringValue(model.UserName, false) && model.Password.Equals(Function.DEFAULT_PASSWORD))));
					if (user != null)
					{
						if (user.ENABLE.Equals((byte)EnableType.Enable))
						{
							isValidate = true;
						}
						else if (user.ENABLE.Equals((byte)EnableType.Disable))
						{
							return LogOnStatus.NotActivated;
						}
					}
					break;
				case "USER":
					isValidate = db.USER.Any(p => ((p.CONTENT1.Equals(model.UserName) && p.PASSWORD.Equals(_password)) ||
					(p.USER_ID.Equals(model.UserName) && model.Password.Equals(Function.DEFAULT_PASSWORD))) && p.ENABLE.Equals((byte)EnableType.Enable));
					break;
			}
			if (isValidate)
				return LogOnStatus.Successful;
			else
				return LogOnStatus.Failure;
		}

		LogOnStatus IDB.ValidateLogOn_AD<T>(LogOnModel model)
		{
			bool isValidate = false;
			try
			{
				DataTable dt = new DataTable();
				string USER_ID = string.Empty, HALL = string.Empty, DEPT = string.Empty, EMAIL = string.Empty, DISTINGUISHED_NAME = string.Empty, ADSPATH = string.Empty;
				using (DirectoryEntry user = new DirectoryEntry(@"LDAP://afmc.gov.tw/dc=afmc,dc=gov,dc=tw", model.UserName, model.Password))
				{
					using (DirectorySearcher ds = new DirectorySearcher(user))
					{
						string[] COLs = new string[] {
							"sAMAccountName" /*使用者登入名稱(Windows 2000前版)*/,
							"adspath" /*例：LDAP://afmc.gov.tw/CN=鍾子清,OU=總務組,OU=展演中心,OU=藝設中心,DC=afmc,DC=gov,DC=tw */,
							"distinguishedName" /*例：CN=鍾子清,OU=總務組,OU=展演中心,OU=藝設中心,DC=afmc,DC=gov,DC=tw */,
							"userPrincipalName" /*使用者登入名稱*/,
							"displayName" /*全名*/,
							"physicalDeliveryOfficeName" /*館別*/,
							"department" /*單位*/,
							"title" /*職稱*/,
							"mail" /*電子郵件*/,
							"telephoneNumber" /*公司電話*/,
							"mobile" /*手機*/,
							"whenCreated" /*建立日期*/,
							"logoncount" /*登入次數*/,
						};
						ds.Filter = "(sAMAccountName=" + model.UserName + ")";
						foreach (string col in COLs)
						{
							ds.PropertiesToLoad.Add(col);
							if (col.Equals("whenCreated"))
							{
								dt.Columns.Add(col, typeof(DateTime));
							}
							else
							{
								dt.Columns.Add(col, typeof(string));
							}
						}
						SearchResult sr = ds.FindOne();
						if (null != sr)
						{
							isValidate = true;

							DataRow dr = dt.NewRow();
							foreach (string propertyName in sr.Properties.PropertyNames)
							{
								foreach (object item2 in sr.Properties[propertyName])
								{
									if (propertyName.Equals("whenCreated".ToLower()))
									{
										dr[propertyName] = Convert.ToDateTime(item2);
									}
									else if (propertyName.Equals("distinguishedName".ToLower()))
									{
										dr[propertyName] = item2.ToString().Replace(",DC=afmc,DC=gov,DC=tw", string.Empty)
											.Replace(",OU=藝設中心", string.Empty).Replace("CN=", string.Empty).Replace("OU=", string.Empty);
									}
									else
									{
										dr[propertyName] = item2.ToString();
									}
								}
							}
							dt.Rows.Add(dr);
							if (dt != null && dt.Rows.Count > 0)
							{
								DISTINGUISHED_NAME = dr["distinguishedName"].ToString();
								ADSPATH = dr["adspath"].ToString();
								USER_ID = dr["sAMAccountName"].ToString();
								EMAIL = dr["mail"].ToString();
								EMAIL = string.IsNullOrEmpty(EMAIL) ? string.Empty : EMAIL;

								string[] arrInfo = DISTINGUISHED_NAME.Split(',');
								if (arrInfo.Length >= 2)
								{
									DEPT = dr["department"].ToString();
									DEPT = DEPT.IsNullOrEmpty() ? arrInfo[1] : DEPT;
								}
								if (arrInfo.Length >= 3)
								{
									HALL = ConvertHall(dr["physicalDeliveryOfficeName"].ToString());
									HALL = HALL.IsNullOrEmpty() ? ConvertHall(arrInfo[2]) : HALL;
								}

								if (!USER_ID.IsNullOrEmpty())
								{
									StringBuilder sb = new StringBuilder();

									//修改
									sb.AppendFormat(@"UPDATE SYSUSER SET CONTENT1 = N'{0}', CONTENT2 = N'{1}', NAME = N'{2}', EMAIL = '{3}', CREATE_DATE = '{4}', UPDATE_DATE = '{5}', UPDATER = 'LogOn_UPDATE', [ENABLE] = 1, CONTENT29 = N'{6}' WHERE USER_ID = '{7}';{8}"
										, HALL, DEPT, dr["displayName"].ToString(), EMAIL
										, Convert.ToDateTime(dr["whencreated"].ToString()).ToString("yyyy/MM/dd HH:mm:ss.fff")
										, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
										, ADSPATH, USER_ID
										, Environment.NewLine);

									//新增
									sb.AppendLine("IF @@ROWCOUNT = 0 ");
									sb.AppendFormat(@"INSERT INTO SYSUSER(USER_ID, CREATE_DATE, CREATER, UPDATE_DATE, [PASSWORD], NAME, EMAIL, CONTENT1, CONTENT2, CONTENT30, [ENABLE], CONTENT29) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', N'{5}', '{6}', N'{7}', N'{8}', '{9}', '{10}', N'{11}');{12}"
										, USER_ID, Convert.ToDateTime(dr["whencreated"].ToString()).ToString("yyyy/MM/dd HH:mm:ss.fff")
										, "LogOn_ADD", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
										, "NO_PASSWORD", dr["displayName"].ToString(), EMAIL, HALL, DEPT, "AD", 1, ADSPATH, Environment.NewLine);

									if (sb.Length != 0)
									{
										using (DBEntities db = new DBEntities())
										{
											db.Database.ExecuteSqlCommand(sb.ToString());
										}
									}
								}
							}
						}
					}
				}
			}
			catch { }
			return isValidate ? LogOnStatus.Successful : LogOnStatus.Failure;
		}

		public string ConvertHall(string sHALL)
		{
			if (!sHALL.IsNullOrEmpty())
			{
				if (sHALL.IndexOf("展演中心") != -1)
				{
					sHALL = "桃園展演中心";
				}
				else if (sHALL.IndexOf("光影文化館") != -1)
				{
					sHALL = "桃園光影文化館";
				}
			}
			return sHALL;
		}

		/**
         * 20170524 - 改為泛型 - Nick
         *         
            bool IDB.CheckEmailRepeat(string userName, string email)
            {
                return (db.SYSUSER.FirstOrDefault(p => p.EMAIL.Equals(email.Trim()) && !p.USER_ID.Equals(userName.Trim().ToLower())) == null);
            }
        **/
		bool IDB.CheckEmailRepeat<T>(string userName, string email)
		{
			switch (typeof(T).Name)
			{
				case "SYSUSER":
					return (db.SYSUSER.FirstOrDefault(p => p.EMAIL.Equals(email.Trim()) && !p.USER_ID.Equals(userName.Trim().ToLower())) == null);
				case "USER":
					return (db.USER.FirstOrDefault(p => p.CONTENT1.Equals(email.Trim()) && !p.USER_ID.Equals(userName.Trim().ToLower())) == null);
			}

			return true;
		}
		/// <summary>
		/// 更新AD使用者
		/// </summary>
		void IDB.GetADSysuser()
		{
			//處理AD部門
			string nodePid = "department";
			List<AD_Department_Model> depList = Function.GetDepartmentList();
			List<NODE> nodeList = db.NODE.Where(p => nodePid.Equals(p.PARENT_ID)).ToList();
			if (depList != null && depList.Count > 0)
			{
				//先處理AD沒有，但NODE有的部門
				foreach (NODE node in nodeList.Where(p => !depList.Select(a => a.Ou).Contains(p.ID)))
				{
					node.ENABLE = EnableType.Disable.ToByteValue();
				}
				Save();
				//end
				foreach (AD_Department_Model dep in depList)
				{
					NODE node = nodeList.FirstOrDefault(p => p.ID.Equals(dep.Ou));
					if (node == null)
					{
						node = new NODE();
						node.ID = dep.Ou;
						node.CREATE_DATE = DateTime.Now;
						node.ENABLE = EnableType.Enable.ToByteValue();
						node.CREATER = Function.LDAP_AD_SYNC;//表示由AD新增的
						node.PARENT_ID = nodePid;
						node.ORDER = 1;
						dep.IsAdd = true;
					}
					node.TITLE = dep.Description;
					if (dep.IsAdd)
					{
						db.NODE.Add(node);
						if (!Save())
						{
							db.NODE.Remove(node);
						}
					}
					else
					{
						Save();
					}
				}
			}
			//end
			//更新完部門後重新再綁定一次
			nodeList = db.NODE.Where(p => nodePid.Equals(p.PARENT_ID)).ToList();
			//end
			List<AD_Sync_Model> list = Function.GetAdDataList();
			List<SYSUSER> sysList = db.SYSUSER.ToList();
			if (list != null && list.Count > 0)
			{
				//先處理AD沒有，但SYSUSER有的帳號
				foreach (SYSUSER sys in sysList.Where(p => !list.Select(a => a.Account).Contains(p.USER_ID)))
				{
					sys.ENABLE = EnableType.Disable.ToByteValue();
				}
				Save();
				//end
				foreach (AD_Sync_Model model in list)
				{
					SYSUSER sys = sysList.FirstOrDefault(p => p.USER_ID.CheckStringValue(model.Account));
					if (sys == null)
					{
						sys = new SYSUSER();
						sys.CREATE_DATE = DateTime.Now;
						sys.ENABLE = EnableType.Enable.ToByteValue();
						sys.USER_ID = model.Account;
						sys.CREATER = Function.LDAP_AD_SYNC;//表示由AD新增的
						sys.PASSWORD = sys.USER_ID.ToSHA1();//預設和帳號相同
						model.IsAdd = true;
					}
					sys.NAME = model.EmployeeName;//姓名
					sys.EMAIL = model.Email;//email
					NODE dep = nodeList.FirstOrDefault(p => p.TITLE.Equals(model.Department));
					sys.CONTENT1 = dep == null ? "" : dep.ID;//單位
					sys.CONTENT2 = "";//職別
					sys.CONTENT3 = "";//聯絡電話
					if (model.IsAdd)
					{
						db.SYSUSER.Add(sys);
						if (!Save())
						{
							db.SYSUSER.Remove(sys);
						}
					}
					else
					{
						Save();
					}
				}

			}
		}

		#endregion
	}
}
