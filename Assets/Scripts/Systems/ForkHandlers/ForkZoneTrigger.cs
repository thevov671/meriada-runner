using UnityEngine;
using UnityEngine.UI;

public class ForkZoneTrigger : MonoBehaviour
{
    [SerializeField] private ForkData _forkData;

    [SerializeField] private Transform _leftPoint;
    [SerializeField] private Transform _rightPoint;

    private void Awake()
    {
        _forkData.LeftDirectionPoint = _leftPoint;
        _forkData.RightDirectionPoint = _rightPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.EnterFork(_forkData);
            gameObject.SetActive(false);
        }
    }
}
