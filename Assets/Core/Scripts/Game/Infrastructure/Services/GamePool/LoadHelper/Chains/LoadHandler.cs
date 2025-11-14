using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.LoadHelper.Chains
{
    public abstract class LoadHandler
    {
        private LoadHandler _nextSoursLoad;

        public async UniTask<Mesh> Execute(string id) =>
            await CanHandle(id) ? await ExecuteLogic(id) : await _nextSoursLoad.Execute(id);

        public void SetNextHandler(LoadHandler soursLoad) => _nextSoursLoad = soursLoad;

        protected abstract UniTask<bool> CanHandle(string prefabId);
        protected abstract UniTask<Mesh> ExecuteLogic(string id);
    }
}