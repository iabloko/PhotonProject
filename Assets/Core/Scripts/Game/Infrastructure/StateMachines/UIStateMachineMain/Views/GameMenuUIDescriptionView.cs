using Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views.Base;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.StateMachines.UIStateMachineMain.Views
{
    [System.Serializable, HideLabel]
    public struct DescriptionBlock
    {
        [Required] public Button startDescriptionButton;
        [Required] public CanvasGroup middleDescriptionUI;
    }

    public sealed class GameMenuUIDescriptionView : GameMenuUIViewBase
    {
        [Title("DESCRIPTION", "BLOCK", TitleAlignments.Right), SerializeField]
        private DescriptionBlock descriptionBlock;

        // [Title("BASE", "BLOCK", TitleAlignments.Right), Required]
        // public Button startEasyGame;
        // [Required] public Button startHardGame;

        // [SerializeField] private PortraitInformation[] portraitInformations;

        // private DescriptionAnimator _descriptionAnimator;

        // public void Setup(HashSet<Banknote> banknotes)
        // {
        //     _descriptionAnimator = new DescriptionAnimator(portraitInformations, descriptionBlock, banknotes);
        // }

        protected override void OnBeforeOpen() => SetupStartLogic();

        protected override void OnOpened()
        {
            base.OnOpened();
            
            StartAnimationsWithDelay().Forget();
        }

        private async UniTaskVoid StartAnimationsWithDelay()
        {
            await UniTask.Delay(3000);
            // _descriptionAnimator.StartPortraitAnimations();
        }

        protected override void OnBeforeClosed()
        {
            // _descriptionAnimator.StopAnimations();
        }

        private void SetupStartLogic()
        {
            descriptionBlock.startDescriptionButton.gameObject.SetActive(true);

            descriptionBlock.middleDescriptionUI.interactable = false;
            descriptionBlock.middleDescriptionUI.blocksRaycasts = false;
            descriptionBlock.middleDescriptionUI.alpha = 0;

            descriptionBlock.startDescriptionButton.onClick.RemoveAllListeners();
            descriptionBlock.startDescriptionButton.onClick.AddListener(ShowDescription);
        }

        private void ShowDescription()
        {
            descriptionBlock.startDescriptionButton.gameObject.SetActive(false);

            descriptionBlock.middleDescriptionUI.interactable = true;
            descriptionBlock.middleDescriptionUI.blocksRaycasts = true;
            descriptionBlock.middleDescriptionUI.DOFade(1, .5f);

            // _descriptionAnimator.StopAnimations();
        }
    }
}