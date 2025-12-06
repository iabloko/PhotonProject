using System;
using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views.Base;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views
{
    public sealed class GameMenuUIDescriptionView : GameMenuUIViewBase
    {
        [Title("DESCRIPTION", "BLOCK", TitleAlignments.Right), SerializeField, Required]
        public Button startGame;
        [Required] public CanvasGroup middleDescriptionUI;
        public event Action StartGameButtonClicked;
        
        protected override void OnBeforeOpen() => SetupStartLogic();

        protected override void OnBeforeClosed() => startGame.onClick.RemoveAllListeners();

        private void SetupStartLogic()
        {
            startGame.gameObject.SetActive(true);

            middleDescriptionUI.interactable = true;
            middleDescriptionUI.blocksRaycasts = true;
            middleDescriptionUI.DOFade(1, .5f);

            startGame.onClick.RemoveAllListeners();
            startGame.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            StartGameButtonClicked?.Invoke();
            startGame.gameObject.SetActive(false);
            
            middleDescriptionUI.interactable = false;
            middleDescriptionUI.blocksRaycasts = false;
            middleDescriptionUI.alpha = 0;
        }
    }
}