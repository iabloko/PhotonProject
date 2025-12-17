namespace Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService
{
    public interface IProjectSettings
    {
        public bool IsGamePaused { get; }
        
        public void SetCursor(bool status);
        
        public void ChangeGamePauseStatus(bool status);
    }
}