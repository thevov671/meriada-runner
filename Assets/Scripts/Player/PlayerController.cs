using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const int BonusPerItem = 100;

    [SerializeField] private PlayerView _view;
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private BackpackView _backpackView;
    [SerializeField] private ScoreDisplay _scoreDisplay;

    [Header("Boundaries")]
    [SerializeField] private float _leftBoundaryX = -3f;
    [SerializeField] private float _rightBoundaryX = 3f;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _pausedCamera;
    [SerializeField] private CinemachineCamera _inGameCamera;
    [SerializeField] private CinemachineCamera _endGameCamera;
    [SerializeField] private CinemachineCamera _fallCamera;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _hitVFX;
    [SerializeField] private ParticleSystem _collectPositiveBonusVFX;
    [SerializeField] private ParticleSystem _collectNegativeBonusVFX;
    [SerializeField] private ParticleSystem _winVFX;
    [SerializeField] private ParticleSystem _loseVFX;

    private IPlayerInput _input;
    private float _speed = 0;
    private float _health = 1;
    private int _bonusPoints = 0;

    private CinemachineCamera _currentCamera;

    private bool _isPaused = true;
    private float _inputX;

    public event Action Win;
    public event Action Lose;
    public event Action TutorialFinished;

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
        // TryClampMotion();
    }

    private void TryClampMotion()
    {
        Vector3 currentPosition = transform.position;

        if (transform.position.x <= _leftBoundaryX)
            currentPosition.x = _leftBoundaryX;

        if (transform.position.x >= _rightBoundaryX)
            currentPosition.x = _rightBoundaryX;

        transform.position = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HealthItem health))
        {
            _health++;
            _backpackView.AddCube(health.transform);

            health.transform.parent = null;
            health.Stop();
            Destroy(health);
        }
    }

    private void FixedUpdate()
    {
        if (_isPaused)
            return;

        MoveForward();
        CheckFall();
    }

    public void TakeHit()
    {
        _health--;
        _hitVFX.Play();

        _backpackView.RemoveItem();

        if (_health <= 0)
            HandleLose(_endGameCamera);
        else
            _view.SetHitTrigger();
    }

    public void AddBonus(int value)
    {
        _bonusPoints += value;

        if (_bonusPoints < 0)
            _bonusPoints = 0;

        if (value > 0)
        {
            _collectPositiveBonusVFX.Play();
        }
        else
        {
            _collectNegativeBonusVFX.Play();
        }

        _scoreDisplay.UpdateText(_bonusPoints);
    }

    public void HandleWin()
    {
        _winVFX.Play();
        _view.SetWinTrigger();
        _isPaused = true;
        Win?.Invoke();
        SwitchCameraTo(_endGameCamera);

        _backpackView.AnimateScoreConversionToCameraCorner(Camera.main, onPointAdded: AddBonus);
    }

    private void HandleLose(CinemachineCamera withCamera)
    {
        _loseVFX.Play();
        _view.SetLoseTrigger();
        _isPaused = true;
        Lose?.Invoke();
        SwitchCameraTo(withCamera);
    }

    private void OnHorizontalInputChanged(float value)
    {
        _inputX = value;
        _view.SetHorizontalSpeed(value);
    }

    private void MoveForward()
    {
        Vector3 movement = new Vector3(_inputX * _speed, 0, _speed) * Time.fixedDeltaTime;

        if (transform.position.x + movement.x >= _rightBoundaryX || transform.position.x + movement.x <= _leftBoundaryX)
            movement.x = 0;

        _rigidbody.MovePosition(_rigidbody.position + movement);
    }

    private void CheckFall()
    {
        if (_rigidbody.linearVelocity.y < -0.5f)
        {
            _fallCamera.transform.parent = null;
            HandleLose(_fallCamera);
        }
    }
}
