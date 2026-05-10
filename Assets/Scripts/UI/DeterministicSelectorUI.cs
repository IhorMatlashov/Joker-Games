using System.Collections.Generic;
using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeterministicSelectorUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Button clearButton;

        private List<int> _values;

        public void Initialize(GameContext ctx)
        {
            BuildOptions(ctx.Variant);
            if (clearButton != null) clearButton.onClick.AddListener(OnClear);
            if (dropdown != null)    dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable()  { EventBus.OnVariantChanged += OnVariant; }
        private void OnDisable() { EventBus.OnVariantChanged -= OnVariant; }

        private void OnVariant(RouletteVariant v) => BuildOptions(v);

        private void BuildOptions(RouletteVariant variant)
        {
            _values = new List<int> { -2 };
            var labels = new List<string> { "Random" };

            _values.Add(0); labels.Add("0");
            if (variant == RouletteVariant.American)
            {
                _values.Add(RouletteVariantExtensions.DoubleZero);
                labels.Add("00");
            }
            for (int n = 1; n <= 36; n++) { _values.Add(n); labels.Add(n.ToString()); }

            if (dropdown != null)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(labels);
                dropdown.SetValueWithoutNotify(0);
            }
        }

        private void OnValueChanged(int idx)
        {
            if (idx <= 0 || idx >= _values.Count) { GameManager.Instance.SetDeterministicNext(null); return; }
            GameManager.Instance.SetDeterministicNext(_values[idx]);
            EventBus.RaiseUiBanner($"Next spin → {RouletteVariantExtensions.DisplayName(_values[idx])}", 1.5f);
        }

        private void OnClear()
        {
            if (dropdown != null) dropdown.SetValueWithoutNotify(0);
            GameManager.Instance.SetDeterministicNext(null);
        }
    }
}
