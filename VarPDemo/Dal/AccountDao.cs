using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VarPDemo.Models;

namespace VarPDemo.Dal
{
     //IDisposable是用于释放所有资源
    class AccountDao : IDisposable
    {
        private SQLiteConnection conn;
        private object lockObj = new object();

        public AccountDao(SQLiteConnection conn)
        {
            this.conn = conn;
        }

        public void Dispose()
        {
            if (conn != null)
            {
                conn.Close();
                conn.ReleaseMemory();
                conn.Dispose();
            }
        }

        public void EmptyTable()
        {
            conn.Open();

            SQLiteCommand command = new SQLiteCommand("delete from AccontInfo", conn);
            command.ExecuteNonQuery();

            conn.Close();
        }

        public int GetRecordCount(AccountModel searchModel = null)
        {
            //获取数量 默认获取所有数据数量
            string whereSql = GenerateWhereSql(searchModel);
            conn.Open();
            SQLiteCommand command = new SQLiteCommand("select count(*) from AccontInfo " + whereSql, conn);
            int count = (int)(long)command.ExecuteScalar();
            conn.Close();
            return count;
        }



        private string GenerateWhereSql(AccountModel infoModel)
        {
            if (infoModel == null)
                return "";

            ICollection<string> whereList = new List<string>();
            if (infoModel.UName != null)
            {
                whereList.Add(string.Format("UName like '%{0}%'", infoModel.UName));
            }
            if (infoModel.UPass != null)
            {
                whereList.Add(string.Format("UPass like '%{0}%'", infoModel.UPass));
            }
            return whereList.Count > 0 ? " where 1=1 and " + string.Join(" and ", whereList.ToArray()) : "";
        }


        public void InsertData(ICollection<AccountModel> AcountModel)
        {
            //lock 关键字将语句块标记为临界区,执行语句，然后释放该锁
            //lock 确保当一个线程位于代码的临界区时，另一个线程不进入临界区。
            //如果其他线程试图进入锁定的代码,则它将一直等待（即被阻止）直到该对象被释放。
            lock (lockObj)
            {
                //UID是自动增长
                conn.Open();
                SQLiteTransaction tran = conn.BeginTransaction();
                SQLiteCommand command = new SQLiteCommand("INSERT INTO AccontInfo(UserName,UName,UPass,ULevel,UState) values(@UserName,@UName,@UPass,@ULevel,@UState)", conn, tran);
                foreach (AccountModel data in AcountModel)
                {
                    command.Parameters.AddWithValue("@UserName", data.UserName);
                    command.Parameters.AddWithValue("@UName", data.UName);
                    command.Parameters.AddWithValue("@UPass", data.UPass);
                    command.Parameters.AddWithValue("@ULevel", data.ULevel);
                    command.Parameters.AddWithValue("@UState", data.UState);
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                conn.Close();
            }
        }


        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ICollection<AccountModel> SelectData(int pageStart, int pageSize, AccountModel AcountModel = null)
        {
            lock (lockObj)
            {
                conn.Open();
                string whereSql = GenerateWhereSql(AcountModel);
                //limit从0开始到多少的数据
                string sql_cmd = string.Format("SELECT UId, UserName, UName,UPass,ULevel,UState FROM AccontInfo {0} limit {1},{2}", whereSql, pageStart, pageSize);
                SQLiteCommand command = new SQLiteCommand(sql_cmd, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                ICollection<AccountModel> datas = new List<AccountModel>();
                while (reader.Read())
                {
                    AccountModel data = new AccountModel()
                    {
                        UId = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0,
                        UserName = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                        UName = !reader.IsDBNull(1) ? reader.GetString(2) : null,
                        UPass = !reader.IsDBNull(1) ? reader.GetString(3) : null,
                        ULevel = !reader.IsDBNull(0) ? reader.GetInt32(4) : 0,
                        UState = !reader.IsDBNull(0) ? reader.GetInt32(5) : 0,
                    };
                    datas.Add(data);
                }
                conn.Close();
                return datas;
            }
        }



    }//end  class AccountDao
}
