



//變更預設分頁筆數
function InputGo(page) {
    $('div.pager').find('input[type=text]').each(function () {
		$(this).val(page);
	});
}
//送出變更分頁筆數
function ChangeDefaultPage() {
	SendSubmit();
}
//轉至url
function GoUrl(url) {
	location.href = url;
}
//轉至url open
function GoUrlOpen(url) {
    window.open(url, '_blank');
}

//cehck url replace &符號
function CheckUrl(url) {
    url = url.replace(/&amp;/g, '&');
    if (url.indexOf('?') > -1) {
        url += "&";
    }
    else {
        url += "?";
    }
    return url;
}
//送出第一個form or 指定
function SendSubmit(index) {
    index = index || 0;
    $("form")[index].submit();
}

//回上一頁
function back() {
    if (1 < history.length) {
        history.back();
        return false;
    }
    return true;
}

//對urlParameter編碼
function EncodeParameter(value) {
	return encodeURIComponent(value);
}


//移除id
function Remove(id) {
	$("#" + id).remove();
}
//關閉 BlockUI
function CloseBlockUI() {
	$.unblockUI();
}

//檢查cookie是否開啟
function CheckCookie() {
	$.cookie('checkCookie', 'test', { path: '/' });
	if ($.cookie('checkCookie') == null) {
		alert('已關閉瀏覽器Cookie功能，部分網站功能無法正常使用!!');
	}
	else {
		$.cookie('checkCookie', null);
	}
}

//show提示對話框
//msg 要提示的訊息
//msgType 1=success ,2=notice ,3=warning ,4=error 
function showMsg(msg, msgType) {
	switch (msgType) {
		case 1:
			msgType="success"
			break;
		case 2:
			msgType = "notice"
			break;
		case 3:
			msgType = "warning"
			break;
		case 4:
			msgType = "error"
			break;
		default:
			msgType = "notice"
			break;

	}
	$().toastmessage('showToast', {
		text: msg,
		stayTime: 3000,
		sticky: false,
		position: 'middle-center',//訊息位置：1.top-left  2.top-center 3.top-right 4.middle-left 5.middle-center 6.middle-right
		type: msgType, //success ,notice ,warning ,error 
		closeText: '',
		close: function () {

		}
	});
}
