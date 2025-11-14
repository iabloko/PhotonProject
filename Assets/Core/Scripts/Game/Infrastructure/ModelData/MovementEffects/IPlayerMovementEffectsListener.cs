using System;

namespace Sandbox.Project.Scripts.Infrastructure.ModelData.MovementEffects
{
    public interface IPlayerMovementEffectsListener
    {
        public event Action<MovementEffectData> StartEvent;
        public void StartMovementEffect(MovementEffectData type);
    }
}