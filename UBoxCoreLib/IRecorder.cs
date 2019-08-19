using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBoxCoreLib
{
    public interface IRecorder
    {
        /// <summary>
        /// 设备初始化
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>

        bool Init(object param);

        /// <summary>
        ///打开设备
        /// </summary>
        /// <returns></returns>
        bool ConnectDevice();
        /// <summary>
        /// 关闭设备
        /// </summary>
        void CloseDevice();


        void RestDevice();


        /// <summary>
        /// 取得已连接设备的通道对象
        /// </summary>
        /// <returns></returns>
        object getCurrentChannel();
        /// <summary>
        /// 拨打电话
        /// </summary>
        /// <param name="phone">电话号</param>
        void CallPhone(string phone);
        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="filename"></param>
        bool StartRecordVoice(string filename);
        /// <summary>
        /// 结束录音
        /// </summary>
        void StopRecordVoice();

        /// <summary>
        /// 开始测量电压
        /// </summary>
        void StartReadLineVoltage();

        /// <summary>
        /// 停止测量电压
        /// </summary>
        void StopReadLineVoltage();

        /// <summary>
        /// 设置摘机电压
        /// </summary>
        /// <param name="voltage"></param>
        void SetHookoffThreshold(int voltage);


        /// <summary>
        /// 设备已连接(型号,通道) OnPlugIn
        /// </summary>
        /// <param name="connected"></param>
        void OnDeviceConnected(Action<object,string> recordevent);

        /// <summary>
        /// 设备已关闭 OnPlugOut
        /// </summary>
        /// <param name="closed"></param>
        void OnDeviceClosed(Action<object> recordevent);

        /// <summary>
        /// 摘机事件(拿起电话)
        /// </summary>
        void OnHookOff(Action<object> recordevent);
        /// <summary>
        ///  挂机事件(挂断电话)
        /// </summary>
        void OnHangUp(Action<object> recordevent);


        /// <summary>
        /// 电压检测
        /// </summary>
        /// <param name="recordevent"></param>
        void OnLineVoltage(Action<object, int> recordevent);

        /// <summary>
        /// 来电事件 (phoneNumber)
        /// </summary>
        /// <param name=""></param>
        void OnCall(Action<object,string> recordevent);



        /// <summary>
        /// 来电结束(铃声停止)
        /// </summary>
        /// <param name="recordevent"></param>
        void OnRinging(Action<object> recordevent);

        /// <summary>
        /// 来电结束(铃声停止)
        /// </summary>
        /// <param name="recordevent"></param>
        void OnRingCancel(Action<object> recordevent);

        /// <summary>
        /// 通知应用程序通道软件拨号完成，但并不表示拨号成功。
        /// </summary>
        void OnCallOutFinish(Action<object> recordevent);
        /// <summary>
        /// UBOX_EVENT_DEVICE_ERROR
        /// -1.
        /// 和电脑的USB接触有问题。供电和信号差，一般要求USB线插到电脑后端。
        /// 如果换了多个USB口或者换了一台电脑，退出软件，硬件重新插拔一下，重新运行软件，还是出现这个设备错误事件，那USB设备有问题了。
        /// 处理方法：退出软件，硬件重新插拔一下，重新运行软件，看是否正常。
        /// 出现的原因：Windows应用程序和设备通信出现异常
        ///
        /// UBOX_EVENT_ALARM
        /// 1.和电脑的USB接触有问题。
        /// 2.USB audio device驱动程序没有安装好（操作系统自动安装），正在安装的过程中，运行了软件。
        /// 3.操作系统不是完整版本，缺少USB audio devcie驱动，安装驱动精灵试一下，看是否能修复驱动程序，如果修复不了，只能安装完整版本操作系统。
        /// 4.供电和信号差，一般要求USB线插到电脑后端。
        /// 5.windows audio 服务被禁用了，将该服务设置为自动和启动，重启电脑。
        /// 6.USB audio device在声卡属性中被禁用，将禁用取消。
        /// 7.运行了多个USB程序打开USB设备，也会导致这种情况。
        /// 8.操作系统出现问题，需要操作系统重启。
        ///
        /// 连接设备错误
        /// UBOX_ERR_SUCCESS = 0,	  操作成功
        /// UBOX_ERR_SYSTEM= -1,	  系统错误，调用操作系统(windows)的方法时出现错误，错误的详细信息可查看日志文件： 
        /// UBOX_ERR_DEVICE_NOT_FOUND	= -2,	  没有这个设备，可能设备已经被拔出
        /// UBOX_ERR_INVALID_HANDLE = -3, 不合法的UBOX_HANDLE
        /// UBOX_ERR_INVALID_PARAMETER	= -4,	  不合法的输入参数
        /// UBOX_ERR_EXCEPTION = -5, 发生异常
        /// UBOX_ERR_INVALID_WORK_MODE	= -6,	  错误的工作模式
        /// UBOX_ERR_UBOX_NOT_OPEN = -7, ubox设备尚未打开
        /// UBOX_ERR_CANNOT_CREATE_DIR	= -10,	  未能创建目录，当指定录音时，如果文件名包含目录路径，则ubox将试图建立相应的目录树。
        /// UBOX_ERR_CANNOT_CREATE_FILE	= -11,	  未能创建录音文件
        /// UBOX_ERR_INVALID_VOICE_CODER = -12, 不支持的语音编码
        /// UBOX_ERR_DEVICE_BUSY		= -13,	  设备忙，当设备已经在录音的时候再次指示其同类型(文件与文件、STREAM与STREAM)的录音，就会返回此错误码
        /// </summary>
        void OnDeviceError(Action<object,string> recordevent);

        void OnLog(Action<object, string> logevent);

    }
}
