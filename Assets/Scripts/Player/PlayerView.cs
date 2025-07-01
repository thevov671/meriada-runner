using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void UpdateSpeedXParam(float value)
    {
        if (value < 0)
            value = -1;
        else if (value > 0)
            value = 1;

        _animator.SetFloat("XSpeed", value);
    }

    public void SetJumpTrigger()
    {
        _animator.SetTrigger("Jump");
    }

    public void SetHitTrigger()
    {
        _animator.SetTrigger("Hit");
    }

    public float GetAnimationClipLength(string clipName)
    {
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                Debug.Log(clip.length + ": jump time");
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation clip '{clipName}' not found.");
        return default;
    }

    public void SetJumpSpeedMultiplier(float value)
    {
        _animator.SetFloat("JumpSpeedMultiplier", value);
    }

    public void SetIdlingState(bool state)
    {
        _animator.SetBool("IsIdling", state);
    }
}
