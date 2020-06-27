using Gma.System.MouseKeyHook;
using MouseImitator.Global.Services.Hooks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MouseImitator.Application.Services.Hooks.Mouse
{
    public class MouseHook : IDeviceHook
    {
        private IKeyboardMouseEvents _globalHook;
        private readonly HashSet<IDeviceEventListener> _listeners;
        private bool _started;
        private bool _dragStarted;

        private IMouseEvent _currentEvent;

        public MouseHook()
        {
            _listeners = new HashSet<IDeviceEventListener>();
            _started = false;
            _dragStarted = false;
            _globalHook = Hook.GlobalEvents();
        }

        public void Dispose()
        {
            _listeners.Clear();
            StopListening();
        }

        public void Register(IDeviceEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void StartListening()
        {
            if (!_started)
            {
                _currentEvent = null;

                _globalHook.MouseClick += _globalHook_MouseClick;
                _globalHook.MouseDoubleClick += _globalHook_MouseDoubleClick;
                _globalHook.MouseDragStartedExt += _globalHook_MouseDragStarted;
                _globalHook.MouseDragFinishedExt += _globalHook_MouseDragFinished; ;
                _globalHook.MouseMoveExt += _globalHook_MouseMove;
            }

            _started = true;
        }


        private void _globalHook_MouseMove(object sender, MouseEventExtArgs e)
        {
            if (_currentEvent == null)
            {
                var action = IMouseEvent.EventAction.MOVE;

                var ev = _toEvent(e, action);
                _currentEvent = ev;
            }

            if (_currentEvent != null)
            {
                _currentEvent.Series.Add(new TimedCoordinatesImpl()
                {
                    Timestamp = e.Timestamp,
                    X = e.X,
                    Y = e.Y
                });

                Console.WriteLine("_globalHook_MouseMove");
            }
        }

        private void _globalHook_MouseDragFinished(object sender, MouseEventExtArgs e)
        {

            if (_dragStarted)
            {
                _currentEvent.Series.Add(new TimedCoordinatesImpl()
                {
                    Timestamp = e.Timestamp,
                    X = e.X,
                    Y = e.Y
                });

                _publish(_currentEvent);
                _currentEvent = null;
            }

            _dragStarted = false;

            Console.WriteLine("_globalHook_MouseDragFinished");
        }

        private void _globalHook_MouseDragStarted(object sender, MouseEventExtArgs e)
        {

            if (_currentEvent != null)
            {
                _publish(_currentEvent);
                _currentEvent = null;
            }

            if (!_dragStarted)
            {
                var action = IMouseEvent.EventAction.DRAG;

                var ev = _toEvent(e, action);

                ev.Series.Add(new TimedCoordinatesImpl()
                {
                    Timestamp = ev.Timestamp,
                    X = ev.X,
                    Y = ev.Y
                });

                _currentEvent = ev;
            }

            _dragStarted = true;

            Console.WriteLine("_globalHook_MouseDragStarted");
        }

        private void _publish(IMouseEvent mouseEvent)
        {

            foreach (var e in _listeners)
            {
                e.OnEventReceived(mouseEvent);
            }
        }

        private void _globalHook_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (_dragStarted) {
                return;
            }

            if (_currentEvent != null)
            {
                _publish(_currentEvent);
                _currentEvent = null;
            }

            var action = IMouseEvent.EventAction.DOUBLE_CLICK;

            var ev = _toEvent(e, action);

            _publish(ev);

            Console.WriteLine("_globalHook_MouseDoubleClick");
        }

        private void _globalHook_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (_dragStarted)
            {
                return;
            }

            if (_currentEvent != null)
            {
                _publish(_currentEvent);
                _currentEvent = null;
            }

            var action = IMouseEvent.EventAction.CLICK;

            var ev = _toEvent(e, action);

            _publish(ev);

            Console.WriteLine("_globalHook_MouseClick");
        }

        private IMouseEvent.EventButton _toButton(MouseButtons e)
        {
            var button = IMouseEvent.EventButton.LEFT;

            if (e == MouseButtons.Right)
            {
                button = IMouseEvent.EventButton.RIGHT;
            }
            else if (e == MouseButtons.Middle)
            {
                button = IMouseEvent.EventButton.MIDDLE;
            }
            return button;
        }

        private IMouseEvent _toEvent(MouseEventArgs e, IMouseEvent.EventAction action)
        {

            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (e.GetType() == typeof(MouseEventExtArgs))
            {
                timestamp = ((MouseEventExtArgs)e).Timestamp;
            }

            var button = _toButton(e.Button);

            return new MouseEventImpl()
            {
                Button = button,
                Action = action,
                Timestamp = timestamp,
                X = e.X,
                Y = e.Y
            };
        }

        public void StopListening()
        {
            if (_started)
            {
                if (_currentEvent != null) {
                    _publish(_currentEvent);
                    _currentEvent = null;
                }

                _globalHook.MouseClick -= _globalHook_MouseClick;
                _globalHook.MouseDoubleClick -= _globalHook_MouseDoubleClick;
                _globalHook.MouseDragStartedExt -= _globalHook_MouseDragStarted;
                _globalHook.MouseDragFinishedExt -= _globalHook_MouseDragFinished; ;
                _globalHook.MouseMoveExt -= _globalHook_MouseMove;
            }

            _started = false;
        }

        public void UnRegister(IDeviceEventListener listener)
        {
            _listeners.Remove(listener);
        }

        [Serializable]
        private class MouseEventImpl : IMouseEvent
        {

            public MouseEventImpl()
            {
                Series = new List<ITimedCoordinates>();
            }

            public IMouseEvent.EventAction Action { get; set; }

            public IMouseEvent.EventButton Button { get; set; }

            public int Id
            {
                get
                {
                    return Action.GetHashCode() + Button.GetHashCode();
                }
            }

            public long Timestamp { get; set; }

            public int X { get; set; }

            public int Y { get; set; }

            public List<ITimedCoordinates> Series { get; set; }
        }

        [Serializable]
        private class TimedCoordinatesImpl : ITimedCoordinates
        {
            public long Timestamp { get; set; }

            public int X { get; set; }

            public int Y { get; set; }
        }
    }
}
