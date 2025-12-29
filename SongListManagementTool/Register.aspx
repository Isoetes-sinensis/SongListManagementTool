<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="SongListManagementTool.Register" MasterPageFile="navbar.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="navbar1" runat="server">
    <div class="row justify-content-center position-absolute top-0 vh-100 vw-100 d-flex justify-content-center align-items-center">
        <h1 class="col-3 text-center">欢迎使用歌单管理系统</h1>
        <span class="col-1"></span>
        <div class="col-3 p-5 rounded-3 text-white" style="background-color: #61B340">
            <div class="d-flex flex-column">
                <label>用户名</label>
                <asp:TextBox ID="TextBox1" runat="server" MaxLength="20"></asp:TextBox>
                <label>密码</label>
                <asp:TextBox ID="TextBox2" runat="server" TextMode="Password" MaxLength="20"></asp:TextBox>
                <label>确认密码</label>
                <asp:TextBox ID="TextBox3" runat="server" TextMode="Password" MaxLength="20"></asp:TextBox>
                <br />
                <asp:Button ID="Button1" runat="server" Text="注册" onclick="Button1_Click" 
                    CssClass="btn btn-light" />
            </div>
        </div>
    </div>
</asp:Content>
