using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackGameBoardManager : MonoBehaviour
    {
        [Header("--------------------- Table Info --------------------- ")]
        [SerializeField]
        internal Text minAmountTxt;
        [SerializeField]
        internal Text maxAmountTxt , tableTxt;
        [Header("--------------------- Player --------------------- ")]
        [SerializeField]
        internal List<BlackJackPlayer> players;
        [Header("--------------------- Set Cards --------------------- ")]
        [SerializeField]
        internal BlackJackCardGenerator cardGenerator;

        internal void LoadNewLobbyData(float minAmount, float maxAmount, string lobbyName)
        {
            minAmountTxt.text = "Min. - " + BlackJackGameManager.instance.SetBalanceFormat(minAmount);
            maxAmountTxt.text = "Max. - " + BlackJackGameManager.instance.SetBalanceFormat(maxAmount);
            tableTxt.text = "Table - " + lobbyName.ToString();
            foreach (var item in players)
            {
                item.SetPlayerInfo();
            }
        }

        internal void NewRoundStart()
        {
            foreach (var item in players)
            {
                item.LoadNewRoundData();
            }
            ResetTable();
        }

        internal void ResetTable()
        {
            cardGenerator.SetRendomCard();
        }

    }
}
