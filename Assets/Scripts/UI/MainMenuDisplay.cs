using UnityEngine;
using UnityEngine.UI;

public class MainMenuDisplay : MonoBehaviour
{
    [SerializeField] private Button _openMenuButton;
    [SerializeField] private RectTransform _menu;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    public void ExitGame()
    {
#if UNITY_EDITOR
        // Для выхода из Play Mode в редакторе
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("Quit");
        Application.Quit();
    }

    public void OpenMenu()
    {
        _menu.gameObject.SetActive(!_menu.gameObject.activeSelf);
        Time.timeScale = _menu.gameObject.activeSelf ? 0 : 1;
    }

    public void Continue()
    {
        Time.timeScale = 1;
        _menu.gameObject.SetActive(false);
    }
}