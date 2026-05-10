using Core;
using Data;
using UnityEngine;

namespace UI
{
    public class BankruptcyHandler : MonoBehaviour
    {
        [SerializeField] private string message = "Out of chips — press RESET to start over";
        [SerializeField] private float bannerDuration = 4f;

        private bool _alreadyAlerted;

        private void OnEnable()
        {
            EventBus.OnRoundResolved += OnResolved;
            EventBus.OnBalanceChanged += OnBalanceChanged;
        }

        private void OnDisable()
        {
            EventBus.OnRoundResolved -= OnResolved;
            EventBus.OnBalanceChanged -= OnBalanceChanged;
        }

        private void OnResolved(SpinResult _) => CheckBroke();

        private void OnBalanceChanged(int _, int newB)
        {
            if (newB > 0) _alreadyAlerted = false;
            CheckBroke();
        }

        private void CheckBroke()
        {
            if (_alreadyAlerted) return;
            var gm = GameManager.Instance;
            if (gm == null || gm.Wallet == null) return;
            if (gm.Wallet.Balance > 0) return;
            if (gm.Bets != null && gm.Bets.ActiveBets.Count > 0) return;
            EventBus.RaiseUiBanner(message, bannerDuration);
            _alreadyAlerted = true;
        }
    }
}
