/* Chinese initialisation for the jQuery UI date picker plugin. */
/* Written by Ressol (ressol@gmail.com). */
jQuery(function ($) {
	$.datepicker.regional['zh-TW'] = {
		closeText: '關閉',
		prevText: '&#x3c;上月',
		nextText: '下月&#x3e;',
		currentText: '今天',
		monthNames: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
		monthNamesShort: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
		dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
		dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
		dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
		weekHeader: '周',
		dateFormat: 'yy/mm/dd',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: true,
		yearSuffix: '年'
	};
	$.datepicker.setDefaults($.datepicker.regional['zh-TW']);
});

/*
jQuery Url Plugin
* Version 1.0
* 2009-03-22 19:30:05
* URL: http://ajaxcssblog.com/jquery/url-read-get-variables/
* Description: jQuery Url Plugin gives the ability to read GET parameters from the actual URL
* Author: Matthias Jaggli
* Copyright: Copyright (c) 2009 Matthias Jaggli 
* Licence: dual, MIT/GPLv2
*/
(function ($) { $.url = {}; $.extend($.url, { _params: {}, init: function () { var paramsRaw = ""; try { paramsRaw = (document.location.href.split("?", 2)[1] || "").split("#")[0].split("&") || []; for (var i = 0; i < paramsRaw.length; i++) { var single = paramsRaw[i].split("="); if (single[0]) this._params[single[0]] = unescape(single[1]); } } catch (e) { alert(e); } }, param: function (name) { return this._params[name] || ""; }, paramAll: function () { return this._params; } }); $.url.init(); })(jQuery);

/**
* Cookie plugin
*
* Copyright (c) 2006 Klaus Hartl (stilbuero.de)
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
*
*/

/**
* Create a cookie with the given name and value and other optional parameters.
*
* @example $.cookie('the_cookie', 'the_value');
* @desc Set the value of a cookie.
* @example $.cookie('the_cookie', 'the_value', {expires: 7, path: '/', domain: 'jquery.com', secure: true});
* @desc Create a cookie with all available options.
* @example $.cookie('the_cookie', 'the_value');
* @desc Create a session cookie.
* @example $.cookie('the_cookie', null);
* @desc Delete a cookie by passing null as value.
*
* @param String name The name of the cookie.
* @param String value The value of the cookie.
* @param Object options An object literal containing key/value pairs to provide optional cookie attributes.
* @option Number|Date expires Either an integer specifying the expiration date from now on in days or a Date object.
*                             If a negative value is specified (e.g. a date in the past), the cookie will be deleted.
*                             If set to null or omitted, the cookie will be a session cookie and will not be retained
*                             when the the browser exits.
* @option String path The value of the path atribute of the cookie (default: path of page that created the cookie).
* @option String domain The value of the domain attribute of the cookie (default: domain of page that created the cookie).
* @option Boolean secure If true, the secure attribute of the cookie will be set and the cookie transmission will
*                        require a secure protocol (like HTTPS).
* @type undefined
*
* @name $.cookie
* @cat Plugins/Cookie
* @author Klaus Hartl/klaus.hartl@stilbuero.de
*/

/**
* Get the value of a cookie with the given name.
*
* @example $.cookie('the_cookie');
* @desc Get the value of a cookie.
*
* @param String name The name of the cookie.
* @return The value of the cookie.
* @type String
*
* @name $.cookie
* @cat Plugins/Cookie
* @author Klaus Hartl/klaus.hartl@stilbuero.de
*/
jQuery.cookie = function (name, value, options) {
	if (typeof value != 'undefined') { // name and value given, set cookie
		options = options || {};
		if (value === null) {
			value = '';
			options.expires = -1;
		}
		var expires = '';
		if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
			var date;
			if (typeof options.expires == 'number') {
				date = new Date();
				date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
			} else {
				date = options.expires;
			}
			expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
		}
		var path = options.path ? '; path=' + options.path : '';
		var domain = options.domain ? '; domain=' + options.domain : '';
		var secure = options.secure ? '; secure' : '';
		document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
	} else { // only name given, get cookie
		var cookieValue = null;
		if (document.cookie && document.cookie != '') {
			var cookies = document.cookie.split(';');
			for (var i = 0; i < cookies.length; i++) {
				var cookie = jQuery.trim(cookies[i]);
				// Does this cookie string begin with the name we want?
				if (cookie.substring(0, name.length + 1) == (name + '=')) {
					cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
					break;
				}
			}
		}
		return cookieValue;
	}
};

/*!
 * jQuery blockUI plugin
 * Version 2.66.0-2013.10.09
 * Requires jQuery v1.7 or later
 *
 * Examples at: http://malsup.com/jquery/block/
 * Copyright (c) 2007-2013 M. Alsup
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 *
 * Thanks to Amir-Hossein Sobhi for some excellent contributions!
 */

