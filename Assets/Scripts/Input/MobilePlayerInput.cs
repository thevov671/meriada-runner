using System;
using UnityEngine;

public class MobilePlayerInput: IPlayerInput
{
    public event Action<float> OnHorizontalChanged;
    public event Action OnJump;
    public event Action OnLeftPressed;
    public event Action OnRightPressed;

    private Vector2 _touchStart;
    private bool _touchActive;
    private float _minSwipeDistance = 50f;
    private bool _enabled;

    public void Enable() => _enabled = true;
    public void Disable() => _enabled = false;

    public void Update()
    {
        if (!_enabled) return;

        if (Input.touchCount == 0)
        {
            OnHorizontalChanged?.Invoke(0f);
            _touchActive = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _touchStart = touch.position;
            _touchActive = true;
        }
        else if (touch.phase == TouchPhase.Moved && _touchActive)
        {
            Vector2 delta = touch.position - _touchStart;

            float horizontal = Mathf.Clamp(delta.x / Screen.width, -1f, 1f);
            OnHorizontalChanged?.Invoke(horizontal);

            if (horizontal < -0.5f)
                OnLeftPressed?.Invoke();
            else if (horizontal > 0.5f)
                OnRightPressed?.Invoke();

            if (delta.y > _minSwipeDistance)
            {
                _touchActive = false;
                OnJump?.Invoke();
            }
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _touchActive = false;
            OnHorizontalChanged?.Invoke(0f);
        }
    }

    public int GetForkDirection()
    {
        throw new NotImplementedException();
    }
}
