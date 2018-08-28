<%@ Page Title="" Language="C#" MasterPageFile="~/mast.Master" AutoEventWireup="true" CodeBehind="questionlist.aspx.cs" Inherits="jsexam.questionlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="condtionpaner" style="margin-left: 12px;">
        <%
            if (QuseTypeList.Rows.Count == 0)
            {%>
        <a href="questionadd.aspx">
            <div style="margin-top:160px;margin-left:320px;font-size:32px;">暂无数据,点击录入试题!</div>
        </a>
        <%}%>
        <%else
        {%>
        <div class="flt">试题类型:</div>
        <select class="flt" id="ipt_tx" onchange="setQtypeSearch();">
            <option value="0">所有试题 [<%= QuseTypeList.Rows[0]["csum"].ToString() %>道]</option>
            <%for (int i = 0; i < QuseTypeList.Rows.Count; i++)
                {
                    string qtype = QuseTypeList.Rows[i]["qtype"].ToString();
                    string qtype_str = qtype == "1" ? "单选题" : (qtype == "2" ? "多选题" : "判断题");
            %>
            <option value="<%=QuseTypeList.Rows[i]["qtype"].ToString() %>"><%=qtype_str%> [<%=QuseTypeList.Rows[i]["ct"].ToString()%>道]</option>
            <%} %>
        </select>
        <div class="flt">模块类型:</div>
        <select id="ctr_module" class="flt" onchange="setQtypeSearch();">
            <option value="-1">所有模块 [<%=ModuleList.Rows[0]["csum"].ToString()%>个知识点]</option>
            <%for (int i = 0; i < ModuleList.Rows.Count; i++)
                {%>
            <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%> [<%=ModuleList.Rows[i]["count"].ToString() %>个知识点]</option>
            <%} %>
        </select>
        <div class="flt">知识点:</div>
        <select id="kng_id" class="flt" onchange="setQtypeSearch();" style="max-width: 260px;">
            <option value="-1">- 请选择 -</option>
        </select>
        <div class="cb"></div>
        <div class="flt" style="margin-top: 6px">试题内容:</div>
        <input class="flt" id="ipt_tg" value="" placeholder="请输入试题内容查询" onchange="setQtypeSearch();" style="width: 520px; margin-top: 6px" />
        <a class="flt" style="margin-left: 10px; margin-top: 6px" href="questionlist.aspx">清除查询</a>
    </div>
    <div class="cb"></div>
    <table class="quse_tb" style="margin-top: 10px; margin-left: 20px;">
        <tr class="quse_head">
            <td style="width: 100px">试题类型</td>
            <td>题干内容</td>
            <td style="width: 80px">操作</td>
        </tr>
        <%
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                string id = tb.Rows[i]["id"].ToString();
                string qtype = tb.Rows[i]["qtype"].ToString();
                string mid = tb.Rows[i]["md_id"].ToString();
                string kng_id = tb.Rows[i]["kng_id"].ToString();
                //string md_str = tb.Rows[i]["md_str"].ToString();
                string content = HttpUtility.HtmlDecode(tb.Rows[i]["qcontent"].ToString());
                string qtype_str = qtype == "1" ? "单选题" : (qtype == "2" ? "多选题" : "判断题");
                string list_str = HttpUtility.UrlEncode(tb.Rows[i]["qlist"].ToString());
        %>
        <tr class="quse_list" id="tr_<%=id %>">
            <td style="width: 100px" data-original="<%=qtype %>"><%=qtype_str%></td>
            <td data-original="<%=mid %>,<%=kng_id%>" style="text-align: left"><%=content %></td>
            <td data-original="<%=list_str%>"><a href="javascript:;" onclick="SetUpPaner(<%=id%>);">修改</a><a href="javascript:;" onclick="DeQues(<%=id%>)"> 删除</a></td>
        </tr>
        <%}%>
        <tr class="quse_bottom">
            <td colspan="3">
                <div class="flt">当前查询共 [<%=SumCount%>] 道试题</div>
            </td>
        </tr>
    </table>
    <div style="margin: 0 auto; margin-left: 20px; margin-top: 10px; height: 60px; margin-bottom: 20px; margin-bottom: 20px; width: 1000px">
        <%=fy%>
    </div>
    <div class="Edit_paner">
        <div class="Edit_title">
            <span>试题编辑</span>
            <span class="Edit_Close">X</span>
        </div>
        <div style="margin-left: 12px;">
            <div style="height: 32px; line-height: 32px; color: #333">
                <div class="flt QuseTitle" style="font-size: 16px;">试题类型:</div>
                <select id="quse_type" class="flt" style="font-size: 14px; height: 24px; margin-top: 4px; margin-left: 4px; margin-right: 4px;">
                    <option value="1">单选题</option>
                    <option value="2">多选题</option>
                    <option value="3">判断题</option>
                </select>
                <div class="flt QuseTitle" style="margin-left: 10px; font-size: 16px;">模块类型:</div>
                <select id="up_ctr_module" class="flt" style="width: 138px; height: 24px; font-size: 14px; margin-top: 4px; margin-left: 4px; margin-right: 4px;" onchange="setkng3(this);">
                    <option value="-1">- 请选择 -</option>
                    <%for (int i = 0; i < ModuleList.Rows.Count; i++)
                        {%>
                    <option value="<%=ModuleList.Rows[i]["id"].ToString() %>"><%=ModuleList.Rows[i]["module_name"].ToString()%></option>
                    <%} %>
                </select>
                <div class="flt QuseTitle" style="margin-left: 10px; font-size: 16px;">知识点:</div>
                <select id="up_kng_id" class="flt" style="width: 232px; height: 24px; font-size: 14px; margin-top: 4px; margin-left: 4px; margin-right: 4px;">
                    <option value="-1">- 请选择 -</option>
                </select>
            </div>
            <div class="cb"></div>
            <div class="QuseTitle" style="font-size: 16px;">试题题干</div>
            <textarea id="qcontent" style="margin-top: 6px; width: 660px; height: 140px; font-size: 14px;"></textarea>
            <div id="anwserpaner">
                <div><span>选项设置</span><span style="margin-left: 520px">正确答案</span></div>
                <div style="height: 160px; overflow-y: auto">
                    <ul class="quse_ul">
                    </ul>
                </div>
                <div class="cb"></div>
                <div style="font-size: 12px; cursor: pointer; color: #1f4ba4" onclick="addChoseItem('quse_ul');">+新增选项</div>
            </div>
            <div class="cb"></div>
            <div class="Edit_submit" style="position: relative; left: 600px;"><a href="javascript:;" onclick="UpQues();">修改</a> <a href="javascript:;" onclick="CloseMaskPaner();">取消</a></div>
        </div>
    </div>

    <script>
        var tp = GetQueryString("tp");
        var tg = GetQueryString("tg");
        var pg = GetQueryString("page");
        var mid = GetQueryString("mid");
        var kngid = GetQueryString("kng_id");
        var dtct = GetQueryString("dtct");
        if (tp != null && tp != "" && tp != "0") {
            $("#ipt_tx").val(tp);
        }
        if (tg != null && tg != "") {
            tg = decodeURIComponent(tg);
            $("#ipt_tg").val(tg);
        }
        if (mid != null && mid != "" && mid != "-1") {
            $("#ctr_module").val(mid);
            setkng2();
        }
        if (kngid != null && kngid != "" && kngid != "-1") {
            $("#kng_id").val(kngid);
        }
        if (dtct != null && dtct != "") {
            $("#dtct").val(dtct);
        }
    //if (pg!=null&&pg!=""&&pg!="0")
    //{
    //    $('#tzym_t').val(pg);
    //}
    </script>
    <%}%>
</asp:Content>
