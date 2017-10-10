/* 同步获取js模块 */
function _VJ_XmlDOM() { }
/*_VJ_XmlDOM内部方法
*-- 参数1：url， js或css的路径
*-- 参数2：获取方式get，post
*-- 参数3：post字符串
*-- 参数4：是否异步，true,false
*-- 参数5：回调方法
*/
_VJ_XmlDOM.prototype.create = function(URL, fun, pStr, isSyn, callBack) {
    //1为新浏览器，用XMLHttpRequest；2为IE5、6，用ActiveXObject("Microsoft.XMLHTTP")；3为本地（火狐除外，fox还会用type：1来读本地xml）
    this.type = null;
    this.responseObj = null;
    //
    this.xmlURL = URL || null;
    this.xmlFun = fun || "get";
    this.postStr = pStr || "";
    if (!this.xmlURL) return;
    //获取xmlReq对象
    this.xhReq = this.getXMLReq();
    if (this.xhReq == null) {
        alert("Your browser does not support XMLHTTP.");
        return;
    }
    //请求处理函数，分为异步和同步分别处理，同步处理需要放在“提交请求”后，负责无效
    //异步的回调处理
    if (isSyn && (isSyn == true || isSyn == "true") && this.type != 3) {
        //alert("异步")
        //指定响应函数
        this.xhReq.onreadystatechange = function() {
            if (this.readyState == 4 && (this.status == 200 || this.status == 0)) {
                if (callBack) {
                    callBack(this.responseXML.documentElement);
                }
                else
                    return this.responseXML.documentElement;
            }
        };
    }
    //提交请求
    //alert(this.type)
    if (this.type != 3) {
        this.xhReq.open(this.xmlFun, this.xmlURL, (isSyn && (isSyn == true || isSyn == "true")) ? true : false);
        this.xhReq.send((this.xmlFun && this.xmlFun.toLowerCase() == "post") ? this.postStr : null);
        this.responseObj = this.xhReq.responseText;
    }
    else if (this.type == 3)//这是IE用来读取本地xml的方法
    {
        this.xhReq.open("get", this.xmlURL, "false");
        this.responseObj = this.xhReq;
    }
    //同步的回调处理
    if ((isSyn != null && (isSyn == false || isSyn == "false")) || this.type == 3) {
        if (callBack)
            callBack(this.responseObj);
        else
            return this.responseObj;
    }
}
/*获取DOM对象兼容各个浏览器，可能不完善，继续测试
*/
_VJ_XmlDOM.prototype.getXMLReq = function() {
    var xmlhttp = null;
    if (window.XMLHttpRequest) {	// code for all new browsers like IE7/8 & FF
        xmlhttp = new XMLHttpRequest();
        this.type = 1;
    }
    else if (window.ActiveXObject) {	// code for IE5 and IE6
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        this.type = 2;
    }
    //如果读取本地文件，则使用AXObject，因为httpRequest读取本地文件会报拒绝访问
    if (document.location.href.indexOf("http://") < 0 && window.ActiveXObject) {
        xmlhttp = new ActiveXObject("Microsoft._VJ_XmlDOM");
        this.type = 3;
    }
    return xmlhttp;
}
/*请求失败
*/
_VJ_XmlDOM.prototype.abort = function() {
    this.xhReq.abort();
}
/*获取js代码后，添加到页面内容下
*/
function _VJ_AppendScript(data,callback) {
    var ua = navigator.userAgent.toLowerCase();
    isOpera = ua.indexOf("opera") > -1
    isIE = !isOpera && ua.indexOf("msie") > -1
    var head = document.getElementsByTagName("head")[0] || document.documentElement, script = document.createElement("script");
    script.type = "text/javascript";
    if (isIE) script.text = data;
    else script.appendChild(document.createTextNode(data));
    // Use insertBefore instead of appendChild  to circumvent an IE6 bug.
    // This arises when a base node is used (#2709).
    var done = false;
	script.type = 'text/javascript';
	script.language = 'javascript';
	script.src = url;
	script.onload = script.onreadystatechange = function(){
		if (!done && (!script.readyState || script.readyState == 'loaded' || script.readyState == 'complete')){
			done = true;
			script.onload = script.onreadystatechange = null;
			if (callback){
				callback.call(script);
			}
		}
	}
    document.getElementsByTagName("head")[0].appendChild(script);

    head.insertBefore(script, head.firstChild);
    head.removeChild(script);
}

VJ = {};

