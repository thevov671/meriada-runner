using UnityEngine;

[CreateAssetMenu(menuName = "Player/Camera Settings")]
public class PlayerCameraSettings : ScriptableObject
{
    [field: SerializeField] public float JumpDistanceIncrease { get; private set; } = 2f;
    [field: SerializeField] public float JumpZoomOutDuration { get; private set; } = 0.15f;
    [field: SerializeField] public float JumpHoldDuration { get; private set; } = 0.1f;
    [field: SerializeField] public float JumpReturnDuration { get; private set; } = 0.4f;
    [field: SerializeField] public float JumpVerticalOffset { get; private set; } = 1f;

}
