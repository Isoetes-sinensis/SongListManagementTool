using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace SongListManagementTool
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] != null) Response.Redirect("MyPage.aspx");
            if (Request.Params["register"] == "success") Response.Write("<script>alert('注册成功。');</script>");
            if (Request.Params["quit"] == "success") Response.Write("<script>alert('退出成功。');</script>");
        }

        // 登录检测。判断用户名和密码是否正确，若正确则进入系统。
        protected void Button1_Click(object sender, EventArgs e)
        {
            string username = this.TextBox1.Text.Trim();
            string password = this.TextBox2.Text.Trim();

            if (username == "" || password == "")
            {
                Response.Write("<script>alert('用户名和密码不可为空。');</script>");
            }
            else if (!Regex.IsMatch(username, "^([A-Za-z0-9]|_)+$"))
            {
                Response.Write("<script>alert('用户名只能包含半角英文字母、半角数字和下划线。');</script>");
            }
            else if (!Regex.IsMatch(password, "^([A-Za-z0-9]|_)+$"))
            {
                Response.Write("<script>alert('密码只能包含半角英文字母、半角数字和下划线。');</script>");
            }
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataOper.Initialise(conn);
                DataSet ds = DataOper.Select(conn, "songListUsers", conditions: ("WHERE username = '" + username + "'"));
                conn.Close();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (password == ds.Tables[0].Rows[0]["password"].ToString().Trim())
                    {
                        Session["username"] = username;
                        Session["uid"] = ds.Tables[0].Rows[0]["uid"].ToString().Trim();
                        Response.Redirect("MyPage.aspx");
                    }
                    else
                    {
                        Response.Write("<script>alert('密码错误。');</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('用户名错误。');</script>");
                }
            }
        }
    }
}