using UnityEngine;

public class WinAreaTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.HandleWin();
            gameObject.SetActive(false);
        }
    }
}
