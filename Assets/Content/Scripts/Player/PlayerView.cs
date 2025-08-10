using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private readonly int HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int WinHash = Animator.StringToHash("Win");

    [SerializeField] private Animator _animator;

    public void SetHorizontalSpeed(float value)
    {
        _animator.SetFloat(HorizontalSpeedHash, value);
    }

    public void SetIdleState(bool state)
    {
        _animator.SetBool(IdleHash, state);
    }

    public void SetWinTrigger()
    {
        _animator.SetTrigger(WinHash);
    }
}
