using System;
using UnityEngine;

public class PcPlayerInput : IPlayerInput
{
    private const float MaxHorizontalValue = 1f;
    private const float MinHorizontalValue = -1f;
    private const float SwipeThreshold = 10f;

    private bool _enabled;
    private Vector2 _startPos;
    private bool _isSwiping;

    public event Action<float> HorizontalInputChanged;

    public void Enable() => _enabled = true;

    public void Disable()
    {
        _enabled = false;
        _isSwiping = false;
        HorizontalInputChanged?.Invoke(0f);
    }

    public void Update()
    {
        if (!_enabled)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;
            _isSwiping = false;
        }

        if (Input.GetMouseButton(0))
        {
            float delta = Mathf.Abs(Input.mousePosition.x - _startPos.x);

            if (!_isSwiping && delta > SwipeThreshold)
                _isSwiping = true;

            if (_isSwiping)
            {
                float mouseX = Input.mousePosition.x;
                float screenCenterX = Screen.width / 2f;
                float normalizedX = (mouseX - screenCenterX) / screenCenterX;
                float horizontal = Mathf.Clamp(normalizedX, MinHorizontalValue, MaxHorizontalValue);
                HorizontalInputChanged?.Invoke(horizontal);
            }
        }
        else
        {
            if (_isSwiping)
            {
                _isSwiping = false;
                HorizontalInputChanged?.Invoke(0f);
            }
        }
    }
}

