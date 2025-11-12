using System;
using Core.Scripts.Game.Infrastructure.StateMachines.BaseData;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views
{
    public sealed class GameMenuUIParentView : MonoBehaviour
    {
        [SerializeField] private RectTransform bodyRT;

        [SerializeField] private Canvas canvas;
        [SerializeField] public CanvasGroup canvasGroup;

        [SerializeField] internal GameMenuUIFadeConfig fadeConfig;
        [SerializeField] internal Image fadeImage;
        
        private MainGameUIStateMachine _stateMachine;

        public void Setup(MainGameUIStateMachine gameMenuUIStateMachine, Camera c)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = c;
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