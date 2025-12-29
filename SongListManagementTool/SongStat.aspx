<%@ Page Title="" Language="C#" MasterPageFile="~/navbarLoggedIn.Master" AutoEventWireup="true" CodeBehind="SongStat.aspx.cs" Inherits="SongListManagementTool.SongStat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="navbar2" runat="server">
    <div class="row p-5 rounded-3">
        <h2>歌曲分类统计</h2>
        <div class="col-4">
            名称：<br />
            <asp:TextBox ID="TextBox1" runat="server" MaxLength="100"></asp:TextBox>
            <br />
            类型：<br />
            <asp:DropDownList ID="DropDownList1" runat="server">
                <asp:ListItem Value="NULL">(请选择)</asp:ListItem>
                <asp:ListItem Value="anime">动画</asp:ListItem>
                <asp:ListItem Value="comic">漫画</asp:ListItem>
                <asp:ListItem Value="novel">轻小说</asp:ListItem>
                <asp:ListItem Value="media">跨媒体企划</asp:ListItem>
                <asp:ListItem Value="game">一般游戏</asp:ListItem>
                <asp:ListItem Value="galgame">GALGAME/视觉小说</asp:ListItem>
                <asp:ListItem Value="vocaloid">泛VOCALOID</asp:ListItem>
                <asp:ListItem Value="other">其他</asp:ListItem>
            </asp:DropDownList>
            <br />
            <asp:Button ID="Button1" runat="server" CssClass="btn btn-light" Text="添加" 
                onclick="Button1_Click" />
            <asp:Button ID="Button2" runat="server" CssClass="btn btn-light" Text="查询" />
        </div>

        <div class="col-8">
            说明：<br />
            1. 该页面按歌曲的tie-up/类型（下称“作品”）对歌曲的数量和整理情况等进行分类统计。<br />
            2. 系列作品一般根据作品的独立性合并或分列。<br />
            3. 若作品涉及多种类型，“类型”字段一般只显示作品中歌曲最主要的来源类型（除明确的跨媒体企划外）。
        </div>
    </div>

    <div class="table-responsive">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            AllowPaging="True" AllowSorting="True" PageSize="50"
            OnPageIndexChanging="GridView1_PageIndexChanging"
            CssClass="table table-striped" onrowcreated="GridView1_RowCreated">
            <Columns>
                <asp:BoundField DataField="tid" HeaderText="tid" ReadOnly="True" />
                <asp:BoundField HeaderText="编号" ReadOnly="True" />
                <asp:BoundField DataField="name" HeaderText="名称" />
                <asp:BoundField DataField="type" HeaderText="类型" />
                <asp:BoundField DataField="total" HeaderText="歌曲数" />
                <asp:BoundField DataField="waiting" HeaderText="待整理" />
                <asp:BoundField DataField="completed" HeaderText="已整理" />
                <asp:BoundField DataField="optional" HeaderText="可整理" />
                <asp:BoundField DataField="excluded" HeaderText="不整理" />
                <asp:BoundField DataField="progress" HeaderText="进度" />
            </Columns>
            <PagerSettings Mode="NumericFirstLast" />
        </asp:GridView>
    </div>

</asp:Content>
