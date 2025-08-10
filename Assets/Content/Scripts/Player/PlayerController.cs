using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerView _view;

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

    private CinemachineCamera _currentCamera;

    public event Action Win;
    public event Action TutorialFinished;
    public event Action<Item> ItemCollected;

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

        if (other.TryGetComponent(out Item item))
        {
            ItemCollected?.Invoke(item);
            item.gameObject.SetActive(false);
        }
    }

    private void OnForkAreaTriggerPathChosen(Transform nextRoadPivot)
    {
        float rotateTime = 0.5f;

        transform.DOLocalRotate(nextRoadPivot.eulerAngles, rotateTime).OnComplete(() =>
        {
            _isPaused = false;
            _view.SetIdleState(false);
            SwitchCameraTo(_inGameCamera);
        });
    }

    private void HandleMovement()
    {
        Vector3 sideMove = transform.right * _inputX * _sideSpeed * Time.deltaTime;
        Vector3 forwardMove = transform.forward * _forwardSpeed * Time.deltaTime;

        transform.position += forwardMove + sideMove;
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