VJ._include = {};
/* 添加 js 和 css 引用
*-- 参数1：url， js或css的路径
*-- 参数2：tag， 标签名称'head'或'body' ，可以为空，默认加在'head'内
*-- 案例：VJ.include("script/jquery1.3/ui.core.js");
*/
VJ.include = function(url, tag, callback) {
//如果已经使用本方法加载过 就不再加载。
    if(VJ._include[url]) return;
    if (tag == null) { tag = 'head'; }
    var parentNode = document.getElementsByTagName(tag).item(0);
    var s = url.split('.');
    var styleTag = s[s.length - 1].toLowerCase();
    if (styleTag == "js") {
        var thisJsDom = new _VJ_XmlDOM();
        thisJsDom.create(url, "get", null, false, function(data) {
            _VJ_AppendScript(data,callback)
        });
    }

    
    if (styleTag == "css") {
        new_element = document.createElement("link");
        new_element.setAttribute("type", "text/css");
        new_element.setAttribute("rel", "stylesheet");
        new_element.setAttribute("href", url);
        new_element.setAttribute("media", 'screen');
        parentNode.appendChild(new_element);
    }
    VJ._include[url] = true;
};
//用于数组，对象的深度合并功能。moveIndex属性用于设定移动至的位置，mergeIndex只用于合并数组中的第几个对象 需要进入reference
//例如
//var ret = VJ.merge({a:22,c:23},{a:34,b:33},{d:"2334",f:true,g:function(){alert("hahaha");}},{h:[1,2,3,4]});
//var ret = VJ.merge({a:[{a:2},{b:3}]},{a:[{moveIndex:3,j:3},{k:4}],b:25});
//var ret = VJ.merge({a:[{a:2},{b:3}]},{a:[{mergeIndex:3,j:3},{k:4}],b:25});
VJ.merge = function(){
	var _merge = function(aim,source){
		if(!(typeof(source)=='object' && typeof(source.length) == 'undefined')) {return aim;}
		for(var i in source){
			if(source[i]){
				if(!aim[i]) {
					aim[i]=source[i];
				} else {
					if(typeof(aim[i])=='object'){
						if(typeof(aim[i].length) == 'undefined'){
							//处理对象
							_merge(aim[i],source[i]);
						} else {
							//处理数组
							var hasmergeIndex = false;
							for(var i3 = 0,k=source[i][i3];i3<source[i].length;i3++,k=source[i][i3]){
								if(typeof(k.mergeIndex)=="number"){
									hasmergeIndex = true;
									if(aim[i].length<(i3+1)) {
										aim[i].push(k);
									} else {
										aim[i][i3] = _merge(aim[i][i3],k);
									}
								} else if(typeof(k.moveIndex)=="number"){
									hasmergeIndex = true;
									aim[i].splice(k.moveIndex,0,k);
								} 
							}
							if(!hasmergeIndex){
								aim[i]=source[i];
							}
						}
					} else {
						aim[i]=source[i];
					}
				}
			}
		}
		return aim;
	};
	if(arguments.length<2) {return arguments[0]?arguments[0]:{}};
	var _ = {};
	for(var i2 = 0;i2<arguments.length;i2++)
		_=_merge(_,arguments[i2]);
	return _;
};

//用于弹出对整个对象的遍历内容
VJ.alertAll = function(p,s){
	for(var ii in s){
		if(typeof(s[ii])=="object"){
			if(typeof(s[ii].length) == 'undefined'){
				VJ.alertAll(p+":"+ii,s[ii]);
			} else {
				for(var i3 = 0,k=s[ii][i3];i3<s[ii].length;i3++,k=s[ii][i3]){
					if(typeof(k)=="object"){
						VJ.alertAll(p+":"+ii+":Array("+i3+")",k);
					} else alert(p+":"+ii+":Array("+i3+"):"+k);
				}						
			}
		} else { alert(p+":"+ii+":"+s[ii]);}
	}
};
//这里标注Bug开关为False
VJ.isDebug = false;
VJ.showException = function(name, e) {
    if (VJ.isDebug) {
        var content = name;
        if (VJ.isValid(e)) {
            content += ("\r\nname:" + e.name + "\r\nmessage:" + e.message + (VJ.userAgent.firefox ? ("\r\nstack:" + e.stack + "\r\nlineNumber:" + e.lineNumber) : ""));
        }
        alert(content);
    }
};
VJ.extend = function(name, data) {
    eval('VJ.merge(VJ._' + name + 'Option,data);');
}

/* 生成新元素
*-- 参数1：tag 标签
*-- 参数2：样式class
*-- 参数3：标签内内容
*-- 案例：VJ.newEl("div","divClass","我的div");
*/
VJ.newEl = function(tag, style, txt) {
    var elm = $(document.createElement(tag));
    if (txt != "") {
        elm.text(txt);
    }
    if (style != "") {
        elm.addClass(style);
    }
    return elm;
};

