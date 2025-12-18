using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Game.ScriptableObjects.Configs.Logger
{
    public enum LogLevel
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
    }
    
    [CreateAssetMenu(menuName = "Settings/Configs/GameLogger", fileName = "GameLogger"), InlineEditor, HideLabel]
    public sealed class GameLogger : ScriptableObject
    {
        public LogLevel logLevel;

        public void Log<T>(LogLevel level, string message)
        {
#if UNITY_EDITOR
            if (!IsLogEnabled(level)) return;

            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log($"{typeof(T).Name}: {message}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"{typeof(T).Name}: {message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"{typeof(T).Name}: {message}");
                    break;
            }
#endif
        }
        
        private bool IsLogEnabled(LogLevel messageLevel) =>
            logLevel != LogLevel.None && messageLevel >= logLevel;
    }
}