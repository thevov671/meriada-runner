using Unity.VisualScripting;
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

    private void Awake()
    {
        Time.timeScale = 1;
        bool isMobile = PlatformDetector.IsMobile();

        PlayerController playerInstance = Instantiate(_playerPrefab, _playerSpawnPosition.position, Quaternion.identity);
        playerInstance.Init(isMobile? new MobilePlayerInput() : new PcPlayerInput());

        _tutorialDisplay.Init(isMobile, playerInstance);
        _endGameDisplay.Init(playerInstance);

        // end game display rework
        // player controller rework (classes)
        // Cameras
        // new traps
        // randomize
    }
}
