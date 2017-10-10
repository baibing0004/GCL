VJ.include("/SAASPortal/scriptsv2/easyui3.0/easyloader.js");
easyloader.base = '/SAASPortal/scriptsv2/easyui3.0/';
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.parser.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.draggable.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.droppable.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.resizable.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.messager.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.layout.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.menu.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.tabs.js");
VJ.include("/SAASPortal/scriptsv2/easyui3.0/jquery.pagination.js");

/* 加载母版页
*-- 参数1：url， 母版页的url
*-- 参数2：placeid， 母版页某标签的id，子页插入该标签位置
*-- 案例：VJ.loadMaster("home/Master.html","div_master");
*/
VJ.loadMaster = function(url, placeid) {
    VJ._MasterPlaceID = placeid;
    VJ._MasterContent = $("div").eq(0); //获取第1个div,赋给 VJ._MasterContent
    var master = VJ.newEl("div", "", ""); //子页面动态创建一个div
    $(master).load(url);
    $(master).insertBefore(VJ._MasterContent); //将母版页的内容加到动态创建 div 之前
}
/* 读取母版页，将母版页的内容加载到子页面，显示内容。
*/
VJ.readyMaster = function() {
    $("#" + VJ._MasterPlaceID).append(VJ._MasterContent);
    VJ._MasterContent.show();
}
/* 在某元素上加载 url 所指定网页内容。 
*-- 参数1：url, 待载入的 HTML 网页网址。
*-- 参数2：node,任意节点 当写为null时，会自动新建document下一个默认div节点 并返回给调用者
*-- 参数3：mode,"iframe"or为空，如果是"iframe",则要动态创建一个iframe,加在node内部;如果为空或者其他，在node上加载url。
*-- 参数4：callback，仅当任意节点使用load方法引入时起效的回调函数，可以用于函数注册与加载
*-- 案例：VJ.part("feeds.html","#feeds") 
*/
VJ.part = function(url, node, mode, callback) {
    if(!VJ.isValid(node)){
        node = VJ.newEl('div','','');
        node.appendTo($(document.body));        
    }
    if ($(node).attr('nodeName') && $(node).attr('nodeName').toLowerCase() == "iframe") {
        /* 在iframe中加载url 指定的网页内容*/
        return $(node).attr("src", url);
    } else if (VJ.getValue(mode, '') == "iframe") {
        //动态创建iframe,追加到指定的node内
        return $(node).append("<IFRAME class=g_iframe border=0 marginWidth=0 frameSpacing=0 marginHeight=0 frameBorder=no allowTransparency=true src=\"" + url + "\"></IFRAME>");
    } else {
        //普通元素
        return $(node).load(url,null,callback);
    }
}
//命令注册变量
if(!VJ.isValid(window.top.VJ)){
	window.top.VJ = {};
}
if(!VJ.isValid(window.top.VJ._regcomms)){    
	window.top.VJ._regcomms = [];
}
VJ._regcomms = window.top.VJ._regcomms;
/*
VJ用于被调用页面注册命令以处理异步命令调用,当命令尚未注册而已经被调用时，参数会先被缓存下来，然后当命令注册时，已知的参数再被调用。
--案例
VJ.registCommand('showXXList',getData)
*/
VJ.registCommand = function(name,func){
    var data=VJ._regcomms[name];
    if(VJ.isValid(data) && typeof(data)!='function'){        
        func.apply(null,data);
    }
    VJ._regcomms[name]=func;    
}
/*
VJ用于调用被调用页面注册的命令以处理异步命令调用，当命令尚未注册而已经被调用时，参数会先被缓存下来，然后当命令注册时，已知的参数再被调用。
--案例
VJ.callCommand('showXXList',[{id:1}])
*/
VJ.callCommand = function(name,data){
    var func = VJ._regcomms[name];
    if (VJ.isValid(func) && typeof (func) == 'function') {
        func.apply(null,data);
    }else{
        VJ._regcomms[name]=data;
    }
}
/*
用来判断是否调用页面,当已经调用过(part)，返回true,否则返回false;
--案例
if (!VJ.hasCommand('editor.open')) VJ.part("/FileServer/layout/editor/editor.htm");
*/
VJ.hasCommand = function(name) {
    var func = VJ._regcomms[name];
    return (VJ.isValid(func) && typeof (func) == 'function');
}

