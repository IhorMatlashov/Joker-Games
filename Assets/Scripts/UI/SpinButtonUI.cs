using Betting;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class SpinButtonUI : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;

        private void Reset() => button = GetComponent<Button>();

        public void Initialize(GameContext ctx)
        {
            button.interactable = ctx.State == GameState.Betting && ctx.Bets.ActiveBets.Count > 0;
            if (label != null) label.text = ctx.State == GameState.Spinning ? "Spinning..." : "SPIN";
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
            EventBus.OnGameStateChanged += OnState;
            EventBus.OnBetPlaced        += OnBetPlaced;
            EventBus.OnBetRemoved       += OnBetRemoved;
            EventBus.OnBetsCleared      += Refresh;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
            EventBus.OnGameStateChanged -= OnState;
            EventBus.OnBetPlaced        -= OnBetPlaced;
            EventBus.OnBetRemoved       -= OnBetRemoved;
            EventBus.OnBetsCleared      -= Refresh;
        }

        private void OnState(GameState s) => Refresh();
        private void OnBetPlaced(Bet b)   => Refresh();
        private void OnBetRemoved(Bet b)  => Refresh();

        private void Refresh()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            button.interactable = gm.State == GameState.Betting && gm.Bets.ActiveBets.Count > 0;
            if (label != null) label.text = gm.State == GameState.Spinning ? "Spinning..." : "SPIN";
        }

        private void OnClick() => GameManager.Instance.Spin();
    }
}
