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

        // 选择。
        public static DataSet Select(SqlConnection conn, string strSQL, Dictionary<string, object> paramDict)
        {
            SqlCommand comm = new SqlCommand(strSQL, conn);
            foreach (var p in paramDict) comm.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value); // p.Value == null时替换为DBNull.Value。
            SqlDataAdapter da = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        // 执行参数化查询。可用于插入、更新或删除。
        public static void Execute(SqlConnection conn, string strSQL, Dictionary<string, object> paramDict)
        {
            SqlCommand comm = new SqlCommand(strSQL, conn);
            foreach (var p in paramDict) comm.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            comm.ExecuteNonQuery();
        }

        // 插入。
        public static void Insert(SqlConnection conn, string table, params string[] values)
        {
            var paramDict = new Dictionary<string, object>();
            string[] parameters = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                string newParam = "@p" + i;
                parameters[i] = newParam;
                paramDict.Add(newParam, values[i]);
            }
            string strSQL = "INSERT INTO " + table + " VALUES (" + string.Join(",", parameters) + ");";
            Execute(conn, strSQL, paramDict);
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