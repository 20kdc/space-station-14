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
        public double TotalSources;
        public double TotalLoads;
        public PowerMonitoringConsoleEntry[] Sources;
        public PowerMonitoringConsoleEntry[] Loads;
        public PowerMonitoringConsoleBoundInterfaceState(double totalSources, double totalLoads, PowerMonitoringConsoleEntry[] sources, PowerMonitoringConsoleEntry[] loads)
        {
            TotalSources = totalSources;
            TotalLoads = totalLoads;
            Sources = sources;
            Loads = loads;
        }
    }

    [Serializable, NetSerializable]
    public class PowerMonitoringConsoleEntry
    {
        public string NameLocalized;
        public string IconEntityPrototypeId;
        public double Size;
        public PowerMonitoringConsoleEntry(string nl, string ipi, double size)
        {
            NameLocalized = nl;
            IconEntityPrototypeId = ipi;
            Size = size;
        }
    }

    [Serializable, NetSerializable]
    public enum PowerMonitoringConsoleUiKey
    {
        Key
    }
}
