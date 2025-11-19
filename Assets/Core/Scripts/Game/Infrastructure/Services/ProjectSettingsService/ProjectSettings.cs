using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService
{
    public sealed class ProjectSettings : IProjectSettings
    {
        public bool IsGamePaused { get; private set; } = false;
        
        void IProjectSettings.SetCursor(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked; 
        }

        void IProjectSettings.ChangeGamePauseStatus(bool visible)
        {
            IsGamePaused = visible;
        }
    }
}