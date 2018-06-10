<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="jsexam.Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="condtionpaner" style="margin-left: 12px">
        <div class="flt QuseTitle" style="font-size: 16px;">模块类型:</div>
        <select id="quse_model" class="flt" style="font-size: 14px; margin-top: 2px; margin-left: 4px; height: 26px; line-height: 26px; margin-right: 4px;" onchange="setResultSearch();">
            <option value="-1">- 未设置 -</option>
            <%for (int i = 0; i < ModuleList.Rows.Count; i++)
                {%>
            <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%> [共:<%=ModuleList.Rows[i]["count"].ToString() %>个知识点]</option>
            <%} %>
        </select>
         <div class="flt QuseTitle" style="font-size: 16px;">所属单位:</div>
        <select id="quse_work" class="flt" style="font-size: 14px; margin-top: 2px; margin-left: 4px; height: 26px; line-height: 26px; margin-right: 4px;" onchange="setResultSearch();">
            <option value="-1">- 未设置 -</option>
            <%for (int i = 0; i < Works.Count; i++)
                {%>
            <option value="<%=Works[i].key %>"><%=Works[i].val%></option>
            <%} %>
        </select>
        <div class="flt QuseTitle" style="font-size: 16px;">考试时间:</div>
        <select id="quse_year" class="flt" style="font-size: 14px; margin-top: 2px; margin-left: 4px; height: 26px; line-height: 26px; margin-right: 4px;" onchange="setResultSearch();">
            <option value="-1">- 未设置 -</option>
            <%for (int i = 0; i < HaveTime.Count; i++)
                {%>
            <option value="<%=HaveTime[i] %>"><%=HaveTime[i]%> 年</option>
            <%} %>
        </select>       
        <div class="cb"></div>
        <div class="flt QuseTitle" style="font-size: 16px;margin-top:6px;">身份证号:</div>
        <input class="flt" id="quse_sfz" value="" placeholder="请输入身份证号码" style="width: 400px; margin-top: 6px" onchange="setResultSearch();" />
        <a class="flt" style="margin-left: 10px; margin-top: 6px" href="Results.aspx">清除查询</a>
    </div>
    <div class="cb"></div>
    <table class="quse_tb" style="margin-top: 10px; margin-left: 20px;">
        <tr class="quse_head">
            <td style="width: 200px">考试模块</td>
            <td>姓名</td>
            <td>性别</td>
            <td>身份证号</td>
            <td>所属单位</td>
            <td>考试场次</td>
            <td>考试得分</td>
        </tr>
        <%
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                int id = int.Parse(tb.Rows[i]["id"].ToString());
                string module_name = tb.Rows[i]["module_name"].ToString();
                string user_name = tb.Rows[i]["user_name"].ToString();
                string user_sex = tb.Rows[i]["user_sex"].ToString();
                string user_card = tb.Rows[i]["user_card"].ToString();
                string work_name = tb.Rows[i]["work_name"].ToString();
                string exami_name = tb.Rows[i]["exami_name"].ToString();
                string score = tb.Rows[i]["score"].ToString();
        %>
        <tr class="quse_list" id="tr_<%=id %>">
            <td style="width: 100px"><%=module_name %></td>
            <td><%=user_name %></td>
            <td><%=user_sex %></td>
            <td><%=user_card %></td>
            <td><%=work_name %></td>
            <td><%=exami_name %></td>
            <td><%=score %></td>
        </tr>
        <%}%>
        <tr class="quse_bottom">
            <td colspan="7">
                <div class="flt">当前查询共 [<%=SumCount%>] 人次</div>
                <div class="flr" style="margin-right:10px;"><a href="javascript:;" style="color:#007aff" onclick="ExportExamitoXls();">导出表格</a></div>
            </td>
        </tr>
    </table>
    <div style="margin: 0 auto; margin-left: 20px; margin-top: 10px; height: 60px; margin-bottom: 20px; margin-bottom: 20px; width: 1000px">
        <%=fy%>
    </div>
    <script>
        var module_id = GetQueryString("exami_module");
        var work_id = GetQueryString("work_id");
        var year = GetQueryString("start_date");
        var user_card = GetQueryString("user_card");
        if (module_id != null && module_id != "" && module_id != "-1") {
            $("#quse_model").val(module_id);
        }
        if (work_id != null && work_id != "" && work_id!="-1") {
            $("#quse_work").val(work_id);
        }
        if (year != null && year != "" && year != "-1") {
            $("#quse_year").val(year);
        }
        if (user_card != null && user_card != "") {
            $("#quse_sfz").val(user_card);
        }
    </script>
</asp:Content>