; (function () {
	/*jshint eqeqeq:false curly:false latedef:false */
	"use strict";

	function setup($) {
		$.fn._fadeIn = $.fn.fadeIn;

		var noOp = $.noop || function () { };

		// this bit is to ensure we don't call setExpression when we shouldn't (with extra muscle to handle
		// confusing userAgent strings on Vista)
		var msie = /MSIE/.test(navigator.userAgent);
		var ie6 = /MSIE 6.0/.test(navigator.userAgent) && ! /MSIE 8.0/.test(navigator.userAgent);
		var mode = document.documentMode || 0;
		var setExpr = $.isFunction(document.createElement('div').style.setExpression);

		// global $ methods for blocking/unblocking the entire page
		$.blockUI = function (opts) { install(window, opts); };
		$.unblockUI = function (opts) { remove(window, opts); };

		// convenience method for quick growl-like notifications  (http://www.google.com/search?q=growl)
		$.growlUI = function (title, message, timeout, onClose) {
			var $m = $('<div class="growlUI"></div>');
			if (title) $m.append('<h1>' + title + '</h1>');
			if (message) $m.append('<h2>' + message + '</h2>');
			if (timeout === undefined) timeout = 3000;

			// Added by konapun: Set timeout to 30 seconds if this growl is moused over, like normal toast notifications
			var callBlock = function (opts) {
				opts = opts || {};

				$.blockUI({
					message: $m,
					fadeIn: typeof opts.fadeIn !== 'undefined' ? opts.fadeIn : 700,
					fadeOut: typeof opts.fadeOut !== 'undefined' ? opts.fadeOut : 1000,
					timeout: typeof opts.timeout !== 'undefined' ? opts.timeout : timeout,
					centerY: false,
					showOverlay: false,
					onUnblock: onClose,
					css: $.blockUI.defaults.growlCSS
				});
			};

			callBlock();
			var nonmousedOpacity = $m.css('opacity');
			$m.mouseover(function () {
				callBlock({
					fadeIn: 0,
					timeout: 30000
				});

				var displayBlock = $('.blockMsg');
				displayBlock.stop(); // cancel fadeout if it has started
				displayBlock.fadeTo(300, 1); // make it easier to read the message by removing transparency
			}).mouseout(function () {
				$('.blockMsg').fadeOut(1000);
			});
			// End konapun additions
		};

		// plugin method for blocking element content
		$.fn.block = function (opts) {
			if (this[0] === window) {
				$.blockUI(opts);
				return this;
			}
			var fullOpts = $.extend({}, $.blockUI.defaults, opts || {});
			this.each(function () {
				var $el = $(this);
				if (fullOpts.ignoreIfBlocked && $el.data('blockUI.isBlocked'))
					return;
				$el.unblock({ fadeOut: 0 });
			});

			return this.each(function () {
				if ($.css(this, 'position') == 'static') {
					this.style.position = 'relative';
					$(this).data('blockUI.static', true);
				}
				this.style.zoom = 1; // force 'hasLayout' in ie
				install(this, opts);
			});
		};

		// plugin method for unblocking element content
		$.fn.unblock = function (opts) {
			if (this[0] === window) {
				$.unblockUI(opts);
				return this;
			}
			return this.each(function () {
				remove(this, opts);
			});
		};

		$.blockUI.version = 2.66; // 2nd generation blocking at no extra cost!

		// override these in your code to change the default behavior and style
		$.blockUI.defaults = {
			// message displayed when blocking (use null for no message)
			message: '<h1>Please wait...</h1>',

			title: null,		// title string; only used when theme == true
			draggable: true,	// only used when theme == true (requires jquery-ui.js to be loaded)

			theme: false, // set to true to use with jQuery UI themes

			// styles for the message when blocking; if you wish to disable
			// these and use an external stylesheet then do this in your code:
			// $.blockUI.defaults.css = {};
			css: {
				padding: 0,
				margin: 0,
				width: '30%',
				top: '40%',
				left: '35%',
				textAlign: 'center',
				color: '#000',
				border: '3px solid #aaa',
				backgroundColor: '#fff',
				cursor: 'wait'
			},

			// minimal style set used when themes are used
			themedCSS: {
				width: '30%',
				top: '40%',
				left: '35%'
			},

			// styles for the overlay
			overlayCSS: {
				backgroundColor: '#000',
				opacity: 0.6,
				cursor: 'wait'
			},

			// style to replace wait cursor before unblocking to correct issue
			// of lingering wait cursor
			cursorReset: 'default',

			// styles applied when using $.growlUI
			growlCSS: {
				width: '350px',
				top: '10px',
				left: '',
				right: '10px',
				border: 'none',
				padding: '5px',
				opacity: 0.6,
				cursor: 'default',
				color: '#fff',
				backgroundColor: '#000',
				'-webkit-border-radius': '10px',
				'-moz-border-radius': '10px',
				'border-radius': '10px'
			},

			// IE issues: 'about:blank' fails on HTTPS and javascript:false is s-l-o-w
			// (hat tip to Jorge H. N. de Vasconcelos)
			/*jshint scripturl:true */
			iframeSrc: /^https/i.test(window.location.href || '') ? 'javascript:false' : 'about:blank',

			// force usage of iframe in non-IE browsers (handy for blocking applets)
			forceIframe: false,

			// z-index for the blocking overlay
			baseZ: 1000,

			// set these to true to have the message automatically centered
			centerX: true, // <-- only effects element blocking (page block controlled via css above)
			centerY: true,

			// allow body element to be stetched in ie6; this makes blocking look better
			// on "short" pages.  disable if you wish to prevent changes to the body height
			allowBodyStretch: true,

			// enable if you want key and mouse events to be disabled for content that is blocked
			bindEvents: true,

			// be default blockUI will supress tab navigation from leaving blocking content
			// (if bindEvents is true)
			constrainTabKey: true,

			// fadeIn time in millis; set to 0 to disable fadeIn on block
			fadeIn: 200,

			// fadeOut time in millis; set to 0 to disable fadeOut on unblock
			fadeOut: 400,

			// time in millis to wait before auto-unblocking; set to 0 to disable auto-unblock
			timeout: 0,

			// disable if you don't want to show the overlay
			showOverlay: true,

			// if true, focus will be placed in the first available input field when
			// page blocking
			focusInput: true,

			// elements that can receive focus
			focusableElements: ':input:enabled:visible',

			// suppresses the use of overlay styles on FF/Linux (due to performance issues with opacity)
			// no longer needed in 2012
			// applyPlatformOpacityRules: true,

			// callback method invoked when fadeIn has completed and blocking message is visible
			onBlock: null,

			// callback method invoked when unblocking has completed; the callback is
			// passed the element that has been unblocked (which is the window object for page
			// blocks) and the options that were passed to the unblock call:
			//	onUnblock(element, options)
			onUnblock: null,

			// callback method invoked when the overlay area is clicked.
			// setting this will turn the cursor to a pointer, otherwise cursor defined in overlayCss will be used.
			onOverlayClick: null,

			// don't ask; if you really must know: http://groups.google.com/group/jquery-en/browse_thread/thread/36640a8730503595/2f6a79a77a78e493#2f6a79a77a78e493
			quirksmodeOffsetHack: 4,

			// class name of the message block
			blockMsgClass: 'blockMsg',

			// if it is already blocked, then ignore it (don't unblock and reblock)
			ignoreIfBlocked: false
		};

		// private data and functions follow...

		var pageBlock = null;
		var pageBlockEls = [];

		function install(el, opts) {
			var css, themedCSS;
			var full = (el == window);
			var msg = (opts && opts.message !== undefined ? opts.message : undefined);
			opts = $.extend({}, $.blockUI.defaults, opts || {});

			if (opts.ignoreIfBlocked && $(el).data('blockUI.isBlocked'))
				return;

			opts.overlayCSS = $.extend({}, $.blockUI.defaults.overlayCSS, opts.overlayCSS || {});
			css = $.extend({}, $.blockUI.defaults.css, opts.css || {});
			if (opts.onOverlayClick)
				opts.overlayCSS.cursor = 'pointer';

			themedCSS = $.extend({}, $.blockUI.defaults.themedCSS, opts.themedCSS || {});
			msg = msg === undefined ? opts.message : msg;

			// remove the current block (if there is one)
			if (full && pageBlock)
				remove(window, { fadeOut: 0 });

			// if an existing element is being used as the blocking content then we capture
			// its current place in the DOM (and current display style) so we can restore
			// it when we unblock
			if (msg && typeof msg != 'string' && (msg.parentNode || msg.jquery)) {
				var node = msg.jquery ? msg[0] : msg;
				var data = {};
				$(el).data('blockUI.history', data);
				data.el = node;
				data.parent = node.parentNode;
				data.display = node.style.display;
				data.position = node.style.position;
				if (data.parent)
					data.parent.removeChild(node);
			}

			$(el).data('blockUI.onUnblock', opts.onUnblock);
			var z = opts.baseZ;

			// blockUI uses 3 layers for blocking, for simplicity they are all used on every platform;
			// layer1 is the iframe layer which is used to supress bleed through of underlying content
			// layer2 is the overlay layer which has opacity and a wait cursor (by default)
			// layer3 is the message content that is displayed while blocking
			var lyr1, lyr2, lyr3, s;
			if (msie || opts.forceIframe)
				lyr1 = $('<iframe class="blockUI" style="z-index:' + (z++) + ';display:none;border:none;margin:0;padding:0;position:absolute;width:100%;height:100%;top:0;left:0" src="' + opts.iframeSrc + '"></iframe>');
			else
				lyr1 = $('<div class="blockUI" style="display:none"></div>');

			if (opts.theme)
				lyr2 = $('<div class="blockUI blockOverlay ui-widget-overlay" style="z-index:' + (z++) + ';display:none"></div>');
			else
				lyr2 = $('<div class="blockUI blockOverlay" style="z-index:' + (z++) + ';display:none;border:none;margin:0;padding:0;width:100%;height:100%;top:0;left:0"></div>');

			if (opts.theme && full) {
				s = '<div class="blockUI ' + opts.blockMsgClass + ' blockPage ui-dialog ui-widget ui-corner-all" style="z-index:' + (z + 10) + ';display:none;position:fixed">';
				if (opts.title) {
					s += '<div class="ui-widget-header ui-dialog-titlebar ui-corner-all blockTitle">' + (opts.title || '&nbsp;') + '</div>';
				}
				s += '<div class="ui-widget-content ui-dialog-content"></div>';
				s += '</div>';
			}
			else if (opts.theme) {
				s = '<div class="blockUI ' + opts.blockMsgClass + ' blockElement ui-dialog ui-widget ui-corner-all" style="z-index:' + (z + 10) + ';display:none;position:absolute">';
				if (opts.title) {
					s += '<div class="ui-widget-header ui-dialog-titlebar ui-corner-all blockTitle">' + (opts.title || '&nbsp;') + '</div>';
				}
				s += '<div class="ui-widget-content ui-dialog-content"></div>';
				s += '</div>';
			}
			else if (full) {
				s = '<div class="blockUI ' + opts.blockMsgClass + ' blockPage" style="z-index:' + (z + 10) + ';display:none;position:fixed"></div>';
			}
			else {
				s = '<div class="blockUI ' + opts.blockMsgClass + ' blockElement" style="z-index:' + (z + 10) + ';display:none;position:absolute"></div>';
			}
			lyr3 = $(s);

			// if we have a message, style it
			if (msg) {
				if (opts.theme) {
					lyr3.css(themedCSS);
					lyr3.addClass('ui-widget-content');
				}
				else
					lyr3.css(css);
			}

			// style the overlay
			if (!opts.theme /*&& (!opts.applyPlatformOpacityRules)*/)
				lyr2.css(opts.overlayCSS);
			lyr2.css('position', full ? 'fixed' : 'absolute');

			// make iframe layer transparent in IE
			if (msie || opts.forceIframe)
				lyr1.css('opacity', 0.0);

			//$([lyr1[0],lyr2[0],lyr3[0]]).appendTo(full ? 'body' : el);
			var layers = [lyr1, lyr2, lyr3], $par = full ? $('body') : $(el);
			$.each(layers, function () {
				this.appendTo($par);
			});

			if (opts.theme && opts.draggable && $.fn.draggable) {
				lyr3.draggable({
					handle: '.ui-dialog-titlebar',
					cancel: 'li'
				});
			}

			// ie7 must use absolute positioning in quirks mode and to account for activex issues (when scrolling)
			var expr = setExpr && (!$.support.boxModel || $('object,embed', full ? null : el).length > 0);
			if (ie6 || expr) {
				// give body 100% height
				if (full && opts.allowBodyStretch && $.support.boxModel)
					$('html,body').css('height', '100%');

				// fix ie6 issue when blocked element has a border width
				if ((ie6 || !$.support.boxModel) && !full) {
					var t = sz(el, 'borderTopWidth'), l = sz(el, 'borderLeftWidth');
					var fixT = t ? '(0 - ' + t + ')' : 0;
					var fixL = l ? '(0 - ' + l + ')' : 0;
				}

				// simulate fixed position
				$.each(layers, function (i, o) {
					var s = o[0].style;
					s.position = 'absolute';
					if (i < 2) {
						if (full)
							s.setExpression('height', 'Math.max(document.body.scrollHeight, document.body.offsetHeight) - (jQuery.support.boxModel?0:' + opts.quirksmodeOffsetHack + ') + "px"');
						else
							s.setExpression('height', 'this.parentNode.offsetHeight + "px"');
						if (full)
							s.setExpression('width', 'jQuery.support.boxModel && document.documentElement.clientWidth || document.body.clientWidth + "px"');
						else
							s.setExpression('width', 'this.parentNode.offsetWidth + "px"');
						if (fixL) s.setExpression('left', fixL);
						if (fixT) s.setExpression('top', fixT);
					}
					else if (opts.centerY) {
						if (full) s.setExpression('top', '(document.documentElement.clientHeight || document.body.clientHeight) / 2 - (this.offsetHeight / 2) + (blah = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + "px"');
						s.marginTop = 0;
					}
					else if (!opts.centerY && full) {
						var top = (opts.css && opts.css.top) ? parseInt(opts.css.top, 10) : 0;
						var expression = '((document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop) + ' + top + ') + "px"';
						s.setExpression('top', expression);
					}
				});
			}

			// show the message
			if (msg) {
				if (opts.theme)
					lyr3.find('.ui-widget-content').append(msg);
				else
					lyr3.append(msg);
				if (msg.jquery || msg.nodeType)
					$(msg).show();
			}

			if ((msie || opts.forceIframe) && opts.showOverlay)
				lyr1.show(); // opacity is zero
			if (opts.fadeIn) {
				var cb = opts.onBlock ? opts.onBlock : noOp;
				var cb1 = (opts.showOverlay && !msg) ? cb : noOp;
				var cb2 = msg ? cb : noOp;
				if (opts.showOverlay)
					lyr2._fadeIn(opts.fadeIn, cb1);
				if (msg)
					lyr3._fadeIn(opts.fadeIn, cb2);
			}
			else {
				if (opts.showOverlay)
					lyr2.show();
				if (msg)
					lyr3.show();
				if (opts.onBlock)
					opts.onBlock();
			}

			// bind key and mouse events
			bind(1, el, opts);

			if (full) {
				pageBlock = lyr3[0];
				pageBlockEls = $(opts.focusableElements, pageBlock);
				if (opts.focusInput)
					setTimeout(focus, 20);
			}
			else
				center(lyr3[0], opts.centerX, opts.centerY);

			if (opts.timeout) {
				// auto-unblock
				var to = setTimeout(function () {
					if (full)
						$.unblockUI(opts);
					else
						$(el).unblock(opts);
				}, opts.timeout);
				$(el).data('blockUI.timeout', to);
			}
		}

		// remove the block
		function remove(el, opts) {
			var count;
			var full = (el == window);
			var $el = $(el);
			var data = $el.data('blockUI.history');
			var to = $el.data('blockUI.timeout');
			if (to) {
				clearTimeout(to);
				$el.removeData('blockUI.timeout');
			}
			opts = $.extend({}, $.blockUI.defaults, opts || {});
			bind(0, el, opts); // unbind events

			if (opts.onUnblock === null) {
				opts.onUnblock = $el.data('blockUI.onUnblock');
				$el.removeData('blockUI.onUnblock');
			}

			var els;
			if (full) // crazy selector to handle odd field errors in ie6/7
				els = $('body').children().filter('.blockUI').add('body > .blockUI');
			else
				els = $el.find('>.blockUI');

			// fix cursor issue
			if (opts.cursorReset) {
				if (els.length > 1)
					els[1].style.cursor = opts.cursorReset;
				if (els.length > 2)
					els[2].style.cursor = opts.cursorReset;
			}

			if (full)
				pageBlock = pageBlockEls = null;

			if (opts.fadeOut) {
				count = els.length;
				els.stop().fadeOut(opts.fadeOut, function () {
					if (--count === 0)
						reset(els, data, opts, el);
				});
			}
			else
				reset(els, data, opts, el);
		}

		// move blocking element back into the DOM where it started
		function reset(els, data, opts, el) {
			var $el = $(el);
			if ($el.data('blockUI.isBlocked'))
				return;

			els.each(function (i, o) {
				// remove via DOM calls so we don't lose event handlers
				if (this.parentNode)
					this.parentNode.removeChild(this);
			});

			if (data && data.el) {
				data.el.style.display = data.display;
				data.el.style.position = data.position;
				if (data.parent)
					data.parent.appendChild(data.el);
				$el.removeData('blockUI.history');
			}

			if ($el.data('blockUI.static')) {
				$el.css('position', 'static'); // #22
			}

			if (typeof opts.onUnblock == 'function')
				opts.onUnblock(el, opts);

			// fix issue in Safari 6 where block artifacts remain until reflow
			var body = $(document.body), w = body.width(), cssW = body[0].style.width;
			body.width(w - 1).width(w);
			body[0].style.width = cssW;
		}

		// bind/unbind the handler
		function bind(b, el, opts) {
			var full = el == window, $el = $(el);

			// don't bother unbinding if there is nothing to unbind
			if (!b && (full && !pageBlock || !full && !$el.data('blockUI.isBlocked')))
				return;

			$el.data('blockUI.isBlocked', b);

			// don't bind events when overlay is not in use or if bindEvents is false
			if (!full || !opts.bindEvents || (b && !opts.showOverlay))
				return;

			// bind anchors and inputs for mouse and key events
			var events = 'mousedown mouseup keydown keypress keyup touchstart touchend touchmove';
			if (b)
				$(document).bind(events, opts, handler);
			else
				$(document).unbind(events, handler);

			// former impl...
			//		var $e = $('a,:input');
			//		b ? $e.bind(events, opts, handler) : $e.unbind(events, handler);
		}

		// event handler to suppress keyboard/mouse events when blocking
		function handler(e) {
			// allow tab navigation (conditionally)
			if (e.type === 'keydown' && e.keyCode && e.keyCode == 9) {
				if (pageBlock && e.data.constrainTabKey) {
					var els = pageBlockEls;
					var fwd = !e.shiftKey && e.target === els[els.length - 1];
					var back = e.shiftKey && e.target === els[0];
					if (fwd || back) {
						setTimeout(function () { focus(back); }, 10);
						return false;
					}
				}
			}
			var opts = e.data;
			var target = $(e.target);
			if (target.hasClass('blockOverlay') && opts.onOverlayClick)
				opts.onOverlayClick(e);

			// allow events within the message content
			if (target.parents('div.' + opts.blockMsgClass).length > 0)
				return true;

			// allow events for content that is not being blocked
			return target.parents().children().filter('div.blockUI').length === 0;
		}

		function focus(back) {
			if (!pageBlockEls)
				return;
			var e = pageBlockEls[back === true ? pageBlockEls.length - 1 : 0];
			if (e)
				e.focus();
		}

		function center(el, x, y) {
			var p = el.parentNode, s = el.style;
			var l = ((p.offsetWidth - el.offsetWidth) / 2) - sz(p, 'borderLeftWidth');
			var t = ((p.offsetHeight - el.offsetHeight) / 2) - sz(p, 'borderTopWidth');
			if (x) s.left = l > 0 ? (l + 'px') : '0';
			if (y) s.top = t > 0 ? (t + 'px') : '0';
		}

		function sz(el, p) {
			return parseInt($.css(el, p), 10) || 0;
		}

	}


	/*global define:true */
	if (typeof define === 'function' && define.amd && define.amd.jQuery) {
		define(['jquery'], setup);
	} else {
		setup(jQuery);
	}

})();

