using Fusion;

namespace Core.Scripts.Game.CharacterLogic
{
    public sealed class RunnerTimeSource : ITimeSource
    {
        private readonly NetworkRunner _runner;
        public RunnerTimeSource(NetworkRunner runner) => _runner = runner;

        public int Tick => _runner.Tick;
        public float DeltaTime => _runner.DeltaTime;
    }
}