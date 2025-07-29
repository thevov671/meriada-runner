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
    [SerializeField] private float _endRotationValueX = -90;
    [SerializeField] private ParticleSystem _addItemVFX;

    [Header("Стабилизация башни")]
    [SerializeField] private float _springForce = 10000f; //5000f — мягко; 10000f — твердо; 20000f — как камень.
    [SerializeField] private float _springDamper = 100f;
    [SerializeField] private float _stabilizeDelay = 0.5f;


    [Header("Поворот кубиков при движении")]
    [SerializeField] private float _yawAngle = 8f;
    [SerializeField] private float _yawDuration = 0.25f;

    [Header("SFX")]
    [SerializeField] private AudioClip _bonusOnFinishSFX;

    private Vector3 _baseScale;
    private Coroutine _stabilizeRoutine;

    public int CubeCount => _cubes.Count;

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


    public void TryStabilizeTower()
    {
        if (_stabilizeRoutine != null)
            StopCoroutine(_stabilizeRoutine);

        _stabilizeRoutine = StartCoroutine(StabilizeTowerRoutine());
    }

    private IEnumerator StabilizeTowerRoutine()
    {
        yield return new WaitForSeconds(_stabilizeDelay);

        foreach (var cube in _cubes)
        {
            HingeJoint joint = cube.GetComponent<HingeJoint>();
            if (joint != null)
            {
                JointSpring spring = joint.spring;
                spring.spring = _springForce;
                spring.damper = _springDamper;
                spring.targetPosition = 0f;
                joint.spring = spring;
            }
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

        RotateChildObject(cube);
        SetupCube(cube);
        _addItemVFX.Play();
    }

    private void RotateChildObject(Transform cube)
    {
        Transform child = cube.GetChild(0);
        Vector3 targetEuler = new Vector3(_endRotationValueX, child.localEulerAngles.y, child.localEulerAngles.z);
        child.DOLocalRotate(targetEuler, 0.3f).SetEase(Ease.OutSine);
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
        rb.mass = 1f;
        rb.solverIterations = 40;
        rb.solverVelocityIterations = 20;

        ConfigurableJoint joint = cube.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = _cubes.Count == 0
            ? _backpackRoot.GetComponent<Rigidbody>()
            : _cubes[^1].GetComponent<Rigidbody>();

        joint.axis = Vector3.forward;
        joint.secondaryAxis = Vector3.up;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 10f; 
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;

        JointDrive angularDrive = new JointDrive
        {
            positionSpring = 20000f,
            positionDamper = 500f,
            maximumForce = Mathf.Infinity
        };
        joint.angularXDrive = angularDrive;

        _cubes.Add(cube);
    }


    public void RemoveItem()
    {
        if (_cubes.Count == 0)
            return;

        Transform topCube = _cubes[^1];
        _cubes.RemoveAt(_cubes.Count - 1);
        topCube.SetParent(null);

        ConfigurableJoint joint = topCube.GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            joint.connectedBody = null;
            Destroy(joint);
        }

        Rigidbody rb = topCube.GetComponent<Rigidbody>();

        Vector3 randomDir = (Vector3.up + UnityEngine.Random.onUnitSphere * 0.5f).normalized;
        randomDir.y = Mathf.Abs(randomDir.y);
        Vector3 jumpTarget = topCube.position + randomDir * 2f;

        Sequence seq = DOTween.Sequence();
        seq.Append(topCube.DOJump(jumpTarget, 1f, 1, 0.5f).SetEase(Ease.OutQuad));
        seq.Join(topCube.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            if (rb != null)
                Destroy(rb);
            Destroy(topCube.gameObject);
        });
    }





    public void AnimateScoreConversionToCameraCorner(Camera cam, float targetDistance = 5f, Action<int> onPointAdded = null, Action onComplete = null)
    {
        StartCoroutine(ConvertCubesToScreenCorner(cam, targetDistance, onPointAdded, onComplete));
    }

    private IEnumerator ConvertCubesToScreenCorner(Camera cam, float targetDistance, Action<int> onPointAdded, Action onComplete)
    {
        yield return new WaitForSeconds(_delayToStart);

        for (int i = _cubes.Count - 1; i >= 0; i--)
        {
            AudioManager.Instance.PlayClip(_bonusOnFinishSFX);
            Transform cube = _cubes[i];
            _cubes.RemoveAt(i);
            cube.SetParent(null);

            // Расстояние до камеры
            float distance = Vector3.Distance(cam.transform.position, cube.position);

            // Определяем экранный угол с корректным z
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