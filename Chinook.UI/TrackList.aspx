<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrackList.aspx.cs" Inherits="Chinook.UI.TrackList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" AllowSorting="True" PageSize="10" DataSourceID="ObjectDataSource1">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="AlbumName" HeaderText="Album" 
                SortExpression="" />
            <asp:BoundField DataField="MediaTypeId" HeaderText="MediaTypeId" 
                SortExpression="MediaTypeId" />
            <asp:BoundField DataField="GenreId" HeaderText="GenreId" 
                SortExpression="GenreId" />
            <asp:BoundField DataField="Composer" HeaderText="Composer" 
                SortExpression="Composer" />
            <asp:BoundField DataField="Milliseconds" HeaderText="Milliseconds" 
                SortExpression="Milliseconds" />
            <asp:BoundField DataField="Bytes" HeaderText="Bytes" SortExpression="Bytes" />
            <asp:BoundField DataField="UnitPrice" HeaderText="UnitPrice" 
                SortExpression="UnitPrice" />
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
    SortParameterName="sortBy"  
    SelectCountMethod="GetCount" 
    EnablePaging="true" 
    SelectMethod="GetAll" 
    TypeName="Chinook.Repository.TrackRepository">
       
    </asp:ObjectDataSource>

</asp:Content>
