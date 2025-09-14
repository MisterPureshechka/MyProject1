using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonAnimConfig", menuName = "UI/Button Animation Config")]
public class ButtonAnimationData : ScriptableObject
{
    [Header("OnEnter")]
    public Ease OnEnterScaleEase;
    public Ease OnClickScaleEase;

    public Vector2 ShakeStrengthPos = new(5f, 5f);
    public Vector3 ShakeStrengthRot = new(0f, 0f, 5f);
    public float OnEnterScaleValue = 1.1f;
    public float OnEnterScaleDuration = 1.1f;
    public float ShakeDuration = .1f;
    public int Vibrato = 100;
    public float Randomness = 0;
    public bool FadeOut = true;

    [Header("OnClick")]
    public float OnClickScaleToValue = 1.1f;
    public float OnClickDuration = 0.1f;
 
    [Header("Scale")]
    public bool UsePunchScale = true;

    [Range(0f, 0.5f)] public float PunchAmount = 0.1f;
}