using MouseImitator.Global.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseImitator.Global.Services.Hooks
{
    public interface IDeviceHook: IDisposable
    {
        void Register(IDeviceEventListener listener);

        void UnRegister(IDeviceEventListener listener);

        void StartListening();

        void StopListening();
    }
}