VJ._loaderOption = {
	modules:{
		draggable:{
			js:'jquery.draggable.js'
		},
		droppable:{
			js:'jquery.droppable.js'
		},
		resizable:{
			js:'jquery.resizable.js'
		},
		linkbutton:{
			js:'jquery.linkbutton.js',
			css:'linkbutton.css'
		},
		progressbar:{
			js:'jquery.progressbar.js',
			css:'progressbar.css'
		},
		pagination:{
			js:'jquery.pagination.js',
			css:'pagination.css',
			dependencies:['linkbutton']
		},
		datagrid:{
			js:'jquery.datagrid.js',
			css:'datagrid.css',
			dependencies:['panel','resizable','linkbutton','pagination']
		},
		treegrid:{
			js:'jquery.treegrid.js',
			css:'tree.css',
			dependencies:['datagrid']
		},
		propertygrid:{
			js:'jquery.propertygrid.js',
			css:'propertygrid.css',
			dependencies:['datagrid']
		},
		panel: {
			js:'jquery.panel.js',
			css:'panel.css'
		},
		window:{
			js:'jquery.window.js',
			css:'window.css',
			dependencies:['resizable','draggable','panel']
		},
		dialog:{
			js:'jquery.dialog.js',
			css:'dialog.css',
			dependencies:['linkbutton','window']
		},
		messager:{
			js:'jquery.messager.js',
			css:'messager.css',
			dependencies:['linkbutton','window','progressbar']
		},
		layout:{
			js:'jquery.layout.js',
			css:'layout.css',
			dependencies:['resizable','panel']
		},
		form:{
			js:'jquery.form.js'
		},
		menu:{
			js:'jquery.menu.js',
			css:'menu.css'
		},
		tabs:{
			js:'jquery.tabs.js',
			css:'tabs.css',
			dependencies:['panel','linkbutton']
		},
		splitbutton:{
			js:'jquery.splitbutton.js',
			css:'splitbutton.css',
			dependencies:['linkbutton','menu']
		},
		menubutton:{
			js:'jquery.menubutton.js',
			css:'menubutton.css',
			dependencies:['linkbutton','menu']
		},
		accordion:{
			js:'jquery.accordion.js',
			css:'accordion.css',
			dependencies:['panel']
		},
		calendar:{
			js:'jquery.calendar.js',
			css:'calendar.css'
		},
		combo:{
			js:'jquery.combo.js',
			css:'combo.css',
			dependencies:['panel','validatebox']
		},
		combobox:{
			js:'jquery.combobox.js',
			css:'combobox.css',
			dependencies:['combo']
		},
		combotree:{
			js:'jquery.combotree.js',
			dependencies:['combo','tree']
		},
		combogrid:{
			js:'jquery.combogrid.js',
			dependencies:['combo','datagrid']
		},
		validatebox:{
			js:'jquery.validatebox.js',
			css:'validatebox.css'
		},
		numberbox:{
			js:'jquery.numberbox.js',
			dependencies:['validatebox']
		},
		searchbox:{
			js:'jquery.searchbox.js',
			css:'searchbox.css',
			dependencies:['menubutton']
		},
		spinner:{
			js:'jquery.spinner.js',
			css:'spinner.css',
			dependencies:['validatebox']
		},
		numberspinner:{
			js:'jquery.numberspinner.js',
			dependencies:['spinner','numberbox']
		},
		timespinner:{
			js:'jquery.timespinner.js',
			dependencies:['spinner']
		},
		tree:{
			js:'jquery.tree.js',
			css:'tree.css',
			dependencies:['draggable','droppable']
		},
		datebox:{
			js:'jquery.datebox.js',
			css:'datebox.css',
			dependencies:['calendar','combo']
		},
		datetimebox:{
			js:'jquery.datetimebox.js',
			dependencies:['datebox','timespinner']
		},
		slider:{
			js:'jquery.slider.js',
			dependencies:['draggable']
		},
		parser:{
			js:'jquery.parser.js'
		}
	},
    locales :{
		'af':'easyui-lang-af.js',
		'bg':'easyui-lang-bg.js',
		'ca':'easyui-lang-ca.js',
		'cs':'easyui-lang-cs.js',
		'cz':'easyui-lang-cz.js',
		'da':'easyui-lang-da.js',
		'de':'easyui-lang-de.js',
		'en':'easyui-lang-en.js',
		'es':'easyui-lang-es.js',
		'fr':'easyui-lang-fr.js',
		'it':'easyui-lang-it.js',
		'nl':'easyui-lang-nl.js',
		'pt_BR':'easyui-lang-pt_BR.js',
		'ru':'easyui-lang-ru.js',
		'tr':'easyui-lang-tr.js',
		'zh_CN':'easyui-lang-zh_CN.js',
		'zh_TW':'easyui-lang-zh_TW.js'
	}
};
/**
 * VJ._loaderOption - jQuery EasyUI
 * 
 * Copyright (c) 2009-2013 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact us: jeasyui@gmail.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 * 
 */
