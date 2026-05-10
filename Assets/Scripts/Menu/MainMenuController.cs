using Core;
using Data;
using Player;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenuController : MonoBehaviour
    {
        public static RouletteVariant? PickedVariant;

        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject variantPanel;
        [SerializeField] private GameObject statsPanel;

        [Header("Main panel")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button statisticButton;
        [SerializeField] private Button quitButton;

        [Header("Variant panel")]
        [SerializeField] private Button europeanButton;
        [SerializeField] private Button americanButton;
        [SerializeField] private Button variantBackButton;

        [Header("Stats panel")]
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private Button statsBackButton;

        [Header("Config")]
        [SerializeField] private GameConfigSo config;

        [Header("Scenes (build index)")]
        [SerializeField] private int rouletteSceneIndex = 2;

        private void Start()
        {
            playButton.onClick.AddListener(() => Show(variantPanel));
            statisticButton.onClick.AddListener(() => { RefreshStats(); Show(statsPanel); });
            if (quitButton != null) quitButton.onClick.AddListener(Application.Quit);

            europeanButton.onClick.AddListener(() => StartRoulette(RouletteVariant.European));
            americanButton.onClick.AddListener(() => StartRoulette(RouletteVariant.American));
            variantBackButton.onClick.AddListener(() => Show(mainPanel));

            statsBackButton.onClick.AddListener(() => Show(mainPanel));

            Show(mainPanel);
        }

        private void Show(GameObject panel)
        {
            mainPanel.SetActive(panel == mainPanel);
            variantPanel.SetActive(panel == variantPanel);
            statsPanel.SetActive(panel == statsPanel);
        }

        private void StartRoulette(RouletteVariant variant)
        {
            PickedVariant = variant;
            SaveSystem.WriteVariant(variant);
            SceneManager.LoadScene(rouletteSceneIndex);
        }

        private void RefreshStats()
        {
            if (statsText == null) return;
            statsText.text = StatsPanelUI.Format(PlayerStats.LoadSnapshot());
        }
    }
}
