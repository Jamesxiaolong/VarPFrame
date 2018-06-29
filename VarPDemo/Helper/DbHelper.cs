using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarPDemo.Helper
{
    class DbHelper
    {
        public static string DbName = @"ProjMgr.sqlite";
        public static string DbPath = Directory.GetCurrentDirectory() + @"\" + DbName;
        public static void InitDataBase()
        {
            //判断数据库文件是否存在
            if (!File.Exists(DbPath))
            {
                CreateDatabase();
                CreateTable();
            }
        }

        public static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DbName);
        }

        public static void DeleteDatabase()
        {
            File.Delete(DbPath);
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0};Version=3;", DbName));
        }

        public static bool IsTableExist(SQLiteConnection conn, string tableName)
        {
            string sql = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            int count = (int)command.ExecuteScalar();
            conn.Close();
            return count > 0 ? true : false;
        }

        public static void CreateTable()
        {
            SQLiteConnection conn = GetConnection();
            conn.Open();

            SQLiteCommand command = new SQLiteCommand(@"create table AccontInfo(UId INTEGER PRIMARY KEY NOT NULL, 
                UserName nvarchar(64), UName nvarchar(64), UPass nvarchar(32), ULevel int, UState int)", conn);

            command.ExecuteNonQuery();

            conn.Close();
        }

    }
}
