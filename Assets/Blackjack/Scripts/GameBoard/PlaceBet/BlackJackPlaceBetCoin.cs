using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BlackJackOffline
{
    public class BlackJackPlaceBetCoin : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text placeBetAmount;
        [SerializeField]
        internal Tween betAnimation = null;

        internal void SetPlaceBetAmount(float amount)
        {
            placeBetAmount.text = BlackJackGameManager.instance.SetBalanceFormat(amount);
        }
    }
}