(function(){	
	var queues = {};	
	function loadJs(url, callback){
		/*		
		var script = document.createElement('script');
		script.type = 'text/javascript';
		script.language = 'javascript';
		script.src = url;
		script.onload = script.onreadystatechange = function(){
			if (!done && (!script.readyState || script.readyState == 'loaded' || script.readyState == 'complete')){
				done = true;
				script.onload = script.onreadystatechange = null;
				if (callback){
					callback.call(script);
				}
			}
		}
        document.getElementsByTagName("head")[0].appendChild(script);
		*/
		VJ.include(url,"head",callback);
	}
	
	function runJs(url, callback){
		loadJs(url, function(){
			document.getElementsByTagName("head")[0].removeChild(this);
			if (callback){
				callback();
			}
		});
	}
	
	function loadCss(url, callback){
	/*
		var link = document.createElement('link');
		link.rel = 'stylesheet';
		link.type = 'text/css';
		link.media = 'screen';
		link.href = url;
		document.getElementsByTagName('head')[0].appendChild(link);
	*/
		VJ.include(url,'head');
		if (callback){
			callback.call(link);
		}
	}
	
	function loadSingle(name, callback){
		queues[name] = 'loading';
		
		var module = VJ._loaderOption.modules[name];
		var jsStatus = 'loading';
		var cssStatus = (VJ.loader.css && module['css']) ? 'loading' : 'loaded';
		
		if (VJ.loader.css && module['css']){
			if (/^http/i.test(module['css'])){
				var url = module['css'];
			} else {
				var url = VJ.loader.base + 'themes/' + VJ.loader.theme + '/' + module['css'];
			}
			loadCss(url, function(){
				cssStatus = 'loaded';
				if (jsStatus == 'loaded' && cssStatus == 'loaded'){
					finish();
				}
			});
		}
		
		if (/^http/i.test(module['js'])){
			var url = module['js'];
		} else {
			var url = VJ.loader.base + 'plugins/' + module['js'];
		}
		loadJs(url, function(){
			jsStatus = 'loaded';
			if (jsStatus == 'loaded' && cssStatus == 'loaded'){
				finish();
			}
		});
		
		function finish(){
			queues[name] = 'loaded';
			VJ.loader.onProgress(name);
			if (callback){
				callback();
			}
		}
	}
	
	function loadModule(name, callback){
		var mm = [];
		var doLoad = false;
		
		if (typeof name == 'string'){
			add(name);
		} else {
			for(var i=0; i<name.length; i++){
				add(name[i]);
			}
		}
		
		function add(name){
			if (!VJ._loaderOption.modules[name]) return;
			var d = VJ._loaderOption.modules[name]['dependencies'];
			if (d){
				for(var i=0; i<d.length; i++){
					add(d[i]);
				}
			}
			mm.push(name);
		}
		
		function finish(){
			if (callback){
				callback();
			}
			VJ.loader.onLoad(name);
		}
		
		var time = 0;
		function loadMm(){
			if (mm.length){
				var m = mm[0];	// the first module
				if (!queues[m]){
					doLoad = true;
					loadSingle(m, function(){
						mm.shift();
						loadMm();
					});
				} else if (queues[m] == 'loaded'){
					mm.shift();
					loadMm();
				} else {
					if (time < VJ.loader.timeout){
						time += 10;
						setTimeout(arguments.callee, 10);
					}
				}
			} else {
				if (VJ.loader.locale && doLoad == true && VJ.loader.locales[VJ.loader.locale]){
					var url = VJ.loader.base + 'locale/' + VJ.loader.locales[VJ.loader.locale];
					runJs(url, function(){
						finish();
					});
				} else {
					finish();
				}
			}
		}
		
		loadMm();
	}
	
	VJ.loader = VJ.merge({
		base:'.',
		theme:'default',
		css:true,
		locale:null,
		timeout:2000,
	
		load: function(name, callback){
			if (/\.css$/i.test(name)){
				if (/^http/i.test(name)){
					loadCss(name, callback);
				} else {
					loadCss(VJ.loader.base + name, callback);
				}
			} else if (/\.js$/i.test(name)){
				if (/^http/i.test(name)){
					loadJs(name, callback);
				} else {
					loadJs(VJ.loader.base + name, callback);
				}
			} else {
				loadModule(name, callback);
			}
		},
		
		onProgress: function(name){},
		onLoad: function(name){}
	});

	var scripts = document.getElementsByTagName('script');
	for(var i=0; i<scripts.length; i++){
		var src = scripts[i].src;
		if (!src) continue;
		var m = src.match(/VJ.loader\.js(\W|$)/i);
		if (m){
			VJ.loader.base = src.substring(0, m.index);
		}
	}

	window.using = VJ.loader.load;
	
	if (window.jQuery){
		jQuery(function(){
			VJ.loader.load('parser', function(){
				jQuery.parser.parse();
			});
		});
	}
	
})();

