using System;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views.Base;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views
{
    [Serializable]
    public struct AdditionalDescription
    {
        public CanvasGroup canvasGroup;
        public Button portraitButton;
        public Button textButton;
    }

    public sealed class GameMenuUIGamePlayView : GameMenuUIViewBase
    {
        public Button toMainView, repeatGame;
        public RectTransform congratulations;

        [SerializeField, BoxGroup("TMP Text")]
        private TMP_Text bender,
            chichikov,
            lopahin,
            raskolnikov,
            bogrov,
            other,
            peresvetova,
            shurik,
            erzac,
            congratulationsResult;

        [SerializeField, BoxGroup("AdditionalDescription")]
        private AdditionalDescription[] descriptions;
        
        public void RepeatGame()
        {
            toMainView.gameObject.SetActive(true);
            MoveUI(-2160, 0, .1f);
        }

        protected override void OnBeforeOpen()
        {
            toMainView.gameObject.SetActive(true);
            AdditionalDescriptionLogic();
        }
        
        protected override void OnBeforeClosed()
        {
            DOTween.Kill(congratulations);
        }
        
        private void MoveUI(int endValue, float delay, float duration)
        {
            congratulations.DOLocalMoveY(endValue, duration).SetDelay(delay).SetEase(Ease.InOutSine);
        }
        
        private void AdditionalDescriptionLogic()
        {
            RemoveAllListeners();
            SubscribeEvents();
            DisableAdditionalTexts();
        }

        private void SubscribeEvents()
        {
            foreach (AdditionalDescription description in descriptions)
            {
                description.textButton.onClick.AddListener(DisableAdditionalTexts);
                description.portraitButton.onClick.AddListener(() => ToggleDescriptionLogic(description));
            }
        }

        private void ToggleDescriptionLogic(AdditionalDescription description)
        {
            if (Mathf.Approximately(description.canvasGroup.alpha, 1))
            {
                description.canvasGroup.alpha = 0;
                description.canvasGroup.blocksRaycasts = false;
                description.canvasGroup.interactable = false;
            }
            else
            {
                DisableAdditionalTexts();

                description.canvasGroup.alpha = 1;
                description.canvasGroup.blocksRaycasts = true;
                description.canvasGroup.interactable = true;
            }
        }

        private void DisableAdditionalTexts()
        {
            foreach (AdditionalDescription d in descriptions)
            {
                d.canvasGroup.alpha = 0;
                d.canvasGroup.blocksRaycasts = false;
                d.canvasGroup.interactable = false;
            }
        }

        private void RemoveAllListeners()
        {
            foreach (AdditionalDescription description in descriptions)
            {
                description.textButton.onClick.RemoveAllListeners();
                description.portraitButton.onClick.RemoveAllListeners();
            }
        }
    }
}