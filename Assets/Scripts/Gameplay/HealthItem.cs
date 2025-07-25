using UnityEngine;
using DG.Tweening;

public class HealthItem : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private Tween _rotateTween;
    private Tween _bobTween;

    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        // Вращение по Y
        _rotateTween = transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        // Покачивание вверх-вниз
        _bobTween = transform.DOMoveY(_initialPosition.y + 0.25f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void Stop()
    {
        // Остановить все твины
        _rotateTween?.Kill();
        _bobTween?.Kill();

        // Вернуть начальные положение и поворот
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.AddItem(this);
        }
    }
}
