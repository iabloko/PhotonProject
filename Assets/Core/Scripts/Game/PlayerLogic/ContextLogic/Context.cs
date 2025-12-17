using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Fusion;
using Fusion.Addons.SimpleKCC;

namespace Core.Scripts.Game.PlayerLogic.ContextLogic
{
    public class Context
    {
        protected readonly bool Authority;

        protected Context(bool authority, SimpleKCC kcc, NetworkRunner runner, RoomSettings roomData, IProjectSettings projectSettings)
        {
            Authority = authority;
            Kcc = kcc;
            Runner = runner;
            RoomData = roomData;
            ProjectSettings = projectSettings;
        }

        public SimpleKCC Kcc { get; }
        public NetworkRunner Runner { get; }
        public RoomSettings RoomData { get; }
        public IProjectSettings ProjectSettings { get; }
    }
}