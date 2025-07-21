using UnityEngine;

public class BonusGate : MonoBehaviour
{
    [SerializeField] private BonusAreaTrigger[] _areas;

    private void OnEnable()
    {
        foreach (var area in _areas)
        {
            area.Triggered += OnAreaTriggered;
        }
    }

    private void OnDisable()
    {
        foreach (var area in _areas)
        {
            area.Triggered -= OnAreaTriggered;
        }
    }

    private void OnAreaTriggered()
    {
        foreach (var area in _areas)
        {
            area.Disable();
        }
    }
}