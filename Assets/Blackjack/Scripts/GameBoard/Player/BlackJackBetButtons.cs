using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackJackBetButtons : MonoBehaviour
    {
        #region unity and all Buttons
        [Header("--------------------- All Bet Buttons --------------------- ")]
        [SerializeField]
        private List<GameObject> allBetsButton;

        // Start is called before the first frame update
        void Start()
        {
            minButton.onClick.AddListener(MinButtonClicked);
            maxButton.onClick.AddListener(MaxButtonClicked);
            betButton.onClick.AddListener(BetButtonClicked);
            confirmButton.onClick.AddListener(ConfirmButtonClicked);
            reBetButton.onClick.AddListener(ReBetButtonClicked);
            splitButton.onClick.AddListener(SplitButtonClicked);
            standButton.onClick.AddListener(StandButtonClicked);
            hitButton.onClick.AddListener(HitButtonClicked);
            doubleBetButton.onClick.AddListener(DoubleBetButtonClicked);
            yesButton.onClick.AddListener(YesInsuranceClicked);
            noButton.onClick.AddListener(NoInsuranceClicked);
            scrollbar.onValueChanged.AddListener((float val) => ScorllValueChanged(val));
        }

        #endregion

        #region Place Bet Buttons

        [Header("--------------------- Place Bet Buttons --------------------- ")]
        [SerializeField]
        private Button minButton;
        [SerializeField]
        private Button maxButton, betButton, confirmButton, reBetButton;
        [SerializeField]
        private GameObject confirmButtonObject;
        [SerializeField]
        private GameObject reBetActive, reBetDeactive;
        [SerializeField]
        internal BlackJackPlayer player;

        [Header("--------------------- Bet Area --------------------- ")]
        [SerializeField]
        private List<Text> betsTexts;
        [SerializeField]
        private TMP_Text SliderText, creditPointTxt;
        [SerializeField]
        private Scrollbar scrollbar;
        [SerializeField]
        private Image fillBar;
        [SerializeField]
        float minValue, diffValue, selectAmount;

        [Header("--------------------- other Bet Area --------------------- ")]
        [SerializeField]
        private Text minBetText;
        [SerializeField]
        private Text maxBetText, reBetText;
        internal float lastBet = 0f;

        internal void LoadNewRoundData()
        {
            player.waitForNextRound.gameObject.SetActive(false);
            PlaceBetButtonAction(true);
            minBetText.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.lobbyMinValue);
            maxBetText.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.lobbyMaxValue);
            minButton.interactable = player.isCraditAvalible(BlackJackDataManager.lobbyMinValue);
            betButton.interactable = player.isCraditAvalible(BlackJackDataManager.lobbyMinValue);
            maxButton.interactable = player.isCraditAvalible(BlackJackDataManager.lobbyMaxValue);
            if (lastBet != 0)
            {
                reBetButton.interactable = player.isCraditAvalible(lastBet);
                reBetText.text = BlackJackGameManager.instance.SetBalanceFormat(lastBet);
                reBetActive.SetActive(true);
                reBetDeactive.SetActive(false);
            }
            else
            {
                reBetButton.interactable = false;
                reBetText.text = "--";
                reBetActive.SetActive(false);
                reBetDeactive.SetActive(true);
            }
            for (int i = 0; i < allBetsButton.Count; i++)
            {
                allBetsButton[i].SetActive(false);
                if (i < 4)
                {
                    allBetsButton[i].SetActive(true);
                }
            }
            confirmButtonObject.gameObject.SetActive(false);
            MoveSide(topPostion);
        }

        private void MinButtonClicked()
        {
            PlaceBetButtonAction(false);
            MoveSide(downPosition);
            float placeAmount = BlackJackDataManager.lobbyMinValue;
            player.AddBet(placeAmount);
            lastBet = placeAmount;
            confirmButtonObject.gameObject.SetActive(false);
        }

        private void MaxButtonClicked()
        {
            PlaceBetButtonAction(false);
            MoveSide(downPosition);
            float placeAmount = BlackJackDataManager.lobbyMaxValue;
            player.AddBet(placeAmount);
            lastBet = placeAmount;
            confirmButtonObject.gameObject.SetActive(false);
        }

        private void BetButtonClicked()
        {
            SetTableAsLimiteBetsText();
            betButton.gameObject.SetActive(false);
            confirmButtonObject.gameObject.SetActive(true);
        }

        internal void SetTableAsLimiteBetsText()
        {
            creditPointTxt.text = BlackJackGameManager.instance.SetBalanceFormat(BlackJackDataManager.creditPoint);
            minValue = BlackJackDataManager.lobbyMinValue;
            diffValue = BlackJackDataManager.lobbyMaxValue - BlackJackDataManager.lobbyMinValue;
            float diffAount = diffValue / 6;
            betsTexts[0].text = BlackJackGameManager.instance.SetBalanceFormat(minValue);
            for (int i = 1; i < betsTexts.Count; i++)
            {
                betsTexts[i].text = BlackJackGameManager.instance.SetBalanceFormat(minValue + (diffAount * i));
            }
            scrollbar.value = 0;
            lastValue = 0;
            selectAmount = minValue;
            SliderText.text = BlackJackGameManager.instance.SetBalanceFormat(minValue);
            float credite = BlackJackDataManager.creditPoint - minValue;
            maxScrollLimit = credite / diffValue;
        }

        public float maxScrollLimit;
        float lastValue = 0;
        private void ScorllValueChanged(float value)
        {
            fillBar.fillAmount = value;
            if (maxScrollLimit > value)
            {
                if (BlackJackDataManager.creditPoint > selectAmount)
                {
                    selectAmount = (diffValue * value) + minValue;
                    lastValue = value;
                }
                else
                {
                    if (lastValue <= value)
                    {
                        selectAmount = BlackJackDataManager.creditPoint;
                        scrollbar.value = lastValue;
                        fillBar.fillAmount = lastValue;
                    }
                    else
                    {
                        selectAmount = (diffValue * value) + minValue;
                    }
                }
            }
            if (maxScrollLimit <= value)
            {
                selectAmount = BlackJackDataManager.creditPoint;
                scrollbar.value = maxScrollLimit;
                fillBar.fillAmount = maxScrollLimit;
                lastValue = maxScrollLimit;
            }
            if (value <= 0)
            {
                selectAmount = minValue;
            }
            selectAmount = BlackJackGameManager.instance.SetRoundFormat(selectAmount);
            SliderText.text = BlackJackGameManager.instance.SetBalanceFormat(selectAmount);
        }

        public void ConfirmButtonClicked()
        {
            PlaceBetButtonAction(false);
            MoveSide(downPosition);
            float placeAmount = 0;
            if (!player.isCraditAvalible(selectAmount))
            {
                placeAmount = BlackJackGameManager.instance.SetRoundFormat(BlackJackDataManager.creditPoint);
            }
            else
            {
                placeAmount = BlackJackGameManager.instance.SetRoundFormat(selectAmount);
            }
            player.AddBet(placeAmount);
            lastBet = placeAmount;
            betButton.gameObject.SetActive(true);
            confirmButtonObject.gameObject.SetActive(false);
        }

        private void ReBetButtonClicked()
        {
            PlaceBetButtonAction(false);
            MoveSide(downPosition);
            float placeAmount = lastBet;
            player.AddBet(placeAmount);
            lastBet = placeAmount;
            confirmButtonObject.gameObject.SetActive(false);
        }

        internal void PlaceBetTimerOver()
        {
            //confirmButtonObject.gameObject.SetActive(false);
            player.placeYourBetTxt.SetActive(true);
        }

        void PlaceBetButtonAction(bool Action)
        {
            minButton.interactable = Action;
            maxButton.interactable = Action;
            betButton.interactable = Action;
            confirmButton.interactable = Action;
            reBetButton.interactable = Action;
        }

        #endregion

        #region Game Play Buttons

        [Header("--------------------- Game Play Buttons --------------------- ")]
        [SerializeField]
        private Button splitButton;
        [SerializeField]
        private Button standButton, hitButton, doubleBetButton;
        BlackJackPlaceBetArea CurrentTurnArea;
        [SerializeField]
        private List<Sprite> buttonToastImages;

        internal void UserTurnStart(BlackJackPlaceBetArea area)
        {
            GamePlayButtonAction(true);
            CurrentTurnArea = area;
            splitButton.interactable = player.FindSplitButtonAction(area);
            doubleBetButton.interactable = player.FindDoubleButtonAction(area);
            for (int i = 0; i < allBetsButton.Count; i++)
            {
                allBetsButton[i].SetActive(false);
                if (i >= 4 && i < 8)
                {
                    allBetsButton[i].SetActive(true);
                }
            }
            MoveSide(topPostion);
        }

        private void SplitButtonClicked()
        {
            if (!player.isCraditAvalible(CurrentTurnArea.playerBetAmount))
            {
                BlackJackGameManager.instance.popupManager.SetToastAlertPopup("Insufficient Balance");
                return;
            }
            CurrentTurnArea.scoreObject.SetActive(false);
            GamePlayButtonAction(false);
            MoveSide(downPosition);
            player.StopTimer();
            player.SplitCards(CurrentTurnArea.playerBetAmount, SplitBetPlaced);
            HandAnimation(buttonToastImages[0], "toolTipAnimation");
        }

        internal void SplitBetPlaced()
        {
            BlackJackGameManager.instance.dealer.AddNewCard(player.placeBetAreas[0], false, thorwSecondCard);
        }

        private void thorwSecondCard()
        {
            BlackJackGameManager.instance.dealer.AddNewCard(player.placeBetAreas[1], true);
        }

        private void StandButtonClicked()
        {
            GamePlayButtonAction(false);
            MoveSide(downPosition);
            player.StopTimer();
            CurrentTurnArea.isAreaTurnOver = true;
            HandAnimation(buttonToastImages[1], "StandAnimation");
        }

        private void HitButtonClicked()
        {
            BlackJackSettingManager.instance.PlaySound("Hit");
            CurrentTurnArea.scoreObject.SetActive(false);
            GamePlayButtonAction(false);
            MoveSide(downPosition);
            player.StopTimer();
            BlackJackGameManager.instance.dealer.AddNewCard(CurrentTurnArea, true);
            HandAnimation(buttonToastImages[2], "HitAnimation");
        }

        private void DoubleBetButtonClicked()
        {
            if (!player.isCraditAvalible(CurrentTurnArea.playerBetAmount))
            {
                BlackJackGameManager.instance.popupManager.SetToastAlertPopup("Insufficient Balance");
                return;
            }
            CurrentTurnArea.scoreObject.SetActive(false);
            GamePlayButtonAction(false);
            MoveSide(downPosition);
            BlackJackGameManager.instance.dealer.StopUserTurn();
            player.DoubleBet(CurrentTurnArea.playerBetAmount, CurrentTurnArea, DoubleBetPlaced);
            HandAnimation(buttonToastImages[3], "toolTipAnimation");
        }

        void HandAnimation(Sprite tooltip, string animation)
        {
            player.toolTip.sprite = tooltip;
            player.buttonAnimation.SetActive(true);
            player.handAnimation.Play(animation);
        }

        internal void DoubleBetPlaced()
        {
            CurrentTurnArea.isAreaTurnOver = true;
            BlackJackGameManager.instance.dealer.AddNewCard(CurrentTurnArea, true);
        }

        internal void UserTurnTimeOver()
        {
            GamePlayButtonAction(false);
            CurrentTurnArea.isAreaTurnOver = true;
            MoveSide(downPosition);
        }

        void GamePlayButtonAction(bool Action)
        {
            standButton.interactable = Action;
            splitButton.interactable = Action;
            hitButton.interactable = Action;
            doubleBetButton.interactable = Action;
        }

        #endregion

        #region Insurance

        [Header("--------------------- Insurance Buttons --------------------- ")]
        [SerializeField]
        private Button yesButton;
        [SerializeField]
        private Button noButton;
        [SerializeField]
        private Text insuranceAmount;

        internal void UserInsurance()
        {
            try
            {


                noButton.interactable = true;
                insuranceAmount.text = BlackJackGameManager.instance.SetBalanceFormat(lastBet / 2);
                for (int i = 0; i < allBetsButton.Count; i++)
                {
                    allBetsButton[i].SetActive(false);
                    if (i >= 8)
                    {
                        allBetsButton[i].SetActive(true);
                    }
                }
                MoveSide(topPostion);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.ToString()}");
            }
        }

        private void NoInsuranceClicked()
        {
            noButton.interactable = false;
            yesButton.interactable = false;
            MoveSide(downPosition);
            player.StopTimer();
            BlackJackGameManager.instance.dealer.totalInsuranceBet++;
        }

        private void YesInsuranceClicked()
        {
            if (!player.isCraditAvalible(lastBet / 2))
            {
                BlackJackGameManager.instance.popupManager.SetToastAlertPopup("Insufficient Balance");
                return;
            }
            noButton.interactable = false;
            yesButton.interactable = false;
            MoveSide(downPosition);
            float insuranceAmount = lastBet / 2;
            player.AddInsurance(insuranceAmount);
        }

        internal void InsuranceTimerOver()
        {
            MoveSide(downPosition);
            BlackJackGameManager.instance.dealer.totalInsuranceBet++;
        }

        #endregion

        #region Button Bar Animation

        [Header("--------------------- Button Bar Animation --------------------- ")]
        [SerializeField]
        private GameObject buttonsBar;
        [SerializeField]
        private Transform topPostion, downPosition;

        void MoveSide(Transform endPoint)
        {
            buttonsBar.transform.DOMove(endPoint.position, 0.5f).SetEase(Ease.InFlash);
        }

        #endregion

    }
}