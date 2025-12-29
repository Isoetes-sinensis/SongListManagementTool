<%@ Page Title="" Language="C#" MasterPageFile="~/navbarLoggedIn.Master" AutoEventWireup="true" CodeBehind="Note.aspx.cs" Inherits="SongListManagementTool.Note" %>

<asp:Content ID="Content1" ContentPlaceHolderID="navbar2" runat="server">
    <asp:LinkButton ID="LinkButton2" runat="server" onclick="LinkButton2_Click">&lt;返回</asp:LinkButton>
    <div class="d-grid">
        <asp:TextBox ID="TextBox1" runat="server" CssClass="fs-5" 
            TextMode="MultiLine" placeholder="请在此处输入笔记。" Height="500px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="保存" onclick="Button1_Click" 
            CssClass="btn btn-light" />
    </div>
</asp:Content>