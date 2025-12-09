using Fusion;

namespace Core.Scripts.Game.PlayerLogic.Combat
{
    public sealed class PlayerCombat : CombatNetworkBehaviour
    {
        public override void Spawned()
        {
            base.Spawned();
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }
    }
}