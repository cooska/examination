<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="questionadd.aspx.cs" Inherits="jsexam.questionadd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-left:20px;width:800px;">
<div style="height:32px;line-height:32px;color:#333">
  <div class="flt QuseTitle" style="font-size:16px;">试题类型:</div>
   <select id="quse_type" class="flt" style="height:26px;font-size:14px;margin-top:4px; margin-left:4px;margin-right:4px;">
      <option value="1">单选题</option>
	  <option value="2">多选题</option>
	  <option value="3">判断题</option>
   </select>
     <div class="flt QuseTitle" style="margin-left:10px;font-size:16px;">模块类型:</div>
    <select id="ctr_module" class="flt" style="height:26px;font-size:14px;margin-top:4px; margin-left:4px;margin-right:4px;" onchange="setkng(this);">
      <option value="-1">- 请选择 -</option>
       <%for (int i = 0; i < ModuleList.Rows.Count; i++)
       {%>
         <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%></option>
     <%} %>
   </select>
    <div class="flt QuseTitle" style="margin-left:10px;font-size:16px;">知识点:</div>
    <select id="kng_id" class="flt" style="height:26px;max-width:260px;font-size:14px;margin-top:4px; margin-left:4px;margin-right:4px;">
      <option value="-1">- 请选择 -</option>
   </select>
</div>
<div class="cb"></div>
<div class="QuseTitle" style="font-size:16px;">试题题干</div>
<textarea id="qcontent" style="margin-top:10px;width:660px;height:140px;font-size:14px; "></textarea>
<div id="anwserpaner">
<div><span>选项设置</span><span style="margin-left:520px">正确答案</span></div>
<ul class="quse_ul">
<li id="li_1">
<input type="text" value=""/>
<input type="checkbox" style="width:20px;margin-left:40px;margin-top:2px;">
<img id="img_1" src="img/yc.png" alt="" onclick="DeChoseItem(this);" />
</li>
<li id="li_2">
<input type="text" value=""/>
<input type="checkbox" style="width:20px;margin-left:40px;margin-top:2px;">
<img id="img_2" src="img/yc.png" alt="" onclick="DeChoseItem(this);"/>
</li>
<li id="li_3">
<input type="text" value=""/>
<input type="checkbox" style="width:20px;margin-left:40px;margin-top:2px;">
<img id="img_3" src="img/yc.png" alt="" onclick="DeChoseItem(this);"/>
</li>
<li id="li_4">
<input type="text" value=""/>
<input type="checkbox" style="width:20px;margin-left:40px;margin-top:2px;">
<img id="img_4" src="img/yc.png" alt="" onclick="DeChoseItem(this);"/>
</li>
</ul>
<div class="cb"></div>
<div style="font-size:12px;cursor:pointer;color:#1f4ba4" onclick="addChoseItem('quse_ul');">+新增选项</div>
<div class="btn" style="margin-top:30px;" onclick="AddQues();">新增试题</div>
</div>
</div>
</asp:Content>
