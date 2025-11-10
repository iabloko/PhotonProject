using System;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views
{
    public sealed class GameMenuUIParentView : MonoBehaviour
    {
        [SerializeField] private RectTransform bodyRT;

        [SerializeField] private Canvas canvas;
        [SerializeField] public CanvasGroup canvasGroup;

        private MainGameUIStateMachine _stateMachine;

        public void Setup(MainGameUIStateMachine gameMenuUIStateMachine)
        {
            _stateMachine = gameMenuUIStateMachine;
        }

        public void ChangeSortOrder(int order)
        {
            canvas.sortingOrder = order;
        }

        internal void Close(bool withAnimation, Action closed = null)
        {
            CloseView(withAnimation, () => { closed?.Invoke(); });
        }

        internal void Open(bool withAnimation, Action opened = null)
        {
            OpenView(withAnimation, () => { opened?.Invoke(); });
        }

        internal void SetChildView(UIAsyncPayloadView stateView)
        {
            stateView.transform.SetParent(bodyRT, false);
        }

        private void CloseView(bool withAnimation, Action onClosed)
        {
            if (withAnimation)
            {
                canvasGroup.DOFade(0, 0.5f)
                    .SetDelay(0.0f)
                    .SetEase(Ease.InSine)
                    .OnComplete(OnComplete);
            }
            else
            {
                canvasGroup.alpha = 0;
                OnComplete();
            }

            return;

            void OnComplete()
            {
                onClosed();
            }
        }

        private void OpenView(bool withAnimation, Action onOpened)
        {
            if (withAnimation)
            {
                canvasGroup.DOFade(1, 0.5f)
                    .SetDelay(0.0f)
                    .SetEase(Ease.InSine)
                    .OnComplete(OnComplete);
            }
            else
            {
                canvasGroup.alpha = 1;
                OnComplete();
            }

            return;

            void OnComplete()
            {
                onOpened();
            }
        }
    }
}