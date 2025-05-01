using UnityEngine;

[CreateAssetMenu(fileName = "HoverSetting", menuName = "UI/HoverSetting")]
public class HoverSetting : ScriptableObject
{
    [Header("Hovering 변화 적용 여부")]
    public bool IsApplyScaleAnimation = true;

    [Header("Hovering 크기 변화")]
    public float scaleOnHover = 1.07f;

    [Header("Hovering 위치 변화")]
    public float scaleTransition = 0.15f;

    [Header("Hover Trigger Animation 흔들림 강도")]
    public float hoverPunchAngle = 5f;

    [Header("Hover Trigger Animation 위치 변화 강도")]
    public float hoverTransition = 0.15f;
}
