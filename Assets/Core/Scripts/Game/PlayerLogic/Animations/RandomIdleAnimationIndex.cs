using System.Collections.Generic;
using Sandbox.Project.Scripts.Helpers.ShuffleHelper;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Animations
{
    public sealed class RandomIdleAnimationIndex : StateMachineBehaviour
    {
        [SerializeField] private int currentIdleIndex;
        private readonly Queue<int> _idleIndexes = new();

        private static readonly int RandomIdleIndex = Animator.StringToHash("RandomIdleIndex");
        private const int NUMBER_OF_ANIMATIONS = 7;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (_idleIndexes.Count <= 0) PrepareQueue();
            
            currentIdleIndex = _idleIndexes.Dequeue();
            animator.SetInteger(RandomIdleIndex, currentIdleIndex);
        }

        private void PrepareQueue()
        {
            for (int i = 0; i < NUMBER_OF_ANIMATIONS; i++)
            {
                _idleIndexes.Enqueue(i);
            }

            _idleIndexes.FisherYatesShuffle();
        }
    }
}