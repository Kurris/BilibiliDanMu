using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBackgroundService.Enums;


/// <summary>The commands that can be used as arguments to <see cref="GetNextWindow" />.</summary>
public enum GetNextWindowCommands
{
    /// <summary>Returns a handle to the window below the given window.</summary>
    GW_HWNDNEXT = GetWindowCommands.GW_HWNDNEXT,

    /// <summary>Returns a handle to the window above the given window.</summary>
    GW_HWNDPREV = GetWindowCommands.GW_HWNDPREV,
}
