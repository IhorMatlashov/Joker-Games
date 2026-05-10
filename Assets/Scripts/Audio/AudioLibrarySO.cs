using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Joker/Audio Library")]
    public class AudioLibrarySo : ScriptableObject
    {
        [Header("Spin")]
        public AudioClip wheelSpinLoop;
        public AudioClip ballRoll;
        public AudioClip ballLand;
        public AudioClip[] fretClacks;
        [Range(0f, 0.3f)] public float fretClackPitchJitter = 0.12f;

        [Range(0f, 0.3f)] public float spinPitchJitter = 0.10f;

        [Header("Bets")]
        public AudioClip chipPlace;
        public AudioClip chipClear;

        public AudioClip[] chipPlaceSounds;

        [Range(0f, 0.3f)] public float chipPitchJitter = 0.12f;

        [Header("Outcome")]
        public AudioClip win;
        public AudioClip lose;

        [Header("UI")]
        public AudioClip click;
    }
}
