using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackjackDailyRewardManager : MonoBehaviour
    {
        [SerializeField]
        private List<BlackjackDay> days;
        [SerializeField]
        private List<Sprite> dayCoinSprites , claimButtonSprites;
        List<string> weekDay = new List<string>{"Monday", "Tuesday", "Wednesday", "Thursday", "Friday" , "Saturday" , "Sunday"};
        string day, dataTime;

        [SerializeField]
        private Text userCreditPointTxt;
        [SerializeField]
        private Text rewardAmountTxt;
        [SerializeField]
        private Button tabToCollectButton, homeRewardButton;
        [SerializeField]
        private GameObject rewardPanel;
        [SerializeField]
        private List<GameObject> rewardCoins;

        void Start()
        {
            homeRewardButton.onClick.AddListener(SetDailyRewareds);
            tabToCollectButton.onClick.AddListener(TapToCollectButtonClicked);
        }

        internal void SetDailyRewareds()
        {
            dataTime = GetCurrentTime().ToString();
            day = GetCurrentDay();
            ResetDay();
            SetAllDayData(); 
            userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
        }

        void ResetDay()
        {
            int CurrentDayIndex = weekDay.FindIndex(x => x == day);
            if (CurrentDayIndex == 0)
            {
                if (PlayerPrefs.GetString("ResetDay") != "ResetDone")
                {
                    PlayerPrefs.SetString("ResetDay", "ResetDone");

                    int dayCount = 1;
                    foreach (var item in days)
                    {
                        string prefName = "Day" + dayCount.ToString();
                        PlayerPrefs.SetString(prefName, "");
                    }
                }
            }
            else
            {
                if (CurrentDayIndex == 6)
                {
                    PlayerPrefs.SetString("ResetDay", "");
                }
            }
        }


        void SetAllDayData()
        {
            int CurrentDayIndex = weekDay.FindIndex(x => x == day);
            CurrentDayIndex++;
            Debug.Log("CurrentDayIndex => " + CurrentDayIndex);
            int dayCount = 1;
            foreach (var item in days)
            {
                item.dayCoinImage.sprite = dayCoinSprites[dayCount - 1];
                item.dayAmountTxt.text = (1000 * dayCount).ToString();
                item.dayCountTxt.text = "Day " + dayCount.ToString();
                item.dayCount = dayCount;
                item.rewardManager = this;
                string prefName = "Day" + dayCount.ToString();
                if (dayCount < CurrentDayIndex) 
                {
                    if (PlayerPrefs.GetString(prefName, "") == "Claimed")
                    {
                        SetDayStatus("Claimed", item);
                        Debug.Log(item.name.ToString() + " Claimed");
                    }
                    else
                    {
                        SetDayStatus("Skip", item);
                        Debug.Log(item.name.ToString() + " Skip");
                    }
                }
                else if (CurrentDayIndex == dayCount)
                {
                    if (PlayerPrefs.GetString(prefName, "") == "Claimed")
                    {
                        SetDayStatus("Claimed", item);
                        Debug.Log(item.name.ToString() + " Claimed");
                    }
                    else
                    {
                        SetDayStatus("Claim", item);
                        Debug.Log(item.name.ToString() + " Claim");
                    }
                }
                else
                {
                    SetDayStatus("NextDay" , item);
                    Debug.Log(item.name.ToString() + " NextDay");
                }
                dayCount++;
            }
        }

        void SetDayStatus(string status, BlackjackDay day)
        {
            switch(status)
            {
                case "Skip":
                    SkipDayReward(day);
                    break;
                case "Claimed":
                    ClaimedDayReward(day);
                    break;
                case "Claim":
                    ClaimDayReward(day);
                    break;
                case "NextDay":
                    NextDayReward(day);
                    break;
            }
        }

        void ClaimedDayReward(BlackjackDay day)
        {
            day.dayClaimSelect.gameObject.SetActive(true);
            day.claimButtonImage.sprite = claimButtonSprites[0];
            day.claimButton.interactable = false;
            day.buttonTxt.text = "Claimed!";
        }

        void ClaimDayReward(BlackjackDay day)
        {
            day.claimDayImage.gameObject.SetActive(true);
            day.claimButtonImage.sprite = claimButtonSprites[0];
            day.claimButton.interactable = true;
            day.buttonTxt.text = "Claim!";
        }

        void NextDayReward(BlackjackDay day)
        {
            day.closeDay.gameObject.SetActive(true);
            day.claimButtonImage.sprite = claimButtonSprites[1];
            day.claimButton.interactable = false;
            day.buttonTxt.text = "Claim!";
        }

        void SkipDayReward(BlackjackDay day)
        {
            day.closeDay.gameObject.SetActive(true);
            day.dayClaimSelect.gameObject.SetActive(true);
            day.claimButtonImage.sprite = claimButtonSprites[1];
            day.claimButton.interactable = false;
            day.buttonTxt.text = "Skip!";
        }

        private DateTime GetCurrentTime()
        {
            return DateTime.UtcNow.AddHours(5).AddMinutes(30);
        }

        private String GetCurrentDay()
        {
            return DateTime.UtcNow.AddHours(5).AddMinutes(30).DayOfWeek.ToString();
        }



        internal void RewardPanelShow(float Amount)
        {
            foreach (var item in rewardCoins)
            {
                item.SetActive(false);
            }
            rewardAmountTxt.text = Amount.ToString();
            tabToCollectButton.interactable = true;
            rewardPanel.SetActive(true);
        }

        Coroutine tabAnimation;
        internal void TapToCollectButtonClicked()
        {
            tabToCollectButton.interactable = false;
            if (tabAnimation != null)
            {
                StopCoroutine(tabAnimation);
            }
            tabAnimation = StartCoroutine(CoinAnimation());
        }

        IEnumerator CoinAnimation()
        {
            foreach (var item in rewardCoins)
            {
                item.SetActive(true);
                BlackJackSettingManager.instance.PlaySound("Chips");
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(1.25f);
            BlackJackDataManager.creditPoint += float.Parse(rewardAmountTxt.text.ToString());
            userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
            rewardPanel.SetActive(false);
            BlackJackGameManager.instance.UpdateUserInfo();
        }
    }
}
