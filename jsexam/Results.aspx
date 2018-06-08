<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="jsexam.Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="condtionpaner" style="margin-left: 12px">
        <div class="flt QuseTitle" style="font-size: 16px;">模块类型:</div>
        <select id="quse_type" class="flt" style="font-size: 14px; margin-top: 2px; margin-left: 4px; height: 26px; line-height: 26px; margin-right: 4px;">
            <option value="-1">- 未设置 -</option>
            <%for (int i = 0; i < ModuleList.Rows.Count; i++)
                {%>
            <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%> [共:<%=ModuleList.Rows[i]["count"].ToString() %>个知识点]</option>
            <%} %>
        </select>
         <div class="flt QuseTitle" style="font-size: 16px;">所属单位:</div>
        <select id="quse_work" class="flt" style="font-size: 14px; margin-top: 2px; margin-left: 4px; height: 26px; line-height: 26px; margin-right: 4px;">
            <option value="-1">- 未设置 -</option>
            <%for (int i = 0; i < Works.Count; i++)
                {%>
            <option value="<%=Works[i].key %>"><%=Works[i].val%></option>
            <%} %>
        </select>
        <div class="flt QuseTitle" style="font-size: 16px;">考试时间:</div>
        <select id="quse_year" class="flt" style="font-size: 14px; margin-top: 2px; margin-left: 4px; height: 26px; line-height: 26px; margin-right: 4px;">
            <option value="-1">- 未设置 -</option>
            <%for (int i = 0; i < HaveTime.Count; i++)
                {%>
            <option value="<%=HaveTime[i] %>"><%=HaveTime[i]%> 年</option>
            <%} %>
        </select>       
        <div class="cb"></div>
        <div class="flt QuseTitle" style="font-size: 16px;margin-top:6px;">身份证号:</div>
        <input class="flt" id="ipt_sfz" value="" placeholder="请输入身份证号码" onchange="" style="width: 400px; margin-top: 6px" />
        <a class="flt" style="margin-left: 10px; margin-top: 6px" href="Results.aspx">清除查询</a>
    </div>
</asp:Content>
