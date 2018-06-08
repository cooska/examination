var Web_View_height=0;//屏幕高度
var Web_View_width=0;//屏幕宽度
var nav_right_width=0;//右侧主显示宽度
var CanClick = false;
var upid = 0;//修改资源id
window.onload = function () {
    SetView();
    SetLiColor();
};
$(document).ready(function () {
  InitFunction();
});
//窗体大小改变
window.onresize = function(){
   SetView();
};
window.onscroll = function () {
   SetView();
};
//设置主控制页面参数
function SetView()
{
    Web_View_height = document.documentElement.clientHeight;
    Web_View_height = Web_View_height + (window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0);
    Web_View_height = Web_View_height-60;
    Web_View_width = document.documentElement.clientWidth;
    nav_right_width = Web_View_width - 240;
    $('body').css("height",Web_View_height+"px");
    $(".nav_right").css("width", nav_right_width + "px");
    $(".MaskPaner").css("height", (Web_View_height+60)+"px");
}
function InitFunction()
{
    //退出登陆
     $(".nrtl_msg").click(function(event)
     {
        ShowSingOut();
        var e = arguments.callee.caller.arguments[0] || event; //若省略此句，下面的e改为event，IE运行可以，但是其他浏览器就不兼容
           if (e && e.stopPropagation) {
               // this code is for Mozilla and Opera
                   e.stopPropagation();
              } else if (window.event) {
                  // this code is for IE
                  window.event.cancelBubble = true;
           }
        
    });
    $("body").click(function(){
        body_Click_ShowSingOut();
     });
    //关闭弹窗
    $('.Edit_Close').click(function () {
        CloseMaskPaner();
    });
}
function ShowSingOut()
{
    var ctr =$(".singout");
    var stat = $(ctr).css("display");
        if(stat=="none")
        {
          $(ctr).show();
        }
        else{
            $(ctr).hide();
        }
}
function body_Click_ShowSingOut()
{
    var ctr = $(".singout");
    var stat = $(ctr).css("display");
        if(stat !="none")
        {
          $(ctr).hide();
        }
}
/*用户登录*/
function usLogin()
{
    var name = $('#usname').val();
    var pass = $("#uspass").val();
    if (name=="")
    {
        alert("请输入用户名!");
        return;
    }
    if (pass=="")
    {
        alert("请输入密码!");
    }
    if (name!=""&&pass!="")
    {
        var rst = ajxpost("action.aspx?actp=login&name=" + name + "&pass=" + pass + "");
        switch (rst)
        {
            case "1":
                gourl("questionadd.aspx");
                break;
            case "2":
                alert("密码错误!");
                break;
            case "0":
                alert("没有该用户!");
                break;
        }
    }
}
function sigout()
{
    if (confirm("确定要退出系统吗!?")) {
        var rst = ajxpost("action.aspx?actp=loginout");
        if (rst > 0) {
            gourl("index.aspx");
        }
        else {
            alert("退出失败!");
        }
    }
}
/*试题模块*/
var ChoseItem = "<li id=\"{li_th}\"><input type=\"text\" value=\"{tx_th}\"/><input type=\"checkbox\" {ck_th} class=\"li_ckbox\"><img id=\"{img_th}\" src=\"img/yc.png\" alt=\"\"/ onclick=\"DeChoseItem(this);\"></li>";
function addChoseItem(id) {
    var ul_ctr = $("." + id);
    var count = $(ul_ctr).find("li");
    count = count.length;
    count = parseInt(count);
    if (count <8) {
        var newid = guid();
        var newChoseItem = ChoseItem.replace("{li_th}", "li_" + newid);
        newChoseItem = newChoseItem.replace("{img_th}", "li_" + newid);
        newChoseItem = newChoseItem.replace("{tx_th}", "");
        newChoseItem = newChoseItem.replace("{ck_th}", "");
        $(ul_ctr).append(newChoseItem);
        var ul_ctr = $("." + id);
        var count = $(ul_ctr).find("li");
        count = count.length;
        if (count < 5) {
            $(".quse_ul li img").hide();
        }
        else {
            $(".quse_ul li img").show();
        }
    }
    else {
        alert("试题选项不能超过8个！");
    }
    SetView();
}
function setkng(ctr)
{
    var mid = $(ctr).val(); 
    var url = "action.aspx?actp=getknglist&mid=" + mid;
    var rst = ajxpost(url);
    if (rst != "") {
        rst = JSON.parse(rst);
        $('#kng_id').empty();
        $('#kng_id').append("<option value=\"-1\">- 请选择 -</option>");
        for (var i = 0; i < rst.length; i++) {
           $('#kng_id').append("<option value=\"" + rst[i]["id"] + "\">" + rst[i]["content"] + "</option>");
        }
    }
    else {
        $('#kng_id').empty();
        $('#kng_id').append("<option value=\"-1\">- 请选择 -</option>");
        alert("获取知识点失败！\r\n请确定该模块下是否有知识点。");
    }
}
function setkng2(ctr) {
    var mid = $("#ctr_module").val();
    var url = "action.aspx?actp=getknglist&mid=" + mid;
    var rst = ajxpost(url);
    if (rst != "") {
        rst = JSON.parse(rst);
        $('#kng_id').empty();
        $('#kng_id').append("<option value=\"-1\">- 请选择 -</option>");
        for (var i = 0; i < rst.length; i++) {
            $('#kng_id').append("<option value=\"" + rst[i]["id"] + "\">" + rst[i]["content"] + "</option>");
        }
    }
    else {
        $('#kng_id').empty();
        $('#kng_id').append("<option value=\"-1\">- 请选择 -</option>");
    }
}
function setkng3(ctr) {
    var mid = $("#up_ctr_module").val();
    var url = "action.aspx?actp=getknglist&mid=" + mid;
    var rst = ajxpost(url);
    if (rst != "") {
        rst = JSON.parse(rst);
        $('#up_kng_id').empty();
        $('#up_kng_id').append("<option value=\"-1\">- 请选择 -</option>");
        for (var i = 0; i < rst.length; i++) {
            $('#up_kng_id').append("<option value=\"" + rst[i]["id"] + "\">" + rst[i]["content"] + "</option>");
        }
    }
    else {
        $('#up_kng_id').empty();
        $('#up_kng_id').append("<option value=\"-1\">- 请选择 -</option>");
    }
}
function addChoseItems(id,content,isright) {
    var ul_ctr = $("." + id);
    var count = $(ul_ctr).find("li");
    count = count.length;
    count = parseInt(count);
    if (count < 8) {
        var newid = guid();
        var newChoseItem = ChoseItem.replace("{li_th}", "li_" + newid);
        newChoseItem = newChoseItem.replace("{img_th}", "li_" + newid);
        newChoseItem = newChoseItem.replace("{tx_th}", content);
        newChoseItem = newChoseItem.replace("{ck_th}", isright=="true"?"checked =\"checked\"":"");
        $(ul_ctr).append(newChoseItem);
        var ul_ctr = $("." + id);
        var count = $(ul_ctr).find("li");
        count = count.length;
        if (count < 5) {
            $(".quse_ul li img").hide();
        }
        else {
            $(".quse_ul li img").show();
        }
    }
    else {
        alert("试题选项不能超过8个！");
    }
    SetView();
}
function hasScrollbar() {
    return document.body.scrollHeight > (window.innerHeight || document.documentElement.clientHeight);
}
function DeChoseItem(s) {
    var id = $(s).attr("id").split("_")[1];
    $("#li_" + id).remove();
    var ul_ctr = $(".quse_ul");
    var count = $(ul_ctr).find("li");
    count = count.length;
    count = parseInt(count);
    if (count < 5) {
        $(".quse_ul li img").hide();
    }
    else {
        $(".quse_ul li img").show();
    }
    SetView();
}
function AddQues() {
    var tp = $("#quse_type").val();
    var ul_ctr = $(".quse_ul");
    var list = $(ul_ctr).find("li");
    var count = list.length;
    count = parseInt(count);
    var ct_ck = 0;
    var ct_tx = 0;
    var list_str = "[";
    for (var i = 0; i < count; i++) {
        var ck = $(list[i]).find("input");
        var tx = ck[0];
        ck = ck[1];
        var isck = $(ck).is(':checked');
        ct_ck = (isck == true ? (ct_ck += 1) : ct_ck);
        var isnull = $(tx).val();
        isnull = isnull.trim();//去重空格
        isnull = encodeURIComponent(HTMLEnCode(isnull));
        ct_tx = (isnull == "" ? (ct_tx += 1) : ct_tx)
        list_str += "{\"anwser\":\"" + isnull + "\",\"isright\":\"" + isck + "\"},";
    }
    list_str = list_str.substr(0, (list_str.length - 1));
    list_str += "]";
    var mid = $("#ctr_module").val();
    if (mid==-1)
    {
        alert("请选择模块类型!");
        return;
    }
    var kng_id = $("#kng_id").val();
    if (kng_id == -1) {
        alert("请选择知识点!");
        return;
    }
    var content = $("#qcontent").val();
    if (content == "") {
        alert("请输入题干!");
        return;
    }
    if (ct_tx != 0) {
        alert("请输入选项内容!");
        return;
    }
    if (tp == 2 && ct_ck < 2) {
        alert("【多选题】请选择至少2个正确答案!");
        return;
    }
    if (tp == 1 && ct_ck == 0) {
        alert("请选择至少1个正确答案!");
        return;
    }
    if (tp == 1 && ct_ck > 1) {
        alert("【单选题】只能选择一个答案!");
        return;
    }
    content = encodeURIComponent(HTMLEnCode(content));
    list_str = encodeURIComponent(list_str);
    //var json = "{\"qtype\":\""+content+"\",\"qcontent\":\""+content+"\",\"list\":"+list_str+"}";
    var url = "action.aspx?actp=addques&qtype=" + tp + "&tg=" + content + "&list=" + list_str + "&mid=" + mid + "&kngid=" + kng_id + "";
    var rst = ajxpost(url);
    if (rst > 0) {
        alert("试题添加成功!");
        ReLoad();
    }
    else {
        alert("试题添加失败...请重试!");
    }
}
function UpQues() {
    var tp = $("#quse_type").val();
    var ul_ctr = $(".quse_ul");
    var list = $(ul_ctr).find("li");
    var count = list.length;
    count = parseInt(count);
    var ct_ck = 0;
    var ct_tx = 0;
    var list_str = "[";
    for (var i = 0; i < count; i++) {
        var ck = $(list[i]).find("input");
        var tx = ck[0];
        ck = ck[1];
        var isck = $(ck).is(':checked');
        ct_ck = (isck == true ? (ct_ck += 1) : ct_ck);
        var isnull = $(tx).val();
        isnull = isnull.trim();//去重空格
        isnull = encodeURIComponent(HTMLEnCode(isnull));
        ct_tx = (isnull == "" ? (ct_tx += 1) : ct_tx)
        list_str += "{\"anwser\":\"" + isnull + "\",\"isright\":\"" + isck + "\"},";
    }
    list_str = list_str.substr(0, (list_str.length - 1));
    list_str += "]";
    var mid = $("#up_ctr_module").val();
    if (mid == -1) {
        alert("请选择模块类型!");
        return;
    }
    var kng_id = $("#up_kng_id").val();
    if (kng_id == -1) {
        alert("请选择知识点!");
        return;
    }
    var content = $("#qcontent").val();
    if (content == "") {
        alert("请输入题干!");
        return;
    }
    if (ct_tx != 0) {
        alert("请输入选项内容!");
        return;
    }
    if (tp == 2 && ct_ck < 2) {
        alert("【多选题】请选择至少2个正确答案!");
        return;
    }
    if (tp == 1 && ct_ck == 0) {
        alert("请选择至少1个正确答案!");
        return;
    }
    if (tp == 1 && ct_ck > 1) {
        alert("【单选题】只能选择一个答案!");
        return;
    }
    content = encodeURIComponent(HTMLEnCode(content));
    list_str = encodeURIComponent(list_str);
    //var json = "{\"qtype\":\""+content+"\",\"qcontent\":\""+content+"\",\"list\":"+list_str+"}";
    var url = "action.aspx?actp=upques&upid=" + upid + "&qtype=" + tp + "&tg=" + content + "&list=" + list_str + "&mid=" + mid + "&kng_id="+kng_id+"";
    var rst = ajxpost(url);
    if (rst > 0) {
        ReLoad();
    }
    else {
        alert("试题修改失败...请重试!");
    }
}
function DeQues(id)
{
    if (confirm("确定要删除该试题吗!?\r\n删除后该试题不可恢复!"))
    {
        var url = "action.aspx?actp=deques&deid=" + id + "";
        var rst = ajxpost(url);
        if (rst > 0) {
            ReLoad();
        }
        else {
            alert("试题删除失败...请重试!");
        }
    }
}
//设置试题修改显示
function SetUpPaner(id) {
    upid = id;
    $('.MaskPaner').show();//开启遮罩
    var tr = $('#tr_' + id);
    var td_list = $(tr).find("td");
    var questp = $(td_list[0]).attr("data-original");
    var content = $(td_list[1]).text();
    content = content;
    var arr = $(td_list[1]).attr("data-original").split(',');
    var mid = arr[0];
    $("#up_ctr_module").val(mid);
    var kng_id = arr[1];
    setkng3();//异步获取知识点
    $("#up_kng_id").val(kng_id);
    var chose_list = $(td_list[2]).attr("data-original");
    chose_list = decodeURIComponent(chose_list);
    //chose_list = HTMLDeCode(chose_list);
    chose_list = JSON.parse(chose_list);
    //先清空所有
    $(".quse_ul").find("li").remove();
    for (var i = 0; i < chose_list.length; i++) {
        addChoseItems("quse_ul", chose_list[i]["anwser"], chose_list[i]["isright"]);
    }
    $('#quse_type').val(questp);
    $('#qcontent').val(content);
    $(".Edit_paner").show();
    //这里必须加
    SetView();
}
function setQtypeSearch()
{
    var tp = $("#ipt_tx").val();
    tp = tp == "0" ? "" : "&tp=" + tp + "";
    var scontetn = $("#ipt_tg").val();
    scontetn = scontetn == "" ? "" : "&tg=" + encodeURIComponent(scontetn) + "";
    var mid = $('#ctr_module').val() == "-1" ? "" : "&mid=" + $('#ctr_module').val();
    var kngid = $('#kng_id').val() == "-1" ? "" : "&kng_id=" + $('#kng_id').val();
    var url = "questionlist.aspx?cx=yes" + tp + scontetn + mid + kngid;
    window.location.href = url;
}
/*知识点模块*/
var knowledgeItem = "<li id=\"{li_th}\"><input type=\"text\" value=\"{tx_th}\"/><img id=\"{img_th}\" src=\"img/yc.png\" alt=\"\"/ onclick=\"Deknowledge(this);\"></li>";
function addknowledgeItem(id) {
    var ul_ctr = $("." + id);
    var count = $(ul_ctr).find("li");
    count = count.length;
    count = parseInt(count);
    if (count < 10) {
        var newid = guid();
        var newChoseItem = knowledgeItem.replace("{li_th}", "li_" + newid);
        newChoseItem = newChoseItem.replace("{img_th}", "li_" + newid);
        newChoseItem = newChoseItem.replace("{tx_th}", "");
        newChoseItem = newChoseItem.replace("{ck_th}", "");
        $(ul_ctr).append(newChoseItem);
        var ul_ctr = $("." + id);
        var count = $(ul_ctr).find("li");
        count = count.length;
        if (count < 5) {
            $(".knowledge_ul li img").hide();
        }
        else {
            $(".knowledge_ul li img").show();
        }
    }
    else {
        alert("知识点录入一次最多不能超过10个！");
    }
    SetView();
}
function Deknowledge(s) {
    var id = $(s).attr("id").split("_")[1];
    $("#li_" + id).remove();
    var ul_ctr = $(".knowledge_ul");
    var count = $(ul_ctr).find("li");
    count = count.length;
    count = parseInt(count);
    if (count < 5) {
        $(".knowledge_ul li img").hide();
    }
    else {
        $(".knowledge_ul li img").show();
    }
    SetView();
}
function Addknowledge() {
    var mid = $("#quse_type").val();
    var mstr = $("#quse_type option:selected").text();
    mstr = encodeURIComponent(HTMLEnCode(mstr));
    var ul_ctr = $(".knowledge_ul");
    var list = $(ul_ctr).find("li");
    var count = list.length;
    count = parseInt(count);
    var ct_tx = 0;
    var list_str = "[";
    for (var i = 0; i < count; i++) {
        var tx = $(list[i]).find("input");
        var isnull = $(tx).val();
        isnull = isnull.trim();//去重空格
        isnull = encodeURIComponent(HTMLEnCode(isnull));
        ct_tx = (isnull == "" ? (ct_tx += 1) : ct_tx)
        list_str += "{\"content\":\"" + isnull + "\"},";
    }
    list_str = list_str.substr(0, (list_str.length - 1));
    list_str += "]";
    if (ct_tx != 0) {
        alert("请输入选项内容!");
        return;
    }
    list_str = encodeURIComponent(list_str);
    var url = "action.aspx?actp=addknowledge&mid=" + mid + "&mstr=" + mstr + "&list=" + list_str + "";
    var rst = ajxpost(url);
    if (rst > 0) {
        alert("知识点添加成功!");
        ReLoad();
    }
    else {
        alert("知识点添加失败...请重试!");
    }
}
function SetKlGUpPaner(id)
{
    upid = id;
    $('.MaskPaner').show();//开启遮罩
    var tr = $('#tr_' + id);
    var td_list = $(tr).find("td");
    var mid = $(td_list[0]).attr("data-original");
    var content = $(td_list[1]).text();
    content = decodeURIComponent(HTMLDeCode(content));
    $('#quse_type').val(mid);
    $('#qcontent').val(content);
    $(".Edit_paner").show();
    //这里必须加
    SetView();
}
function setKNGtypeSearch()
{
    var mid = $("#ipt_tx").val();
    mid = mid == "0" ? "" : "&mid=" + mid + "";
    var scontetn = $("#ipt_tg").val();
    scontetn = scontetn == "" ? "" : "&zsd=" + encodeURIComponent(HTMLEnCode(scontetn)) + "";
    var url = "knowledgepoint.aspx?cx=yes" + mid + scontetn;
    window.location.href = url;
}
function DeKLG(id)
{
    if (confirm("确定要删除该知识点吗!?\r\n删除后不可恢复!")) {
        var url = "action.aspx?actp=deklg&deid=" + id + "";
        var rst = ajxpost(url);
        if (rst > 0) {
            ReLoad();
        }
        else {
            alert("知识点删除失败...请重试!");
        }
    }
}
function UpKLG()
{
    var mid = $("#quse_type").val();
    var mstr = $("#quse_type option:selected").text()
    mstr = mstr.split(" ")[0];
    mstr = encodeURIComponent(HTMLEnCode(mstr));
    var content = $("#qcontent").val();
    if (content == "") {
        alert("请输入知识点!");
        return;
    }
    content = encodeURIComponent(HTMLEnCode(content));
    //var json = "{\"qtype\":\""+content+"\",\"qcontent\":\""+content+"\",\"list\":"+list_str+"}";
    var url = "action.aspx?actp=upklg&upid=" + upid + "&mid=" + mid + "&mstr=" + mstr + "&content=" + content + "";
    var rst = ajxpost(url);
    if (rst > 0) {
        ReLoad();
    }
    else {
        alert("试题修改失败...请重试!");
    }
}
/*配置知识点*/
var kng_str = "<div class=\"ChoiceItem\" style=\"height:26px;line-height:26px;\"><div class=\"flt\" id=\"kng_{th_id}\" style=\"margin-left:4px; width: 366px;\">{th_val}</div><div class=\"flt\" id=\"choise_paner\"><div>判断题[ <input id=\"ipt_pd\" data-original=\"{th_pdsum}\" type=\"text\" value=\"{th_pdval}\"/>/{th_pdsum} ]</div><div>多选题[ <input id=\"ipt_ddx\" data-original=\"{th_ddxsum}\" type=\"text\" value=\"{th_ddxval}\"/>/{th_ddxsum} ]</div><div>单选题[ <input id=\"ipt_dx\" data-original=\"{th_dxsum}\" type=\"text\" value=\"{th_dxval}\"/>/{th_dxsum} ]</div></div></div>";
function setmodulekng(mid)
{
    upid = mid;
    $('.MaskPaner').show();//开启遮罩
    var url = "action.aspx?actp=getknglist_pro&mid=" + mid;
    var rst = ajxpost(url);
    $('#kng_paner').empty();
    //获取jsonData
    var jsonData = $("#tr_" + mid).find("td");
    jsonData = $(jsonData[0]).attr("data-original");
    var kng_list;
    if (jsonData!="")
    {
        jsonData = decodeURIComponent(jsonData);
        jsonData = jsonData.replaceAll("\"\"", "\"");
        kng_list = JSON.parse(jsonData);
    }
    if (rst != "") {
        rst = JSON.parse(rst);
        for (var i = 0; i < rst.length; i++) {
           var new_kng_str = kng_str.replace("{th_id}", rst[i]["id"]);
           new_kng_str = new_kng_str.replace("{th_val}", rst[i]["content"]);
           new_kng_str = new_kng_str.replaceAll("{th_dxsum}", rst[i]["dx"]);
           new_kng_str = new_kng_str.replaceAll("{th_ddxsum}", rst[i]["ddx"]);
           new_kng_str = new_kng_str.replaceAll("{th_pdsum}", rst[i]["pd"]);
            //设置数据值
           var dx_val = jsonData != "" ? kng_list[i]["choices"][0]["dx"]:"0";
           var ddx_val = jsonData != "" ? kng_list[i]["choices"][1]["ddx"]:"0";
           var pd_val = jsonData != "" ? kng_list[i]["choices"][2]["pd"]:"0";

           new_kng_str = new_kng_str.replaceAll("{th_dxval}", dx_val);
           new_kng_str = new_kng_str.replaceAll("{th_ddxval}", ddx_val);
           new_kng_str = new_kng_str.replaceAll("{th_pdval}", pd_val);
           $('#kng_paner').append(new_kng_str);
        }
    }
      

    var sum = 100;
    var tmp_sum = 0;
    $(".Edit_paner").show();
    $("#kng_paner").find("input").change(function () {
        var dx_num = 0;
        var ddx_num = 0;
        var pd_num = 0;
        var ctr = $(this);
        $(ctr).css("border-bottom", "1px solid #919191");
        cc = $(ctr).val();
        cc = cc == "" ? "0" : cc;
        cc = parseInt(cc);
        maxcc = $(ctr).attr("data-original");
        maxcc = parseInt(maxcc);
        if (isNaN(cc))
        {
            alert("请输入数字");
            $(ctr).css("border-bottom", "1px solid red");
            return;
        }
        if (cc > maxcc)
        {
            alert("配置的题型数量不能超过存在的试题数量!");
            $(ctr).css("border-bottom", "1px solid red");
            return;
        }
        if (cc > sum)
        {
            alert("试题数量不能大于" + sum + "");
            return;
        }
        $("#kng_paner").find("input").each(function () {
            var tp = $(this).attr("id");
            var val = $(this).val();
            val = val == "" ? "0" : val;
            switch (tp) {
                case "ipt_dx":
                    dx_num += parseInt(val);
                    break;
                case "ipt_ddx":
                    ddx_num += parseInt(val);
                    break;
                case "ipt_pd":
                    pd_num += parseInt(val);
                    break;
            }
        });
        tmp_sum = dx_num + ddx_num + pd_num;
        if (tmp_sum > sum) {
            alert("试题数量不能大于" + sum + "");
            return;
        }
        $("#kng_tip").html("已设置[ 单选题:" + dx_num + "道,多选题:" + ddx_num + "道,判断题:" + pd_num + "道 ] / " + (sum - tmp_sum) + "");
    });
    //这里必须加
    SetView();
}
var md_info_str = "{\"kng_id\":\"{th_id}\",\"choices\":[{\"dx\":\"{th_dx}\"},{\"ddx\":\"{th_ddx}\"},{\"pd\":\"{th_pd}\"}]}";
function upModuleInfo()
{
    var dx_num = 0;
    var ddx_num = 0;
    var pd_num = 0;
    var hasErr = false;
    $("#kng_paner").find("input").each(function () {
        var ctr = $(this);
        $(ctr).css("border-bottom", "1px solid #919191");
        var tp = $(ctr).attr("id");
        var val = $(ctr).val();
        val = val == "" ? "0" : val;
        val = parseInt(val);
        var maxcc = $(ctr).attr("data-original");
        maxcc = parseInt(maxcc);
        if (isNaN(val)) {
            alert("请输入数字!");
            $(this).css("border-bottom", "1px solid red");
            hasErr = true;
            return false;
        }
        if (val > maxcc) {
            alert("配置的题型数量不能超过存在的试题数量!");
            $(this).css("border-bottom", "1px solid red");
            hasErr = true;
            return false;
        }
        switch (tp) {
            case "ipt_dx":
                dx_num += parseInt(val);
                break;
            case "ipt_ddx":
                ddx_num += parseInt(val);
                break;
            case "ipt_pd":
                pd_num += parseInt(val);
                break;
        }
    });
    if ((dx_num + ddx_num + pd_num)>100)
    {
        alert("配置试题数量不能超过100道");
        return;
    }
    if (!hasErr)//没错时执行入库操作
    {
        var JsonList = "[";
        $("#kng_paner .ChoiceItem").each(function () {
            var id = $(this).children("div").attr("id");
            id = id.replace("kng_", "");
            var dx_ct = 0;
            var ddx_ct = 0;
            var pd_ct = 0;
            $(this).find("input").each(function () {
                var tp = $(this).attr("id");
                var val = $(this).val();
                val = isNaN(val) ? "0" : (val == "" ? "0" : val);
                switch (tp) {
                    case "ipt_dx":
                        dx_ct = parseInt(val);
                        break;
                    case "ipt_ddx":
                        ddx_ct = parseInt(val);
                        break;
                    case "ipt_pd":
                        pd_ct = parseInt(val);
                        break;
                }
            });
            var New_md_info_str = md_info_str.replace("{th_id}", id);
            New_md_info_str = New_md_info_str.replace("{th_dx}", dx_ct);
            New_md_info_str = New_md_info_str.replace("{th_ddx}", ddx_ct);
            New_md_info_str = New_md_info_str.replace("{th_pd}", pd_ct);
            JsonList += New_md_info_str + ",";
        });
        JsonList = JsonList.substr(0, (JsonList.length - 1));
        JsonList += "]";
        JsonList = encodeURIComponent(JsonList);
        var url = "action.aspx?actp=upmdlist&upid=" + upid + "&nkg_list=" + JsonList + "";
        var rst = ajxpost(url);
        if (rst > 0) {
            ReLoad();
        }
        else {
            alert("保存知识点失败!");
        }
    }
   
}
/*帮助函数*/
String.prototype.replaceAll = function (FindText, RepText) {
    let regExp = new RegExp(FindText, 'g');
    return this.replace(regExp, RepText);
}; 
function ajxpost(url) {
    var ajxurl = url;
    var htmlobj = $.ajax({ url: ajxurl, contentType: "application/x-www-form-urlencoded; charset=utf-8", async: false });
    var rst = htmlobj.responseText.replace(" ", "");
    rst = rst.replace("\r\n", "");
    rst = rst.replace("\r\n", "");
    return rst;
}
function guid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}
function ReLoad() {
    location.reload();
}
function gourl(url) {
    window.location.href = url;
}
function SearchGoPage(pagename) {
    var cs = $('#tzym_t').val();
    if (cs != "") {
        var host = window.location.host;
        var url = "http://" + host + "/" + (pagename + (pagename.indexOf("?cx")>=0?"&page=":"?page=") + cs);
        gourl(url);
    }
    else {
        alert("请输入跳转页码!");
    }

}
function GetUrlName() {
    var url = window.location.href;
    var loc = url.substring(url.lastIndexOf('/') + 1, url.length);
    var loc = loc.indexOf("?") == -1 ? loc : loc.substring(0, loc.indexOf("?"));
    return loc.replace(".aspx", "");
}
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = "";
    var bb = isIE();
    if (bb) {
        r = window.location.search;
    }
    else {
        r = encodeURI(window.location.search);
    }
    r = r.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
