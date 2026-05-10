using Core;
using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class VariantIndicatorUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private string format = "Variant: {0}";

        public void Initialize(GameContext ctx) => Render(ctx.Variant);

        private void OnEnable() => EventBus.OnVariantChanged += Render;
        private void OnDisable() => EventBus.OnVariantChanged -= Render;

        private void Render(RouletteVariant v)
        {
            if (label == null) return;
            label.text = string.Format(format, v == RouletteVariant.American ? "American" : "European");
        }
    }
}
