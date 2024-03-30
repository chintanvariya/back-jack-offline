using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackPlayer : MonoBehaviour
    {
        [Header("--------------------- Player Info --------------------- ")]
        [SerializeField]
        internal bool isBot;
        [SerializeField]
        internal TMP_Text userNameTxt;
        [SerializeField]
        internal Image userImage;
        [SerializeField]
        internal List<Sprite> avatarSprites;
        [SerializeField]
        internal TMP_Text userCreditPointTxt;
        [SerializeField]
        internal float botCraditPoints;
        [SerializeField]
        internal GameObject waitForNextRound;
        [SerializeField]
        internal GameObject placeYourBetTxt;

        Coroutine waitToast;

        internal void WaitForNextRound()
        {
            waitForNextRound.SetActive(false);
            if (waitToast != null)
            {
                StopCoroutine(waitToast);
            }
            waitToast = StartCoroutine(WaitToastCoroutine());
        }

        IEnumerator WaitToastCoroutine()
        {
            waitForNextRound.SetActive(true);
            yield return new WaitForSeconds(2f);
            waitForNextRound.SetActive(false);
        }

        internal void SetPlayerInfo()
        {
            if (!isBot)
            {
                userNameTxt.text = BlackJackDataManager.userName;
                userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
                userImage.sprite = avatarSprites[BlackJackDataManager.ProfileImage - 1];
            }
            else
            {
                botCraditPoints = (BlackJackDataManager.lobbyMaxValue * 8) / UnityEngine.Random.Range(1, 4);
                userNameTxt.text = BlackJackGameManager.instance.botsName[UnityEngine.Random.Range(0, BlackJackGameManager.instance.botsName.Count)];
                userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(botCraditPoints);
                userImage.sprite = avatarSprites[UnityEngine.Random.Range(1, avatarSprites.Count)];
            }
        }

        internal void LoadNewRoundData()
        {
            Debug.Log("Load New Round Data ");
            Debug.Log("Load New Round Data isBot ==> " + isBot);

            if (!isBot)
            {
                // Player Place Bet
                BlackJackGameManager.instance.betButtons.LoadNewRoundData();
            }
            else
            {
                //Bot Place Bet
                botPlaceBet();
            }

            // For Player And Bot 
            ResetPlayerInfo();
            //StartTimer
            StartTimer(BlackJackGameManager.instance.placeBetTimer , "placeBetTimer");
        }

        void ResetPlayerInfo()
        {
            foreach (var item in placeBetAreas)
            {
                item.ResetPlaceBetArea();
                item.gameObject.SetActive(false);
            }
            placeBetAreas[0].ActivePlacebetArea();
            insuranceValue = 0;
            insuranceCoinObject.SetActive(false);
            handAnimation.Play("ResetAnimation");
            placeYourBetTxt.SetActive(false);
        }

        internal void UpdateCreditPoints(float Amount)
        {
            if (!isBot)
            {
                Debug.Log("UpdateCreditPoints => " + Amount);
                // Player Credit Points
                BlackJackDataManager.creditPoint += Amount;
                userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
            }
            else
            {
                // Bot Credit Points
                botCraditPoints += Amount;
                userCreditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(botCraditPoints);
            }
        }

        #region place bet Area

        [Header("--------------------- Place bet Info --------------------- ")]
        [SerializeField]
        internal List<BlackJackPlaceBetArea> placeBetAreas;
        [SerializeField]
        internal GameObject CloneArea;
        [SerializeField]
        internal GameObject craditAreaPoision;
        [SerializeField]
        internal GameObject buttonAnimation;
        [SerializeField]
        internal Animator handAnimation;
        [SerializeField]
        internal Image toolTip;

        Coroutine placebetFunction , splitBet;
        Tween cloneAreaAnimation , cardAnimation;

        internal void AddBet(float PlaceBetAmount)
        {
            foreach (var item in placeBetAreas)
            {
                if (item.isPlaceBetAreaActive && !item.isPlayerPlaceBet && gameObject.activeInHierarchy)
                {
                    if(placebetFunction != null)
                    {
                        StopCoroutine(placebetFunction);
                    }
                    placebetFunction = StartCoroutine(AddBetAnimation(PlaceBetAmount, item));
                    StopTimer();
                    if(!isBot)
                    {
                        BlackJackDataManager.RoundPlay++;
                    }
                }
            }
        }

        IEnumerator AddBetAnimation(float PlaceBetAmount , BlackJackPlaceBetArea placeBetArea , Action AnimationComplete = null)
        {
            Debug.Log("AddBetAnimation ==>");
            Debug.Log("AddBetAnimation ==> " +this.name);
            BlackJackSettingManager.instance.PlaySound("Chips");
            placeYourBetTxt.SetActive(false);
            placeBetArea.gameObject.SetActive(true);
            UpdateCreditPoints(-PlaceBetAmount);
            yield return new WaitForSeconds(0.1f);
            BlackJackPlaceBetCoin placeBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", craditAreaPoision.transform, PlaceBetAmount);
            if (placeBet.betAnimation != null)
            {
                placeBet.betAnimation = null;
            }

            placeBet.betAnimation = placeBet.transform.DOMove(placeBetArea.userBetAmountText.transform.position,0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                placeBetArea.SetPlayerPlaceBet(PlaceBetAmount);
                placeBet.gameObject.SetActive(false);
                BlackJackGameManager.instance.dealer.totalPlayerRoundPlacebet++;
                if (AnimationComplete != null)
                {
                    AnimationComplete.Invoke();
                }
            }); 
        }

        internal void DoubleBet(float PlaceBetAmount , BlackJackPlaceBetArea area , Action AnimationComplete)
        {
            if (area.isPlaceBetAreaActive && area.isPlayerPlaceBet)
            {
                if (placebetFunction != null)
                {
                    StopCoroutine(placebetFunction);
                }
                placebetFunction = StartCoroutine(AddBetAnimation(PlaceBetAmount, area , AnimationComplete));
                StopTimer();
            }
        }

        internal void SplitCards(float PlaceBetAmount , Action AnimationComplete)
        {
            StopTimer();
            if (splitBet != null)
            {
                StopCoroutine(splitBet);
            }
            splitBet = StartCoroutine(SplitBetAnimation(PlaceBetAmount, AnimationComplete));
        }

        IEnumerator SplitBetAnimation(float PlaceBetAmount, Action AnimationComplete)
        {
            // Area and Area Animation
            placeBetAreas[1].ActivePlacebetArea();
            placeBetAreas[1].placeBetAreaObject.SetActive(false);
            placeBetAreas[1].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);

            CloneArea.transform.position = placeBetAreas[0].transform.position;
            CloneArea.SetActive(true);
            if (cloneAreaAnimation != null)
            {
                cloneAreaAnimation = null;
            }
            cloneAreaAnimation = CloneArea.transform.DOMove(placeBetAreas[1].transform.position , 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                placeBetAreas[1].placeBetAreaObject.SetActive(true);
                CloneArea.SetActive(false);
            });
            yield return new WaitForSeconds(1.5f);

            // Card Animation
            var card = placeBetAreas[0].cards[1];

            card.transform.SetParent(placeBetAreas[1].cardPosition.transform);
            placeBetAreas[0].cards.Remove(card);
            placeBetAreas[1].cards.Add(card);
            card.gameObject.transform.localScale = new Vector3 (0, 0, 0);

            BlackJackEmptyCard emptyCard = BlackJackGameManager.instance.pooler.SpawnFromEmptyCards("EmptyCard", placeBetAreas[0].cards[0].transform);

            emptyCard.emptyImage.sprite = card.cardImage.sprite;
            emptyCard.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.25f);

            if (cardAnimation != null)
            {
                cardAnimation = null;
            }
            cardAnimation = emptyCard.transform.DOMove(card.transform.position, 0.75f).SetEase(Ease.Linear).OnComplete(() =>
            {
                card.gameObject.transform.localScale = new Vector3(1, 1, 1);
                emptyCard.gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(1f);

            //Add New Area Bet
            if (placebetFunction != null)
            {
                StopCoroutine(placebetFunction);
            }
            placebetFunction = StartCoroutine(AddBetAnimation(PlaceBetAmount, placeBetAreas[1], AnimationComplete));
        }

        #endregion

        #region Player Timer

        [Header("--------------------- Timer Info --------------------- ")]
        [SerializeField]
        private GameObject timerObject;
        [SerializeField]
        private Image timerFillImage;
        [SerializeField]
        private Transform timerTop;
        Coroutine timerFuction;
        Image tempTurnImage;

        internal void StartTimer(float tTime, string action , Image currnetTurnImage = null)
        {
            StopTimer();
            tempTurnImage = currnetTurnImage;
            timerObject.SetActive(true);
            timerFuction = StartCoroutine(StartCountDown(tTime , action));
        }

        internal void StopTimer()
        {
            if (timerFuction != null)
            {
                StopCoroutine(timerFuction);
            }
            timerObject.SetActive(false);
            if (tempTurnImage != null)
            {
                tempTurnImage.sprite = BlackJackGameManager.instance.userTurnSprites[0];
                tempTurnImage = null;
            }
        }

        IEnumerator StartCountDown(float tTime, string action)
        {
            if (tempTurnImage != null)
            {
                tempTurnImage.sprite = BlackJackGameManager.instance.userTurnSprites[1];
            }
            float TotalTime = tTime;
            while (tTime > 0)
            {
                tTime -= Time.deltaTime;
                timerFillImage.fillAmount = tTime / TotalTime;
                yield return null;
            }
            SetActiveOnTimeOver(action);
        }

        void SetActiveOnTimeOver(string action)
        {
            switch(action)
            {
                case "placeBetTimer":
                    BlackJackGameManager.instance.betButtons.PlaceBetTimerOver();
                    break;
                case "UserTurn":
                    BlackJackGameManager.instance.betButtons.UserTurnTimeOver();
                    break;
                case "UserInsurance":
                    BlackJackGameManager.instance.betButtons.InsuranceTimerOver();
                    break;
            }
            timerObject.SetActive(false);
        }
        #endregion

        #region User Insurance

        [Header("--------------------- User Insurance --------------------- ")]
        [SerializeField]
        internal GameObject insuranceCoinObject;
        [SerializeField]
        internal Text insuranceAmountText;
        [SerializeField]
        internal float insuranceValue;

        Coroutine insuranceFunction;
        internal void AddInsurance(float InsuranceAmount)
        {
            if (insuranceFunction != null)
            {
                StopCoroutine(insuranceFunction);
            }
            insuranceFunction = StartCoroutine(AddInsuranceAnimation(InsuranceAmount));
            StopTimer();
        }

        IEnumerator AddInsuranceAnimation(float InsuranceAmount)
        {
            insuranceValue = InsuranceAmount;
            UpdateCreditPoints(-InsuranceAmount);

            yield return new WaitForSeconds(0.1f);

            BlackJackPlaceBetCoin Bet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", craditAreaPoision.transform, InsuranceAmount);
            if (Bet.betAnimation != null)
            {
                Bet.betAnimation = null;
            }

            Bet.betAnimation = Bet.transform.DOMove(insuranceCoinObject.transform.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                insuranceAmountText.text = BlackJackGameManager.instance.SetBalanceFormat(insuranceValue);
                insuranceCoinObject.SetActive(true);
                Bet.gameObject.SetActive(false);
                BlackJackGameManager.instance.dealer.totalInsuranceBet++;
                BlackJackGameManager.instance.dealer.totalInsurancePlaced++; 
            });
        }
        #endregion

        #region Bot Place Bet

        Coroutine BotPlaceBetCoroutine;

        void botPlaceBet()
        {
            if (BotPlaceBetCoroutine != null)
            {
                StopCoroutine(BotPlaceBetCoroutine);
            }
            BotPlaceBetCoroutine = StartCoroutine(BotPlaceBet());
        }

        IEnumerator BotPlaceBet()
        {
            float placeBetdelay = UnityEngine.Random.Range(0, BlackJackGameManager.instance.placeBetTimer - 3);
            yield return new WaitForSeconds(placeBetdelay);
            if (isCraditAvalible(botCraditPoints))
            {
                float AddAmount = BlackJackDataManager.lobbyMinValue * UnityEngine.Random.Range(1, 2.5f);
                float placeAmount = BlackJackDataManager.lobbyMinValue + AddAmount;
                float amount = BlackJackGameManager.instance.SetRoundFormat(placeAmount);
                AddBet(amount);
            }
        }

        #endregion

        #region Splite Bet Active

        internal bool FindSplitButtonAction(BlackJackPlaceBetArea area)
        {
            if (placeBetAreas[0].isPlaceBetAreaActive && !placeBetAreas[1].isPlaceBetAreaActive)
            {
                if (area.isPlaceBetAreaActive && area.cards.Count == 2 )
                {
                    if (area.cards[0].cardNumber == area.cards[1].cardNumber)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        

        internal bool FindDoubleButtonAction(BlackJackPlaceBetArea area)
        {
            if (placeBetAreas[0].isPlaceBetAreaActive && !placeBetAreas[1].isPlaceBetAreaActive)
            {
                if (area.isPlaceBetAreaActive && area.cards.Count == 2 )
                {
                    return true;
                }
            }
            return false;
        }

        internal bool isCraditAvalible(float amount)
        {
            if (!isBot)
            {
                if (BlackJackDataManager.creditPoint >= amount)
                {
                    return true;
                }
            }
            else
            {
                if (botCraditPoints >= amount)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Insurance Animation

        Coroutine insuranceLoss, insuranceWin;

        internal void InsuranceWinAnimatin()
        {
            if (insuranceWin != null)
            {
                StopCoroutine(insuranceWin);
            }
            insuranceWin = StartCoroutine(InsuranceWinAnimation());
        }

        IEnumerator InsuranceWinAnimation()
        {
            var dealer = BlackJackGameManager.instance.dealer;
            float WinningAmount;
            WinningAmount = insuranceValue * 2f;

            BlackJackPlaceBetCoin placeBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", dealer.dealerChipArea, WinningAmount);
            if (placeBet.betAnimation != null)
            {
                placeBet.betAnimation = null;
            }
            yield return new WaitForSeconds(0.2f);
            placeBet.betAnimation = placeBet.transform.DOMove(insuranceCoinObject.transform.position, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                insuranceValue += WinningAmount;
                insuranceAmountText.text = BlackJackGameManager.instance.SetBalanceFormat(insuranceValue);
                placeBet.gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(1.3f);

            BlackJackPlaceBetCoin winBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", insuranceCoinObject.transform, insuranceValue);
            insuranceCoinObject.SetActive(false);
            if (winBet.betAnimation != null)
            {
                winBet.betAnimation = null;
            }
            yield return new WaitForSeconds(0.2f);
            winBet.betAnimation = winBet.transform.DOMove(craditAreaPoision.transform.position, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                UpdateCreditPoints(insuranceValue);
                winBet.gameObject.SetActive(false);
            });
        }

        internal void InsuranceLossAnimatin()
        {
            if (insuranceLoss != null)
            {
                StopCoroutine(insuranceLoss);
            }
            insuranceLoss = StartCoroutine(InsuranceLostAnimation());
        }

        IEnumerator InsuranceLostAnimation()
        {
            var dealer = BlackJackGameManager.instance.dealer;
            BlackJackPlaceBetCoin lossBet = BlackJackGameManager.instance.pooler.SpawnCoinFromPlayer("placeBetCoin", insuranceCoinObject.transform, insuranceValue);
            insuranceCoinObject.SetActive(false);
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
        #endregion

        internal void StopAllPlayerCorotine()
        {
            foreach (var item in placeBetAreas)
            {
                item.StopAllCoroutines();
            }
            StopAllCoroutines();
        }

    }
}