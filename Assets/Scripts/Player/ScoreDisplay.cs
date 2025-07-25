using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _animationDuration = 0.5f;

    private int _currentValue = 0;
    private Tween _activeTween;

    public void UpdateText(int newValue)
    {
        _activeTween?.Kill();

        _activeTween = DOTween.To(
            () => _currentValue,
            x => {
                _currentValue = x;
                _text.text = _currentValue.ToString();
            },
            newValue,
            _animationDuration
        ).SetEase(Ease.OutQuad);
    }
}
