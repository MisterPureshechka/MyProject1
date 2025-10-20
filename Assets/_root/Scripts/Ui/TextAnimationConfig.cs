using UnityEngine;

namespace Scripts.Ui
{
    [CreateAssetMenu(menuName = "ScriptableObject/Ui/BaseTextAnimation")]
    public class TextAnimationConfig : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float RotatateToValue { get; private set; }
        [field: SerializeField] public float ScaleToValue { get; private set; }
        [field: SerializeField] public int Vibrato { get; private set; }
        [field: SerializeField] public float Randomness { get; set; }
        [field: SerializeField] public bool FadeOut { get; set; }
    }
}