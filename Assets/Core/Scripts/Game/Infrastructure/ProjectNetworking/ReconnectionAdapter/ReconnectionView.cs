using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.ReconnectionAdapter
{
    public sealed class ReconnectionView : MonoBehaviour
    {
        [SerializeField] private Button reconnectButton;
        [SerializeField] private Button returnToLobbyButton;
        [SerializeField] private TMP_Text reconnectionText;
        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform spinner;
        
        private INetworkService _networkService;
        private GameStateMachine _stateMachine;

        public void Constructor(GameStateMachine stateMachine, INetworkService networkService)
        {
            _stateMachine = stateMachine;
            _networkService = networkService;
            StartLogic();
        }

        private void StartLogic()
        {
            reconnectionText.text = string.Concat("Disconnect:", _networkService.CurrentShutdownReason.ToString());
            
            reconnectButton.onClick.AddListener(() => OnReconnectionClicked().Forget());
            returnToLobbyButton.onClick.AddListener(OnReturnToLobbyClicked);

            string reconnectionSessionName = _networkService.CurrentSessionName;
            bool isReconnectionSessionExist = !string.IsNullOrEmpty(reconnectionSessionName);
            reconnectButton.gameObject.SetActive(isReconnectionSessionExist);
        }

        private void OnDisable()
        {
            reconnectButton.onClick.RemoveAllListeners();
            returnToLobbyButton.onClick.RemoveAllListeners();
        }

        private void OnReturnToLobbyClicked()
        {
            DisableButtons();
            _stateMachine.Enter<LoadLevelState, LoadLevelData>(new LoadLevelData(
                AssetPaths.TRANSITION_SCENE, () => _stateMachine.Enter<PhotonLobbyState>()));
        }

        private async UniTaskVoid OnReconnectionClicked()
        {
            DisableButtons();
            bool result = await _networkService.ConnectToSession(_networkService.CurrentSessionName);
            
            if (!result)
            {
                reconnectionText.text = string.Concat("Connection error");
                EnableButtons();
            }
            else
            {
                _stateMachine.Enter<GamePlayState>();
            }
        }

        private void EnableButtons()
        {
            spinner.gameObject.SetActive(false);
            parent.gameObject.SetActive(true);
        }

        private void DisableButtons()
        {
            spinner.gameObject.SetActive(true);
            parent.gameObject.SetActive(false);
        }
    }
}