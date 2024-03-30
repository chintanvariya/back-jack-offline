using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackDealer : MonoBehaviour
    {
        [Header("---------------------- Dealer Info --------------------")]
        [SerializeField]
        internal int totalPlayerRoundPlacebet;
        [SerializeField]
        internal Transform fullDeck, EmptyDeck;
        [SerializeField]
        private Transform dealerCardArea;
        [SerializeField]
        private List<BlackJackCard> dealerCardList = new List<BlackJackCard>();
        [SerializeField]
        internal List<int> scoreValue;
        [SerializeField]
        private Text dealerCardTxt;
        [SerializeField]
        private GameObject dealerCardObject;
        [SerializeField]
        BlackJackPlayer player;
        [Header("---------------------- Top Area Info --------------------")]
        [SerializeField]
        internal Transform dealerChipArea;

        int cValueCounter;
        Coroutine startNewRoundCoroutine , throwNewRoundCorotine , addNewCard , loadExtraCards , winnner;
        BlackJackGameManager gameManager;
        BlackJackGameBoardManager boradManager;
        List<BlackJackPlayer> players;
        BlackJackCardGenerator cardGenerator;
        BlackJackEmptyCard dealerEmptyCard;

        private void Start()
        {
            gameManager = BlackJackGameManager.instance;
            boradManager = gameManager.boradManager;
            players = boradManager.players;
            cardGenerator = boradManager.cardGenerator;
        }

        #region New Round Start

        internal void ResetDealerDeatalis()
        {
            foreach (var item in dealerCardList)
            {
                item.ResetCard();
            }
            dealerCardList.Clear();
            insuranceImage.sprite = insuranceSprites[0];
            totalInsurancePlaced = 0;
            totalInsuranceBet = 0;
            totalPlayerRoundPlacebet = 0;
            dealerCardObject.SetActive(false);
        }

        internal void StartNewRound(bool isNextRound)
        {
            if (startNewRoundCoroutine != null)
            {
                StopCoroutine(startNewRoundCoroutine);
            }
            startNewRoundCoroutine = StartCoroutine(StartRound(isNextRound));
        }

        IEnumerator StartRound(bool isNextRound)
        {
            if (!isNextRound)
            {
                boradManager.LoadNewLobbyData(BlackJackDataManager.lobbyMinValue, BlackJackDataManager.lobbyMaxValue, BlackJackDataManager.lobbyName);
            }
            else
            {
                if (BlackJackDataManager.creditPoint < BlackJackDataManager.lobbyMinValue)
                {
                    int newLobbyIndex = 100;
                    newLobbyIndex = gameManager.blackJackLobbies.FindIndex(x => x.minAmount <= BlackJackDataManager.creditPoint);
                    Debug.Log(" New Lobby Index ==> " + newLobbyIndex);
                    if (newLobbyIndex  == -1)
                    {
                        gameManager.popupManager.SetAlertPopup("Insufficient Balance");
                        yield return new WaitUntil(() => 1000 <= BlackJackDataManager.creditPoint); 
                        var item = gameManager.blackJackLobbies[4];
                        BlackJackDataManager.lobbyName = item.lobbyName;
                        BlackJackDataManager.lobbyMinValue = item.minAmount;
                        BlackJackDataManager.lobbyMaxValue = item.maxAmount;
                        BlackJackDataManager.lobbyIndex = item.lobbyIndex;
                        boradManager.LoadNewLobbyData(BlackJackDataManager.lobbyMinValue, BlackJackDataManager.lobbyMaxValue, BlackJackDataManager.lobbyName);
                    }
                    else
                    {
                        var item = gameManager.blackJackLobbies[newLobbyIndex];
                        BlackJackDataManager.lobbyName = item.lobbyName;
                        BlackJackDataManager.lobbyMinValue = item.minAmount;
                        BlackJackDataManager.lobbyMaxValue = item.maxAmount;
                        BlackJackDataManager.lobbyIndex = item.lobbyIndex;
                        boradManager.LoadNewLobbyData(BlackJackDataManager.lobbyMinValue, BlackJackDataManager.lobbyMaxValue, BlackJackDataManager.lobbyName);
                    }
                }
            }
            gameManager.pooler.ResetAtNewRoundStart();
            //Reset Dealer Data
            ResetDealerDeatalis();
            //Start New Round 
            boradManager.NewRoundStart();
            yield return new WaitUntil(() => totalPlayerRoundPlacebet == players.Count);
            //Start Card Animation
            if (throwNewRoundCorotine != null)
            {
                StopCoroutine(throwNewRoundCorotine);
            }
            throwNewRoundCorotine = StartCoroutine(ThrowNewRoundCards());
        }

        private IEnumerator ThrowNewRoundCards()
        {
            Debug.Log("ThrowNewRoundCards ==>");
            for (int i = 0; i < 2; i++)
            {
                // for Player
                foreach (var player in players)
                {
                    foreach (var area in player.placeBetAreas)
                    {
                        if (area.isPlaceBetAreaActive && area.isPlaceBetAreaActive)
                        {
                            BlackJackCard card = FindCurrentCard(area.cardPosition.transform);
                            area.cards.Add(card);
                        }
                    }
                }
                BlackJackCard dealercard = FindCurrentCard(dealerCardArea.transform);
                dealerCardList.Add(dealercard);

            }
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < 2; i++)
            {
                // for Player
                foreach (var player in players)
                {
                    foreach (var area in player.placeBetAreas)
                    {
                        if (area.isPlaceBetAreaActive && area.isPlaceBetAreaActive)
                        {
                            BlackJackCard card = area.cards[i];
                            yield return new WaitForSeconds(0.1f);
                            ThrowAndFlipCard(card , true);
                            if (i == 1)
                            {
                                area.FindPlayerCardValue();
                            }
                        }
                    }
                }


                BlackJackCard dealercard = dealerCardList[i];
                yield return new WaitForSeconds(0.1f);
                if (i == 0)
                {
                    ThrowAndFlipCard(dealercard, true);
                }
                else
                {
                    ThrowAndFlipCard(dealercard, false);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            yield return new WaitForSeconds(0.2f);

            //Find Insurance
            if (FindUserHaveInsurance())
            {
                UserInsurance();
                yield return new WaitForSeconds(0.1f);
                Debug.Log("totalPlaceBet => " + totalPlaceBet);
                Debug.Log("totalInsuranceBet => " + totalInsuranceBet);
                yield return new WaitUntil(() => totalPlaceBet == totalInsuranceBet);
                insuranceImage.sprite = insuranceSprites[0];
            }

            // User Turn Start
            UserTurnStart();
        }

        #region Add New Card
        //Add New Card after User turn Start

        internal void AddNewCard(BlackJackPlaceBetArea area, bool ChangeTurn , Action splitAction = null)
        {
            if (addNewCard != null)
            {
                StopCoroutine(addNewCard);
            }
            addNewCard = StartCoroutine(AddNewCardCoroution(area , ChangeTurn , splitAction));
        }

        IEnumerator AddNewCardCoroution(BlackJackPlaceBetArea area , bool ChangeTurn , Action splitAction)
        {
            Debug.Log("Add New Card ==>");
            BlackJackCard card = FindCurrentCard(area.cardPosition.transform);
            area.cards.Add(card);
            yield return new WaitForSeconds(0.1f);
            ThrowAndFlipCard(card, true);
            yield return new WaitForSeconds(0.3f);
            area.FindPlayerCardValue();
            if (ChangeTurn)
            {
                UserTurnStart();
            }
            else
            {
                if (splitAction != null)
                {
                    splitAction.Invoke();
                }
            }
        }
        #endregion
        
        #endregion

        #region Dealer Card Throw

        private void ThrowAndFlipCard(BlackJackCard card , bool isFilp)
        {
            BlackJackSettingManager.instance.PlaySound("Deal");
            BlackJackEmptyCard emptyCard = gameManager.pooler.SpawnFromEmptyCards("EmptyCard", fullDeck);
            // TODO : add tween here
            if (isFilp)
            {
                emptyCard.emptyImage.sprite = card.cardImage.sprite;
            }
            else
            {
                dealerEmptyCard = emptyCard;
            }
            emptyCard.transform.DOMove(card.transform.position, 0.35f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (isFilp)
                {
                    card.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    emptyCard.gameObject.SetActive(false);
                }
            });
        }

        void CardFlipAnimation(BlackJackEmptyCard emptyCard , BlackJackCard card)
        {
            BlackJackSettingManager.instance.PlaySound("Deal");
            emptyCard.transform.DOScaleX(0, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
            {
                emptyCard.emptyImage.sprite = card.cardImage.sprite;
                emptyCard.transform.DOScaleX(1, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    card.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    emptyCard.gameObject.SetActive(false);
                });
            });
        }

        BlackJackCard FindCurrentCard(Transform cardPosition)
        {
            BlackJackCard card = gameManager.pooler.SpawnFromCards("Card", cardPosition);
            Sprite cardSprite = cardGenerator.randomBoradCard[0];
            cardGenerator.randomBoradCard.RemoveAt(0);
            card.cardImage.sprite = cardSprite;
            card.gameObject.name = cardSprite.name;
            SetCardValueAndNumber(card, cardSprite);
            return card;
        }
        
        void SetCardValueAndNumber(BlackJackCard card , Sprite cardImage)
        {
            string cardName = cardImage.name;
            string[] cardSplitName = cardName.Split('-');
            card.cardNumber = int.Parse(cardSplitName[1]);
            if (int.Parse(cardSplitName[1]) == 1)
            {
                card.cardValue.Add(1);
                card.cardValue.Add(11);
                card.isACard = true;
            }
            else if (int.Parse(cardSplitName[1]) > 10)
            {
                card.cardValue.Add(10);
            }
            else
            {
                card.cardValue.Add(int.Parse(cardSplitName[1]));
            }
        }
        #endregion

        #region User Turn Start

        Coroutine userTurnStartCoroutine;

        internal void UserTurnStart()
        {
            Debug.Log("UserTurnStart ==> ");
            StopUserTurn();
            userTurnStartCoroutine = StartCoroutine(UserTurn());
        }

        internal void StopUserTurn()
        {
            if (userTurnStartCoroutine != null)
            {
                StopCoroutine(userTurnStartCoroutine);
            }
        }

        IEnumerator UserTurn()
        {
            Debug.Log("UserTurn ==> ");
            foreach (var player in boradManager.players)
            {
                foreach (var area in player.placeBetAreas)
                {
                    if (area.isPlaceBetAreaActive && area.isPlayerPlaceBet && !area.isAreaTurnOver)
                    {
                        if (!area.isBust && !area.isBlackJack)
                        {
                            Debug.Log("UserTurn ==> " + area.name);
                            area.SetUserTurn();
                            yield return new WaitUntil(() => area.isAreaTurnOver);
                        }
                    }
                }
            }

            // User Turn Over
            AllUserTurnOver();
        }
        #endregion

        #region User Insurance

        Coroutine userInsuranceCoroutine;
        internal int totalPlaceBet, totalInsuranceBet , totalInsurancePlaced;
        [Header("---------------------- User Insurance --------------------")]
        [SerializeField]
        private Image insuranceImage;
        [SerializeField]
        private List<Sprite> insuranceSprites;

        internal void UserInsurance()
        {
            StopInsurance();
            userInsuranceCoroutine = StartCoroutine(UserInsuranceCall());
        }

        internal void StopInsurance()
        {
            if (userInsuranceCoroutine != null)
            {
                StopCoroutine(userInsuranceCoroutine);
            }
        }

        IEnumerator UserInsuranceCall()
        {
            Debug.Log("UserInsuranceCall ==> ");
            insuranceImage.sprite = insuranceSprites[1];
            totalPlaceBet = 0;
            foreach (var player in boradManager.players)
            {
                foreach (var area in player.placeBetAreas)
                {
                    if (area.isPlaceBetAreaActive && area.isPlayerPlaceBet)
                    {
                        Debug.Log("UserInsuranceCall ==> " + totalPlaceBet);
                        totalPlaceBet++;
                        area.UserInsurance();
                        yield return null;
                    }
                }
            }
        }

        bool FindUserHaveInsurance()
        {
            if (dealerCardList[0].isACard)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region User Turn Over

        private void AllUserTurnOver()
        {
            CardFlipAnimation(dealerEmptyCard, dealerCardList[1]);

            if (loadExtraCards != null)
            {
                StopCoroutine(loadExtraCards);
            }
            loadExtraCards = StartCoroutine(LoadExtraCardsInDealer());
        }

        int areaCount, BustCount ;
        IEnumerator LoadExtraCardsInDealer()
        {
            yield return new WaitForSeconds(1f);
            int maxValue = DealerCardValue(false);

            areaCount = BustCount = 0;
            foreach (var player in boradManager.players)
            {
                foreach (var area in player.placeBetAreas)
                {
                    if (area.isPlaceBetAreaActive && area.isPlaceBetAreaActive)
                    {
                        areaCount++;
                        if (area.isBust)
                        {
                            BustCount++;
                        }
                    }
                }
            }

            if (areaCount != BustCount)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (maxValue < 17)
                    {
                        BlackJackCard dealercard = FindCurrentCard(dealerCardArea.transform);
                        yield return new WaitForSeconds(0.1f);
                        ThrowAndFlipCard(dealercard, true);
                        dealerCardList.Add(dealercard);
                        yield return new WaitForSeconds(0.65f);
                        maxValue = DealerCardValue(false);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
            //Winner Declared
            WinnerDeclared();
        }

        int DealerCardValue(bool isResultValue)
        {
            Debug.Log("Find Dealer Card Value ==>");
            //Dealer Card Value
            scoreValue.Clear();
            foreach (var item in dealerCardList)
            {
                foreach (var cValue in item.cardValue)
                {
                    cValueCounter = cValue;
                    foreach (var card in dealerCardList)
                    {
                        if (item != card)
                        {
                            cValueCounter += card.cardValue[0];
                        }
                    }
                    scoreValue.Add(cValueCounter);
                }
            }

            scoreValue = scoreValue.Distinct().ToList();
            int maxValue = scoreValue.Max();
            int resultValue = scoreValue.Max();
            scoreValue.RemoveAll(x => x > 21);
            if (scoreValue.Count != 0)
            {
                resultValue = scoreValue.Max();
            }

            dealerCardTxt.text = "";
            if (scoreValue.Count == 0)
            {
                //Bust
                dealerCardTxt.text = "BUST";
            }
            else
            {
                if (FindBlackJack(scoreValue))
                {
                    //BlackJack
                    dealerCardTxt.text = "BlackJack";
                }
                else
                {
                    foreach (var item in scoreValue)
                    {
                        if (dealerCardTxt.text == "")
                        {
                            dealerCardTxt.text = item.ToString();
                        }
                        else
                        {
                            dealerCardTxt.text += " / " + item.ToString();
                        }
                    }
                }
            }
            dealerCardObject.SetActive(true);
            if (isResultValue)
            {
                return resultValue;
            }
            else
            {
                return maxValue;
            }
        }

        bool FindBlackJack(List<int> sValue)
        {
            foreach (var item in sValue)
            {
                if (item == 21 && dealerCardList.Count == 2)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Winner Declared

        private void WinnerDeclared()
        {
            if (winnner != null)
            {
                StopCoroutine(winnner);
            }
            winnner = StartCoroutine(WinnerDecleredCorotines());
        }

        IEnumerator WinnerDecleredCorotines()
        {
            Debug.Log("WinnerDeclered ==>");
            yield return new WaitForSeconds(0.5f);

            int dealerCardValue = DealerCardValue(true);

            Debug.Log("WinnerDeclered totalInsurancePlaced ==> " + totalInsurancePlaced);
            if (totalInsurancePlaced > 0)
            {
                foreach (var player in boradManager.players)
                {
                    if (player.insuranceValue > 0)
                    {
                        if (dealerCardList[1].cardValue[0] == 10)
                        {
                            //Insurance Return with 150%
                            player.InsuranceWinAnimatin();
                        }
                        else
                        {
                            //Insurance loss
                            player.InsuranceLossAnimatin();
                        }
                        yield return new WaitForSeconds(2f);
                    }
                }
            }

            Debug.Log("WinnerDeclered isPlaceBetAreaActive ==> ");
            foreach (var player in boradManager.players)
            {
                foreach (var area in player.placeBetAreas)
                {
                    if (area.isPlaceBetAreaActive && area.isPlaceBetAreaActive )
                    {
                        int areaCardsValue = 0;
                        if (area.scoreValue.Count > 0)
                        {
                            if (area.scoreValue.Count > 1)
                            {
                                areaCardsValue = area.scoreValue.Max();
                            }
                            else
                            {
                                areaCardsValue = area.scoreValue[0];
                            }
                        }

                        if (area.isBust)
                        {
                            SetWinLossCount(false, player);
                        }
                        else if (area.isBlackJack)
                        {
                            SetWinLossCount(true, player);
                            //Win
                            area.scoreTxt.text = "WIN";
                            area.AreaWinCurrentRound(true);
                        }
                        else if (dealerCardValue == areaCardsValue)
                        {
                            SetWinLossCount(true, player);
                            // Take Back Same Value
                            area.scoreTxt.text = "PUSH";
                            area.AreaTakeSameBet();
                        }
                        else if (dealerCardValue > areaCardsValue && dealerCardValue <= 21)
                        {
                            SetWinLossCount(false, player);
                            //Lost
                            area.scoreTxt.text = "LOSS";
                            area.AreaLossCurrentRound();
                        }
                        else if (dealerCardValue < areaCardsValue || dealerCardValue > 21)
                        {
                            //Win
                            SetWinLossCount(true, player);
                            area.scoreTxt.text = "WIN";
                            area.AreaWinCurrentRound(false);
                        }
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (areaCount != BustCount)
            {
                yield return new WaitForSeconds(2.5f);
            }

            foreach (var player in boradManager.players)
            {
                foreach (var area in player.placeBetAreas)
                {
                    if (area.isPlaceBetAreaActive && area.isPlaceBetAreaActive)
                    {
                        area.RoundEndAnimation();
                    }
                }
            }

            if (roundEndAnimation != null)
            {
                StopCoroutine(roundEndAnimation);
            }
            roundEndAnimation = StartCoroutine(RoundEndCardFilpAnimation(dealerCardList));

            yield return new WaitForSeconds(2f);
            StartNewRound(true);
        }

        void SetWinLossCount(bool isWin , BlackJackPlayer player)
        {
            if (!player.isBot)
            {
                if (isWin)
                {
                    if (BlackJackDataManager.RoundPlay != BlackJackDataManager.RoundWin + BlackJackDataManager.RoundLoss)
                    {
                        BlackJackDataManager.RoundWin++;
                    }
                }
                else
                {
                    if (BlackJackDataManager.RoundPlay != BlackJackDataManager.RoundWin + BlackJackDataManager.RoundLoss)
                    {
                        BlackJackDataManager.RoundLoss++;
                    }
                }
            }
        }

        #endregion

        

        #region Round End Card Filp Animation
        Coroutine roundEndAnimation;
        IEnumerator RoundEndCardFilpAnimation( List<BlackJackCard> cards)
        {
            List<BlackJackEmptyCard> emptyCardList = new List<BlackJackEmptyCard> ();
            foreach (var card in cards)
            {
                BlackJackEmptyCard emptyCard = gameManager.pooler.SpawnFromEmptyCards("EmptyCard", EmptyDeck);
                emptyCard.transform.position = card.transform.position;
                emptyCardList.Add(emptyCard);
                emptyCard.emptyImage.sprite = card.cardImage.sprite;
                card.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.05f);
           
            ResetDealerDeatalis();

            foreach (var item in emptyCardList)
            {
                item.transform.DOScaleX(0, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    item.emptyImage.sprite = item.emptySprite;
                    item.transform.DOScaleX(1, 0.1f).SetEase(Ease.Linear);
                });
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.4f);
            for (int i = 0; i < emptyCardList.Count; i++)
            {
                if (i != 0)
                {
                    emptyCardList[i].transform.DOMove(emptyCardList[0].transform.position, 0.3f).SetEase(Ease.Linear);
                }
            }
            yield return new WaitForSeconds(0.8f); 
            for (int i = 0; i < emptyCardList.Count; i++)
            {
                if (i != 0)
                {
                    emptyCardList[i].gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(0.1f);
            emptyCardList[0].transform.DOMove(EmptyDeck.transform.position, 0.35f).SetEase(Ease.Linear).OnComplete(() =>
            {
                emptyCardList[0].gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(0.175f);
            emptyCardList[0].transform.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.35f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.175f);
        }
        #endregion

    }
}
