using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SongListManagementTool
{
    public partial class SongInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null) Response.Redirect("Login.aspx");
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataSet ds = DataOper.Select(conn, "songListSongs", conditions: ("WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"]));
                DataSet ds2 = DataOper.Select(conn, "songListResources", conditions: ("WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"]));
                conn.Close();

                try
                {
                    this.TitleTextBox.Text = ds.Tables[0].Rows[0]["title"].ToString().Trim();
                    this.ArtistTextBox.Text = ds.Tables[0].Rows[0]["artist"].ToString().Trim();
                    this.LyricistTextBox.Text = ds.Tables[0].Rows[0]["lyricist"].ToString().Trim();
                    this.ComposerTextBox.Text = ds.Tables[0].Rows[0]["composer"].ToString().Trim();
                    this.ArrangerTextBox.Text = ds.Tables[0].Rows[0]["arranger"].ToString().Trim();

                    if (!IsPostBack)
                    {
                        MyPage.BindTieUpDropDownList(this.DropDownList1, Session["uid"].ToString(), false);
                        this.DropDownList1.SelectedValue = ds.Tables[0].Rows[0]["tieup"].ToString().Trim();
                    }

                    this.AlbumTextBox.Text = ds.Tables[0].Rows[0]["album"].ToString().Trim();

                    if (!IsPostBack) this.DropDownList2.SelectedValue = ds.Tables[0].Rows[0]["status"].ToString().Trim();
                    this.HiddenField1.Value = ds.Tables[0].Rows[0]["status"].ToString().Trim(); // 用于缓存原先的歌曲整理情况，以便更新时作比较。

                    this.DataList1.DataSource = ds2;
                    this.DataList1.DataBind();
                }
                catch
                {
                    Response.Redirect("MyPage.aspx");
                }
            }
        }

        // 返回不同类别歌曲链接对应的图片路径。
        protected string GetResourceImage(string type)
        {
            return "./Pics/" + type + ".png";
        }

        // 添加歌曲链接。
        protected void Button2_Click(object sender, EventArgs e)
        {
            string link = this.LinkTextBox.Text.Trim();
            string type;

            if (link == "") Response.Write("<script>alert('链接不可为空。');</script>");
            else
            {
                if (Regex.IsMatch(link, "^.*://www[.]bilibili[.]com/.*$")) type = "bilibili";
                else if (Regex.IsMatch(link, "^.*://music[.]163[.]com/.*$")) type = "163";
                else if (Regex.IsMatch(link, "^.*://music[.]apple[.]com/.*$")) type = "apple";
                else if (Regex.IsMatch(link, "^.*://www[.]youtube[.]com/.*$")) type = "youtube";
                else type = "others";

                SqlConnection conn = DataOper.Connect();
                DataSet ds = DataOper.Select(conn, "songListResources", conditions: ("WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"] + " AND resource='" + DataOper.Escape(link) + "'"));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    conn.Close();
                    Response.Write("<script>alert('链接重复。');</script>");
                }
                else
                {
                    DataOper.Insert(conn, "songListResources", Request.Params["sid"], (string)Session["uid"], "'" + DataOper.Escape(link) + "'", "'" + type + "'");
                    conn.Close();
                    this.LinkTextBox.Text = "";
                    Response.Redirect(Request.Url.ToString()); // 刷新页面以显示添加的链接。
                }
            }
        }

        // 删除歌曲链接。
        protected void DataList1_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                SqlConnection conn = DataOper.Connect();
                DataOper.Delete(conn, "songListResources", "WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"] + " AND resource='" + DataOper.Escape(e.CommandArgument.ToString()) + "'");
                conn.Close();
                Response.Redirect(Request.Url.ToString());
            }
        }

        // 跳转至歌曲笔记页面。
        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("Note.aspx?sid=" + Request.Params["sid"]);
        }

        // 更新歌曲tie-up。
        protected void Button4_Click(object sender, EventArgs e)
        {
            string tieup = this.DropDownList1.SelectedItem.Value;
            SqlConnection conn = DataOper.Connect();
            DataOper.Update(conn, "songListSongs", "WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"], "tieup=" + tieup);
            conn.Close();
            Response.Redirect(Request.Url.ToString());
        }

        // 更新歌曲整理情况。
        protected void Button5_Click(object sender, EventArgs e)
        {
            string status = this.DropDownList2.SelectedItem.Value;
            if (this.HiddenField1.Value == "1")
            {
                string strStatus = "";
                switch (status)
                {
                    case "0":
                        strStatus = "待整理";
                        break;
                    case "2":
                        strStatus = "可整理";
                        break;
                    case "3":
                        strStatus = "不整理";
                        break;
                }
                Response.Write("<script>alert('存在歌曲笔记，无法更改整理情况至" + strStatus + "。');</script>");
            }
            else if (status == "1") Response.Write("<script>alert('不存在歌曲笔记，无法更改整理情况至已整理。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataOper.Update(conn, "songListSongs", "WHERE uid=" + Session["uid"] + " AND sid=" + Request.Params["sid"], "status=" + status);
                conn.Close();
                Response.Redirect(Request.Url.ToString());
            }
        }
    }
}