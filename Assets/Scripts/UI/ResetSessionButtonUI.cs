using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ResetSessionButtonUI : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        private void OnEnable()  { _btn.onClick.AddListener(OnClick); }
        private void OnDisable() { _btn.onClick.RemoveListener(OnClick); }

        private void OnClick()
        {
            GameManager.Instance.ResetSession();
            EventBus.RaiseUiBanner("Session reset.", 1.5f);
        }
    }
}
