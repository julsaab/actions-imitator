using System;
using System.Collections.Generic;
using System.Text;

namespace MouseImitator.Global.Services.Hooks
{
    public interface IDeviceEventListener
    {
        void OnEventReceived(IDeviceEvent deviceEvent);
    }
}