/*!
 * jQuery Templates Plugin 1.0.0pre
 * http://github.com/jquery/jquery-tmpl
 * Requires jQuery 1.4.2
 *
 * Copyright Software Freedom Conservancy, Inc.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 */
(function (jQuery, undefined) {
	var oldManip = jQuery.fn.domManip, tmplItmAtt = "_tmplitem", htmlExpr = /^[^<]*(<[\w\W]+>)[^>]*$|\{\{\! /,
		newTmplItems = {}, wrappedItems = {}, appendToTmplItems, topTmplItem = { key: 0, data: {} }, itemKey = 0, cloneIndex = 0, stack = [];

	function newTmplItem(options, parentItem, fn, data) {
		// Returns a template item data structure for a new rendered instance of a template (a 'template item').
		// The content field is a hierarchical array of strings and nested items (to be
		// removed and replaced by nodes field of dom elements, once inserted in DOM).
		var newItem = {
			data: data || (data === 0 || data === false) ? data : (parentItem ? parentItem.data : {}),
			_wrap: parentItem ? parentItem._wrap : null,
			tmpl: null,
			parent: parentItem || null,
			nodes: [],
			calls: tiCalls,
			nest: tiNest,
			wrap: tiWrap,
			html: tiHtml,
			update: tiUpdate
		};
		if (options) {
			jQuery.extend(newItem, options, { nodes: [], parent: parentItem });
		}
		if (fn) {
			// Build the hierarchical content to be used during insertion into DOM
			newItem.tmpl = fn;
			newItem._ctnt = newItem._ctnt || newItem.tmpl(jQuery, newItem);
			newItem.key = ++itemKey;
			// Keep track of new template item, until it is stored as jQuery Data on DOM element
			(stack.length ? wrappedItems : newTmplItems)[itemKey] = newItem;
		}
		return newItem;
	}

	// Override appendTo etc., in order to provide support for targeting multiple elements. (This code would disappear if integrated in jquery core).
	jQuery.each({
		appendTo: "append",
		prependTo: "prepend",
		insertBefore: "before",
		insertAfter: "after",
		replaceAll: "replaceWith"
	}, function (name, original) {
		jQuery.fn[name] = function (selector) {
			var ret = [], insert = jQuery(selector), elems, i, l, tmplItems,
				parent = this.length === 1 && this[0].parentNode;

			appendToTmplItems = newTmplItems || {};
			if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
				insert[original](this[0]);
				ret = this;
			} else {
				for (i = 0, l = insert.length; i < l; i++) {
					cloneIndex = i;
					elems = (i > 0 ? this.clone(true) : this).get();
					jQuery(insert[i])[original](elems);
					ret = ret.concat(elems);
				}
				cloneIndex = 0;
				ret = this.pushStack(ret, name, insert.selector);
			}
			tmplItems = appendToTmplItems;
			appendToTmplItems = null;
			jQuery.tmpl.complete(tmplItems);
			return ret;
		};
	});

	jQuery.fn.extend({
		// Use first wrapped element as template markup.
		// Return wrapped set of template items, obtained by rendering template against data.
		tmpl: function (data, options, parentItem) {
			return jQuery.tmpl(this[0], data, options, parentItem);
		},

		// Find which rendered template item the first wrapped DOM element belongs to
		tmplItem: function () {
			return jQuery.tmplItem(this[0]);
		},

		// Consider the first wrapped element as a template declaration, and get the compiled template or store it as a named template.
		template: function (name) {
			return jQuery.template(name, this[0]);
		},

		domManip: function (args, table, callback, options) {
			if (args[0] && jQuery.isArray(args[0])) {
				var dmArgs = jQuery.makeArray(arguments), elems = args[0], elemsLength = elems.length, i = 0, tmplItem;
				while (i < elemsLength && !(tmplItem = jQuery.data(elems[i++], "tmplItem"))) { }
				if (tmplItem && cloneIndex) {
					dmArgs[2] = function (fragClone) {
						// Handler called by oldManip when rendered template has been inserted into DOM.
						jQuery.tmpl.afterManip(this, fragClone, callback);
					};
				}
				oldManip.apply(this, dmArgs);
			} else {
				oldManip.apply(this, arguments);
			}
			cloneIndex = 0;
			if (!appendToTmplItems) {
				jQuery.tmpl.complete(newTmplItems);
			}
			return this;
		}
	});

	jQuery.extend({
		// Return wrapped set of template items, obtained by rendering template against data.
		tmpl: function (tmpl, data, options, parentItem) {
			var ret, topLevel = !parentItem;
			if (topLevel) {
				// This is a top-level tmpl call (not from a nested template using {{tmpl}})
				parentItem = topTmplItem;
				tmpl = jQuery.template[tmpl] || jQuery.template(null, tmpl);
				wrappedItems = {}; // Any wrapped items will be rebuilt, since this is top level
			} else if (!tmpl) {
				// The template item is already associated with DOM - this is a refresh.
				// Re-evaluate rendered template for the parentItem
				tmpl = parentItem.tmpl;
				newTmplItems[parentItem.key] = parentItem;
				parentItem.nodes = [];
				if (parentItem.wrapped) {
					updateWrapped(parentItem, parentItem.wrapped);
				}
				// Rebuild, without creating a new template item
				return jQuery(build(parentItem, null, parentItem.tmpl(jQuery, parentItem)));
			}
			if (!tmpl) {
				return []; // Could throw...
			}
			if (typeof data === "function") {
				data = data.call(parentItem || {});
			}
			if (options && options.wrapped) {
				updateWrapped(options, options.wrapped);
			}
			ret = jQuery.isArray(data) ?
				jQuery.map(data, function (dataItem) {
					return dataItem ? newTmplItem(options, parentItem, tmpl, dataItem) : null;
				}) :
				[newTmplItem(options, parentItem, tmpl, data)];
			return topLevel ? jQuery(build(parentItem, null, ret)) : ret;
		},

		// Return rendered template item for an element.
		tmplItem: function (elem) {
			var tmplItem;
			if (elem instanceof jQuery) {
				elem = elem[0];
			}
			while (elem && elem.nodeType === 1 && !(tmplItem = jQuery.data(elem, "tmplItem")) && (elem = elem.parentNode)) { }
			return tmplItem || topTmplItem;
		},

		// Set:
		// Use $.template( name, tmpl ) to cache a named template,
		// where tmpl is a template string, a script element or a jQuery instance wrapping a script element, etc.
		// Use $( "selector" ).template( name ) to provide access by name to a script block template declaration.

		// Get:
		// Use $.template( name ) to access a cached template.
		// Also $( selectorToScriptBlock ).template(), or $.template( null, templateString )
		// will return the compiled template, without adding a name reference.
		// If templateString includes at least one HTML tag, $.template( templateString ) is equivalent
		// to $.template( null, templateString )
		template: function (name, tmpl) {
			if (tmpl) {
				// Compile template and associate with name
				if (typeof tmpl === "string") {
					// This is an HTML string being passed directly in.
					tmpl = buildTmplFn(tmpl);
				} else if (tmpl instanceof jQuery) {
					tmpl = tmpl[0] || {};
				}
				if (tmpl.nodeType) {
					// If this is a template block, use cached copy, or generate tmpl function and cache.
					tmpl = jQuery.data(tmpl, "tmpl") || jQuery.data(tmpl, "tmpl", buildTmplFn(tmpl.innerHTML));
					// Issue: In IE, if the container element is not a script block, the innerHTML will remove quotes from attribute values whenever the value does not include white space.
					// This means that foo="${x}" will not work if the value of x includes white space: foo="${x}" -> foo=value of x.
					// To correct this, include space in tag: foo="${ x }" -> foo="value of x"
				}
				return typeof name === "string" ? (jQuery.template[name] = tmpl) : tmpl;
			}
			// Return named compiled template
			return name ? (typeof name !== "string" ? jQuery.template(null, name) :
				(jQuery.template[name] ||
					// If not in map, and not containing at least on HTML tag, treat as a selector.
					// (If integrated with core, use quickExpr.exec)
					jQuery.template(null, htmlExpr.test(name) ? name : jQuery(name)))) : null;
		},

		encode: function (text) {
			// Do HTML encoding replacing < > & and ' and " by corresponding entities.
			return ("" + text).split("<").join("&lt;").split(">").join("&gt;").split('"').join("&#34;").split("'").join("&#39;");
		}
	});

	jQuery.extend(jQuery.tmpl, {
		tag: {
			"tmpl": {
				_default: { $2: "null" },
				open: "if($notnull_1){__=__.concat($item.nest($1,$2));}"
				// tmpl target parameter can be of type function, so use $1, not $1a (so not auto detection of functions)
				// This means that {{tmpl foo}} treats foo as a template (which IS a function).
				// Explicit parens can be used if foo is a function that returns a template: {{tmpl foo()}}.
			},
			"wrap": {
				_default: { $2: "null" },
				open: "$item.calls(__,$1,$2);__=[];",
				close: "call=$item.calls();__=call._.concat($item.wrap(call,__));"
			},
			"each": {
				_default: { $2: "$index, $value" },
				open: "if($notnull_1){$.each($1a,function($2){with(this){",
				close: "}});}"
			},
			"if": {
				open: "if(($notnull_1) && $1a){",
				close: "}"
			},
			"else": {
				_default: { $1: "true" },
				open: "}else if(($notnull_1) && $1a){"
			},
			"html": {
				// Unecoded expression evaluation.
				open: "if($notnull_1){__.push($1a);}"
			},
			"=": {
				// Encoded expression evaluation. Abbreviated form is ${}.
				_default: { $1: "$data" },
				open: "if($notnull_1){__.push($.encode($1a));}"
			},
			"!": {
				// Comment tag. Skipped by parser
				open: ""
			}
		},

		// This stub can be overridden, e.g. in jquery.tmplPlus for providing rendered events
		complete: function (items) {
			newTmplItems = {};
		},

		// Call this from code which overrides domManip, or equivalent
		// Manage cloning/storing template items etc.
		afterManip: function afterManip(elem, fragClone, callback) {
			// Provides cloned fragment ready for fixup prior to and after insertion into DOM
			var content = fragClone.nodeType === 11 ?
				jQuery.makeArray(fragClone.childNodes) :
				fragClone.nodeType === 1 ? [fragClone] : [];

			// Return fragment to original caller (e.g. append) for DOM insertion
			callback.call(elem, fragClone);

			// Fragment has been inserted:- Add inserted nodes to tmplItem data structure. Replace inserted element annotations by jQuery.data.
			storeTmplItems(content);
			cloneIndex++;
		}
	});

	//========================== Private helper functions, used by code above ==========================

	function build(tmplItem, nested, content) {
		// Convert hierarchical content into flat string array
		// and finally return array of fragments ready for DOM insertion
		var frag, ret = content ? jQuery.map(content, function (item) {
			return (typeof item === "string") ?
				// Insert template item annotations, to be converted to jQuery.data( "tmplItem" ) when elems are inserted into DOM.
				(tmplItem.key ? item.replace(/(<\w+)(?=[\s>])(?![^>]*_tmplitem)([^>]*)/g, "$1 " + tmplItmAtt + "=\"" + tmplItem.key + "\" $2") : item) :
				// This is a child template item. Build nested template.
				build(item, tmplItem, item._ctnt);
		}) :
		// If content is not defined, insert tmplItem directly. Not a template item. May be a string, or a string array, e.g. from {{html $item.html()}}.
		tmplItem;
		if (nested) {
			return ret;
		}

		// top-level template
		ret = ret.join("");

		// Support templates which have initial or final text nodes, or consist only of text
		// Also support HTML entities within the HTML markup.
		ret.replace(/^\s*([^<\s][^<]*)?(<[\w\W]+>)([^>]*[^>\s])?\s*$/, function (all, before, middle, after) {
			frag = jQuery(middle).get();

			storeTmplItems(frag);
			if (before) {
				frag = unencode(before).concat(frag);
			}
			if (after) {
				frag = frag.concat(unencode(after));
			}
		});
		return frag ? frag : unencode(ret);
	}

	function unencode(text) {
		// Use createElement, since createTextNode will not render HTML entities correctly
		var el = document.createElement("div");
		el.innerHTML = text;
		return jQuery.makeArray(el.childNodes);
	}

	// Generate a reusable function that will serve to render a template against data
	function buildTmplFn(markup) {
		return new Function("jQuery", "$item",
			// Use the variable __ to hold a string array while building the compiled template. (See https://github.com/jquery/jquery-tmpl/issues#issue/10).
			"var $=jQuery,call,__=[],$data=$item.data;" +

			// Introduce the data as local variables using with(){}
			"with($data){__.push('" +

			// Convert the template into pure JavaScript
			jQuery.trim(markup)
				.replace(/([\\'])/g, "\\$1")
				.replace(/[\r\t\n]/g, " ")
				.replace(/\$\{([^\}]*)\}/g, "{{= $1}}")
				.replace(/\{\{(\/?)(\w+|.)(?:\(((?:[^\}]|\}(?!\}))*?)?\))?(?:\s+(.*?)?)?(\(((?:[^\}]|\}(?!\}))*?)\))?\s*\}\}/g,
				function (all, slash, type, fnargs, target, parens, args) {
					var tag = jQuery.tmpl.tag[type], def, expr, exprAutoFnDetect;
					if (!tag) {
						throw "Unknown template tag: " + type;
					}
					def = tag._default || [];
					if (parens && !/\w$/.test(target)) {
						target += parens;
						parens = "";
					}
					if (target) {
						target = unescape(target);
						args = args ? ("," + unescape(args) + ")") : (parens ? ")" : "");
						// Support for target being things like a.toLowerCase();
						// In that case don't call with template item as 'this' pointer. Just evaluate...
						expr = parens ? (target.indexOf(".") > -1 ? target + unescape(parens) : ("(" + target + ").call($item" + args)) : target;
						exprAutoFnDetect = parens ? expr : "(typeof(" + target + ")==='function'?(" + target + ").call($item):(" + target + "))";
					} else {
						exprAutoFnDetect = expr = def.$1 || "null";
					}
					fnargs = unescape(fnargs);
					return "');" +
						tag[slash ? "close" : "open"]
							.split("$notnull_1").join(target ? "typeof(" + target + ")!=='undefined' && (" + target + ")!=null" : "true")
							.split("$1a").join(exprAutoFnDetect)
							.split("$1").join(expr)
							.split("$2").join(fnargs || def.$2 || "") +
						"__.push('";
				}) +
			"');}return __;"
		);
	}
	function updateWrapped(options, wrapped) {
		// Build the wrapped content.
		options._wrap = build(options, true,
			// Suport imperative scenario in which options.wrapped can be set to a selector or an HTML string.
			jQuery.isArray(wrapped) ? wrapped : [htmlExpr.test(wrapped) ? wrapped : jQuery(wrapped).html()]
		).join("");
	}

	function unescape(args) {
		return args ? args.replace(/\\'/g, "'").replace(/\\\\/g, "\\") : null;
	}
	function outerHtml(elem) {
		var div = document.createElement("div");
		div.appendChild(elem.cloneNode(true));
		return div.innerHTML;
	}

	// Store template items in jQuery.data(), ensuring a unique tmplItem data data structure for each rendered template instance.
	function storeTmplItems(content) {
		var keySuffix = "_" + cloneIndex, elem, elems, newClonedItems = {}, i, l, m;
		for (i = 0, l = content.length; i < l; i++) {
			if ((elem = content[i]).nodeType !== 1) {
				continue;
			}
			elems = elem.getElementsByTagName("*");
			for (m = elems.length - 1; m >= 0; m--) {
				processItemKey(elems[m]);
			}
			processItemKey(elem);
		}
		function processItemKey(el) {
			var pntKey, pntNode = el, pntItem, tmplItem, key;
			// Ensure that each rendered template inserted into the DOM has its own template item,
			if ((key = el.getAttribute(tmplItmAtt))) {
				while (pntNode.parentNode && (pntNode = pntNode.parentNode).nodeType === 1 && !(pntKey = pntNode.getAttribute(tmplItmAtt))) { }
				if (pntKey !== key) {
					// The next ancestor with a _tmplitem expando is on a different key than this one.
					// So this is a top-level element within this template item
					// Set pntNode to the key of the parentNode, or to 0 if pntNode.parentNode is null, or pntNode is a fragment.
					pntNode = pntNode.parentNode ? (pntNode.nodeType === 11 ? 0 : (pntNode.getAttribute(tmplItmAtt) || 0)) : 0;
					if (!(tmplItem = newTmplItems[key])) {
						// The item is for wrapped content, and was copied from the temporary parent wrappedItem.
						tmplItem = wrappedItems[key];
						tmplItem = newTmplItem(tmplItem, newTmplItems[pntNode] || wrappedItems[pntNode]);
						tmplItem.key = ++itemKey;
						newTmplItems[itemKey] = tmplItem;
					}
					if (cloneIndex) {
						cloneTmplItem(key);
					}
				}
				el.removeAttribute(tmplItmAtt);
			} else if (cloneIndex && (tmplItem = jQuery.data(el, "tmplItem"))) {
				// This was a rendered element, cloned during append or appendTo etc.
				// TmplItem stored in jQuery data has already been cloned in cloneCopyEvent. We must replace it with a fresh cloned tmplItem.
				cloneTmplItem(tmplItem.key);
				newTmplItems[tmplItem.key] = tmplItem;
				pntNode = jQuery.data(el.parentNode, "tmplItem");
				pntNode = pntNode ? pntNode.key : 0;
			}
			if (tmplItem) {
				pntItem = tmplItem;
				// Find the template item of the parent element.
				// (Using !=, not !==, since pntItem.key is number, and pntNode may be a string)
				while (pntItem && pntItem.key != pntNode) {
					// Add this element as a top-level node for this rendered template item, as well as for any
					// ancestor items between this item and the item of its parent element
					pntItem.nodes.push(el);
					pntItem = pntItem.parent;
				}
				// Delete content built during rendering - reduce API surface area and memory use, and avoid exposing of stale data after rendering...
				delete tmplItem._ctnt;
				delete tmplItem._wrap;
				// Store template item as jQuery data on the element
				jQuery.data(el, "tmplItem", tmplItem);
			}
			function cloneTmplItem(key) {
				key = key + keySuffix;
				tmplItem = newClonedItems[key] =
					(newClonedItems[key] || newTmplItem(tmplItem, newTmplItems[tmplItem.parent.key + keySuffix] || tmplItem.parent));
			}
		}
	}

	//---- Helper functions for template item ----

	function tiCalls(content, tmpl, data, options) {
		if (!content) {
			return stack.pop();
		}
		stack.push({ _: content, tmpl: tmpl, item: this, data: data, options: options });
	}

	function tiNest(tmpl, data, options) {
		// nested template, using {{tmpl}} tag
		return jQuery.tmpl(jQuery.template(tmpl), data, options, this);
	}

	function tiWrap(call, wrapped) {
		// nested template, using {{wrap}} tag
		var options = call.options || {};
		options.wrapped = wrapped;
		// Apply the template, which may incorporate wrapped content,
		return jQuery.tmpl(jQuery.template(call.tmpl), call.data, options, call.item);
	}

	function tiHtml(filter, textOnly) {
		var wrapped = this._wrap;
		return jQuery.map(
			jQuery(jQuery.isArray(wrapped) ? wrapped.join("") : wrapped).filter(filter || "*"),
			function (e) {
				return textOnly ?
					e.innerText || e.textContent :
					e.outerHTML || outerHtml(e);
			});
	}

	function tiUpdate() {
		var coll = this.nodes;
		jQuery.tmpl(null, null, null, this).insertBefore(coll[0]);
		jQuery(coll).remove();
	}
})(jQuery);
/*!
 * tmplPlus.js: for jQuery Templates Plugin 1.0.0pre
 * Additional templating features or support for more advanced/less common scenarios.
 * Requires jquery.tmpl.js
 * http://github.com/jquery/jquery-tmpl
 *
 * Copyright Software Freedom Conservancy, Inc.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 */
