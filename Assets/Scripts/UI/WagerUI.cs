using Betting;
using Core;
using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WagerUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private string format = "Bet: ${0:N0}";

        public void Initialize(GameContext ctx) => Render(ctx.Bets.TotalWagered);

        private void OnEnable()
        {
            EventBus.OnBetPlaced     += OnBetPlaced;
            EventBus.OnBetRemoved    += OnBetRemoved;
            EventBus.OnBetsCleared   += OnBetsCleared;
            EventBus.OnRoundResolved += OnRoundResolved;
        }

        private void OnDisable()
        {
            EventBus.OnBetPlaced     -= OnBetPlaced;
            EventBus.OnBetRemoved    -= OnBetRemoved;
            EventBus.OnBetsCleared   -= OnBetsCleared;
            EventBus.OnRoundResolved -= OnRoundResolved;
        }

        private void OnBetPlaced(Bet _) => Refresh();
        private void OnBetRemoved(Bet _) => Refresh();
        private void OnBetsCleared() => Refresh();
        private void OnRoundResolved(SpinResult _) => Refresh();

        private void Refresh()
        {
            int wager = GameManager.Instance != null && GameManager.Instance.Bets != null
                ? GameManager.Instance.Bets.TotalWagered
                : 0;
            Render(wager);
        }

        private void Render(int wager)
        {
            if (label == null) return;
            label.text = string.Format(System.Globalization.CultureInfo.InvariantCulture, format, wager);
        }

    }
}
