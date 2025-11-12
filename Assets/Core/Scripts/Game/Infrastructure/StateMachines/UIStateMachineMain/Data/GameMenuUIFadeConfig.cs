using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Data
{
    [CreateAssetMenu(menuName = "Game/FadeData", fileName = "FadeData")]
    public sealed class GameMenuUIFadeConfig : ScriptableObject, IPayload
    {
        public float duration = 1f;
        public Ease easeType = Ease.Linear;
        public float delay = 1f;
    }
}