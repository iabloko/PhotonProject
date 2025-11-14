using System.Collections.Generic;
using System.Text;
using System.Threading;
using Core.Scripts.Game.Infrastructure.ProjectNetworking.Service;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain;
using Core.Scripts.Game.Infrastructure.StateMachines.GameStateMachineMain.States;
using Cysharp.Threading.Tasks;
using Fusion;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.MatchmakingAdapter
{
    public sealed class MatchmakingUIAdapter : MonoBehaviour, IMatchmakingView
    {
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button createNewSession;

        [Title("Connected", titleAlignment: TitleAlignments.Right), SerializeField]
        private RectTransform connectedParent;

        [SerializeField] private Transform pagesRoot;
        [SerializeField] private GameObject spinner;

        [Title("Pagination", titleAlignment: TitleAlignments.Right), SerializeField, Min(1)]
        private int pageSize = 6;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text pageCounterText;
        [SerializeField] private GameObject noSessionsPlaceholder;

        private INetworkService _networkService;
        private IAssetProvider _assetProvider;
        private CancellationTokenSource _cts;

        private static readonly List<SessionInfo> s_emptyList = new(0);

        private readonly List<PageView> _pagePool = new();
        private readonly List<SessionInfo> _sessionsCache = new();

        private int _currentPageIndex;
        private GameStateMachine _stateMachine;

        private static readonly System.Random Random = new();
        private const string RANDOM_PATTERN = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public void Init(INetworkService networkService, IAssetProvider assetProvider, GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _networkService = networkService;
            _assetProvider = assetProvider;

            _cts = new CancellationTokenSource();

            _networkService.SessionsUpdated += OnSessionsUpdated;
            _networkService.StateChanged += ShowState;
            _networkService.ErrorRaised += ShowError;

            createNewSession.onClick.AddListener(CreateNewSessionClicked);
            prevButton.onClick.AddListener(OnPrevClicked);
            nextButton.onClick.AddListener(OnNextClicked);

            UpdatePaginationUI();
        }

        private void OnDisable()
        {
            if (_networkService != null)
            {
                _networkService.SessionsUpdated -= OnSessionsUpdated;
                _networkService.StateChanged -= ShowState;
                _networkService.ErrorRaised -= ShowError;
            }

            createNewSession.onClick.RemoveListener(CreateNewSessionClicked);
            prevButton.onClick.RemoveListener(OnPrevClicked);
            nextButton.onClick.RemoveListener(OnNextClicked);

            _cts.Cancel();
            _cts.Dispose();
        }

        private void OnSessionsUpdated(IReadOnlyList<SessionInfo> sessions) => ShowSessions(sessions);

        public void ShowState(NetUiState state)
        {
            if (statusText) statusText.text = state.ToString();

            bool inLobby = state is NetUiState.InLobby;
            connectedParent.gameObject.SetActive(inLobby);

            spinner.SetActive(state is NetUiState.ConnectingLobby or NetUiState.ConnectingSession);
        }

        public void ShowError(string message)
        {
            if (statusText) statusText.text = $"Error: {message}";
        }

        public void ShowSessions(IReadOnlyList<SessionInfo> sessions) => ShowSessionsAsync(sessions).Forget();

        private async UniTaskVoid ShowSessionsAsync(IReadOnlyList<SessionInfo> sessions)
        {
            _sessionsCache.Clear();
            if (sessions != null) _sessionsCache.AddRange(sessions);

            int requiredPages = Mathf.CeilToInt((_sessionsCache.Count) / (float)pageSize);
            if (requiredPages == 0) requiredPages = 1;

            await EnsurePagePoolAsync(requiredPages);

            if (_currentPageIndex >= requiredPages) _currentPageIndex = Mathf.Max(0, requiredPages - 1);

            for (int page = 0; page < requiredPages; page++)
            {
                var slice = GetSliceForPage(_sessionsCache, page, pageSize);

                await _pagePool[page].EnsureCapacityAsync(slice.Count, _assetProvider, _cts);

                _pagePool[page].BindSlice(slice, JoinSelectedSession);
                _pagePool[page].gameObject.SetActive(page == _currentPageIndex);
            }

            for (int i = requiredPages; i < _pagePool.Count; i++)
                _pagePool[i].gameObject.SetActive(false);

            UpdatePaginationUI();
        }

        private async UniTask EnsurePagePoolAsync(int requiredPages)
        {
            int pageIndex = 0;
            while (_pagePool.Count < requiredPages)
            {
                pageIndex++;
                PageView page = await _assetProvider.InstantiateAsync<PageView>(
                    AssetPaths.MATCHMAKING_PAGE, _cts, pagesRoot);

                page.gameObject.SetActive(false);
                page.transform.name = string.Concat("Page_", pageIndex);
                page.Stretch();
                page.transform.localScale = Vector3.one;
                _pagePool.Add(page);

                await UniTask.Yield(cancellationToken: _cts.Token);
            }
        }

        private static List<SessionInfo> GetSliceForPage(List<SessionInfo> all, int pageIndex, int pageSize)
        {
            int start = pageIndex * pageSize;
            int len = Mathf.Clamp(all.Count - start, 0, pageSize);

            if (len <= 0) return s_emptyList;
            var slice = new List<SessionInfo>(len);
            for (int i = 0; i < len; i++)
                slice.Add(all[start + i]);

            return slice;
        }

        private void UpdatePaginationUI()
        {
            int totalItems = _sessionsCache.Count;
            int totalPages = Mathf.Max(1, Mathf.CeilToInt(totalItems / (float)pageSize));

            bool hasAny = totalItems > 0;

            if (noSessionsPlaceholder)
                noSessionsPlaceholder.SetActive(!hasAny);

            bool showNav = hasAny && totalPages > 1;
            prevButton.gameObject.SetActive(showNav);
            nextButton.gameObject.SetActive(showNav);

            prevButton.interactable = showNav && _currentPageIndex > 0;
            nextButton.interactable = showNav && _currentPageIndex < totalPages - 1;

            if (pageCounterText)
                pageCounterText.text = hasAny ? $"{_currentPageIndex + 1} / {totalPages}" : "0 / 0";
        }

        private void OnPrevClicked()
        {
            int totalPages = Mathf.Max(1, Mathf.CeilToInt(_sessionsCache.Count / (float)pageSize));
            if (_currentPageIndex <= 0) return;

            _currentPageIndex--;
            ActivateOnlyCurrentPage(totalPages);
        }

        private void OnNextClicked()
        {
            int totalPages = Mathf.Max(1, Mathf.CeilToInt(_sessionsCache.Count / (float)pageSize));
            if (_currentPageIndex >= totalPages - 1) return;

            _currentPageIndex++;
            ActivateOnlyCurrentPage(totalPages);
        }

        private void ActivateOnlyCurrentPage(int totalPages)
        {
            for (int i = 0; i < _pagePool.Count; i++)
                _pagePool[i].gameObject.SetActive(i < totalPages && i == _currentPageIndex);

            UpdatePaginationUI();
        }

        private void JoinSelectedSession(SessionInfo s)
        {
            if (!s.IsValid) return;
            ShowState(NetUiState.ConnectingSession);
            ConnectToSession(s.Name).Forget();
        }

        private void CreateNewSessionClicked()
        {
            ShowState(NetUiState.ConnectingSession);
            string serverName = RandomServerName(16);
            ConnectToSession(serverName).Forget();
        }

        private async UniTaskVoid ConnectToSession(string serverName)
        {
            bool ok = await _networkService.ConnectToSession(serverName);
            if (ok) _stateMachine.Enter<GamePlayState>();
            ShowState(ok ? NetUiState.InSession : NetUiState.Error);
        }

        private string RandomServerName(int length)
        {
            StringBuilder result = new(length);
            for (int i = 0; i < length; i++)
                result.Append(RANDOM_PATTERN[Random.Next(RANDOM_PATTERN.Length)]);

            return $"SERVER-{result.ToString().ToUpper()}";
        }
    }
}