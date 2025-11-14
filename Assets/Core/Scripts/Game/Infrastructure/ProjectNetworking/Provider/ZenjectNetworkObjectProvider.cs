using Core.Scripts.Game.Infrastructure.RequiresInjection;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Zenject;

namespace Core.Scripts.Game.Infrastructure.ProjectNetworking.Provider
{
    [DisallowMultipleComponent, Preserve]
    public sealed class ZenjectNetworkObjectProvider : NetworkObjectProviderDefault
    {
        private DiContainer _container;
        private SceneContextRegistry _sceneRegistry;

        [Inject]
        public void Constructor(DiContainer container, SceneContextRegistry sceneRegistry)
        {
            _container = container;
            _sceneRegistry = sceneRegistry;
        }

        protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            SceneContext context = _sceneRegistry.TryGetSceneContextForScene(activeScene);

            if (context == null) Debug.Log("Instantiating ZenjectNetworkObjectProvider error, context not found!");
            
            prefab.TryGetComponent(out IRequiresInjection injection);

            GameObject go;
            
            if (context != null)
            {
                go = injection is not { RequiresInjection: true }
                    ? Instantiate(prefab.gameObject)
                    : context.Container.InstantiatePrefab(prefab.gameObject);
            }
            else
            {
                go = injection is not { RequiresInjection: true }
                    ? Instantiate(prefab.gameObject)
                    : _container.InstantiatePrefab(prefab.gameObject);
            }

            SceneManager.MoveGameObjectToScene(go, activeScene);
            return go.GetComponent<NetworkObject>();
        }
    }
}