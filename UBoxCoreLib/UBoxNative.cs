using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UBoxCoreLib
{
    public enum UBoxEvent
    {
        UBOX_EVENT_DEVICE_PLUG_IN = 1,  //检测到设备插入，回调函数中param1、param2、param3、param4未使用，
                                        //参数如果没有说明,未使用，下同
        UBOX_EVENT_DEVICE_PLUG_OUT = 2, //检测到设备拔出，

        UBOX_EVENT_ALARM = 3,            //报警,param1参照UBOX_ALARM的定义，设备工作不正常，软件退出，检查设备驱动程序是否安装好

        UBOX_EVENT_LINE_RESET = 10,       //复位：摘机->挂机，振铃->停振  硬件LED灭 已经将该事件分为两个挂机和停振事件
        UBOX_EVENT_LINE_RINGING = 11,         //振铃，硬件LED亮
        UBOX_EVENT_LINE_HOOK_OFF = 12,        //摘机，硬件LED亮
        UBOX_EVENT_LINE_HANG = 13,           //检测到线路悬空 ，默认线路电压小于3V，认为线路悬空，硬件LED闪烁

        //UBOX_EVENT_LING_FLUSH		= 14,	       //?? 需要吗？

        UBOX_EVENT_RING_CANCEL = 15,            //振铃取消，当末次振铃停止超过6秒，则触发此事件，

        UBOX_EVENT_LINE_VOLTAGE = 16,     //线路电压事件，param1是线路电压值，param1不会出现等于0的电压。
        UBOX_EVENT_STREAM_VOICE = 20,       //流式录音事件，param1是录音数据地址，需要转换，unsigned char* pvoice= (unsigned char*)param1
                                            // param2是录音数据长度


        UBOX_EVENT_CALLER_ID = 21,             //param1是号码地址  param2是时间地址  param3是姓名地址 param4没有用
                                               //需要类型转换 char* pszcallId =(char*)param1, char* pszcalltime=(char*)param2, 
                                               //char* pszcallname=(char*) param3

        UBOX_EVENT_DTMF_DOWN = 22,             //按键事件，param1是按键键值
        UBOX_EVENT_DTMF_UP = 23,
        UBOX_EVENT_DEVICE_ERROR = 24,           //设备错误,需要软件重新启动，不然接收不到主叫号码
        UBOX_EVENT_DEVICE_PLAY_END = 25,          //放音完毕
        UBOX_EVENT_DEVICE_PLAY_ERROR = 26,       //放音异常
        UBOX_EVENT_DEVICE_BUSY_TONE = 27,        //检测到忙音，对方挂机
        UBOX_EVENT_CALLOUTFINISH = 28,           //呼出完成，表示拨号结束
        UBOX_EVENT_POLARITY = 29,          //检测到极性反转
        UBOX_EVENT_LINE_HOOK_UP = 30,          //挂机     无线设备param1有效，其它类型的设备param1无效， param1: 1 gsm挂机 2 话机挂机 3 耳麦挂机
        UBOX_EVENT_LINE_RING_STOP = 31,          //停振 

        UBOX_EVENT_SIM_STATE = 32,           //param1: 1 sim卡1  2：sim卡2  param2: 0 未插入(新版本已经不支持了)  1：设备当前工作sim卡序号 2:GSM 模块检查Sim卡未插入 3. 检测到sim卡插入，其它值检测到sim有错误
        UBOX_EVENT_ANSWER = 33,
        UBOX_EVENT_GSM_MSG = 34,         //收到GSM返回的消息
        UBOX_EVENT_SHORT_MSG = 35,          ////短信   param1:发短信电话号码  param2：短信时间  param3:短信内容 param4:"" 为空，短信没有被分割
        UBOX_EVENT_SIGNALE_SIZE = 36,         //信号大小
        UBOX_EVENT_GSM_VOL_SIZE = 37,         //param1: 1  gsm play 2：gsm mic  param2: 声音大小
        UBOX_EVENT_SHORT_MSG_SIZE = 38,         //短信数量   param1：SIM卡 短信数目， param2: SIM卡短信存储空间数目
        UBOX_EVENT_SIM_REG = 39,          //SIM 是否注册  param1: sim序号 0：sim1 ,1:sim2   param2: sim卡注册返回值，  
                                          /*
                                                   0    未注册；ME 当前没有搜索到要注册业务的新营运商 
                                                   1    已注册，本地网 
                                                   2    未注册，但 ME正在搜索要注册业务的新营运商 
                                                   3    注册被拒绝 
                                                   4    未知 
                                                   5    已注册，漫游 
                                         */
        UBOX_EVENT_SHORT_MSG_SEND_REPORT = 40,          //短信发送报告， param1=0，发送失败 param1=1,发送成功 
    }

    /// <summary>
    /// 一般推荐用CODER_ALAW, CODER_MP3和CODER_SPEEX编码。
    /// </summary>
    public enum ENUM_VOICE_CODER
    {
        CODER_ALAW = 0,
        CODER_PCM = 1,
        CODER_G729 = 3,
        CODER_MP3 = 38,
        CODER_SPEEX = 20,   // 这是8倍压缩，安装speexw.exe的插件，播放器就可以播放了
        CODER_ULAW = 100,
    }


    [UnmanagedFunctionPointer(CallingConvention.StdCall)]

    public delegate void UBoxEventNotifyProc(IntPtr uboxHnd, UBoxEvent eventID, IntPtr param1, IntPtr param2, IntPtr param3, IntPtr param4);

    public class UBoxNative
    {
        //UBoxEventNotifyProc
        [DllImport("Phonic_ubox.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int ubox_open(IntPtr uboxEventNotifyCallback, UInt32 mode);
        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern void ubox_close();


        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern void ubox_open_logfile(long level);


        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern void ubox_close_logfile();


        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern int ubox_send_dtmf(IntPtr uboxHnd,string dmftfstring);


        /// <summary>
        /// 函数返回：0成功 , < 0 失败
        /// </summary>
        /// <param name="uboxHnd"></param>
        /// <param name="dmftfstring"></param>
        /// <param name="coder"></param>
        /// <returns></returns>
        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern int ubox_record_file(IntPtr uboxHnd, string filename, ENUM_VOICE_CODER coder);


        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern int ubox_stop_record(IntPtr uboxHnd);


        /// <summary>
        /// 设置摘机电压
        /// </summary>
        /// <param name="uboxHnd"></param>
        /// <param name="threshold">是摘机电压和挂机电压的之和的一半，摘机电压和挂机电压测量调用</param>
        /// <returns></returns>
        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern int ubox_set_hookoff_threshold(IntPtr uboxHnd, int threshold);



        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern int ubox_start_read_line_voltage(IntPtr uboxHnd);

        [DllImport("Phonic_ubox.dll", CharSet = CharSet.Ansi)]
        public static extern int ubox_stop_read_line_voltage(IntPtr uboxHnd);

    }
}
