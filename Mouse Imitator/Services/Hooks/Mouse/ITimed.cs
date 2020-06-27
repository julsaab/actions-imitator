using System;
using System.Collections.Generic;
using System.Text;

namespace MouseImitator.Application.Services.Hooks.Mouse
{
    public interface ITimed
    {
        public long Timestamp { get; }
    }
}
