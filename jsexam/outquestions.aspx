<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="outquestions.aspx.cs" Inherits="jsexam.outquestions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table class="quse_tb" style="margin-top:10px;margin-left:20px;width:800px;">
    <tr class="quse_head">
        <td style="width: 100px">模块名称</td>
        <td style="width: 80px">操作</td>
    </tr>
        <%
            for (int i = 0; i < ModuleTb.Rows.Count; i++)
            {
                string id = ModuleTb.Rows[i]["id"].ToString();
                string module_name = ModuleTb.Rows[i]["module_name"].ToString();
                string kng_list = ModuleTb.Rows[i]["kng_list"].ToString();
                kng_list = HttpUtility.UrlEncode(kng_list,Encoding.UTF8);
                string ct = ModuleTb.Rows[i]["count"].ToString();
          %>
         <tr class="quse_list" id="tr_<%=id %>">
        <td data-original="<%=kng_list%>"><%=module_name%> [<%=ct%>个知识点]</td>
        <td><a href="javascript:;" onclick="setmodulekng(<%=id%>);">配置知识点</a></td>
    </tr>
       <%}%>
</table>
<div class="Edit_paner" style="width:800px;margin-left:-484px;">
    <div class="Edit_title">
        <span>知识点配置</span>
        <span class="Edit_Close">X</span>
    </div>
    <div class="cb"></div>
    <div id="kng_paner" style="margin-left:12px; width:786px;height:430px;overflow-y:auto;font-size:14px;">

    </div>
    <div class="cb"></div>
    <div class="flt" id="kng_tip">已设置[ 单选题:0道,多选题:0道,判断题:0道 ]</div>
    <div class="Edit_submit" style="position: relative;left: 366px;"><a href="javascript:;" onclick="upModuleInfo();" >保存</a> <a href="javascript:;" onclick="CloseMaskPaner();">取消</a></div>
</div>
</asp:Content>
