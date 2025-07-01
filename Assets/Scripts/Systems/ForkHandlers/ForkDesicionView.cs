using DG.Tweening;
using UnityEngine;

public class ForkDesicionView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;

    private PlayerController _player;

    public void Init(PlayerController player)
    {
        _player = player;

        _player.ForkEntered += Show;
        _player.ForkExited += Hide;
    }

    private void OnDestroy()
    {
        _player.ForkEntered -= Show;
        _player.ForkExited -= Hide;
    }

    public void Show(ForkData data)
    {
        gameObject.SetActive(true);
        _panel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
    }

    public void Hide()
    {
        _panel.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() => gameObject.SetActive(false));
    }
}
