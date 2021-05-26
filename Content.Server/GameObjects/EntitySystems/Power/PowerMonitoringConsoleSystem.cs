#nullable enable
using Content.Server.GameObjects.Components.Power.PowerNetComponents;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;

namespace Content.Server.GameObjects.EntitySystems
{
    /// <summary>
    /// Responsible for updating power monitoring consoles.
    /// Theoretically, these could be made event-driven.
    /// In practice, that at most ought to be limited to a "dirty" boolean as it runs the risk of updating them too often.
    /// </summary>
    [UsedImplicitly]
    internal sealed class PowerMonitoringConsoleSystem : EntitySystem
    {
        /// <summary>
        /// Timer used to avoid updating the UI state every frame (which would be overkill)
        /// </summary>
        private float _updateTimer;

        public override void Update(float frameTime)
        {
            _updateTimer += frameTime;
            if (_updateTimer >= 2)
            {
                _updateTimer -= 2;
                foreach (var component in ComponentManager.EntityQuery<PowerMonitoringConsoleComponent>(true))
                {
                    if (component.AnyoneWatchingMe)
                    {
                        component.UpdateUIState();
                    }
                }
            }
        }
    }
}
