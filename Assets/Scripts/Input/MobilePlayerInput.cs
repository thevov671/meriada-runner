using System;
using UnityEngine;

public class MobilePlayerInput : IPlayerInput
{
    private const float MaxHorizontalValue = 1f;
    private const float MinHorizontalValue = -1f;
    private const float SwipeThreshold = 20f;

    private bool _enabled;
    private Vector2 _startPos;
    private int _activeTouchId = -1;
    private float _screenCenterX;
    private bool _isSwiping = false;

    public event Action<float> HorizontalInputChanged;

    public void Enable()
    {
        _enabled = true;
        _screenCenterX = Screen.width / 2f;
    }

    public void Disable()
    {
        _enabled = false;
        _activeTouchId = -1;
        _isSwiping = false;
        HorizontalInputChanged?.Invoke(0f);
    }

    public void Update()
    {
        if (!_enabled || Input.touchCount == 0)
            return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _activeTouchId = touch.fingerId;
                _startPos = touch.position;
                _isSwiping = false;
            }

            if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && touch.fingerId == _activeTouchId)
            {
                if (!_isSwiping)
                {
                    float delta = Mathf.Abs(touch.position.x - _startPos.x);
                    if (delta > SwipeThreshold)
                        _isSwiping = true;
                    else
                        return;
                }

                float normalizedX = (touch.position.x - _screenCenterX) / _screenCenterX;
                float horizontal = Mathf.Clamp(normalizedX, MinHorizontalValue, MaxHorizontalValue);
                HorizontalInputChanged?.Invoke(horizontal);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == _activeTouchId)
                {
                    _activeTouchId = -1;
                    _isSwiping = false;
                    HorizontalInputChanged?.Invoke(0f);
                }
            }
        }
    }
}
