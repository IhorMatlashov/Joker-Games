using System.Collections;
using Core;
using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BetResultDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI betResult;
        [SerializeField] private float displaySeconds = 3f;

        private Coroutine _hideCo;

        private void OnEnable()
        {
            EventBus.OnRoundResolved += OnResolved;
            betResult.gameObject.SetActive(false);
        }

        private void OnDisable() => EventBus.OnRoundResolved -= OnResolved;

        private void OnResolved(SpinResult r)
        {
            if (r.TotalWagered <= 0) return;

            if (r.PlayerWon)
            {
                betResult.text = $"You win {r.NetProfit}$!";
                betResult.color = Color.yellow;
            }
            else
            {
                betResult.text = $"You lost {r.TotalWagered}$";
                betResult.color = Color.red;
            }

            betResult.gameObject.SetActive(true);

            if (_hideCo != null) StopCoroutine(_hideCo);
            _hideCo = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(displaySeconds);
            betResult.gameObject.SetActive(false);
            _hideCo = null;
        }
    }
}
