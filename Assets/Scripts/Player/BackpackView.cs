using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class BackpackView : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _backpackRoot;
    [SerializeField] private List<Transform> _cubes;

    [Header("Настройки стаканья")]
    [SerializeField] private float _verticalOffset = 0.31f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _cubeFlySpeed = 7f;
    [SerializeField] private float _arriveDistance = 0.05f;
    [SerializeField] private float _targetScale = 0.25f;

    [Header("Настройки раскачки")]
    [SerializeField] private float _swayAngle = 8f;
    [SerializeField] private float _swayDuration = 0.2f;

    [Header("Настройки бонусов")]
    [SerializeField] private float _delayToStart = 0.5f;
    [SerializeField] private float _delayBetweenCubes = 0.1f;
    [SerializeField] private float _flyDuration = 0.5f;
    [SerializeField] private float _endScaleFactor = 2f;

    [Header("Поворот кубиков при движении")]
    [SerializeField] private float _yawAngle = 20f;
    [SerializeField] private float _yawDuration = 0.25f;

    [Header("SFX")]
    [SerializeField] private AudioClip _bonusOnFinishSFX;


    private Vector3 _baseScale;

    private void Start()
    {
        _baseScale = transform.localScale;
    }

    public void RotateWithMovement(float direction)
    {
        float targetYaw = Mathf.Clamp(direction, -1f, 1f) * _yawAngle;

        for (int i = 0; i < _cubes.Count; i++)
        {
            Transform cube = _cubes[i];
            Vector3 targetEuler = new Vector3(cube.localRotation.eulerAngles.x, targetYaw, cube.localRotation.eulerAngles.z);
            cube.DOLocalRotate(targetEuler, _yawDuration).SetEase(Ease.OutSine);
        }
    }


    public void AddCube(Transform newCube)
    {
        newCube.SetParent(null);
        StartCoroutine(FlyToBackpack(newCube));
    }

    private IEnumerator FlyToBackpack(Transform cube)
    {
        Vector3 jumpTarget = cube.position + Vector3.up * _jumpHeight;
        Vector3 scaleTarget = Vector3.one * _targetScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(cube.DOJump(jumpTarget, 2f, 1, 0.3f).SetEase(Ease.OutQuad));
        seq.Join(cube.DOScale(scaleTarget, 0.3f).SetEase(Ease.InOutQuad));
        yield return seq.WaitForCompletion();

        // Подлёт к цели
        while (true)
        {
            Vector3 target = GetCurrentTargetPosition();
            cube.position = Vector3.MoveTowards(cube.position, target, _cubeFlySpeed * Time.deltaTime);
            if (Vector3.Distance(cube.position, target) <= _arriveDistance)
                break;

            yield return null;
        }

        SetupCube(cube);
    }

    private Vector3 GetCurrentTargetPosition()
    {
        if (_cubes.Count == 0)
            return _backpackRoot.position;
        else
            return _cubes[_cubes.Count - 1].position + Vector3.up * _verticalOffset;
    }

    private void SetupCube(Transform cube)
    {
        cube.SetParent(null);

        Rigidbody rb = cube.gameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 5f + _cubes.Count * 0.1f;
        rb.solverIterations = 20;
        rb.solverVelocityIterations = 20;

        HingeJoint joint = cube.gameObject.AddComponent<HingeJoint>();
        joint.axis = Vector3.forward;
        joint.useLimits = true;
        joint.limits = new JointLimits { min = -5f, max = 5f };
        joint.useSpring = true;
        joint.spring = new JointSpring { spring = 200f, damper = 5f, targetPosition = 0f };

        joint.connectedBody = _cubes.Count == 0
            ? _backpackRoot.GetComponent<Rigidbody>()
            : _cubes[^1].GetComponent<Rigidbody>();

        _cubes.Add(cube);
    }

    public void RemoveItem()
    {
        if (_cubes.Count == 0)
            return;

        Transform topCube = _cubes[^1];
        _cubes.RemoveAt(_cubes.Count - 1);
        topCube.SetParent(null);

        HingeJoint joint = topCube.GetComponent<HingeJoint>();
        if (joint != null)
        {
            joint.connectedBody = null;
            Destroy(joint);
        }

        Rigidbody rb = topCube.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Vector3 jumpTarget = topCube.position + Vector3.back + Vector3.up;
        Sequence seq = DOTween.Sequence();
        seq.Append(topCube.DOJump(jumpTarget, 1f, 1, 0.5f).SetEase(Ease.OutQuad));
        seq.Join(topCube.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            if (rb != null) Destroy(rb);
            Destroy(topCube.gameObject);
        });
    }

    public void AnimateScoreConversionToCameraCorner(Camera cam, float targetDistance = 5f, Action<int> onPointAdded = null, Action onComplete = null)
    {
        StartCoroutine(ConvertCubesToScreenCorner(cam, targetDistance, onPointAdded, onComplete));
    }

    private IEnumerator ConvertCubesToScreenCorner(Camera cam, float distance, Action<int> onPointAdded, Action onComplete)
    {
        yield return new WaitForSeconds(_delayToStart);


        for (int i = _cubes.Count - 1; i >= 0; i--)
        {
            AudioManager.Instance.PlayClip(_bonusOnFinishSFX);
            Transform cube = _cubes[i];
            _cubes.RemoveAt(i);
            cube.SetParent(null);

            Vector3 screenCorner = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

            Sequence seq = DOTween.Sequence();
            seq.Append(cube.DOJump(screenCorner, 2f, 1, _flyDuration).SetEase(Ease.InQuad));
            seq.Join(cube.DOScale(_baseScale * _endScaleFactor, _flyDuration).SetEase(Ease.InQuad));
            seq.OnComplete(() =>
            {
                onPointAdded?.Invoke(PlayerController.BonusPerItem);
                Destroy(cube.gameObject);
            });

            yield return new WaitForSeconds(_delayBetweenCubes);
        }

        onComplete?.Invoke();
    }

    public void Lean(float direction)
    {
        float targetAngle = Mathf.Clamp(direction, -1f, 1f) * -_swayAngle;
        _backpackRoot.DOLocalRotate(new Vector3(0, 0, targetAngle), _swayDuration).SetEase(Ease.OutSine);
    }

    public void ResetLean()
    {
        for (int i = 0; i < _cubes.Count; i++)
        {
            _cubes[i].DOLocalRotate(Vector3.zero, _swayDuration).SetEase(Ease.OutSine);
        }

        _backpackRoot.DOLocalRotate(Vector3.zero, _swayDuration).SetEase(Ease.OutSine);
    }

}