(function (jQuery) {
	var oldComplete = jQuery.tmpl.complete, oldManip = jQuery.fn.domManip;

	// Override jQuery.tmpl.complete in order to provide rendered event.
	jQuery.tmpl.complete = function (tmplItems) {
		var tmplItem;
		oldComplete(tmplItems);
		for (tmplItem in tmplItems) {
			tmplItem = tmplItems[tmplItem];
			if (tmplItem.addedTmplItems && jQuery.inArray(tmplItem, tmplItem.addedTmplItems) === -1) {
				tmplItem.addedTmplItems.push(tmplItem);
			}
		}
		for (tmplItem in tmplItems) {
			tmplItem = tmplItems[tmplItem];
			// Raise rendered event
			if (tmplItem.rendered) {
				tmplItem.rendered(tmplItem);
			}
		}
	};

	jQuery.extend({
		tmplCmd: function (command, data, tmplItems) {
			var retTmplItems = [], before;
			function find(data, tmplItems) {
				var found = [], tmplItem, ti, tl = tmplItems.length, dataItem, di = 0, dl = data.length;
				for (; di < dl;) {
					dataItem = data[di++];
					for (ti = 0; ti < tl;) {
						tmplItem = tmplItems[ti++];
						if (tmplItem.data === dataItem) {
							found.push(tmplItem);
						}
					}
				}
				return found;
			}

			data = jQuery.isArray(data) ? data : [data];
			switch (command) {
				case "find":
					return find(data, tmplItems);
				case "replace":
					data.reverse();
			}
			jQuery.each(tmplItems ? find(data, tmplItems) : data, function (i, tmplItem) {
				coll = tmplItem.nodes;
				switch (command) {
					case "update":
						tmplItem.update();
						break;
					case "remove":
						jQuery(coll).remove();
						if (tmplItems) {
							tmplItems.splice(jQuery.inArray(tmplItem, tmplItems), 1);
						}
						break;
					case "replace":
						before = before ?
							jQuery(coll).insertBefore(before)[0] :
							jQuery(coll).appendTo(coll[0].parentNode)[0];
						retTmplItems.unshift(tmplItem);
				}
			});
			return retTmplItems;
		}
	});

	jQuery.fn.extend({
		domManip: function (args, table, callback, options) {
			var data = args[1], tmpl = args[0], dmArgs;
			if (args.length >= 2 && typeof data === "object" && !data.nodeType && !(data instanceof jQuery)) {
				// args[1] is data, for a template.
				dmArgs = jQuery.makeArray(arguments);

				// Eval template to obtain fragment to clone and insert
				dmArgs[0] = [jQuery.tmpl(jQuery.template(tmpl), data, args[2], args[3])];

				dmArgs[2] = function (fragClone) {
					// Handler called by oldManip when rendered template has been inserted into DOM.
					jQuery.tmpl.afterManip(this, fragClone, callback);
				};
				return oldManip.apply(this, dmArgs);
			}
			return oldManip.apply(this, arguments);
		}
	});
})(jQuery);

