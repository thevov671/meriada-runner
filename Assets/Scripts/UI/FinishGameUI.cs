using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FinishGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        // Отображаем финальный счёт
        int finalScore = PlayerPrefsManager.GetScore();
        _scoreText.text = $"Вы завершили игру!\nФинальный счёт: {finalScore}";

        // Подписка на кнопки
        _restartButton.onClick.AddListener(OnRestartClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnRestartClicked()
    {
        // Очистка сохранений
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Перезагрузка первой сцены (по индексу 0)
        SceneManager.LoadScene(0);
    }

    private void OnQuitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Для выхода из Play Mode в редакторе
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
