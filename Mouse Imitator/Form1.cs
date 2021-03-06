﻿using Gma.System.MouseKeyHook;
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
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseImitator.Application;

namespace Mouse_Imitator
{
    public partial class Form1 : Form, IDeviceEventListener
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;

        private static IntPtr _hookId = IntPtr.Zero;

        private readonly MouseHook _mouseHook;
        private readonly LowLevelKeyboardProc _proc;

        private Thread _workerThread;

        private MouseEventList _mouseEvents;
        private State _currentState;


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
            this._proc = HookCallback;
            this.replayButton.Enabled = false;

            _mouseHook.Register(this);

            toolStripMenuItem3.Click += SaveMenuItem_Click;
            toolStripMenuItem4.Click += LoadMenuItem_Click;
        }

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            var filter = TextResources.files_filter;
            using var openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileName = openFileDialog.FileName;
            var serializedEvents = File.ReadAllText(fileName);
            _mouseEvents = DeserializeFromString<MouseEventList>(serializedEvents);

            replayButton.Enabled = _mouseEvents.GetCount() > 0;
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            var filter = TextResources.files_filter;

            using var saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                CheckFileExists = false
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileName = saveFileDialog.FileName;
            var serializedEvents = SerializeToString(_mouseEvents);
            File.WriteAllText(fileName, serializedEvents);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WhKeyboardLl, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr) WmKeydown)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if ((Keys) vkCode == Keys.F6)
                {
                    PerformReplay();
                }
                else if ((Keys) vkCode == Keys.F7)
                {
                    PerformRecord();
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod,
            uint dwThreadId);

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

            //It is recommended to dispose it
            _mouseHook.Dispose();
            UnhookWindowsHookEx(_hookId);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            _hookId = SetHook(_proc);
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
                    mouse_event((uint) MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                    mouse_event((uint) MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.RIGHT:
                    mouse_event((uint) MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event((uint) MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.MIDDLE:
                    mouse_event((uint) MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
                    mouse_event((uint) MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
                    break;
            }
        }

        public static void OnMouseDown(IMouseEvent.EventButton button)
        {
            switch (button)
            {
                case IMouseEvent.EventButton.LEFT:
                    mouse_event((uint) MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.RIGHT:
                    mouse_event((uint) MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.MIDDLE:
                    mouse_event((uint) MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
                    break;
            }
        }

        public static void OnMouseUp(IMouseEvent.EventButton button)
        {
            switch (button)
            {
                case IMouseEvent.EventButton.LEFT:
                    mouse_event((uint) MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.RIGHT:
                    mouse_event((uint) MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
                    break;
                case IMouseEvent.EventButton.MIDDLE:
                    mouse_event((uint) MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
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

        private bool HasMouseEvents()
        {
            return _mouseEvents.GetCount() > 0;
        }

        private void PerformReplay()
        {
            if (!HasMouseEvents())
            {
                return;
            }

            if (IsIdle())
            {

                Reset();

                replayButton.Text = TextResources.stop_replaying;
                recordButton.Enabled = false;

                this._workerThread = new Thread(new ThreadStart(this.Replay));
                this._workerThread.Start();

                UpdateState(State.REPLAYING);
            }
            else if (IsReplaying())
            {
                _workerThread.Interrupt();
                replayButton.Text = TextResources.replay;
                recordButton.Enabled = true;

                UpdateState(State.IDLE);

                //We don't want to reset the UI here, maybe the user wants to view where he stopped
            }
        }

        private void Reset()
        {

            label1.Invoke((MethodInvoker)(() => label1.Text = TextResources.no_replays));
            replayProgressBar.Invoke((MethodInvoker)(() =>
            {
                replayProgressBar.Value = 0;
                eventsProgressLabel.Text = null;
            }));
        }

        private void UpdateState(State newState)
        {
            _currentState = newState;

            var stateText = TextResources.idle;

            switch (_currentState)
            {
                case State.RECORDING:
                    stateText = TextResources.recording;
                    break;
                case State.REPLAYING:
                    stateText = TextResources.replaying;
                    break;
            }

            currentStateLabel.Text = stateText;

        }

        private bool IsRecording()
        {
            return _currentState == State.RECORDING;
        }

        private bool IsIdle()
        {
            return _currentState == State.IDLE;
        }

        private bool IsReplaying()
        {
            return _currentState == State.REPLAYING;
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            if (IsRecording())
            {
                return;
            }

            PerformReplay();
        }

        private void Replay()
        {
            replayProgressBar.Invoke((MethodInvoker) (() => { replayProgressBar.Maximum = _mouseEvents.GetCount(); }));

            long i = 0;
            do
            {
                lock (_mouseEvents)
                {
                    IEnumerable<LinkedMouseEvent> enumerable = _mouseEvents.GetEvents();
                    using IEnumerator<LinkedMouseEvent> enumerator = enumerable.GetEnumerator();

                    int replayProgress = 0;

                    while (enumerator.MoveNext())
                    {
                        var ev = enumerator.Current;

                        if (ev == null)
                        {
                            break;
                        }

                        long timeDiff = ev.GetTimeDifference();

                        var cev = ev.CurrentEvent;

                        if (timeDiff > 0)
                        {
                            try
                            {
                                Thread.Sleep((int) timeDiff);
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
                                Trace(cev.Series);
                            }
                            catch (ThreadInterruptedException)
                            {
                                return;
                            }
                        }
                        else if (cev.Action == IMouseEvent.EventAction.DRAG)
                        {
                            OnMouseDown(cev.Button);
                            try
                            {
                                Trace(cev.Series);
                            }
                            catch (ThreadInterruptedException)
                            {
                                return;
                            }

                            OnMouseUp(cev.Button);
                        }


                        replayProgress++;

                        var progress = replayProgress;
                        replayProgressBar.Invoke((MethodInvoker) (() =>
                        {
                            replayProgressBar.Value = progress;
                            eventsProgressLabel.Text = String.Format(TextResources.steps_progress, progress, _mouseEvents.GetCount());
                        }));
                    }
                }

                i++;

                label1.Invoke((MethodInvoker) (() => label1.Text = String.Format(TextResources.replays_progress, i)));
            } while (true);
        }

        private void Trace(List<ITimedCoordinates> series)
        {
            long lastTimestamp = 0L;

            foreach (var tc in series)
            {
                if (lastTimestamp > 0)
                {
                    long diff = tc.Timestamp - lastTimestamp;

                    Thread.Sleep((int) diff);
                }

                SetCursorPos(tc.X, tc.Y);
                lastTimestamp = tc.Timestamp;
            }
        }

        private void PerformRecord()
        {
            if (IsRecording())
            {
                Unsubscribe();
                recordButton.Text = TextResources.record;
                replayButton.Enabled = true;

                UpdateState(State.IDLE);
            }
            else if (IsIdle())
            {
                _mouseEvents.Clear();
                Subscribe();
                recordButton.Text = TextResources.stop_recording;
                replayButton.Enabled = false;

                UpdateState(State.RECORDING);
            }
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            if (IsReplaying())
            {
                return;
            }

            if (IsRecording())
            {
                _mouseEvents.RemoveLast();
            }

            PerformRecord();
        }

        public void OnEventReceived(IDeviceEvent deviceEvent)
        {
            var mouseEvent = (IMouseEvent) deviceEvent;


            lock (_mouseEvents)
            {
                _mouseEvents.AddMouseEvent(mouseEvent);
            }
        }

        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using var stream = new MemoryStream(b);
            var formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            return (TData) formatter.Deserialize(stream);
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, settings);
            stream.Flush();
            stream.Position = 0;
            return Convert.ToBase64String(stream.ToArray());
        }
    }
}