using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackCard : MonoBehaviour
    {
        [Header("--------------------- Card Info --------------------- ")]
        [SerializeField]
        internal GameObject cardObject;
        [SerializeField]
        internal Image cardImage;
        [SerializeField]
        internal List<int> cardValue;
        [SerializeField]
        internal int cardNumber;
        [SerializeField]
        internal bool isACard;
        [SerializeField]
        internal Transform resetParent;

        internal void ResetCard()
        {
            cardValue.Clear();
            cardNumber = 0;
            isACard = false;
            gameObject.transform.SetParent(resetParent);
            transform.localPosition = new Vector3(0, 0, 0);
            gameObject.SetActive(false);
        }
    }
}
