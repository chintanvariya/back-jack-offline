using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace BlackJackOffline
{
    public class BlackJackPooler : MonoBehaviour
    {
        #region UNITY
        void Start()
        {
            //Cards
            cardsPoolsDictionary = new Dictionary<string, Queue<BlackJackCard>>();

            Queue<BlackJackCard> Pooler = new Queue<BlackJackCard>();

            for (int i = 0; i < Pools.size; i++)
            {
                BlackJackCard obj = Instantiate(Pools.Card, Pools.position);
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.SetParent(Pools.position);
                obj.gameObject.SetActive(false);
                Pools.cardList.Add(obj);
                Pooler.Enqueue(obj);
            }
            cardsPoolsDictionary.Add(Pools.tag, Pooler);

            //Cards
            EmptyCardsPoolsDictionary = new Dictionary<string, Queue<BlackJackEmptyCard>>();

            Queue<BlackJackEmptyCard> EmptyPooler = new Queue<BlackJackEmptyCard>();

            for (int i = 0; i < emptyCardPooler.size; i++)
            {
                BlackJackEmptyCard obj = Instantiate(emptyCardPooler.Card, emptyCardPooler.position);
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.SetParent(emptyCardPooler.position);
                obj.gameObject.SetActive(false);
                emptyCardPooler.emptyCardList.Add(obj);
                EmptyPooler.Enqueue(obj);
            }
            EmptyCardsPoolsDictionary.Add(emptyCardPooler.tag, EmptyPooler);

            // Place Bet Coins
            placebetCoinPoolsDictionary = new Dictionary<string, Queue<BlackJackPlaceBetCoin>>();

            Queue<BlackJackPlaceBetCoin> CoinPooler = new Queue<BlackJackPlaceBetCoin>();

            for (int i = 0; i < Pools.size; i++)
            {
                BlackJackPlaceBetCoin obj = Instantiate(placeBetCoins.coin, placeBetCoins.position);
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.SetParent(Pools.position);
                obj.gameObject.SetActive(false);
                placeBetCoins.coinList.Add(obj);
                CoinPooler.Enqueue(obj);
            }
            placebetCoinPoolsDictionary.Add(placeBetCoins.tag, CoinPooler);
        }
        #endregion

        #region Cards Pooler
        [Serializable]
        public class Pool
        {
            public string tag;
            public BlackJackCard Card;
            public int size;
            public Transform position;
            public List<BlackJackCard> cardList;
        }

        [Header("-------------- Card Pools ---------------")]
        public Pool Pools;
        public Dictionary<string, Queue<BlackJackCard>> cardsPoolsDictionary;

        public BlackJackCard SpawnFromCards(string tag, Transform Parent)
        {
            if (!cardsPoolsDictionary.ContainsKey(tag))
            {
                Debug.Log("Can't spawn " + tag + " don't excist.");
                return null;
            }

            BlackJackCard temp = cardsPoolsDictionary[tag].Dequeue();

            temp.transform.SetParent(Parent);
            temp.resetParent = Pools.position;
            temp.transform.rotation = Quaternion.identity;
            temp.transform.localScale = new Vector3(0, 0, 0);
            temp.gameObject.SetActive(true);

            cardsPoolsDictionary[tag].Enqueue(temp);
            return temp;
        }

        #endregion

        #region Empty Card Pooler
        [Serializable]
        public class EmptyCardPooler
        {
            public string tag;
            public BlackJackEmptyCard Card;
            public int size;
            public Transform position;
            public List<BlackJackEmptyCard> emptyCardList;
        }

        [Header("-------------- Empty Card Pooler ---------------")]
        public EmptyCardPooler emptyCardPooler;
        public Dictionary<string, Queue<BlackJackEmptyCard>> EmptyCardsPoolsDictionary;

        public BlackJackEmptyCard SpawnFromEmptyCards(string tag, Transform Parent)
        {
            if (!EmptyCardsPoolsDictionary.ContainsKey(tag))
            {
                Debug.Log("Can't spawn " + tag + " don't excist.");
                return null;
            }

            BlackJackEmptyCard temp = EmptyCardsPoolsDictionary[tag].Dequeue();

            temp.transform.SetParent(Parent);
            temp.transform.localPosition = new Vector3(0,0,0);
            temp.resetParent = Pools.position;
            temp.emptyImage.sprite = temp.emptySprite;
            temp.transform.rotation = Quaternion.identity;
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.gameObject.SetActive(true);

            EmptyCardsPoolsDictionary[tag].Enqueue(temp);
            return temp;
        }

        #endregion

        #region Place Bet Coins Pooler
        [Serializable]
        public class PlaceBetCoins
        {
            public string tag;
            public BlackJackPlaceBetCoin coin;
            public int size;
            public Transform position;
            public List<BlackJackPlaceBetCoin> coinList;
        }

        [Header("-------------- Place Bet Coin Pools ---------------")]
        public PlaceBetCoins placeBetCoins;
        public Dictionary<string, Queue<BlackJackPlaceBetCoin>> placebetCoinPoolsDictionary;

        public BlackJackPlaceBetCoin SpawnCoinFromPlayer(string tag, Transform Parent , float Amount)
        {
            if (!placebetCoinPoolsDictionary.ContainsKey(tag))
            {
                Debug.Log("Can't spawn " + tag + " don't excist.");
                return null;
            }

            BlackJackPlaceBetCoin temp = placebetCoinPoolsDictionary[tag].Dequeue();

            temp.transform.position = Parent.transform.position;
            temp.transform.rotation = Quaternion.identity;
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.gameObject.SetActive(true);

            temp.SetPlaceBetAmount(Amount);

            placebetCoinPoolsDictionary[tag].Enqueue(temp);
            return temp;
        }

        #endregion

        #region Reset All Pooler Objects

        internal void ResetAtNewRoundStart()
        {
            foreach (var item in Pools.cardList)
            {
                item.transform.SetParent(Pools.position);
                item.gameObject.SetActive(false);
            }
            foreach (var item in emptyCardPooler.emptyCardList)
            {
                item.transform.SetParent(emptyCardPooler.position);
                item.gameObject.SetActive(false);
            }
            foreach (var item in placeBetCoins.coinList)
            {
                item.transform.SetParent(Pools.position);
                item.gameObject.SetActive(false);
            }
        }


        #endregion
    }
}
