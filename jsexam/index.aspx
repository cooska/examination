<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="jsexam.index" %>
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <meta name="language" content="zh-CN">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,Chrome=1"> 
<title>湘西州专业技术人员公需科目考试系统</title>
    <meta name="Keywords" content="湘西州专业技术人员公需科目考试系统"/>
    <meta name="Description" content="湘西州专业技术人员公需科目考试系统"/>
	<link rel="stylesheet" type="text/css" href="css/p_css.css">
    <script src="js/jquery.min.js"></script>
    <script src="js/help.js"></script>
</head>
<body style="overflow:hidden">
<div class="loginpaner">
<div class="cb"></div>
<div class="login_title">湘西州专业技术人员公需科目考试系统</div>
<div class="login_tag">请输入账号,密码登录</div>
<div class="login_input" style="margin-top: 40px">
    <img src="img/uslogin.png" height="32" width="32" class="flt"/>
    <input id="usname" type="text" value="" class="flt"/>
</div>
<div class="login_input" style="margin-top: 20px">
    <img src="img/uspass.png" height="32" width="32" class="flt"/>
    <input id="uspass" type="password" value="" class="flt"/>
</div>
<div class="cb"></div>
<div style="text-align: center; border: 2px solid white;height: 42px;line-height: 42px;width: 100px;cursor: pointer;margin-left: 60px;margin-top: 40px" onclick="usLogin();">登录</div>
</div>
    <script>
        document.onkeydown = function () {
            if (event.keyCode == 13) {
                usLogin();
            }
        }
    </script>
</body>

</html>
