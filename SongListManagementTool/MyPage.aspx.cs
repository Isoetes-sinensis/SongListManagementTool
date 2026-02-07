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
        public static void BindTieUpDropDownList(DropDownList d, string userid, bool hasDefaultItem = true)
        {
            SqlConnection conn = DataOper.Connect();
            string strSQL = "SELECT * FROM songListTieUp WHERE uid=@uid";
            DataSet ds = DataOper.Select(conn, strSQL, new Dictionary<string, object> { ["@uid"] = userid });
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

        // 返回更新数据库用的字符串。
        protected string GetUpdateString(int rowID, int cellID)
        {
            string ret = ((TextBox)this.GridView1.Rows[rowID].Cells[cellID].Controls[0]).Text.ToString().Trim();
            if (ret == "") return null;
            else return ret;
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string sid = this.GridView1.Rows[e.RowIndex].Cells[10].Text;
            string title = this.GetUpdateString(e.RowIndex, 1);
            string artist = this.GetUpdateString(e.RowIndex, 2);
            string lyricist = this.GetUpdateString(e.RowIndex, 3);
            string composer = this.GetUpdateString(e.RowIndex, 4);
            string arranger = this.GetUpdateString(e.RowIndex, 5);
            string album = this.GetUpdateString(e.RowIndex, 6);

            if (title == null) Response.Write("<script>alert('歌名不可为空。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();
                string strSQL = @"
                    UPDATE songListSongs
                    SET title=@title, artist=@artist, lyricist=@lyricist, composer=@composer, arranger=@arranger, album=@album
                    WHERE uid=@uid AND sid=@sid
                ";
                var paramDict = new Dictionary<string, object> { ["@uid"] = Session["uid"], ["@sid"] = sid, ["@title"] = title, ["@artist"] = artist, ["@lyricist"] = lyricist, ["@composer"] = composer, ["@arranger"] = arranger, ["@album"] = album };

                DataOper.Execute(conn, strSQL, paramDict);
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
            string strSQL = "DELETE FROM songListSongs WHERE uid=@uid AND sid=@sid";
            string strSQL2 = "DELETE FROM songListResources WHERE uid=@uid AND sid=@sid";
            var paramDict = new Dictionary<string, object> { ["@uid"] = Session["uid"], ["@sid"] = sid };
            DataOper.Execute(conn, strSQL, paramDict); // 删除歌曲信息。
            DataOper.Execute(conn, strSQL2, paramDict); // 删除歌曲链接。
            conn.Close();

            string dirPath = "./Notes/" + Session["uid"];
            dirPath = Server.MapPath(dirPath);
            string docPath = dirPath + "/" + sid + ".txt";
            if (File.Exists(docPath)) File.Delete(docPath); // 删除歌曲笔记。

            this.Bind(true);
            Response.Write("<script>alert('删除成功。');</script>");
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.GridView1.EditIndex = -1;
            this.Bind();
        }

        // 根据TextBox的值向查询歌曲信息的SQL语句中添加条件。若值为空，返回false。
        protected bool AddConditions(TextBox textbox, string field, ref string strSQL, Dictionary<string, object> paramDict)
        {
            string text = textbox.Text.Trim();
            textbox.Text = "";
            if (text != "")
            {
                string paramName = "@" + field;
                strSQL += " AND ss." + field + "=" + paramName;
                paramDict[paramName] = text;
                return true;
            }
            else return false;
        }

        // 根据DropDownList的值向查询歌曲信息的SQL语句中添加条件。若值为空，返回false。
        protected bool AddConditions(DropDownList dropDownList, string field, ref string strSQL, Dictionary<string, object> paramDict)
        {
            string text = dropDownList.SelectedItem.Value;
            if (text != "NULL")
            {
                string paramName = "@" + field;
                strSQL += " AND ss." + field + "=" + paramName;
                paramDict[paramName] = text;
                return true;
            }
            else return false;
        }

        // 查询符合条件的歌曲数据。
        protected void Button1_Click(object sender, EventArgs e)
        {
            string strSQL = @"
                SELECT ss.*, st.name strTieUp
                FROM SongListSongs ss, SongListTieUp st
                WHERE ss.tieup=st.tid
                AND ss.uid=@uid
            ";
            var paramDict = new Dictionary<string, object> { ["@uid"] = Session["uid"] };

            bool title = AddConditions(this.TitleTextBox, "title", ref strSQL, paramDict);
            bool artist = AddConditions(this.ArtistTextBox, "artist", ref strSQL, paramDict);
            bool lyricist = AddConditions(this.LyricistTextBox, "lyricist", ref strSQL, paramDict);
            bool composer = AddConditions(this.ComposerTextBox, "composer", ref strSQL, paramDict);
            bool arranger = AddConditions(this.ArrangerTextBox, "arranger", ref strSQL, paramDict);
            bool album = AddConditions(this.AlbumTextBox, "album", ref strSQL, paramDict);
            bool tieup = AddConditions(this.TieUpDropDownList, "tieup", ref strSQL, paramDict);
            bool status = AddConditions(this.StatusDropDownList, "status", ref strSQL, paramDict);

            if (paramDict.ContainsKey("@composer")) this.AlbumTextBox.Text = paramDict["@composer"].ToString();
            if (!(title || artist || lyricist || composer || arranger || album || tieup || status)) Response.Write("<script>alert('查询条件不可为空。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataSet ds = DataOper.Select(conn, strSQL, paramDict);
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
            if (ret == "") return null;
            else return ret;
        }

        protected string GetInsertionString(DropDownList dropDownList)
        {
            string ret = dropDownList.SelectedItem.Value;
            if (ret == "NULL") return null;
            else return ret;
        }

        // 插入输入的歌曲数据。
        protected void Button2_Click(object sender, EventArgs e)
        {
            string title = GetInsertionString(this.TitleTextBox);
            string artist = GetInsertionString(this.ArtistTextBox);
            string lyricist = GetInsertionString(this.LyricistTextBox);
            string composer = GetInsertionString(this.ComposerTextBox);
            string arranger = GetInsertionString(this.ArrangerTextBox);
            string album = GetInsertionString(this.AlbumTextBox);
            string tieup = GetInsertionString(this.TieUpDropDownList);
            string status = GetInsertionString(this.StatusDropDownList);

            if (title == null) Response.Write("<script>alert('歌名不可为空。');</script>");
            else if (tieup == null) Response.Write("<script>alert('tie-up不可为空。');</script>");
            else if (status == null) Response.Write("<script>alert('整理情况不可为空。');</script>");
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