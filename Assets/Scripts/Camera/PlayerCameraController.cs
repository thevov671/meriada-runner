using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineThirdPersonFollow _thirdPersonFollow;
    [SerializeField] private PlayerCameraSettings _settings;
    [SerializeField] private CameraOccluderFader _occluderFader;

    private PlayerController _player;
    private Coroutine _jumpCameraRoutine;
    private float _defaultDistance;

    public void Init(PlayerController player)
    {
        _player = player;
        _camera.Target.TrackingTarget = player.transform;

        _defaultDistance = _thirdPersonFollow.CameraDistance;

        _player.Jumped += HandleJump;

        _occluderFader.SetTarget(_player.transform);
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.Jumped -= HandleJump;
    }

    private void HandleJump()
    {
        if (_jumpCameraRoutine != null)
            StopCoroutine(_jumpCameraRoutine);

        _jumpCameraRoutine = StartCoroutine(JumpCameraRoutine());
    }

    private IEnumerator JumpCameraRoutine()
    {
        float targetDistance = _defaultDistance + _settings.JumpDistanceIncrease;
        float zoomOutDuration = _settings.JumpZoomOutDuration;
        float returnDuration = _settings.JumpReturnDuration;
        float holdDuration = _settings.JumpHoldDuration;

        float defaultArm = _thirdPersonFollow.VerticalArmLength;
        float targetArm = defaultArm + _settings.JumpVerticalOffset;

        // Параллельный tween: отдаление и подъем камеры
        Sequence jumpSequence = DOTween.Sequence();

        jumpSequence.Join(DOTween.To(
            () => _thirdPersonFollow.CameraDistance,
            v => _thirdPersonFollow.CameraDistance = v,
            targetDistance,
            zoomOutDuration
        ).SetEase(Ease.OutQuad));

        jumpSequence.Join(DOTween.To(
            () => _thirdPersonFollow.VerticalArmLength,
            v => _thirdPersonFollow.VerticalArmLength = v,
            targetArm,
            zoomOutDuration
        ).SetEase(Ease.OutQuad));

        yield return jumpSequence.WaitForCompletion();

        // Пауза в максимальной точке
        yield return new WaitForSeconds(holdDuration);

        // Плавный возврат к исходному положению
        Sequence returnSequence = DOTween.Sequence();

        returnSequence.Join(DOTween.To(
            () => _thirdPersonFollow.CameraDistance,
            v => _thirdPersonFollow.CameraDistance = v,
            _defaultDistance,
            returnDuration
        ).SetEase(Ease.OutQuad));

        returnSequence.Join(DOTween.To(
            () => _thirdPersonFollow.VerticalArmLength,
            v => _thirdPersonFollow.VerticalArmLength = v,
            defaultArm,
            returnDuration
        ).SetEase(Ease.OutQuad));

        yield return returnSequence.WaitForCompletion();

        _jumpCameraRoutine = null;
    }
}
