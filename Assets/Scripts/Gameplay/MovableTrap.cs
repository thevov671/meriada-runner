using UnityEngine;
using DG.Tweening;

public class MovableTrap : Trap
{
    [Header("Move Settings")]
    [SerializeField] private float _moveDuration = 2f;
    [SerializeField] private bool _startFromLeft = true;

    [Header("Debug")]
    [SerializeField] private Color _gizmoColor = Color.red;

    private float _leftX;
    private float _rightX;
    private float _targetX;
    private bool _movingToLeft;

    private void Awake()
    {
        var left = GameObject.FindGameObjectWithTag("LeftSide");
        var right = GameObject.FindGameObjectWithTag("RightSide");

        _leftX = left.transform.position.x;
        _rightX = right.transform.position.x;
    }

    private void Start()
    {
        _movingToLeft = _startFromLeft;
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        _targetX = _movingToLeft ? _leftX : _rightX;
        float currentX = transform.position.x;
        float distance = Mathf.Abs(_targetX - currentX);

        float speed = Mathf.Abs(_rightX - _leftX) / _moveDuration;
        float duration = distance / speed;

        transform.DOMoveX(_targetX, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _movingToLeft = !_movingToLeft;
                MoveToTarget(); 
            });
    }
}