//控件打开
VJ._windowOption = {
	node:null,	
    WHStyle:1,
	title:'新窗口',
	collapsible:false,
	minimizable:false,
	maximizable:false,
	closable:true,
	closed:true,
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
// 控件
VJ._panelOption = {};

VJ._openWindowOption = {
    toolbar: 'no',
    location: 'yes',
    menubar: 'no',
    resizable: 'yes',
    scrollbars: 'yes',
    status: 'no'
};
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

//VJ获取远程JSON的默认参数
VJ._getRemoteJSONOption = {
    filtURI: function(url) { return url; }
    /*	Global.js中可以定义为
    if ($.cookie("ssic") != null) {
    if (url.indexOf("?") > -1) {
    url += "&c=" + encodeURIComponent($.cookie("ssic"));
    } else {
    url += "?c=" + encodeURIComponent($.cookie("ssic"));
    }
    }
    */
};
//就是为了少写点
VJ._ajaxOption = {
    async: false,
    type: "POST",
    dataType: "dataString",
    cache: false,
    beforeSend: function(request) {
    }, success: function(data, status) {
        try {
            var hasFalse = false;
            switch (typeof (data)) {
                case "string":
                    hasFalse = (data.indexOf('[False]') >= 0 || data.indexOf('[false]') >= 0)
                    break;
                case "object":
                    $(eval(data)).each(function(i, v) {
                        hasFalse = (hasFalse || v == 'False' || v == 'false');
                    });
                    break;
                default:
                    VJ.showException('VJ.Query success方法 name:typeof错误 type:' + typeof (data));
                    break;
            }
            if (!hasFalse) {
                this.bindData(this.filtData(eval(data)));
            }
        } catch (e) {
            VJ.showException('VJ._ajaxOption success方法', e);
        }
    }, error: function(request, status, error) {
        VJ.showException('VJ._ajaxOption error方法 status:' + status, error);
    }, complete: function(request, status) {
    }, filtData: function(data) {
        //用来处理数据过滤的			
        return data[1][0];
    }, bindData: function(data) {
        //这里使用的是过滤后的数据
    }
};
//查询类默认值
VJ._queryOption = {
    //一般用来指定不包含表头的表体部分。譬如 table tbody 标签
    node: null,
    //请求的固定参数地址
    url: '',
    //这里规定 是否跨域 如果是跨域那么使用的是 true 默认为 false
    jsonp: false,
    success: function(data, status) {
        //JQuery.ajax方法这里设定其默认操作用来调用一系列默认事件方法以方便用户进行具体上的业务更新和处理。
        try {
            var hasFalse = false;
            switch (typeof (data)) {
                case "string":
                    hasFalse = (data.indexOf('[False]') >= 0 || data.indexOf('[false]') >= 0)
                    break;
                case "object":
                    $(eval(data)).each(function(i, v) {
                        hasFalse = (hasFalse || v == 'False' || v == 'false');
                    });
                    break;
                default:
                    VJ.showException('VJ.Query success方法 name:typeof错误 type:' + typeof (data));
                    break;
            }
            if (!hasFalse) {
                this.bindData(this.node, this.filtData(eval(data)));
            }
        } catch (e) {
            VJ.showException('VJ.Query success方法', e);
        }
    }, filtData: function(data) {
        //新加方法 可以重载用来确定要处理的返回值，必须返回的是数组，第一个值为总数，第二个值为数据列表。 
		//TODO 如果是wi实现那么最后一句需要使用 data = (data.length <= 3 && !(/^(\+|-)?\d+$/.test(data[1]))) ? [data[1][0].length, data[1][0]] : [data[1][0][data[1][0][0]], data[2][0]];
        data = (data.length <= 3 && !(/^(\+|-)?\d+$/.test(data[1]))) ? [data[1][0].length, data[1][0]] : [data[1], data[2][0]];
		//20121018 baibing 使用TJSON时使用
		$(data[1]).each(function(i,v){
			data[1][i] = VJ.evalTJson(v);
		})
		return data;
    }, bindData: function(node, data) {
        //新加方法 可以重载用来绑定Table实现对Table的填充。
        this.beforeGetRow(node, data); //清空Table
        if (VJ.isValid(data[1])) {
            var _this = this;
            $(data[1]).each(function(i, v) {
                var row = _this.getRow(node, i, v);
                if (VJ.isValid(row)) {
                    node.append(row);
                }
            });
        }
        if (node.children().length == 0) {
            var row = this.getNoDataRow(node);
            if (VJ.isValid(row)) {
                node.append(row);
            }
        }
        this.afterGetRow(node, data);
    }, getRow: function(node, i, v) {
        //新加方法 用来生成行数据 数据不合法可以返回null 这时null不会被添加到html中
        return VJ.newEl("span", "", "").text('VJ测试');
    }, getNoDataRow: function(node) {
        //新加方法 可以用来处理无数据返回结果，包括全部结果经过getRow处理发现都不合法时。
        return null;
    }, beforeGetRow: function(node, data) {
        //新加方法 可以重载用来清除该表内容。
        node.empty();
    }, afterGetRow: function(node, data) {
        //新加方法 可以重载用来设置表隔行样式，或者触发某种事件.
    }
};
//xpager可以同步更新Xpager的Query默认值 其中使用pager属性扩展了VJ.QueryOption而不影响其ajax调用。
VJ._xpagerQueryOption = {
    pager: {
        node: null,
        pSizeValuSpace: [5, 10, 15],
        defaultPageSize: 10,
        xPagerListCallback: null,
        xPagerInsertCallback: null,
        xPagerDeleteCallback: null
    }, bindData: function(node, data) {
        this.beforeGetRow(node, data);
        if (VJ.isValid(data[1])) {
            var _this = this;
            $(data[1]).each(function(i, v) {
                var row = _this.getRow(node, i, v);
                if (VJ.isValid(row)) {
                    node.append(row);
                }
            });
        }
        if (VJ.isValid(this.pager.xPagerListCallback)) {
            this.pager.xPagerListCallback(data[0]);
        }
        if (node.children().length == 0) {
            var row = this.getNoDataRow(node);
            if (VJ.isValid(row)) {
                node.append(row);
            }
        }
        this.afterGetRow(node, data);
    }
};

//xpager可以同步更新Xpager的Query默认值 其中使用pager属性扩展了VJ.QueryOption而不影响其ajax调用。
VJ._datagridOption = $.extend({},VJ._xpagerQueryOption,{
	beforeGetRow: function(node, data) {
		//这里一般进行 列数据判断
    }
});

function _VJ_QueryString(qs) { // optionally pass a querystring to parse
    this.params = {};

    if (qs == null) qs = location.search.substring(1, location.search.length);
    if (qs.length == 0) return;

    // Turn <plus> back to <space>
    // See: http://www.w3.org/TR/REC-html40/interact/forms.html#h-17.13.4.1
    qs = qs.replace(/\+/g, ' ');
    var args = qs.split('&'); // parse out name/value pairs separated via &

    // split out each name=value pair
    for (var i = 0; i < args.length; i++) {
        var pair = args[i].split('=');
        var name = decodeURIComponent(pair[0]);

        var value = (pair.length == 2)
			? decodeURIComponent(pair[1])
			: name;

        this.params[name] = value;
    }
    //等同_VJ_QueryString.prototype.get
    this.get = function(key, default_) {
        var value = this.params[key];
        return (value != null) ? value : default_;
    }
    this.contains = function(key) {
        var value = this.params[key];
        return (value != null);
    }
}
VJ.qs = new _VJ_QueryString();
VJ._treeOption = {
	animate:true,
	//用于获得Ajax数据
    getAjaxData: function(root) {
		//todo
        return { ParentID: (root == 'source' ? '0' : root) };
    },
	onBeforeEdit:function(node){
		this.editNode = VJ.extend({},node);
	},
	//自定义处理事件
	valid:function(args,text){
		if(this.form) { this.form.remove();}
		this.form = VJ.newEl('form','','').hide().appendTo($(document.body));
		this.formInput = VJ.newEl('input',"","").attr('type','text').attr('name','_txtVal').val(text).appendTo(this.form);
		this.form.validate({
			rules:{
				_txtVal:$.extend(args,{required: true})
			}
			//todo regex
		});
		return !this.form.validate()?this.formInput.attr('title'):"";
	},
	onAfterEdit:function(node){
		var _this = this;
		var args = {};
		node.text = $.trim(node.text).replace(/[\u00c2|\u00a0|]/g,'')
		//需要确定是哪个状态
		switch (_this.menuValue) {
			case "edit":
				args = _this.contextMenu.editNode(_this,node,function(){
					//此时编辑操作已经完成
				});
				break;
			case "append":
			case "add":
				node.val = function(){return this.text;};
				//这里处理的时候已经分别定义了
				args = _this.editCallback(_this,node,function(lable, txtBox, oldValue, id){
					//此时编辑操作已经完成 但是未更新id;
					_this.caller.tree('update',$.extend(node,{id:id,text:txtBox.val()}));
				});
				break;
			default:
				VJ.showException("VJ._treeOption.onAfterEdit错误：未知的菜单选项"+_this.menuValue);
			return;
		}		
		var text = _this.valid(args,node.text);
		if(this.args.length>1 && text){
			_this.caller.tree('update',$.extend(node,{text:text}));
			_this.caller.tree('beginEdit',node.target);
			return;
		} else {
			if(args.callback) args.callback(undefined,node,_this.editNodeText);
		}
		_this.caller.tree('select',node);
		
		//重新设置完成初始化信息
		_this.editNodeText = undefined;
		_this.menuValue = undefined;
		_this.editCallback = undefined;
	},
	onCancelEdit:function(node){
		var _this = this;
		//需要确定是哪个状态
		switch (_this.menuValue) {
			case "append":
				_this.caller.tree('pop',node.target);
				break;
			case "add":
				_this.caller.tree('pop',node.target);
				break;
			default:
				VJ.showException("VJ._treeOption.onCancelEdit错误：未知的菜单选项"+_this.menuValue);
				return;
		}		
		
		//重新设置完成初始化信息
		_this.editNodeText = undefined;
		_this.menuValue = undefined;
		_this.editCallback = undefined;
	},
	//用于生成节点原型信息
	getNodeDefData:function(){
		return {
			id:'',
			text:'',
			state:'open' //checked：指示节点是否被选中。 Indicate whether the node is checked selected.
			attributes:[],
			children:[]
		};
	},
    contextMenu: {
        id: null,
        createMenuID: function() {
            return Math.round(Math.random() * 100000000);
        },
        //根据menu,add,append,edit,del设定是否显示
        checkVisible: function(operate) { return true; },
        //可以设置为'组织机构'等
        menuItemName: '',
        //新建菜单UL
        createMenu: function(settings) {
            var newul = VJ.newEl('div', '', '').attr("id", settings.contextMenu.id).attr("display", 'none').css('width:120px').addClass('easyui-menu');
            if (settings.contextMenu.checkVisible('edit')) {
                var newdiv = VJ.newEl('div', '', '重命名').attr('menuvalue','edit').attr('iconCls',"icon-edit");
                newul.append(newdiv);
            }
            if (settings.contextMenu.checkVisible('append')) {
				//todo icon-append 没有
				var newdiv = VJ.newEl('div', '', '新建下级' + settings.contextMenu.menuItemName).attr('menuvalue','append').attr('iconCls',"icon-append");
                newul.append(newdiv);
            }
            if (settings.contextMenu.checkVisible('add')) {
				var newdiv = VJ.newEl('div', '', '新建同级' + settings.contextMenu.menuItemName).attr('menuvalue','add').attr('iconCls',"icon-add");
                newul.append(newdiv);
            }
            if (settings.contextMenu.checkVisible('del')) {
				var newdiv = VJ.newEl('div', '', '删除').attr('menuvalue','del').attr('iconCls',"icon-del");
                newul.append(newdiv);
            };
            if(newul.find('div').length == 0)
                return null;
            else
                return newul;
        },
		//可以直接获得li对象
        click: function(settings, node) {
        },
        bindCallback: function(settings, node, container) {
            if (!settings.contextMenu.checkVisible('menu')) return;            
            if (!VJ.isValid(settings.contextMenu.id)) {
                settings.contextMenu.id = settings.contextMenu.createMenuID();
                var me = settings.contextMenu.createMenu(settings);
                if(me == null)
                    return;
                else {
                    $(document.body).append(me);
					settings.contextMenu.node = me;
					//todo 定义me menu
					settings.contextMenu.node.menu({
						onClick:function(item){
							try {
								//定义菜单的当前选项
								var current = $(item.target); //.find('.selected').parent();
								settings.menuValue = current.attr('menuvalue');
								settings.editNodeText = '';
								switch (current.attr('menuvalue')) {
									case "edit":
										settings.editNodeText = node.text;
										settings.caller.tree("beginEdit",node.target);										
										break;
									case "append":
										settings.caller.tree("append",{
												parent: node.target,
												data:[settings.getNodeDefData()]
											}
										);
										var node2 = settings.caller.tree('getChildren',node.target);
										if(node2.length==0){
											VJ.showException('VJ._treeOption.contextMenu.bindCallback append方法错误:'+node.text+'节点下新增节点失败!');
											return;
										}
										settings.contextMenu._addNode(settings, node2[0], container, settings.contextMenu.appendNode);
										break;
									case "add":
										//默认放置在下面
										settings.caller.tree("insert",{
												after: node.target,
												data:[settings.getNodeDefData()]
											}
										);
										//找到父节点下的新增子节点
										var node2 = settings.caller.tree('getParent',node.target);
										var isNext = false;
										$(settings.caller.tree('getChildren',node2?node2.target:null)).each(function(i,v){
											//这里可能有问题
											if(v.id == node.id) {isNext = true;}
											else if(isNext){
												node2 = v;
												break;
											}
										});
										if(!isNext){
											VJ.showException('VJ._treeOption.contextMenu.bindCallback add方法错误:'+node.text+'节点下添加下级节点失败!');
											return;
										}
										settings.contextMenu._addNode(settings, node2, container, settings.contextMenu.addNode);
										break;
									case "del":
										var args = settings.contextMenu.delNode(settings, node);
										args.callback(function(){
											settings.caller.tree('remove',node.target);
										});
										break;
									default:
										settings.contextMenu.actionNode(settings,node,current.attr('menuvalue'));				                
										break;
								}
							} catch (e) {
								VJ.showException('VJ._treeOption.contextMenu.bindCallback', e);
							}
						}
					});
				}
            }
        },
        //这里返回一个JSON对象作为treeview edit操作的args参数以处理该修改是否通过，及其校验工作，默认修改都通过。
        editNode: function(settings, node, editCallBack) {
            return {
                callback: function(lable, txtBox, oldValue) {
                    VJ.showException('VJ._treeOption.contextMenu.editNode 值由:\'' + oldValue + '\'换为\'' + txtBox.val() + '\'');
                    editCallBack(lable, txtBox, oldValue);
                    return;
                }
            };
        },
        //这里返回一个JSON对象作为treeview append操作的edit args参数以处理该修改是否通过，及其校验工作，默认修改都通过。特别的当这里和AddNode使用alert时，容易出现失去焦点事件的2次触发，导致本方法被两次调用
        appendNode: function(settings, node, appendCallBack) {
            return {
                callback: function(lable, txtBox, oldValue) {
                    // 这里很重要，该节点的编号	
                    VJ.showException('VJ._treeOption.contextMenu.appendNode 值由:\'' + oldValue + '\'换为\'' + txtBox.val() + '\'');
                    var id = '';
                    appendCallBack(lable, txtBox, oldValue, id);
                    return;
                }
            };
        },
        //这里返回一个JSON对象作为treeview edit操作的args参数以处理该修改是否通过，及其校验工作，默认修改都通过。
        addNode: function(settings, node, addCallBack) {
            return settings.contextMenu.appendNode(settings, node, addCallBack);
        },
        _addNode: function(settings, node, container, func) {
			settings.editCallback = func;
			//这里可能会出问题
			settings.caller.tree("beginEdit",node.target);
        },
        //这里返回一个JSON对象作为treeview del操作的args参数以处理该修改是否通过，及其校验工作，默认修改都通过。
        delNode: function(settings, node) {
            return {
                callback: function(removeCallBack) {
                    // 这里很重要，该节点的编号	
                    VJ.showException('VJ._treeOption.contextMenu.appendNode 值由:\'' + oldValue + '\'换为\'' + txtBox.val() + '\'');
                    removeCallBack();
                }
            };
        },
        //这里用来处理自定义的菜单项 action为菜单项名，node为当前节点。
        actionNode:function(settings,node,action){
            VJ.showException('VJ._treeOption.contextMenu.bindCallback 未处理 Option:' + action);
        }
    }
};
//VJ._datepickerOption = {
//    //年选择范围
//    yearRangeScorp:40,
//    //范围方向 min,max,both（范围*2）
//    scorpOption:'min',
//    firstDay: 0,
//    dateFormat: 'yy-mm-dd',
//    defaultDate: '-25y',
//    hideIfNoPrevNext: true
//};

///对字符串进行编码，解决字符串中包含特殊字符被IIS组织的问题
VJ.encHtml = function(html) {
    //20120328 白冰 只转换标点符号!    
    //return encodeURIComponent(VJ.getValue(html, '').replace(/\r\n/g, ''));
    return (VJ.getValue(html, '').replace(/\r\n/g, '')).replace(new RegExp('~|!|@|#|\\$|%|\\^|;|\\*|\\(|\\)|_|\\+|\\{|\\}|\\||:|\"|\\?|`|\\-|=|\\[|\\]|\\\|;|\'|,|\\.|/|，|；','g'),function(a){return encodeURIComponent(a);});
};
///对字符串进行解码
VJ.decHtml = function(html) {
    return decodeURIComponent(VJ.getValue(html, ''));
};
///处理自定义TJson格式 如[['RIndex','ID'],['1','6e014f804b8f46e1b129faa4b923af2d'],['2','6e014f804b8f46e1b129faa4b923a23d']]
VJ.evalTJson = function(data){
    data = eval(data);
    var res = [];
    $(data).each(function(i,v){
	    var s = {};
	    $(v).each(function(q,v2){
		    s[data[q]] = v2;
	    });
	    res[i-1]=s;
	});
	return res;
}