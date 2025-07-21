using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BackpackView : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _backpackRoot;
    [SerializeField] private List<Transform> _cubes;

    [Header("Настройки движения")]
    [SerializeField] private float _verticalOffset = 0.31f;
    [SerializeField] private float _followSpeed = 10f;
    [SerializeField] private float _swayAngle = 5f;
    [SerializeField] private float _swaySpeed = 10f;   

    [Header("Настройки очков")]
    [SerializeField] private float _delayBetweenCubes = 0.1f;
    [SerializeField] private float _flyDuration = 0.5f;
    [SerializeField] private float _delayToStart = 0.5f;
    [SerializeField] private float _endScaleFactor = 2f;

    private Vector3 _baseScale;

    private void Start()
    {
        _baseScale = transform.localScale;
    }

    public void AddCube(Transform newCube)
    {
        newCube.SetParent(null);

        StartCoroutine(FlyToBackpack(newCube));
    }

    private IEnumerator FlyToBackpack(Transform cube)
    {
        float flySpeed = 7f;
        float arriveDistance = 0.05f;

        Vector3 originalScale = cube.localScale;
        Vector3 targetScale = Vector3.one * 0.25f;

        // Цель прыжка вверх — на 3 юнита выше текущей позиции
        Vector3 jumpPeak = cube.position + Vector3.up * 3f;

        // Прыжок вверх и уменьшение
        Sequence seq = DOTween.Sequence();
        seq.Append(cube.DOJump(jumpPeak, 2f, 1, 0.3f).SetEase(Ease.OutQuad));
        seq.Join(cube.DOScale(targetScale, 0.3f).SetEase(Ease.InOutQuad));

        yield return seq.WaitForCompletion();

        // После прыжка вверх — начинаем следовать к башне
        while (true)
        {
            Vector3 target = GetCurrentTargetPosition();
            cube.position = Vector3.MoveTowards(cube.position, target, flySpeed * Time.deltaTime);

            if (Vector3.Distance(cube.position, target) <= arriveDistance)
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


    public void SetupCube(Transform cube)
    {
        cube.SetParent(null);
        var rb = cube.gameObject.AddComponent<Rigidbody>();
        rb.mass = 5f + _cubes.Count * 0.1f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.solverIterations = 12; // стандарт: 6
        rb.solverVelocityIterations = 12; // стандарт: 1

        var joint = cube.gameObject.AddComponent<HingeJoint>();
        joint.axis = Vector3.forward;
        joint.useLimits = true;

        JointLimits limits = new JointLimits
        {
            min = -5f,
            max = 5f
        };

        joint.limits = limits;

        if (_cubes.Count == 0)
        {
            joint.connectedBody = _backpackRoot.GetComponent<Rigidbody>();
        }
        else
        {
            joint.connectedBody = _cubes[_cubes.Count - 1].GetComponent<Rigidbody>();
        }

        _cubes.Add(cube);
    }


    public void AnimateScoreConversionToCameraCorner(Camera cam, float targetDistance = 5f, Action<int> onPointAdded = null)
    {
        StartCoroutine(ConvertCubesToScreenCorner(cam, targetDistance, onPointAdded));
    }

    private IEnumerator ConvertCubesToScreenCorner(Camera cam, float distance, Action<int> onPointAdded)
    {
        yield return new WaitForSeconds(_delayToStart);

        for (int i = _cubes.Count - 1; i >= 0; i--)
        {
            Transform cube = _cubes[i];
            _cubes.RemoveAt(i);

            cube.SetParent(null);

            Vector3 screenCorner = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

            Sequence seq = DOTween.Sequence();

            seq.Append(
                cube.DOJump(screenCorner, 2f, 1, _flyDuration).SetEase(Ease.InQuad)
            );

            seq.Join(
                cube.DOScale(_baseScale * _endScaleFactor, _flyDuration).SetEase(Ease.InQuad)
            );

            seq.OnComplete(() =>
            {
                onPointAdded?.Invoke(PlayerController.BonusPerItem);
                Destroy(cube.gameObject);
            });

            yield return new WaitForSeconds(_delayBetweenCubes);
        }
    }

    public void RemoveItem()
    {
        if (_cubes.Count == 0)
            return;

        Transform topCube = _cubes[_cubes.Count - 1];
        _cubes.RemoveAt(_cubes.Count - 1);

        topCube.SetParent(null);

        // Удаляем Joint и Rigidbody правильно
        var joint = topCube.GetComponent<HingeJoint>();
        if (joint != null)
        {
            joint.connectedBody = null;
            Destroy(joint);
        }

        var rb = topCube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // чтобы не мешал физикой пока DOTween играет
        }

        Sequence seq = DOTween.Sequence();

        Vector3 jumpTarget = topCube.position + Vector3.back * 1f + Vector3.up * 1f;

        seq.Append(topCube.DOJump(jumpTarget, 1f, 1, 0.5f).SetEase(Ease.OutQuad));
        seq.Join(topCube.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            if (rb != null) Destroy(rb);
            Destroy(topCube.gameObject);
        });
    }

}
