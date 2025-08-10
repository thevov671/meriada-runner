using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerView _view;
    [SerializeField] private CharacterController _controller;

    [Header("Speed Settings")]
    [SerializeField] private float _forwardSpeed = 10;
    [SerializeField] private float _sideSpeed = 8;

    [Header("Boundaries")]
    [SerializeField] private float _leftBoundaryX = -3f;
    [SerializeField] private float _rightBoundaryX = 3f;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _pausedCamera;
    [SerializeField] private CinemachineCamera _inGameCamera;
    [SerializeField] private CinemachineCamera _endGameCamera;
    [SerializeField] private CinemachineCamera _forkCamera;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _collectPositiveBonusVFX;
    [SerializeField] private ParticleSystem _collectNegativeBonusVFX;
    [SerializeField] private ParticleSystem _winVFX;

    [Header("SFX")]
    [SerializeField] private AudioClip _collectPositiveBonusSFX;
    [SerializeField] private AudioClip _collectNegativeBonusSFX;
    [SerializeField] private AudioClip _winSFX;

    private bool _isPaused = true;

    private IPlayerInput _input;
    private float _inputX;
    private float _currentInputX;

    private CinemachineCamera _currentCamera;

    public event Action Win;
    public event Action TutorialFinished;

    public void Init(IPlayerInput input)
    {
        _input = input;

        _input.Enable();
        _input.HorizontalInputChanged += OnHorizontalInputChanged;

        SwitchCameraTo(_pausedCamera);
        StartCoroutine(TutorialCoroutine());

        ForkAreaTrigger.PathChosen += OnForkAreaTriggerPathChosen;
    }

    private void OnDestroy()
    {
        _input.HorizontalInputChanged -= OnHorizontalInputChanged;
        ForkAreaTrigger.PathChosen -= OnForkAreaTriggerPathChosen;
    }

    private void Update()
    {
        _input.Update();

        if (_isPaused)
            return;

        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ForkAreaTrigger forkAreaTrigger))
        {
            _isPaused = true;
            _view.SetIdleState(true);
            forkAreaTrigger.Activate(_input);
            SwitchCameraTo(_forkCamera);
        }
    }

    private void OnForkAreaTriggerPathChosen(Transform nextRoadPivot)
    {
        Vector3 direction = nextRoadPivot.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotateTime = 0.5f;

        transform.DORotateQuaternion(targetRotation, rotateTime).OnComplete(() =>
        {
            _isPaused = false;
            _view.SetIdleState(false);
            SwitchCameraTo(_inGameCamera);
        });
    }

    private void HandleMovement()
    {
        float lerpSpeed = 10f;
        _currentInputX = Mathf.Lerp(_currentInputX, _inputX, Time.deltaTime * lerpSpeed);

        Vector3 localMovement = new Vector3(_currentInputX * _sideSpeed, 0f, _forwardSpeed) * Time.deltaTime;
        Vector3 worldMovement = transform.TransformDirection(localMovement);

        Vector3 nextPosition = transform.position + new Vector3(worldMovement.x, 0f, 0f);
        nextPosition.x = Mathf.Clamp(nextPosition.x, _leftBoundaryX, _rightBoundaryX);
        worldMovement.x = nextPosition.x - transform.position.x;

        _controller.Move(worldMovement);
    }


    private IEnumerator TutorialCoroutine()
    {
        float startDelay = 0.3f;

        yield return new WaitForSeconds(startDelay);

        yield return new WaitUntil(() => Input.anyKeyDown);

        _isPaused = false;
        _view.SetIdleState(false);

        SwitchCameraTo(_inGameCamera);

        TutorialFinished?.Invoke();
    }

    private void SwitchCameraTo(CinemachineCamera newCamera)
    {
        if (_currentCamera != null)
            _currentCamera.gameObject.SetActive(false);

        _currentCamera = newCamera;
        _currentCamera.gameObject.SetActive(true);
    }

    private void OnHorizontalInputChanged(float value)
    {
        _inputX = value;
        _view.SetHorizontalSpeed(value);
    }
}
