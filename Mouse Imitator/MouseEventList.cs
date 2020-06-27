using MouseImitator.Application.Services.Hooks.Mouse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mouse_Imitator
{
    [Serializable]
    class MouseEventList
    {

        private LinkedList<LinkedMouseEvent> Events;

        public MouseEventList()
        {
            Events = new LinkedList<LinkedMouseEvent>();
        }

        public void AddMouseEvent(IMouseEvent e)
        {
            lock (Events)
            {
                var last = Events.Count > 0 ?
                    Events.Last.Value.CurrentEvent :
                    null;

                var newEvent = new LinkedMouseEvent
                {
                    CurrentEvent = e,
                    LastEvent = last
                };
                Events.AddLast(newEvent);
            }
        }

        public int GetCount()
        {
            lock (Events)
            {
                return Events.Count;
            }
        }

        public LinkedMouseEvent GetMouseEvent(int index)
        {
            lock (Events)
            {
                int i = 0;
                LinkedList<LinkedMouseEvent>.Enumerator enumerator = Events.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (i == index)
                    {
                        return enumerator.Current;
                    }
                    i++;
                }
                return null;
            }
        }

        public IEnumerable<LinkedMouseEvent> GetEvents()
        {
            lock (Events)
            {
                return Events;
            }
        }

        public void RemoveLast()
        {
            if (this.Events.Count > 0)
            {
                this.Events.RemoveLast();
            }

        }
        public void Clear()
        {
            lock (Events)
            {
                Events.Clear();
            }
        }
    }
}
