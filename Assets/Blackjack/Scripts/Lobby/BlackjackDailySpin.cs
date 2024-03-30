using BlackJackOffline;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackDailySpin : MonoBehaviour
{
    List<string> weekDay = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    string day, dataTime;
    [SerializeField]
    private Button spinButton;
    [SerializeField]
    private Button adSpinButton , homeSpinButton , closeButton;
    [SerializeField]
    private List<float> spinPostion;
    [SerializeField]
    private List<float> spinAmount;
    [SerializeField]
    private GameObject spinWheel , spinPopup;
    [SerializeField]
    private Text userCreditPointTxt;
    [SerializeField]
    private Text rewardAmountTxt;
    [SerializeField]
    private Button tabToCollectButton;
    [SerializeField]
    private GameObject rewardPanel;
    [SerializeField]
    private List<GameObject> rewardCoins;
    [SerializeField]
    private GameObject endPoint;

    // Start is called before the first frame update
    void Start()
    {
        homeSpinButton.onClick.AddListener(SetDailySpin);
        adSpinButton.onClick.AddListener(AdsSpin);
        spinButton.onClick.AddListener(SpinWheelAnimation);
        tabToCollectButton.onClick.AddListener(TapToCollectButtonClicked);
    }

    internal void SpinWheelAnimation()
    {
        int CurrentDayIndex = weekDay.FindIndex(x => x == day);
        CurrentDayIndex++;
        spinButton.interactable = false;
        closeButton.interactable = false;

        string prefName = "DaySpin" + CurrentDayIndex.ToString();
        PlayerPrefs.SetString(prefName, "SpieComplete");

        int RandomNumber = UnityEngine.Random.Range(0, 101);
        int SpinStopValue = 0;
        Debug.Log("RandomNumber => " + RandomNumber);

        if (RandomNumber <= 30)
        {
            SpinStopValue = 0;
        }
        else if (RandomNumber <= 60)
        {
            SpinStopValue = UnityEngine.Random.Range(1, 4);
        }
        else if(RandomNumber <= 80)
        {
            SpinStopValue = UnityEngine.Random.Range(4, 6);
        }
        else if (RandomNumber <= 90)
        {
            SpinStopValue = 6;
        }
        else if (RandomNumber <= 100)
        {
            SpinStopValue = 7;
        }

        Vector3 endPoint = new Vector3(0, 0, 2520 + spinPostion[SpinStopValue]);
        spinWheel.transform.DOLocalRotate(endPoint, 6f, RotateMode.FastBeyond360).OnComplete(() => {
            if (SpinStopValue != 0)
            {
                RewardPanelShow(spinAmount[SpinStopValue]);
            }
            closeButton.interactable = true;
            SetAllDayData();
        });
    }

    private void AdsSpin()
    {
        if (BlackJackGameManager.instance.popupManager.InternetCheck())
        {
            BlackJackGoogleAdmobManage.Instance.ShowRewardedAd("SpinReward");
        }
    }


    internal void SetDailySpin()
    {
        userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
        dataTime = GetCurrentTime().ToString();
        day = GetCurrentDay();
        ResetDay();
        SetAllDayData();
        spinPopup.SetActive(true);
    }

    void ResetDay()
    {
        int CurrentDayIndex = weekDay.FindIndex(x => x == day);

        if (CurrentDayIndex == 0)
        {
            if (PlayerPrefs.GetString("ResetSpinDay") != "ResetDone")
            {
                PlayerPrefs.SetString("ResetSpinDay", "ResetDone");

                int dayCount = 1;
                foreach (var item in weekDay)
                {
                    string prefName = "DaySpin" + dayCount.ToString();
                    PlayerPrefs.SetString(prefName, "");
                }
            }
        }
        else
        {
            if (CurrentDayIndex == 6)
            {
                PlayerPrefs.SetString("ResetSpinDay", "");
            }
        }
    }

    void SetAllDayData()
    {
        int CurrentDayIndex = weekDay.FindIndex(x => x == day);
        CurrentDayIndex++;
        Debug.Log("CurrentDayIndex => " + CurrentDayIndex);
        int dayCount = 1;
        foreach (var item in weekDay)
        {
            string prefName = "DaySpin" + dayCount.ToString();
            if (dayCount < CurrentDayIndex)
            {
                if (PlayerPrefs.GetString(prefName, "") != "SpieComplete")
                {
                    PlayerPrefs.SetString(prefName, "SpieComplete");
                }
            }
            else if (CurrentDayIndex == dayCount)
            {
                if (PlayerPrefs.GetString(prefName, "") == "SpieComplete")
                {
                    spinButton.interactable = false;
                    adSpinButton.gameObject.SetActive(true);
                    //FindAdsAvalible();
                }
                else
                {
                    spinButton.interactable = true;
                }
            }
            dayCount++;
        }
    }

    /*private void FindAdsAvalible()
    {
        if (AdsLoad != null)
        {
            StopCoroutine(AdsLoad);
        }
        AdsLoad = StartCoroutine(WaitForAdsLoad());
    }

    Coroutine AdsLoad;

    IEnumerator WaitForAdsLoad()
    {
       yield return new WaitUntil(() => BlackJackGoogleAdmobManage.Instance.isRewardedLoaded());
        Debug.Log("ads load");
        adSpinButton.gameObject.SetActive(true);
    }*/

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
