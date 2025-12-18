namespace Core.Scripts.Game.CharacterLogic
{
    public interface ITimeSource
    {
        int Tick { get; }
        float DeltaTime { get; }
    }
}