namespace Core.Scripts.Game.CharacterLogic
{
    public interface ITimeSource
    {
        public int Tick { get; }
        public float DeltaTime { get; }
    }
}