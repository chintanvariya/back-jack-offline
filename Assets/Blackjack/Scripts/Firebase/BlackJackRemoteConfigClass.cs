using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackJackOffline
{
    public class BlackJackRemoteConfigClass
    {
        [Serializable]
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class AdsIds
        {
            public string blackJackAppOpen;
            public string blackJackBanner;
            public string blackJackInterstitial;
            public string blackJackReward;
            public string blackJackRewardedInterstitial;
        }

        [Serializable]
        public class BlackJackRemoteConfig
        {
            public FlagDetails flagDetails;
            public AdsDetails adsDetails;
            public LevelDetails levelDetails;
        }
        [Serializable]
        public class LevelDetails
        {
            public List<int> coinsToClearLevel = new List<int>();
            public List<int> allLobbyAmount = new List<int>();
        }
        [Serializable]
        public class FlagDetails
        {
            public bool isAds;
            public bool isSuccess;
            public bool isForceUpdate;
        }
        [Serializable]
        public class AdsDetails
        {
            public bool isShowInterstitialAdsOnLobby;
            public bool isShowInterstitialAdsOnScoreBoard;
            public int numberOfAds;
            public AdsIds androidAdsIds;
            public AdsIds iosAdsIds;
        }
    }
}