/*
 * In-Field Label jQuery Plugin
 * http://fuelyourcoding.com/scripts/infield.html
 *
 * Copyright (c) 2009 Doug Neiner
 * Dual licensed under the MIT and GPL licenses.
 * Uses the same license as jQuery, see:
 * http://docs.jquery.com/License
 *
 * @version 0.1
 */
(function ($) {

	$.InFieldLabels = function (label, field, options) {
		// To avoid scope issues, use 'base' instead of 'this'
		// to reference this class from internal events and functions.
		var base = this;

		// Access to jQuery and DOM versions of each element
		base.$label = $(label);
		base.label = label;

		base.$field = $(field);
		base.field = field;

		base.$label.data("InFieldLabels", base);
		base.showing = true;

		base.init = function () {
			// Merge supplied options with default options
			base.options = $.extend({}, $.InFieldLabels.defaultOptions, options);

			// Check if the field is already filled in
			if (base.$field.val() != "") {
				base.$label.hide();
				base.showing = false;
			};

			base.$field.focus(function () {
				base.fadeOnFocus();
			}).blur(function () {
				base.checkForEmpty(true);
			}).bind('keydown.infieldlabel', function (e) {
				// Use of a namespace (.infieldlabel) allows us to
				// unbind just this method later
				base.hideOnChange(e);
			}).change(function (e) {
				base.checkForEmpty();
			}).bind('onPropertyChange', function () {
				base.checkForEmpty();
			});
		};

		// If the label is currently showing
		// then fade it down to the amount
		// specified in the settings
		base.fadeOnFocus = function () {
			if (base.showing) {
				base.setOpacity(base.options.fadeOpacity);
			};
		};

		base.setOpacity = function (opacity) {
			base.$label.stop().animate({ opacity: opacity }, base.options.fadeDuration);
			base.showing = (opacity > 0.0);
		};

		// Checks for empty as a fail safe
		// set blur to true when passing from
		// the blur event
		base.checkForEmpty = function (blur) {
			if (base.$field.val() == "") {
				base.prepForShow();
				base.setOpacity(blur ? 1.0 : base.options.fadeOpacity);
			} else {
				base.setOpacity(0.0);
			};
		};

		base.prepForShow = function (e) {
			if (!base.showing) {
				// Prepare for a animate in...
				base.$label.css({ opacity: 0.0 }).show();

				// Reattach the keydown event
				base.$field.bind('keydown.infieldlabel', function (e) {
					base.hideOnChange(e);
				});
			};
		};

		base.hideOnChange = function (e) {
			if (
				(e.keyCode == 16) || // Skip Shift
				(e.keyCode == 9) // Skip Tab
			  ) return;

			if (base.showing) {
				base.$label.hide();
				base.showing = false;
			};

			// Remove keydown event to save on CPU processing
			base.$field.unbind('keydown.infieldlabel');
		};

		// Run the initialization method
		base.init();
	};

	$.InFieldLabels.defaultOptions = {
		fadeOpacity: 0.5, // Once a field has focus, how transparent should the label be
		fadeDuration: 300 // How long should it take to animate from 1.0 opacity to the fadeOpacity
	};


	$.fn.inFieldLabels = function (options) {
		return this.each(function () {
			// Find input or textarea based on for= attribute
			// The for attribute on the label must contain the ID
			// of the input or textarea element
			var for_attr = $(this).attr('for');
			if (!for_attr) return; // Nothing to attach, since the for field wasn't used


			// Find the referenced input or textarea element
			var $field = $(
				"input#" + for_attr + "[type='text']," +
				"input#" + for_attr + "[type='password']," +
				"textarea#" + for_attr
				);

			if ($field.length == 0) return; // Again, nothing to attach

			// Only create object for input[text], input[password], or textarea
			(new $.InFieldLabels(this, $field[0], options));
		});
	};

})(jQuery);
/**
 *  Version 2.4.0 Copyright (C) 2013
 *  Tested in IE 11, FF 28.0 and Chrome 33.0.1750.154
 *  No official support for other browsers, but will TRY to accommodate challenges in other browsers.
 *  Example:
 *      Print Button: <div id="print_button">Print</div>
 *      Print Area  : <div class="PrintArea" id="MyId" class="MyClass"> ... html ... </div>
 *      Javascript  : <script>
 *                       $("div#print_button").click(function(){
 *                           $("div.PrintArea").printArea( [OPTIONS] );
 *                       });
 *                     </script>
 *  options are passed as json (example: {mode: "popup", popClose: false})
 *
 *  {OPTIONS}   | [type]     | (default), values      | Explanation
 *  ---------   | ---------  | ---------------------- | -----------
 *  @mode       | [string]   | (iframe),popup         | printable window is either iframe or browser popup
 *  @popHt      | [number]   | (500)                  | popup window height
 *  @popWd      | [number]   | (400)                  | popup window width
 *  @popX       | [number]   | (500)                  | popup window screen X position
 *  @popY       | [number]   | (500)                  | popup window screen Y position
 *  @popTitle   | [string]   | ('')                   | popup window title element
 *  @popClose   | [boolean]  | (false),true           | popup window close after printing
 *  @extraCss   | [string]   | ('')                   | comma separated list of extra css to include
 *  @retainAttr | [string[]] | ["id","class","style"] | string array of attributes to retain for the containment area. (ie: id, style, class)
 *  @standard   | [string]   | strict, loose, (html5) | Only for popup. For html 4.01, strict or loose document standard, or html 5 standard
 *  @extraHead  | [string]   | ('')                   | comma separated list of extra elements to be appended to the head tag
 */
