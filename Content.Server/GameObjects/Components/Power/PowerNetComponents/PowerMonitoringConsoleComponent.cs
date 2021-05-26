#nullable enable
using System.Collections.Generic;
using Content.Server.GameObjects.Components.Power.ApcNetComponents;
using Content.Server.GameObjects.EntitySystems;
using Content.Server.Utility;
using Content.Shared.GameObjects.Components.Power;
using Content.Shared.Interfaces.GameObjects.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameObjects.Components.Power.PowerNetComponents
{
    [RegisterComponent]
    [ComponentReference(typeof(IActivate))]
    public class PowerMonitoringConsoleComponent : BaseComputerUserInterfaceComponent
    {
        public override string Name => "PowerMonitoringConsole";
        public PowerMonitoringConsoleComponent() : base(PowerMonitoringConsoleUiKey.Key) {}

        public override void Initialize()
        {
            base.Initialize();

            UpdateUIState();
        }

        public void UpdateUIState()
        {
            var totalSources = 121.0;
            var totalLoads = 212.0;
            var sources = new List<PowerMonitoringConsoleEntry>();
            var loads = new List<PowerMonitoringConsoleEntry>();
            sources.Add(new PowerMonitoringConsoleEntry("IPI", "SalternSubstation", 1234));
            UserInterface?.SetState(new PowerMonitoringConsoleBoundInterfaceState(totalSources, totalLoads, sources.ToArray(), loads.ToArray()));
        }
    }
}

