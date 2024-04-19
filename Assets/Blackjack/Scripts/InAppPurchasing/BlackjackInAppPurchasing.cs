using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackjackInAppPurchasing : MonoBehaviour, IStoreListener
    {
        [SerializeField]
        private BlackjackProduct UIProductPrefab;
        [SerializeField]
        private Transform ContenPanel;
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        public Product product1;
        public Product product2;
        public Product product3;
        public Product product4;
        public Product product5;

        public Product removeAds;

        //public Text coinText;
        //public GameObject shop, outerObject, InternetDialog, loader;


        //public int[] P1Coins;
        //		public int help;
        //		public Map map;
        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        //public static string kProductID300Consumable = "com.coin300";
        //public static string kProductID800Consumable = "com.coin800";
        //public static string kProductID1200Consumable = "com.coin1200";
        //public static string kProductID1500Consumable = "com.coin1500";
        //public static string kProductIDUnlockConsumable = "com.unlock";
        //public static string kProductIDRemoveAdNonConsumable = "com.removeadvertisement";
        //		T_Money tMoney;

        public static string coin1k = "coinpack01";
        public static string coin2k = "coinpack02";
        public static string coin3k = "coinpack03";
        public static string coin4k = "coinpack04";
        public static string coin5k = "coinpack05";
        public static string noAds = "removeads";
        //public static string coin10k = "callbreakcoinpack5";

        private string environment = "production";

        public static BlackjackInAppPurchasing Instance;

        async void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);

                if (m_StoreController == null)
                {
                    // Begin to configure our connection to Purchasing
                    InitializePurchasing();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                // An error occurred during initialization.
            }


            // If we haven't set up the Unity Purchasing reference

        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            //builder.AddProduct(kProductID300Consumable, ProductType.Consumable);
            //builder.AddProduct(kProductID800Consumable, ProductType.Consumable);
            //builder.AddProduct(kProductID1200Consumable, ProductType.Consumable);
            //builder.AddProduct(kProductID1500Consumable, ProductType.Consumable);
            //builder.AddProduct(kProductIDUnlockConsumable, ProductType.NonConsumable);
            //builder.AddProduct(kProductIDRemoveAdNonConsumable, ProductType.NonConsumable);

            Debug.Log(" ==== IAP INIT ====");
            builder.AddProduct(coin1k, ProductType.Consumable);
            builder.AddProduct(coin2k, ProductType.Consumable);
            builder.AddProduct(coin3k, ProductType.Consumable);
            builder.AddProduct(coin4k, ProductType.Consumable);
            builder.AddProduct(coin5k, ProductType.Consumable);
            builder.AddProduct(noAds, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
            Debug.Log(" ==== IAP INIT ====");
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.         
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void CheckPurchase(Product pro)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                m_StoreController.InitiatePurchase(pro);
            }
            else
            {
                //UIManager.Instance.OnePanelOn(GameScreen.Internet);
            }

            /* WWW www = new WWW("http://www.google.com");// http://google.com      
             yield return www;
             if (www.error != null)
             {
                 // GameUIManager.isIAPPopup = true;
                 //  GameUIManager.isInternetPopup = true;
                 // Debug.Log(GameUIManager.isIAPPopup);
                 //GameUIManager.panelAction("internetPopup");
                 //outerObject.SetActive(true);
                 //shop.SetActive(false);
                 //loader.SetActive(false);
             }
             else
             {

                 m_StoreController.InitiatePurchase(pro);
                 //loader.SetActive(false);
             }*/
        }


        public void BuyConsumable_1KCoins() //  Product 1  Coin Add
        {
            CheckPurchase(product1);
        }

        public void BuyConsumable_3KCoins() // Product 2 Unlock All Balls
        {
            CheckPurchase(product2);
        }

        public void BuyConsumable_5KCoins() // Product 3 Unlock All Gamemodes
        {
            CheckPurchase(product3);
        }

        public void BuyConsumable_7KCoins() // Product 3 Unlock All Gamemodes
        {
            CheckPurchase(product4);
        }
        public void BuyConsumable_10KCoins() // Product 3 Unlock All Gamemodes
        {
            CheckPurchase(product5);
        }
        public void BuyNonConsumable_NoAds()// Product 4 No Ads
        {
            CheckPurchase(removeAds);
        }

        void BuyProductID()
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // system's products collection.
                product1 = m_StoreController.products.WithID(coin1k);
                product2 = m_StoreController.products.WithID(coin2k);
                product3 = m_StoreController.products.WithID(coin3k);
                product4 = m_StoreController.products.WithID(coin4k);
                product5 = m_StoreController.products.WithID(coin5k);
                removeAds = m_StoreController.products.WithID(noAds);

                //product4 = m_StoreController.products.WithID(kProductIDUnlockConsumable);
                //removeAds = m_StoreController.products.WithID(kProductIDRemoveAdNonConsumable);
                if (product1 != null && product1.availableToPurchase)
                {
                    PlayerPrefs.SetString("price1", product1.metadata.localizedPriceString);
                    PlayerPrefs.SetString("descriptioncoinpack1", product1.metadata.localizedDescription);
                    BlackjackProduct uiProduct = Instantiate(UIProductPrefab);
                    uiProduct.transform.SetParent(ContenPanel, false);
                    uiProduct.Setup(product1);
                }
                if (product2 != null && product2.availableToPurchase)
                {
                    PlayerPrefs.SetString("price2", product2.metadata.localizedPriceString);
                    PlayerPrefs.SetString("descriptioncoinpack2", product2.metadata.localizedDescription);
                    BlackjackProduct uiProduct = Instantiate(UIProductPrefab);
                    uiProduct.transform.SetParent(ContenPanel, false);
                    uiProduct.Setup(product2);
                }
                if (product3 != null && product3.availableToPurchase)
                {
                    PlayerPrefs.SetString("price3", product3.metadata.localizedPriceString);
                    PlayerPrefs.SetString("descriptioncoinpack3", product3.metadata.localizedDescription);
                    BlackjackProduct uiProduct = Instantiate(UIProductPrefab);
                    uiProduct.transform.SetParent(ContenPanel, false);
                    uiProduct.Setup(product3);
                }
                if (product4 != null && product4.availableToPurchase)
                {
                    PlayerPrefs.SetString("price4", product4.metadata.localizedPriceString);
                    PlayerPrefs.SetString("descriptioncoinpack4", product4.metadata.localizedDescription);
                    BlackjackProduct uiProduct = Instantiate(UIProductPrefab);
                    uiProduct.transform.SetParent(ContenPanel, false);
                    uiProduct.Setup(product4);
                }
                if (product5 != null && product5.availableToPurchase)
                {
                    PlayerPrefs.SetString("price5", product5.metadata.localizedPriceString);
                    PlayerPrefs.SetString("descriptioncoinpack5", product5.metadata.localizedDescription);
                    BlackjackProduct uiProduct = Instantiate(UIProductPrefab);
                    uiProduct.transform.SetParent(ContenPanel, false);
                    uiProduct.Setup(product5);
                }
                if (removeAds != null && removeAds.availableToPurchase)
                {
                    PlayerPrefs.SetString("price6", removeAds.metadata.localizedPriceString);
                }
                // Otherwise ...
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            //			Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

            BuyProductID();
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error.ToString());
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {

            // A consumable product has been purchased by this user.
            if (String.Equals(args.purchasedProduct.definition.id, coin1k, StringComparison.Ordinal))
            {

                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins") + P1Coins[0]);

                BlackJackDataManager.creditPoint += 100;
            }
            else if (String.Equals(args.purchasedProduct.definition.id, coin2k, StringComparison.Ordinal))
            {

                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins") + P1Coins[1]);
                //ShopHandler.unlockAllShopBalls();
                BlackJackDataManager.creditPoint += 300;

            }
            else if (String.Equals(args.purchasedProduct.definition.id, coin3k, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // GameModesData.unlockAllGamemodes();
                BlackJackDataManager.creditPoint += 500;

            }
            else if (String.Equals(args.purchasedProduct.definition.id, coin4k, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // GameModesData.unlockAllGamemodes();
                BlackJackDataManager.creditPoint += 1000;

            }
            else if (String.Equals(args.purchasedProduct.definition.id, coin5k, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // GameModesData.unlockAllGamemodes();
                BlackJackDataManager.creditPoint += 5000;

            }
            else if (String.Equals(args.purchasedProduct.definition.id, noAds, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //StaticData.RemoveAds = 1;
            }

            //coinText.text = PlayerPrefs.GetInt("TotalCoins").ToString();

            BlackJackGameManager.instance.UpdateUserInfo();
            BlackJackGameManager.instance.popupManager.SetAlertPopup("PurchaseSuccess");
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log($"OnInitializeFailed InitializationFailureReason: 212121212 { error.ToString()} || {message }");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"OnInitializeFailed InitializationFailureReason: 2121212121 " + failureDescription.ToString());

        }
    }
}