#nullable enable
using System;
using System.Collections.Generic;
using Content.Server.GameObjects.Components.Power.PowerNetComponents;
using Content.Server.GameObjects.Components.NodeContainer.NodeGroups;
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
        [ViewVariables]
        [ComponentDependency] private readonly PowerMonitoringConsoleConnectorComponent? _connector = default!;
        public PowerMonitoringConsoleComponent() : base(PowerMonitoringConsoleUiKey.Key) {}

        public override void Initialize()
        {
            base.Initialize();

            UpdateUIState();
        }

        public override void ComputerJustBeforeFirstObserve()
        {
            UpdateUIState();
        }

        public void UpdateUIState()
        {
            var totalSources = 0.0d;
            var totalLoads = 0.0d;
            var sources = new List<PowerMonitoringConsoleEntry>();
            var loads = new List<PowerMonitoringConsoleEntry>();
            PowerMonitoringConsoleEntry LoadOrSource(Component comp, double rate)
            {
                var prototype = comp.Owner.Prototype?.ID ?? "";
                return new PowerMonitoringConsoleEntry(comp.Owner.Name, prototype, rate);
            }
            if (_connector != null)
            {
                // Right, so, here's what needs to be considered here.
                var net = _connector.Net;
                foreach (Priority priority in Enum.GetValues(typeof(Priority)))
                {
                    var list = net.ConsumersByPriority.GetValueOrDefault(priority);
                    if (list == null)
                        continue;
                    foreach (PowerConsumerComponent pcc in list)
                    {
                        loads.Add(LoadOrSource(pcc, pcc.DrawRate));
                        totalLoads += pcc.DrawRate;
                    }
                }
                foreach (PowerSupplierComponent pcc in net.Suppliers)
                {
                    sources.Add(LoadOrSource(pcc, pcc.SupplyRate));
                    totalSources += pcc.SupplyRate;
                }
            }
            // Actually set state.
            UserInterface?.SetState(new PowerMonitoringConsoleBoundInterfaceState(totalSources, totalLoads, sources.ToArray(), loads.ToArray()));
        }
    }

    [RegisterComponent]
    public class PowerMonitoringConsoleConnectorComponent : BasePowerNetComponent
    {
        public override string Name => "PowerMonitoringConsoleConnector";

        protected override void AddSelfToNet(IPowerNet net) {}

        protected override void RemoveSelfFromNet(IPowerNet net) {}
    }
}

