using UnityEngine;

public static class PlatformDetector
{
    private const int MaxMobileWidth = 900;
    private const float MinMobileAspectRatio = 1.6f; // например 16:10 или 16:9
    private const int MinMobileDPI = 100; // обычные мониторы ~70-90, мобилки выше 100+

    public static bool IsMobile()
    {
        // Явная проверка по Unity API
        if (Application.isMobilePlatform)
            return true;

        // Проверка по физическим параметрам
        bool looksLikePhoneResolution = Screen.width <= MaxMobileWidth;
        bool tallAspectRatio = (float)Screen.height / Screen.width > MinMobileAspectRatio;
        bool highDPI = Screen.dpi > MinMobileDPI;

        return looksLikePhoneResolution && tallAspectRatio && highDPI;
    }
}
