using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace SongListManagementTool
{
    public partial class SongStat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null) Response.Redirect("Login.aspx");
            else if (!IsPostBack) Bind(true);
        }

        // 绑定数据源至GridView。
        protected void Bind(bool reset = false)
        {
            if (Session["statdv"] == null || reset)
            {
                SqlConnection conn = DataOper.Connect();
                string strSQL = @"
                    SELECT st.tid, st.name, st.type,
                    ss0.total waiting, ss1.total completed, ss2.total optional, ss3.total excluded

                    FROM songListTieUp AS st

                    LEFT JOIN (
	                    SELECT COUNT(*) total, tieup, status FROM songListSongs
	                    GROUP BY tieup, status
	                    HAVING status = 0
                    ) AS ss0
                    ON ss0.tieup = st.tid

                    LEFT JOIN(
	                    SELECT COUNT(*) total, tieup, status FROM songListSongs
	                    GROUP BY tieup, status
	                    HAVING status = 1
                    ) AS ss1
                    ON ss1.tieup = st.tid

                    LEFT JOIN(
	                    SELECT COUNT(*) total, tieup, status FROM songListSongs
	                    GROUP BY tieup, status
	                    HAVING status = 2
                    ) AS ss2
                    ON ss2.tieup = st.tid


                    LEFT JOIN(
	                    SELECT COUNT(*) total, tieup, status FROM songListSongs
	                    GROUP BY tieup, status
	                    HAVING status = 3
                    ) AS ss3
                    ON ss3.tieup = st.tid

                    ORDER BY st.type DESC, st.name ASC;
                ";
                SqlCommand comm = new SqlCommand(strSQL, conn);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataSet ds = new DataSet();
                da.Fill(ds);
                conn.Close();

                // 以下调整表格显示的内容。
                ds.Tables[0].Columns.Add("total", typeof(string));
                ds.Tables[0].Columns.Add("progress", typeof(string));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string waiting = ds.Tables[0].Rows[i]["waiting"].ToString();
                    string completed = ds.Tables[0].Rows[i]["completed"].ToString();
                    string optional = ds.Tables[0].Rows[i]["optional"].ToString();
                    string excluded = ds.Tables[0].Rows[i]["excluded"].ToString();
                    ds.Tables[0].Rows[i]["total"] = this.CalcTotalCount(waiting, completed, optional, excluded); // 计算歌曲数。

                    int compNum = completed == "" ? 0 : Convert.ToInt32(completed);
                    int totalNum = compNum + (waiting == "" ? 0 : Convert.ToInt32(waiting));
                    if (totalNum == 0) ds.Tables[0].Rows[i]["progress"] = "-";
                    else
                    {
                        double progress = 100.00 * Convert.ToDouble(compNum) / Convert.ToDouble(totalNum);
                        ds.Tables[0].Rows[i]["progress"] = progress.ToString("F2") + "%"; // 计算进度。
                    }

                    string strType = "";
                    switch (ds.Tables[0].Rows[i]["type"].ToString())
                    {
                        case "anime":
                            strType = "动画";
                            break;
                        case "comic":
                            strType = "漫画";
                            break;
                        case "novel":
                            strType = "轻小说";
                            break;
                        case "media":
                            strType = "跨媒体企划";
                            break;
                        case "game":
                            strType = "一般游戏";
                            break;
                        case "galgame":
                            strType = "GALGAME/视觉小说";
                            break;
                        case "vocaloid":
                            strType = "泛VOCALOID";
                            break;
                        case "other":
                            strType = "其他";
                            break;
                    }
                    ds.Tables[0].Rows[i]["type"] = strType;
                }

                DataView dv = new DataView();
                dv.Table = ds.Tables[0];
                Session["statdv"] = dv; // 不可使用dv.Sort，否则进度列颜色不对应。
            }

            this.GridView1.DataSource = Session["statdv"];
            this.GridView1.DataBind();
        }

        // 根据给定列，求和算得每个tie-up类别下的总歌曲数。
        protected string CalcTotalCount(params string[] values)
        {
            int sum = 0;
            foreach (string value in values)
            {
                if (value != "") sum += Convert.ToInt32(value);
            }
            return sum.ToString();
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex > -1)
            {
                e.Row.Cells[1].Text = (e.Row.RowIndex + 1).ToString(); // 创建行号，即按顺序递增的tie-up编号。注意可能不同于tid。
                
                // 以下根据进度更新进度列的颜色。
                string progress = ((DataView)Session["statdv"]).Table.Rows[this.GridView1.PageIndex * this.GridView1.PageSize + e.Row.RowIndex]["progress"].ToString();
                int greenness = (progress == "-") ? 0 : Convert.ToInt32(255.00 * Convert.ToDouble(progress.TrimEnd('%')) / 100.00);
                e.Row.Cells[9].BackColor = Color.FromArgb(255 - greenness, 255, 255 - greenness);
                if (greenness > 127) e.Row.Cells[9].ForeColor = Color.White;
                if (greenness == 0 || greenness == 255) e.Row.Cells[9].Font.Bold = true;
            }
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header) e.Row.Cells[0].Visible = false; // 隐藏tid列。
        }

        // 添加tie-up信息。
        protected void Button1_Click(object sender, EventArgs e)
        {
            string tieup = this.TextBox1.Text.Trim();
            string type = this.DropDownList1.SelectedItem.Value;

            if (tieup == "") Response.Write("<script>alert('名称不可为空。');</script>");
            else if (type == "NULL") Response.Write("<script>alert('类型不可为空。');</script>");
            else
            {
                SqlConnection conn = DataOper.Connect();

                // 此处可考虑添加去重功能。

                DataOper.Insert(conn, "songListTieUp", (string)Session["uid"], tieup, type);
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