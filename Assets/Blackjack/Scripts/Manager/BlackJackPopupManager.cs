using BlackJackOffline;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackJackPopupManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameLeaveButton.onClick.AddListener(() =>
        {
            SetLeavePopupData("LeaveTable");
        });
        exitButton.onClick.AddListener(() =>
        {
            SetLeavePopupData("LeaveGame");
        });
        yesButton.onClick.AddListener(YesButtonClicked);
        noButton.onClick.AddListener(NoButtonClicked);
        Application.targetFrameRate = 60;
    }

    #region Leave Popup
    [Header("----------- Leave Popup ----------------")]
    [SerializeField]
    private GameObject popupObject;
    [SerializeField]
    private Text headerTxt, descriptionTxt;
    [SerializeField]
    private Button yesButton, noButton, gameLeaveButton, exitButton;
    [SerializeField]
    private Text yesButtonTxt, noButtonTxt;
    [SerializeField]
    private GameObject mainManu;
    string type;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainManu.activeInHierarchy)
            {
                SetLeavePopupData("LeaveGame");
            }
            else
            {
                SetLeavePopupData("LeaveTable");
            }
        }
    }

    internal void SetLeavePopupData(string popuptype)
    {
        type = popuptype;
        yesButtonTxt.text = "Yes";
        noButtonTxt.text = "No";
        SettingPopupObject.SetActive(false);
        switch (type)
        {
            case "LeaveGame":
                headerTxt.text = "LEAVE GAME";
                descriptionTxt.text = "Are you sure you want to leave?";
                break;
            case "LeaveTable":
                headerTxt.text = "LEAVE TABLE";
                descriptionTxt.text = "Do you want to leave this table?";
                break;
            case "GamePause":
                headerTxt.text = "PAUSE";
                descriptionTxt.text = "Do you want to play this table?";
                yesButtonTxt.text = "Resume";
                noButtonTxt.text = "Exit";
                break;
        }
        Time.timeScale = 0;
        popupObject.SetActive(true);
    }

    void YesButtonClicked()
    {
        Time.timeScale = 1;
        switch (type)
        {
            case "LeaveGame":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
#if PLATFORM_ANDROID
                Application.Quit();
#endif
                break;
            case "LeaveTable":
                LeaveTable();
                break;
        }
        popupObject.SetActive(false);
    }

    void NoButtonClicked()
    {
        Time.timeScale = 1;
        switch (type)
        {
            case "GamePause":
                LeaveTable();
                break;
        }
        popupObject.SetActive(false);
    }

    internal void LeaveTable()
    {
        Debug.Log("LeaveTable");
        blackScreen.SetActive(true);
        StartCoroutine(LeaveTableWithDelay());
    }

    public GameObject blackScreen;

    IEnumerator LeaveTableWithDelay()
    {
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        BlackJackGameManager.instance.ScreenChange("MainManuPanel");
        LeaveTableStatisticUpdate();
        BlackJackDataManager.lobbyName = "";
        BlackJackDataManager.lobbyMinValue = 0;
        BlackJackDataManager.lobbyMaxValue = 0;
        BlackJackGameManager.instance.betButtons.lastBet = 0;
        //SceneManager.LoadScene(0);
        SettingPopupObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        BlackJackGoogleAdmobManage.Instance.ShoWInterstitialAds();
    }

    private void LeaveTableStatisticUpdate()
    {
        foreach (var player in BlackJackGameManager.instance.boradManager.players)
        {
            if (!player.isBot)
            {
                if (BlackJackDataManager.RoundPlay != BlackJackDataManager.RoundWin + BlackJackDataManager.RoundLoss)
                {
                    BlackJackDataManager.RoundLoss++;
                }
                player.StopAllPlayerCorotine();
            }
        }
        BlackJackGameManager.instance.dealer.StopAllCoroutines();
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause => " + pause);
        if (!pause)
        {
            if (!mainManu.activeInHierarchy && BlackJackGameManager.instance.gameScreens[1].activeInHierarchy)
            {
                //BlackJackGoogleAdmobManage.Instance.ShoWInterstitialAds();
                SetLeavePopupData("GamePause");
            }
        }
        else
        {
            if (!mainManu.activeInHierarchy && BlackJackGameManager.instance.gameScreens[1].activeInHierarchy)
            {
                Time.timeScale = 0;
            }
        }
    }

    private void OnApplicationQuit()
    {
        LeaveTableStatisticUpdate();
    }
    #endregion

    #region User First Time Enter in Game
    [Header("-------- User Profile Popup -----------")]
    [SerializeField]
    private GameObject userProfilePopup;
    [SerializeField]
    private InputField UserName;
    [SerializeField]
    private Button OkayButton;

    public void SetInputFileValueChange()
    {
        if (UserName.text.Length > 0)
        {
            UserName.text = UserName.text.Replace(" ", "");
            if (UserName.text.Length > 0)
            {
                OkayButton.interactable = true;
            }
        }
        else
        {
            OkayButton.interactable = false;
        }
    }

    public void SetUserPopupActive()
    {
        UserName.text = "";
        if (BlackJackDataManager.userName == "")
        {
            userProfilePopup.SetActive(true);
        }
    }

    public void UserPopupOkayButtonClicked()
    {
        BlackJackDataManager.userName = UserName.text;
        BlackJackGameManager.instance.UpdateUserInfo();
        userProfilePopup.SetActive(false);
    }

    #endregion

    #region Alert Popup
    [Header("-------- Alert Popup -----------")]
    [SerializeField]
    private GameObject alertPopupObject;
    [SerializeField]
    private Text discriptionTxt;
    [SerializeField]
    private Button alertOkButton;
    [SerializeField]
    private Text ToastDiscriptionTxt;
    [SerializeField]
    private GameObject ToastObj;

    internal void SetAlertPopup(string Action)
    {
        SettingPopupObject.SetActive(false);
        alertOkButton.onClick.RemoveAllListeners();
        alertOkButton.onClick.AddListener(() =>
        {
            AlertOkClicked(Action);
        });
        switch (Action)
        {
            case "Insufficient Balance":
                discriptionTxt.text = "Insufficient balance, buy coin from store.";
                alertPopupObject.SetActive(true);
                break;
            case "PurchaseSuccess":
                discriptionTxt.text = "Your purchase was successful.";
                alertPopupObject.SetActive(true);
                if (BlackJackDataManager.creditPoint >= 1000)
                {
                    coinPopupObject.gameObject.SetActive(false);
                }
                break;
            case "PurchaseFailed":
                discriptionTxt.text = "your purchase was Failed.";
                alertPopupObject.SetActive(true);
                if (BlackJackDataManager.creditPoint >= 1000)
                {
                    coinPopupObject.gameObject.SetActive(false);
                }
                break;
        }
    }

    void AlertOkClicked(string Action)
    {
        switch (Action)
        {
            case "Insufficient Balance":
                CoinStoreShow("Insufficient Balance");
                break;
        }
        alertPopupObject.SetActive(false);
    }

    internal void SetToastAlertPopup(string message)
    {
        ToastDiscriptionTxt.text = message;
        ShowToastMessage();
    }
    private void ShowToastMessage()
    {
        ToastObj.SetActive(false);
        if (toastShow != null)
        {
            StopCoroutine(toastShow);
        }
        toastShow = StartCoroutine(ToastShow());
    }

    Coroutine toastShow;

    IEnumerator ToastShow()
    {
        ToastObj.SetActive(true);
        yield return new WaitForSeconds(2);
        ToastObj.SetActive(false);
    }
    #endregion

    #region Coin Store Popup
    [Header("-------- Coin Store Popup -----------")]
    [SerializeField]
    public GameObject coinPopupObject;
    [SerializeField]
    private Button coinStoreCloseButton;

    internal void CoinStoreShow(string Action)
    {
        if (InternetCheck())
        {
            coinStoreCloseButton.onClick.RemoveAllListeners();
            coinStoreCloseButton.onClick.AddListener(() =>
            {
                CloseCoinStore(Action);
            });
            coinPopupObject.gameObject.SetActive(true);
            BlackjackInAppPurchasing.Instance.OpenScreen();
            SettingPopupObject.SetActive(false);
        }
        else
        {
            if (Action == "Insufficient Balance")
            {
                LeaveTable();
            }
        }
    }

    void CloseCoinStore(string Action)
    {
        switch (Action)
        {
            case "Insufficient Balance":
                if (BlackJackDataManager.creditPoint <= 1000)
                {
                    LeaveTable();
                }
                break;
        }
        coinPopupObject.gameObject.SetActive(false);
    }

    #endregion


    #region Alert Popup
    [Header("-------- No Internet Popup -----------")]
    [SerializeField]
    private GameObject noInternetPopupObject;

    internal bool InternetCheck()
    {
        if (!IsInternetConnection())
        {
            noInternetPopupObject.SetActive(true);
            SettingPopupObject.SetActive(false);
            return false;
        }
        return true;
    }

    bool IsInternetConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    #endregion

    [Header("-------- Setting Popup -----------")]
    [SerializeField]
    private GameObject SettingPopupObject;


}
