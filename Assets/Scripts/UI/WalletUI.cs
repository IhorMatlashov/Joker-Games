using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WalletUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private TextMeshProUGUI label;

        private void Reset() => label = GetComponent<TextMeshProUGUI>();

        private void OnEnable()  { EventBus.OnBalanceChanged += OnBalanceChanged; }
        private void OnDisable() { EventBus.OnBalanceChanged -= OnBalanceChanged; }

        public void Initialize(GameContext ctx) => Render(ctx.Wallet.Balance);

        private void OnBalanceChanged(int oldB, int newB) => Render(newB);

        public void Render(int balance)
        {
            if (label == null) return;
            label.text = $"{balance}$";
        }
    }
}
