using MouseImitator.Global.Services.Hooks;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MouseImitator.Application.Services.Hooks.Mouse
{
    public interface IMouseEvent: IDeviceEvent, ITimed, ICoordinates
    {
        [Flags]
        public enum EventButton { 
            LEFT = 0x00,
            MIDDLE = 0x01,
            RIGHT = 0x02
        }
        [Flags]
        public enum EventAction { 
            DOWN = 0x00,
            UP = 0x01,
            CLICK = 0x02,
            DOUBLE_CLICK = 0x03,
            DRAG = 0x04,
            MOVE = 0x05
        }

        public EventAction Action { get; }
        public EventButton Button { get; }
        public List<ITimedCoordinates> Series { get; }
    }
}