/*
仅限iframe方式调用时，先取消原页面添加的方法
//业务逻辑深度交叉，iframe落后的控件连接方式时使用
一定要在part前
--案例
VJ.cleanCommand('editor.open');
VJ.part("/FileServer/layout/editor/editor.htm",null,"iframe",function(){});
*/
VJ.cleanCommand = function(name){
    VJ._regcomms[name] = '';
}
/*判断是否有效，返回值为bool类型，true为有效，false为无效
*-- 参数1：data,  任意类型
*-- 案例：VJ.isValid(option); VJ.isValid(option.node);
*/
VJ.isValid = function(data) {
    if (typeof (data) != "undefined" && data != null && data != 'null' && !(data==='')){        
        return true;
    } else {
        return false;
    }
}
/*获取有效值。如果所判断值data有效，则返回data的值；否则返回默认值defaultData
*-- 参数1：data, 要进行判断的值
*-- 参数2：defaultData，默认值
*-- 案例：VJ.getValue(option.node, VJ.newEl('div', '', ''))
*/
VJ.getValue = function(data, defaultData) {
    return VJ.isValid(data) ? data : defaultData;
}
//这里添加对浏览器的判断
VJ.userAgent = {
    ie:false,
    firefox:false,
    chrome:false,
    safari:false,
    opera:false
};
{
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/msie ([\d.]+)/)) ? VJ.userAgent.ie = s[1] :
    (s = ua.match(/firefox\/([\d.]+)/)) ? VJ.userAgent.firefox = s[1] :
    (s = ua.match(/chrome\/([\d.]+)/)) ? VJ.userAgent.chrome = s[1] :
    (s = ua.match(/opera.([\d.]+)/)) ? VJ.userAgent.opera = s[1] :
    (s = ua.match(/version\/([\d.]+).*safari/)) ? VJ.userAgent.safari = s[1] : 0;
    for(key in VJ.userAgent){if(VJ.getValue(VJ.userAgent[key],false))VJ.userAgent.name=key;}
	VJ.showException("VJ.userAgent:"+VJ.userAgent.name);
	if(VJ.getValue(VJ.userAgent.ie,false))
	{
	    var ver = VJ.userAgent.ie.substring(0,1);
	    eval('VJ.userAgent.ie'+ver+' = true;VJ.userAgent.name=\'ie'+ver+'\';');
	}
}       

