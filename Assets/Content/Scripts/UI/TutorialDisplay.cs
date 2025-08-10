using System;
using UnityEngine;

public class TutorialDisplay : MonoBehaviour
{
    [SerializeField] private Animator _mouseAnimPrefab;
    [SerializeField] private Animator _fingerAnimPrefab;
    [SerializeField] private Transform _container;

    private PlayerController _player;

    public void Init(bool isMobile, PlayerController player)
    {
        _container.gameObject.SetActive(true);
        Instantiate(isMobile? _fingerAnimPrefab : _mouseAnimPrefab, _container);

        _player = player;

        _player.TutorialFinished += OnTutorialFinished;
    }

    private void OnDestroy()
    {
        if (_player == null)
            return;

        _player.TutorialFinished -= OnTutorialFinished;
    }

    private void OnTutorialFinished()
    {
        _container.gameObject.SetActive(false);
    }
}
