using Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;

namespace Core.Scripts.Game.InteractionObjects
{
    public sealed class PrimaryPortal : Portal
    {
        public override bool IsInteractable => true;
        public override InteractionObjectType Type => InteractionObjectType.PRIMARY_PORTAL;
        
        public override void UpdatePortalKey(int prefix)
        {
            PortalID = prefix;
            PortalKey = string.Concat(prefix, "_", nameof(PortalKeys.PrimaryPortal));
        }
    }
}