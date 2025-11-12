using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Core.Scripts.Game.GameHelpers
{
    public static class DoTweenExtensions
    {
        public static UniTask ToUniTask(this Tween tween)
        {
            UniTaskCompletionSource tcs = new();

            tween.OnComplete(() => tcs.TrySetResult());
            tween.OnKill(() => tcs.TrySetCanceled());

            return tcs.Task;
        }
    }
}