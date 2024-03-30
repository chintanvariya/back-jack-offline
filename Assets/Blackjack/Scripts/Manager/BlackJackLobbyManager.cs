using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackJackOffline
{
    public class BlackJackLobbyManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        struct ManuButtons
        {
            
        }

        public void videoAdsButtonClicked()
        {
            if (BlackJackGameManager.instance.popupManager.InternetCheck())
            {
                BlackJackGoogleAdmobManage.Instance.ShowRewardedAd("10kReward");
            }
        }

        public void StoreClicked()
        {
            BlackJackGameManager.instance.popupManager.CoinStoreShow(""); 
        }

        public GameObject RateAsObject;

        public void ReteAsPopup()
        {
            if (BlackJackGameManager.instance.popupManager.InternetCheck())
            {
                RateAsObject.SetActive(true);
            }
        }

        public GameObject OfferPopupObject;

        public void OfferPopup()
        {
            if (BlackJackGameManager.instance.popupManager.InternetCheck())
            {
                OfferPopupObject.SetActive(true);
            }
        }
    }
}
