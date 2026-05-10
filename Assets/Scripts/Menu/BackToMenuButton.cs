using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(Button))]
    public class BackToMenuButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private int sceneBuildIndex = 1;

        private void Reset() => button = GetComponent<Button>();
        private void Awake() => button.onClick.AddListener(OnClick);

        private void OnClick() => SceneManager.LoadScene(sceneBuildIndex);
    }
}
