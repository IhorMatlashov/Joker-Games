using Audio;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(SaveSystem.MusicVolume);
                musicSlider.onValueChanged.AddListener(OnMusicChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(SaveSystem.SfxVolume);
                sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            }
            if (openButton != null)  openButton.onClick.AddListener(() => Show(true));
            if (closeButton != null) closeButton.onClick.AddListener(() => Show(false));

            Show(false);
        }

        private void Show(bool visible)
        {
            if (panel != null) panel.SetActive(visible);
        }

        private void OnMusicChanged(float v)
        {
            SaveSystem.WriteVolumes(v, SaveSystem.SfxVolume);
            if (AudioManager.Instance != null) AudioManager.Instance.ApplySavedVolumes();
        }

        private void OnSfxChanged(float v)
        {
            SaveSystem.WriteVolumes(SaveSystem.MusicVolume, v);
            if (AudioManager.Instance != null) AudioManager.Instance.ApplySavedVolumes();
        }
    }
}
