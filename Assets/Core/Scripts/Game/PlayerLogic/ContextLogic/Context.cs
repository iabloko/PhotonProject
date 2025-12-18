using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Fusion;
using Fusion.Addons.SimpleKCC;

namespace Core.Scripts.Game.PlayerLogic.ContextLogic
{
    public class Context
    {
        protected readonly bool Authority;

        protected Context(bool authority, SimpleKCC kcc, NetworkRunner runner, GameplaySettings gameplayData, IProjectSettings projectSettings)
        {
            Authority = authority;
            Kcc = kcc;
            Runner = runner;
            GameplayData = gameplayData;
            ProjectSettings = projectSettings;
        }

        public SimpleKCC Kcc { get; }
        public NetworkRunner Runner { get; }
        public GameplaySettings GameplayData { get; }
        public IProjectSettings ProjectSettings { get; }
    }
}