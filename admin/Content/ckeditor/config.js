/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
    //config.language = 'zh-tw';
    // config.uiColor = '#AADC6E';
    //config.removePlugins = 'Basket';

    config.font_names = '新細明體;標楷體;微軟正黑體;Arial/Arial, Helvetica, sans-serif;Comic Sans MS/Comic Sans MS, cursive;Courier New/Courier New, Courier, monospace;Georgia/Georgia, serif;Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;Tahoma/Tahoma, Geneva, sans-serif;Times New Roman/Times New Roman, Times, serif;Trebuchet MS/Trebuchet MS, Helvetica, sans-serif;Verdana/Verdana, Geneva, sans-serif'; // 為加入額外字體  
    config.toolbar = 'MyBar';//把工具列名稱改為‘MyBar’ http://ckeditor.com/latest/samples/plugins/toolbar/toolbar.html
    config.toolbar_MyBar =
    [
       ['Source', '-', 'Preview'],
       ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord'],
       ['Undo', 'Redo', '-', 'Find', 'Replace'],
       ['Link', 'Unlink','Image'],
	   ['Link', 'Unlink'],
       '/',
       ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
       ['NumberedList', 'BulletedList', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
       ['Styles', 'Format', 'Font', 'FontSize'],
       ['TextColor', 'BGColor']
    ];
   

    config.enterMode = CKEDITOR.ENTER_BR;
    config.shiftEnterMode = CKEDITOR.ENTER_P;

    // 預設字體 - 詳見最下方(註一)
    config.font_defaultLabel = '標楷體';

    // 預設字體尺寸 - 詳見最下方(註一)
    config.fontSize_defaultLabel = '16px';

    //防止移除p裡面的target
    config.allowedContent = true;

   
    var ckfinderPath = "/admin/content/ckeditor/ckfinder"; //ckfinder路徑，記得放置主機時要改
    config.filebrowserBrowseUrl = ckfinderPath + '/ckfinder.html';
    config.filebrowserImageBrowseUrl = ckfinderPath + '/ckfinder.html?type=Images';
    config.filebrowserFlashBrowseUrl = ckfinderPath + '/ckfinder.html?type=Flash';
    config.filebrowserUploadUrl = ckfinderPath + '/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = ckfinderPath + '/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    config.filebrowserFlashUploadUrl = ckfinderPath + '/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';
    //config.filebrowserWindowWidth = '800';
   

};


