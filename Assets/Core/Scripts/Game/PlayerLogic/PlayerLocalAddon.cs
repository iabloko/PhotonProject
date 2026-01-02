using Core.Scripts.Game.CharacterLogic;
using Core.Scripts.Game.CharacterLogic.Presenter;
using Core.Scripts.Game.Infrastructure.Services.CinemachineService;
using Core.Scripts.Game.Infrastructure.Services.NickName;
using Core.Scripts.Game.Infrastructure.Services.ProjectSettingsService;
using Core.Scripts.Game.PlayerLogic.InputLogic;
using UnityEngine;
using Zenject;

namespace Core.Scripts.Game.PlayerLogic
{
    public sealed class PlayerLocalAddon : MonoBehaviour
    {
        private ICinemachine _cinemachine;
        private IProjectSettings _projectSettings;
        private INickNameFadeEffect _nickNameFadeEffect;

        private CameraPresenter _cameraPresenter;

        [Inject]
        private void Construct(
            ICinemachine cinemachine,
            IProjectSettings projectSettings,
            INickNameFadeEffect nickNameFadeEffect)
        {
            _cinemachine = cinemachine;
            _projectSettings = projectSettings;
            _nickNameFadeEffect = nickNameFadeEffect;
        }

        public void Bind(ICharacterMotor motor, PlayerInput input, Transform previewRotation)
        {
            PlayerInputAdapter inputAdapter = new(input);

            _nickNameFadeEffect.Initialization(Camera.main);

            _cameraPresenter = new CameraPresenter(
                motor,
                inputAdapter,
                _cinemachine,
                _projectSettings,
                previewRotation,
                rotationSpeed: 2f);

            _cameraPresenter.AfterSpawned();
        }

        private void LateUpdate()
        {
            _cameraPresenter.LateUpdate();
            _nickNameFadeEffect.FixedUpdateNetwork();
        }
    }
}
