<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="jsexam.Results" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="height:32px;line-height:32px;color:#333">
        <div style="height:32px;line-height:32px;color:#333">
  <div class="flt QuseTitle" style="font-size:16px;">模块类型:</div>
   <select id="quse_type" class="flt" style="font-size:14px;margin-top:2px;margin-left:4px;height:26px;line-height:26px; margin-right:4px;">
	  <%for (int i = 0; i < ModuleList.Rows.Count; i++)
       {%>
         <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%> [共:<%=ModuleList.Rows[i]["count"].ToString() %>个知识点]</option>
     <%} %>
   </select>
</div>
    </div>
 
</asp:Content>
