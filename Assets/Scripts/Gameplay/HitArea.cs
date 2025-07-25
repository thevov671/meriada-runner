using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private GameObject _parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.TakeHit();
            _parent.SetActive(false);
        }
    }
}
