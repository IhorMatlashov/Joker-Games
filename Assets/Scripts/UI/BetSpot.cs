using System.Collections.Generic;
using Betting;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class BetSpot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private BetSpotConfig area = new();
        [SerializeField] private Image chipMarker;
        [SerializeField] private TMPro.TextMeshProUGUI chipMarkerLabel;
        [SerializeField] private Transform chip3DAnchor;
        [SerializeField] private float chipThickness = 0.012f;

        public BetSpotConfig Area => area;
        public void SetChipAnchor(Transform anchor) => chip3DAnchor = anchor;

        private readonly List<Bet> _myBets = new();
        private readonly List<GameObject> _spawnedChips = new();

        private Color _restColor;

        private void OnEnable()
        {
            EventBus.OnBetsCleared   += OnBetsCleared;
            EventBus.OnRoundResolved += OnRoundResolved;
            RefreshMarker();
        }

        private void OnDisable()
        {
            EventBus.OnBetsCleared   -= OnBetsCleared;
            EventBus.OnRoundResolved -= OnRoundResolved;
        }

        private void OnDestroy() => ClearChips();

        public void OnPointerClick(PointerEventData e)
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.State != Core.GameState.Betting) return;

            if (e.button == PointerEventData.InputButton.Right)
            {
                RefundOne();
                return;
            }

            int chip = gm.ChipSelector != null ? gm.ChipSelector.CurrentValue : 0;
            if (chip <= 0) return;

            var bet = area.Build(chip);
            if (gm.Bets.TryPlaceBet(bet))
            {
                _myBets.Add(bet);
                SpawnChip(bet.Amount);
                RefreshMarker();
            }
            else EventBus.RaiseUiBanner("Insufficient balance.");
        }

        private void RefundOne()
        {
            int last = _myBets.Count - 1;
            if (last < 0) return;
            var bet = _myBets[last];
            if (GameManager.Instance.Bets.RemoveBet(bet))
            {
                _myBets.RemoveAt(last);
                DestroyTopChip();
                RefreshMarker();
            }
        }

        private void OnBetsCleared() { _myBets.Clear(); ClearChips(); RefreshMarker(); }
        private void OnRoundResolved(Data.SpinResult _) { _myBets.Clear(); ClearChips(); RefreshMarker(); }

        private void RefreshMarker()
        {
            int sum = 0;
            for (int i = 0; i < _myBets.Count; i++) sum += _myBets[i].Amount;

            if (chipMarker != null) chipMarker.gameObject.SetActive(sum > 0);
            if (chipMarkerLabel != null) chipMarkerLabel.text = sum > 0 ? sum.ToString() : "";
        }

        private void SpawnChip(int amount)
        {
            if (chip3DAnchor == null) return;
            var cfg = GameManager.Instance != null ? GameManager.Instance.Config : null;
            int max = cfg != null ? cfg.maxStackedChipsPerSpot : 25;
            if (_spawnedChips.Count >= max) return;
            var prefab = ResolveChipPrefab(amount);
            if (prefab == null) return;

            var pos = chip3DAnchor.position + new Vector3(0f, _spawnedChips.Count * chipThickness, 0f);
            var go = ChipPool.Instance != null
                ? ChipPool.Instance.Get(prefab, pos, chip3DAnchor.rotation, chip3DAnchor)
                : Instantiate(prefab, pos, chip3DAnchor.rotation, chip3DAnchor);
            _spawnedChips.Add(go);
        }

        private void DestroyTopChip()
        {
            int last = _spawnedChips.Count - 1;
            if (last < 0) return;
            ReleaseOrDestroy(_spawnedChips[last]);
            _spawnedChips.RemoveAt(last);
        }

        private void ClearChips()
        {
            for (int i = 0; i < _spawnedChips.Count; i++)
                ReleaseOrDestroy(_spawnedChips[i]);
            _spawnedChips.Clear();
        }

        private static void ReleaseOrDestroy(GameObject go)
        {
            if (go == null) return;
            if (ChipPool.Instance != null) ChipPool.Instance.Release(go);
            else Destroy(go);
        }

        private GameObject ResolveChipPrefab(int amount)
        {
            var cfg = GameManager.Instance != null ? GameManager.Instance.Config : null;
            if (cfg == null || cfg.chipDenominations == null) return null;
            for (int i = 0; i < cfg.chipDenominations.Length; i++)
            {
                var c = cfg.chipDenominations[i];
                if (c != null && c.value == amount && c.prefab != null) return c.prefab;
            }
            return null;
        }
    }
}
