using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackjackProduct : MonoBehaviour
    {
        [SerializeField]
        private Image Icon;
        [SerializeField]
        private Text PriceText, discriptionText;
        [SerializeField]
        private Button PurchaseButton;

        public delegate void PurchaseEvent(Product Model, Action OnComplete);
        public event PurchaseEvent OnPurchase;

        private Product product;

        public void Setup(Product Product)
        {
            product = Product;
            PriceText.text = $"{Product.metadata.localizedPriceString} ";

            Debug.Log($"Unity------------> {Product.metadata.localizedPriceString}");
            Debug.Log($"Unity------------> {Product.metadata.localizedDescription}");
            // +$"{Product.metadata.isoCurrencyCode}");
            discriptionText.text = $"{Product.metadata.localizedDescription}";
            //Texture2D texture = null;//= BlackjackStoreIconProvider.GetIcon(Product.definition.id);
            //if (texture != null)
            //{
            //    Sprite sprite = Sprite.Create(texture,
            //        new Rect(0, 0, texture.width, texture.height),
            //        Vector2.one / 2f
            //    );
            //    Icon.sprite = sprite;
            //}
            //else
            //{
            //    Debug.LogError($"No Sprite found for {Product.definition.id}!");
            //}
        }

        public void Purchase()
        {
            BlackjackInAppPurchasing.Instance.GoingToPurchase(product);
            //PurchaseButton.enabled = false;
            //OnPurchase?.Invoke(Model, HandlePurchaseComplete);
        }

        public void OnButtonClicked()
        {
          
        }

        private void HandlePurchaseComplete()
        {
            PurchaseButton.enabled = true;
        }
    }
}