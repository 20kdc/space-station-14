#nullable enable
using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.GameObjects.Components.Power
{
    [Serializable, NetSerializable]
    public class PowerMonitoringConsoleBoundInterfaceState : BoundUserInterfaceState
    {
        public PowerMonitoringConsoleBoundInterfaceState()
        {
        }
    }

    [Serializable, NetSerializable]
    public enum PowerMonitoringConsoleUiKey
    {
        Key
    }
}