/* 打开一个新页面
*-- 参数1：json对象，其中必须定义url属性，指向的新地址，同时可以设置其它参数从而取代默认值
默认值如下：
VJ._openWindowOption = {
	toolbar:'no',
	location:'yes',
	menubar:'no',	
	resizable:'yes',
	scrollbars:'yes',
	status:'no'
};
可以在不同项目的Globle.js中更新更新成自己这个项目所使用的openWindow标准，一般不建议每个页面都自己设定一遍默认值。
*-- 案例：VJ.openWindow({url:'/lcms/layout/zh-cn/a.htm?'+'c='+c,height:320});
*/
VJ.openWindow = function(option) {
    //这里重新计算中心位置
    option = $.extend({}, VJ._openWindowOption, {
        left: (screen.width - VJ.getValue(option.width, VJ._openWindowOption.width)) / 2,
        top: (screen.height - VJ.getValue(option.height, VJ._openWindowOption.height)) / 2
    }, option);
    var newwindow = window.open(option.url, '_AteJ_openWindow' + Math.floor(Math.random()*1000000), $.param(option).replace(new RegExp("(&)", "g"), ','))
	newwindow.focus();
	return newwindow;
}
/*
获取远程JSON 
//VJ获取远程JSON的默认参数
VJ._getRemoteJSONOption={
filtURI:function(url){return url;}
}   
--案例
VJ.getRemoteJSON(RemoutUri.LCMS_Test);
*/
VJ.getRemoteJSON = function(url) {
    if ($.browser.msie) {
        //解决IE界面线程停滞，无法显示动画的问题
        window.setTimeout(function() {
            $.getScript(VJ._getRemoteJSONOption.filtURI(url), function() { });
        }, 500);
    } else {
        $.getScript(VJ._getRemoteJSONOption.filtURI(url), function() { });
    }
};
/*
//  用来设置checkBox,radio的checked属性，主要解决ie6下不兼容性
    在JavaScript操作Checkbox的过程中，不管新创建一个Checkbox对象或者clone一个对象，当使用appendChild方法，将新生成的CheckBox对象添加到父对象上的时候，ChecBox的checked属性将会丢失。
--案例
    VJ.setChecked($("#"),true);
*/
VJ.setChecked = function(node,value){
    function setCheckBox(node2,value){
        $(node2).attr('checked',value);
        if(VJ.userAgent.ie6 ||VJ.userAgent.ie7) {
            var chk = $(node2);
            if(VJ.isValid(chk.get(0)))
                chk.get(0).defaultChecked=value;
        }
    };
    if(node.length){
        $(node).each(function(i,v){
            setCheckBox(v,value);
        });
    }else{
        setCheckBox(node,value);
    }
}
VJ.maxlength=function(){
    $("textarea[maxlength]").unbind('change').change(function(event) {
        this.value = this.value.substring(0, $(this).attr("maxlength"));
        return;
        //先试试看
        var key;
        if ($.browser.msie) {//ie浏览器

            var key = event.keyCode;
        }
        else {//火狐浏览器
            key = event.which;
        }

        //all keys including return.
        if (key >= 33 || key == 13) {
            var maxLength = $(this).attr("maxlength");
            var length = this.value.length;
            if (length >= maxLength) {
                event.preventDefault();
            }
        }
    });

}
//用一个Map确定每一个查询对象本身
if(!VJ.isValid(window.top.VJ._AteJ_Query_Map)){    
	window.top.VJ._AteJ_Query_Map = [];
}
VJ._AteJ_Query_Map = window.top.VJ._AteJ_Query_Map;
//查询类
function _AteJ_query(option) {
    var _this = this;
    var randomid = Math.round(Math.random() * 100000000);
    var defaultOption = null;
    var init = function(option) {
        defaultOption = $.extend(defaultOption, VJ._ajaxOption, VJ._queryOption, option);
        if (defaultOption.jsonp) {
            VJ._AteJ_Query_Map[randomid] = _this;
        }
		_this.node = defaultOption.node;
    };
    init(option);
    _this.bind = function(data, option) {
        data = VJ.getValue(data, {});
        var dop = $.extend({}, defaultOption, VJ.getValue(option, {}));
        if (!dop.jsonp) {
            $.ajax($.extend({}, dop, { data: data }));
        } else {
            VJ.getRemoteJSON(
			dop.url + (VJ.isValid(data) ?
			('?jsonp=VJ._AteJ_Query_Map[' + randomid + '].afterBind' +
				($.param(data).length > 0 ? ('&' + $.param(data)) : '')
			) : ''));
        }
    };
    _this.afterBind = function(data) {
        defaultOption.success(data, 'success');
    };
}
//查询类
function _AteJ_datagrid(option) {
    var _this = this;
    var randomid = Math.round(Math.random() * 100000000);
    var defaultOption = null;
	_this.count = 0;
    _this.pIndex = 0;
//    var hasStart = false;
    _this.ListCallBack = function() {
        defaultOption.node.datagrid('load');
    };
    _this.InsertCallBack = function(isEnd) {
        defaultOption.node.datagrid('reload');
    };
    _this.DeleteCallBack = function() {
        defaultOption.node.datagrid('reload');
    };
	
    var init = function(option) {
        defaultOption = VJ.merge(defaultOption, VJ._ajaxOption, VJ._queryOption, VJ._datagridOption, option);
		var _before = defaultOption.beforeGetRow;
		defaultOption = $.extend(defaultOption,{
			pageNumber:1,
			pageSize:option.pager.defaultPageSize,
			pageList:option.pager.pSizeValuSpace,
			onLoadSuccess:function(data){
				defaultOption.afterGetRow(defaultOption.node,data);
            },
			beforeGetRow:function(node,data){
				_this.count = data.total;
				//todo 不知道是不是正确
				_this.pIndex = node.datagrid('options').pageNumber;
				_before(node,data);
			}
		});
		if(defaultOption.pager.node) {defaultOption.pager.node.remove();}
		defaultOption.pager = null;
		//转换 pagePosition //
        if (defaultOption.jsonp) {
            VJ._AteJ_Query_Map[randomid] = _this;
			defaultOption.randomid = randomid;
        }
		_this.node = defaultOption.node;
    };
    init(option);
    _this.bind = function(data, option) {
        data = VJ.getValue(data, {});
		var dop = $.extend({}, defaultOption, VJ.getValue(option, {}));
		//能多次datagrid吗
		defaultOption.node.datagrid($.extend({}, dop, { queryParams: data }));
    };
}

