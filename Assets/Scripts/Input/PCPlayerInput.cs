using System;
using UnityEngine;

public class PCPlayerInput : IPlayerInput
{
    public event Action<float> OnHorizontalChanged;
    public event Action OnJump;
    public event Action OnLeftPressed;
    public event Action OnRightPressed;

    private bool _enabled;

    public void Enable() => _enabled = true;
    public void Disable() => _enabled = false;

    public void Update()
    {
        if (!_enabled) 
            return;

        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
            OnLeftPressed?.Invoke();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
            OnRightPressed?.Invoke();
        }

        OnHorizontalChanged?.Invoke(horizontal);

        if (Input.GetKeyDown(KeyCode.Space))
            OnJump?.Invoke();
    }

    public int GetForkDirection()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
            return -1;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
            return 1;

        return 0;
    }
}
