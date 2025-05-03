using UnityEngine;

[CreateAssetMenu(fileName = "ClickSetting", menuName = "UI/ClickSetting")]
public class ClickSetting : ScriptableObject
{
    [Header("On PointDown Scale 변화 적용 여부")]
    public bool IsApplyScaleAnimation = true;

    [Header("On PointDown Scale 크기 변화")]
    public float scaleOnSelect = 1.2f;

    [Header("On PointDown Scale 위치 변화")]
    public float scaleTransition = 0.15f;

    [Header("On PointDown 그림자 위치 변화")]
    public float shadowOffset = 20f;

    [Header("Select 시 흔들림 크기")]
    public float selectPunchAmount = 20f;

    [Header("Select 중일 때 Card y좌표 변화")]
    public float selectPositionY_Offset = 40f;
}
