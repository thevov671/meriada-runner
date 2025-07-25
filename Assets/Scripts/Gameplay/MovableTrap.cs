using UnityEngine;
using DG.Tweening;

public class MovableTrap : Trap
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _moveDuration = 2f;

    private void Start()
    {
        Vector3 startPos = _startPoint.position;
        Vector3 endPos = _endPoint.position;

        transform.position = startPos;

        transform.DOMove(endPos, _moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
