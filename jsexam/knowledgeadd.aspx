<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="knowledgeadd.aspx.cs" Inherits="jsexam.knowledgeadd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div style="margin-left:20px;width:800px;margin-top:10px;">
<div style="height:32px;line-height:32px;color:#333">
  <div class="flt QuseTitle" style="font-size:16px;">模块类型:</div>
   <select id="quse_type" class="flt" style="font-size:14px;margin-top:2px;margin-left:4px;height:26px;line-height:26px; margin-right:4px;">
	  <%for (int i = 0; i < ModuleList.Rows.Count; i++)
       {%>
         <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%></option>
     <%} %>
   </select>
</div>
<div id="anwserpaner">
<div><span>知识点设置</span></div>
<ul class="knowledge_ul">
<li id="li_1">
<input type="text" value=""/>
<img id="img_1" src="img/yc.png" alt="" onclick="Deknowledge(this);" />
</li>
<li id="li_2">
<input type="text" value=""/>
<img id="img_2" src="img/yc.png" alt="" onclick="Deknowledge(this);"/>
</li>
<li id="li_3">
<input type="text" value=""/>
<img id="img_3" src="img/yc.png" alt="" onclick="Deknowledge(this);"/>
</li>
<li id="li_4">
<input type="text" value=""/>
<img id="img_4" src="img/yc.png" alt="" onclick="Deknowledge(this);"/>
</li>
</ul>
<div class="cb"></div>
<div style="font-size:12px;cursor:pointer;color:#1f4ba4" onclick="addknowledgeItem('knowledge_ul');">+新增知识点</div>
<div class="btn" style="margin-top:30px;" onclick="Addknowledge();">新增知识点</div>
</div>
</div>
</asp:Content>
