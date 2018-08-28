<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="knowledgepoint.aspx.cs" Inherits="jsexam.knowledgepoint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <table class="quse_tb" style="margin-top:20px;margin-left:20px">
    <tr class="quse_head">
        <td style="width: 100px">模块类型</td>
        <td>知识点</td>
        <td style="width: 120px">操作</td>
    </tr>
    <tr class="quse_search">
        <td style="width: 100px">
            <select id="ipt_tx"  onchange="setKNGtypeSearch();">
                <option value="0">查询全部</option>
                <%for (int i = 0; i < ModuleList.Rows.Count; i++)
                    {%>
                      <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%> [共:<%=ModuleList.Rows[i]["count"].ToString() %>个知识点]</option>
                    <%}
                    %>
            </select>
        </td>
        <td>
            <input id="ipt_tg" value="" placeholder="请输入试知识点内容查询" onchange="setKNGtypeSearch();" />
        </td>
        <td style="width: 120px">
            <a href="knowledgepoint.aspx">清除查询</a>
        </td>
    </tr>
        <%
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                string id = tb.Rows[i]["id"].ToString();
                string mid = tb.Rows[i]["mid"].ToString();
                string mstr = tb.Rows[i]["mstr"].ToString();
                string content = HttpUtility.HtmlDecode(tb.Rows[i]["content"].ToString());
          %>
                 <tr class="quse_list" id="tr_<%=id %>">
        <td style="width: 100px" data-original="<%=mid %>"><%=mstr%></td>
        <td><%=content %></td>
        <td style="width: 120px"><a href="javascript:;" onclick="SetKlGUpPaner(<%=id%>);">修改</a><a href="javascript:;" onclick="DeKLG(<%=id%>)" > 删除</a></td>
    </tr>
       <%}%>
   <tr class="quse_bottom">
       <td colspan="3">
          <div class="flt">当前查询共 [<%=SumCount%>] 个知识点</div>
       </td>
   </tr>
</table>
<div style="margin:0 auto;margin-left:20px; margin-top:10px; margin-bottom:20px; width:1000px">
 <%=fy%>
</div>
<div class="Edit_paner" style="width:500px;margin-left:-294px">
    <div class="Edit_title">
        <span>知识点编辑</span>
        <span class="Edit_Close">X</span>
    </div>
    <div style="margin-left: 12px;">
        <div style="height:32px;line-height:32px;color:#333">
  <div class="flt QuseTitle" style="font-size:18px;width:84px;text-align:center">模块类型:</div>
   <select id="quse_type" class="flt" style="font-size:14px;margin-top:3px;margin-left:4px;width:220px;height:26px;line-height:26px; margin-right:4px;">
	  <%for (int i = 0; i < ModuleList.Rows.Count; i++)
      {%>
         <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%> [共:<%=ModuleList.Rows[i]["count"].ToString() %>个知识点]</option>
     <%}%>
   </select>
</div>
<div class="cb"></div>
<div style="margin-top:10px;">
<div class="flt QuseTitle" style="font-size:18px;width:84px;text-align:center">知 识 点:</div>
<input id="qcontent" class="flt" type="text" style="margin-left:4px; width:380px;height:26px;line-height:26px;font-size:14px; "/>
</div>
<div class="cb"></div>

<div class="Edit_submit" style="position: relative;left: 416px;"><a href="javascript:;" onclick="UpKLG();" >修改</a> <a href="javascript:;" onclick="CloseMaskPaner();">取消</a></div>
</div>
</div>

<script>
    var mid = GetQueryString("mid");
    var zsd = GetQueryString("zsd");
    var pg = GetQueryString("page");
    var dtct = GetQueryString("dtct");
    if (mid != null && mid != "" && mid != "0") {
        $("#ipt_tx").val(mid);
    }
    if (zsd != null && zsd != "") {
        zsd = decodeURIComponent(HTMLDeCode(zsd));
        $("#ipt_tg").val(zsd);
    }
    if (dtct != null && dtct != "") {
        $("#dtct").val(dtct);
    }
    //if (pg!=null&&pg!=""&&pg!="0")
    //{
    //    $('#tzym_t').val(pg);
    //}
</script>
</asp:Content>
