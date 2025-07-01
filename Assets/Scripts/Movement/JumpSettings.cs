using UnityEngine;

[CreateAssetMenu(menuName = "Player/Jump Settings")]
public class JumpSettings : ScriptableObject
{
    [field: SerializeField] public AnimationCurve JumpCurveY { get; private set; }
    [field: SerializeField] public AnimationCurve JumpCurveZ { get; private set; }
    [field: SerializeField] public float JumpTime { get; private set; }
    [field: SerializeField] public float JumpHeight { get; private set; }
    [field: SerializeField] public float JumpLength { get; private set; }
}
