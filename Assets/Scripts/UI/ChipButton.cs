using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ChipButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private ChipDenominationSo chip;

        public Button Button => button;
        public ChipDenominationSo Chip => chip;

        private void Reset() => button = GetComponent<Button>();
    }
}
