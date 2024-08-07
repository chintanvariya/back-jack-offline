using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackSettingManager : MonoBehaviour
    {
        #region UNITY
        public static BlackJackSettingManager instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        [SerializeField]
        private GameObject HowToPlayLobby, HowToPlayGamePlay , SettingPanel , privacy ,Term;

        internal void OpenSettingPanel(bool isLobby)
        {
            HowToPlayLobby.SetActive(isLobby);
            HowToPlayGamePlay.SetActive(!isLobby);
            privacy.SetActive(isLobby);
            Term.SetActive(isLobby);
            SettingPanel.SetActive(true);
        }

        void Start()
        {
            settingButton[0].SetDefault();
            settingButton[1].SetDefault();
            settingButton[2].SetDefault();
        }

        public void privacyAndTermButton(bool isPrivacy)
        {
            if (isPrivacy)
            {
                Application.OpenURL("https://finixgamesstudio.com/privacy-policy/");
            } 
            else
            {
                Application.OpenURL("https://finixgamesstudio.com/privacy-policy/");
            }
        }

        #endregion

        #region Sound Controller
        [Header("----------- Sound And Music --------------")]
        [SerializeField]
        private List<SettingButton> settingButton;
        public AudioSource audioSource;

        public void PlaySettingButtonSound()
        {
            audioSource.Play();
        }

        public void PlaySound(string SoundName)
        {
            settingButton[0].PlaySound(SoundName);
        }

        internal void StopSound(string SoundName)
        {
            settingButton[0].StopSound(SoundName);
        }

        public void ClickSound()
        {
            PlaySound("Click");
        }

        #endregion

        #region Viration Effect

        internal void PlayVirationEffect()
        {
            settingButton[2].playVibration(100);
        }

        #endregion

        #region Button Manager

        [Serializable]
        public struct SettingButton
        {
            public string buttonName;
            public Image buttonImage , iconImage;
            public Button clickButton;
            public Sprite[] sprites , iconSprites;
            public List<AudioSource> audioSource;

            void ButtonClicked()
            {
                Debug.Log("Button Clicked :: " + buttonName);
                string value = findCurrentValue();
                Debug.Log("Button Clicked :: " + buttonName + " Mode Change to : " + value);
                PlayerPrefs.SetString(buttonName, value);
                SetSprite();
            }

            string findCurrentValue()
            {
                if (PlayerPrefs.GetString(buttonName, "On") == "On")
                {
                    return "Off";
                }
                return "On";
            }

            void SetSprite()
            {
                if (PlayerPrefs.GetString(buttonName, "On") == "On")
                {
                    buttonImage.sprite = sprites[0];
                    iconImage.sprite = iconSprites[0];
                    if (audioSource.Count != 0)
                    {
                        foreach (var item in audioSource)
                        {
                            item.enabled = true;
                        }
                    }
                    if (buttonName == "MusicEffect")
                    {
                        audioSource[0].Play();
                    }
                }
                else if (PlayerPrefs.GetString(buttonName, "On") == "Off")
                {
                    buttonImage.sprite = sprites[1];
                    iconImage.sprite = iconSprites[1];
                    if (audioSource.Count != 0)
                    {
                        foreach (var item in audioSource)
                        {
                            if (item.gameObject.name != "SettingButton")
                            {
                                item.Stop();
                                item.enabled = false;
                            }

                        }
                    }
                }
            }

            public void SetDefault()
            {
                Debug.Log("Set Default :: " + buttonName);
                SetSprite();
                clickButton.onClick.RemoveAllListeners();
                clickButton.onClick.AddListener(ButtonClicked);

            }

            public void PlaySound(string clipName)
            {
                if (PlayerPrefs.GetString(buttonName, "On") == "On")
                {
                    if (clipName != "")
                    {
                        Debug.Log("Play Sound : " + clipName);
                        int index = audioSource.FindIndex(a => a.gameObject.name == clipName);
                        Debug.Log("index : " + index);
                        audioSource[index].Play();
                    }
                    else if (buttonName == "MusicEffect")
                    {
                        audioSource[0].Play();
                    }
                }
            }

            public void StopSound(string clipName)
            {
                if (PlayerPrefs.GetString(buttonName, "On") == "On")
                {
                    if (clipName != "")
                    {
                        Debug.Log("Play Sound : " + clipName);
                        int index = audioSource.FindIndex(a => a.gameObject.name == clipName);
                        audioSource[index].Stop();
                    }
                    else if (buttonName == "MusicEffect")
                    {
                        audioSource[0].Stop();
                    }
                }
            }

            public void playVibration(long Amount)
            {
                if (PlayerPrefs.GetString(buttonName, "On") == "On")
                {
                    Debug.Log("Play Vibration");
                    BlackJackVibration.Vibrate(Amount);
                }
            }
        }

        #endregion
    }
}
