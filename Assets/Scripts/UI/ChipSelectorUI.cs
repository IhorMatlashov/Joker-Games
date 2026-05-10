using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UI
{
    public class ChipSelectorUI : MonoBehaviour
    {
        [SerializeField] private List<ChipButton> chips = new();

        public ChipDenominationSo Selected { get; private set; }
        public int CurrentValue => Selected != null ? Selected.value : 0;

        private void Awake()
        {
            for (int i = 0; i < chips.Count; i++)
            {
                int captured = i;
                if (chips[i] == null || chips[i].Button == null) continue;
                chips[i].Button.onClick.RemoveAllListeners();
                chips[i].Button.onClick.AddListener(() => Select(captured));
            }
            if (chips.Count > 0) Select(0);
        }

        public void Select(int index)
        {
            if (index < 0 || index >= chips.Count) return;
            Selected = chips[index].Chip;
            for (int i = 0; i < chips.Count; i++)
                if (chips[i] != null && chips[i].Button != null)
                    chips[i].Button.interactable = i != index;
        }
    }
}
