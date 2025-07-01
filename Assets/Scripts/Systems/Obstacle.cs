using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagableActor))
        {
            damagableActor.TakeDamage();
            Destroy(gameObject);
        }
    }
}