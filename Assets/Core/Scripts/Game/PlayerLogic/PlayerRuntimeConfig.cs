using System;
using Core.Scripts.Game.CharacterLogic.Data;
using Core.Scripts.Game.GamePlay.UsableItems;
using Core.Scripts.Game.Infrastructure.ModelData;
using Core.Scripts.Game.PlayerLogic.InputLogic;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class PlayerRuntimeConfig
    {
        public SimpleKCC Kcc { get; }
        public PlayerInput Input { get; }
        public Animator Animator { get; }
        public Transform PreviewRotation { get; }
        public ParticleSystem FootprintParticles { get; }
        public ParticleSystem OnGroundParticles { get; }

        public CharacterVisual VisualData { get; }
        public WeaponData[] WeaponData { get; }
        public GameplaySettings GameplayData { get; }

        public NetworkRunner Runner { get; }
        public bool HasStateAuthority { get; }

        private readonly Func<int> _getAttackSequence;
        private readonly Action<int> _setAttackSequence;
        private readonly Func<int> _getLastAttackTick;
        private readonly Action<int> _setLastAttackTick;

        public PlayerRuntimeConfig(
            SimpleKCC kcc,
            PlayerInput input,
            Animator animator,
            Transform previewRotation,
            ParticleSystem footprintParticles,
            ParticleSystem onGroundParticles,
            CharacterVisual visualData,
            WeaponData[] weaponData,
            GameplaySettings gameplayData,
            NetworkRunner runner,
            bool hasStateAuthority,
            Func<int> getAttackSequence,
            Action<int> setAttackSequence,
            Func<int> getLastAttackTick,
            Action<int> setLastAttackTick)
        {
            Kcc = kcc;
            Input = input;
            Animator = animator;
            PreviewRotation = previewRotation;
            FootprintParticles = footprintParticles;
            OnGroundParticles = onGroundParticles;
            VisualData = visualData;
            WeaponData = weaponData;
            GameplayData = gameplayData;
            Runner = runner;
            HasStateAuthority = hasStateAuthority;
            _getAttackSequence = getAttackSequence;
            _setAttackSequence = setAttackSequence;
            _getLastAttackTick = getLastAttackTick;
            _setLastAttackTick = setLastAttackTick;
        }

        public int GetAttackSequence() => _getAttackSequence();
        public void SetAttackSequence(int value) => _setAttackSequence(value);
        public int GetLastAttackTick() => _getLastAttackTick();
        public void SetLastAttackTick(int value) => _setLastAttackTick(value);
    }
}