/* 查询操作，支持跨域或者不跨域的查询和table绑定
--默认值 继承_ajaxOption:
	VJ._queryOption={
		//一般用来指定不包含表头的表体部分。譬如 table tbody 标签
		node: null,
		//请求的固定参数地址
		url: '',
		//这里规定 是否跨域 如果是跨域那么使用的是 true 默认为 false
		jsonp: false,
		success: function(data, status) {
			//JQuery.ajax方法这里设定其默认操作用来调用一系列默认事件方法以方便用户进行具体上的业务更新和处理。
			try{		    
				var hasFalse = false;
				switch(typeof(data)){
					case "string":
						hasFalse = (data.indexOf('[False]')>=0 || data.indexOf('[false]')>=0)
						break;
					case "object":
						$(eval(data)).each(function(i,v){
							 hasFalse = (hasFalse || v == 'False' || v == 'false');
						});
						break;
					default:
						VJ.showException('VJ.Query success方法 name:typeof错误 type:'+typeof(data));
						break;
				}
				if(!hasFalse){
					this.bindData(this.node,this.filtData(eval(data)));
				}
			} catch(e) {
				VJ.showException('VJ.Query success方法',e);
			}
		}, filtData: function(data){	
			//新加方法 可以重载用来确定要处理的返回值，必须返回的是数组，第一个值为总数，第二个值为数据列表。 		
			return (data.length <= 3 && !(/^(\+|-)?\d+$/.test(data[1]))) ? [data[1][0].length, data[1][0]] : [data[1], data[2][0]];
		}, bindData: function(node,data){
			//新加方法 可以重载用来绑定Table实现对Table的填充。
			this.beforeGetRow(node,data); //清空Table
			if(VJ.isValid(data[1])){			
				var _this=this;			
				$(data[1]).each(function(i,v){
					var row = _this.getRow(node,i,v);
					if(VJ.isValid(row)){
						node.append(row);
					}
				});			
			}
			if(node.children().length==0){
				var row = this.getNoDataRow(node);
				if(VJ.isValid(row)){
					node.append(row);
				}
			}
			this.afterGetRow(node,data);		
		},getRow: function(node,i,v){
		//新加方法 用来生成行数据 数据不合法可以返回null 这时null不会被添加到html中
		return VJ.newEl("span","","").text('VJ测试');
		},getNoDataRow: function (node) {        
			//新加方法 可以用来处理无数据返回结果，包括全部结果经过getRow处理发现都不合法时。
			return null;
		},beforeGetRow: function(node,data){
		//新加方法 可以重载用来清除该表内容。
		node.empty();
		},afterGetRow: function(node,data){
		//新加方法 可以重载用来设置表隔行样式，或者触发某种事件.
		}
	};
--案例本域调用
var query=VJ.query({
    node: $("#table tbody"),
    url: "../../../layout/zh-cn/home/test.json",
    getRow:function(node,i,v){
        return VJ.newEl('tr','','');
    },getNoDataRow: function (node) {
        return null;
    },afterGetRow: function(node,data){
        node.children('tr:odd').addClass('g_tr_Odd');
        node.children('tr:even').addClass('g_tr_Even');
    }
});
query.bind({
    A:'gaga'
});
--跨域调用
var query=VJ.query({
    node: $("#table tbody"),
    url: RemoutUri.LCMS_getTest,
    jsonp:true, --特别的设置
    getRow:function(node,i,v){
        return VJ.newEl('tr','','');
    },getNoDataRow: function (node) {
        return null;
    },afterGetRow: function(node,data){
        node.children('tr:odd').addClass('g_tr_Odd');
        node.children('tr:even').addClass('g_tr_Even');
    }
});
query.bind({
A:'gaga'
});
*/
VJ.query = function(option) {
    if (VJ.isValid(option) && VJ.isValid(option.node) && VJ.isValid(option.url)) {
		if(option.getRow){
			return new _AteJ_query(option);
		} else {
			return new _AteJ_datagrid(option);
		}
    } else {
        return alert("错误：VJ.query()或者option.node对象或者option.url参数不能为空。");
    }
}
//xpagerquery类
//xpager可以同步更新Xpager的Query默认值 其中使用pager属性扩展了VJ.QueryOption而不影响其ajax调用。
function _AteJ_xpagerquery(option) {
    var _this = this;
    var query;
    var defaultOption;
    _this.count = 0;
    _this.pIndex = 0;
    var init = function(option) {
        defaultOption = $.extend({},VJ._ajaxOption, VJ._queryOption, VJ._xpagerQueryOption, option, {
            pager: $.extend({}, VJ._xpagerQueryOption.pager, option.pager)
        });
        var _beforeGetRow = defaultOption.beforeGetRow;
        $.extend(defaultOption,{
            beforeGetRow:function(node,data){
                _beforeGetRow(node,data);
                _this.count =  data[0];
            }
        });
        query = VJ.query(defaultOption);
		_this.node = defaultOption.node;
    };
    init(option);
//    var hasStart = false;
    _this.ListCallBack = function() {
        if (VJ.isValid(defaultOption.pager.xPagerListCallback)) {
            return defaultOption.pager.xPagerListCallback(_this.count);
        }
    };
    _this.InsertCallBack = function(isEnd) {
        if (VJ.isValid(defaultOption.pager.xPagerInsertCallback)) {
            defaultOption.pager.xPagerInsertCallback(isEnd);
        }
    };
    _this.DeleteCallBack = function() {
        if (VJ.isValid(defaultOption.pager.xPagerDeleteCallback)) {
            return defaultOption.pager.xPagerDeleteCallback();
        }
    };
    _this.bind = function(data,option) {
//        if (!hasStart) {
            window.setTimeout(function() {
                defaultOption.pager.node.xpager($.extend({}, defaultOption.pager, {
                    callback: function(pSize, pIndex, dlg1, dlg2, dlg3) {
                        _this.pIndex = pIndex;
                        defaultOption.pager.xPagerListCallback = dlg1;
                        defaultOption.pager.xPagerInsertCallback = dlg2;
                        defaultOption.pager.xPagerDeleteCallback = dlg3;
                        data = $.extend({}, VJ.getValue(data, {}), {
                            pageIndex: pIndex,
                            pageSize: pSize
                        });  
                        query.bind(data,option);
                    }
                }, VJ.isValid(data) && VJ.isValid(data.pIndex) ? { pIndex: data.pIndex} : {}));
            }, 100);
//        }
//        hasStart = true;
    };
}
/* 查询操作，支持跨域或者不跨域的查询和table绑定
--默认值 继承AteJ_queryOption:
	VJ._xpagerQueryOption = {
		pager:{
			node:null,
			pSizeValuSpace: [5, 10, 15],
			defaultPageSize: 10,
			xPagerListCallback: null,
			xPagerInsertCallback: null,
			xPagerDeleteCallback: null
		}, bindData: function(node,data){
			this.beforeGetRow(node,data); 
			if(VJ.isValid(data[1])){
				var _this=this;			
				$(data[1]).each(function(i,v){
					var row = _this.getRow(node,i,v);
					if(VJ.isValid(row)){
						node.append(row);
					}
				});			
			}
			if(VJ.isValid(this.pager.xPagerListCallback)){
				this.pager.xPagerListCallback(data[0]);
			}
			if(node.children().length==0){
				var row = this.getNoDataRow(node);
				if(VJ.isValid(row)){
					node.append(row);
				}
			}
			this.afterGetRow(node,data);					
		}
	};
--案例本域调用
var query=VJ.xpagerQuery({
    node: $("#table tbody"),
    url: RemoutUri.LCMS_getTest,
    jsonp:true, --特别的设置
    getRow:function(node,i,v){
        return VJ.newEl('tr','','');
    },getNoDataRow: function (node) {
        return null;
    },afterGetRow: function(node,data){
        node.children('tr:odd').addClass('g_tr_Odd');
        node.children('tr:even').addClass('g_tr_Even');
    },pager:{
        node: $("#xpager")
    }		
});
query.bind({
A:'gaga'
});
--跨域调用
var query=VJ.xpagerQuery({
    node: $("#table tbody"),
    url: RemoutUri.LCMS_getTest,
    jsonp:true, -- 这里特别的需要声明
    getRow:function(node,i,v){
        return VJ.newEl('tr','','');
    },getNoDataRow: function (node) {
        return null;
    },afterGetRow: function(node,data){
        node.children('tr:odd').addClass('g_tr_Odd');
        node.children('tr:even').addClass('g_tr_Even');
    },pager:{
        node: $("#xpager")
    }
});
query.bind({
    A:'gaga'
});
*/
VJ.xpagerQuery = function(option) {
    if (VJ.isValid(option.pager)) {
		if(option.getRow && option.pager.node){
			return new _AteJ_xpagerquery(option);
		} else {
			return new _AteJ_datagrid(option);
		}
    }
    else {
        return alert("错误：option.xpager对象参数不能为空。");
    }
}
/*
//对话框使用的默认参数
//对话框使用的默认参数
VJ._dialogOption = {
    WHStyle:1,
    closed:	true,
    title: "对话框标题",
    //只执行一次，且在beforeShow以前
    firstBeforeShow: function(div, data) { },
    //每次show时都执行，一般用于对open时输入的json数据进行处理，比如对引入的Dialog的table进行重新填充
    beforeShow: function(div, data) { },
    //每次show时都执行，一般用于每次Show自动发出一个通知
    afterShow: function(div, data) { },
    //只执行一次，且在afterShow之后，一般用于对Dialog进行样式上的设定，比如div.parent.addClass('g_Dialog')
    firstAfterShow: function(div, data) { },
    //处理窗口关闭问题
    onClose:function(event,ui){ }
};
*/
//VJ对话框类
function _AteJ_Dialog(option) {
    var _this = this;
    //记录本地的div对象
    var newdiv;
    //记录最终的iframe对象
    var newiframe;
    var defaultOption;
    //是否Dialog本地内容
    var isLocal = false;
    var hasLoad = false;
    var WHParam={};
    var init = function(option) {
        isLocal = VJ.isValid(option.node);
        if (isLocal && $(option.node).attr('nodeName').toLowerCase() == 'iframe') {
            //对于只有一个iframe的node对象 需要新建div并将iframe装入
            newdiv = VJ.newEl('div', '', '');
            newdiv.append($(option.node));
            newiframe = $(option.node);
            newdiv.appendTo($(document.body));
        } else {            
            newdiv = $(VJ.getValue(option.node, VJ.newEl('div', '', '')));
            if (VJ.getValue(option.mode, '') == 'iframe') {
                VJ.part('', newdiv, 'iframe');
                newiframe = $(newdiv.children('iframe:first')[0]);
            }
            if(!isLocal){
                newdiv.appendTo($(document.body));
            }
        }
        newdiv.attr('style', 'display:none;');
        switch (option.WHStyle){
            case 1:WHParam={width:300, height:200};break;
            case 2:WHParam={width:450, height:300};break;
            case 3:WHParam={width:650, height:500};break;
            case 4:WHParam={width:900, height:650};break;
            case 5:WHParam={width:300, height:360};break;
            default:WHParam={};break;        
        }
        defaultOption = $.extend({}, VJ._dialogOption, option,WHParam);
        if(VJ.isValid(defaultOption.url)){
            defaultOption.url+=(option.url.indexOf('?')>=0?'':'?')+"_r="+Math.random();
        }
		if(defaultOption.buttons && !defaultOption.buttons[0].text){
			//进行easyui转换
			var buttons = [];
			$(defaultOption.buttons).each(function(i,v){
				buttons[i] = {text:v,handler:defaultOption.buttons[v]};
			});
			defaultOption.buttons = buttons;
		}
		_this.node = newdiv;
    };
    init(option);
    //定义方法应对异步加载情况!
    var afterLoad = function(data) {
        if (!hasLoad) {
            newdiv.dialog(defaultOption);
            defaultOption.firstBeforeShow(newdiv, data);
        }
		if(!defaultOption.onBeforeOpen){
			newdiv.dialog('options').onBeforeOpen = function(){
				var ret = defaultOption.beforeShow(newdiv, data);
				return VJ.getValue(ret,true);
			};
		}		
        //defaultOption.beforeShow(newdiv, data);
        if(VJ.isValid(data) && VJ.isValid(data.Title)){
            //重新设置对话框的标题
            newdiv.dialog("setTitle",data.Title);            
        }
		newdiv.dialog("left",(screen.width - data.width) / 2);
		newdiv.dialog("top",(screen.height - data.height) / 2);
        newdiv.show().dialog("open");
        defaultOption.afterShow(newdiv, data);
        if (!hasLoad) {
            defaultOption.firstAfterShow(newdiv, data);
        }
        hasLoad = true;
    };
    /* 白冰 20110221日 取消!isLocal不能判断是否本地div，而使用url是否为空判断 && 针对只要是div,就load
             * 如果是本地 div 那么不load
               如果是本地 div iframe 那么load
               如果是本地 div 外部url 那么load
               如果是iframe 那么load
             */
    _this.open = function(data) {
        if (VJ.isValid(newiframe)) {
            var url = defaultOption.url + (VJ.isValid(data) ?  $.param(data) : '');
            //使用iframe的方式 只要src与要求的地址不符，那么就必须重新设定
            if (url != newiframe.attr('src')) {
                VJ.part(url, newiframe);
                afterLoad(data);
            }
        }else if (VJ.isValid(defaultOption.url) && !hasLoad){
            VJ.part(defaultOption.url, newdiv , null , function() {                    
                afterLoad(data);
            });
        }else{
            afterLoad(data);
        }
    };
    var close = _this.close = function() {
        newdiv.dialog("close");
        if (VJ.isValid(newiframe) && newiframe.attr('src').indexOf('?') >= 0) {
            newiframe.attr('src','');
        }
    };
}

