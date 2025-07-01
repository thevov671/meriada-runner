using UnityEngine;
using System.Collections;

public class JumpHandler
{
    private readonly MonoBehaviour _actor;
    private readonly JumpSettings _settings;
    private Coroutine _jumpingCoroutine;

    public bool IsJumping => _jumpingCoroutine != null;

    public JumpHandler(MonoBehaviour actor, JumpSettings settings)
    {
        _actor = actor;
        _settings = settings;
    }

    public bool TryJump()
    {
        if (IsJumping)
            return false;

        if (_jumpingCoroutine != null)
            _actor.StopCoroutine(_jumpingCoroutine);

        _jumpingCoroutine = _actor.StartCoroutine(Jumping());
        return true;
    }

    private IEnumerator Jumping()
    {
        float timer = 0f;

        Transform transform = _actor.transform;
        Vector3 startPosition = transform.position;
        Vector3 jumpDirection = transform.forward.normalized;

        while (timer < _settings.JumpTime)
        {
            float t = timer / _settings.JumpTime;

            float yOffset = _settings.JumpCurveY.Evaluate(t) * _settings.JumpHeight;
            float forwardOffset = _settings.JumpCurveZ.Evaluate(t) * _settings.JumpLength;

            // Смещение вверх и вперёд по направлению взгляда
            Vector3 newPosition = startPosition + (jumpDirection * forwardOffset) + Vector3.up * yOffset;

            transform.position = newPosition;

            timer += Time.deltaTime;
            yield return null;
        }

        // Гарантируем финальную точку
        transform.position = startPosition + (jumpDirection * _settings.JumpLength);

        _jumpingCoroutine = null;
    }
}
