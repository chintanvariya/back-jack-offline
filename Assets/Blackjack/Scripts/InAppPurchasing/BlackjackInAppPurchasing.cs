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
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackjackInAppPurchasing : MonoBehaviour, IStoreListener
    {
        [SerializeField]
        private BlackjackProduct UIProductPrefab;
        [SerializeField]
        private Transform ContenPanel;

        private Action OnPurchaseCompleted;
        private IStoreController StoreController;
        private IExtensionProvider ExtensionProvider;

        private void Start()
        {
            Inst();
        }

        internal async void Inst()
        {
            InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);
            ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
            operation.completed += HandleIAPCatalogLoaded;
        }

        private void HandleIAPCatalogLoaded(AsyncOperation Operation)
        {
            ResourceRequest request = Operation as ResourceRequest;

            Debug.Log($"Loaded Asset: {request.asset}");
            ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
            Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;

#if UNITY_ANDROID
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                StandardPurchasingModule.Instance(AppStore.GooglePlay)
            );
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified)
        );
#endif
            foreach (ProductCatalogItem item in catalog.allProducts)
            {
                builder.AddProduct(item.id, item.type);
            }

            Debug.Log($"Initializing Unity IAP with {builder.products.Count} products");
            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            StoreController = controller;
            ExtensionProvider = extensions;
            Debug.Log($"Successfully Initialized Unity IAP. Store Controller has {StoreController.products.all.Length} products");
            BlackjackStoreIconProvider.Initialize(StoreController.products);
            BlackjackStoreIconProvider.OnLoadComplete += HandleAllIconsLoaded;

        }

        private void HandleAllIconsLoaded()
        {
            StartCoroutine(CreateUI());
        }

        internal List<BlackjackProduct> GetProducts = new List<BlackjackProduct>();

        private IEnumerator CreateUI()
        {
            List<Product> sortedProducts = StoreController.products.all.ToList();

            foreach (Product product in sortedProducts)
            {
                BlackjackProduct uiProduct = Instantiate(UIProductPrefab);
                uiProduct.OnPurchase += HandlePurchase;
                uiProduct.Setup(product);
                uiProduct.transform.SetParent(ContenPanel, false);
                GetProducts.Add(uiProduct);
                yield return null;
            }
        }

        private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
        {
            this.OnPurchaseCompleted = OnPurchaseCompleted;
            StoreController.InitiatePurchase(Product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null; 
            BlackJackGameManager.instance.popupManager.SetAlertPopup("PurchaseFailed");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null;

            int Count = 0;
            foreach (var item in productIds)
            {
                if (item == purchaseEvent.purchasedProduct.definition.id)
                {
                    BlackJackDataManager.creditPoint += productAmount[Count];
                    BlackJackGameManager.instance.UpdateUserInfo();
                }
                Count++;
            }
            BlackJackGameManager.instance.popupManager.SetAlertPopup("PurchaseSuccess");

            return PurchaseProcessingResult.Complete;
        }

        [SerializeField]
        private List<string> productIds;
        [SerializeField]
        private List<float> productAmount;

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Error initializing IAP because of {error}." +
                 $"\r\nShow a message to the player depending on the error.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Error initializing IAP because of {error}." +
                 $"\r\nShow a message to the player depending on the error.");
        }
    }
}