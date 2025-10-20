using UnityEngine;

namespace _root.Notification
{
    [CreateAssetMenu(fileName = "SpeechBubbleConfig", menuName = "Configs/SpeechBubbleConfig")]
    public class TextBubbleConfig : ScriptableObject
    {
        public int BubbleVibrato = 50;
        public float BubbleStartSize = 0.5f;
        public float GrowDuration = 0.1f;
        public float HideDuration = 0.1f;
        public float YOffset = 100f;

        [Header("Hide Settings")]
        public float AutoHideDelay = 10f;

        [Header("Typing Settings")]
        [Tooltip("Время между появлением символов (секунды).")]
        public float CharacterDelay = 0.03f;

        [Tooltip("Дополнительная случайная задержка, чтобы текст выглядел естественнее.")]
        public float RandomDelay = 0.01f;

        [Header("Animation Settings")]
        [Tooltip("Смещение символов при появлении.")]
        public float CharMoveY = 10f;

        [Tooltip("Длительность анимации появления символа.")]
        public float CharAnimDuration = 0.15f;

        [Tooltip("Прозрачность символа при старте (0–1).")]
        public float StartAlpha = 0f;
    }
}