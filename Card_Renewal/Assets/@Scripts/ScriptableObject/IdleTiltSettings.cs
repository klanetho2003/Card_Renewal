using UnityEngine;

[CreateAssetMenu(menuName = "UI/IdleTiltSettings", fileName = "IdleTiltSettings")]
public class IdleTiltSettings : ScriptableObject
{
    [Header("Idle Tilt 최대 각도")]
    public float maxAngle = 15f;

    [Header("Idle Tilt 보간 속도 (근데 조정해도 별 차이가 없)")]
    public float lerpSpeed = 40f;
}
