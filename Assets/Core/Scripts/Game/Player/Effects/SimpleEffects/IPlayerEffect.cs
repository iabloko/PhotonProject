namespace Core.Scripts.Game.Player.Effects.SimpleEffects
{
    public interface IPlayerEffect
    {
        public void OnPlayerMovement();
        public void OnPlayerStop();
        public void OnUpdateCall();
    }
}