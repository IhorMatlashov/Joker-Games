using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Joker/Game Config")]
    public class GameConfigSo : ScriptableObject
    {
        [Min(0)] public int startingBalance = 1000;

        public RouletteVariant defaultVariant = RouletteVariant.European;

        [Min(0.1f)] public float wheelSpinDuration = 5f;
        [Min(0.0f)] public float settledDelay = 0f;

        [Min(1)] public int maxStackedChipsPerSpot = 25;

        public ChipDenominationSo[] chipDenominations;
    }
}
