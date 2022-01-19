WEB_SOCKET_SWF_LOCATION = "WebSocketMainInsecure.swf";
WEB_SOCKET_DEBUG = true; 

$(window).ready(function () {

    //ubox.connectServer();

    uboxApi.connectServer().then(function(){ return uboxApi.getDeviceStatusAsync() } )
        .then(function (rs) {
            AppendStatus(JSON.stringify(rs));
        });


});

ubox.OnCall = function (phone) {
    AppendStatus("来电号码为:" + phone);
}

ubox.OnDeviceStatus = function (status) {
    AppendStatus("On Device Status is:" + status);
};

ubox.OnDeviceError = function(error){
    AppendStatus("退出软件，重新插拔一下电脑端USB口或换一个USB口，重新运行软件，看是否正常；如还有异常请重启电脑。错误码:" + error);
}



ubox.OnDownloadComplated = function () {
    console.log("download complated");
};
ubox.OnDownloadProgressChanged = function (e) {
 
    $('#downloadDiv').text("进度:" + e.Percentage + "%, 网络速度:" + e.Speed);

    //console.log(e)
};


function getCustomFunc() {

    uboxApi.getUserInfoAsync("30", "maxzhang").then(function (rs) {
        console.log("async api");
        console.log(rs);
    });

}

function getDirectories() {
    uboxApi.getDirectoriesAsync('d:\\')
        .then(function(rs)  {
            console.log(rs);
        });
}

function getDirFilesAsync() {
    uboxApi.getDirFilesAsync('d:\\mp3\\')
        .then(function(rs)  {
            console.log(rs);
        });
}


function callPhone(phone) {
    uboxApi.postCallPhoneAsync(phone)
        .then(function(rs)  {
            console.log(rs);
        });
}

function startRecordVoice() {
    var path = 'd:/mp3/88888.mp3';
    uboxApi.startRecordVoiceAsync(path)
        .then(function(rs)  {
            console.log(rs);
        });
}

function stopRecordVoice() {
    uboxApi.stopRecordVoiceAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}


function setHookoffThresholdAsync(v) {
    var thv = parseInt(v);
    uboxApi.setHookoffThresholdAsync(thv)
        .then(function(rs)  {
            console.log(rs);
        });
}


function startReadLineVoltage() {
    uboxApi.startReadLineVoltageAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}


function stopReadLineVoltage() {
    uboxApi.stopReadLineVoltageAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}



function upfile() {
    uboxApi.postfileAsync('http://chexian.ubox.cn/gateway/boxapi/callrecords/upload',
        'd:\\mp3\\588.mp3', '462', '50002218')
        .then(function(rs)  {
            console.log(rs);
        });


}

function openDev() {
    uboxApi.connectDeviceAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}
function closeDev() {
    uboxApi.closeDeviceAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}

function getHookStatus() {

    uboxApi.getHookStatusAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}

function getRecordStatus() {
    uboxApi.getRecordStatusAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}

function getDeviceStatus() {
    uboxApi.getDeviceStatusAsync()
        .then(function(rs)  {
            console.log(rs);
        });
}

function getHardDiskSpaceAsync() {
    uboxApi.getHardDiskSpaceAsync('D')
        .then(function(rs)  {
            console.log(rs);
        });
}

function getNetworkSpeed() {
    uboxApi.pingNetworkAsync().then(function(rs)  {
        console.log(rs)
    })
}

function getSystemInfo() {
    uboxApi.getSystemInfoAsync().then(function(rs)  {

        console.log(rs)
    })
}

function shellRun() {
    uboxApi.shellRunAsync('calc.exe', '')
        .then(function(rs)  {
            console.log(rs);
        })
}

function openFileDialog() {
    uboxApi.openFileDialogAsync('打开文件')
        .then(function(rs)  {
            console.log(rs);
        })
}


function saveFileDialog() {
    uboxApi.saveFileDialogAsync('保存文件')
        .then(function(rs)  {
            console.log(rs);
        })
}

function downloadFile() {
    var uri = 'https://download.visualstudio.microsoft.com/download/pr/408b9eb6-c213-4498-abf3-317b73e2eb54/0ca48259be33b961af8980cd2bbaac51/dotnet-sdk-2.1.801-win-x86.exe'

    uboxApi.downloadFileAsync(uri, 'D:/mp3/dotnetcore3.exe').then(function()  {
        console.log("download file begin!")
    });
}

function createDirAndWriteFile() {
    var path = 'd:/mp3/test/js/';
    uboxApi.createDirectoryAsync(path)
    .then(function()  {
        var json = { a: 1 , b:2 , c:3  };
        var strJson = JSON.stringify(json);
        
        return uboxApi.writeTextToFileAsync(path + "demo.json",strJson);
    })
    .then(function(rs)  {
        console.log(rs)
    });
}


function readFile(){
    var path = 'd:/mp3/test/js/demo.json';
    uboxApi.readTextFormFileAsync(path)
    .then(function(rs)  {
        console.log(rs);
    });
}


