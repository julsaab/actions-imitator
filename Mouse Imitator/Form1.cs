using Gma.System.MouseKeyHook;
using MouseImitator.Application.Services.Hooks.Mouse;
using MouseImitator.Global.Services.Hooks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mouse_Imitator
{
    public partial class Form1 : Form, IDeviceEventListener
    {

        private MouseHook _mouseHook;
        private MouseEventList _mouseEvents;
        private bool recording;
        private bool replaying;
        private Thread workerThread;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;


        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.Shown += Form1_Shown;
            this.Disposed += Form1_Disposed;
            this._mouseEvents = new MouseEventList();
            this.TopMost = true;
            // Note: for the application hook, use the Hook.AppEvents() instead
            _mouseHook = new MouseHook();
            _proc = HookCallback;
            replayButton.Enabled = false;

            _mouseHook.Register(this);

            toolStripMenuItem3.Click += ToolStripMenuItem3_Click;
            toolStripMenuItem4.Click += ToolStripMenuItem4_Click;

        }

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            var serializedEvents = File.ReadAllText("events.dat");
            _mouseEvents = DeserializeFromString<MouseEventList>(serializedEvents);

            replayButton.Enabled = _mouseEvents.GetCount() > 0;
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            var serializedEvents = SerializeToString(_mouseEvents);
            File.WriteAllText("events.dat", serializedEvents);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if ((Keys)vkCode == Keys.F6)
                {

                    performReplay();
                }
                else if ((Keys)vkCode == Keys.F7)
                {

                    performRecord();
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private void Form1_Disposed(object sender, EventArgs e)
        {

            //m_GlobalHook.KeyPress -= GlobalHookKeyPress;
            Unsubscribe();

            //It is recommened to dispose it
            _mouseHook.Dispose();
            UnhookWindowsHookEx(_hookID);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            _hookID = SetHook(_proc);
            //Subscribe();

            // m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }

        /// <summary>
        /// simulates a mouse click see http://pinvoke.net/default.aspx/user32/mouse_event.html?diff=y
        /// </summary>
        /// <param name="button">which button to press (left middle up)</param>
        public static void OnMouseClick(IMouseEvent.EventButton button)
        {
            switch (button)
            {
                case IMouseEvent.EventButton.LEFT:
                    mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                    mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.RIGHT:
                    mouse_event((uint)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event((uint)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.MIDDLE:
                    mouse_event((uint)MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
                    mouse_event((uint)MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
                    break;
            }
        }

        public static void OnMouseDown(IMouseEvent.EventButton button)
        {
            switch (button)
            {
                case IMouseEvent.EventButton.LEFT:
                    mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.RIGHT:
                    mouse_event((uint)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.MIDDLE:
                    mouse_event((uint)MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
                    break;
            }
        }

        public static void OnMouseUp(IMouseEvent.EventButton button)
        {
            switch (button)
            {
                case IMouseEvent.EventButton.LEFT:
                    mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.RIGHT:
                    mouse_event((uint)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.MIDDLE:
                    mouse_event((uint)MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
                    break;
            }
        }

        [Flags]
        enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        public void Subscribe()
        {
            _mouseHook.StartListening();
        }

        public void Unsubscribe()
        {
            _mouseHook.StopListening();
        }

        private void performReplay()
        {

            if (_mouseEvents.GetCount() < 1)
            {
                return;
            }

            replaying = !replaying;

            if (replaying)
            {
                replayButton.Text = "Stop Replaying";
                recordButton.Enabled = false;

                this.workerThread = new Thread(new ThreadStart(this.replay));
                this.workerThread.Start();
            }
            else
            {
                workerThread.Interrupt();
                replayButton.Text = "Replay";
                recordButton.Enabled = true;
            }
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            if (recording)
            {
                return;
            }

            performReplay();
        }

        private void replay()
        {

            replayProgressBar.Invoke((MethodInvoker)(() =>
            {
                replayProgressBar.Maximum = _mouseEvents.GetCount();
            }));

            long i = 0;
            do
            {
                IEnumerable<LinkedMouseEvent> enumerable = _mouseEvents.GetEvents();
                IEnumerator<LinkedMouseEvent> enumerator = enumerable.GetEnumerator();

                int replayProgress = 0;

                while (enumerator.MoveNext())
                {
                    var ev = enumerator.Current;
                    long timeDiff = ev.GetTimeDifference();

                    var cev = ev.CurrentEvent;

                    if (timeDiff > 0)
                    {
                        try
                        {
                            Thread.Sleep((int)timeDiff);
                        }
                        catch (ThreadInterruptedException)
                        {
                            return;
                        }
                    }

                    SetCursorPos(cev.X, cev.Y);

                    if (cev.Action == IMouseEvent.EventAction.CLICK)
                    {
                        OnMouseClick(cev.Button);
                    }
                    else if (cev.Action == IMouseEvent.EventAction.DOUBLE_CLICK)
                    {
                        OnMouseClick(cev.Button);
                        OnMouseClick(cev.Button);
                    }
                    else if (cev.Action == IMouseEvent.EventAction.MOVE)
                    {
                        try
                        {
                            trace(cev.Series);
                        }
                        catch (ThreadInterruptedException)
                        {
                            return;
                        }
                    }
                    else if (cev.Action == IMouseEvent.EventAction.DRAG) {

                        OnMouseDown(cev.Button);
                        try
                        {
                            trace(cev.Series);
                        }
                        catch (ThreadInterruptedException)
                        {
                            return;
                        }
                        OnMouseUp(cev.Button);
                    }


                    replayProgress++;

                    replayProgressBar.Invoke((MethodInvoker)(() =>
                    {
                        replayProgressBar.Value = replayProgress;
                        eventsProgressLabel.Text = replayProgress + "/" + _mouseEvents.GetCount();
                    }));
                }

                i++;

                label1.Invoke((MethodInvoker)(() => label1.Text = "Replays: " + i));

            } while (true);
        }

        private void trace(List<ITimedCoordinates> series)
        {
            long lastTimestamp = 0L;

            foreach (var tc in series)
            {

                if (lastTimestamp > 0)
                {
                    long diff = tc.Timestamp - lastTimestamp;

                    Thread.Sleep((int)diff);
                }

                SetCursorPos(tc.X, tc.Y);
                lastTimestamp = tc.Timestamp;
            }
        }

        private void performRecord()
        {
            recording = !recording;

            if (recording)
            {
                _mouseEvents.Clear();
                Subscribe();
                recordButton.Text = "Stop Recording";
                replayButton.Enabled = false;
            }
            else
            {
                Unsubscribe();
                recordButton.Text = "Record";
                replayButton.Enabled = true;
            }
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            if (replaying)
            {
                return;
            }

            if (recording)
            {
                _mouseEvents.RemoveLast();
            }

            performRecord();
        }

        public void OnEventReceived(IDeviceEvent deviceEvent)
        {
            var mouseEvent = (IMouseEvent)deviceEvent;


            lock (_mouseEvents)
            {
                _mouseEvents.AddMouseEvent(mouseEvent);
            }
        }

        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
