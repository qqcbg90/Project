
var _fileUploadIsSend = false;
//載入頁面統一要做的事
$(function ()
{

})

$(window).resize(function () {
    
});

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
//縮圖
function DrawImage(ImgD, FitWidth, FitHeight) {
	var image = new Image();
	image.src = ImgD.src;
	if (image.width > 0 && image.height > 0) {
		if (image.width / image.height >= FitWidth / FitHeight) {
			if (image.width > FitWidth) {
				ImgD.width = FitWidth;
				ImgD.height = (image.height * FitWidth) / image.width;
			} else {
				ImgD.width = image.width;
				ImgD.height = image.height;
			}
		} else {
			if (image.height > FitHeight) {
				ImgD.height = FitHeight;
				ImgD.width = (image.width * FitHeight) / image.height;
			} else {
				ImgD.width = image.width;
				ImgD.height = image.height;
			}
		}
	}

}
//檢查上下線日期
function CheckDateUpDown(date1, date2)
{
    if ($.datepicker.parseDate("yy/mm/dd", date1) >
                $.datepicker.parseDate("yy/mm/dd", date2)) {
        alert("日期範圍輸入錯誤！！");
        return false;
    }
}

//移除id
function Remove(id) {
	$("#" + id).remove();
}
//關閉 BlockUI
function CloseBlockUI() {
	$.unblockUI();
}
//取得網址#後的部分
function UrlReplace(url) {
	return url.replace(/^.*#/, '');
}

//給FileUpload 用的送出
function StartSendSubmitByFileUpload(sec) {
    sec = sec || 1000;
    var _canSend = true;
    $("div.ajax-file-upload-container").each(function () {
        if ($(this).find('div.ajax-file-upload-statusbar').length > 0) {
            _canSend = false;
            return false;
        }
    });
    if (_canSend) {
        if (!_fileUploadIsSend) {
            _fileUploadIsSend = true;
            SendSubmit();//基本上會有2個form 所以要送第1個
        }
    }
    else {
        setInterval("StartSendSubmitByFileUpload()", sec);
    }
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

//取得int
function GetInt(value) {
    return parseInt(value) || 0;
}

//取得float
function GetFloat(value) {
    return parseFloat(value) || 0;
}

//計算小數
function GetFormatFloat(num, pos) {
    var size = Math.pow(10, pos);
    return Math.round(num * size) / size;
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

function GetGuid() {
    var d = Date.now();
    if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
        d += performance.now(); //use high-precision timer if available
    }
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}


if (!Array.indexOf) {
	Array.prototype.indexOf = function (obj) {
		for (var i = 0; i < this.length; i++) {
			if (this[i] == obj) {
				return i;
			}
		}
		return -1;
	}
}

// https://tc39.github.io/ecma262/#sec-array.prototype.findIndex
if (!Array.prototype.findIndex) {
	Object.defineProperty(Array.prototype, 'findIndex', {
		value: function (predicate) {
			// 1. Let O be ? ToObject(this value).
			if (this == null) {
				throw new TypeError('"this" is null or not defined');
			}

			var o = Object(this);

			// 2. Let len be ? ToLength(? Get(O, "length")).
			var len = o.length >>> 0;

			// 3. If IsCallable(predicate) is false, throw a TypeError exception.
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}

			// 4. If thisArg was supplied, let T be thisArg; else let T be undefined.
			var thisArg = arguments[1];

			// 5. Let k be 0.
			var k = 0;

			// 6. Repeat, while k < len
			while (k < len) {
				// a. Let Pk be ! ToString(k).
				// b. Let kValue be ? Get(O, Pk).
				// c. Let testResult be ToBoolean(? Call(predicate, T, « kValue, k, O »)).
				// d. If testResult is true, return k.
				var kValue = o[k];
				if (predicate.call(thisArg, kValue, k, o)) {
					return k;
				}
				// e. Increase k by 1.
				k++;
			}

			// 7. Return -1.
			return -1;
		}
	});
}

// https://tc39.github.io/ecma262/#sec-array.prototype.find
if (!Array.prototype.find) {
	Object.defineProperty(Array.prototype, 'find', {
		value: function (predicate) {
			// 1. Let O be ? ToObject(this value).
			if (this == null) {
				throw new TypeError('"this" is null or not defined');
			}

			var o = Object(this);

			// 2. Let len be ? ToLength(? Get(O, "length")).
			var len = o.length >>> 0;

			// 3. If IsCallable(predicate) is false, throw a TypeError exception.
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}

			// 4. If thisArg was supplied, let T be thisArg; else let T be undefined.
			var thisArg = arguments[1];

			// 5. Let k be 0.
			var k = 0;

			// 6. Repeat, while k < len
			while (k < len) {
				// a. Let Pk be ! ToString(k).
				// b. Let kValue be ? Get(O, Pk).
				// c. Let testResult be ToBoolean(? Call(predicate, T, « kValue, k, O »)).
				// d. If testResult is true, return kValue.
				var kValue = o[k];
				if (predicate.call(thisArg, kValue, k, o)) {
					return kValue;
				}
				// e. Increase k by 1.
				k++;
			}

			// 7. Return undefined.
			return undefined;
		}
	});
}