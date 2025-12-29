<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyPage.aspx.cs" Inherits="SongListManagementTool.MyPage" MasterPageFile="navbarLoggedIn.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="navbar2" runat="server">
    <div class="table-responsive">
        <asp:Table ID="Table1" runat="server" CssClass="table">
            <asp:TableRow runat="server">
                <asp:TableCell runat="server">歌名</asp:TableCell>
                <asp:TableCell runat="server">演唱者</asp:TableCell>
                <asp:TableCell runat="server">作词</asp:TableCell>
                <asp:TableCell runat="server">作曲</asp:TableCell>
                <asp:TableCell runat="server">编曲</asp:TableCell>
                <asp:TableCell runat="server">专辑</asp:TableCell>
                <asp:TableCell runat="server">Tie-Up/类型</asp:TableCell>
                <asp:TableCell runat="server">整理情况</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell runat="server">
                    <asp:TextBox ID="TitleTextBox" runat="server" MaxLength="100"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell runat="server">
                    <asp:TextBox ID="ArtistTextBox" runat="server" MaxLength="100"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell runat="server">
                    <asp:TextBox ID="LyricistTextBox" runat="server" MaxLength="100"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell runat="server">
                    <asp:TextBox ID="ComposerTextBox" runat="server" MaxLength="100"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell runat="server">
                    <asp:TextBox ID="ArrangerTextBox" runat="server" MaxLength="100"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell runat="server">
                    <asp:TextBox ID="AlbumTextBox" runat="server" MaxLength="100"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell ID="TableCell1" runat="server">
                    <asp:DropDownList ID="TieUpDropDownList" runat="server">
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell runat="server">
                    <asp:DropDownList ID="StatusDropDownList" runat="server">
                        <asp:ListItem Value="NULL">(请选择)</asp:ListItem>
                        <asp:ListItem Value="0">待整理</asp:ListItem>
                        <asp:ListItem Value="1">已整理</asp:ListItem>
                        <asp:ListItem Value="2">可整理</asp:ListItem>
                        <asp:ListItem Value="3">不整理</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Button ID="Button1" runat="server" Text="查询" onclick="Button1_Click" 
            CssClass="btn btn-light" />
        <asp:Button ID="Button2" runat="server" Text="添加" onclick="Button2_Click" 
            CssClass="btn btn-light" />
    </div>
    <br />
    <br />

    <div class="table-responsive">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            onrowediting="GridView1_RowEditing" OnRowDeleting="GridView1_RowDeleting"
            OnRowUpdating="GridView1_RowUpdating" 
            OnRowCancelingEdit="GridView1_RowCancelingEdit" AllowPaging="True" 
            PageSize="50" onrowcreated="GridView1_RowCreated"
            OnPageIndexChanging="GridView1_PageIndexChanging" 
            CssClass="table table-striped">
            <Columns>
                <asp:BoundField HeaderText="编号" ReadOnly="True" />
                <asp:BoundField DataField="title" HeaderText="歌名" />
                <asp:BoundField DataField="artist" HeaderText="演唱者" />
                <asp:BoundField DataField="lyricist" HeaderText="作词" />
                <asp:BoundField DataField="composer" HeaderText="作曲" />
                <asp:BoundField DataField="arranger" HeaderText="编曲" />
                <asp:BoundField DataField="album" HeaderText="专辑" />
                <asp:BoundField DataField="strTieUp" HeaderText="Tie-Up/类型" ReadOnly="True" />
                <asp:BoundField DataField="strStatus" HeaderText="状态" ReadOnly="True" ItemStyle-Width="70" />
                <asp:BoundField DataField="tieup" HeaderText="tieup" ReadOnly="True" />
                <asp:BoundField DataField="sid" HeaderText="sid" ReadOnly="True" />
                <asp:BoundField DataField="status" HeaderText="status" ReadOnly="True" />
                <asp:HyperLinkField Text="详情" DataNavigateUrlFields="sid" 
                    DataNavigateUrlFormatString="~\SongInfo.aspx?sid={0}" />
                <asp:CommandField ShowEditButton="True" />
                <asp:CommandField ShowDeleteButton="True" />
            </Columns>
            <PagerSettings Mode="NumericFirstLast" />
        </asp:GridView>
    </div>
</asp:Content>