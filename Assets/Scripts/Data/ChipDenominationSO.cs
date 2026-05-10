using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Chip_", menuName = "Joker/Chip Denomination")]
    public class ChipDenominationSo : ScriptableObject
    {
        [Min(1)] public int value = 5;
        public GameObject prefab;
    }
}