(function ($) {
	var counter = 0;
	var modes = { iframe: "iframe", popup: "popup" };
	var standards = { strict: "strict", loose: "loose", html5: "html5" };
	var defaults = {
		mode: modes.iframe,
		standard: standards.html5,
		popHt: 500,
		popWd: 400,
		popX: 200,
		popY: 200,
		popTitle: '',
		popClose: false,
		extraCss: '',
		extraHead: '',
		retainAttr: ["id", "class", "style"]
	};

	var settings = {};//global settings

	$.fn.printArea = function (options) {
		$.extend(settings, defaults, options);

		counter++;
		var idPrefix = "printArea_";
		$("[id^=" + idPrefix + "]").remove();

		settings.id = idPrefix + counter;

		var $printSource = $(this);

		var PrintAreaWindow = PrintArea.getPrintWindow();

		PrintArea.write(PrintAreaWindow.doc, $printSource);

		setTimeout(function () { PrintArea.print(PrintAreaWindow); }, 1000);
	};

	var PrintArea = {
		print: function (PAWindow) {
			var paWindow = PAWindow.win;

			$(PAWindow.doc).ready(function () {
				paWindow.focus();
				paWindow.print();

				if (settings.mode == modes.popup && settings.popClose)
					setTimeout(function () { paWindow.close(); }, 2000);
			});
		},
		write: function (PADocument, $ele) {
			PADocument.open();
			PADocument.write(PrintArea.docType() + "<html>" + PrintArea.getHead() + PrintArea.getBody($ele) + "</html>");
			PADocument.close();
		},
		docType: function () {
			if (settings.mode == modes.iframe) return "";

			if (settings.standard == standards.html5) return "<!DOCTYPE html>";

			var transitional = settings.standard == standards.loose ? " Transitional" : "";
			var dtd = settings.standard == standards.loose ? "loose" : "strict";

			return '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01' + transitional + '//EN" "http://www.w3.org/TR/html4/' + dtd + '.dtd">';
		},
		getHead: function () {
			var extraHead = "";
			var links = "";

			if (settings.extraHead) settings.extraHead.replace(/([^,]+)/g, function (m) { extraHead += m });

			$(document).find("link")
                .filter(function () { // Requirement: <link> element MUST have rel="stylesheet" to be considered in print document
                	var relAttr = $(this).attr("rel");
                	return ($.type(relAttr) === 'undefined') == false && relAttr.toLowerCase() == 'stylesheet';
                })
                .filter(function () { // Include if media is undefined, empty, print or all
                	var mediaAttr = $(this).attr("media");
                	return $.type(mediaAttr) === 'undefined' || mediaAttr == "" || mediaAttr.toLowerCase() == 'print' || mediaAttr.toLowerCase() == 'all'
                })
                .each(function () {
                	links += '<link type="text/css" rel="stylesheet" href="' + $(this).attr("href") + '" >';
                });
			if (settings.extraCss) settings.extraCss.replace(/([^,\s]+)/g, function (m) { links += '<link type="text/css" rel="stylesheet" href="' + m + '">' });

			return "<head><title>" + settings.popTitle + "</title>" + extraHead + links + "</head>";
		},
		getBody: function (elements) {
			var htm = "";
			var attrs = settings.retainAttr;
			elements.each(function () {
				var ele = PrintArea.getFormData($(this));

				var attributes = ""
				for (var x = 0; x < attrs.length; x++) {
					var eleAttr = $(ele).attr(attrs[x]);
					if (eleAttr) attributes += (attributes.length > 0 ? " " : "") + attrs[x] + "='" + eleAttr + "'";
				}

				htm += '<div ' + attributes + '>' + $(ele).html() + '</div>';
			});

			return "<body>" + htm + "</body>";
		},
		getFormData: function (ele) {
			var copy = ele.clone();
			var copiedInputs = $("input,select,textarea", copy);
			$("input,select,textarea", ele).each(function (i) {
				var typeInput = $(this).attr("type");
				if ($.type(typeInput) === 'undefined') typeInput = $(this).is("select") ? "select" : $(this).is("textarea") ? "textarea" : "";
				var copiedInput = copiedInputs.eq(i);

				if (typeInput == "radio" || typeInput == "checkbox") copiedInput.attr("checked", $(this).is(":checked"));
				else if (typeInput == "text") copiedInput.attr("value", $(this).val());
				else if (typeInput == "select")
					$(this).find("option").each(function (i) {
						if ($(this).is(":selected")) $("option", copiedInput).eq(i).attr("selected", true);
					});
				else if (typeInput == "textarea") copiedInput.text($(this).val());
			});
			return copy;
		},
		getPrintWindow: function () {
			switch (settings.mode) {
				case modes.iframe:
					var f = new PrintArea.Iframe();
					return { win: f.contentWindow || f, doc: f.doc };
				case modes.popup:
					var p = new PrintArea.Popup();
					return { win: p, doc: p.doc };
			}
		},
		Iframe: function () {
			var frameId = settings.id;
			var iframeStyle = 'border:0;position:absolute;width:0px;height:0px;right:0px;top:0px;';
			var iframe;

			try {
				iframe = document.createElement('iframe');
				document.body.appendChild(iframe);
				$(iframe).attr({ style: iframeStyle, id: frameId, src: "#" + new Date().getTime() });
				iframe.doc = null;
				iframe.doc = iframe.contentDocument ? iframe.contentDocument : (iframe.contentWindow ? iframe.contentWindow.document : iframe.document);
			}
			catch (e) { throw e + ". iframes may not be supported in this browser."; }

			if (iframe.doc == null) throw "Cannot find document.";

			return iframe;
		},
		Popup: function () {
			var windowAttr = "location=yes,statusbar=no,directories=no,menubar=no,titlebar=no,toolbar=no,dependent=no";
			windowAttr += ",width=" + settings.popWd + ",height=" + settings.popHt;
			windowAttr += ",resizable=yes,screenX=" + settings.popX + ",screenY=" + settings.popY + ",personalbar=no,scrollbars=yes";

			var newWin = window.open("", "_blank", windowAttr);

			newWin.doc = newWin.document;

			return newWin;
		}
	};
})(jQuery);

