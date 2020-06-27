using MouseImitator.Application.Services.Hooks.Mouse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mouse_Imitator
{
    [Serializable]
    class LinkedMouseEvent
    {

        public IMouseEvent CurrentEvent { set; get; }
        public IMouseEvent LastEvent { set; get; }

        public long GetTimeDifference()
        {
            if (LastEvent == null)
            {
                return 0L;
            }
            else
            {
                return CurrentEvent.Timestamp - LastEvent.Timestamp;
            }
        }
    }
}
