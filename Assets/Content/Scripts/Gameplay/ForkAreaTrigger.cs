using System;
using System.Collections;
using UnityEngine;

public class ForkAreaTrigger : MonoBehaviour
{
    private const float ValueToChoose = 1;

    [SerializeField] private Transform _leftRoadPivot;
    [SerializeField] private Transform _rightRoadPivot;
    [SerializeField] private CanvasGroup _canvasGroup;

    private IPlayerInput _playerInput;
    private float _currentChooseValue;

    public static event Action<Transform> PathChosen;

    public void Activate(IPlayerInput input)
    {
        _playerInput = input;

        _playerInput.HorizontalInputChanged += OnPlayerHorizontalInputChanged;

        _canvasGroup.alpha = 1;
    }

    private void OnDisable()
    {
        if (_playerInput != null)
            _playerInput.HorizontalInputChanged -= OnPlayerHorizontalInputChanged;
    }

    private void OnPlayerHorizontalInputChanged(float value)
    {
        _currentChooseValue += value * Time.deltaTime;
        Debug.Log(_currentChooseValue);

        if (Mathf.Abs(_currentChooseValue) > ValueToChoose)
        {
            PathChosen?.Invoke(_currentChooseValue > 0? _rightRoadPivot : _leftRoadPivot);
            _canvasGroup.alpha = 0;
        }
    }
}
