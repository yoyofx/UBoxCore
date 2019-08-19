using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UBoxCoreLib
{
    public class Channel
    {
        public string _strDTMF;
        public long _step;
        public long _timer;
        public IntPtr _handle;
        public int _lineID;
        public int _ringCount;
        public bool _bRecording;
        public int _updown = 0; //0 down , 1 up
        public void Inital(IntPtr handle, int lineID)
        {
            _lineID = lineID;
            _handle = handle;
            _strDTMF = "";
            _step = 0;
            _timer = 0;
            _ringCount = 0;
            _bRecording = false;
        }

        public static Channel New(IntPtr handle)
        {
            var c = new Channel();
            c.Inital(handle, 0);
            return c;
        }


    }
}
