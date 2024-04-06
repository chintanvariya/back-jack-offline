using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackGoogleAdmobManage : MonoBehaviour
    {
        public static BlackJackGoogleAdmobManage Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                BlackJackGameManager.instance.ScreenChange("Splash");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            tabToCollectButton.onClick.AddListener(TapToCollectButtonClicked);
            InitializeAds();
        }


        internal void InitializeAds()
        {
            Debug.Log("Initializing Admob ..........");
            MobileAds.Initialize(initStatus =>
            {
                StartGameLoad();
            });
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
        }

        internal void StartGameLoad()
        {
            //TODO: Request Interstitial And Request Rewarded
            LoadInterstitialAds();
            LoadRewaredAds();
            //LoadBannerAds();
        }

        #region Banner Ads

        internal string isBannerShow = "";
        private BannerView bannerAds;
        void RequestBanner()
        {
#if UNITY_ANDROID && UNITY_EDITOR//TODO:Add proper IDs
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "";
#else
        string adUnitId = "";
#endif
            if (bannerAds != null)
            {
                //DistoryBanner();
            }
            // AdSize adSize = new AdSize(320, 45);
            this.bannerAds = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        }

        //internal void BannerLoadAd()
        //{
        //    // create an instance of a banner view first.
        //    if (bannerAds == null)
        //    {
        //        LoadBannerAds();
        //        return;
        //    }
        //    // create our request used to load the ad.
        //    var adRequest = new AdRequest();

        //    this.bannerAds.LoadAd(adRequest);

        //    bannerAds.OnAdFullScreenContentOpened += () =>
        //    {
        //        isBannerShow = "Show";
        //        if (BlackJackGameManager.instance.currentScreen != "MainManuPanel")
        //        {
        //            DistoryBanner();
        //        }
        //        Debug.Log("Banner view full screen content opened.");
        //    };
        //    // Raised when the ad closed full screen content.
        //    bannerAds.OnAdFullScreenContentClosed += () =>
        //    {
        //        isBannerShow = "Failed";
        //        LoadBannerAds();
        //        Debug.Log("Banner view full screen content closed.");
        //    };
        //}

        //internal void DistoryBanner()
        //{
        //    if (bannerAds != null)
        //    {
        //        Debug.Log("Destroying banner ad.");
        //        bannerAds.Destroy();
        //        bannerAds = null;
        //        if (bannerAds == null)
        //        {
        //            LoadBannerAds();
        //        }
        //    }
        //}

        #endregion

        #region  Interstitial Ads

        private InterstitialAd interstitial;

        private void RequestInterstitial()
        {
#if UNITY_ANDROID  &&  !UNITY_EDITOR
            string adUnitId = "ca-app-pub-5918737477932362/3438833279";
#elif UNITY_IPHONE
        string adUnitId = "";
#elif  UNITY_EDITOR
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#endif

            // Clean up the old ad before loading a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            InterstitialAd.Load(adUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    interstitial = ad;
                    RegisterReloadHandler(ad);
                });
        }
        private void RegisterReloadHandler(InterstitialAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial Ad full screen content closed.");
                LoadInterstitialAds();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);
                // Reload the ad so that we can show another as soon as possible.
                LoadInterstitialAds();
                isInterstitialShow = "Failed";
            };
        }

        internal string isInterstitialShow = "";
        internal void ShoWInterstitialAds()
        {
            if (interstitial != null && interstitial.CanShowAd())
            {
                this.interstitial.Show();
                isInterstitialShow = "Show";
            }
            else
            {
                LoadInterstitialAds();
                isInterstitialShow = "Failed";
            }
        }


        #endregion

        #region Rewarded Ads

        internal RewardedAd rewardedAd;
        void RequestRewarded()
        {
            string adUnitId;
#if UNITY_ANDROID && !UNITY_EDITOR
        adUnitId = "ca-app-pub-5918737477932362/5873424927";
#elif UNITY_IPHONE
        adUnitId = "";
#else
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
#endif
            // Clean up the old ad before loading a new one.
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            RewardedAd.Load(adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    rewardedAd = ad;

                    // Raised when the ad closed full screen content.
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        Debug.Log("Rewarded Ad full screen content closed.");
                        // Reload the ad so that we can show another as soon as possible.
                        LoadRewaredAds();
                    };
                    // Raised when the ad failed to open full screen content.
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        Debug.LogError("Rewarded ad failed to open full screen content " +
                                       "with error : " + error);

                        // Reload the ad so that we can show another as soon as possible.
                        LoadRewaredAds();
                    };
                });
        }

        [SerializeField]
        private string actionType = "";
        internal string isRewardedShow = "";
        internal void ShowRewardedAd(string typeOfAds)
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                actionType = typeOfAds;
                rewardedAd.Show((Reward reward) =>
                {
                    // TODO: Reward the user.
                    UserEarnedReward(actionType);
                });
                isRewardedShow = "Show";
            }
            else
            {
                LoadRewaredAds();
                isRewardedShow = "Failed";
            }
        }

        private void UserEarnedReward(string actionType)
        {
            switch (actionType)
            {
                case "SpinReward":
                    BlackJackGameManager.instance.dailySpin.SpinWheelAnimation();
                    break;
                case "10kReward":
                    RewardPanelShow(10000);
                    break;
            }
        }

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


        internal void RewardPanelShow(float Amount)
        {
            userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
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
            BlackJackDataManager.creditPoint += 10000;
            userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
            rewardPanel.SetActive(false);
            BlackJackGameManager.instance.UpdateUserInfo();
        }

        internal bool isRewardedLoaded()
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                return true;
            }
            return false;
        }
        #endregion


        #region Wait For 10 Sec

        internal void LoadBannerAds()
        {
            if (banner != null)
            {
                StopCoroutine(banner);
            }
            banner = StartCoroutine(BannerLoder());
        }

        internal void LoadInterstitialAds()
        {
            if (inter != null)
            {
                StopCoroutine(inter);
            }
            inter = StartCoroutine(InterstitialLoder());
        }

        internal void LoadRewaredAds()
        {
            if (reward != null)
            {
                StopCoroutine(reward);
            }
            reward = StartCoroutine(RewaredLoder());
        }

        Coroutine banner, inter, reward;

        int StartingR = 1;
        int StartingI = 1;
        int StartingB = 1;

        IEnumerator BannerLoder()
        {
            yield return new WaitForSecondsRealtime(StartingB);
            RequestBanner();
            StartingB = 1;
        }

        IEnumerator InterstitialLoder()
        {
            yield return new WaitForSecondsRealtime(StartingI);
            RequestInterstitial();
            StartingI = 1;
        }

        IEnumerator RewaredLoder()
        {
            yield return new WaitForSecondsRealtime(StartingR);
            RequestRewarded();
            StartingR = 1;
        }

        #endregion

    }
}