function isIE() { //ie?
    if (!!window.ActiveXObject || "ActiveXObject" in window)
        return true;
    else
        return false;
}
function SetLiColor()
{
    var name = GetUrlName();
    var ctr = $("#" + name);
    $(ctr).addClass("liHover");
    $(".nav_right_title span").text("当前操作: "+$(ctr).text()); 
}
//关闭弹窗
function CloseMaskPaner() {
    $(".MaskPaner").hide();
    $(".Edit_paner").hide();
}
function Trim(str) {
    return str.replace(/(^\s*)|(\s*$)/g, "");
}
function HTMLEnCode(str) {
    var s = "";
    if (str.length == 0) return "";
    s = str.replace(/&/g, "&gt;");
    s = s.replace(/</g, "&lt;");
    s = s.replace(/>/g, "&gt;");
    s = s.replace(/ /g, "&nbsp;");
    s = s.replace(/    /g, "&nbsp;");
    s = s.replace(/\'/g, "'");
    s = s.replace(/\"/g, "&quot;");
    s = s.replace(/\n/g, "<br>");
    return s;
}
function HTMLDeCode(str) {
    var s = "";
    if (str.length == 0) return "";
    s = str.replace(/&gt;/g, "&");
    s = s.replace(/&lt;/g, "<");
    s = s.replace(/&gt;/g, ">");
    s = s.replace(/&nbsp;/g, " ");
    s = s.replace(/'/g, "\'");
    s = s.replace(/&quot;/g, "\"");
    s = s.replace(/<br>/g, "\n");
    return s;
}  