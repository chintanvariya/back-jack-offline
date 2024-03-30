using DG.Tweening.Core.Easing;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackPlaceBetArea : MonoBehaviour
    {
        [Header("--------------------- Card Bet Area --------------------- ")]
        [SerializeField]
        internal List<BlackJackCard> cards;
        [SerializeField]
        internal GameObject userBets;
        [SerializeField]
        internal Text userBetAmountText;
        [SerializeField]
        internal GameObject cardPosition;
        [SerializeField]
        internal bool isPlaceBetAreaActive;
        [SerializeField]
        internal bool isPlayerPlaceBet;
        [SerializeField]
        internal bool isAreaTurnOver , isBlackJack , isBust;
        [SerializeField]
        internal int areaCardValue;
        [SerializeField]
        internal GameObject placeBetAreaObject;
        [SerializeField]
        internal Image placeBetAreaImage;

        [Header("--------------------- Player Info --------------------- ")]
        [SerializeField]
        internal float playerBetAmount;
        [SerializeField]
        private BlackJackPlayer player;
        [SerializeField]
        private GameObject winAnimationObject;

        internal void SetPlayerPlaceBet(float placebetAmount)
        {
            playerBetAmount += placebetAmount;
            isPlayerPlaceBet = true;
            userBetAmountText.text = BlackJackGameManager.instance.SetBalanceFormat(playerBetAmount);
            userBets.SetActive(true);
        }

        internal void ResetPlaceBetArea()
        {
            playerBetAmount = 0;
            isPlaceBetAreaActive = false;
            isPlayerPlaceBet = false;
            isAreaTurnOver = false;
            userBetAmountText.text = "";
            userBets.SetActive(false);
            cardBgImage.enabled = false;
            isBlackJack = isBust = false;
            backjackImage.SetActive(false);
            BustImage.SetActive(false);
            scoreObject.SetActive(false);

            foreach (var item in cards)
            {
                item.ResetCard();
            }
            cards.Clear();
            for (int i = 0; i < scoreValue.Count; i++)
            {
                scoreValue[i] = 0;
            }
            winAnimationObject.SetActive(false);
        }

        internal void ActivePlacebetArea()
        {
            isPlaceBetAreaActive = true;
        }

        #region Score Info
        [Header("--------------------- Score Info --------------------- ")]
        [SerializeField]
        internal List<int> scoreValue;
        [SerializeField]
        internal GameObject scoreObject;
        [SerializeField]
        internal Text scoreTxt;
        [SerializeField]
        private Sprite blackJackSprite, bustSprite;
        [SerializeField]
        private Image cardBgImage;
        [SerializeField]
        private GameObject backjackImage, BustImage;

        int cValueCounter;
        internal void FindPlayerCardValue()
        {
            Debug.Log("FindPlayerCardValue ==>");

            scoreValue.Clear();
            foreach (var item in cards)
            {
                foreach (var cValue in item.cardValue)
                {
                    cValueCounter = cValue;
                    foreach (var card in cards)
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
            int bustValue = scoreValue.Find(x => x > 21);
            areaCardValue = scoreValue.Max();
            scoreValue.RemoveAll(x => x > 21);
            scoreTxt.text = "";
            scoreObject.SetActive(false);
            if (scoreValue.Count == 0)
            {
                //Bust
                cardBgImage.sprite = bustSprite;
                cardBgImage.enabled = true;
                isBust = true;
                scoreTxt.text = bustValue.ToString();
                scoreObject.SetActive(true);
                BustImage.SetActive(true);
                isAreaTurnOver = true;
                //Lost
                AreaLossCurrentRound();

            }
            else
            {
                if (FindBlackJack(scoreValue))
                {
                    //BlackJack
                    cardBgImage.sprite = blackJackSprite;
                    cardBgImage.enabled = true;
                    isBlackJack = true;
                    scoreTxt.text = "21";
                    scoreObject.SetActive(true);
                    backjackImage.SetActive(true);
                    isAreaTurnOver = true;
                }
                else
                {
                    foreach (var item in scoreValue)
                    {
                        if (scoreTxt.text == "")
                        {
                            scoreTxt.text = item.ToString();
                        }
                        else
                        {
                            scoreTxt.text += " / " + item.ToString();
                        }
                    }
                    scoreObject.SetActive(true);
                }
            }
        }

        bool FindBlackJack(List<int> sValue)
        {
            foreach (var item in sValue)
            {
                if (item == 21 && cards.Count == 2)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Turn System

        internal void SetUserTurn()
        {
            if (!player.isBot)
            {
                //Player
                BlackJackSettingManager.instance.PlaySound("Turn");
                BlackJackSettingManager.instance.PlayVirationEffect();
                player.StartTimer(BlackJackGameManager.instance.turnTimer , "UserTurn" , placeBetAreaImage);
                BlackJackGameManager.instance.betButtons.UserTurnStart(this);
            }
            else
            {
                //bot
                player.StartTimer(BlackJackGameManager.instance.turnTimer, "UserTurn", placeBetAreaImage);
                BotTurnStart();
            }
        }

        internal void UserInsurance()
        {
            if (!player.isBot)
            {
                //Player
                player.StartTimer(BlackJackGameManager.instance.insuranceTimer, "UserInsurance");
                BlackJackGameManager.instance.betButtons.UserInsurance();
            }
            else
            {
                //bot
                player.StartTimer(BlackJackGameManager.instance.insuranceTimer, "UserInsurance");
                BotInsuranceStart();
            }
        }

        #endregion

        #region Bot

        #region Bot Turn
        Coroutine botTurn;

        void BotTurnStart()
        {
            if (botTurn != null)
            {
                StopCoroutine(botTurn);
            }
            botTurn = StartCoroutine(BotTurnStartDelay());
        }

        IEnumerator BotTurnStartDelay()
        {
            yield return new WaitForSeconds(Random.Range(0, BlackJackGameManager.instance.turnTimer - 3));
            bool botSplit = false;
            if (player.FindSplitButtonAction(this))
            {
                int splitRandom = Random.Range(0, 5);
                if (splitRandom >= 3)
                {
                    botSplit = true;
                }
            }
            if (botSplit && player.isCraditAvalible(playerBetAmount))
            {
                scoreObject.SetActive(false);
                player.StopTimer();
                player.SplitCards(playerBetAmount, SplitBetPlaced);
            }
            else if (areaCardValue < 16)
            {
                // Hit or Double
                int doubleRandom = 0;
                if (areaCardValue > 11)
                {
                    doubleRandom = Random.Range(0, 5);
                }

                if (doubleRandom >= 3 && player.isCraditAvalible(playerBetAmount) && cards.Count == 2)
                {
                    //Double
                    scoreObject.SetActive(false);
                    BlackJackGameManager.instance.dealer.StopUserTurn();
                    player.DoubleBet(playerBetAmount, this, DoubleBetPlaced);
                } 
                else
                {
                    //Hit
                    scoreObject.SetActive(false);
                    player.StopTimer();
                    BlackJackGameManager.instance.dealer.AddNewCard(this, true);
                }
            }
            else
            {
                //stand
                player.StopTimer();
                isAreaTurnOver = true;
            }
        }

        internal void SplitBetPlaced()
        {
            BlackJackGameManager.instance.dealer.AddNewCard(player.placeBetAreas[0], false, thorwSecondCard);
        }

        private void thorwSecondCard()
        {
            BlackJackGameManager.instance.dealer.AddNewCard(player.placeBetAreas[1], true);
        }

        internal void DoubleBetPlaced()
        {
            isAreaTurnOver = true;
            BlackJackGameManager.instance.dealer.AddNewCard(this, true);
        }

        #endregion

        #region bot Insurance
        Coroutine botInsurance;

        void BotInsuranceStart()
        {
            if (botInsurance != null)
            {
                StopCoroutine(botInsurance);
            }
            botInsurance = StartCoroutine(BotInsuranceDelay());
        }

        IEnumerator BotInsuranceDelay()
        {
            yield return new WaitForSeconds(Random.Range(1, BlackJackGameManager.instance.insuranceTimer - 4));
            int splitRandom = Random.Range(0, 5);
            if (splitRandom >= 3 && player.isCraditAvalible(playerBetAmount / 2))
            {
                float insuranceAmount = playerBetAmount / 2;
                player.AddInsurance(insuranceAmount);
            }
            else
            {
                player.StopTimer();
                BlackJackGameManager.instance.dealer.totalInsuranceBet++;
            }
        }
        #endregion

        #endregion

        #region Round End Card Filp Animation
        Coroutine roundEndAnimation;

        internal void RoundEndAnimation()
        {
            if (roundEndAnimation != null)
            {
                StopCoroutine(roundEndAnimation);
            }
            roundEndAnimation = StartCoroutine(RoundEndCardFilpAnimation(cards));
        }

        IEnumerator RoundEndCardFilpAnimation(List<BlackJackCard> cards)
        {
            List<BlackJackEmptyCard> emptyCardList = new List<BlackJackEmptyCard>();
            var pooler = BlackJackGameManager.instance.pooler;
            var dealer = BlackJackGameManager.instance.dealer;
            foreach (var card in cards)
            {
                BlackJackEmptyCard emptyCard = pooler.SpawnFromEmptyCards("EmptyCard", dealer.EmptyDeck);
                emptyCard.transform.position = card.transform.position;
                emptyCardList.Add(emptyCard);
                emptyCard.emptyImage.sprite = card.cardImage.sprite;
                card.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.05f);
            ResetPlaceBetArea();
           
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
                    emptyCardList[i].transform.DOMove(emptyCardList[0].transform.position, 0.2f).SetEase(Ease.Linear);
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
            emptyCardList[0].transform.DOMove(dealer.EmptyDeck.transform.position, 0.35f).SetEase(Ease.Linear).OnComplete(() =>
            {
                emptyCardList[0].gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(0.175f);
            emptyCardList[0].transform.DOScale(new Vector3(0.35f, 0.35f, 0.35f), 0.35f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.175f);
        }
        #endregion

        #region Win & Loss

        Coroutine loss, win, tackBack;

        internal void AreaLossCurrentRound()
        {
            if (loss != null)
            {
                StopCoroutine(loss);
            }
            loss = StartCoroutine(LostAnimation());
        }

        IEnumerator LostAnimation()
        {
            var dealer = BlackJackGameManager.instance.dealer;
            BlackJackPlaceBetCoin lossBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", userBets.transform, playerBetAmount);
            userBets.SetActive(false);
            if (lossBet.betAnimation != null)
            {
                lossBet.betAnimation = null;
            }
            yield return new WaitForSeconds(0.2f);
            lossBet.betAnimation = lossBet.transform.DOMove(dealer.dealerChipArea.position, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                lossBet.gameObject.SetActive(false);
            });
        }

        internal void AreaWinCurrentRound(bool isBlackJack)
        {
            if (win != null)
            {
                StopCoroutine(win);
            }
            win = StartCoroutine(WinAnimation(player, isBlackJack));
        }

        IEnumerator WinAnimation(BlackJackPlayer player, bool isBlackJack)
        {
            BlackJackSettingManager.instance.PlaySound("Win");
            var dealer = BlackJackGameManager.instance.dealer;
            float WinningAmount;
            if (isBlackJack)
            {
                WinningAmount = playerBetAmount * 1.5f;
            }
            else
            {
                WinningAmount = playerBetAmount;
            }
            winAnimationObject.SetActive(true);

            BlackJackPlaceBetCoin placeBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", dealer.dealerChipArea, WinningAmount);
            if (placeBet.betAnimation != null)
            {
                placeBet.betAnimation = null;
            }
            yield return new WaitForSeconds(0.1f);
            placeBet.betAnimation = placeBet.transform.DOMove(userBetAmountText.transform.position, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                SetPlayerPlaceBet(WinningAmount);
                placeBet.gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(1.2f);

            BlackJackPlaceBetCoin winBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", userBets.transform, playerBetAmount);
            userBets.SetActive(false);
            if (winBet.betAnimation != null)
            {
                winBet.betAnimation = null;
            }
            yield return new WaitForSeconds(0.1f);
            winBet.betAnimation = winBet.transform.DOMove(player.craditAreaPoision.transform.position, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                player.UpdateCreditPoints(playerBetAmount);
                winBet.gameObject.SetActive(false);
            });
        }

        internal void AreaTakeSameBet()
        {
            if (tackBack != null)
            {
                StopCoroutine(tackBack);
            }
            tackBack = StartCoroutine(TakeBackAnimation(player));
        }

        IEnumerator TakeBackAnimation(BlackJackPlayer player)
        {
            BlackJackPlaceBetCoin tackBackBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", userBets.transform, playerBetAmount);
            userBets.SetActive(false);
            if (tackBackBet.betAnimation != null)
            {
                tackBackBet.betAnimation = null;
            }
            yield return new WaitForSeconds(0.2f);
            tackBackBet.betAnimation = tackBackBet.transform.DOMove(player.craditAreaPoision.transform.position, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                player.UpdateCreditPoints(playerBetAmount);
                tackBackBet.gameObject.SetActive(false);
            });
        }
        #endregion
    }
}
 