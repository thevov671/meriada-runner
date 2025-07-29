using UnityEngine;
using DG.Tweening;

public class MovableTrap : Trap
{
    [SerializeField] private float _moveLength = 3f;
    [SerializeField] private float _moveDuration = 2f;
    [SerializeField] private Color _gizmoColor = Color.red;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private void Start()
    {
        _startPos = transform.position;
        _endPos = _startPos + Vector3.right * _moveLength;

        transform.DOMove(_endPos, _moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 previewStart = Application.isPlaying ? _startPos : transform.position;
        Vector3 previewEnd = previewStart + Vector3.right * _moveLength;

        Gizmos.color = _gizmoColor;
        Gizmos.DrawLine(previewStart, previewEnd);
        Gizmos.DrawSphere(previewEnd, 0.2f);
    }
}
