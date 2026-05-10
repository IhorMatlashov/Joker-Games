using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class VariantToggleUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;

        private void Reset() => button = GetComponent<Button>();

        public void Initialize(GameContext ctx) => OnVariant(ctx.Variant);

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
            EventBus.OnVariantChanged   += OnVariant;
            EventBus.OnGameStateChanged += OnState;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
            EventBus.OnVariantChanged   -= OnVariant;
            EventBus.OnGameStateChanged -= OnState;
        }

        private void OnState(GameState s) { button.interactable = s == GameState.Betting; }

        private void OnVariant(RouletteVariant v)
        {
            string txt = v == RouletteVariant.European ? "European (0)" : "American (0/00)";
            if (label != null) label.text = txt;
        }

        private void OnClick() => GameManager.Instance.ToggleVariant();
    }
}
