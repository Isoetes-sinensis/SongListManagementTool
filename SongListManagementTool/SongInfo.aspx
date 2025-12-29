<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SongInfo.aspx.cs" Inherits="SongListManagementTool.SongInfo" MasterPageFile="navbarLoggedIn.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="navbar2" runat="server">
    <script src="./Scripts/jquery-1.4.1.js"></script>
    <script>
        $().ready(() => {
            $("#resources img").width(25).height(25);
        });
    </script>

    <div class="p-5 rounded-3">
        <h2>基本信息</h2>
        <div>
            歌名：<br />
            <asp:TextBox ID="TitleTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            <br />演唱者：<br />
            <asp:TextBox ID="ArtistTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            <br />
            作词：<br />
            <asp:TextBox ID="LyricistTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            <br />
            作曲：<br />
            <asp:TextBox ID="ComposerTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            <br />
            编曲：<br />
            <asp:TextBox ID="ArrangerTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            <br />
            Tie-up：<br />
            <asp:DropDownList ID="DropDownList1" runat="server">
            </asp:DropDownList>
            <asp:Button ID="Button4" runat="server" Text="更新" onclick="Button4_Click" 
                CssClass="btn btn-light" />
            <br />
            专辑：<br />
            <asp:TextBox ID="AlbumTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            <br />
            整理情况：<br />
            <asp:DropDownList ID="DropDownList2" runat="server">
                <asp:ListItem Value="0">待整理</asp:ListItem>
                <asp:ListItem Value="1">已整理</asp:ListItem>
                <asp:ListItem Value="2">可整理</asp:ListItem>
                <asp:ListItem Value="3">不整理</asp:ListItem>
            </asp:DropDownList>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <asp:Button ID="Button5" runat="server" Text="更新" onclick="Button5_Click" 
                CssClass="btn btn-light" />
        </div>
        <br />

        <h2>歌曲资源</h2>
        <div id="resources">
            外部链接：<br />
            <asp:DataList ID="DataList1" runat="server" RepeatDirection="Horizontal" 
                RepeatLayout="Flow" onitemcommand="DataList1_ItemCommand">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" 
                        ImageUrl='<%# GetResourceImage((string) Eval("type")) %>' 
                        NavigateUrl='<%# Eval("resource") %>'>
                        
                    [HyperLink1]</asp:HyperLink>
                    <asp:LinkButton ID="LinkButton2" runat="server" 
                        CommandArgument='<%# Eval("resource") %>' CommandName="Delete">删除</asp:LinkButton>
                </ItemTemplate>
            </asp:DataList>
            <br />
            <br />
            添加链接：<br />
            <asp:TextBox ID="LinkTextBox" runat="server" MaxLength="120"></asp:TextBox>
            <asp:Button ID="Button2" runat="server" Text="添加" onclick="Button2_Click" 
                CssClass="btn btn-light" />
        </div>
        <br />

        <h2>歌曲笔记</h2>
        <div>
            <asp:Button ID="Button3" runat="server" Text="查看笔记" CssClass="btn btn-light" 
                onclick="Button3_Click" />
        </div>
    </div>
</asp:Content>