function setRequired(isAdd, sID) {
    if (isAdd) {
        $("#" + sID).rules("add", { required: true, messages: { required: "*" } });
	}
	else {
		$("#" + sID).rules("remove");
		var vClass = $("#" + sID).prop("class");
		if (vClass != undefined && vClass.indexOf("input-validation-error") != -1) {
			$("#" + sID).removeClass("input-validation-error");
			$("#" + sID + "-error").remove();
		}
	}
	//$("form").valid();
}

function setNumber(isAdd, sID) {
    if (isAdd) {
        $("#" + sID).rules("add", { number: true, messages: { required: "必須為數字" } });
    }
    else {
        $("#" + sID).rules("remove");
        var vClass = $("#" + sID).prop("class");
        if (vClass != undefined && vClass.indexOf("input-validation-error") != -1) {
            $("#" + sID).removeClass("input-validation-error");
            $("#" + sID + "-error").remove();
        }
    }
    //$("form").valid();
}

/*
※ funcAutoComplete 的使用方式
$(document).ready(function () {
	var vURL = "@Url.Action("fnAutoComplete", "Json")";
	funcAutoComplete(vURL, "CONTENT1", 1);
	funcAutoComplete(vURL, "CONTENT7", 2);
});
 */
function funcAutoComplete(vURL, vID, vType) {
	$("#" + vID).autocomplete({
		source: function (request, response) {
			$.ajax({
				url: vURL,
				type: 'post',
				dataType: 'json',
				contentType: "application/json; charset=utf-8",
				data: JSON.stringify({ iType: vType, k: $("#" + vID).val().trim() }),
				success: function (data) {
					response($.map(data, function (item) {
						return { label: item };
					}));
				}
			});
		},
		messages: {
			noResults: "", results: function (resultsCount) { }
		},
		delay: 10,
		width: 120,
		minChars: 1, //至少輸入幾個字元才開始給提示?
		matchSubset: false,
		matchContains: false,
		cacheLength: 0,
		//noCache: true, //黑暗版自訂參數，每次都重新連後端查詢(適用總資料筆數很多時)
		//onItemSelect: findValue,
		//onFindValue: findValue,
		autoFill: false,
		mustMatch: true //允許輸入提示清單上沒有的值
	});
}

Date.prototype.Format = function (fmt) { //author: meizz 
	var o = {
		"M+": this.getMonth() + 1, //月份 
		"d+": this.getDate(), //日 
		"h+": this.getHours(), //小时 
		"m+": this.getMinutes(), //分 
		"s+": this.getSeconds(), //秒 
		"q+": Math.floor((this.getMonth() + 3) / 3), //季度 
		"S": this.getMilliseconds() //毫秒 
	};
	if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
	for (var k in o)
		if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
	return fmt;
}