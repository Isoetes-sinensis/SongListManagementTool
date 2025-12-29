using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SongListManagementTool
{
    public class DataOper
    {
        // 连接至数据库。
        public static SqlConnection Connect()
        {
                string strConn;
                strConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString().Trim();
                SqlConnection conn = new SqlConnection(strConn);
                if (conn.State != ConnectionState.Open) conn.Open();
                return conn;
        }

        // 处理SQL语句中的特殊字符。
        public static string Escape(string rawSQL)
        {
            string ret = rawSQL.Replace("'", "''");
            return ret;
        }

        // 选择。
        public static DataSet Select(SqlConnection conn, string table, string selected="*", string conditions="")
        {
            string strSQL = "SELECT " + selected + " FROM " + table + " " + conditions;
            SqlCommand comm = new SqlCommand(strSQL, conn);
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        // 插入。
        public static void Insert(SqlConnection conn, string table, params string[] values)
        {
            string strSQL = "INSERT INTO " + table + " VALUES (" + string.Join(",", values) + ");";
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.ExecuteNonQuery();
        }

        // 更新。
        public static void Update(SqlConnection conn, string table, string conditions="", params string[] updated)
        {
            string strSQL = "UPDATE " + table + " SET " + string.Join(",", updated) + " " + conditions;
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.ExecuteNonQuery();
        }

        // 删除。
        public static void Delete(SqlConnection conn, string table, string conditions)
        {
            string strSQL = "DELETE FROM " + table + " " + conditions;
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.ExecuteNonQuery();
        }

        // 初次使用系统时需要初始化，包括创建数据库等操作。
        // 若确认数据库已存在，直接注释掉其中所有语句即可。
        public static void Initialise(SqlConnection conn)
        {
            string strSQL = "IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'songListUsers') CREATE TABLE songListUsers (uid INT IDENTITY(1,1) PRIMARY KEY, username NVARCHAR(50), password NVARCHAR(50));";
            strSQL += "IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'songListSongs') CREATE TABLE songListSongs (sid INT IDENTITY(1,1) PRIMARY KEY, uid INT, title NVARCHAR(220), artist NVARCHAR(220), lyricist NVARCHAR(220), composer NVARCHAR(220), arranger NVARCHAR(220), tieup INT, album NVARCHAR(220), status INT);";
            strSQL += "IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'songListResources') CREATE TABLE songListResources (sid INT, uid INT, resource NVARCHAR(255), type NVARCHAR(50));";
            strSQL += "IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'songListTieUp') CREATE TABLE songListTieUp (tid INT IDENTITY(1,1) PRIMARY KEY, uid INT, name NVARCHAR(220), type NVARCHAR(50));";
            SqlCommand comm = new SqlCommand(strSQL, conn);
            comm.ExecuteNonQuery();
        }
    }
}