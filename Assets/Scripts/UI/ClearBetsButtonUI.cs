using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ClearBetsButtonUI : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Reset() => button = GetComponent<Button>();
        private void OnEnable()  { button.onClick.AddListener(OnClick);    EventBus.OnGameStateChanged += OnState; }
        private void OnDisable() { button.onClick.RemoveListener(OnClick); EventBus.OnGameStateChanged -= OnState; }
        private void OnState(GameState s) { button.interactable = s == GameState.Betting; }
        private void OnClick() => GameManager.Instance.ClearAllBets();
    }
}
