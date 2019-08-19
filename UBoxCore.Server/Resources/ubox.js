var ubox = {
    /**
     * 设备连接事件
     */
    OnDeviceConnected: function () {

    },
    /**
     * 设备关闭事件
     */
    OnDeviceClosed: function () {

    },
    /**
     * 设备错误事件
     * @param {*} error 错误信息
     */
    OnDeviceError: function (error) {

    },
    /**
     * 挂机事件
     */
    OnHangUp: function () {

    },

    /**
     * 摘机事件
     */
    OnHookOff: function () {

    },
    /**
     * 电压检测事件,需要配合电压检测的开始和结束方法
     */
    OnLineVoltage: function (v) {

    },
    /**
     * 响铃结束
     */
    OnRingEnd: function () {

    },
    /**
     * 响铃开始
     */
    OnRinging: function () {

    },
    /**
     * 来电事件
     * @param {来电号码} phone 来电号码
     */
    OnCall: function (phone) {
        AppendStatus("on call event: phone number=" + phone);
    },
    /**
     * 设备状态事件
     * @param {设备状态} status 设备状态
     */
    OnDeviceStatus: function (status) {
        //AppendStatus("On Device Status is:" + status);
    },
    /**
     * 下载进度事件
     * @param {*} p 进度0-100, 网速k/bs
     */
    OnDownloadProgressChanged:function(p){

    },
    /**
     * 下载完成
     */
    OnDownloadComplated:function(){
        
    },

    /**
     * 连接Websocket服务
     */
    connectServer: function (callback) {
        var self = this;
        if (window.WebSocket) {
            AppendStatus("brower supports WebSocket");
    
            ws = new WebSocket('wss://localhost:44321/ws');
    
            ws.onopen = function (ev) {
                devicestatus.wsconnected = true;
                if(callback){
                    callback(devicestatus);
                }
                ubox.getStatusAsync();
            };
    
            ws.onerror = function (ev) {
                devicestatus.wsconnected = false;
                AppendStatus("websocket error:" + JSON.stringify(ev));
            };
            ws.onclose = function (ev) {
                devicestatus.wsconnected = false;
                AppendStatus("websocket close:" + JSON.stringify(ev)); ws = null;
            };
    
            ws.onmessage = function (ev) {
                //console.log(ev);
                // AppendStatus(ev.data);
                var msg = eval('(' + ev.data + ')');
    
                if (msg.Event == "OnLog") {
                    AppendStatus(msg.Data);
                }
                else if (msg.Event == "OnError") {
                    ubox.OnDeviceError(msg.Data);
                }
                else if (msg.Event == "OnConnected") {
                    ubox.OnDeviceConnected();
                }
                else if (msg.Event == "OnClosed") {
                    ubox.OnDeviceClosed();
                }
                else if (msg.Event == "OnHangUp") {
                    ubox.OnHangUp();
    
                }
                else if (msg.Event == "OnHookOff") {
                    ubox.OnHookOff();
                }
                else if (msg.Event == "OnLineVoltage") {
                    ubox.OnLineVoltage(msg.Data);
                }
                else if (msg.Event == "OnRingCancel") {
                    ubox.OnRingEnd();
                }
                else if (msg.Event == "OnRinging") {
                    ubox.OnRinging();
                }
                else if (msg.Event == "OnCall") {
                    ubox.OnCall(msg.Data);
                }
                else if (msg.Event == "OnDeviceStatus") {
                    var status = parseInt(msg.Data);
                    ubox.OnDeviceStatus(status)
                    if (status >= 0) {
                        devicestatus.devconnected = true;
                    }
                    else {
                        devicestatus.devconnected = false;
                    }
    
                }
                else if(msg.Event == "DownloadProgressChanged") {
                    ubox.OnDownloadProgressChanged(msg.Data);
                }
                else if(msg.Event == "DownloadComplated") {
                    ubox.OnDownloadComplated(msg.Data);
                }
                else {
                    try {
                        ubox[msg.Event](msg);
                    }
                    catch{

                    }
                }
    
    
    
            }
        }
        

    },

    /**
     * 获取连接状态
     */
    getStatus: function () {
        return devicestatus;
    },

    /**
     * 获取初始化状态
     */
    getStatusAsync: function () {
        var msginfo = {
            Method: "getStatus",
            Parameters: []
        };
        websocket_send_msg(JSON.stringify(msginfo));
    },
    /**
     * 上传文件
     * @param {URL} url URL
     * @param {文件名路径} filename 文件名路径
     * @param {记录ID} callRecordsId 记录ID
     * @param {经销商ID} dealerid 经销商ID
     * @param {*} callback 
     */
    postfileAsync: function (url, filename, callRecordsId, dealerid,callback) {

        var msginfo = {
            Method: "upfiles",
            Parameters: [url, filename, callRecordsId, dealerid]
        };
        invokeAsync(this, msginfo, callback);


    },
    /**
     * 获取本地目录下的文件夹列表
     * @param {目录路径} path 目录路径
     * @param {*} callback 
     */
    getDirectoriesAsync: function (path,callback) {
        var msginfo = {
            Method: "getDirectories",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取本地目录下的文件列表
     * @param {目录路径} path 目录路径
     * @param {*} callback 
     */
    getDirFilesAsync: function (path, callback) {
        var msginfo = {
            Method: "getDirFiles",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取磁盘空间大小(Mb)
     * @param {路径} path 路径 
     * @param {*} callback 
     */
    getHardDiskSpaceAsync: function (path, callback) {
        var msginfo = {
            Method: "getHardDiskSpace",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取文件大小(Kb)
     * @param {*} path 文件路径
     * @param {*} callback 
     */
    getFileSizeAsync: function (path, callback) {
        var msginfo = {
            Method: "getFileSize",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 删除本地文件
     * @param {文件路径} path 文件路径
     * @param {*} callback 
     */
    deleteFileAsync: function (path, callback) {
        var msginfo = {
            Method: "deleteFile",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取网络速度
     * @param {*} callback 
     */
    pingNetworkAsync: function (callback) {
        var msginfo = {
            Method: "pingNetwork",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取系统信息
     * @param {*} callback 
     */
    getSystemInfoAsync: function (callback) {
        var msginfo = {
            Method: "getSystemInfo",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 打开系统进程
     * @param {文件路径} path 文件路径
     * @param {*} arguments 参数
     * @param {*} callback 
     */
    shellRunAsync: function (path,arguments,callback) {
        var msginfo = {
            Method: "shellRun",
            Parameters: [path,arguments]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 拨打电话
     * @param {电话号码} phone 电话号码
     * @param {*} callback 
     */
    postCallPhoneAsync: function (phone, callback) {
        var msginfo = {
            Method: "callPhone",
            Parameters: [phone]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 开始录音
     * @param {文件名} filename 文件名
     * @param {*} callback 
     */
    startRecordVoiceAsync: function (filename,callback) {
        var msginfo = {
            Method: "startRecordVoice",
            Parameters: [filename]
        };

        invokeAsync(this,msginfo,callback);
    },
    /**
     * 结束录音
     * @param {*} callback 
     */
    stopRecordVoiceAsync: function (callback) {
        var msginfo = {
            Method: "stopRecordVoice",
            Parameters: []
        };

        invokeAsync(this,msginfo,callback);
    },
    /**
     * 开始电压检测
     * @param {*} callback 
     */
    startReadLineVoltageAsync: function (callback) {
        var msginfo = {
            Method: "startReadLineVoltage",
            Parameters: []
        };

        invokeAsync(this, msginfo, callback);
    },
    /**
     * 结束电压检测
     * @param {*} callback 
     */
    stopReadLineVoltageAsync: function (callback) {
        var msginfo = {
            Method: "stopReadLineVoltage",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 设置摘机电压值
     * @param {电压值} value 电压值
     * @param {*} callback 
     */
    setHookoffThresholdAsync: function (value,callback) {
        var msginfo = {
            Method: "setHookoffThreshold",
            Parameters: [value]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 打开设备(盒子)
     * @param {*} callback 
     */
    connectDeviceAsync: function (callback) {
        var msginfo = {
            Method: "connectDevice",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 关闭设备(盒子)
     * @param {*} callback 
     */
    closeDeviceAsync: function (callback) {
        var msginfo = {
            Method: "closeDevice",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取摘机状态(0:挂机 , 1:摘机)
     * @param {*} callback 
     */
    getHookStatusAsync: function (callback) {
        var msginfo = {
            Method: "getHookStatus",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取录音状态
     * @param {*} callback 
     */
    getRecordStatusAsync: function (callback) {
        var msginfo = {
            Method: "getRecordStatus",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 获取设备可用状态
     * @param {*} callback 
     */
    getDeviceStatusAsync: function (callback) {
        var msginfo = {
            Method: "getDeviceStatus",
            Parameters: []
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 执行命令
     * @param {*} path 文件路径
     * @param {*} param 参数
     * @param {*} callback 
     */
    shellRunAsync:function(path,param,callback){
        var msginfo = {
            Method: "shellRun",
            Parameters: [path,param]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 显示打开对话框
     * @param {*} title 标题
     * @param {*} callback 
     */
    openFileDialogAsync:function(title,callback){
        var msginfo = {
            Method: "openFileDialog",
            Parameters: [title]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 显示保存对话框
     * @param {*} title 标题
     * @param {*} callback 
     */
    saveFileDialogAsync:function(title,callback){
        var msginfo = {
            Method: "saveFileDialog",
            Parameters: [title]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 下载指定文件到目录
     * @param {*} url 下载文件地址
     * @param {*} path 保存本地路径
     * @param {*} callback 
     */
    downloadFileAsync:function(url,path,callback){
        var msginfo = {
            Method: "downloadFile",
            Parameters: [url,path]
        };
        invokeAsync(this, msginfo, callback);
        
    },
    /**
     * 创建目录
     * @param {目录} path 目录路径
     * @param {*} callback 
     */
    createDirectoryAsync:function(path,callback){
        var msginfo = {
            Method: "createDirectory",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    writeTextToFileAsync:function(path,content,callback){
        var msginfo = {
            Method: "writeTextToFile",
            Parameters: [path,content]
        };
        invokeAsync(this, msginfo, callback);
    },
    readTextFormFileAsync:function(path,callback){
        var msginfo = {
            Method: "readTextFormFile",
            Parameters: [path]
        };
        invokeAsync(this, msginfo, callback);
    },
    /**
     * 自定义方法demo
     * @param {*} id 
     * @param {*} name 
     * @param {*} callback 
     */
    getUserInfoAsync:function(id,name,callback){
        var msginfo = {
            Method: "getUserInfo",
            Parameters: [id,name]
        };
        invokeAsync(this,msginfo,callback);
    },
    /**
     * 
     * @param {请求方法参数} request { func , params , success  }
     */
    invokeAsync:function(request,callback){
        var msginfo ={
            Method : request.func,
            Parameters: request.params
        }
        invokeAsync(this,msginfo,callback);
    }




};



function AppendStatus(szStatus) {
    var thisTime = new Date();

    var time = thisTime.getHours() + ":" + thisTime.getMinutes() + ":" + thisTime.getSeconds() + "    ";
    try {
        document.getElementById("StatusArea").value += time;
        document.getElementById("StatusArea").value += szStatus;
        document.getElementById("StatusArea").value += "\r\n";
        var scrollTop = $("#StatusArea")[0].scrollHeight;
        $("#StatusArea").scrollTop(scrollTop);
    }
    catch (e) {
        console.error(e);
    }


    console.log(time)
    console.log(szStatus)
}


function invokeAsync(obj,msginfo,callback) {

    var st_callback = msginfo.Method + 'Callback';
    obj[st_callback] = callback;

    var msg = JSON.stringify(msginfo)
    ws.send(msg);
}


function websocket_send_msg(msg) {
    ws.send(msg);
}

var devicestatus = {
    wsconnected: false,
    devconnected: false
};




var UBoxAsyncApi = new ProxyFunc(ubox, {
    get: function(target, property) {
      if (property in target) {
        return function() {
            obj = Array.prototype.slice.call(arguments); 
            
            return new Promise(function (resolve, reject) {
                

                obj.push(resolve);
                target[property].apply(ubox,obj);
            });
        }
      }
       else {
        throw new IllegalAPIException(property);
      }
  
    }
  });


  var uboxApi = UBoxAsyncApi
