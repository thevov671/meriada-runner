using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TutorialDisplay _tutorialDisplay;
    [SerializeField] private CollectedItemsDisplay _collectedItemsDisplay;

    [Header("Player Start Settings")]
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private Transform _playerSpawnPosition;


    private void Awake()
    {
        bool isMobile = PlatformDetector.IsMobile();

        PlayerController playerInstance = Instantiate(_playerPrefab, _playerSpawnPosition.position, Quaternion.identity);
        playerInstance.Init(isMobile? new MobilePlayerInput() : new PcPlayerInput());

        _tutorialDisplay.Init(isMobile, playerInstance);
        _collectedItemsDisplay.Init(playerInstance);
    }
}
