using UnityEngine;

namespace Scripts.Sounds
{
    [CreateAssetMenu(fileName = "SoundService", menuName = "ScriptableObjects/SoundService")]
    public class SoundConfig : ScriptableObject
    {
        [field: SerializeField] public AudioClip MusicClip { get; set; }
        [field: SerializeField] public AudioClip DevClip { get; set; }
        [field: SerializeField] public AudioClip ShowerClip { get; set; }
        [field: SerializeField] public AudioClip ReadClip { get; set; }
        [field: SerializeField] public AudioClip PlayClip { get; set; }
        [field: SerializeField] public AudioClip OpenCatalogueClip { get; set; }
        [field: SerializeField] public AudioClip ShakeWorldClip { get; set; }
        [field: SerializeField] public AudioClip StepClip { get; set; }
        [field: SerializeField] public AudioClip[] BackgroundClip { get; set; }
    }
}