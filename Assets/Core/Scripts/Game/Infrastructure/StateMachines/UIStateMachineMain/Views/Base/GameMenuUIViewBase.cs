using System;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views.Base
{
    public abstract class GameMenuUIViewBase : UIAsyncPayloadView
    {
        [SerializeField] public CanvasGroup canvasGroup;

        public virtual void Setup()
        {
        }

        public override async UniTask Close()
        {
            OnBeforeClosed();
            CloseView(OnClosed);

            while (IsOpened) await UniTask.DelayFrame(1);
        }

        public override async UniTask Open()
        {
            OnBeforeOpen();
            OpenView(OnOpened);

            while (!IsOpened) await UniTask.DelayFrame(1);
        }

        protected virtual void CloseView(Action onClosed)
        {
            //Tween = CanvasGroup.DOFade(0, 0.25f)
            //                          .SetDelay(0.0f)
            //                          .SetEase(Ease.Linear)
            //                          .OnComplete(OnComplete);

            canvasGroup.alpha = 0;
            OnComplete();
            return;

            void OnComplete()
            {
                onClosed();
            }
        }

        protected virtual void OpenView(Action onOpened)
        {
            //Tween = CanvasGroup.DOFade(1, 0.5f)
            //                                 .SetDelay(0.0f)
            //                                 .SetEase(Ease.InSine)
            //                                 .OnComplete(OnComplete);

            canvasGroup.alpha = 1;
            OnComplete();
            return;

            void OnComplete()
            {
                onOpened();
            }
        }
    }
}