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
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] != null) Response.Redirect("MyPage.aspx");
        }

        // 注册检测。
        protected void Button1_Click(object sender, EventArgs e)
        {
            string username = this.TextBox1.Text.Trim();
            string password = this.TextBox2.Text.Trim();
            string confirmedPassword = this.TextBox3.Text.Trim();

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
            //else if (username.Length > 20)
            //{
            //    Response.Write("<script>alert('用户名过长。');</script>");
            //}
            //else if (password.Length > 20)
            //{
            //    Response.Write("<script>alert('密码过长。');</script>");
            //}
            else if (password != confirmedPassword)
            {
                Response.Write("<script>alert('确认密码错误。');</script>");
            }
            else
            {
                SqlConnection conn = DataOper.Connect();
                DataOper.Initialise(conn);
                string strSQL = "SELECT * FROM songListUsers WHERE username=@username";
                DataSet ds = DataOper.Select(conn, strSQL, new Dictionary<string, object> { ["@username"] = username } );

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Response.Write("<script>alert('用户名重复。');</script>");
                    conn.Close();
                }
                else
                {
                    DataOper.Insert(conn, "songListUsers", username, password);
                    conn.Close();
                    Response.Redirect("Login.aspx?register=success");
                }
            }
        }
    }
}