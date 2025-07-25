using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public const int BonusPerItem = 100;
    public int Score => _score;

    [SerializeField] private PlayerView _view;
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private BackpackView _backpackView;
    [SerializeField] private ScoreDisplay _scoreDisplay;

    [Header("Boundaries")]
    [SerializeField] private float _leftBoundaryX = -3f;
    [SerializeField] private float _rightBoundaryX = 3f;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _pausedCamera;
    [SerializeField] private CinemachineCamera _inGameCamera;
    [SerializeField] private CinemachineCamera _endGameCamera;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _hitVFX;
    [SerializeField] private ParticleSystem _collectPositiveBonusVFX;
    [SerializeField] private ParticleSystem _collectNegativeBonusVFX;
    [SerializeField] private ParticleSystem _winVFX;
    [SerializeField] private ParticleSystem _loseVFX;

    [Header("SFX")]
    [SerializeField] private AudioClip _hitSFX;
    [SerializeField] private AudioClip _collectItemSFX;
    [SerializeField] private AudioClip _winSFX;
    [SerializeField] private AudioClip _loseSFX;

    private bool _isPaused = true;

    private IPlayerInput _input;
    private float _inputX;
    private float _currentInputX;

    private float _speed = 0;
    private float _health = 1;
    private int _score = 0;

    private CharacterController _controller;
    private CinemachineCamera _currentCamera;

    public event Action Win;
    public event Action Lose;
    public event Action TutorialFinished;

    private void HandleMovement()
    {
        _currentInputX = Mathf.Lerp(_currentInputX, _inputX, Time.deltaTime * 10f);
        Vector3 movement = new Vector3(_currentInputX * _speed, 0, _speed) * Time.deltaTime;

        Vector3 nextPosition = transform.position + new Vector3(movement.x, 0, 0);
        nextPosition.x = Mathf.Clamp(nextPosition.x, _leftBoundaryX, _rightBoundaryX);
        movement.x = nextPosition.x - transform.position.x;

        _controller.Move(movement);
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        AudioManager.Instance.Unpause();
    }

    public void Init(IPlayerInput input)
    {
        _input = input;
        _input.Enable();
        _input.HorizontalInputChanged += OnHorizontalInputChanged;

        _speed = _config.MovementSpeed;
        _health = 1;

        SwitchCameraTo(_pausedCamera);

        StartCoroutine(TutorialCoroutine());
    }

    private IEnumerator TutorialCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => Input.anyKeyDown);

        _isPaused = false;
        SwitchCameraTo(_inGameCamera);
        _view.SetIdleState(false);
        TutorialFinished?.Invoke();
    }

    private void SwitchCameraTo(CinemachineCamera newCamera)
    {
        if (_currentCamera != null)
            _currentCamera.gameObject.SetActive(false);

        _currentCamera = newCamera;
        _currentCamera.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_input != null)
            _input.HorizontalInputChanged -= OnHorizontalInputChanged;
    }

    private void Update()
    {
        _input.Update();

        if (_isPaused)
            return;

        HandleMovement();
    }

    public void AddItem(HealthItem item)
    {
        if (_isPaused)
            return;

        _health++;
        _backpackView.AddCube(item.transform);
        AudioManager.Instance.PlayClip(_collectItemSFX);

        item.transform.parent = null;
        item.Stop();
        Destroy(item);
    }

    public void TakeHit()
    {
        _health--;
        _hitVFX.Play();

        _backpackView.RemoveItem();

        if (_health <= 0)
        {
            HandleLose();
        }
        else
        {
            _view.SetHitTrigger();
            AudioManager.Instance.PlayClip(_hitSFX);
        }
    }

    public void AddBonus(int value)
    {
        _score += value;

        if (_score < 0)
            _score = 0;

        if (value > 0)
            _collectPositiveBonusVFX.Play();
        else
            _collectNegativeBonusVFX.Play();

        _scoreDisplay.UpdateText(_score);
    }

    public void HandleWin()
    {
        AudioManager.Instance.Pause();
        AudioManager.Instance.PlayClip(_winSFX);

        _winVFX.Play();
        _view.SetWinTrigger();
        _isPaused = true;
        SwitchCameraTo(_endGameCamera);

        _backpackView.AnimateScoreConversionToCameraCorner(Camera.main, onPointAdded: AddBonus, onComplete: () => Win?.Invoke());
    }

    private void HandleLose()
    {
        AudioManager.Instance.Pause();
        AudioManager.Instance.PlayClip(_loseSFX);

        _loseVFX.Play();
        _view.SetLoseTrigger();
        _isPaused = true;
        Lose?.Invoke();
        SwitchCameraTo(_endGameCamera);
    }

    private void OnHorizontalInputChanged(float value)
    {
        _inputX = value;
        _view.SetHorizontalSpeed(value);

        if (Mathf.Abs(value) > 0.01f)
        {
            _backpackView.Lean(value);
            _backpackView.RotateWithMovement(value);
        }
        else
        {
            _backpackView.ResetLean();
        }

    }
}
