using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackSplashScreen : MonoBehaviour
    {
        public Image fillImage;

        public Transform glow;

        private void Update()
        {
            glow.Rotate(Vector3.forward, 10 * Time.deltaTime);
        }

        private void OnEnable()
        {
            LoadingfillAnimation(0.15f);
            if (loadGame != null)
            {
                StopCoroutine(loadGame);
            }
            loadGame = StartCoroutine(LoadingAni());
        }

        Coroutine loadGame;

        float endTime = 1f;
        float startTime = 0f;

        IEnumerator LoadingAni()
        {
            startTime = 0f;
            while (startTime < endTime)
            {
                if (startTime > 0.9f)
                {
                    LoadLoadingOrManu();
                    LoadingfillAnimation(0.98f);
                    yield return null;
                }
                startTime += 0.1f;
                yield return new WaitForSeconds(0.1f);
                float value = 0.48f + (startTime * 0.4f);
                LoadingfillAnimation(value);
            }
        }

        private void LoadLoadingOrManu()
        {
            BlackJackGameManager.instance.ScreenChange("MainManuPanel");
            BlackJackGameManager.instance.popupManager.SetUserPopupActive();
        }

        void LoadingfillAnimation(float fillAmount)
        {
            fillImage.fillAmount = fillAmount;
        }
    }
}