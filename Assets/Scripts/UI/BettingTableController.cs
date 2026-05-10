using System.Collections.Generic;
using Betting;
using Core;
using Data;
using UnityEngine;

namespace UI
{
    public class BettingTableController : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private RectTransform cells;
        [SerializeField] private Transform chipAnchorsRoot;
        [SerializeField] private List<GameObject> americanSpots = new();
        [SerializeField] private List<GameObject> europeanSpots = new();

        private void Awake() => ResolveChipAnchors();

        public void Initialize(GameContext ctx) => Apply(ctx.Variant);

        private void OnEnable() => EventBus.OnVariantChanged += Apply;
        private void OnDisable() => EventBus.OnVariantChanged -= Apply;

        public void Apply(RouletteVariant variant)
        {
            bool american = variant == RouletteVariant.American;
            SetActive(americanSpots, american);
            SetActive(europeanSpots, !american);
        }


        private void ResolveChipAnchors()
        {
            if (cells == null || chipAnchorsRoot == null) return;
            foreach (var spot in cells.GetComponentsInChildren<BetSpot>(true))
            {
                var cfg = spot.Area;
                if (cfg == null) continue;
                var variant = americanSpots.Contains(spot.gameObject)
                    ? RouletteVariant.American
                    : RouletteVariant.European;
                var anchor = FindAnchor(cfg.category, cfg.a, variant);
                if (anchor != null) spot.SetChipAnchor(anchor);
            }
        }

        private Transform FindAnchor(BetCategory cat, int a, RouletteVariant variant)
        {
            string name = AnchorName(cat, a, variant);
            return name != null ? chipAnchorsRoot.Find(name) : null;
        }

        private static string AnchorName(BetCategory cat, int a, RouletteVariant variant) => cat switch
        {
            BetCategory.Straight when a == 0 => variant == RouletteVariant.American ? "0 American" : "0 European",
            BetCategory.Straight when a == RouletteVariantExtensions.DoubleZero => "00 American",
            BetCategory.Straight => a.ToString(),
            BetCategory.Column   => $"Col {a}",
            BetCategory.Dozen    => a == 0 ? "1 st 12" : a == 1 ? "2 nd 12" : "3 rd 12",
            BetCategory.HighLow  => a == 0 ? "1 to 18" : "19 to 36",
            BetCategory.EvenOdd  => a == 1 ? "Even" : "Odd",
            BetCategory.RedBlack => a == 1 ? "Red" : "Black",
            _                    => null
        };

        private static void SetActive(List<GameObject> spots, bool active)
        {
            for (int i = 0; i < spots.Count; i++)
                if (spots[i] != null) spots[i].SetActive(active);
        }
    }
}
