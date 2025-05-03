using UnityEngine;

[CreateAssetMenu(fileName = "MoveSetting", menuName = "UI/MoveSetting")]
public class MoveSetting : ScriptableObject
{
    [Header("최대 Card 기울기")]
    public float maxZRotation = 60f;

    [Header("Card 기울림 민감도")]
    public float baseSensitivity = 100f;

    [Header("Card 최저 이동 속도")]
    public float minLerpSpeed = 1f;

    [Header("Card 최대 이동 속도")]
    public float maxLerpSpeed = 25f;

    [Header("Card 기울기 복귀 속도")]
    public float restoreLerpSpeed = 8f;

    [Header("손떨림 방지")]
    public float minMoveThreshold = 0.1f;
}
