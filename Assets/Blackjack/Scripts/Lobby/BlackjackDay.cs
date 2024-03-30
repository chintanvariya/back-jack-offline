using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackjackDay : MonoBehaviour
    {
        [SerializeField]
        internal Text dayCountTxt;
        [SerializeField]
        internal Image dayCoinImage , dayClaimSelect;
        [SerializeField]
        internal Text dayAmountTxt , buttonTxt;
        [SerializeField]
        internal Image claimButtonImage,claimDayImage, closeDay;
        [SerializeField]
        internal Button claimButton; 
        [SerializeField]
        internal int dayCount;
        internal BlackjackDailyRewardManager rewardManager;

        // Start is called before the first frame update
        void Start()
        {
            claimButton.onClick.AddListener(ClaimButtonClicked);
        }

        private void ClaimButtonClicked()
        {
            string prefName = "Day" + dayCount.ToString();
            PlayerPrefs.SetString(prefName, "Claimed");
            rewardManager.SetDailyRewareds();
            rewardManager.RewardPanelShow(float.Parse(dayAmountTxt.text));
        }
    }
}
