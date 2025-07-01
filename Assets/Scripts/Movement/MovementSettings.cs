using UnityEngine;

[CreateAssetMenu(menuName = "Player/Movement Settings")]
public class MovementSettings : ScriptableObject
{
    [field: SerializeField] public float ForwardSpeed { get; private set; }
    [field: SerializeField] public float SideSpeed { get; private set; }
    [field: SerializeField] public float SideClampDistance { get; private set; }
}
