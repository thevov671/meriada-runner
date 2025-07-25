using UnityEngine;
using DG.Tweening;

public class RotatableTrap : Trap
{
    [Header("Маятниковое вращение")]
    [SerializeField] private Transform _target;                  // объект для вращения
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;
    [SerializeField] private float _maxAngle = 30f;
    [SerializeField] private float _duration = 2f;

    private Quaternion _startRotation;

    private void Start()
    {
        if (_target == null)
            _target = transform;

        _startRotation = _target.localRotation;
        Vector3 axis = _rotationAxis.normalized;

        if (axis == Vector3.right)
        {
            AnimateRotation(Vector3.right);
        }
        else if (axis == Vector3.up)
        {
            AnimateRotation(Vector3.up);
        }
        else if (axis == Vector3.forward)
        {
            AnimateRotation(Vector3.forward);
        }
        else
        {
            AnimateRotationCustomAxis(axis);
        }
    }

    private void AnimateRotation(Vector3 axis)
    {
        Vector3 startEuler = _target.localEulerAngles;

        startEuler = NormalizeAngles(startEuler);

        float startAngle = 0f;

        if (axis == Vector3.right)
            startAngle = startEuler.x;
        else if (axis == Vector3.up)
            startAngle = startEuler.y;
        else if (axis == Vector3.forward)
            startAngle = startEuler.z;

        float fromAngle = startAngle - _maxAngle;
        float toAngle = startAngle + _maxAngle;

        Sequence seq = DOTween.Sequence();
        seq.Append(
            DOTween.To(() => fromAngle, x =>
            {
                Vector3 euler = _target.localEulerAngles;
                euler = NormalizeAngles(euler);
                if (axis == Vector3.right) euler.x = x;
                else if (axis == Vector3.up) euler.y = x;
                else if (axis == Vector3.forward) euler.z = x;
                _target.localEulerAngles = euler;
            }, toAngle, _duration)
            .SetEase(Ease.InOutSine)
        );

        seq.SetLoops(-1, LoopType.Yoyo);
    }

    private void AnimateRotationCustomAxis(Vector3 axis)
    {
        float fromAngle = -_maxAngle;
        float toAngle = _maxAngle;

        Sequence seq = DOTween.Sequence();

        seq.Append(
            DOTween.To(() => fromAngle, x =>
            {
                Quaternion rot = Quaternion.AngleAxis(x, axis.normalized);
                _target.localRotation = _startRotation * rot;
            }, toAngle, _duration)
            .SetEase(Ease.InOutSine)
        );

        seq.SetLoops(-1, LoopType.Yoyo);
    }

    private Vector3 NormalizeAngles(Vector3 angles)
    {
        return new Vector3(
            NormalizeAngle(angles.x),
            NormalizeAngle(angles.y),
            NormalizeAngle(angles.z)
        );
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360;

        if (angle > 180)
            angle -= 360;
        if (angle < -180)
            angle += 360;

        return angle;
    }
}
