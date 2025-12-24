using System;
using Core.Scripts.Game.CharacterLogic.CharacterCombat;
using Core.Scripts.Game.CharacterLogic.Data;
using Core.Scripts.Game.CharacterLogic.Presenter;
using Core.Scripts.Game.CharacterLogic.Simulation;
using Fusion;
using CombatSimulation = Core.Scripts.Game.CharacterLogic.Simulation.CombatSimulation;

namespace Core.Scripts.Game.CharacterLogic
{
    public sealed class CharacterRuntime : IDisposable
    {
        private readonly CharacterEffectsPresenter _effects;
        private readonly CharacterAnimationPresenter _anim;
        private readonly SkinPresenter _skin;
        private readonly WeaponPresenter _weapons;
        private readonly CharacterVisualPresenter _visual;

        private readonly MoveSimulation _moveSim;
        private readonly LookSimulation _lookSim;
        private readonly CombatSimulation _combatSim;
        private readonly CombatStateMachine _combatState;

        public CharacterRuntime(
            CharacterEffectsPresenter effects,
            CharacterAnimationPresenter anim,
            SkinPresenter skin,
            CharacterVisualPresenter visual,
            WeaponPresenter weapons,
            MoveSimulation moveSim,
            LookSimulation lookSim,
            CombatSimulation combatSim,
            CombatStateMachine combatState)
        {
            _visual = visual;
            _effects = effects;
            _anim = anim;
            _skin = skin;
            _weapons = weapons;

            _moveSim = moveSim;
            _lookSim = lookSim;
            _combatSim = combatSim;
            _combatState = combatState;
        }

        public CharacterVisualNetwork CreateRandomVisual() => _visual.CreateRandomVisual();
        public NetworkString<_16> CreateDefaultNickname() => _visual.CreateDefaultNickname();
        public string FormatNickname(string value, NetworkId objectId) => _visual.FormatNickname(value, objectId);

        public void BeforeTick()
        {
            _effects.BeforeTick();
        }

        public void AfterTick()
        {
            _moveSim.AfterTick();
        }

        public void FixedTickSimulation()
        {
            _lookSim.FixedTick();
            _moveSim.FixedTick();
            _combatSim.FixedTick();
            _combatState.FixedUpdate();
        }

        public void FixedTickPresentation()
        {
            _effects.OnGroundEffect();
        }

        public void LateTickPresentation()
        {
            _effects.LateUpdate();
            _anim.LateUpdate();
        }

        public void ApplySkin(CharacterVisualNetwork visual) => _skin.Apply(visual);
        public void ApplyWeapon(int weaponId) => _weapons.Apply(weaponId);

        public void ApplyAttackSequence(int seq)
        {
            _anim.SetCombatStatus(seq != 0);
            _anim.SetAttackAnimation(seq);
        }

        public void Dispose()
        {
            // Nothing to dispose currently.
        }
    }
}