/* 初始化dialog，返回值为_AteJ_Dialog实例或alert出错框
*-- 参数1：option  json 类型，不能为空
参数中的某些类型如果不填写则为默认值，如果填写那么会自动替换默认值生效，默认值如下
//对话框使用的默认参数
VJ._dialogOption={
	WHStyle:1,
	autoOpen: false,
	modal: true,
	draggable: true,
	overlay: {
		opacity: 0.5
	},
	title: "对话框标题",
	//只执行一次，且在beforeShow以前
	firstBeforeShow:function(div,data){},
	//每次show时都执行，一般用于对open时输入的json数据进行处理，比如对引入的Dialog的table进行重新填充
	beforeShow:function(div,data){},
	//每次show时都执行，一般用于每次Show自动发出一个通知
	afterShow:function(div,data){},
	//只执行一次，且在afterShow之后，一般用于对Dialog进行样式上的设定，比如div.parent.addClass('g_Dialog')
	firstAfterShow:function(div,data){},
	//处理窗口关闭问题
    onClose:function(event,ui){ }
};
可以在不同项目的Globle.js中更新更新成自己这个项目所使用的Dialog标准，一般不建议每个页面都自己设定一遍默认值。
*-- 案例：
VJ.dialog调用的三种方式:
1. VJ.dialog调用内容为本页面内但隐藏的对话框 那么json中必须定义node且指向一个$('')的对象.
2.（推荐）VJ.dialog调用内容为其它页面，但是是同域的且想通过load方式进行装载（装载进入的对话框及其JS代码可以直接与本页的JS相互调用，不使用Window.parent方式），那么json中必须定义url属性，可以定义node属性指向一个$('')的对象，也可以不定义.
3. VJ.dialog调用内容为其它页面，但是使用iframe方式进行装载和调用.可以
	a.	json中定义node为一个$('')对象，并指向一个iframe标签
	b.	json中不定义node，但是定义了url和mode:'iframe'这样的属性

var dia= VJ.dialog({
    node:$('#container'),
    WHStyle:3,
    title:"查看课程",
    buttons:{
        "取消":function(){
            dia.close();
        },
        "确定":function(){
        }        
    },
	//只执行一次，且在beforeShow以前
	firstBeforeShow:function(div,data){},
	//每次show时都执行，一般用于对open时输入的json数据进行处理，比如对引入的Dialog的table进行重新填充
	beforeShow:function(div,data){},
	//每次show时都执行，一般用于每次Show自动发出一个通知
	afterShow:function(div,data){},
	//只执行一次，且在afterShow之后，一般用于对Dialog进行样式上的设定，比如div.parent.addClass('g_Dialog')
	firstAfterShow:function(div,data){},
	//处理窗口关闭问题
    onClose:function(event,ui){ }
});
--调用方法：特别的:如果参数中设置Title:''将更新对话框的标题
dia.open({
    Title:新标题,
    a:2,
    b:3});
dia.open();
dia.close();
*/
VJ.dialog = function(option) {
    if (VJ.isValid(option)) {
        return new _AteJ_Dialog(option);
    } else {
        return alert("错误：VJ.Dialog() 参数不能为空。");
    }
};

