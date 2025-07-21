using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class EndGameDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _loseScreen;
    [SerializeField] private CanvasGroup _winScreen;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _nextButton;

    private PlayerController _player;

    private void Awake()
    {
        _loseScreen.alpha = 0f;
        _winScreen.alpha = 0f;

        _loseScreen.gameObject.SetActive(false);
        _winScreen.gameObject.SetActive(false);
    }

    public void Init(PlayerController player)
    {
        _player = player;

        _player.Win += OnPlayerWin;
        _player.Lose += OnPlayerLose;

        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnNextButtonClicked()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("End Game");
            return;
            nextSceneIndex = 0;
        }

        PlayerPrefsManager.TrySaveGameLevel(nextSceneIndex);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void OnDestroy()
    {
        if (_player == null)
            return;

        _player.Win -= OnPlayerWin;
        _player.Lose -= OnPlayerLose;

        _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        _nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    private void OnPlayerWin()
    {
        _winScreen.gameObject.SetActive(true);

        _winScreen.DOFade(1f, 1f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutQuad);
    }

    private void OnPlayerLose()
    {
        _loseScreen.gameObject.SetActive(true);

        _loseScreen.DOFade(1f, 1f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutQuad);
    }
}
