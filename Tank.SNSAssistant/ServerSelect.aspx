<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerSelect.aspx.cs" Inherits="Tank.SNSAssistant.ServerSelect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>弹弹堂 - 频道选择</title>
    <meta
    <link href="images/style.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        function selectServerClick(link) 
        {
            if(link == null)
            {
                return;
            }
            var curUrl = document.location.href;
            var params = curUrl.split("?")[1];
            var url = link + "?" + params;
            document.location.href = url;
        }
    </script>
</head>
<body>
<table width="1000" border="0" align="center" cellpadding="0" cellspacing="0">
  <tr>
    <td height="600" align="center" valign="top" background="images/bg3.jpg">
    	<div class="top"></div>
        <div class="menu_button">
        	<ul>
           	  <li class="menu_1"><a href="http://hi.baidu.com/%B0%D9%B6%C8%B5%AF%B5%AF%CC%C3/blog/category/%D3%CE%CF%B7%BD%E9%C9%DC" target="_blank" title="游戏资料"></a></li>
                <li class="menu_2"><a href="http://hi.baidu.com/百度弹弹堂" target="_blank" title="进入官网"></a></li>
                <li class="menu_3"><a href="http://tieba.baidu.com/f?kw=%B5%AF%B5%AF%CC%C3" target="_blank" title="游戏论坛"></a></li>
            </ul>
        </div>
        <br clear="all" />
        <div class="sev_list">
        	<div class="sev_menu">
            	<ul>
            	    <li class="sev_2"><a href="#" target="_self" onclick="selectServerClick('http://assist2.baiduddt.cn/LoginTransfer.aspx')" title="点击进去百度二区"></a></li>
                    <li class="sev_1"><a href="#" target="_self" onclick="selectServerClick('http://assist1.baiduddt.cn/LoginTransfer.aspx')" title="点击进去百度一区"></a></li>
                </ul>
            </div>
        </div>
    </td>
  </tr>
</table>
</body>
</html>