VJ.tree = function(node,settings){
	settings=VJ.merge({},VJ._treeOption,settings,{
        caller:node
    });
	settings = $.extend(settings,{
		onClick:function(node2){
			//单击事件
			$(node).find('.g_tree_selected').removeClass('.g_tree_selected');
			$(node2).addClass('.g_tree_selected');
			settings.contextMenu.click(settings,node2);
		},
		onContextMenu:function(e,node2){
			if (settings.contextMenu.checkVisible('menu')) {
				if(!settings.contextMenu.node){
					settings.contextMenu.bindCallback(settings, node2, node);
				}
				//每次都调用
				//pageX/Y
				//screenX/Y
				settings.contextMenu.node.menu("show",{
					left: e.pageX,
					top: e.pageY
				});
			}			
		}
	});
    return node.tree(settings);
};

/*
VJ.ajax用于使用默认值
--默认状态
VJ._ajaxOption={
	async: false,
	type: "POST",
	dataType: "json",
	cache: false,
	beforeSend: function(request) {
	}, success: function(data, status) {
		try{
			if(data.indexOf('[False]')<0 || data.indexOf('[false]')<0){
				this.bindData(this.filtData(eval(data)));
			}
		} catch(e) {
			if(VJ.isDebug){alert('VJ._ajaxOption success方法 name:'+e.name+' error:'+e.message);}
		}
	}, error: function(request, status, error) {
		if(VJ.isDebug){alert('VJ._ajaxOption error方法 status:'+status+' error:'+error);}
	}, complete: function(request, status) {
	},  filtData: function(data){
	    //用来处理数据过滤的			
		return data[1][0];
	},  bindData: function(data){		
	    //这里使用的是过滤后的数据
	}
};
--案例
    VJ.ajax({
        url:"",
        data:{},
        //已经默认实现处理返回单值的数据，一般不用替换
        //filtData:function(data){
        //  return data[1][0];
        //},
        bindData:function(data){            
        }
    });
*/
VJ.ajax = function(data){
    $.ajax($.extend({},VJ._ajaxOption,data));
}
/* 20110316因为封装太烂而取消 datepicker.css未能抽象
VJ.datepicker用于统一日期数据格式
--默认状态
VJ._datepickerOption = {
    //年选择范围
    yearRangeScorp:40,
    //范围方向 min,max,both（范围*2）
    scorpOption:'min',
    firstDay: 0,
    dateFormat: 'yy-mm-dd',
    defaultDate: '-25y',
    hideIfNoPrevNext: true,
        beforeShowDay: function(date1) {
        var result = new Array();
        result[0] = date1.valueOf() > this.date.valueOf() ? false : true;
        return result;
    }
};
--案例
    VJ.datepicker({
        node:$('#')
    });
*/
//VJ.datepicker = function(option){
//    var option1 = $.extend({
//        date:new Date(),
//        node:null
//    },VJ._datepickerOption,option);
//    if(!VJ.isValid(option1.node)) {VJ.showException('警报','node节点设置为空，请检查!');return;}
//    var minScorp = ((option1.scorpOption=='min' || option1.scorpOption=='both')?(option1.yearRangeScorp/2):0);
//    var maxScorp = ((option1.scorpOption=='max' || option1.scorpOption=='both')?(option1.yearRangeScorp/2):0);
//    $(option1.node).datepicker($.extend(option,{
//        yearRange:((option1.date.getFullYear() - minScorp ) + ":" + (option1.date.getFullYear()+maxScorp)),
//        minDate: new Date(option1.date.getFullYear() - maxScorp,1,1),
//        maxDate: new Date(option1.date.getFullYear() + maxScorp,12,30),        
//        beforeShowDay: function(date1) {
//            var result = new Array();
//            result[0] = date1.valueOf() > option1.date.valueOf() ? false : true;
//            return result;
//        }
//    },option1));    
//}