using System.Linq;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.PlayerLogic.Visual
{
    public sealed class PlayerDeathEffect : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] playerSkin;
        private IAssetProvider _factoryService;
        private CancellationTokenSource TokenSource;

        [SerializeField] private ParticleSystem lineVfx;
        [SerializeField] private ParticleSystem lavaVfx;

        [Inject]
        public void Constructor(IAssetProvider projectFactory) => _factoryService = projectFactory;

        private void Start()
        {
            TokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            TokenSource.Cancel();
            TokenSource.Dispose();
        }

        public async UniTaskVoid CreateDeathEffect(Vector3 position, Vector3 rotation, Vector3 scale,
            bool isLine = false)
        {
            CreateVisualEffect(position, rotation, scale, isLine);
            await CreateFragments();
            await UniTask.Delay(750, cancellationToken: TokenSource.Token);
            ChangePlayerSkinDeathVisibility(true);
        }

        private void CreateVisualEffect(Vector3 position, Vector3 rotation, Vector3 scale,
            bool isLine = false)
        {
            if (lineVfx == null || lavaVfx == null) return;

            Transform fx = isLine ? Instantiate(lineVfx).transform : Instantiate(lavaVfx).transform;

            fx.position = position;
            fx.rotation = Quaternion.Euler(rotation);
            fx.localScale = scale;
            fx.gameObject.SetActive(true);
        }

        private async UniTask CreateFragments()
        {
            ChangePlayerSkinDeathVisibility(false);

            FakePlayerDeathEffect mDeath =
                await _factoryService.InstantiateAsync<FakePlayerDeathEffect>(
                    AssetPaths.PLAYER_DEATH_MESH, TokenSource, dontDestroy: true);

            Transform playerTransform = transform;
            Vector3 fragmentsPosition = playerTransform.position;

            mDeath.transform.position = new Vector3(fragmentsPosition.x, fragmentsPosition.y + 1, fragmentsPosition.z);
            Vector3 fragmentsRotation = playerTransform.eulerAngles;
            fragmentsRotation.x = mDeath.transform.eulerAngles.x;
            fragmentsRotation.z = mDeath.transform.eulerAngles.z;
            mDeath.transform.eulerAngles = fragmentsRotation;
            mDeath.transform.parent = null;

            mDeath.gameObject.SetActive(true);

            mDeath.StartEffect(playerTransform.forward);
        }

        private void ChangePlayerSkinDeathVisibility(bool status)
        {
            foreach (MeshRenderer r in playerSkin.Where(r => r != null))
            {
                r.enabled = status;
            }
        }
    }
}