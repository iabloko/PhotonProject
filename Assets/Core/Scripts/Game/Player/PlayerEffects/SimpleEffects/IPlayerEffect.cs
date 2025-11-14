namespace Core.Scripts.Game.Player.PlayerEffects.SimpleEffects
{
    public interface IPlayerEffect
    {
        public void OnPlayerMovement();
        public void OnPlayerStop();
        public void OnUpdateCall();
    }
}