using System.Text;
using Core;
using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StatsPanelUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private TextMeshProUGUI label;

        private static readonly StringBuilder Sb = new();

        private void OnEnable()  { EventBus.OnStatsChanged += OnStats; }
        private void OnDisable() { EventBus.OnStatsChanged -= OnStats; }

        public void Initialize(GameContext ctx) => Render(ctx.Stats.Snapshot());

        private void OnStats(PlayerStatsSnapshot s) => Render(s);

        public void Render(PlayerStatsSnapshot s)
        {
            if (label == null) return;
            label.text = Format(s);
        }

        public static string Format(PlayerStatsSnapshot s)
        {
            Sb.Clear();
            Sb.AppendLine($"Spins:         {s.SpinsPlayed}");
            Sb.AppendLine($"Wins:          {s.RoundsWon}");
            Sb.AppendLine($"Losses:        {s.RoundsLost}");
            Sb.AppendLine($"Win Rate:      {s.WinRate:P0}");
            Sb.AppendLine($"Total Wagered: {s.TotalWagered}");
            Sb.AppendLine($"Total Returned:{s.TotalReturned}");
            Sb.AppendLine($"Net Profit:    {(s.NetProfit >= 0 ? "+" : "")}{s.NetProfit}");
            Sb.AppendLine($"Biggest Win:   {s.BiggestWin}");
            Sb.AppendLine($"Biggest Loss:  {s.BiggestLoss}");
            Sb.Append    ($"Streak:        {(s.CurrentStreak > 0 ? "W" : s.CurrentStreak < 0 ? "L" : "-")} {Mathf.Abs(s.CurrentStreak)}");
            return Sb.ToString();
        }
    }
}
