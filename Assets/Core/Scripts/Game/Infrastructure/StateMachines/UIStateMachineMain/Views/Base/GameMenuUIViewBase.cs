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