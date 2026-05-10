using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private float smoothTime = 5f;
        [SerializeField] private int nextSceneIndex = 1;

        private void Start()
        {
            if (progressBar != null) progressBar.value = 0;
            SaveSystem.Load();
            StartCoroutine(BootSequence());
        }

        private IEnumerator BootSequence()
        {
            float currentDisplay = 0f;
            var op = SceneManager.LoadSceneAsync(nextSceneIndex);
            op.allowSceneActivation = false;

            while (currentDisplay < 1f)
            {
                float normalised = Mathf.Clamp01(op.progress / 0.9f);
                float target = Mathf.Lerp(0.2f, 1f, normalised);
                currentDisplay = Mathf.MoveTowards(currentDisplay, target, Time.deltaTime * smoothTime);
                if (progressBar != null) progressBar.value = currentDisplay;

                if (op.progress >= 0.9f && Mathf.Approximately(currentDisplay, 1f)) break;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            op.allowSceneActivation = true;
        }
    }
}
