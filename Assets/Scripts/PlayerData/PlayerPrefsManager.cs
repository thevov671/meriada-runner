using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    private const string GameLevel = nameof(GameLevel);
    private const int FirstGameLevel = 1;

    public static int GetGameLevel()
    {
        if (PlayerPrefs.HasKey(GameLevel))
            return PlayerPrefs.GetInt(GameLevel);
        else
            return FirstGameLevel;
    }

    public static bool TrySaveGameLevel(int level)
    {
        if (GetGameLevel() >= level)
            return false;

        PlayerPrefs.SetInt(GameLevel, level);

        return true;
    }

    public static void SaveScore(int value)
    {
        int score = PlayerPrefs.GetInt("Score");

        score += value;

        PlayerPrefs.SetInt("Score", score);
    }

    public static int GetScore()
    {
        return PlayerPrefs.GetInt("Score");
    }
}
