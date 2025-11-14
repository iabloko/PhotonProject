using System;
using System.Collections.Generic;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.MatchmakingAdapter
{
    public sealed class PageView : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private RectTransform contentRoot;

        private readonly List<SessionListItem> _items = new();

        public void Stretch()
        {
            root.anchorMin = new Vector2(0, 0);
            root.anchorMax = new Vector2(1, 1);
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
        }

        public async UniTask EnsureCapacityAsync(
            int targetCount,
            IAssetProvider provider,
            CancellationTokenSource token)
        {
            while (_items.Count < targetCount)
            {
                SessionListItem item = await provider.InstantiateAsync<SessionListItem>(
                    AssetPaths.MATCHMAKING_SESSION, token, contentRoot);

                item.transform.localScale = Vector3.one;
                _items.Add(item);
                
                await UniTask.Yield(cancellationToken: token.Token);
            }
        }

        public void BindSlice(IReadOnlyList<SessionInfo> slice, Action<SessionInfo> onJoin)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                bool active = i < slice.Count;
                _items[i].gameObject.SetActive(active);
                if (active)
                    _items[i].Bind(slice[i], onJoin);
            }
        }
    }
}