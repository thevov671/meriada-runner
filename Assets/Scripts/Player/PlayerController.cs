using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    public event Action Jumped;
    public event Action<ForkData> ForkEntered;
    public event Action ForkExited;

    [SerializeField] private JumpSettings _jumpSettings;
    [SerializeField] private PlayerView _view;
    [SerializeField] private MovementSettings _movementSettings;

    private MovementHandler _movementHandler;
    private JumpHandler _jumpHandler;

    private bool _onFork = false;
    private float _currentHorizontal = 0f;

    public IPlayerInput Input { get; private set; } = null;

    public void Init(IPlayerInput input)
    {
        Input = input;
        Input.Enable();

        Input.OnHorizontalChanged += OnHorizontalChanged;
        Input.OnJump += OnJumpRequested;

        _jumpHandler = new JumpHandler(this, _jumpSettings);
        _movementHandler = new MovementHandler(transform, _movementSettings);

        _view.SetJumpSpeedMultiplier(_view.GetAnimationClipLength("Jump") / _jumpSettings.JumpTime);
    }

    private void OnDestroy()
    {
        Input.OnHorizontalChanged -= OnHorizontalChanged;
        Input.OnJump -= OnJumpRequested;
    }

    private void Update()
    {
        Input.Update();

        if (_jumpHandler.IsJumping == false && _onFork == false)
        {
            _movementHandler.Update(_currentHorizontal, Time.deltaTime);
            _view.UpdateSpeedXParam(_currentHorizontal);
        }
    }

    private void OnHorizontalChanged(float value)
    {
        _currentHorizontal = value;
    }

    private void OnJumpRequested()
    {
        if (_jumpHandler.TryJump())
        {
            _view.SetJumpTrigger();
            Jumped.Invoke();
        }
    }

    public void TakeDamage()
    {
        _view.SetHitTrigger();
    }

    public void EnterFork(ForkData forkData)
    {
        StartCoroutine(ForkRoutine(forkData));
    }

    private IEnumerator ForkRoutine(ForkData forkData)
    {
        _onFork = true;
        _currentHorizontal = 0;
        _view.SetIdlingState(true);

        ForkEntered?.Invoke(forkData);

        yield return new WaitUntil(() => _currentHorizontal != 0);

        float angleY = _currentHorizontal < 0 ? -45f : 45f;
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y + angleY, 0f);
        float rotateTime = 0.5f;

        yield return transform
            .DORotateQuaternion(targetRotation, rotateTime)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        _onFork = false;
        _view.SetIdlingState(false);
        ForkExited?.Invoke();
    }
}
