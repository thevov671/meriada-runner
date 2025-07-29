using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TutorialDisplay _tutorialDisplay;
    [SerializeField] private EndGameDisplay _endGameDisplay;

    [Space(20)]

    [Header("Player Start Settings")]
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private Transform _playerSpawnPosition;

    [Header("Pooling")]
    [SerializeField] private HealthItem _healthItemPrefab;
    [SerializeField] private int _poolSize = 50;

    private void Awake()
    {
        Time.timeScale = 1;
        bool isMobile = PlatformDetector.IsMobile();

        var pool = new HealthItemPool(_healthItemPrefab, _poolSize, transform);

        PlayerController playerInstance = Instantiate(_playerPrefab, _playerSpawnPosition.position, Quaternion.identity);
        playerInstance.Init(isMobile ? new MobilePlayerInput() : new PcPlayerInput(), pool);

        _tutorialDisplay.Init(isMobile, playerInstance);
        _endGameDisplay.Init(playerInstance);
    }
}
