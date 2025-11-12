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
        // public GamePlayAfkTimer AfkTimer;

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
        
        public void SetUpGameMenuUIGamePlayView()
        {
            UpdateBanknotesCount();
        }

        public void RepeatGame()
        {
            toMainView.gameObject.SetActive(true);
            MoveUI(-2160, 0, .1f);
            UpdateBanknotesCount();
        }

        protected override void OnBeforeOpen()
        {
            // AfkTimer = new GamePlayAfkTimer();
            toMainView.gameObject.SetActive(true);
            AdditionalDescriptionLogic();
        }
        
        protected override void OnBeforeClosed()
        {
            DOTween.Kill(congratulations);
            
            // _gameLogic.OnBanknoteValidated -= UpdateScore;
            // _gameLogic.OnGameFinished -= OnGameFinished;
        }

        private void OnGameFinished()
        {
            toMainView.gameObject.SetActive(false);
            UpdateCongratulationsText();
            MoveUI(0, 1.5f, .5f);
        }

        private void UpdateCongratulationsText()
        {
            // const string congratulationsTextMask = "Количество ошибок: {0} \nВремя игры: {1}";
            // string formattedPlayTime = "";

            // if (_gameLogic.PlayTime.Days > 0)
            // {
            //     formattedPlayTime += $"{_gameLogic.PlayTime.Days} д. ";
            // }
            //
            // if (_gameLogic.PlayTime.Hours > 0)
            // {
            //     formattedPlayTime += $"{_gameLogic.PlayTime.Hours} ч. ";
            // }
            //
            // if (_gameLogic.PlayTime.Minutes > 0)
            // {
            //     formattedPlayTime += $"{_gameLogic.PlayTime.Minutes} м. ";
            // }
            //
            // formattedPlayTime += $"{_gameLogic.PlayTime.Seconds} с.";

            // congratulationsResult.text =
            //     string.Format(congratulationsTextMask, _gameLogic.FailedAttempts, formattedPlayTime);
        }

        private void MoveUI(int endValue, float delay, float duration)
        {
            congratulations.DOLocalMoveY(endValue, duration).SetDelay(delay).SetEase(Ease.InOutSine);
        }

        private void UpdateBanknotesCount()
        {
            // foreach (BanknoteTags banknoteTags in Enum.GetValues(typeof(BanknoteTags)))
            // {
            //     UpdateScore(banknoteTags, _gameLogic.GetBanknotesInGame(banknoteTags));
            // }
        }

        // private void UpdateScore(BanknoteTags banknoteTag, int currentScore)
        // {
        //     switch (banknoteTag)
        //     {
        //         case BanknoteTags.Bender:
        //             bender.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Chichikov:
        //             chichikov.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Lopahin:
        //             lopahin.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Raskolnikov:
        //             raskolnikov.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Bogrov:
        //             bogrov.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Other:
        //             other.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Peresvetova:
        //             peresvetova.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Shurik:
        //             shurik.text = currentScore.ToString();
        //             break;
        //         case BanknoteTags.Erzac:
        //             erzac.text = currentScore.ToString();
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(banknoteTag), banknoteTag, null);
        //     }
        // }

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

        private void Update()
        {
            // AfkTimer.UpdateTimer();
        }
    }
}