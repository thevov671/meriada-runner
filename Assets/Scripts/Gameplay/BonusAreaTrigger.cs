using System;
using TMPro;
using UnityEngine;

public class BonusAreaTrigger : MonoBehaviour
{
    [SerializeField] private int _bonusValue;
    [SerializeField] private MeshRenderer _triggerAreaRenderer;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Material _positiveBonusMat;
    [SerializeField] private Material _negativeBonusMat;

    private bool _triggered = false;

    public event Action Triggered;

    private void OnValidate()
    {
        _triggerAreaRenderer.material = _bonusValue > 0? _positiveBonusMat : _negativeBonusMat;
        _text.text = _bonusValue > 0? "+" + _bonusValue.ToString() : _bonusValue.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
            return;

        if (other.TryGetComponent(out PlayerController player))
        {
            player.AddBonus(_bonusValue);
            _triggered = true;
            Triggered?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void Disable()
    {
        _triggered = true;
    }
}
