using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private static readonly int HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WinHash = Animator.StringToHash("Win");
    private static readonly int LoseHash = Animator.StringToHash("Lose");

    [SerializeField] private Animator _animator;

    public void SetHorizontalSpeed(float value)
    {
        _animator.SetFloat(HorizontalSpeedHash, value);
    }

    public void SetHitTrigger()
    {
        _animator.SetTrigger(HitHash);
    }

    public void SetIdleState(bool state)
    {
        _animator.SetBool(IdleHash, state);
    }

    public void SetWinTrigger()
    {
        _animator.SetTrigger(WinHash);
    }

    public void SetLoseTrigger()
    {
        _animator.SetTrigger(LoseHash);
    }
}
