using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data.SqlClient;

namespace SongListManagementTool
{
    public partial class Note : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string dirPath = "./Notes/" + Session["uid"]; // 保存特定用户笔记的用户目录路径
                dirPath = Server.MapPath(dirPath);
                string docPath = dirPath + "/" + Request.Params["sid"] + ".txt"; // 特定笔记的文件路径

                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                if (File.Exists(docPath)) this.TextBox1.Text = File.ReadAllText(docPath, Encoding.UTF8);
            }
        }

        // 保存笔记。
        protected void Button1_Click(object sender, EventArgs e)
        {
            string dirPath = "./Notes/" + Session["uid"];
            dirPath = Server.MapPath(dirPath);
            string docPath = dirPath + "/" + Request.Params["sid"] + ".txt";

            if (!File.Exists(docPath)) File.Create(docPath).Close();

            SqlConnection conn = DataOper.Connect();
            if (this.TextBox1.Text == "")
            {
                File.Delete(docPath);
                DataOper.Update(conn, "songListSongs", "WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"], "status=0"); // 更改整理情况至未整理。
            }
            else
            {
                File.WriteAllText(docPath, this.TextBox1.Text, Encoding.UTF8);
                DataOper.Update(conn, "songListSongs", "WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"], "status=1"); // 更改整理情况至已整理。
            }

            conn.Close();
            Response.Write("<script>alert('笔记已保存。');</script>");
        }

        // 返回歌曲信息页面。
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("SongInfo.aspx?sid=" + Request.Params["sid"]);
        }
    }
}