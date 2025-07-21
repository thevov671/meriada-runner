using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        int level = PlayerPrefsManager.GetGameLevel();

        SceneManager.LoadScene(level);
    }
}
