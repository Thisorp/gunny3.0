<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Confirm.aspx.cs" Inherits="Tank.SNSAssistant.Confirm" codepage="936" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=gbk" />
    <title>Untitled Page</title>
 <script language="javascript" type="text/javascript"> 
//js获取url参数的function
function request(paras){ 
    var url = location.href;   
    var paraString = url.substring(url.indexOf("?")+1,url.length).split("&");   
    var paraObj = {}   
    for (i=0; j=paraString[i]; i++){   
    paraObj[j.substring(0,j.indexOf("=")).toLowerCase()] = j.substring(j.indexOf 
    ("=")+1,j.length);   
    }
    var returnValue = paraObj[paras.toLowerCase()];   
    if(typeof(returnValue)=="undefined"){   
    return "";   
    }else{   
    return returnValue;   
    }   
} 
function InitPara()
{
    document.getElementById('order_number').value = request("order");
    document.getElementById('amount_number').value = request("amount");
    document.getElementById('sig_code').value =  request("sig");
    document.getElementById('order_msg').value =  "点券";
    document.getElementById('getForm').submit();
}

</script>

</head>
<body onload="InitPara()"  >

    <div>
<form action="http://apps.hi.baidu.com/pay/confirm" method="post" id="getForm" target="_top" accept-charset="gbk" onsubmit="document.charset='gbk'" > 
	<input type="hidden" name="app_id" value="10020" />
	<input type="hidden" name="order" id="order_number" value="" />
	<input type="hidden" name="amount" id="amount_number" value="" />
	<input type="hidden" size="50" name="sig" id="sig_code" value="" />
	<input type="hidden" size="50" name="order_msg" id="order_msg" value="" />
	</form>
    </div>
</body>
</html>
