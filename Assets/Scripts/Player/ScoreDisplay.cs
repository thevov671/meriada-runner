using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _animationDuration = 0.5f;

    private int _currentValue = 0;
    private Tween _activeTween;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowText()
    {
        gameObject.SetActive(true);
    }

    public void UpdateText(int value)
    {
        _activeTween?.Kill();

        _activeTween = DOTween.To(
            () => _currentValue,
            x => {
                _currentValue = x;
                _text.text = _currentValue.ToString();
            },
            value,
            _animationDuration
        ).SetEase(Ease.OutQuad);
    }
}
