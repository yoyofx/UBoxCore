using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UBoxCoreLib
{
    public class HPRecorder : IRecorder
    {
        private Channel _channel;

        private string updown = "down"; 


        public bool Init(object param)
        {
            Console.WriteLine("设备正在初始化...");
            return true;
        }

        private void uBoxEventNotifyCallback(IntPtr uboxHnd, UBoxEvent eventID, IntPtr param1, IntPtr param2, IntPtr param3, IntPtr param4)
        {
            var currentChannel = Channel.New(uboxHnd);
            if (this.onlog!=null)
            {
                this.onlog.Invoke(currentChannel, $"HWND:{uboxHnd} , EventID:{eventID} ,[ P1:{param1},P2:{param2},P3:{param3},P4:{param4}]");
            }


            Console.WriteLine($"HWND:{uboxHnd} , EventID:{eventID} , Param1:{param1}");


            switch (eventID)
            {
                case UBoxEvent.UBOX_EVENT_DEVICE_PLUG_IN:
                    if (ondeviceconnected != null)
                    {
                        ondeviceconnected.Invoke(currentChannel, "");
                    }
                    _channel = currentChannel;
                    break;
                case UBoxEvent.UBOX_EVENT_DEVICE_PLUG_OUT:
                    if (ondeviceclosed != null)
                        ondeviceclosed.Invoke(currentChannel);
                    _channel = null;
                    break;
                case UBoxEvent.UBOX_EVENT_ALARM:
                    if (ondeviceerror != null)
                    {
                        string errorcode = param1.ToInt32().ToString();
                        ondeviceerror.Invoke(currentChannel, errorcode);
                    }
                    break;
                case UBoxEvent.UBOX_EVENT_DEVICE_ERROR:
                    if (ondeviceerror != null)
                        ondeviceerror.Invoke(currentChannel, "-1");
                    break;
                case UBoxEvent.UBOX_EVENT_LINE_VOLTAGE:
                    if (this.onlinevoltage != null)
                        this.onlinevoltage.Invoke(currentChannel, param1.ToInt32());
                    break;
                case UBoxEvent.UBOX_EVENT_LINE_RINGING:
                    if (onringing != null)
                        onringing.Invoke(currentChannel);
                    break;
                case UBoxEvent.UBOX_EVENT_LINE_RING_STOP:
                    if (onringcancel != null)
                        onringcancel.Invoke(currentChannel);
                    break;

                case UBoxEvent.UBOX_EVENT_LINE_HOOK_OFF:
                    _channel._updown = 1;
                    if (onhookoff != null)
                        onhookoff.Invoke(currentChannel);
                    break;
                case UBoxEvent.UBOX_EVENT_LINE_HOOK_UP:
                    _channel._updown = 0;
                    if (onhangup != null)
                        onhangup.Invoke(currentChannel);
                    break;
                case UBoxEvent.UBOX_EVENT_CALLER_ID:
                    if (oncall != null)
                    {
                        string phone = Marshal.PtrToStringAnsi(param1);
                        oncall.Invoke(currentChannel, phone);
                    }
                    break;



                //default:
                //    break;

            }


        }

        public void CloseDevice()
        {
            Console.WriteLine("设备关闭...");
            UBoxNative.ubox_close_logfile();
            UBoxNative.ubox_close();
            _channel = null;
            callbackPtr = IntPtr.Zero;
            callbackProc = null;
        }


        private UBoxEventNotifyProc callbackProc = null;
        private IntPtr callbackPtr;

        public bool ConnectDevice()
        {
            UBoxNative.ubox_open_logfile(0);
            if (callbackProc == null)
            {
                callbackProc = new UBoxEventNotifyProc(uBoxEventNotifyCallback);
               
                callbackPtr = Marshal.GetFunctionPointerForDelegate(callbackProc);
                GC.KeepAlive(callbackPtr);
            }
            UBoxNative.ubox_close();
            return UBoxNative.ubox_open(callbackPtr, 0) == 0;
        }

        public object getCurrentChannel()
        {
            return _channel;
        }

     
        public void RestDevice()
        {
            this.CloseDevice();
            this.ConnectDevice();
        }

        public bool StartRecordVoice(string filename)
        {
            bool ret = false;
            if (_channel != null)
            {
                _channel._bRecording = true;
                ret = UBoxNative.ubox_record_file(_channel._handle, filename, ENUM_VOICE_CODER.CODER_MP3) == 0;
            }
            return ret;
        }

        public void StopRecordVoice()
        {
            if (_channel != null)
            {
                _channel._bRecording = false;
                UBoxNative.ubox_stop_record(_channel._handle);
            }
        }

        public void CallPhone(string phone)
        {
            if (_channel != null)
            {
                UBoxNative.ubox_send_dtmf(_channel._handle, phone);
            }
        }

        public void StartReadLineVoltage()
        {
            if (_channel != null)
                UBoxNative.ubox_start_read_line_voltage(this._channel._handle);
        }

        public void StopReadLineVoltage()
        {
            if (_channel != null)
                UBoxNative.ubox_stop_read_line_voltage(this._channel._handle);
        }

        public void SetHookoffThreshold(int voltage)
        {
            if (_channel != null)
                UBoxNative.ubox_set_hookoff_threshold(this._channel._handle, voltage);
        }


        #region 外部事件

        private Action<object, string> onlog;

        private Action<object, string> oncall;

        private Action<object> oncalloutfinish;

        private Action<object> ondeviceclosed;

        private Action<object, string> ondeviceconnected;

        private Action<object, string> ondeviceerror;

        private Action<object> onhangup;
        private Action<object> onhookoff;

        private Action<object> onringing;
        private Action<object> onringcancel;


        private Action<object, int> onlinevoltage;


        public void OnLineVoltage(Action<object, int> recordevent)
        {
            this.onlinevoltage = recordevent;
        }

        public void OnCall(Action<object, string> recordevent)
        {
            this.oncall = recordevent;
        }

        public void OnCallOutFinish(Action<object> recordevent)
        {
            throw new NotImplementedException();
        }

        public void OnDeviceClosed(Action<object> recordevent)
        {
            this.ondeviceclosed = recordevent;
        }

        public void OnDeviceConnected(Action<object, string> recordevent)
        {
            this.ondeviceconnected = recordevent;
        }

        public void OnDeviceError(Action<object, string> recordevent)
        {
            this.ondeviceerror = recordevent;
        }

        public void OnHangUp(Action<object> recordevent)
        {
            this.onhangup = recordevent;
        }

        public void OnHookOff(Action<object> recordevent)
        {
            this.onhookoff = recordevent;
        }

        public void OnRingCancel(Action<object> recordevent)
        {
            this.onringcancel = recordevent;
        }

        public void OnRinging(Action<object> recordevent)
        {
            this.onringing = recordevent;
        }

        public void OnLog(Action<object, string> logevent)
        {
            this.onlog = logevent;
        }

     

        #endregion



    }
}
