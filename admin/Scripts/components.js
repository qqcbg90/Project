$(function () {
	//取得token傳回api中，頁面上若沒有token就不傳
	AddAntiForgeryToken = function (data) {
		var token = $('input[name="__RequestVerificationToken"]').val();
		if (token != null)
			data.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
		return data;
	};
	//浮動視窗
	Vue.component('floating-search', {
		template: '#floatingSearch', //Shared/template
		props: {
			parent_id: String, //母物件編號
			title: String, //視窗標題
			targets: String, //預設選取的選項 ex:id1,id2
			url: String, //呼叫的api的位置，來取得資料 ex:json
			multiple: Boolean, //是否多選 true / false
			excludeitems: String //設定要排除的選項 ex:id1,id2
		},
		data: function () {
			return {
				keywordEvent: null, //搜尋框事件
				sourceData: null, //原始資料
				keyword: '', //關鍵字
				filterData: null, //搜尋結果
				targetsValue: [] //已被選取得值
			};
		},
		mounted: function () {
			var vm = this;
			$.ajax({
				url: vm.url,
				type: 'POST',
				data: AddAntiForgeryToken({}),
				dataType: 'json'
			})
			.done(function (data) {
				//json to obj
				var _sourceData = JSON.parse(data);
				if (_sourceData != null) {
					//排除選項
					if (vm.excludeitems != null) {
						var excludeitemsArr = vm.excludeitems.split(',');
						_sourceData = _sourceData.filter(function (item, index) {
							return excludeitemsArr.indexOf(item.Value) == -1;
						})
					}
					//取得預設選項
					if (vm.targets != null && vm.targets != '') {
						var targetsArr = vm.targets.split(',');
						vm.targetsValue = _sourceData.filter(function (item, index) {
							return targetsArr.indexOf(item.Value) > -1;
						});
					}
					//將預設的選項設定為已選取
					vm.targetsValue.forEach(function (item) {
						var findIndex = _sourceData.findIndex(function (x) { return item.Value == x.Value });
						if (findIndex > -1)
							_sourceData[findIndex].Selected = true;
					});

					vm.sourceData = _sourceData; //原始資料 arary
					vm.filterData = _sourceData; //顯示用 array
				}
			})
			.fail(function (data) {
			})
		},
		watch: {
			targetsValue: function () {
				var vm = this;
				if (vm.sourceData != null) {
					vm.sourceData.forEach(function (item, index) {
						var findIndex = vm.targetsValue.findIndex(function (x) { return item.Value == x.Value });
						if (findIndex > -1)
							vm.sourceData[index].Selected = true;
						else
							vm.sourceData[index].Selected = false;
					});
				}
			},
			keyword: function () {
				var vm = this;
				vm.filterData = vm.sourceData.filter(function (item, index) {
					return item.Text.toUpperCase().indexOf(vm.keyword.toUpperCase()) > -1;
				});
			}
		},
		methods: {
			setKeywordEvent: function ($event) {
				this.keywordEvent = $event; //將目標事件儲存起來，方便直接使用
			},
			//開啟視窗
			toFadeln: function () {
				var parent_id = this.parent_id;
				$(".find").fadeOut(0);
				$("." + parent_id + " .find").fadeIn(0);
			},
			//關閉視窗
			toFadeOut: function () {
				this.keywordEvent.target.blur(); //移除焦點

				var parent_id = this.parent_id;
				$("." + parent_id + " .find").fadeOut(0);
				this.keyword = '';
			},
			//新增標籤
			targetsAdd: function (data) {
				//多選
				if (this.multiple) {
					var findIndex = this.targetsValue.findIndex(function (x) { return x.Value == data.Value });
					if (findIndex > -1) {
						this.targetsValue.splice(findIndex, 1);
					}
					else {
						this.targetsValue.push({ Text: data.Text, Value: data.Value });
					}
				}
				else {
					//Vue.set(this, 'targetsValue', [{
					//	Text: data.Text,
					//	Value: data.Value
					//}]);

					//單選
					var vm = this;

					vm.targetsValue = [{
						Text: data.Text,
						Value: data.Value
					}];

					//關閉視窗
					setTimeout(function () {
						vm.toFadeOut();
					}, 50);
				}
				//console.log('targetsAdd', this.targetsValue);
			},
			//刪除標籤
			targetsDel: function (data) {
				var findIndex = this.targetsValue.findIndex(function (x) { return x.Value == data.Value });
				if (findIndex > -1) {
					this.targetsValue.splice(findIndex, 1);
					//Vue.set(this, 'targetsValue', this.targetsValue);
				}
			},
			//傳回選取的值
			returnData: function () {
				return this.targetsValue.map(function (item, index) {
					return item.Value;
				}).join(',');
			}
		}
	});
});