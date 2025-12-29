using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace SongListManagementTool
{
    public partial class MyPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null) Response.Redirect("Login.aspx");
            else if (!IsPostBack)
            {
                Bind(true);
                MyPage.BindTieUpDropDownList(this.TieUpDropDownList, Session["uid"].ToString());
            }
        }

        // 根据status列的值生成包含对应字符串的列。
        protected DataSet GetStrStatus(DataSet ds)
        {
            ds.Tables[0].Columns.Add("strStatus", typeof(string));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string strStatus = "";
                switch (ds.Tables[0].Rows[i]["status"].ToString())
                {
                    case "0":
                        strStatus = "待整理";
                        break;
                    case "1":
                        strStatus = "已整理";
                        break;
                    case "2":
                        strStatus = "可整理";
                        break;
                    case "3":
                        strStatus = "不整理";
                        break;
                }
                ds.Tables[0].Rows[i]["strStatus"] = strStatus;
            }

            return ds;
        }

        // 绑定数据源至GridView。
        protected void Bind(bool reset=false)
        {
            if (Session["ds"] == null || reset)
            {
                SqlConnection conn = DataOper.Connect();
                string strSQL = @"
                    SELECT ss.*, st.name strTieUp FROM SongListSongs ss, SongListTieUp st
                    WHERE ss.uid=" + Session["uid"] + @" AND ss.tieup=st.tid
                    ORDER BY ss.sid;
                ";
                SqlCommand comm = new SqlCommand(strSQL, conn);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataSet ds = new DataSet();
                da.Fill(ds);
                ds = this.GetStrStatus(ds);
                Session["ds"] = ds;
                conn.Close();
            }

            this.GridView1.DataSource = Session["ds"];
            this.GridView1.DataBind();
        }

        // 绑定数据至tie-up的DropDownList。
        public static void BindTieUpDropDownList(DropDownList d, string userid, bool hasDefaultItem=true)
        {
            SqlConnection conn = DataOper.Connect();
            DataSet ds = DataOper.Select(conn, "songListTieUp", conditions: ("WHERE uid=" + userid));
            conn.Close();

            d.DataSource = ds;
            d.DataTextField = "name";
            d.DataValueField = "tid";
            d.DataBind();
            
            if (hasDefaultItem)
            {
                ListItem defaultItem = new ListItem();
                defaultItem.Text = "(请选择)";
                defaultItem.Value = "NULL";
                d.Items.Insert(0, defaultItem);
            }
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex > -1) e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString(); // 创建行号，即按顺序递增的歌曲编号。注意可能不同于sid。
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[9].Visible = false; // 隐藏tieup列。
                e.Row.Cells[10].Visible = false; // 隐藏sid列。
                e.Row.Cells[11].Visible = false; // 隐藏status列。
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            this.GridView1.EditIndex = e.NewEditIndex;
            this.Bind(); // 不可重置数据源，否则无法在查询后的结果中准确定位行。

            // 编辑时限制输入TextBox文本的长度。
            TextBox tb;
            for (int i = 1; i < 7; i++)
            {
                tb = (TextBox)this.GridView1.Rows[e.NewEditIndex].Cells[i].Controls[0];
                tb.MaxLength = this.TitleTextBox.MaxLength;
            }
        }

        // 返回更新数据库用的字符串（SET “column=...”）。
        protected string GetUpdateString(int rowID, int cellID, string column)
        {
            string ret = ((TextBox) this.GridView1.Rows[rowID].Cells[cellID].Controls[0]).Text.ToString().Trim();
            if (ret == "") return column + "=NULL";
            else return column + "=N'" + DataOper.Escape(ret) + "'";
        }
        
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sid = this.GridView1.Rows[e.RowIndex].Cells[10].Text;
            string title = this.GetUpdateString(e.RowIndex, 1, "title");
            string artist = this.GetUpdateString(e.RowIndex, 2, "artist");
            string lyricist = this.GetUpdateString(e.RowIndex, 3, "lyricist");
            string composer = this.GetUpdateString(e.RowIndex, 4, "composer");
            string arranger = this.GetUpdateString(e.RowIndex, 5, "arranger");
            string album = this.GetUpdateString(e.RowIndex, 6, "album");

            if (title.EndsWith("NULL")) Response.Write("<script>alert('歌名不可为空。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataOper.Update(conn, "songListSongs", "WHERE uid=" + Session["uid"] + " AND sid=" + sid, title, artist, lyricist, composer, arranger, album);
                conn.Close();

                this.GridView1.EditIndex = -1;
                this.Bind(true);
                Response.Write("<script>alert('更新成功。');</script>");
            }
        }

        // 删除时同步删除歌曲数据、歌曲链接数据和歌曲笔记文件。
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string sid = this.GridView1.Rows[e.RowIndex].Cells[10].Text;
            SqlConnection conn = DataOper.Connect();
            DataOper.Delete(conn, "songListSongs", "WHERE uid=" + Session["uid"] + " AND sid=" + sid);
            DataOper.Delete(conn, "songListResources", "WHERE uid=" + Session["uid"] + " AND sid=" + sid);
            conn.Close();

            string dirPath = "./Notes/" + Session["uid"];
            dirPath = Server.MapPath(dirPath);
            string docPath = dirPath + "/" + sid + ".txt";
            if (File.Exists(docPath)) File.Delete(docPath);

            this.Bind(true);
            Response.Write("<script>alert('删除成功。');</script>");
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.GridView1.EditIndex = -1;
            this.Bind();
        }

        // 获取查询数据用的字符串（WHERE ... “AND column=...”）。
        protected string GetSelectionString(TextBox textbox, string column)
        {
            string ret = textbox.Text.Trim();
            textbox.Text = "";
            if (ret == "") return "";
            else return " AND " + column + "=N'" + DataOper.Escape(ret) + "'";
        }

        // 查询符合条件的歌曲数据。
        protected void Button1_Click(object sender, EventArgs e)
        {
            string title = GetSelectionString(this.TitleTextBox, "ss.title");
            string artist = GetSelectionString(this.ArtistTextBox, "ss.artist");
            string lyricist = GetSelectionString(this.LyricistTextBox, "ss.lyricist");
            string composer = GetSelectionString(this.ComposerTextBox, "ss.composer");
            string arranger = GetSelectionString(this.ArrangerTextBox, "ss.arranger");
            string tieup = this.TieUpDropDownList.SelectedItem.Value;
            string album = GetSelectionString(this.AlbumTextBox, "ss.album");
            string status = this.StatusDropDownList.SelectedItem.Value;

            if (tieup == "NULL") tieup = "";
            else tieup = "AND ss.tieup=" + tieup;
            if (status == "NULL") status = "";
            else status = "AND ss.status=" + status;
            if (title == "" && artist == "" && lyricist == "" && composer == "" && arranger == "" && tieup == "" && album == "" && status == "") Response.Write("<script>alert('查询条件不可为空。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataSet ds = DataOper.Select(conn, "SongListSongs ss, SongListTieUp st", "ss.*, st.name strTieUp", conditions: ("WHERE ss.uid=" + Session["uid"] + title + artist + lyricist + composer + arranger + tieup + album + status + " AND ss.tieup=st.tid"));
                conn.Close();

                if (ds.Tables[0].Rows.Count == 0) Response.Write("<script>alert('没有符合条件的查询结果。');</script>");
                else
                {
                    ds = this.GetStrStatus(ds);
                    Session["ds"] = ds;
                    this.Bind();
                }
            }
        }

        // 获取插入数据用的字符串（VALUES (“...”)）。
        protected string GetInsertionString(TextBox textbox)
        {
            string ret = textbox.Text.Trim();
            // textbox.Text = "";
            if (ret == "") return "NULL";
            else return "N'" + DataOper.Escape(ret) + "'";
        }

        // 插入输入的歌曲数据。
        protected void Button2_Click(object sender, EventArgs e)
        {
            string title = GetInsertionString(this.TitleTextBox);
            string artist = GetInsertionString(this.ArtistTextBox);
            string lyricist = GetInsertionString(this.LyricistTextBox);
            string composer = GetInsertionString(this.ComposerTextBox);
            string arranger = GetInsertionString(this.ArrangerTextBox);
            string tieup = this.TieUpDropDownList.SelectedItem.Value;
            string album = GetInsertionString(this.AlbumTextBox);
            string status = this.StatusDropDownList.SelectedItem.Value;

            if (title == "NULL") Response.Write("<script>alert('歌名不可为空。');</script>");
            else if (tieup == "NULL") Response.Write("<script>alert('tie-up不可为空。');</script>");
            else if (status == "NULL") Response.Write("<script>alert('整理情况不可为空。');</script>");
            else if (status == "1") Response.Write("<script>alert('不可直接添加已整理的歌曲。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataOper.Insert(conn, "songListSongs", (string)Session["uid"], title, artist, lyricist, composer, arranger, tieup, album, status);

                this.Bind(true);
                Response.Write("<script>alert('插入成功。');</script>");
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView1.PageIndex = e.NewPageIndex;
            this.Bind();
        }
    }
}